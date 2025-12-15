using Microsoft.EntityFrameworkCore;
using ThemePark.DataAccess;
using ThemePark.DataAccess.Repositories;
using ThemePark.Entities;
using ThemePark.IDataAccess.Exceptions;

namespace ThemePark.TestDataAccess;

[TestClass]
public class VisitRepositoryTests
{
    private ThemeParkDbContext _context = null!;
    private VisitRepository _repository = null!;
    private Visit _testVisit = null!;

    [TestInitialize]
    public void TestInitialize()
    {
        var options = new DbContextOptionsBuilder<ThemeParkDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ThemeParkDbContext(options);
        _repository = new VisitRepository(_context);

        _testVisit = new Visit(Guid.NewGuid(), "Montaña Rusa", DateTime.Now);
    }

    [TestCleanup]
    public void TestCleanup()
    {
        _context.Dispose();
        _context = null!;
        _repository = null!;
        _testVisit = null!;
    }

    [TestMethod]
    public void Save_ShouldReturnVisit_WhenValidVisitProvided()
    {
        var result = _repository.Save(_testVisit);

        Assert.IsNotNull(result);
        Assert.AreEqual(_testVisit.Id, result.Id);
        Assert.AreEqual(_testVisit.UserId, result.UserId);
    }

    [TestMethod]
    public void Save_ShouldReturnVisitWithCorrectAttractionAndTime_WhenValidVisitProvided()
    {
        var result = _repository.Save(_testVisit);

        Assert.AreEqual(_testVisit.AttractionName, result.AttractionName);
        Assert.AreEqual(_testVisit.EntryTime, result.EntryTime);
        Assert.IsTrue(result.IsActive);
    }

    [TestMethod]
    public void GetById_ShouldReturnVisit_WhenVisitExists()
    {
        _repository.Save(_testVisit);

        var result = _repository.GetById(_testVisit.Id);

        Assert.IsNotNull(result);
        Assert.AreEqual(_testVisit.Id, result.Id);
        Assert.AreEqual(_testVisit.UserId, result.UserId);
    }

    [TestMethod]
    [ExpectedException(typeof(VisitNotFoundException))]
    public void GetById_ShouldThrowException_WhenVisitNotFound()
    {
        var nonExistentId = Guid.NewGuid();

        _repository.GetById(nonExistentId);
    }

    [TestMethod]
    public void Update_ShouldReturnUpdatedVisit_WhenValidVisitProvided()
    {
        _repository.Save(_testVisit);
        var exitTime = DateTime.Now.AddHours(1);
        _testVisit.MarkExit(exitTime);

        var result = _repository.Update(_testVisit);

        Assert.IsNotNull(result);
        Assert.AreEqual(_testVisit.Id, result.Id);
        Assert.AreEqual(exitTime, result.ExitTime);
    }

    [TestMethod]
    public void Update_ShouldMarkVisitAsInactive_WhenValidVisitProvided()
    {
        _repository.Save(_testVisit);
        var exitTime = DateTime.Now.AddHours(1);
        _testVisit.MarkExit(exitTime);

        var result = _repository.Update(_testVisit);

        Assert.IsFalse(result.IsActive);
    }

    [TestMethod]
    public void GetByUserId_ShouldReturnVisits_WhenUserHasVisits()
    {
        var visit2 = new Visit(_testVisit.UserId, "Rueda de la Fortuna", DateTime.Now.AddMinutes(30));
        _repository.Save(_testVisit);
        _repository.Save(visit2);

        var result = _repository.GetByUserId(_testVisit.UserId);

        Assert.AreEqual(2, result.Count);
        Assert.IsTrue(result.Any(v => v.AttractionName == "Montaña Rusa"));
        Assert.IsTrue(result.Any(v => v.AttractionName == "Rueda de la Fortuna"));
    }

    [TestMethod]
    public void GetActiveVisitsByAttraction_ShouldReturnActiveVisits_WhenAttractionHasActiveVisits()
    {
        var visit2 = new Visit(Guid.NewGuid(), "Montaña Rusa", DateTime.Now.AddMinutes(15));
        _repository.Save(_testVisit);
        _repository.Save(visit2);

        var result = _repository.GetActiveVisitsByAttraction("Montaña Rusa");

        Assert.AreEqual(2, result.Count);
        Assert.IsTrue(result.All(v => v.IsActive));
        Assert.IsTrue(result.All(v => v.AttractionName == "Montaña Rusa"));
    }

