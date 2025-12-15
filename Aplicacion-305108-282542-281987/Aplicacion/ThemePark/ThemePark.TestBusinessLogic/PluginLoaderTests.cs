using ThemePark.BusinessLogic.ScoringStrategy;

namespace ThemePark.TestBusinessLogic;

[TestClass]
public class PluginLoaderTests
{
    [TestMethod]
    public void GetAllPlugins_ShouldReturnEmptyList_WhenNoPluginsLoaded()
    {
        var pluginLoader = new ScoringStrategyPluginLoader();

        var result = pluginLoader.GetAllPlugins();

        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public void LoadPlugins_ShouldScanPluginsFolderAndLoadDlls()
    {
        var pluginLoader = new ScoringStrategyPluginLoader();

        pluginLoader.LoadPlugins("./Plugins");

        var result = pluginLoader.GetAllPlugins();
        Assert.IsNotNull(result);
    }

    [TestMethod]
    public void GetPlugin_ShouldReturnNull_WhenPluginNotFound()
    {
        var pluginLoader = new ScoringStrategyPluginLoader();

        var result = pluginLoader.GetPlugin("NonExistentPlugin");

        Assert.IsNull(result);
    }
}
