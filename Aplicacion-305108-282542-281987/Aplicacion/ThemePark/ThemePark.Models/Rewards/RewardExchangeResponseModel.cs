namespace ThemePark.Models.Rewards;

public class RewardExchangeResponseModel
{
    public int Id { get; set; }
    public int RewardId { get; set; }
    public Guid UserId { get; set; }
    public int PuntosDescontados { get; set; }
    public int PuntosRestantesUsuario { get; set; }
    public System.DateTime FechaCanje { get; set; }
    public string Estado { get; set; } = string.Empty;

    public static RewardExchangeResponseModel FromRewardExchange(dynamic exchange)
    {
        return new RewardExchangeResponseModel
        {
            Id = exchange.Id,
            RewardId = exchange.RewardId,
            UserId = exchange.UserId,
            PuntosDescontados = exchange.PuntosDescontados,
            PuntosRestantesUsuario = exchange.PuntosRestantesUsuario,
            FechaCanje = exchange.FechaCanje,
            Estado = exchange.Estado
        };
    }
}
