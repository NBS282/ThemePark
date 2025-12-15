using ThemePark.Entities;

namespace ThemePark.Models.Users;

public class PointHistoryResponseModel
{
    public string Fecha { get; set; } = string.Empty;
    public string Origen { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public int Puntos { get; set; }
    public string EstrategiaPuntaje { get; set; } = string.Empty;

    public static PointHistoryResponseModel FromDynamic(object item)
    {
        if(item is Visit visit)
        {
            return new PointHistoryResponseModel
            {
                Fecha = visit.EntryTime.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ"),
                Origen = visit.AttractionName,
                Tipo = "Ganancia", // Las visitas siempre son ganancias de puntos
                Puntos = visit.Points,
                EstrategiaPuntaje = visit.ScoringStrategyName ?? "Sin estrategia"
            };
        }
        else if(item is RewardExchange exchange)
        {
            return new PointHistoryResponseModel
            {
                Fecha = exchange.FechaCanje.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ"),
                Origen = $"Recompensa #{exchange.RewardId}",
                Tipo = "Gasto",
                Puntos = -exchange.PuntosDescontados, // Negativo porque es un gasto
                EstrategiaPuntaje = "Canje de Recompensa"
            };
        }

        return new PointHistoryResponseModel();
    }
}
