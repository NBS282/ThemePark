using ThemePark.Entities;
using ThemePark.IDataAccess;

namespace ThemePark.DataAccess.Repositories;

public class RewardRepository(ThemeParkDbContext context) : IRewardRepository
{
    private readonly ThemeParkDbContext _context = context;

    public bool ExistsByName(string nombre)
    {
        return _context.Rewards.Any(r => r.Nombre == nombre);
    }

    public void Save(Reward reward)
    {
        var existing = _context.Rewards.Find(reward.Id);
        if(existing == null)
        {
            _context.Rewards.Add(reward);
        }
        else
        {
            _context.Entry(existing).CurrentValues.SetValues(reward);
        }

        _context.SaveChanges();
    }

    public Reward? GetById(int id)
    {
        return _context.Rewards.Find(id);
    }

    public List<Reward> GetAll()
    {
        return _context.Rewards.ToList();
    }

    public void Delete(int id)
    {
        var reward = _context.Rewards.Find(id);
        if(reward != null)
        {
            _context.Rewards.Remove(reward);
            _context.SaveChanges();
        }
    }
}
