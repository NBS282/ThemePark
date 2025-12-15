using ThemePark.Entities;

namespace ThemePark.IBusinessLogic;

public interface IRewardsBusinessLogic
{
    Reward CreateReward(string nombre, string descripcion, int costoPuntos, int cantidadDisponible, int? nivelMembresiaRequerido);
    void DeleteReward(int id);
    List<Reward> GetAllRewards();
    Reward GetRewardById(int id);
    Reward UpdateReward(int id, string? descripcion, int? costoPuntos, int? cantidadDisponible, int? nivelMembresiaRequerido);
    List<Reward> GetAvailableRewardsForUser();
    RewardExchange ExchangeReward(int id);
    List<RewardExchange> GetUserExchanges();
}
