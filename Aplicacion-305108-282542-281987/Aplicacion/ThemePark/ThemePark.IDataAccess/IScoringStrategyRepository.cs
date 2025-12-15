using ThemePark.Entities;

namespace ThemePark.IDataAccess;

public interface IScoringStrategyRepository
{
    ScoringStrategy Add(ScoringStrategy scoringStrategy);
    ScoringStrategy Update(ScoringStrategy scoringStrategy);
    ScoringStrategy GetById(Guid id);
    List<ScoringStrategy> GetAll();
    ScoringStrategy? GetActiveStrategy();
    bool Delete(Guid id);
    bool ExistsByName(string name);
    void DeactivateStrategiesByPluginTypeIdentifier(string pluginTypeIdentifier);
}
