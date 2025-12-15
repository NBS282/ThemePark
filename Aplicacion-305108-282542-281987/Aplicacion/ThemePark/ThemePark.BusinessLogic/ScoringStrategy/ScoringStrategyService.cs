using ThemePark.Entities;
using ThemePark.IBusinessLogic;
using ThemePark.IBusinessLogic.Exceptions;
using ThemePark.IDataAccess;
using ThemePark.IDataAccess.Exceptions;

namespace ThemePark.BusinessLogic.ScoringStrategy;

public class ScoringStrategyService(IScoringStrategyRepository repository, IScoringAlgorithmFactory algorithmFactory, IPluginLoader pluginLoader) : IScoringStrategyService
{
    private readonly IPluginLoader _pluginLoader = pluginLoader;
    public List<Entities.ScoringStrategy> GetAllStrategies()
    {
        var strategies = repository.GetAll();
        foreach(var strategy in strategies)
        {
            EnsurePluginConfigurationLoaded(strategy);
        }

        return strategies;
    }

    public Entities.ScoringStrategy GetByName(string name)
    {
        var strategy = repository.GetAll().FirstOrDefault(s => s.Name == name);
        if(strategy == null)
        {
            throw new ScoringStrategyNotFoundException(name);
        }

        EnsurePluginConfigurationLoaded(strategy);
        return strategy;
    }

    public Entities.ScoringStrategy? GetActiveStrategy()
    {
        var strategy = repository.GetActiveStrategy();
        if(strategy != null)
        {
            EnsurePluginConfigurationLoaded(strategy);
        }

        return strategy;
    }

    public Entities.ScoringStrategy Create(Entities.ScoringStrategy entity)
    {
        return repository.Add(entity);
    }

    public int CalculateVisitPoints(Entities.Visit visit, List<Entities.Visit> userVisits)
    {
        var activeStrategy = repository.GetActiveStrategy();
        if(activeStrategy == null)
        {
            throw ScoringStrategyException.NoActiveStrategy();
        }

        EnsurePluginConfigurationLoaded(activeStrategy);

        if(!activeStrategy.IsPluginAvailable)
        {
            throw new ScoringStrategyException(
                $"La estrategia activa '{activeStrategy.Name}' usa un plugin que no está disponible ('{activeStrategy.PluginTypeIdentifier}'). " +
                "Por favor, instale el plugin o active otra estrategia.");
        }

        if(activeStrategy.ConfiguracionTyped == null)
        {
            throw ScoringStrategyException.NoTypedConfiguration();
        }

        var algorithm = algorithmFactory.CreateAlgorithm(activeStrategy);
        return algorithm.CalculatePoints(visit, activeStrategy.ConfiguracionTyped, userVisits);
    }

    public Entities.ScoringStrategy ToggleActive(string name)
    {
        var strategy = repository.GetAll().FirstOrDefault(s => s.Name == name);
        if(strategy == null)
        {
            throw new ScoringStrategyNotFoundException(name);
        }

        if(!strategy.Active)
        {
            var allStrategies = repository.GetAll();
            var hasAnotherActive = allStrategies.Any(s => s.Active && s.Name != name);
            if(hasAnotherActive)
            {
                throw ScoringStrategyException.CannotActivateStrategy();
            }

            EnsurePluginConfigurationLoaded(strategy);
            if(!strategy.IsPluginAvailable)
            {
                throw new ScoringStrategyException(
                    $"No se puede activar la estrategia '{strategy.Name}' porque el plugin '{strategy.PluginTypeIdentifier}' no está disponible. " +
                    "Por favor, instale el plugin correspondiente.");
            }

            strategy.Active = true;
            var updatedStrategy = repository.Update(strategy);
            return updatedStrategy;
        }
        else
        {
            throw ScoringStrategyException.CannotActivateStrategy();
        }
    }

