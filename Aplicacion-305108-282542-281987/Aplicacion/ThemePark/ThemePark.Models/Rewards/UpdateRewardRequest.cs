using System.Text.Json.Serialization;

namespace ThemePark.Models.Rewards;

[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
public class UpdateRewardRequest
{
    public string? Descripcion { get; set; }
    public int? CostoPuntos { get; set; }
    public int? CantidadDisponible { get; set; }
    public int? NivelMembresiaRequerido { get; set; }
}
