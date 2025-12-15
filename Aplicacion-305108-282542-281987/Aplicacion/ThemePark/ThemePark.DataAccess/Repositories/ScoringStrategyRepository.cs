using Microsoft.EntityFrameworkCore;
using ThemePark.Entities;
using ThemePark.IDataAccess;

namespace ThemePark.DataAccess.Repositories;

public class ScoringStrategyRepository(ThemeParkDbContext context) : IScoringStrategyRepository
{
    private readonly ThemeParkDbContext _context = context;

    public ScoringStrategy Add(ScoringStrategy scoringStrategy)
    {
        if(!string.IsNullOrEmpty(scoringStrategy.PluginTypeIdentifier))
        {
            var config = scoringStrategy.ConfiguracionTyped;
            scoringStrategy.ConfiguracionTyped = null!;
            _context.ScoringStrategies.Add(scoringStrategy);
            _context.SaveChanges();
            scoringStrategy.ConfiguracionTyped = config;
        }
        else
        {
            _context.ScoringStrategies.Add(scoringStrategy);
            _context.SaveChanges();
        }

        return scoringStrategy;
    }

    public ScoringStrategy Update(ScoringStrategy scoringStrategy)
    {
        if(!string.IsNullOrEmpty(scoringStrategy.PluginTypeIdentifier))
        {
            var config = scoringStrategy.ConfiguracionTyped;
            scoringStrategy.ConfiguracionTyped = null!;
            _context.ScoringStrategies.Update(scoringStrategy);
            _context.SaveChanges();
            scoringStrategy.ConfiguracionTyped = config;
        }
        else
        {
            _context.ScoringStrategies.Update(scoringStrategy);
            _context.SaveChanges();
        }

        return scoringStrategy;
    }

    public ScoringStrategy GetById(Guid id)
    {
        return _context.ScoringStrategies
            .Include(s => s.ConfiguracionTyped)
            .First(s => s.Id == id);
    }

    public List<ScoringStrategy> GetAll()
    {
        return _context.ScoringStrategies
            .AsNoTracking()
            .Include(s => s.ConfiguracionTyped)
            .ToList();
    }

    public ScoringStrategy? GetActiveStrategy()
    {
        return _context.ScoringStrategies
            .AsNoTracking()
            .Include(s => s.ConfiguracionTyped)
            .FirstOrDefault(s => s.Active);
    }

    public bool Delete(Guid id)
    {
        var strategy = _context.ScoringStrategies.First(s => s.Id == id);
        _context.ScoringStrategies.Remove(strategy);
        _context.SaveChanges();
        return true;
    }

    public bool ExistsByName(string name)
    {
        return _context.ScoringStrategies.Any(s => s.Name == name);
    }

    public void DeactivateStrategiesByPluginTypeIdentifier(string pluginTypeIdentifier)
    {
        var strategies = _context.ScoringStrategies
            .Where(s => s.PluginTypeIdentifier == pluginTypeIdentifier)
            .ToList();

        foreach(var strategy in strategies)
        {
            strategy.Active = false;
        }

        _context.SaveChanges();
    }
}
