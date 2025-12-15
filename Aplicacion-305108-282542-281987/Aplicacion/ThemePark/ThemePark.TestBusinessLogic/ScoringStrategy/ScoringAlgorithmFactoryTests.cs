using Moq;
using ThemePark.BusinessLogic.ScoringStrategy;
using ThemePark.Enums;
using ThemePark.IBusinessLogic;
using ThemePark.IBusinessLogic.Exceptions;
using ThemePark.IDataAccess;
using ScoringStrategyEntity = ThemePark.Entities.ScoringStrategy;

namespace ThemePark.TestBusinessLogic.ScoringStrategy;

[TestClass]
public class
    ScoringAlgorithmFactoryTests
{
    private Mock<IEventRepository> _mockEventRepository = null!;
    private Mock<IAttractionRepository> _mockAttractionRepository = null!;
    private Mock<IPluginLoader> _mockPluginLoader = null!;
    private Mock<IServiceProvider> _mockServiceProvider = null!;
    private IScoringAlgorithmFactory _factory = null!;

    [TestInitialize]
    public void TestInitialize()
    {
        _mockEventRepository = new Mock<IEventRepository>(MockBehavior.Strict);
        _mockAttractionRepository = new Mock<IAttractionRepository>(MockBehavior.Strict);
        _mockPluginLoader = new Mock<IPluginLoader>(MockBehavior.Strict);
        _mockServiceProvider = new Mock<IServiceProvider>(MockBehavior.Strict);
        _factory = new ScoringAlgorithmFactory(_mockEventRepository.Object, _mockAttractionRepository.Object, _mockPluginLoader.Object, _mockServiceProvider.Object);
    }

    [TestMethod]
    public void CreateAlgorithm_ShouldReturnPuntuacionPorAtraccionAlgorithm_WhenTypePuntuacionPorAtraccion()
    {
        var algorithm = _factory.CreateAlgorithm(TipoEstrategia.PuntuacionPorAtraccion);
        Assert.IsNotNull(algorithm);
        Assert.AreEqual("ThemePark.BusinessLogic.Algorithms.PuntuacionPorAtraccionAlgorithm", algorithm.GetType().FullName);
    }

    [TestMethod]
    public void CreateAlgorithm_ShouldReturnPuntuacionComboAlgorithm_WhenTypePuntuacionCombo()
    {
        var algorithm = _factory.CreateAlgorithm(TipoEstrategia.PuntuacionCombo);
        Assert.IsNotNull(algorithm);
        Assert.AreEqual("ThemePark.BusinessLogic.Algorithms.PuntuacionComboAlgorithm", algorithm.GetType().FullName);
    }

    [TestMethod]
    public void CreateAlgorithm_ShouldReturnPuntuacionPorEventoAlgorithm_WhenTypePuntuacionPorEvento()
    {
        var algorithm = _factory.CreateAlgorithm(TipoEstrategia.PuntuacionPorEvento);
        Assert.IsNotNull(algorithm);
        Assert.AreEqual("ThemePark.BusinessLogic.Algorithms.PuntuacionPorEventoAlgorithm", algorithm.GetType().FullName);
    }

    [TestMethod]
    [ExpectedException(typeof(ScoringStrategyException))]
    public void CreateAlgorithm_ShouldThrowScoringStrategyException_WhenTypeNotSupported()
    {
        _factory.CreateAlgorithm((TipoEstrategia)999);
    }

    [TestMethod]
    public void CreateAlgorithm_ShouldUsePluginAlgorithm_WhenStrategyIsPlugin()
    {
        var pluginTypeId = "PuntuacionPorHora";
        var mockConfig = new ThemePark.Entities.ConfiguracionPorAtraccion([]);
        var pluginStrategy = new ScoringStrategyEntity("Test Plugin", pluginTypeId, "Plugin test", mockConfig, "{}");

        var mockAlgorithm = new Mock<IScoringAlgorithm>();
        var mockPlugin = new Mock<IScoringStrategyPlugin>();
        mockPlugin.Setup(p => p.CreateAlgorithm(_mockServiceProvider.Object)).Returns(mockAlgorithm.Object);

        _mockPluginLoader.Setup(l => l.GetPlugin(pluginTypeId)).Returns(mockPlugin.Object);

        var result = _factory.CreateAlgorithm(pluginStrategy);

        Assert.IsNotNull(result);
        Assert.AreSame(mockAlgorithm.Object, result);
        mockPlugin.Verify(p => p.CreateAlgorithm(_mockServiceProvider.Object), Times.Once);
    }

    [TestMethod]
    public void CreateAlgorithm_ShouldUseNativeAlgorithm_WhenStrategyIsNative()
    {
        var mockConfig = new ThemePark.Entities.ConfiguracionPorAtraccion([]);
        var nativeStrategy = new ScoringStrategyEntity("Test Native", TipoEstrategia.PuntuacionPorAtraccion, "Native test", mockConfig);

        var result = _factory.CreateAlgorithm(nativeStrategy);

        Assert.IsNotNull(result);
        Assert.AreEqual("ThemePark.BusinessLogic.Algorithms.PuntuacionPorAtraccionAlgorithm", result.GetType().FullName);
    }
}
