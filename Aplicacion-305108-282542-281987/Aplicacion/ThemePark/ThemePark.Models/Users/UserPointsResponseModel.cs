namespace ThemePark.Models.Users;

public class UserPointsResponseModel
{
    public int PuntosDisponibles { get; set; }
    public int PuntosAcumulados { get; set; }
    public int PuntosGastados { get; set; }

    public static UserPointsResponseModel FromDynamic(object item)
    {
        var type = item.GetType();
        return new UserPointsResponseModel
        {
            PuntosDisponibles = (int)(type.GetProperty("PuntosDisponibles")?.GetValue(item) ?? 0),
            PuntosAcumulados = (int)(type.GetProperty("PuntosAcumulados")?.GetValue(item) ?? 0),
            PuntosGastados = (int)(type.GetProperty("PuntosGastados")?.GetValue(item) ?? 0)
        };
    }
}
