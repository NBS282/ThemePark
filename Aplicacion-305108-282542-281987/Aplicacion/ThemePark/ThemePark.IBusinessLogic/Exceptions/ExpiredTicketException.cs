using ThemePark.Exceptions;

namespace ThemePark.IBusinessLogic.Exceptions;

public sealed class ExpiredTicketException(string ticketId, DateTime expirationDate) : BaseCustomException($"El ticket '{ticketId}' ha expirado. Fecha de vencimiento: {expirationDate:dd/MM/yyyy}",
           $"Ticket '{ticketId}' has expired. Expiration date: {expirationDate:dd/MM/yyyy}")
{
}
