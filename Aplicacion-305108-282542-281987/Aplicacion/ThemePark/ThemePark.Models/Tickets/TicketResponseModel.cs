using ThemePark.Entities.Tickets;

namespace ThemePark.Models.Tickets;

public class TicketResponseModel
{
    public Guid CodigoQR { get; set; }
    public string FechaVisita { get; set; } = string.Empty;
    public string TipoEntrada { get; set; } = string.Empty;
    public string CodigoIdentificacionUsuario { get; set; } = string.Empty;
    public string FechaCompra { get; set; } = string.Empty;
    public string? NombreEvento { get; set; }

    public static TicketResponseModel FromEntity(Ticket ticket)
    {
        var tipoEntrada = ticket switch
        {
            GeneralTicket => "general",
            EventTicket => "evento",
            _ => "general"
        };

        return new TicketResponseModel
        {
            CodigoQR = ticket.CodigoQR,
            FechaVisita = ticket.FechaVisita.ToString("yyyy-MM-dd"),
            TipoEntrada = tipoEntrada,
            CodigoIdentificacionUsuario = ticket.CodigoIdentificacionUsuario,
            FechaCompra = ticket.FechaCompra.ToString("yyyy-MM-ddTHH:mm:ss"),
            NombreEvento = null
        };
    }
}
