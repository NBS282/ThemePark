using ThemePark.Entities;
using ThemePark.Entities.Tickets;
using ThemePark.IBusinessLogic;
using ThemePark.IBusinessLogic.Exceptions;
using ThemePark.IDataAccess;
using ThemePark.IDataAccess.Exceptions;

namespace ThemePark.BusinessLogic.Attractions;

public class AccessControlService(
    IAttractionRepository attractionRepository,
    IDateTimeRepository dateTimeRepository,
    IUserRepository userRepository,
    IVisitRepository visitRepository,
    ITicketsRepository ticketsRepository,
    IEventRepository eventRepository,
    IScoringStrategyService scoringStrategyService,
    IScoringAlgorithmFactory algorithmFactory) : IAccessControlService
{
    private readonly IAttractionRepository _attractionRepository = attractionRepository;
    private readonly IDateTimeRepository _dateTimeRepository = dateTimeRepository;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IVisitRepository _visitRepository = visitRepository;
    private readonly ITicketsRepository _ticketsRepository = ticketsRepository;
    private readonly IEventRepository _eventRepository = eventRepository;
    private readonly IScoringStrategyService _scoringStrategyService = scoringStrategyService;
    private readonly IScoringAlgorithmFactory _algorithmFactory = algorithmFactory;

    public Visit ValidateTicketAndRegisterAccess(string nombre, string tipoEntrada, string codigo)
    {
        if(!_attractionRepository.ExistsByName(nombre))
        {
            throw new AttractionNotFoundException(nombre);
        }

        var attraction = _attractionRepository.GetByName(nombre);

        if(attraction.TieneIncidenciaActiva)
        {
            throw BusinessLogicException.AttractionOutOfService();
        }

        if(attraction.AforoActual >= attraction.CapacidadMaxima)
        {
            throw BusinessLogicException.AttractionCapacityReached();
        }

        User user;
        Ticket ticket;

        if(tipoEntrada.Equals("QR", StringComparison.OrdinalIgnoreCase))
        {
            if(!Guid.TryParse(codigo, out var qrCode))
            {
                throw BusinessLogicException.InvalidQRCode();
            }

            ticket = _ticketsRepository.GetByQRCode(qrCode);
            user = _userRepository.GetByCodigoIdentificacion(ticket.CodigoIdentificacionUsuario);

            if(ticket is EventTicket eventTicket)
            {
                ValidateEventTicketForAttraction(eventTicket, nombre);
            }
        }
        else if(tipoEntrada.Equals("NFC", StringComparison.OrdinalIgnoreCase))
        {
            user = _userRepository.GetByCodigoIdentificacion(codigo);
            ticket = GetValidTicketForAttraction(codigo, nombre);
        }
        else
        {
            throw BusinessLogicException.InvalidEntryType();
        }

        var entryTime = _dateTimeRepository.GetCurrentDateTime() ?? DateTime.Now;
        var edad = CalculateAge(user.FechaNacimiento, entryTime);
        if(edad < attraction.EdadMinima)
        {
            throw new AgeLimitException(attraction.Nombre, attraction.EdadMinima, edad);
        }

        if(ticket.FechaVisita.Date < entryTime.Date)
        {
            throw new ExpiredTicketException(ticket.CodigoQR.ToString(), ticket.FechaVisita);
        }

        if(ticket.FechaVisita.Date != entryTime.Date)
        {
            throw new TicketNotValidForDateException(ticket.CodigoQR.ToString(), ticket.FechaVisita, entryTime);
        }

        var existingVisit = _visitRepository.GetActiveVisitByUserAndAttraction(user.Id, nombre);
        if(existingVisit != null)
        {
            throw new VisitAlreadyActiveException(attraction.Nombre);
        }

        var activeVisitInAnyAttraction = _visitRepository.GetActiveVisitByUser(user.Id);
        if(activeVisitInAnyAttraction != null)
        {
            throw new UserAlreadyInAttractionException(activeVisitInAnyAttraction.Attraction.Nombre);
        }

        var points = 0;
        string? strategyName = null;
        var activeStrategy = _scoringStrategyService.GetActiveStrategy();

        if(activeStrategy != null && activeStrategy.ConfiguracionTyped != null)
        {
            var algorithm = _algorithmFactory.CreateAlgorithm(activeStrategy);
            var userVisitsToday = _visitRepository.GetVisitsByUserAndDate(user.Id, entryTime.Date);
            var tempVisit = new Visit(user.Id, attraction, entryTime);
            points = algorithm.CalculatePoints(tempVisit, activeStrategy.ConfiguracionTyped, userVisitsToday);
            strategyName = activeStrategy.Name;
        }

        var visit = new Visit(user.Id, attraction, entryTime, points, strategyName);

        _visitRepository.Save(visit);
        attraction.IncrementarAforo();
        _attractionRepository.Save(attraction);

        return visit;
    }

    public Visit RegisterExit(string nombre, string tipoEntrada, string codigo)
    {
        if(!_attractionRepository.ExistsByName(nombre))
        {
            throw new AttractionNotFoundException(nombre);
        }

        var attraction = _attractionRepository.GetByName(nombre);

        User user;

        if(tipoEntrada.Equals("QR", StringComparison.OrdinalIgnoreCase))
        {
            if(!Guid.TryParse(codigo, out var qrCode))
            {
                throw BusinessLogicException.InvalidQRCode();
            }

            var ticket = _ticketsRepository.GetByQRCode(qrCode);
            user = _userRepository.GetByCodigoIdentificacion(ticket.CodigoIdentificacionUsuario);
        }
        else if(tipoEntrada.Equals("NFC", StringComparison.OrdinalIgnoreCase))
        {
            user = _userRepository.GetByCodigoIdentificacion(codigo);
        }
        else
        {
            throw BusinessLogicException.InvalidEntryType();
        }

        var activeVisit = _visitRepository.GetActiveVisitByUserAndAttraction(user.Id, nombre);
        if(activeVisit == null)
        {
            throw new NoActiveVisitException(attraction.Nombre, user.CodigoIdentificacion);
        }

        var exitTime = _dateTimeRepository.GetCurrentDateTime() ?? DateTime.Now;
        activeVisit.MarkExit(exitTime);

        _visitRepository.Update(activeVisit);
        attraction.DecrementarAforo();
        _attractionRepository.Save(attraction);

        return activeVisit;
    }

    private static int CalculateAge(DateTime birthDate, DateTime currentDate)
    {
        var age = currentDate.Year - birthDate.Year;
        if(currentDate.Date < birthDate.AddYears(age))
        {
            age--;
        }

        return age;
    }

    private Ticket GetValidTicketForAttraction(string codigoIdentificacionUsuario, string attractionName)
    {
        var userTickets = _ticketsRepository.GetByCodigoIdentificacionUsuario(codigoIdentificacionUsuario);
        var today = (_dateTimeRepository.GetCurrentDateTime() ?? DateTime.Now).Date;

        var validTickets = userTickets.Where(t => t.FechaVisita.Date == today).ToList();

        if(!validTickets.Any())
        {
            throw BusinessLogicException.NoValidTickets();
        }

        foreach(var ticket in validTickets)
        {
            if(ticket is GeneralTicket)
            {
                return ticket;
            }
            else if(ticket is EventTicket eventTicket)
            {
                try
                {
                    ValidateEventTicketForAttraction(eventTicket, attractionName);
                    return ticket;
                }
                catch(WrongAttractionException)
                {
                    continue;
                }
                catch(EventTicketNotValidForTimeException)
                {
                    continue;
                }
            }
        }

        throw new WrongAttractionException("la atracción solicitada");
    }

    private void ValidateEventTicketForAttraction(EventTicket eventTicket, string attractionName)
    {
        var evento = _eventRepository.GetById(eventTicket.EventoId);

        if(!evento.Atracciones.Any(a => a.Nombre.Equals(attractionName, StringComparison.OrdinalIgnoreCase)))
        {
            throw new WrongAttractionException(attractionName);
        }

        var currentDateTime = _dateTimeRepository.GetCurrentDateTime() ?? DateTime.Now;
        var eventStartDateTime = evento.Fecha.Date.Add(evento.Hora);
        var eventEndDateTime = eventStartDateTime.AddHours(4);

        if(currentDateTime < eventStartDateTime || currentDateTime > eventEndDateTime)
        {
            throw new EventTicketNotValidForTimeException(evento.Name, eventStartDateTime, eventEndDateTime, currentDateTime);
        }
    }
}
