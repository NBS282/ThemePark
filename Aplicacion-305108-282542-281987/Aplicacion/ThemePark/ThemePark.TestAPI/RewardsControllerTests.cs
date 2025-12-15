using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Moq;
using ThemePark.Entities;
using ThemePark.Enums;
using ThemePark.IBusinessLogic;
using ThemePark.Models.Rewards;
using ThemeParkApi.Controllers;
using ThemeParkApi.Filters;

namespace ThemePark.TestAPI;

[TestClass]
public class RewardsControllerTests
{
    private Mock<IRewardsBusinessLogic> _mockRewardsBusinessLogic = null!;
    private RewardsController _controller = null!;
    private ExceptionFilter _exceptionFilter = null!;
    private AuthenticationFilter _authenticationFilter = null!;
    private Mock<HttpContext> _httpContextMock = null!;
    private AuthorizationFilterContext _authorizationContext = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockRewardsBusinessLogic = new Mock<IRewardsBusinessLogic>(MockBehavior.Strict);
        _controller = new RewardsController(_mockRewardsBusinessLogic.Object);
        _exceptionFilter = new ExceptionFilter();
        _authenticationFilter = new AuthenticationFilter();
        _httpContextMock = new Mock<HttpContext>(MockBehavior.Strict);
        _authorizationContext = new AuthorizationFilterContext(
            new ActionContext(_httpContextMock.Object, new RouteData(), new ActionDescriptor()),
            []);
    }

    [TestMethod]
    public void CreateReward_ShouldReturnCreated_WhenValidRequestProvided()
    {
        var request = new CreateRewardRequest
        {
            Nombre = "Entrada VIP",
            Descripcion = "Acceso prioritario a todas las atracciones",
            CostoPuntos = 500,
            CantidadDisponible = 10,
            NivelMembresiaRequerido = 2
        };

        var mockReward = new Reward("Entrada VIP", "Acceso prioritario a todas las atracciones", 500, 10, NivelMembresia.VIP);

        _mockRewardsBusinessLogic.Setup(x => x.CreateReward(
            request.Nombre, request.Descripcion, request.CostoPuntos!.Value, request.CantidadDisponible!.Value, request.NivelMembresiaRequerido))
            .Returns(mockReward);

        var result = _controller.CreateReward(request);

        Assert.IsInstanceOfType(result, typeof(CreatedResult));
        var createdResult = result as CreatedResult;
        Assert.AreEqual(201, createdResult!.StatusCode);
        _mockRewardsBusinessLogic.Verify(x => x.CreateReward(
            request.Nombre, request.Descripcion, request.CostoPuntos!.Value, request.CantidadDisponible!.Value, request.NivelMembresiaRequerido), Times.Once);
    }

    [TestMethod]
    public void CreateReward_ShouldCallBusinessLogicOnce_WhenValidRequestProvided()
    {
        var request = new CreateRewardRequest
        {
            Nombre = "Entrada VIP",
            Descripcion = "Acceso prioritario a todas las atracciones",
            CostoPuntos = 500,
            CantidadDisponible = 10,
            NivelMembresiaRequerido = 2
        };

        var mockReward = new Reward("Entrada VIP", "Acceso prioritario a todas las atracciones", 500, 10, NivelMembresia.VIP);

        _mockRewardsBusinessLogic.Setup(x => x.CreateReward(
            request.Nombre, request.Descripcion, request.CostoPuntos!.Value, request.CantidadDisponible!.Value, request.NivelMembresiaRequerido))
            .Returns(mockReward);

        _controller.CreateReward(request);

        _mockRewardsBusinessLogic.Verify(x => x.CreateReward(
            request.Nombre, request.Descripcion, request.CostoPuntos!.Value, request.CantidadDisponible!.Value, request.NivelMembresiaRequerido), Times.Once);
    }

    [TestMethod]
    public void DeleteReward_ShouldReturnNoContent_WhenValidIdProvided()
    {
        var rewardId = 1;

        _mockRewardsBusinessLogic.Setup(x => x.DeleteReward(rewardId));

        var result = _controller.DeleteReward(rewardId);

        Assert.IsInstanceOfType(result, typeof(NoContentResult));
        var noContentResult = result as NoContentResult;
        Assert.AreEqual(204, noContentResult!.StatusCode);
        _mockRewardsBusinessLogic.Verify(x => x.DeleteReward(rewardId), Times.Once);
    }

    [TestMethod]
    public void GetAllRewards_ShouldReturnOk_WhenCalled()
    {
        var expectedRewards = new List<Reward>
        {
            new Reward("Entrada VIP", "Descripción 1", 500, 10, null),
            new Reward("Descuento Combo", "Descripción 2", 300, 20, NivelMembresia.Premium)
        };

        _mockRewardsBusinessLogic.Setup(x => x.GetAllRewards()).Returns(expectedRewards);

        var result = _controller.GetAllRewards();

        Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        _mockRewardsBusinessLogic.Verify(x => x.GetAllRewards(), Times.Once);
    }

    [TestMethod]
    public void GetRewardById_ShouldReturnOk_WhenValidIdProvided()
    {
        var rewardId = 1;
        var expectedReward = new Reward("Entrada VIP", "Descripción", 500, 10, null);

        _mockRewardsBusinessLogic.Setup(x => x.GetRewardById(rewardId)).Returns(expectedReward);

        var result = _controller.GetRewardById(rewardId);

        Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        _mockRewardsBusinessLogic.Verify(x => x.GetRewardById(rewardId), Times.Once);
    }

    [TestMethod]
    public void UpdateReward_ShouldReturnOk_WhenValidRequestProvided()
    {
        var rewardId = 1;
        var request = new UpdateRewardRequest
        {
            Descripcion = "Nueva descripción",
            CostoPuntos = 600
        };

        var expectedReward = new Reward("Entrada VIP", "Nueva descripción", 600, 10, null);

        _mockRewardsBusinessLogic.Setup(x => x.UpdateReward(
            rewardId, request.Descripcion, request.CostoPuntos, request.CantidadDisponible, request.NivelMembresiaRequerido))
            .Returns(expectedReward);

        var result = _controller.UpdateReward(rewardId, request);

        Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        _mockRewardsBusinessLogic.Verify(x => x.UpdateReward(
            rewardId, request.Descripcion, request.CostoPuntos, request.CantidadDisponible, request.NivelMembresiaRequerido), Times.Once);
    }

    [TestMethod]
    public void GetAvailableRewardsForUser_ShouldReturnOk_WhenCalled()
    {
        var expectedRewards = new List<Reward>
        {
            new Reward("Entrada VIP", "Descripción", 500, 10, null)
        };

        _mockRewardsBusinessLogic.Setup(x => x.GetAvailableRewardsForUser()).Returns(expectedRewards);

        var result = _controller.GetAvailableRewardsForUser();

        Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        _mockRewardsBusinessLogic.Verify(x => x.GetAvailableRewardsForUser(), Times.Once);
    }

    [TestMethod]
    public void ExchangeReward_ShouldReturnCreated_WhenValidIdProvided()
    {
        var rewardId = 1;
        var expectedExchange = new RewardExchange(rewardId, Guid.NewGuid(), 500, 500);

        _mockRewardsBusinessLogic.Setup(x => x.ExchangeReward(rewardId)).Returns(expectedExchange);

        var result = _controller.ExchangeReward(rewardId);

        Assert.IsInstanceOfType(result, typeof(CreatedResult));
        _mockRewardsBusinessLogic.Verify(x => x.ExchangeReward(rewardId), Times.Once);
    }

    [TestMethod]
    public void GetUserExchanges_ShouldReturnOk_WhenCalled()
    {
        var expectedExchanges = new List<RewardExchange>
        {
            new RewardExchange(1, Guid.NewGuid(), 500, 500)
        };

        _mockRewardsBusinessLogic.Setup(x => x.GetUserExchanges()).Returns(expectedExchanges);

        var result = _controller.GetUserExchanges();

        Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        _mockRewardsBusinessLogic.Verify(x => x.GetUserExchanges(), Times.Once);
    }
}
