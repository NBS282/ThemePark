using ThemePark.Entities;

namespace ThemePark.IBusinessLogic;

public interface IEventsService
{
    Event CreateEvent(string nombre, System.DateTime fecha, System.TimeSpan hora, int aforo, decimal costoAdicional, List<string> atraccionesIncluidas);
    Event UpdateEvent(string id, string nombre, System.DateTime fecha, System.TimeSpan hora, int aforo, decimal costoAdicional, List<string> atraccionesIncluidas);
    void DeleteEvent(string id);
    List<Event> GetAllEvents();
    Event GetEventById(string id);
}
