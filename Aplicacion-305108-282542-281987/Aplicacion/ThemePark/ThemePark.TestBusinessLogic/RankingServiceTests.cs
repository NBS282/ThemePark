using Moq;
using ThemePark.BusinessLogic;
using ThemePark.Entities;
using ThemePark.IBusinessLogic;
using ThemePark.IDataAccess;

namespace ThemePark.TestBusinessLogic;

[TestClass]
public class RankingServiceTests
{
    private Mock<IVisitRepository> _mockVisitRepository = null!;
    private Mock<IUserRepository> _mockUserRepository = null!;
    private Mock<IDateTimeRepository> _mockDateTimeRepository = null!;
    private IRankingService _rankingService = null!;

    [TestInitialize]
    public void TestInitialize()
    {
        _mockVisitRepository = new Mock<IVisitRepository>(MockBehavior.Strict);
        _mockUserRepository = new Mock<IUserRepository>(MockBehavior.Strict);
        _mockDateTimeRepository = new Mock<IDateTimeRepository>(MockBehavior.Strict);
        _rankingService = new RankingService(_mockVisitRepository.Object, _mockUserRepository.Object, _mockDateTimeRepository.Object);
    }

    [TestCleanup]
    public void TestCleanup()
    {
        _mockVisitRepository.VerifyAll();
        _mockUserRepository.VerifyAll();
        _mockDateTimeRepository.VerifyAll();
    }

    [TestMethod]
    public void GetDailyRanking_ShouldReturnCorrectResultAndCount_WhenVisitsExistForToday()
    {
        var today = DateTime.Today;
        var user1 = new User { Id = Guid.NewGuid(), Nombre = "Juan", Apellido = "Pérez", Email = "juan@email.com" };
        var user2 = new User { Id = Guid.NewGuid(), Nombre = "María", Apellido = "López", Email = "maria@email.com" };

        var visits = new List<Visit>
        {
            new Visit(user1.Id, "Montaña Rusa", today.AddHours(10), 10),
            new Visit(user2.Id, "Carrusel", today.AddHours(11), 10),
            new Visit(user1.Id, "Simulador", today.AddHours(12), 10)
        };

        _mockDateTimeRepository.Setup(x => x.GetCurrentDateTime()).Returns(today);
        _mockVisitRepository.Setup(x => x.GetVisitsByDate(today.Date)).Returns(visits);
        _mockUserRepository.Setup(x => x.GetById(user1.Id)).Returns(user1);
        _mockUserRepository.Setup(x => x.GetById(user2.Id)).Returns(user2);

        var result = _rankingService.GetDailyRanking();

        Assert.IsNotNull(result);
        Assert.AreEqual(2, result.Count);
        Assert.AreEqual(1, result[0].Posicion);
    }

    [TestMethod]
    public void GetDailyRanking_ShouldReturnFirstUserCorrectly_WhenVisitsExistForToday()
    {
        var today = DateTime.Today;
        var user1 = new User { Id = Guid.NewGuid(), Nombre = "Juan", Apellido = "Pérez", Email = "juan@email.com" };
        var user2 = new User { Id = Guid.NewGuid(), Nombre = "María", Apellido = "López", Email = "maria@email.com" };

        var visits = new List<Visit>
        {
            new Visit(user1.Id, "Montaña Rusa", today.AddHours(10), 10),
            new Visit(user2.Id, "Carrusel", today.AddHours(11), 10),
            new Visit(user1.Id, "Simulador", today.AddHours(12), 10)
        };

        _mockDateTimeRepository.Setup(x => x.GetCurrentDateTime()).Returns(today);
        _mockVisitRepository.Setup(x => x.GetVisitsByDate(today.Date)).Returns(visits);
        _mockUserRepository.Setup(x => x.GetById(user1.Id)).Returns(user1);
        _mockUserRepository.Setup(x => x.GetById(user2.Id)).Returns(user2);

        var result = _rankingService.GetDailyRanking();

        Assert.AreEqual("Juan", result[0].Usuario.Nombre);
        Assert.AreEqual(20, result[0].Puntos);
    }

    [TestMethod]
    public void GetDailyRanking_ShouldReturnSecondUserCorrectly_WhenVisitsExistForToday()
    {
        var today = DateTime.Today;
        var user1 = new User { Id = Guid.NewGuid(), Nombre = "Juan", Apellido = "Pérez", Email = "juan@email.com" };
        var user2 = new User { Id = Guid.NewGuid(), Nombre = "María", Apellido = "López", Email = "maria@email.com" };

        var visits = new List<Visit>
        {
            new Visit(user1.Id, "Montaña Rusa", today.AddHours(10), 10),
            new Visit(user2.Id, "Carrusel", today.AddHours(11), 10),
            new Visit(user1.Id, "Simulador", today.AddHours(12), 10)
        };

        _mockDateTimeRepository.Setup(x => x.GetCurrentDateTime()).Returns(today);
        _mockVisitRepository.Setup(x => x.GetVisitsByDate(today.Date)).Returns(visits);
        _mockUserRepository.Setup(x => x.GetById(user1.Id)).Returns(user1);
        _mockUserRepository.Setup(x => x.GetById(user2.Id)).Returns(user2);

        var result = _rankingService.GetDailyRanking();

        Assert.AreEqual(2, result[1].Posicion);
        Assert.AreEqual("María", result[1].Usuario.Nombre);
        Assert.AreEqual(10, result[1].Puntos);
    }

    [TestMethod]
    public void GetDailyRanking_ShouldReturnEmptyList_WhenNoVisitsExist()
    {
        var today = DateTime.Today;

        _mockDateTimeRepository.Setup(x => x.GetCurrentDateTime()).Returns(today);
        _mockVisitRepository.Setup(x => x.GetVisitsByDate(today.Date)).Returns([]);

        var result = _rankingService.GetDailyRanking();

        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public void GetDailyRanking_ShouldUseCurrentDateTime_WhenRepositoryReturnsNull()
    {
        var user1 = new User { Id = Guid.NewGuid(), Nombre = "Juan", Apellido = "Pérez", Email = "juan@email.com" };
        var visits = new List<Visit>
        {
            new Visit(user1.Id, "Montaña Rusa", DateTime.Now, 10)
        };

        _mockDateTimeRepository.Setup(x => x.GetCurrentDateTime()).Returns((DateTime?)null);
        _mockVisitRepository.Setup(x => x.GetVisitsByDate(It.IsAny<DateTime>())).Returns(visits);
        _mockUserRepository.Setup(x => x.GetById(user1.Id)).Returns(user1);

        var result = _rankingService.GetDailyRanking();

        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Count);
    }
}
