using ThemePark.Entities.Tickets;
using ThemePark.Exceptions;
using ThemePark.IBusinessLogic;
using ThemePark.IBusinessLogic.Exceptions;
using ThemePark.IDataAccess;

namespace ThemePark.BusinessLogic;

public class TicketsBusinessLogic(ITicketsRepository ticketsRepository, IUserRepository userRepository, IEventRepository eventRepository, ISessionRepository sessionRepository) : ITicketsBusinessLogic
{
    private readonly ITicketsRepository _ticketsRepository = ticketsRepository;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IEventRepository _eventRepository = eventRepository;
    private readonly ISessionRepository _sessionRepository = sessionRepository;

    public Ticket CreateTicket(Ticket ticket)
    {
        if(ticket == null)
        {
            throw new InvalidTicketException("El ticket no puede ser nulo");
        }

        if(!_userRepository.ExistsByCodigoIdentificacion(ticket.CodigoIdentificacionUsuario))
        {
            throw BusinessLogicException.UserNotExists();
        }

        if(ticket is EventTicket eventTicket)
        {
            if(!_eventRepository.Exists(eventTicket.EventoId))
            {
                throw BusinessLogicException.EventNotExists();
            }

            var evento = _eventRepository.GetById(eventTicket.EventoId);
            var ticketsVendidos = _ticketsRepository.CountTicketsByEventId(eventTicket.EventoId);

            if(ticketsVendidos >= evento.Aforo)
            {
                throw new EventCapacityExceededException(evento.Name, evento.Aforo);
            }
        }

        return _ticketsRepository.Create(ticket);
    }

    public Ticket GetTicketById(Guid id)
    {
        return _ticketsRepository.GetById(id);
    }

    public List<Ticket> GetAllTickets()
    {
        return _ticketsRepository.GetAll();
    }

    public List<Ticket> GetMyTickets()
    {
        var userId = _sessionRepository.GetCurrentUserId();
        var user = _userRepository.GetById(userId);
        return _ticketsRepository.GetByCodigoIdentificacionUsuario(user.CodigoIdentificacion);
    }
}
