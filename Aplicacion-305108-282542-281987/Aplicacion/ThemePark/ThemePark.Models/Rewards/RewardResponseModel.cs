namespace ThemePark.Models.Rewards;

public class RewardResponseModel
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public int CostoPuntos { get; set; }
    public int CantidadDisponible { get; set; }
    public string NivelMembresiaRequerido { get; set; } = string.Empty;
    public bool Activa { get; set; }
    public System.DateTime FechaCreacion { get; set; }

    public static RewardResponseModel FromReward(dynamic reward)
    {
        return new RewardResponseModel
        {
            Id = reward.Id,
            Nombre = reward.Nombre,
            Descripcion = reward.Descripcion,
            CostoPuntos = reward.CostoPuntos,
            CantidadDisponible = reward.CantidadDisponible,
            NivelMembresiaRequerido = reward.NivelMembresiaRequerido?.ToString() ?? string.Empty,
            Activa = reward.Activa,
            FechaCreacion = reward.FechaCreacion
        };
    }
}
