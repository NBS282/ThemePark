using ThemePark.Exceptions;

namespace ThemePark.IDataAccess.Exceptions;

public sealed class EventNotFoundException(Guid eventId) : BaseCustomException($"Evento con ID {eventId} no encontrado",
           $"Event with ID {eventId} not found")
{
}
