namespace ThemePark.IBusinessLogic;

public interface IPluginLoader
{
    List<IScoringStrategyPlugin> GetAllPlugins();
    IScoringStrategyPlugin? GetPlugin(string typeIdentifier);
}
