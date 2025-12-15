using Microsoft.EntityFrameworkCore;
using ThemePark.DataAccess;
using ThemePark.DataAccess.Repositories;
using ThemePark.Entities.Tickets;
using ThemePark.IDataAccess.Exceptions;

namespace ThemePark.TestDataAccess;

[TestClass]
public class TicketsRepositoryTests
{
    private ThemeParkDbContext _context = null!;
    private TicketsRepository _repository = null!;
    private GeneralTicket _testTicket = null!;

    [TestInitialize]
    public void TestInitialize()
    {
        var options = new DbContextOptionsBuilder<ThemeParkDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ThemeParkDbContext(options);
        _repository = new TicketsRepository(_context);

        _testTicket = new GeneralTicket
        {
            Id = Guid.NewGuid(),
            CodigoQR = Guid.NewGuid(),
            FechaVisita = DateTime.Now.AddDays(1),
            CodigoIdentificacionUsuario = "USER123",
            FechaCompra = DateTime.Now
        };
    }

    [TestCleanup]
    public void TestCleanup()
    {
        _context.Dispose();
        _context = null!;
        _repository = null!;
        _testTicket = null!;
    }

    [TestMethod]
    public void Create_ShouldReturnTicket_WhenValidTicketProvided()
    {
        var result = _repository.Create(_testTicket);

        Assert.IsNotNull(result);
        Assert.AreEqual(_testTicket.Id, result.Id);
        Assert.AreEqual(_testTicket.CodigoQR, result.CodigoQR);
    }

    [TestMethod]
    public void Create_ShouldReturnTicketWithCorrectUserCode_WhenValidTicketProvided()
    {
        var result = _repository.Create(_testTicket);

        Assert.AreEqual(_testTicket.CodigoIdentificacionUsuario, result.CodigoIdentificacionUsuario);
    }

    [TestMethod]
    public void Create_ShouldPersistTicketInDatabase_WhenValidTicketProvided()
    {
        var result = _repository.Create(_testTicket);

        var savedTicket = _context.Tickets.FirstOrDefault(t => t.Id == _testTicket.Id);
        Assert.IsNotNull(savedTicket);
        Assert.AreEqual(_testTicket.CodigoQR, savedTicket.CodigoQR);
    }

    [TestMethod]
    public void GetById_ShouldReturnTicket_WhenTicketExists()
    {
        _context.Tickets.Add(_testTicket);
        _context.SaveChanges();

        var result = _repository.GetById(_testTicket.Id);

        Assert.IsNotNull(result);
        Assert.AreEqual(_testTicket.Id, result.Id);
        Assert.AreEqual(_testTicket.CodigoQR, result.CodigoQR);
    }

    [TestMethod]
    public void GetById_ShouldReturnTicketWithCorrectUserCode_WhenTicketExists()
    {
        _context.Tickets.Add(_testTicket);
        _context.SaveChanges();

        var result = _repository.GetById(_testTicket.Id);

        Assert.AreEqual(_testTicket.CodigoIdentificacionUsuario, result.CodigoIdentificacionUsuario);
    }

    [TestMethod]
    [ExpectedException(typeof(TicketNotFoundException))]
    public void GetById_ShouldThrowException_WhenTicketNotFound()
    {
        var nonExistentId = Guid.NewGuid();

        _repository.GetById(nonExistentId);
    }

    [TestMethod]
    public void GetAll_ShouldReturnAllTickets_WhenTicketsExist()
    {
        var ticket2 = new EventTicket
        {
            Id = Guid.NewGuid(),
            CodigoQR = Guid.NewGuid(),
            FechaVisita = DateTime.Now.AddDays(2),
            CodigoIdentificacionUsuario = "USER456",
            FechaCompra = DateTime.Now,
            EventoId = Guid.NewGuid()
        };

        _context.Tickets.Add(_testTicket);
        _context.Tickets.Add(ticket2);
        _context.SaveChanges();

        var result = _repository.GetAll();

        Assert.IsNotNull(result);
        Assert.AreEqual(2, result.Count);
    }

    [TestMethod]
    public void GetAll_ShouldReturnAllTicketIds_WhenTicketsExist()
    {
        var ticket2 = new EventTicket
        {
            Id = Guid.NewGuid(),
            CodigoQR = Guid.NewGuid(),
            FechaVisita = DateTime.Now.AddDays(2),
            CodigoIdentificacionUsuario = "USER456",
            FechaCompra = DateTime.Now,
            EventoId = Guid.NewGuid()
        };

        _context.Tickets.Add(_testTicket);
        _context.Tickets.Add(ticket2);
        _context.SaveChanges();

        var result = _repository.GetAll();

        Assert.IsTrue(result.Any(t => t.Id == _testTicket.Id));
        Assert.IsTrue(result.Any(t => t.Id == ticket2.Id));
    }

    [TestMethod]
    public void GetByQRCode_ShouldReturnTicket_WhenQRCodeExists()
    {
        _context.Tickets.Add(_testTicket);
        _context.SaveChanges();

        var result = _repository.GetByQRCode(_testTicket.CodigoQR);

        Assert.IsNotNull(result);
        Assert.AreEqual(_testTicket.Id, result.Id);
        Assert.AreEqual(_testTicket.CodigoQR, result.CodigoQR);
    }

