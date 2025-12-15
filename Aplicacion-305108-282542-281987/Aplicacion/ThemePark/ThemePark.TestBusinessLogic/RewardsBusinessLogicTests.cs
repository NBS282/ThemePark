using Moq;
using ThemePark.BusinessLogic;
using ThemePark.Entities;
using ThemePark.Enums;
using ThemePark.Exceptions;
using ThemePark.IBusinessLogic;
using ThemePark.IBusinessLogic.Exceptions;
using ThemePark.IDataAccess;

namespace ThemePark.TestBusinessLogic;

[TestClass]
public class RewardsBusinessLogicTests
{
    private Mock<IRewardRepository> _mockRewardRepository = null!;
    private Mock<IRewardExchangeRepository> _mockRewardExchangeRepository = null!;
    private Mock<IUserRepository> _mockUserRepository = null!;
    private Mock<IVisitRepository> _mockVisitRepository = null!;
    private Mock<ISessionRepository> _mockSessionRepository = null!;
    private IRewardsBusinessLogic _rewardsBusinessLogic = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockRewardRepository = new Mock<IRewardRepository>(MockBehavior.Strict);
        _mockRewardExchangeRepository = new Mock<IRewardExchangeRepository>(MockBehavior.Strict);
        _mockUserRepository = new Mock<IUserRepository>(MockBehavior.Strict);
        _mockVisitRepository = new Mock<IVisitRepository>(MockBehavior.Strict);
        _mockSessionRepository = new Mock<ISessionRepository>(MockBehavior.Strict);

