using ThemePark.DataAccess;
using ThemePark.DataAccess.Repositories;
using ThemePark.DataAccess.Utils;
using ThemePark.Entities;
using ThemePark.Enums;

namespace ThemePark.TestDataAccess;

[TestClass]
public class RewardRepositoryTests
{
    private readonly ThemeParkDbContext _context = DbContextBuilder.BuildTestDbContext();
    private readonly RewardRepository _repository;

    public RewardRepositoryTests()
    {
        _repository = new RewardRepository(_context);
    }

    [TestInitialize]
    public void Setup()
    {
        _context.Database.EnsureCreated();
    }

    [TestCleanup]
    public void Cleanup()
    {
        _context.Database.EnsureDeleted();
    }

    [TestMethod]
    public void ExistsByName_ShouldReturnFalse_WhenRewardDoesNotExist()
    {
        var nombre = "Recompensa Inexistente";

        var result = _repository.ExistsByName(nombre);

        Assert.IsFalse(result);
    }

    [TestMethod]
    public void Save_ShouldStoreReward_WhenValidRewardProvided()
    {
        var reward = new Reward("Entrada VIP", "Acceso prioritario", 500, 10, NivelMembresia.Premium);

        _repository.Save(reward);

        var exists = _repository.ExistsByName("Entrada VIP");
        Assert.IsTrue(exists);
    }

    [TestMethod]
    public void GetById_ShouldReturnReward_WhenRewardExists()
    {
        var reward = new Reward("Descuento Combo", "Descuento en comida", 300, 20, null);
        _repository.Save(reward);

        var result = _repository.GetById(reward.Id);

        Assert.IsNotNull(result);
        Assert.AreEqual("Descuento Combo", result.Nombre);
    }

    [TestMethod]
    public void GetAll_ShouldReturnAllRewards()
    {
        var reward1 = new Reward("Entrada VIP", "Acceso prioritario", 500, 10, NivelMembresia.Premium);
        var reward2 = new Reward("Descuento Combo", "Descuento en comida", 300, 20, null);
        _repository.Save(reward1);
        _repository.Save(reward2);

        var result = _repository.GetAll();

        Assert.AreEqual(2, result.Count);
    }

    [TestMethod]
    public void Delete_ShouldRemoveReward_WhenRewardExists()
    {
        var reward = new Reward("Entrada VIP", "Acceso prioritario", 500, 10, NivelMembresia.Premium);
        _repository.Save(reward);

        _repository.Delete(reward.Id);

        var exists = _repository.ExistsByName("Entrada VIP");
        Assert.IsFalse(exists);
    }

    [TestMethod]
    public void Delete_ShouldNotThrowException_WhenRewardDoesNotExist()
    {
        _repository.Delete(999);

        Assert.IsTrue(true);
    }

    [TestMethod]
    public void Save_ShouldUpdateReward_WhenRewardAlreadyExists()
    {
        var reward = new Reward("Entrada VIP", "Acceso prioritario", 500, 10, NivelMembresia.Premium);
        _repository.Save(reward);

        reward.UpdateInfo("Descripción actualizada", 600, 15, NivelMembresia.Premium);
        _repository.Save(reward);

        var result = _repository.GetById(reward.Id);
        Assert.IsNotNull(result);
        Assert.AreEqual("Descripción actualizada", result.Descripcion);
        Assert.AreEqual(600, result.CostoPuntos);
        Assert.AreEqual(15, result.CantidadDisponible);
    }
}
