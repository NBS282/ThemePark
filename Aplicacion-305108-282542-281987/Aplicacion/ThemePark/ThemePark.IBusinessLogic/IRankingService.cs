using ThemePark.Entities;

namespace ThemePark.IBusinessLogic;

public interface IRankingService
{
    List<UserRanking> GetDailyRanking();
}