        _rewardsBusinessLogic = new RewardsBusinessLogic(
            _mockRewardRepository.Object,
            _mockRewardExchangeRepository.Object,
            _mockVisitRepository.Object,
            _mockUserRepository.Object,
            _mockSessionRepository.Object);
    }

    [TestMethod]
    public void CreateReward_ShouldCreateAndSaveReward_WhenValidDataProvided()
    {
        var nombre = "Entrada VIP";
        var descripcion = "Acceso prioritario a todas las atracciones";
        var costoPuntos = 500;
        var cantidadDisponible = 10;
        int? nivelMembresiaRequerido = (int)NivelMembresia.Premium;

        _mockRewardRepository.Setup(x => x.ExistsByName(nombre)).Returns(false);
        _mockRewardRepository.Setup(x => x.Save(It.IsAny<Reward>()));

        var result = _rewardsBusinessLogic.CreateReward(nombre, descripcion, costoPuntos, cantidadDisponible, nivelMembresiaRequerido);

        _mockRewardRepository.Verify(x => x.Save(It.Is<Reward>(r =>
            r.Nombre == nombre &&
            r.Descripcion == descripcion &&
            r.CostoPuntos == costoPuntos &&
            r.CantidadDisponible == cantidadDisponible &&
            r.NivelMembresiaRequerido == (NivelMembresia)nivelMembresiaRequerido)), Times.Once);
    }

    [TestMethod]
    public void DeleteReward_ShouldDeleteReward_WhenIdExists()
    {
        var rewardId = 1;
        var existingReward = new Reward("Entrada VIP", "Descripción", 500, 10, null);

        _mockRewardRepository.Setup(x => x.GetById(rewardId)).Returns(existingReward);
        _mockRewardRepository.Setup(x => x.Delete(rewardId));

        _rewardsBusinessLogic.DeleteReward(rewardId);

        _mockRewardRepository.Verify(x => x.Delete(rewardId), Times.Once);
    }

    [TestMethod]
    public void GetAllRewards_ShouldReturnAllRewards_WhenCalled()
    {
        var rewards = new List<Reward>
        {
            new Reward("Entrada VIP", "Descripción 1", 500, 10, null),
            new Reward("Descuento Combo", "Descripción 2", 300, 20, NivelMembresia.Premium)
        };

        _mockRewardRepository.Setup(x => x.GetAll()).Returns(rewards);

        var result = _rewardsBusinessLogic.GetAllRewards();

        Assert.IsInstanceOfType(result, typeof(List<Reward>));
        _mockRewardRepository.Verify(x => x.GetAll(), Times.Once);
    }

    [TestMethod]
    public void GetRewardById_ShouldReturnReward_WhenIdExists()
    {
        var rewardId = 1;
        var reward = new Reward("Entrada VIP", "Descripción", 500, 10, null);

        _mockRewardRepository.Setup(x => x.GetById(rewardId)).Returns(reward);

        var result = _rewardsBusinessLogic.GetRewardById(rewardId);

        Assert.IsInstanceOfType(result, typeof(Reward));
        _mockRewardRepository.Verify(x => x.GetById(rewardId), Times.Once);
    }

    [TestMethod]
    public void CreateReward_ShouldThrowException_WhenRewardAlreadyExists()
    {
        var nombre = "Entrada VIP";
        _mockRewardRepository.Setup(x => x.ExistsByName(nombre)).Returns(true);

        Assert.ThrowsException<BusinessLogicException>(() =>
            _rewardsBusinessLogic.CreateReward(nombre, "Descripción", 500, 10, null));
    }

    [TestMethod]
    public void CreateReward_ShouldCreateRewardWithoutMembership_WhenNivelMembresiaIsNull()
    {
        var nombre = "Descuento General";
        var descripcion = "Descuento sin requisitos";
        var costoPuntos = 200;
        var cantidadDisponible = 50;

        _mockRewardRepository.Setup(x => x.ExistsByName(nombre)).Returns(false);
        _mockRewardRepository.Setup(x => x.Save(It.IsAny<Reward>()));

        var result = _rewardsBusinessLogic.CreateReward(nombre, descripcion, costoPuntos, cantidadDisponible, null);

        Assert.IsNull(result.NivelMembresiaRequerido);
    }

    [TestMethod]
    public void DeleteReward_ShouldThrowException_WhenRewardNotFound()
    {
        var rewardId = 999;
        _mockRewardRepository.Setup(x => x.GetById(rewardId)).Returns((Reward?)null);

        Assert.ThrowsException<BusinessLogicException>(() => _rewardsBusinessLogic.DeleteReward(rewardId));
    }

    [TestMethod]
    public void GetRewardById_ShouldThrowException_WhenRewardNotFound()
    {
        var rewardId = 999;
        _mockRewardRepository.Setup(x => x.GetById(rewardId)).Returns((Reward?)null);

        Assert.ThrowsException<BusinessLogicException>(() => _rewardsBusinessLogic.GetRewardById(rewardId));
    }

    [TestMethod]
    public void UpdateReward_ShouldThrowException_WhenRewardNotFound()
    {
        var rewardId = 999;
        _mockRewardRepository.Setup(x => x.GetById(rewardId)).Returns((Reward?)null);

        Assert.ThrowsException<BusinessLogicException>(() =>
            _rewardsBusinessLogic.UpdateReward(rewardId, "Nueva desc", 200, 5, null));
    }

    [TestMethod]
    public void UpdateReward_ShouldKeepOriginalValues_WhenAllParametersAreNull()
    {
        var rewardId = 1;
        var reward = new Reward("Entrada VIP", "Descripción original", 500, 10, NivelMembresia.Premium);

        _mockRewardRepository.Setup(x => x.GetById(rewardId)).Returns(reward);
        _mockRewardRepository.Setup(x => x.Save(reward));

        _rewardsBusinessLogic.UpdateReward(rewardId, null, null, null, null);

        Assert.AreEqual("Descripción original", reward.Descripcion);
        Assert.AreEqual(500, reward.CostoPuntos);
        Assert.AreEqual(10, reward.CantidadDisponible);
        Assert.AreEqual(NivelMembresia.Premium, reward.NivelMembresiaRequerido);
    }

    [TestMethod]
    public void UpdateReward_ShouldUpdateAndSaveReward_WhenValidDataProvided()
    {
        var rewardId = 1;
        var reward = new Reward("Entrada VIP", "Descripción original", 500, 10, NivelMembresia.Premium);
        var nuevaDescripcion = "Nueva descripción";
        int? nuevoCosto = 600;
        int? nuevaCantidad = 20;
        int? nuevoNivel = (int)NivelMembresia.VIP;

        _mockRewardRepository.Setup(x => x.GetById(rewardId)).Returns(reward);
        _mockRewardRepository.Setup(x => x.Save(reward));

        var result = _rewardsBusinessLogic.UpdateReward(rewardId, nuevaDescripcion, nuevoCosto, nuevaCantidad, nuevoNivel);

        _mockRewardRepository.Verify(x => x.Save(reward), Times.Once);
        Assert.IsInstanceOfType(result, typeof(Reward));
    }

    [TestMethod]
    public void GetUserExchanges_ShouldReturnUserExchanges_WhenCalled()
    {
        var userId = Guid.NewGuid();
        var exchanges = new List<RewardExchange>
        {
            new RewardExchange(1, userId, 500, 500),
            new RewardExchange(2, userId, 300, 200)
        };

        _mockSessionRepository.Setup(x => x.GetCurrentUserId()).Returns(userId);
        _mockRewardExchangeRepository.Setup(x => x.GetByUserId(userId)).Returns(exchanges);

        var result = _rewardsBusinessLogic.GetUserExchanges();

        Assert.IsInstanceOfType(result, typeof(List<RewardExchange>));
        _mockRewardExchangeRepository.Verify(x => x.GetByUserId(userId), Times.Once);
    }

    [TestMethod]
    public void ExchangeReward_ShouldThrowException_WhenRewardDoesNotExist()
    {
        var rewardId = 999;
        _mockRewardRepository.Setup(x => x.GetById(rewardId)).Returns((Reward?)null);

        Assert.ThrowsException<BusinessLogicException>(() => _rewardsBusinessLogic.ExchangeReward(rewardId));
        _mockRewardRepository.Verify(x => x.GetById(rewardId), Times.Once);
    }

    [TestMethod]
    public void ExchangeReward_ShouldThrowException_WhenRewardIsInactive()
    {
        var rewardId = 1;
        var reward = new Reward("Entrada VIP", "Descripción", 500, 10, null);
        reward.Desactivar();

        _mockRewardRepository.Setup(x => x.GetById(rewardId)).Returns(reward);

        Assert.ThrowsException<BusinessLogicException>(() => _rewardsBusinessLogic.ExchangeReward(rewardId));
        _mockRewardRepository.Verify(x => x.GetById(rewardId), Times.Once);
    }

    [TestMethod]
    public void ExchangeReward_ShouldThrowException_WhenRewardHasNoStock()
    {
        var rewardId = 1;
        var reward = new Reward("Entrada VIP", "Descripción", 500, 0, null);
        var userId = Guid.NewGuid();
        var userVisits = new List<Visit>
        {
            new Visit(userId, "Atracción 1", DateTime.Now, 600)
        };
        var previousExchanges = new List<RewardExchange>();

        _mockRewardRepository.Setup(x => x.GetById(rewardId)).Returns(reward);
        _mockSessionRepository.Setup(x => x.GetCurrentUserId()).Returns(userId);
        _mockVisitRepository.Setup(x => x.GetByUserId(userId)).Returns(userVisits);
        _mockRewardExchangeRepository.Setup(x => x.GetByUserId(userId)).Returns(previousExchanges);

        Assert.ThrowsException<InvalidRequestDataException>(() => _rewardsBusinessLogic.ExchangeReward(rewardId));
        _mockRewardRepository.Verify(x => x.GetById(rewardId), Times.Once);
    }

    [TestMethod]
    public void ExchangeReward_ShouldThrowException_WhenUserHasInsufficientPoints()
    {
        var rewardId = 1;
        var reward = new Reward("Entrada VIP", "Descripción", 500, 10, null);
        var userId = Guid.NewGuid();
        var userVisits = new List<Visit>
        {
            new Visit(userId, "Atracción 1", DateTime.Now, 100),
            new Visit(userId, "Atracción 2", DateTime.Now, 200)
        };
        var previousExchanges = new List<RewardExchange>();

        _mockRewardRepository.Setup(x => x.GetById(rewardId)).Returns(reward);
        _mockSessionRepository.Setup(x => x.GetCurrentUserId()).Returns(userId);
        _mockVisitRepository.Setup(x => x.GetByUserId(userId)).Returns(userVisits);
        _mockRewardExchangeRepository.Setup(x => x.GetByUserId(userId)).Returns(previousExchanges);

        Assert.ThrowsException<BusinessLogicException>(() => _rewardsBusinessLogic.ExchangeReward(rewardId));
        _mockRewardRepository.Verify(x => x.GetById(rewardId), Times.Once);
    }

    [TestMethod]
    public void ExchangeReward_ShouldThrowException_WhenUserDoesNotMeetMembershipLevel()
    {
        var rewardId = 1;
        var reward = new Reward("Entrada VIP", "Descripción", 500, 10, NivelMembresia.VIP);
        var userId = Guid.NewGuid();
        var user = new User([new ThemePark.Entities.Roles.RolVisitante { NivelMembresia = NivelMembresia.Estándar }])
        {
            Id = userId,
            Nombre = "Juan",
            Apellido = "Perez",
            Email = "juan@test.com",
            Contraseña = "Password123!",
            FechaNacimiento = DateTime.Now.AddYears(-25)
        };
        var userVisits = new List<Visit>
        {
            new Visit(userId, "Atracción 1", DateTime.Now, 600)
        };
        var previousExchanges = new List<RewardExchange>();

        _mockRewardRepository.Setup(x => x.GetById(rewardId)).Returns(reward);
        _mockSessionRepository.Setup(x => x.GetCurrentUserId()).Returns(userId);
        _mockVisitRepository.Setup(x => x.GetByUserId(userId)).Returns(userVisits);
        _mockRewardExchangeRepository.Setup(x => x.GetByUserId(userId)).Returns(previousExchanges);
        _mockUserRepository.Setup(x => x.GetById(userId)).Returns(user);

        Assert.ThrowsException<BusinessLogicException>(() => _rewardsBusinessLogic.ExchangeReward(rewardId));
        _mockRewardRepository.Verify(x => x.GetById(rewardId), Times.Once);
    }

    [TestMethod]
    public void ExchangeReward_ShouldCreateExchangeSuccessfully_WhenAllValidationsPass()
    {
        var rewardId = 1;
        var reward = new Reward("Entrada VIP", "Descripción", 500, 10, null);
        var userId = Guid.NewGuid();
        var userVisits = new List<Visit>
        {
            new Visit(userId, "Atracción 1", DateTime.Now, 600)
        };
        var previousExchanges = new List<RewardExchange>();

        _mockRewardRepository.Setup(x => x.GetById(rewardId)).Returns(reward);
        _mockSessionRepository.Setup(x => x.GetCurrentUserId()).Returns(userId);
        _mockVisitRepository.Setup(x => x.GetByUserId(userId)).Returns(userVisits);
        _mockRewardExchangeRepository.Setup(x => x.GetByUserId(userId)).Returns(previousExchanges);
        _mockRewardExchangeRepository.Setup(x => x.Save(It.IsAny<RewardExchange>()));
        _mockRewardRepository.Setup(x => x.Save(reward));

        var result = _rewardsBusinessLogic.ExchangeReward(rewardId);

        Assert.IsNotNull(result);
        Assert.IsInstanceOfType(result, typeof(RewardExchange));
        Assert.AreEqual(rewardId, result.RewardId);
        Assert.AreEqual(userId, result.UserId);
        Assert.AreEqual(500, result.PuntosDescontados);
        Assert.AreEqual(100, result.PuntosRestantesUsuario);
        _mockRewardExchangeRepository.Verify(x => x.Save(It.IsAny<RewardExchange>()), Times.Once);
        _mockRewardRepository.Verify(x => x.Save(reward), Times.Once);
    }
}
