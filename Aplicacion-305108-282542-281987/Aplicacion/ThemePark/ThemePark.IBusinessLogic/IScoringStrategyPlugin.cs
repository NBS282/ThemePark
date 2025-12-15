using ThemePark.Entities;

namespace ThemePark.IBusinessLogic;

public interface IScoringStrategyPlugin
{
    string StrategyName { get; }
    string Description { get; }
    string StrategyTypeIdentifier { get; }

    object GetConfigurationSchema();

    Configuracion CreateDefaultConfiguration();
    Configuracion CreateConfigurationFromJson(string json);
    string SerializeConfiguration(Configuracion config);
    bool ValidateConfiguration(Configuracion config);

    IScoringAlgorithm CreateAlgorithm(IServiceProvider serviceProvider);
}
