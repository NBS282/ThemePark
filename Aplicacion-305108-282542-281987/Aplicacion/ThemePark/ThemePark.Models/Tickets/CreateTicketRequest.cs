using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using ThemePark.Entities.Tickets;
using ThemePark.Exceptions;
using static ThemePark.DateParser;

namespace ThemePark.Models.Tickets;

[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
public class CreateTicketRequest
{
    [Required(ErrorMessage = "El campo 'fechaVisita' es requerido")]
    public string FechaVisita { get; set; } = string.Empty;

    [Required(ErrorMessage = "El campo 'tipoEntrada' es requerido")]
    public string TipoEntrada { get; set; } = string.Empty;

    public Guid? EventoId { get; set; }

    [Required(ErrorMessage = "El campo 'codigoIdentificacionUsuario' es requerido")]
    public string CodigoIdentificacionUsuario { get; set; } = string.Empty;

    public Ticket ToEntity()
    {
        var fechaVisita = ParseDate(FechaVisita, "fechaVisita");

        if(TipoEntrada == "general")
        {
            return new GeneralTicket
            {
                Id = Guid.NewGuid(),
                CodigoQR = Guid.NewGuid(),
                FechaVisita = fechaVisita,
                CodigoIdentificacionUsuario = CodigoIdentificacionUsuario,
                FechaCompra = System.DateTime.Now
            };
        }
        else if(TipoEntrada == "evento" && EventoId.HasValue)
        {
            return new EventTicket
            {
                Id = Guid.NewGuid(),
                CodigoQR = Guid.NewGuid(),
                FechaVisita = fechaVisita,
                CodigoIdentificacionUsuario = CodigoIdentificacionUsuario,
                FechaCompra = System.DateTime.Now,
                EventoId = EventoId.Value
            };
        }

        throw new InvalidTicketException("el 'tipoEntrada' debe ser 'general' o 'evento'");
    }
}
