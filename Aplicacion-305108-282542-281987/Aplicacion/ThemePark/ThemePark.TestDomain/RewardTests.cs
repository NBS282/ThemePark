using ThemePark.Entities;
using ThemePark.Enums;
using ThemePark.Exceptions;

namespace ThemePark.TestDomain;

[TestClass]
public class RewardTests
{
    [TestMethod]
    public void Reward_Constructor_ShouldCreateReward_WhenValidParametersProvided()
    {
        var nombre = "Entrada VIP";
        var descripcion = "Acceso prioritario a todas las atracciones";
        var costoPuntos = 500;
        var cantidadDisponible = 10;
        NivelMembresia? nivelMembresiaRequerido = NivelMembresia.Premium;

        var reward = new Reward(nombre, descripcion, costoPuntos, cantidadDisponible, nivelMembresiaRequerido);

        Assert.AreEqual(nombre, reward.Nombre);
        Assert.AreEqual(descripcion, reward.Descripcion);
        Assert.AreEqual(costoPuntos, reward.CostoPuntos);
        Assert.AreEqual(cantidadDisponible, reward.CantidadDisponible);
        Assert.AreEqual(nivelMembresiaRequerido, reward.NivelMembresiaRequerido);
        Assert.IsTrue(reward.Activa);
        Assert.IsTrue((DateTime.Now - reward.FechaCreacion).TotalSeconds < 2);
    }

    [TestMethod]
    public void Reward_Constructor_ShouldThrowException_WhenNombreIsEmpty()
    {
        var nombre = string.Empty;
        var descripcion = "Descripción válida";
        var costoPuntos = 500;
        var cantidadDisponible = 10;
        NivelMembresia? nivelMembresiaRequerido = null;

        Assert.ThrowsException<InvalidRequestDataException>(() =>
            new Reward(nombre, descripcion, costoPuntos, cantidadDisponible, nivelMembresiaRequerido));
    }

    [TestMethod]
    public void Reward_Constructor_ShouldThrowException_WhenDescripcionIsEmpty()
    {
        var nombre = "Entrada VIP";
        var descripcion = string.Empty;
        var costoPuntos = 500;
        var cantidadDisponible = 10;
        NivelMembresia? nivelMembresiaRequerido = null;

        Assert.ThrowsException<InvalidRequestDataException>(() =>
            new Reward(nombre, descripcion, costoPuntos, cantidadDisponible, nivelMembresiaRequerido));
    }

    [TestMethod]
    public void Reward_Constructor_ShouldThrowException_WhenCostoPuntosIsZeroOrNegative()
    {
        var nombre = "Entrada VIP";
        var descripcion = "Descripción válida";
        var costoPuntos = 0;
        var cantidadDisponible = 10;
        NivelMembresia? nivelMembresiaRequerido = null;

        Assert.ThrowsException<InvalidRequestDataException>(() =>
            new Reward(nombre, descripcion, costoPuntos, cantidadDisponible, nivelMembresiaRequerido));
    }

    [TestMethod]
    public void Reward_Constructor_ShouldThrowException_WhenCantidadDisponibleIsNegative()
    {
        var nombre = "Entrada VIP";
        var descripcion = "Descripción válida";
        var costoPuntos = 500;
        var cantidadDisponible = -1;
        NivelMembresia? nivelMembresiaRequerido = null;

        Assert.ThrowsException<InvalidRequestDataException>(() =>
            new Reward(nombre, descripcion, costoPuntos, cantidadDisponible, nivelMembresiaRequerido));
    }

    [TestMethod]
    public void Reward_DecrementarStock_ShouldDecreaseQuantity_WhenStockAvailable()
    {
        var reward = new Reward("Entrada VIP", "Descripción", 500, 10, null);

        reward.DecrementarStock();

        Assert.AreEqual(9, reward.CantidadDisponible);
    }

    [TestMethod]
    public void Reward_DecrementarStock_ShouldThrowException_WhenNoStockAvailable()
    {
        var reward = new Reward("Entrada VIP", "Descripción", 500, 0, null);

        Assert.ThrowsException<InvalidRequestDataException>(reward.DecrementarStock);
    }

    [TestMethod]
    public void Reward_UpdateInfo_ShouldUpdateProperties_WhenValidDataProvided()
    {
        var reward = new Reward("Entrada VIP", "Descripción original", 500, 10, NivelMembresia.Estándar);

        reward.UpdateInfo("Nueva descripción", 600, 20, NivelMembresia.Premium);

        Assert.AreEqual("Nueva descripción", reward.Descripcion);
        Assert.AreEqual(600, reward.CostoPuntos);
        Assert.AreEqual(20, reward.CantidadDisponible);
        Assert.AreEqual(NivelMembresia.Premium, reward.NivelMembresiaRequerido);
    }

    [TestMethod]
    public void Reward_Desactivar_ShouldSetActivaToFalse()
    {
        var reward = new Reward("Entrada VIP", "Descripción", 500, 10, null);

        reward.Desactivar();

        Assert.IsFalse(reward.Activa);
    }
}
