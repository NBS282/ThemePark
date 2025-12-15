using Microsoft.EntityFrameworkCore;
using ThemePark.DataAccess;
using ThemePark.DataAccess.Repositories;
using ThemePark.Entities;
using ThemePark.IDataAccess.Exceptions;

namespace ThemePark.TestDataAccess;

[TestClass]
public class IncidentRepositoryTests
{
    private ThemeParkDbContext _context = null!;
    private IncidentRepository _repository = null!;

    [TestInitialize]
    public void TestInitialize()
    {
        var options = new DbContextOptionsBuilder<ThemeParkDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ThemeParkDbContext(options);
        _repository = new IncidentRepository(_context);
    }

    [TestCleanup]
    public void TestCleanup()
    {
        _context.Dispose();
        _context = null!;
        _repository = null!;
    }

    [TestMethod]
    public void Save_ShouldReturnIncident_WhenValidIncidentProvided()
    {
        var incident = new Incident("Montaña Rusa", "Falla en motor", DateTime.Now);

        var result = _repository.Save(incident);

        Assert.IsNotNull(result);
        Assert.AreEqual(incident.Id, result.Id);
        Assert.AreEqual(incident.AttractionName, result.AttractionName);
    }

    [TestMethod]
    public void Save_ShouldReturnIncidentWithCorrectDescription_WhenValidIncidentProvided()
    {
        var incident = new Incident("Montaña Rusa", "Falla en motor", DateTime.Now);

        var result = _repository.Save(incident);

        Assert.AreEqual(incident.Descripcion, result.Descripcion);
        Assert.IsTrue(result.IsActive);
    }

    [TestMethod]
    public void GetById_ShouldReturnIncident_WhenIncidentExists()
    {
        var incident = new Incident("Montaña Rusa", "Falla en motor", DateTime.Now);
        _repository.Save(incident);

        var result = _repository.GetById(incident.Id);

        Assert.IsNotNull(result);
        Assert.AreEqual(incident.Id, result.Id);
        Assert.AreEqual(incident.AttractionName, result.AttractionName);
    }

    [TestMethod]
    [ExpectedException(typeof(IncidentNotFoundException))]
    public void GetById_ShouldThrowIncidentNotFoundException_WhenIncidentDoesNotExist()
    {
        var nonExistentId = Guid.NewGuid();

        _repository.GetById(nonExistentId);
    }

    [TestMethod]
    public void GetActiveByAttractionName_ShouldReturnActiveIncidents_WhenExist()
    {
        var incident1 = new Incident("Montaña Rusa", "Falla en motor", DateTime.Now);
        var incident2 = new Incident("Montaña Rusa", "Falla en frenos", DateTime.Now);
        var incident3 = new Incident("Torre de Caída", "Mantenimiento", DateTime.Now);
        _repository.Save(incident1);
        _repository.Save(incident2);
        _repository.Save(incident3);

        incident1.Resolve(DateTime.Now);
        _context.SaveChanges();

        var result = _repository.GetActiveByAttractionName("Montaña Rusa");

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual(incident2.Id, result[0].Id);
        Assert.IsTrue(result[0].IsActive);
    }

    [TestMethod]
    public void Update_ShouldUpdateIncident_WhenIncidentIsResolved()
    {
        var incident = new Incident("Montaña Rusa", "Falla en motor", DateTime.Now);
        _repository.Save(incident);

        incident.Resolve(new DateTime(2025, 9, 29, 15, 0, 0));
        var result = _repository.Update(incident);

        Assert.IsNotNull(result);
        Assert.IsFalse(result.IsActive);
        Assert.AreEqual(new DateTime(2025, 9, 29, 15, 0, 0), result.FechaResolucion);
    }

    [TestMethod]
    public void GetAll_ShouldReturnAllIncidents_WhenIncidentsExist()
    {
        var incident1 = new Incident("Montaña Rusa", "Falla en motor", new DateTime(2025, 10, 1));
        var incident2 = new Incident("Torre de Caída", "Problema eléctrico", new DateTime(2025, 10, 2));
        var incident3 = new Incident("Carrusel", "Mantenimiento preventivo", new DateTime(2025, 10, 3));
        _repository.Save(incident1);
        _repository.Save(incident2);
        _repository.Save(incident3);

        incident3.Resolve(new DateTime(2025, 10, 3, 15, 0, 0));
        _context.SaveChanges();

        var result = _repository.GetAll();

        Assert.AreEqual(3, result.Count);
        Assert.IsTrue(result.Any(i => i.AttractionName == "Montaña Rusa"));
        Assert.IsTrue(result.Any(i => i.AttractionName == "Torre de Caída"));
        Assert.IsTrue(result.Any(i => i.AttractionName == "Carrusel"));
    }

    [TestMethod]
    public void GetAll_ShouldReturnEmptyList_WhenNoIncidentsExist()
    {
        var result = _repository.GetAll();

        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.Count);
    }
}
