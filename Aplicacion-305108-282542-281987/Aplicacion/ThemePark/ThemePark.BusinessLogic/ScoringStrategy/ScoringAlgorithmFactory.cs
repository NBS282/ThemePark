using ThemePark.BusinessLogic.Algorithms;
using ThemePark.Enums;
using ThemePark.IBusinessLogic;
using ThemePark.IBusinessLogic.Exceptions;
using ThemePark.IDataAccess;

namespace ThemePark.BusinessLogic.ScoringStrategy;

public class ScoringAlgorithmFactory(IEventRepository eventRepository, IAttractionRepository attractionRepository, IPluginLoader pluginLoader, IServiceProvider serviceProvider) : IScoringAlgorithmFactory
{
    private readonly IEventRepository _eventRepository = eventRepository;
    private readonly IAttractionRepository _attractionRepository = attractionRepository;
    private readonly IPluginLoader _pluginLoader = pluginLoader;
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    public IScoringAlgorithm CreateAlgorithm(TipoEstrategia? tipoEstrategia)
    {
        return tipoEstrategia switch
        {
            TipoEstrategia.PuntuacionPorAtraccion => new PuntuacionPorAtraccionAlgorithm(_attractionRepository),
            TipoEstrategia.PuntuacionCombo => new PuntuacionComboAlgorithm(),
            TipoEstrategia.PuntuacionPorEvento => new PuntuacionPorEventoAlgorithm(_eventRepository),
            _ => throw ScoringStrategyException.UnsupportedAlgorithmType(tipoEstrategia.ToString())
        };
    }

    public IScoringAlgorithm CreateAlgorithm(Entities.ScoringStrategy strategy)
    {
        if(!string.IsNullOrEmpty(strategy.PluginTypeIdentifier))
        {
            var plugin = _pluginLoader.GetPlugin(strategy.PluginTypeIdentifier);
            if(plugin == null)
            {
                throw new ScoringStrategyException($"Plugin '{strategy.PluginTypeIdentifier}' no encontrado");
            }

            return plugin.CreateAlgorithm(_serviceProvider);
        }

        return CreateAlgorithm(strategy.Type);
    }
}
