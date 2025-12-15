using ThemePark.Entities;
using ThemePark.Exceptions;

namespace ThemePark.TestDomain;

[TestClass]
public class EventTests
{
    private Event _event = null!;

    [TestInitialize]
    public void TestInitialize()
    {
        _event = new Event();
    }

    [TestCleanup]
    public void TestCleanup()
    {
        _event = null!;
    }

    [TestMethod]
    public void Event_ShouldInitializeWithEmptyId()
    {
        Assert.AreEqual(Guid.Empty, _event.Id);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidEventDataException))]
    public void Event_ShouldThrowArgumentException_WhenIdIsEmpty()
    {
        _event.Id = Guid.Empty;
    }

    [TestMethod]
    public void Event_ShouldSetValidId()
    {
        var validId = Guid.NewGuid();
        _event.Id = validId;
        Assert.AreEqual(validId, _event.Id);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidEventDataException))]
    public void Event_ShouldThrowArgumentException_WhenNameIsEmpty()
    {
        _event.Name = string.Empty;
    }

    [TestMethod]
    public void Event_ShouldSetValidName()
    {
        _event.Name = "Concierto de Verano";
        Assert.AreEqual("Concierto de Verano", _event.Name);
    }

    [TestMethod]
    public void Event_ShouldSetValidFecha()
    {
        var validDate = new DateTime(2025, 12, 25);
        _event.Fecha = validDate;
        Assert.AreEqual(validDate, _event.Fecha);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidEventDataException))]
    public void Event_ShouldThrowArgumentException_WhenFechaIsInPast()
    {
        _event.Fecha = DateTime.Now.AddDays(-1);
    }

    [TestMethod]
    public void Event_ShouldSetValidHora()
    {
        var validTime = new TimeSpan(14, 30, 0);
        _event.Hora = validTime;
        Assert.AreEqual(validTime, _event.Hora);
    }

    [TestMethod]
    public void Event_ShouldSetValidAforo()
    {
        _event.Aforo = 500;
        Assert.AreEqual(500, _event.Aforo);
    }

    [TestMethod]
    public void Event_ShouldSetValidCostoAdicional()
    {
        _event.CostoAdicional = 25.50m;
        Assert.AreEqual(25.50m, _event.CostoAdicional);
    }

    [TestMethod]
    public void Event_ShouldSetValidAtracciones()
    {
        var atracciones = new List<Attraction>();

        _event.Atracciones = atracciones;
        Assert.IsNotNull(_event.Atracciones);
    }
}
