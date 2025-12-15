using ThemePark.BusinessLogic.Algorithms;
using ThemePark.Entities;
using ThemePark.Enums;
using ThemePark.IBusinessLogic;
using ThemePark.IBusinessLogic.Exceptions;

namespace ThemePark.TestBusinessLogic.Algorithms;

[TestClass]
public class PuntuacionComboAlgorithmTests
{
    private IScoringAlgorithm _algorithm = null!;
    private Visit _testVisit = null!;
    private Attraction _testAttraction = null!;

    [TestInitialize]
    public void TestInitialize()
    {
        _algorithm = new PuntuacionComboAlgorithm();
        _testAttraction = new Attraction("Montaña Rusa", TipoAtraccion.MontañaRusa, 12, 50, "Emocionante", DateTime.Now, 100);
        _testVisit = new Visit(Guid.NewGuid(), _testAttraction, DateTime.Now);
    }

    [TestMethod]
    public void CalculatePoints_ShouldReturnBonusPoints_WhenCompletingCombo()
    {
        var configuracion = new ConfiguracionPorCombo(10, 50, 2);
        var userId = Guid.NewGuid();
        var baseTime = DateTime.Now;

        var userVisits = new List<Visit>
        {
            new Visit(userId, "Simulador", baseTime.AddMinutes(-5))
        };
        var points = _algorithm.CalculatePoints(_testVisit, configuracion, userVisits);
        Assert.AreEqual(150, points);
    }

    [TestMethod]
    public void CalculatePoints_ShouldReturnBasePoints_WhenNotCompletingCombo()
    {
        var configuracion = new ConfiguracionPorCombo(10, 50, 3);
        var userId = Guid.NewGuid();
        var baseTime = DateTime.Now;

        var userVisits = new List<Visit>
        {
            new Visit(userId, "Montaña Rusa", baseTime.AddMinutes(-5))
        };
        var points = _algorithm.CalculatePoints(_testVisit, configuracion, userVisits);
        Assert.AreEqual(100, points);
    }

    [TestMethod]
    public void CalculatePoints_ShouldReturnBonusPoints_WhenUsingConfiguracionPorCombo()
    {
        var configuracion = new ConfiguracionPorCombo(10, 50, 2);
        var userId = Guid.NewGuid();
        var baseTime = DateTime.Now;

        var userVisits = new List<Visit>
        {
            new Visit(userId, "Simulador", baseTime.AddMinutes(-5))
        };
        var points = _algorithm.CalculatePoints(_testVisit, configuracion, userVisits);
        Assert.AreEqual(150, points);
    }

    [TestMethod]
    [ExpectedException(typeof(ScoringStrategyException))]
    public void CalculatePoints_ShouldThrowScoringStrategyException_WhenConfigurationIsNotConfiguracionPorCombo()
    {
        var configuracion = new ConfiguracionPorAtraccion(new Dictionary<string, int> { ["montaña rusa"] = 100 });
        var userVisits = new List<Visit>();
        _algorithm.CalculatePoints(_testVisit, configuracion, userVisits);
    }

    [TestMethod]
    public void CalculatePoints_ShouldReturnBasePoints_WhenVisitsOutsideTimeWindow()
    {
        var configuracion = new ConfiguracionPorCombo(10, 50, 2);
        var userId = Guid.NewGuid();
        var baseTime = DateTime.Now;

        var userVisits = new List<Visit>
        {
            new Visit(userId, "Simulador", baseTime.AddMinutes(-15))
        };
        var points = _algorithm.CalculatePoints(_testVisit, configuracion, userVisits);
        Assert.AreEqual(100, points);
    }
}
