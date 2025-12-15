using ThemePark.Entities;
using ThemePark.Enums;

namespace ThemePark.IBusinessLogic;

public interface IScoringStrategyService
{
    List<ScoringStrategy> GetAllStrategies();
    ScoringStrategy GetByName(string name);
    ScoringStrategy? GetActiveStrategy();
    ScoringStrategy Create(ScoringStrategy entity);
    ScoringStrategy CreateStrategy(string nombre, string descripcion, TipoEstrategia? algoritmo, Configuracion? configuracion, string? pluginTypeIdentifier, string? configurationJson);
    int CalculateVisitPoints(Visit visit, List<Visit> userVisits);
    ScoringStrategy ToggleActive(string name);
    void Deactivate();
    ScoringStrategy Update(string name, ScoringStrategy partialUpdate);
    void Delete(string name);
    List<object> GetAvailableStrategyTypes();
}
