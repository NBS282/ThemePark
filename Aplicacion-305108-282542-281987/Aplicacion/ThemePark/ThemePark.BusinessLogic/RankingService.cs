using ThemePark.Entities;
using ThemePark.IBusinessLogic;
using ThemePark.IDataAccess;

namespace ThemePark.BusinessLogic;

public class RankingService(IVisitRepository visitRepository,
                            IUserRepository userRepository, IDateTimeRepository dateTimeRepository) : IRankingService
{
    private readonly IVisitRepository _visitRepository = visitRepository;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IDateTimeRepository _dateTimeRepository = dateTimeRepository;

    public List<UserRanking> GetDailyRanking()
    {
        var today = _dateTimeRepository.GetCurrentDateTime() ?? DateTime.Now;
        var visits = _visitRepository.GetVisitsByDate(today.Date);

        var userPoints = visits
            .GroupBy(v => v.UserId)
            .Select(g => new { UserId = g.Key, TotalPoints = g.Sum(v => v.Points) })
            .OrderByDescending(x => x.TotalPoints)
            .Take(10)
            .ToList();

        var ranking = new List<UserRanking>();
        var position = 1;

        foreach(var userPoint in userPoints)
        {
            var user = _userRepository.GetById(userPoint.UserId);
            ranking.Add(new UserRanking(position++, user, userPoint.TotalPoints, today.Date));
        }

        return ranking;
    }
}
