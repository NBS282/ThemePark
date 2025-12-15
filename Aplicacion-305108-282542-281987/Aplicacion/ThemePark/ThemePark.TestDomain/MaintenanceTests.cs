using ThemePark.Entities;
using ThemePark.Exceptions;

namespace ThemePark.TestDomain;

[TestClass]
public class MaintenanceTests
{
    [TestMethod]
    public void Constructor_ShouldSetBasicProperties_WhenValidParametersProvided()
    {
        var attractionName = "Montaña Rusa";
        var fecha = DateTime.Now;
        var horaInicio = new TimeSpan(10, 0, 0);
        var duracionMinutos = 120;
        var descripcion = "Mantenimiento preventivo del motor";
        var incidentId = Guid.NewGuid();

        var maintenance = new Maintenance(attractionName, fecha, horaInicio, duracionMinutos, descripcion, incidentId);

        Assert.AreEqual(attractionName, maintenance.AttractionName);
        Assert.AreEqual(fecha, maintenance.Fecha);
        Assert.AreEqual(horaInicio, maintenance.HoraInicio);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidMaintenanceDataException))]
    public void Constructor_ShouldThrowInvalidRequestDataException_WhenAttractionNameIsEmpty()
    {
        new Maintenance(string.Empty, DateTime.Now, new TimeSpan(10, 0, 0), 120, "Descripcion", Guid.NewGuid());
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidMaintenanceDataException))]
    public void Constructor_ShouldThrowInvalidRequestDataException_WhenDescripcionIsEmpty()
    {
        new Maintenance("Montaña Rusa", DateTime.Now, new TimeSpan(10, 0, 0), 120, string.Empty, Guid.NewGuid());
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidMaintenanceDataException))]
    public void Constructor_ShouldThrowInvalidMaintenanceDataException_WhenDuracionMinutosIsZero()
    {
        new Maintenance("Montaña Rusa", DateTime.Now, new TimeSpan(10, 0, 0), 0, "Descripcion", Guid.NewGuid());
    }

    [TestMethod]
    public void Constructor_ShouldGenerateId_WhenValidParametersProvided()
    {
        var maintenance = new Maintenance("Montaña Rusa", DateTime.Now, new TimeSpan(10, 0, 0), 120, "Descripcion", Guid.NewGuid());

        Assert.AreNotEqual(Guid.Empty, maintenance.Id);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidMaintenanceDataException))]
    public void Constructor_ShouldThrowInvalidMaintenanceDataException_WhenFechaIsInPast()
    {
        var fechaPasada = DateTime.Now.Date.AddDays(-1);
        new Maintenance("Montaña Rusa", fechaPasada, new TimeSpan(10, 0, 0), 120, "Descripcion", Guid.NewGuid());
    }
}