    [TestMethod]
    public void GetActiveVisitByUserAndAttraction_ShouldReturnVisit_WhenActiveVisitExists()
    {
        _repository.Save(_testVisit);

        var result = _repository.GetActiveVisitByUserAndAttraction(_testVisit.UserId, "Montaña Rusa");

        Assert.IsNotNull(result);
        Assert.AreEqual(_testVisit.Id, result.Id);
        Assert.IsTrue(result.IsActive);
    }

    [TestMethod]
    public void GetVisitsByDate_ShouldReturnVisits_WhenVisitsExistForDate()
    {
        var today = DateTime.Today;
        var visit1 = new Visit(Guid.NewGuid(), "Montaña Rusa", today.AddHours(10));
        var visit2 = new Visit(Guid.NewGuid(), "Carrusel", today.AddHours(14));
        var visitOtherDay = new Visit(Guid.NewGuid(), "Simulador", today.AddDays(1).AddHours(12));

        _repository.Save(visit1);
        _repository.Save(visit2);
        _repository.Save(visitOtherDay);

        var result = _repository.GetVisitsByDate(today);

        Assert.AreEqual(2, result.Count);
        Assert.IsTrue(result.Any(v => v.AttractionName == "Montaña Rusa"));
        Assert.IsTrue(result.Any(v => v.AttractionName == "Carrusel"));
    }

    [TestMethod]
    public void GetVisitsByDate_ShouldExcludeOtherDays_WhenVisitsExistForDate()
    {
        var today = DateTime.Today;
        var visit1 = new Visit(Guid.NewGuid(), "Montaña Rusa", today.AddHours(10));
        var visit2 = new Visit(Guid.NewGuid(), "Carrusel", today.AddHours(14));
        var visitOtherDay = new Visit(Guid.NewGuid(), "Simulador", today.AddDays(1).AddHours(12));

        _repository.Save(visit1);
        _repository.Save(visit2);
        _repository.Save(visitOtherDay);

        var result = _repository.GetVisitsByDate(today);

        Assert.IsFalse(result.Any(v => v.AttractionName == "Simulador"));
    }

    [TestMethod]
    public void GetVisitsByDateRange_ShouldReturnVisitsInRange_WhenVisitsExist()
    {
        var startDate = new DateTime(2025, 9, 1);
        var endDate = new DateTime(2025, 9, 30);

        var visit1 = new Visit(Guid.NewGuid(), "Montaña Rusa", new DateTime(2025, 9, 5));
        var visit2 = new Visit(Guid.NewGuid(), "Carrusel", new DateTime(2025, 9, 15));
        var visit3 = new Visit(Guid.NewGuid(), "Simulador", new DateTime(2025, 9, 25));
        var visitOutOfRange = new Visit(Guid.NewGuid(), "Torre", new DateTime(2025, 10, 5));

        _repository.Save(visit1);
        _repository.Save(visit2);
        _repository.Save(visit3);
        _repository.Save(visitOutOfRange);

        var result = _repository.GetVisitsByDateRange(startDate, endDate);

        Assert.AreEqual(3, result.Count);
        Assert.IsTrue(result.Any(v => v.AttractionName == "Montaña Rusa"));
        Assert.IsTrue(result.Any(v => v.AttractionName == "Carrusel"));
    }

    [TestMethod]
    public void GetVisitsByDateRange_ShouldIncludeAllRangeVisits_WhenVisitsExist()
    {
        var startDate = new DateTime(2025, 9, 1);
        var endDate = new DateTime(2025, 9, 30);

        var visit1 = new Visit(Guid.NewGuid(), "Montaña Rusa", new DateTime(2025, 9, 5));
        var visit2 = new Visit(Guid.NewGuid(), "Carrusel", new DateTime(2025, 9, 15));
        var visit3 = new Visit(Guid.NewGuid(), "Simulador", new DateTime(2025, 9, 25));
        var visitOutOfRange = new Visit(Guid.NewGuid(), "Torre", new DateTime(2025, 10, 5));

        _repository.Save(visit1);
        _repository.Save(visit2);
        _repository.Save(visit3);
        _repository.Save(visitOutOfRange);

        var result = _repository.GetVisitsByDateRange(startDate, endDate);

        Assert.IsTrue(result.Any(v => v.AttractionName == "Simulador"));
        Assert.IsFalse(result.Any(v => v.AttractionName == "Torre"));
    }
}
