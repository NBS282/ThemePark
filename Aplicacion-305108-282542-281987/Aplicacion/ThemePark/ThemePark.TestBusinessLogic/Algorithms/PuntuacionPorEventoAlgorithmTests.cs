using Moq;
using ThemePark.BusinessLogic.Algorithms;
using ThemePark.Entities;
using ThemePark.Enums;
using ThemePark.IBusinessLogic;
using ThemePark.IBusinessLogic.Exceptions;
using ThemePark.IDataAccess;

namespace ThemePark.TestBusinessLogic.Algorithms;

[TestClass]
public class PuntuacionPorEventoAlgorithmTests
{
    private Mock<IEventRepository> _mockEventRepository = null!;
    private IScoringAlgorithm _algorithm = null!;
    private Visit _testVisit = null!;
    private Attraction _testAttraction = null!;
    private Event _testEvent = null!;
    private Event _testEvent2 = null!;

    [TestInitialize]
    public void TestInitialize()
    {
        _mockEventRepository = new Mock<IEventRepository>(MockBehavior.Strict);
        _algorithm = new PuntuacionPorEventoAlgorithm(_mockEventRepository.Object);

        _testAttraction = new Attraction("Montaña Rusa", TipoAtraccion.MontañaRusa, 12, 50, "Emocionante", DateTime.Now, 100);
        _testVisit = new Visit(Guid.NewGuid(), _testAttraction, DateTime.Parse("2026-03-14"));

        _testEvent = new Event
        {
            Id = Guid.NewGuid(),
            Name = "Noche de Dinosaurios",
            Fecha = DateTime.Parse("2026-03-14"),
            Hora = TimeSpan.FromHours(00),
            Aforo = 100,
            CostoAdicional = 10.00m,
            Atracciones = [_testAttraction]
        };
        _testEvent2 = new Event
        {
            Id = Guid.NewGuid(),
            Name = "Noche de Carreras",
            Fecha = DateTime.Parse("2026-03-14"),
            Hora = TimeSpan.FromHours(00),
            Aforo = 100,
            CostoAdicional = 10.00m,
            Atracciones = []
        };
    }

    [TestCleanup]
    public void TestCleanup()
    {
        _mockEventRepository.VerifyAll();
    }

    [TestMethod]
    public void CalculatePoints_ShouldReturnEventPoints_WhenVisitDuringEventAndAttractionInEvent()
    {
        var configuracion = new ConfiguracionPorEvento(2, "Noche de Dinosaurios");
        var userVisits = new List<Visit>();

        _mockEventRepository.Setup(r => r.Get(It.IsAny<System.Linq.Expressions.Expression<System.Func<Event, bool>>>()))
                           .Returns([_testEvent]);

        var points = _algorithm.CalculatePoints(_testVisit, configuracion, userVisits);

        Assert.AreEqual(200, points);
    }

    [TestMethod]
    public void CalculatePoints_ShouldReturnBasePoints_WhenVisitDuringEventAndAttractionNotInEvent()
    {
        var configuracion = new ConfiguracionPorEvento(100, "Noche de Carreras");
        var userVisits = new List<Visit>();

        _mockEventRepository.Setup(r => r.Get(It.IsAny<System.Linq.Expressions.Expression<System.Func<Event, bool>>>()))
            .Returns([_testEvent2]);

        var points = _algorithm.CalculatePoints(_testVisit, configuracion, userVisits);

        Assert.AreEqual(100, points);
    }

    [TestMethod]
    public void CalculatePoints_ShouldReturnEventPoints_WhenUsingConfiguracionPorEvento()
    {
        var configuracion = new ConfiguracionPorEvento(4, "Noche de Dinosaurios");
        var userVisits = new List<Visit>();

        _mockEventRepository.Setup(r => r.Get(It.IsAny<System.Linq.Expressions.Expression<System.Func<Event, bool>>>()))
                           .Returns([_testEvent]);

        var points = _algorithm.CalculatePoints(_testVisit, configuracion, userVisits);

        Assert.AreEqual(400, points);
    }

    [TestMethod]
    [ExpectedException(typeof(ScoringStrategyException))]
    public void CalculatePoints_ShouldThrowScoringStrategyException_WhenConfigurationIsNotConfiguracionPorEvento()
    {
        var configuracion = new ConfiguracionPorAtraccion(new Dictionary<string, int> { ["montaña rusa"] = 100 });
        var userVisits = new List<Visit>();
        _algorithm.CalculatePoints(_testVisit, configuracion, userVisits);
    }

    [TestMethod]
    public void CalculatePoints_ShouldReturnBasePoints_WhenVisitOutsideEventTimeWindow()
    {
        var configuracion = new ConfiguracionPorEvento(2, "Noche de Dinosaurios");
        var userVisits = new List<Visit>();
        var lateVisit = new Visit(Guid.NewGuid(), _testAttraction, DateTime.Parse("2026-03-14 05:00:00"));

        _mockEventRepository.Setup(r => r.Get(It.IsAny<System.Linq.Expressions.Expression<System.Func<Event, bool>>>()))
                           .Returns([_testEvent]);

        var points = _algorithm.CalculatePoints(lateVisit, configuracion, userVisits);

        Assert.AreEqual(100, points);
    }
}
