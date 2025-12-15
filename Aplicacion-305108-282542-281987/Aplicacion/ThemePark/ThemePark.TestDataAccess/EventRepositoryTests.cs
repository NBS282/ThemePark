using Microsoft.EntityFrameworkCore;
using ThemePark.DataAccess;
using ThemePark.DataAccess.Repositories;
using ThemePark.Entities;
using ThemePark.IDataAccess;
using ThemePark.IDataAccess.Exceptions;

namespace ThemePark.TestDataAccess;

[TestClass]
public class EventRepositoryTests
{
    private ThemeParkDbContext _context = null!;
    private IEventRepository _repository = null!;
    private Event _testEvent = null!;

    [TestInitialize]
    public void TestInitialize()
    {
        var options = new DbContextOptionsBuilder<ThemeParkDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ThemeParkDbContext(options);
        _repository = new EventRepository(_context);

        _testEvent = new Event
        {
            Id = Guid.NewGuid(),
            Name = "Evento Test",
            Fecha = DateTime.Today.AddDays(1),
            Hora = new TimeSpan(20, 0, 0),
            Aforo = 100,
            CostoAdicional = 50.0m
        };
    }

    [TestCleanup]
    public void TestCleanup()
    {
        _context.Dispose();
    }

    [TestMethod]
    public void Add_ShouldAddEventToDatabase()
    {
        var result = _repository.Add(_testEvent);

        Assert.AreEqual(_testEvent, result);
        var savedEvent = _context.Events.Find(_testEvent.Id);
        Assert.AreEqual(_testEvent.Name, savedEvent?.Name);
    }

    [TestMethod]
    public void GetAll_ShouldReturnAllEventsFromDatabase()
    {
        _context.Events.Add(_testEvent);
        _context.SaveChanges();

        var result = _repository.GetAll();

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual(_testEvent.Name, result[0].Name);
    }

    [TestMethod]
    public void GetById_ShouldReturnSpecificEventFromDatabase()
    {
        _context.Events.Add(_testEvent);
        _context.SaveChanges();

        var result = _repository.GetById(_testEvent.Id);

        Assert.AreEqual(_testEvent.Id, result.Id);
        Assert.AreEqual(_testEvent.Name, result.Name);
    }

    [TestMethod]
    [ExpectedException(typeof(EventNotFoundException))]
    public void GetById_ShouldThrowKeyNotFoundException_WhenEventDoesNotExist()
    {
        var nonExistentId = Guid.NewGuid();

        _repository.GetById(nonExistentId);
    }

    [TestMethod]
    public void ExistsByName_ShouldReturnTrue_WhenEventNameExists()
    {
        _context.Events.Add(_testEvent);
        _context.SaveChanges();

        var result = _repository.ExistsByName(_testEvent.Name);

        Assert.IsTrue(result);
    }

    [TestMethod]
    public void ExistsByName_ShouldReturnFalse_WhenEventNameDoesNotExist()
    {
        var result = _repository.ExistsByName("Evento Inexistente");

        Assert.IsFalse(result);
    }

    [TestMethod]
    public void Delete_ShouldRemoveEventFromDatabase()
    {
        _context.Events.Add(_testEvent);
        _context.SaveChanges();

        _repository.Delete(_testEvent.Id);

        var deletedEvent = _context.Events.Find(_testEvent.Id);
        Assert.IsNull(deletedEvent);
    }

    [TestMethod]
    [ExpectedException(typeof(EventNotFoundException))]
    public void Delete_ShouldThrowKeyNotFoundException_WhenEventDoesNotExist()
    {
        var nonExistentId = Guid.NewGuid();

        _repository.Delete(nonExistentId);
    }

    [TestMethod]
    public void Update_ShouldUpdateExistingEventInDatabase()
    {
        _context.Events.Add(_testEvent);
        _context.SaveChanges();

        _testEvent.Name = "Evento Actualizado";
        _testEvent.Aforo = 200;

        var result = _repository.Update(_testEvent);

        Assert.AreEqual(_testEvent.Name, result.Name);
        Assert.AreEqual(_testEvent.Aforo, result.Aforo);
    }

    [TestMethod]
    public void Update_ShouldPersistChangesInDatabase()
    {
        _context.Events.Add(_testEvent);
        _context.SaveChanges();

        _testEvent.Name = "Evento Actualizado";
        _testEvent.Aforo = 200;
        _ = _repository.Update(_testEvent);

        var updatedEvent = _context.Events.Find(_testEvent.Id);
        Assert.AreEqual("Evento Actualizado", updatedEvent?.Name);
        Assert.AreEqual(200, updatedEvent?.Aforo);
    }

    [TestMethod]
    public void Exists_ShouldReturnTrue_WhenEventExists()
    {
        _context.Events.Add(_testEvent);
        _context.SaveChanges();

        var result = _repository.Exists(_testEvent.Id);

        Assert.IsTrue(result);
    }
}
