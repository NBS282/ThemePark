using Moq;
using ThemePark.BusinessLogic.Algorithms;
using ThemePark.Entities;
using ThemePark.Enums;
using ThemePark.IBusinessLogic;
using ThemePark.IBusinessLogic.Exceptions;
using ThemePark.IDataAccess;

namespace ThemePark.TestBusinessLogic.Algorithms;

[TestClass]
public class PuntuacionPorAtraccionAlgorithmTests
{
    private Mock<IAttractionRepository> _mockAttractionRepository = null!;
    private IScoringAlgorithm _algorithm = null!;
    private Visit _testVisit = null!;
    private Attraction _testAttraction = null!;

    [TestInitialize]
    public void TestInitialize()
    {
        _mockAttractionRepository = new Mock<IAttractionRepository>(MockBehavior.Strict);
        _algorithm = new PuntuacionPorAtraccionAlgorithm(_mockAttractionRepository.Object);
        _testAttraction = new Attraction("Montaña Rusa", TipoAtraccion.MontañaRusa, 12, 50, "Emocionante montaña rusa", DateTime.Now, 100);
        _testVisit = new Visit(Guid.NewGuid(), _testAttraction, DateTime.Now);
    }

    [TestMethod]
    public void CalculatePoints_ShouldReturnPointsBasedOnAttractionType()
    {
        _mockAttractionRepository.Setup(r => r.GetByName("Montaña Rusa")).Returns(_testAttraction);
        var configuracion = new ConfiguracionPorAtraccion(new Dictionary<string, int> { ["MontañaRusa"] = 100, ["Simulador"] = 50 });
        var userVisits = new List<Visit>();
        var points = _algorithm.CalculatePoints(_testVisit, configuracion, userVisits);
        Assert.AreEqual(100, points);
    }

    [TestMethod]
    public void CalculatePoints_ShouldReturnBasePoints_WhenAttractionTypeNotInConfiguration()
    {
        _mockAttractionRepository.Setup(r => r.GetByName("Montaña Rusa")).Returns(_testAttraction);
        var configuracion = new ConfiguracionPorAtraccion(new Dictionary<string, int> { ["Simulador"] = 50 });
        var userVisits = new List<Visit>();
        var points = _algorithm.CalculatePoints(_testVisit, configuracion, userVisits);
        Assert.AreEqual(100, points);
    }

    [TestMethod]
    public void CalculatePoints_ShouldReturnPointsBasedOnAttractionType_WhenUsingConfiguracionPorAtraccion()
    {
        _mockAttractionRepository.Setup(r => r.GetByName("Montaña Rusa")).Returns(_testAttraction);
        var configuracion = new ConfiguracionPorAtraccion(new Dictionary<string, int> { ["MontañaRusa"] = 150, ["Simulador"] = 75 });
        var userVisits = new List<Visit>();
        var points = _algorithm.CalculatePoints(_testVisit, configuracion, userVisits);
        Assert.AreEqual(150, points);
    }

    [TestMethod]
    [ExpectedException(typeof(ScoringStrategyException))]
    public void CalculatePoints_ShouldThrowScoringStrategyException_WhenConfigurationIsNotConfiguracionPorAtraccion()
    {
        var configuracion = new ConfiguracionPorCombo(10, 50, 2);
        var userVisits = new List<Visit>();
        _algorithm.CalculatePoints(_testVisit, configuracion, userVisits);
    }
}