    public void Deactivate()
    {
        var activeStrategy = repository.GetAll().FirstOrDefault(s => s.Active);
        if(activeStrategy == null)
        {
            throw ScoringStrategyException.NoActiveStrategyToDeactivate();
        }

        activeStrategy.Active = false;
        repository.Update(activeStrategy);
    }

    public Entities.ScoringStrategy Update(string name, Entities.ScoringStrategy partialUpdate)
    {
        var existingStrategy = repository.GetAll().FirstOrDefault(s => s.Name == name);
        if(existingStrategy == null)
        {
            throw new ScoringStrategyNotFoundException(name);
        }

        if(!string.IsNullOrEmpty(partialUpdate.Description))
        {
            existingStrategy.Description = partialUpdate.Description;
        }

        if(!string.IsNullOrEmpty(existingStrategy.PluginTypeIdentifier))
        {
            if(!string.IsNullOrEmpty(partialUpdate.ConfigurationJson))
            {
                var plugin = _pluginLoader.GetPlugin(existingStrategy.PluginTypeIdentifier);
                if(plugin == null)
                {
                    throw new ScoringStrategyException($"Plugin '{existingStrategy.PluginTypeIdentifier}' no encontrado");
                }

                Entities.Configuracion newConfig;
                try
                {
                    newConfig = plugin.CreateConfigurationFromJson(partialUpdate.ConfigurationJson);
                }
                catch(Exception ex)
                {
                    throw new ScoringStrategyException($"Error al deserializar la configuración del plugin: {ex.Message}");
                }

                if(!plugin.ValidateConfiguration(newConfig))
                {
                    throw new ScoringStrategyException("La configuración no es válida para este plugin");
                }

                existingStrategy.ConfigurationJson = partialUpdate.ConfigurationJson;
                existingStrategy.ConfiguracionTyped = newConfig;
            }
        }
        else
        {
            if(partialUpdate.Type != null)
            {
                existingStrategy.Type = partialUpdate.Type;
            }

            if(partialUpdate.ConfiguracionTyped != null && existingStrategy.ConfiguracionTyped != null)
            {
                if(existingStrategy.ConfiguracionTyped is ConfiguracionPorAtraccion existingAtraccion
                   && partialUpdate.ConfiguracionTyped is ConfiguracionPorAtraccion newAtraccion)
                {
                    existingAtraccion.Valores = newAtraccion.Valores;
                }
                else if(existingStrategy.ConfiguracionTyped is ConfiguracionPorCombo existingCombo
                        && partialUpdate.ConfiguracionTyped is ConfiguracionPorCombo newCombo)
                {
                    existingCombo.VentanaTemporalMinutos = newCombo.VentanaTemporalMinutos;
                    existingCombo.BonusMultiplicador = newCombo.BonusMultiplicador;
                    existingCombo.MinimoAtracciones = newCombo.MinimoAtracciones;
                }
                else if(existingStrategy.ConfiguracionTyped is ConfiguracionPorEvento existingEvento
                        && partialUpdate.ConfiguracionTyped is ConfiguracionPorEvento newEvento)
                {
                    existingEvento.Puntos = newEvento.Puntos;
                    existingEvento.Evento = newEvento.Evento;
                }

                existingStrategy.ConfigurationJson = System.Text.Json.JsonSerializer.Serialize(existingStrategy.ConfiguracionTyped);
            }
        }

        return repository.Update(existingStrategy);
    }

    public void Delete(string name)
    {
        var strategy = repository.GetAll().FirstOrDefault(s => s.Name == name);
        if(strategy == null)
        {
            throw new ScoringStrategyNotFoundException(name);
        }

        if(strategy.Active)
        {
            throw ScoringStrategyException.CannotDeleteActiveStrategy();
        }

        repository.Delete(strategy.Id);
    }

