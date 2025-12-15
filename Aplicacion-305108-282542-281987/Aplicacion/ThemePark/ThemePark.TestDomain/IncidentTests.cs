using ThemePark.Entities;

namespace ThemePark.TestDomain;

[TestClass]
public class IncidentTests
{
    [TestMethod]
    public void Constructor_ShouldSetInputProperties_WhenValidParametersProvided()
    {
        var attractionName = "Montaña Rusa";
        var descripcion = "Falla en motor principal";
        var fechaCreacion = new DateTime(2025, 9, 29, 10, 30, 0);

        var incident = new Incident(attractionName, descripcion, fechaCreacion);

        Assert.AreEqual(attractionName, incident.AttractionName);
        Assert.AreEqual(descripcion, incident.Descripcion);
        Assert.AreEqual(fechaCreacion, incident.FechaCreacion);
    }

    [TestMethod]
    public void Constructor_ShouldSetDefaultValues_WhenValidParametersProvided()
    {
        var attractionName = "Montaña Rusa";
        var descripcion = "Falla en motor principal";
        var fechaCreacion = new DateTime(2025, 9, 29, 10, 30, 0);

        var incident = new Incident(attractionName, descripcion, fechaCreacion);

        Assert.AreNotEqual(Guid.Empty, incident.Id);
        Assert.IsNull(incident.FechaResolucion);
        Assert.IsTrue(incident.IsActive);
    }

    [TestMethod]
    public void Resolve_ShouldMarkIncidentAsResolved_WhenCalled()
    {
        var incident = new Incident("Montaña Rusa", "Falla en motor", DateTime.Now);
        var fechaResolucion = new DateTime(2025, 9, 29, 15, 0, 0);

        incident.Resolve(fechaResolucion);

        Assert.IsFalse(incident.IsActive);
        Assert.AreEqual(fechaResolucion, incident.FechaResolucion);
    }

    [TestMethod]
    public void Constructor_ShouldSetMaintenanceFieldsToNull_WhenCreatedWithoutMaintenance()
    {
        var attractionName = "Montaña Rusa";
        var descripcion = "Falla en motor principal";
        var fechaCreacion = new DateTime(2025, 9, 29, 10, 30, 0);

        var incident = new Incident(attractionName, descripcion, fechaCreacion);

        Assert.IsNull(incident.MaintenanceId);
        Assert.IsNull(incident.FechaProgramada);
        Assert.IsNull(incident.HoraProgramada);
    }

    [TestMethod]
    public void Constructor_ShouldSetMaintenanceFields_WhenCreatedForMaintenance()
    {
        var attractionName = "Montaña Rusa";
        var descripcion = "Mantenimiento preventivo motor";
        var fechaCreacion = new DateTime(2025, 9, 29, 10, 30, 0);
        var maintenanceId = Guid.NewGuid();
        var fechaProgramada = new DateTime(2025, 10, 15);
        var horaProgramada = new TimeSpan(14, 30, 0);

        var incident = new Incident(attractionName, descripcion, fechaCreacion, maintenanceId, fechaProgramada, horaProgramada);

        Assert.AreEqual(maintenanceId, incident.MaintenanceId);
        Assert.AreEqual(fechaProgramada, incident.FechaProgramada);
        Assert.AreEqual(horaProgramada, incident.HoraProgramada);
    }

    [TestMethod]
    public void IsMaintenanceIncident_ShouldReturnTrue_WhenIncidentHasMaintenanceId()
    {
        var maintenanceId = Guid.NewGuid();
        var incident = new Incident("Montaña Rusa", "Mantenimiento", DateTime.Now, maintenanceId, DateTime.Now.AddDays(1), new TimeSpan(10, 0, 0));

        var result = incident.IsMaintenanceIncident();

        Assert.IsTrue(result);
    }
}
