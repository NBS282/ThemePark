namespace ThemePark.Exceptions;

public sealed class InvalidTicketException(string reason) : BaseCustomException($"El ticket no es válido: {reason}",
           $"Ticket is invalid: {reason}")
{
}
