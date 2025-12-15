using ThemePark.Entities;
using ThemePark.IBusinessLogic;
using ThemePark.IBusinessLogic.Exceptions;
using ThemePark.IDataAccess;

namespace ThemePark.BusinessLogic.Algorithms;

public class PuntuacionPorAtraccionAlgorithm(IAttractionRepository attractionRepository) : IScoringAlgorithm
{
    private readonly IAttractionRepository _attractionRepository = attractionRepository;

    public int CalculatePoints(Visit visit, Configuracion configuration, List<Visit> userVisits)
    {
        if(configuration is not ConfiguracionPorAtraccion atraccionConfig)
        {
            throw ScoringStrategyException.InvalidConfigurationType("ConfiguracionPorAtraccion");
        }

        var attraction = _attractionRepository.GetByName(visit.AttractionName);
        var attractionType = attraction.Tipo.ToString();

        if(atraccionConfig.Valores.TryGetValue(attractionType, out var points))
        {
            return points;
        }

        return attraction.Points;
    }
}
