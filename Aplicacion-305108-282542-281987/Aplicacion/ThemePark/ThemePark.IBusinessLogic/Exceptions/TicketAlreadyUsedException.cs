using ThemePark.Exceptions;

namespace ThemePark.IBusinessLogic.Exceptions;

public sealed class TicketAlreadyUsedException(string ticketId, string attractionName) : BaseCustomException($"El ticket '{ticketId}' ya fue utilizado para acceder a la atracción '{attractionName}'",
           $"Ticket '{ticketId}' has already been used to access attraction '{attractionName}'")
{
}
