using ThemePark.Exceptions;

namespace ThemePark.IBusinessLogic.Exceptions;

public sealed class EventTicketNotValidForTimeException(string eventName, DateTime eventStart, DateTime eventEnd, DateTime currentTime)
    : BaseCustomException(
        $"El ticket para el evento '{eventName}' solo es válido entre {eventStart:dd/MM/yyyy HH:mm} y {eventEnd:dd/MM/yyyy HH:mm}. Hora actual: {currentTime:dd/MM/yyyy HH:mm}",
        $"Ticket for event '{eventName}' is only valid between {eventStart:dd/MM/yyyy HH:mm} and {eventEnd:dd/MM/yyyy HH:mm}. Current time: {currentTime:dd/MM/yyyy HH:mm}")
{
}