    public List<object> GetAvailableStrategyTypes()
    {
        var types = new List<object>
        {
            new
            {
                Id = "PuntuacionPorAtraccion",
                Name = "Puntuación por Atracción",
                Description = "Asigna puntos según el tipo de atracción visitada",
                IsPlugin = false
            },
            new
            {
                Id = "PuntuacionCombo",
                Name = "Puntuación Combo",
                Description = "Bonifica al visitar combinaciones específicas de atracciones",
                IsPlugin = false
            },
            new
            {
                Id = "PuntuacionPorEvento",
                Name = "Puntuación por Evento",
                Description = "Otorga puntos al participar en eventos especiales",
                IsPlugin = false
            }
        };

        var plugins = _pluginLoader.GetAllPlugins();
        foreach(var plugin in plugins)
        {
            types.Add(new
            {
                Id = plugin.StrategyTypeIdentifier,
                Name = plugin.StrategyName,
                Description = plugin.Description,
                IsPlugin = true
            });
        }

        return types;
    }

    public Entities.ScoringStrategy CreateStrategy(string nombre, string descripcion, Enums.TipoEstrategia? algoritmo, Entities.Configuracion? configuracion, string? pluginTypeIdentifier, string? configurationJson)
    {
        if(algoritmo.HasValue && !string.IsNullOrEmpty(pluginTypeIdentifier))
        {
            throw new ScoringStrategyException(
                "Debe especificar 'algoritmo' O 'pluginTypeIdentifier', no ambos");
        }

        if(!algoritmo.HasValue && string.IsNullOrEmpty(pluginTypeIdentifier))
        {
            throw new ScoringStrategyException(
                "Debe especificar 'algoritmo' o 'pluginTypeIdentifier'");
        }

        Entities.ScoringStrategy entity;
        if(!string.IsNullOrEmpty(pluginTypeIdentifier))
        {
            if(string.IsNullOrEmpty(configurationJson))
            {
                throw new ScoringStrategyException(
                    "'configurationJson' es requerido para plugins");
            }

            var plugin = _pluginLoader.GetPlugin(pluginTypeIdentifier);
            if(plugin == null)
            {
                throw new ScoringStrategyException(
                    $"Plugin '{pluginTypeIdentifier}' no encontrado");
            }

            Entities.Configuracion config;
            try
            {
                config = plugin.CreateConfigurationFromJson(configurationJson);
            }
            catch(Exception ex)
            {
                throw new ScoringStrategyException(
                    $"Error al deserializar la configuración del plugin: {ex.Message}");
            }

            if(!plugin.ValidateConfiguration(config))
            {
                throw new ScoringStrategyException(
                    "La configuración no es válida para este plugin");
            }

            entity = new Entities.ScoringStrategy(nombre, pluginTypeIdentifier, descripcion, config, configurationJson);
        }
        else
        {
            if(configuracion == null)
            {
                throw new ScoringStrategyException(
                    "'configuracion' es requerido para estrategias nativas");
            }

            entity = new Entities.ScoringStrategy(nombre, algoritmo!.Value, descripcion, configuracion);
        }

        return repository.Add(entity);
    }

    private void EnsurePluginConfigurationLoaded(Entities.ScoringStrategy strategy)
    {
        if(!string.IsNullOrEmpty(strategy.PluginTypeIdentifier))
        {
            var plugin = _pluginLoader.GetPlugin(strategy.PluginTypeIdentifier);
            if(plugin == null)
            {
                strategy.IsPluginAvailable = false;
                Console.WriteLine($"[WARNING] Plugin '{strategy.PluginTypeIdentifier}' no encontrado para la estrategia '{strategy.Name}'");
                return;
            }

            try
            {
                strategy.ConfiguracionTyped = plugin.CreateConfigurationFromJson(strategy.ConfigurationJson);
                strategy.IsPluginAvailable = true;
            }
            catch(Exception ex)
            {
                strategy.IsPluginAvailable = false;
                Console.WriteLine($"[ERROR] Error al cargar configuración del plugin '{strategy.PluginTypeIdentifier}' para la estrategia '{strategy.Name}': {ex.Message}");
            }

            return;
        }

        if(strategy.ConfiguracionTyped != null)
        {
            strategy.IsPluginAvailable = true;
            return;
        }

        strategy.IsPluginAvailable = true;
    }
}
