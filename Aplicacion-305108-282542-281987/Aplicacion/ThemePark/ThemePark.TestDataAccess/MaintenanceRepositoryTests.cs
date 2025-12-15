using Microsoft.EntityFrameworkCore;
using ThemePark.DataAccess;
using ThemePark.DataAccess.Repositories;
using ThemePark.Entities;

namespace ThemePark.TestDataAccess;

[TestClass]
public class MaintenanceRepositoryTests
{
    private ThemeParkDbContext _context = null!;
    private MaintenanceRepository _repository = null!;

    [TestInitialize]
    public void TestInitialize()
    {
        var options = new DbContextOptionsBuilder<ThemeParkDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ThemeParkDbContext(options);
        _repository = new MaintenanceRepository(_context);
    }

    [TestCleanup]
    public void TestCleanup()
    {
        _context.Dispose();
        _context = null!;
        _repository = null!;
    }

    [TestMethod]
    public void Save_ShouldReturnMaintenance_WhenValidMaintenanceProvided()
    {
        var maintenance = new Maintenance("Montaña Rusa", DateTime.Now.AddDays(1), new TimeSpan(10, 0, 0), 120, "Mantenimiento preventivo");

        var result = _repository.Save(maintenance);

        Assert.IsNotNull(result);
        Assert.AreEqual(maintenance.Id, result.Id);
        Assert.AreEqual(maintenance.AttractionName, result.AttractionName);
    }

    [TestMethod]
    public void GetByAttractionName_ShouldReturnMaintenances_WhenMaintenancesExist()
    {
        var attractionName = "Montaña Rusa";
        var maintenance1 = new Maintenance(attractionName, DateTime.Now.AddDays(1), new TimeSpan(10, 0, 0), 120, "Mantenimiento 1");
        var maintenance2 = new Maintenance(attractionName, DateTime.Now.AddDays(2), new TimeSpan(14, 0, 0), 90, "Mantenimiento 2");
        _repository.Save(maintenance1);
        _repository.Save(maintenance2);

        var result = _repository.GetByAttractionName(attractionName);

        Assert.AreEqual(2, result.Count);
        Assert.IsTrue(result.Any(m => m.Id == maintenance1.Id));
    }

    [TestMethod]
    public void GetById_ShouldReturnMaintenance_WhenMaintenanceExists()
    {
        var maintenance = new Maintenance("Montaña Rusa", DateTime.Now.AddDays(1), new TimeSpan(10, 0, 0), 120, "Mantenimiento");
        _repository.Save(maintenance);

        var result = _repository.GetById(maintenance.Id);

        Assert.IsNotNull(result);
        Assert.AreEqual(maintenance.Id, result.Id);
    }

    [TestMethod]
    public void Delete_ShouldRemoveMaintenance_WhenMaintenanceExists()
    {
        var maintenance = new Maintenance("Montaña Rusa", DateTime.Now.AddDays(1), new TimeSpan(10, 0, 0), 120, "Mantenimiento");
        _repository.Save(maintenance);

        _repository.Delete(maintenance.Id);

        var result = _repository.GetById(maintenance.Id);
        Assert.IsNull(result);
    }
}
