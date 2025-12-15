using Microsoft.AspNetCore.Mvc;
using Moq;
using ThemePark.Entities;
using ThemePark.Entities.Roles;
using ThemePark.Enums;
using ThemePark.IBusinessLogic;
using ThemePark.Models.Scoring;
using ThemeParkApi.Controllers;

namespace ThemePark.TestAPI;

[TestClass]
public class ScoringControllerTests
{
    private Mock<IRankingService> _mockRankingService = null!;
    private ScoringController _controller = null!;

    [TestInitialize]
    public void TestInitialize()
    {
        _mockRankingService = new Mock<IRankingService>(MockBehavior.Strict);
        _controller = new ScoringController(_mockRankingService.Object);
    }

    [TestCleanup]
    public void TestCleanup()
    {
        _mockRankingService.VerifyAll();
    }

    [TestMethod]
    public void GetRanking_ShouldReturnOk_WhenRankingExists()
    {
        var user1 = new User
        {
            Id = Guid.NewGuid(),
            Nombre = "Juan",
            Apellido = "Pérez",
            Email = "juan@email.com",
            Contraseña = "password123",
            FechaNacimiento = DateTime.Parse("1990-01-15"),
            Roles = [new RolVisitante { NivelMembresia = NivelMembresia.Estándar }],
            FechaRegistro = DateTime.Now
        };

        var user2 = new User
        {
            Id = Guid.NewGuid(),
            Nombre = "María",
            Apellido = "López",
            Email = "maria@email.com",
            Contraseña = "password456",
            FechaNacimiento = DateTime.Parse("1985-03-20"),
            Roles = [new RolVisitante { NivelMembresia = NivelMembresia.Premium }],
            FechaRegistro = DateTime.Now
        };

        var today = DateTime.Today;
        var expectedRanking = new List<UserRanking>
        {
            new UserRanking(1, user1, 1250, today),
            new UserRanking(2, user2, 1100, today)
        };

        _mockRankingService.Setup(x => x.GetDailyRanking())
                          .Returns(expectedRanking);

        var result = _controller.GetRanking();

        Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        var okResult = (OkObjectResult)result;
        Assert.AreEqual(200, okResult.StatusCode);
        _mockRankingService.Verify(x => x.GetDailyRanking(), Times.Once);
    }

    [TestMethod]
    public void GetRanking_ShouldReturnRankingWithCorrectCount_WhenRankingExists()
    {
        var user1 = new User
        {
            Id = Guid.NewGuid(),
            Nombre = "Juan",
            Apellido = "Pérez",
            Email = "juan@email.com",
            Contraseña = "password123",
            FechaNacimiento = DateTime.Parse("1990-01-15"),
            Roles = [new RolVisitante()],
            FechaRegistro = DateTime.Now
        };

        var user2 = new User
        {
            Id = Guid.NewGuid(),
            Nombre = "María",
            Apellido = "López",
            Email = "maria@email.com",
            Contraseña = "password456",
            FechaNacimiento = DateTime.Parse("1985-03-20"),
            Roles = [new RolVisitante { NivelMembresia = NivelMembresia.Premium }],
            FechaRegistro = DateTime.Now
        };

        var today = DateTime.Today;
        var expectedRanking = new List<UserRanking>
        {
            new UserRanking(1, user1, 1250, today),
            new UserRanking(2, user2, 1100, today)
        };

        _mockRankingService.Setup(x => x.GetDailyRanking())
                          .Returns(expectedRanking);

        var result = _controller.GetRanking();
        var okResult = (OkObjectResult)result;
        var ranking = (List<RankingResponseModel>)okResult.Value!;

        Assert.AreEqual(2, ranking.Count);
        Assert.AreEqual(1, ranking[0].Posicion);
        Assert.AreEqual("Juan", ranking[0].Usuario.Nombre);
    }

    [TestMethod]
    public void GetRanking_ShouldReturnFirstUserWithCorrectPoints_WhenRankingExists()
    {
        var user1 = new User
        {
            Id = Guid.NewGuid(),
            Nombre = "Juan",
            Apellido = "Pérez",
            Email = "juan@email.com",
            Contraseña = "password123",
            FechaNacimiento = DateTime.Parse("1990-01-15"),
            Roles = [new RolVisitante()],
            FechaRegistro = DateTime.Now
        };

        var user2 = new User
        {
            Id = Guid.NewGuid(),
            Nombre = "María",
            Apellido = "López",
            Email = "maria@email.com",
            Contraseña = "password456",
            FechaNacimiento = DateTime.Parse("1985-03-20"),
            Roles = [new RolVisitante { NivelMembresia = NivelMembresia.Premium }],
            FechaRegistro = DateTime.Now
        };

        var today = DateTime.Today;
        var expectedRanking = new List<UserRanking>
        {
            new UserRanking(1, user1, 1250, today),
            new UserRanking(2, user2, 1100, today)
        };

        _mockRankingService.Setup(x => x.GetDailyRanking())
                          .Returns(expectedRanking);

        var result = _controller.GetRanking();
        var okResult = (OkObjectResult)result;
        var ranking = (List<RankingResponseModel>)okResult.Value!;

        Assert.AreEqual(1250, ranking[0].Puntos);
    }
}
