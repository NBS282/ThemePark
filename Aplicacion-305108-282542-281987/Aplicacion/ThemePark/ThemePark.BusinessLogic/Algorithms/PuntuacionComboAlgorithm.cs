using ThemePark.Entities;
using ThemePark.IBusinessLogic;
using ThemePark.IBusinessLogic.Exceptions;

namespace ThemePark.BusinessLogic.Algorithms;

public class PuntuacionComboAlgorithm : IScoringAlgorithm
{
    public int CalculatePoints(Visit visit, Configuracion configuration, List<Visit> userVisits)
    {
        if(configuration is not ConfiguracionPorCombo comboConfig)
        {
            throw ScoringStrategyException.InvalidConfigurationType("ConfiguracionPorCombo");
        }

        var currentTime = visit.EntryTime;
        var timeWindow = TimeSpan.FromMinutes(comboConfig.VentanaTemporalMinutos);

        var recentVisits = userVisits.Where(v =>
            currentTime - v.EntryTime <= timeWindow &&
            v.AttractionName != visit.AttractionName)
            .ToList();

        var basePoints = visit.Attraction?.Points ?? 0;

        if(recentVisits.Count + 1 >= comboConfig.MinimoAtracciones)
        {
            return comboConfig.BonusMultiplicador + basePoints;
        }

        return basePoints;
    }
}
