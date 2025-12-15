using ThemePark.Entities;
using ThemePark.IDataAccess;

namespace ThemePark.DataAccess.Repositories;

public class RewardExchangeRepository(ThemeParkDbContext context) : IRewardExchangeRepository
{
    private readonly ThemeParkDbContext _context = context;

    public void Save(RewardExchange exchange)
    {
        _context.RewardExchanges.Add(exchange);
        _context.SaveChanges();
    }

    public List<RewardExchange> GetByUserId(Guid userId)
    {
        return _context.RewardExchanges.Where(e => e.UserId == userId).ToList();
    }
}
