using ThemePark.Entities;

namespace ThemePark.IDataAccess;

public interface IRewardExchangeRepository
{
    void Save(RewardExchange exchange);
    List<RewardExchange> GetByUserId(Guid userId);
}
