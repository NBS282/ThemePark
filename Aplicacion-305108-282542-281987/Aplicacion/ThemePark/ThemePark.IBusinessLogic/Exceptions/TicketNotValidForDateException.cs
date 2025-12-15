using ThemePark.Exceptions;

namespace ThemePark.IBusinessLogic.Exceptions;

public sealed class TicketNotValidForDateException(string ticketId, DateTime validDate, DateTime currentDate) : BaseCustomException($"El ticket '{ticketId}' no es válido para la fecha {currentDate:dd/MM/yyyy}. Fecha válida: {validDate:dd/MM/yyyy}",
           $"Ticket '{ticketId}' is not valid for date {currentDate:dd/MM/yyyy}. Valid date: {validDate:dd/MM/yyyy}")
{
}
