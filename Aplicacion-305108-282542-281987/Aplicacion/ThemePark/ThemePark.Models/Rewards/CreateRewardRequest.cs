using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ThemePark.Models.Rewards;

[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
public class CreateRewardRequest
{
    [Required(ErrorMessage = "El campo 'nombre' es requerido")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "El campo 'descripcion' es requerido")]
    public string Descripcion { get; set; } = string.Empty;

    [Required(ErrorMessage = "El campo 'costoPuntos' es requerido")]
    public int? CostoPuntos { get; set; }

    [Required(ErrorMessage = "El campo 'cantidadDisponible' es requerido")]
    public int? CantidadDisponible { get; set; }

    public int? NivelMembresiaRequerido { get; set; }
}
