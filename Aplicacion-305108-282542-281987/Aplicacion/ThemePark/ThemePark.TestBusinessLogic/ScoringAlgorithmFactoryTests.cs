using Moq;
using ThemePark.BusinessLogic.Algorithms;
using ThemePark.BusinessLogic.ScoringStrategy;
using ThemePark.Enums;
using ThemePark.IBusinessLogic;
using ThemePark.IDataAccess;

namespace ThemePark.TestBusinessLogic;

[TestClass]
public class ScoringAlgorithmFactoryTests
{
    private IScoringAlgorithmFactory _factory = null!;
    private Mock<IEventRepository> _mockEventRepository = null!;
    private Mock<IAttractionRepository> _mockAttractionRepository = null!;
    private Mock<IPluginLoader> _mockPluginLoader = null!;
    private Mock<IServiceProvider> _mockServiceProvider = null!;

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
    public void CreateAlgorithm_ShouldReturnPuntuacionPorAtraccionAlgorithm_WhenTypeIsPuntuacionPorAtraccion()
    {
        var result = _factory.CreateAlgorithm(TipoEstrategia.PuntuacionPorAtraccion);

        Assert.IsInstanceOfType(result, typeof(PuntuacionPorAtraccionAlgorithm));
    }

    [TestMethod]
    public void CreateAlgorithm_ShouldReturnPuntuacionComboAlgorithm_WhenTypeIsPuntuacionCombo()
    {
        var result = _factory.CreateAlgorithm(TipoEstrategia.PuntuacionCombo);

        Assert.IsInstanceOfType(result, typeof(PuntuacionComboAlgorithm));
    }

    [TestMethod]
    public void CreateAlgorithm_ShouldReturnPuntuacionPorEventoAlgorithm_WhenTypeIsPuntuacionPorEvento()
    {
        var result = _factory.CreateAlgorithm(TipoEstrategia.PuntuacionPorEvento);

        Assert.IsInstanceOfType(result, typeof(PuntuacionPorEventoAlgorithm));
    }
}
