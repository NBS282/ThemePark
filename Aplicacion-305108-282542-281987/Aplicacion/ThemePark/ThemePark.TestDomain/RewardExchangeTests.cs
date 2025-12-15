using ThemePark.Entities;
using ThemePark.Exceptions;

namespace ThemePark.TestDomain;

[TestClass]
public class RewardExchangeTests
{
    [TestMethod]
    public void RewardExchange_Constructor_ShouldCreate_WhenValidParametersProvided()
    {
        var rewardId = 1;
        var userId = Guid.NewGuid();
        var puntosDescontados = 500;
        var puntosRestantesUsuario = 1000;

        var exchange = new RewardExchange(rewardId, userId, puntosDescontados, puntosRestantesUsuario);

        Assert.AreEqual(rewardId, exchange.RewardId);
        Assert.AreEqual(userId, exchange.UserId);
        Assert.AreEqual(puntosDescontados, exchange.PuntosDescontados);
        Assert.AreEqual(puntosRestantesUsuario, exchange.PuntosRestantesUsuario);
        Assert.AreEqual("Confirmado", exchange.Estado);
        Assert.IsTrue((DateTime.Now - exchange.FechaCanje).TotalSeconds < 2);
    }

    [TestMethod]
    public void RewardExchange_Constructor_ShouldThrowException_WhenRewardIdIsInvalid()
    {
        var rewardId = 0;
        var userId = Guid.NewGuid();
        var puntosDescontados = 500;
        var puntosRestantesUsuario = 1000;

        Assert.ThrowsException<InvalidRequestDataException>(() =>
            new RewardExchange(rewardId, userId, puntosDescontados, puntosRestantesUsuario));
    }

    [TestMethod]
    public void RewardExchange_Constructor_ShouldThrowException_WhenUserIdIsEmpty()
    {
        var rewardId = 1;
        var userId = Guid.Empty;
        var puntosDescontados = 500;
        var puntosRestantesUsuario = 1000;

        Assert.ThrowsException<InvalidRequestDataException>(() =>
            new RewardExchange(rewardId, userId, puntosDescontados, puntosRestantesUsuario));
    }

    [TestMethod]
    public void RewardExchange_Constructor_ShouldThrowException_WhenPuntosDescontadosIsInvalid()
    {
        var rewardId = 1;
        var userId = Guid.NewGuid();
        var puntosDescontados = 0;
        var puntosRestantesUsuario = 1000;

        Assert.ThrowsException<InvalidRequestDataException>(() =>
            new RewardExchange(rewardId, userId, puntosDescontados, puntosRestantesUsuario));
    }

    [TestMethod]
    public void RewardExchange_Constructor_ShouldThrowException_WhenPuntosRestantesIsNegative()
    {
        var rewardId = 1;
        var userId = Guid.NewGuid();
        var puntosDescontados = 500;
        var puntosRestantesUsuario = -1;

        Assert.ThrowsException<InvalidRequestDataException>(() =>
            new RewardExchange(rewardId, userId, puntosDescontados, puntosRestantesUsuario));
    }
}