    [TestMethod]
    public void GetByQRCode_ShouldReturnTicketWithCorrectUserCode_WhenQRCodeExists()
    {
        _context.Tickets.Add(_testTicket);
        _context.SaveChanges();

        var result = _repository.GetByQRCode(_testTicket.CodigoQR);

        Assert.AreEqual(_testTicket.CodigoIdentificacionUsuario, result.CodigoIdentificacionUsuario);
    }

    [TestMethod]
    [ExpectedException(typeof(TicketNotFoundException))]
    public void GetByQRCode_ShouldThrowException_WhenQRCodeNotFound()
    {
        var nonExistentQR = Guid.NewGuid();

        _repository.GetByQRCode(nonExistentQR);
    }

    [TestMethod]
    public void GetByCodigoIdentificacionUsuario_ShouldReturnUserTickets_WhenUserHasTickets()
    {
        var codigoUsuario = "USER789";
        var ticket1 = new GeneralTicket
        {
            Id = Guid.NewGuid(),
            CodigoQR = Guid.NewGuid(),
            FechaVisita = DateTime.Now.AddDays(1),
            CodigoIdentificacionUsuario = codigoUsuario,
            FechaCompra = DateTime.Now
        };

        var ticket2 = new EventTicket
        {
            Id = Guid.NewGuid(),
            CodigoQR = Guid.NewGuid(),
            FechaVisita = DateTime.Now.AddDays(2),
            CodigoIdentificacionUsuario = codigoUsuario,
            FechaCompra = DateTime.Now,
            EventoId = Guid.NewGuid()
        };

        _context.Tickets.Add(ticket1);
        _context.Tickets.Add(ticket2);
        _context.Tickets.Add(_testTicket);
        _context.SaveChanges();

        var result = _repository.GetByCodigoIdentificacionUsuario(codigoUsuario);

        Assert.IsNotNull(result);
        Assert.AreEqual(2, result.Count);
        Assert.IsTrue(result.Any(t => t.Id == ticket1.Id));
    }

    [TestMethod]
    public void GetByCodigoIdentificacionUsuario_ShouldReturnOnlyUserTickets_WhenUserHasTickets()
    {
        var codigoUsuario = "USER789";
        var ticket1 = new GeneralTicket
        {
            Id = Guid.NewGuid(),
            CodigoQR = Guid.NewGuid(),
            FechaVisita = DateTime.Now.AddDays(1),
            CodigoIdentificacionUsuario = codigoUsuario,
            FechaCompra = DateTime.Now
        };

        var ticket2 = new EventTicket
        {
            Id = Guid.NewGuid(),
            CodigoQR = Guid.NewGuid(),
            FechaVisita = DateTime.Now.AddDays(2),
            CodigoIdentificacionUsuario = codigoUsuario,
            FechaCompra = DateTime.Now,
            EventoId = Guid.NewGuid()
        };

        _context.Tickets.Add(ticket1);
        _context.Tickets.Add(ticket2);
        _context.Tickets.Add(_testTicket);
        _context.SaveChanges();

        var result = _repository.GetByCodigoIdentificacionUsuario(codigoUsuario);

        Assert.IsTrue(result.Any(t => t.Id == ticket2.Id));
        Assert.IsFalse(result.Any(t => t.Id == _testTicket.Id));
    }

    [TestMethod]
    public void GetByCodigoIdentificacionUsuario_ShouldReturnEmptyList_WhenUserHasNoTickets()
    {
        var codigoUsuario = "NONEXISTENT";

        _context.Tickets.Add(_testTicket);
        _context.SaveChanges();

        var result = _repository.GetByCodigoIdentificacionUsuario(codigoUsuario);

        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public void CountTicketsByEventId_ShouldReturnCorrectCount_WhenEventTicketsExist()
    {
        var eventId = Guid.NewGuid();
        var eventTicket1 = new EventTicket
        {
            Id = Guid.NewGuid(),
            CodigoQR = Guid.NewGuid(),
            FechaVisita = DateTime.Now.AddDays(1),
            CodigoIdentificacionUsuario = "USER123",
            FechaCompra = DateTime.Now,
            EventoId = eventId
        };

        var eventTicket2 = new EventTicket
        {
            Id = Guid.NewGuid(),
            CodigoQR = Guid.NewGuid(),
            FechaVisita = DateTime.Now.AddDays(1),
            CodigoIdentificacionUsuario = "USER456",
            FechaCompra = DateTime.Now,
            EventoId = eventId
        };

        var differentEventTicket = new EventTicket
        {
            Id = Guid.NewGuid(),
            CodigoQR = Guid.NewGuid(),
            FechaVisita = DateTime.Now.AddDays(1),
            CodigoIdentificacionUsuario = "USER789",
            FechaCompra = DateTime.Now,
            EventoId = Guid.NewGuid()
        };

        _context.Tickets.Add(eventTicket1);
        _context.Tickets.Add(eventTicket2);
        _context.Tickets.Add(differentEventTicket);
        _context.Tickets.Add(_testTicket);
        _context.SaveChanges();

        var result = _repository.CountTicketsByEventId(eventId);

        Assert.AreEqual(2, result);
    }
}
