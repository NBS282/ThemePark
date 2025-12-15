using ThemePark.Entities;

namespace ThemePark.IBusinessLogic;

public interface IScoringAlgorithm
{
    int CalculatePoints(Visit visit, Configuracion configuration, List<Visit> userVisits);
}
