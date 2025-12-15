using ThemePark.Entities;
using ThemePark.IBusinessLogic;
using ThemePark.IBusinessLogic.Exceptions;
using ThemePark.IDataAccess;

namespace ThemePark.BusinessLogic;

public class EventService(IEventRepository repository, IAttractionRepository attractionRepository) : IEventsService
{
    public Event CreateEvent(string nombre, DateTime fecha, TimeSpan hora, int aforo, decimal costoAdicional, List<string> atraccionesIncluidas)
    {
        if(repository.ExistsByName(nombre))
        {
            throw BusinessLogicException.EventAlreadyExists(nombre);
        }

        var atraccionesNoExistentes = atraccionesIncluidas
            .Where(nombreAtraccion => !attractionRepository.ExistsByName(nombreAtraccion))
            .ToList();

        if(atraccionesNoExistentes.Count != 0)
        {
            throw BusinessLogicException.AttractionsNotFound(atraccionesNoExistentes);
        }

        var newEvent = new Event
        {
            Id = Guid.NewGuid(),
            Name = nombre,
            Fecha = fecha,
            Hora = hora,
            Aforo = aforo,
            CostoAdicional = costoAdicional,
            Atracciones = atraccionesIncluidas
                .Select(attractionRepository.GetByName)
                .ToList()
        };

        return repository.Add(newEvent);
    }

    public Event UpdateEvent(string id, string nombre, DateTime fecha, TimeSpan hora, int aforo, decimal costoAdicional, List<string> atraccionesIncluidas)
    {
        var eventId = Guid.Parse(id);
        var existingEvent = repository.GetById(eventId);

        if(repository.ExistsByName(nombre) && existingEvent.Name != nombre)
        {
            throw BusinessLogicException.EventAlreadyExists(nombre);
        }

        var atraccionesNoExistentes = atraccionesIncluidas
            .Where(nombreAtraccion => !attractionRepository.ExistsByName(nombreAtraccion))
            .ToList();

        if(atraccionesNoExistentes.Count != 0)
        {
            throw BusinessLogicException.AttractionsNotFound(atraccionesNoExistentes);
        }

        existingEvent.Name = nombre;
        existingEvent.Fecha = fecha;
        existingEvent.Hora = hora;
        existingEvent.Aforo = aforo;
        existingEvent.CostoAdicional = costoAdicional;
        existingEvent.Atracciones = atraccionesIncluidas
            .Select(attractionRepository.GetByName)
            .ToList();

        return repository.Update(existingEvent);
    }

    public void DeleteEvent(string id)
    {
        var eventId = Guid.Parse(id);

        repository.GetById(eventId);

        repository.Delete(eventId);
    }

    public List<Event> GetAllEvents()
    {
        var events = repository.GetAll();
        return events;
    }

    public Event GetEventById(string id)
    {
        var eventId = Guid.Parse(id);
        return repository.GetById(eventId);
    }
}
