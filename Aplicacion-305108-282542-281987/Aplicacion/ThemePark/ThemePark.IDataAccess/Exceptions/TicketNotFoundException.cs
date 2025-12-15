using ThemePark.Exceptions;

namespace ThemePark.IDataAccess.Exceptions;

public sealed class TicketNotFoundException : BaseCustomException
{
    public TicketNotFoundException(Guid ticketId)
        : base($"Ticket con ID {ticketId} no encontrado",
               $"Ticket with ID {ticketId} not found")
    {
    }

    public TicketNotFoundException(string qrCode)
        : base($"Ticket con código QR {qrCode} no encontrado",
               $"Ticket with QR code {qrCode} not found")
    {
    }
}
