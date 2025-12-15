using Moq;
using ThemePark.BusinessLogic;
using ThemePark.Entities.Tickets;
using ThemePark.Exceptions;
using ThemePark.IBusinessLogic.Exceptions;
using ThemePark.IDataAccess;

namespace ThemePark.TestBusinessLogic;

[TestClass]
public class TicketsBusinessLogicTests
{
    private Mock<ITicketsRepository> _mockTicketsRepository = null!;
    private Mock<IUserRepository> _mockUserRepository = null!;
    private Mock<IEventRepository> _mockEventRepository = null!;
    private Mock<ISessionRepository> _mockSessionRepository = null!;
    private TicketsBusinessLogic _ticketsBusinessLogic = null!;
    private string _fechaVisita = null!;
    private GeneralTicket _expectedTicket = null!;
    private string _codigoIdentificacionUsuario = "USER123";

    [TestInitialize]
    public void TestInitialize()
    {
        _mockTicketsRepository = new Mock<ITicketsRepository>(MockBehavior.Strict);
        _mockUserRepository = new Mock<IUserRepository>(MockBehavior.Strict);
        _mockEventRepository = new Mock<IEventRepository>(MockBehavior.Strict);
        _mockSessionRepository = new Mock<ISessionRepository>(MockBehavior.Strict);
        _ticketsBusinessLogic = new TicketsBusinessLogic(_mockTicketsRepository.Object, _mockUserRepository.Object, _mockEventRepository.Object, _mockSessionRepository.Object);

        _fechaVisita = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd");
        _codigoIdentificacionUsuario = "USER123";

        _expectedTicket = new GeneralTicket
        {
            Id = Guid.NewGuid(),
            CodigoQR = Guid.NewGuid(),
            FechaVisita = DateTime.Parse(_fechaVisita),
            CodigoIdentificacionUsuario = _codigoIdentificacionUsuario,
            FechaCompra = DateTime.Now
        };
    }

    [TestMethod]
    public void CreateTicket_ShouldReturnTicket_WhenValidTicket()
    {
        var inputTicket = new GeneralTicket
        {
            Id = Guid.NewGuid(),
            CodigoQR = Guid.NewGuid(),
            FechaVisita = DateTime.Parse(_fechaVisita),
            CodigoIdentificacionUsuario = _codigoIdentificacionUsuario,
            FechaCompra = DateTime.Now
        };

        _mockUserRepository.Setup(r => r.ExistsByCodigoIdentificacion(inputTicket.CodigoIdentificacionUsuario))
            .Returns(true);
        _mockTicketsRepository.Setup(r => r.Create(inputTicket))
            .Returns(_expectedTicket);

        var result = _ticketsBusinessLogic.CreateTicket(inputTicket);

        Assert.AreEqual(_expectedTicket.Id, result.Id);
        Assert.AreEqual(_expectedTicket.CodigoQR, result.CodigoQR);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidTicketException))]
    public void CreateTicket_ShouldThrowInvalidTicketException_WhenTicketIsNull()
    {
        _ticketsBusinessLogic.CreateTicket(null!);
    }

    [TestMethod]
    public void GetTicketById_ShouldReturnTicket_WhenValidId()
    {
        var ticketId = Guid.NewGuid();

        _mockTicketsRepository.Setup(r => r.GetById(ticketId))
            .Returns(_expectedTicket);

        var result = _ticketsBusinessLogic.GetTicketById(ticketId);

        Assert.IsNotNull(result);
        Assert.AreEqual(_expectedTicket.Id, result.Id);
        _mockTicketsRepository.Verify(r => r.GetById(ticketId), Times.Once);
    }

    [TestMethod]
    public void GetAllTickets_ShouldReturnValidListOfTickets()
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

        var expectedTickets = new List<Ticket> { _expectedTicket, ticket2 };

        _mockTicketsRepository.Setup(r => r.GetAll())
            .Returns(expectedTickets);

        var result = _ticketsBusinessLogic.GetAllTickets();

        Assert.IsNotNull(result);
        Assert.AreEqual(2, result.Count);
        _mockTicketsRepository.Verify(r => r.GetAll(), Times.Once);
    }

    [TestMethod]
    public void GetAllTickets_ShouldReturnTicketsWithCorrectIds()
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

        var expectedTickets = new List<Ticket> { _expectedTicket, ticket2 };

        _mockTicketsRepository.Setup(r => r.GetAll())
            .Returns(expectedTickets);

        var result = _ticketsBusinessLogic.GetAllTickets();

        Assert.AreEqual(_expectedTicket.Id, result[0].Id);
        Assert.AreEqual(ticket2.Id, result[1].Id);
    }

    [TestMethod]
    [ExpectedException(typeof(BusinessLogicException))]
    public void CreateTicket_ShouldThrowBusinessLogicException_WhenUserDoesNotExist()
    {
        var inputTicket = new GeneralTicket
        {
            Id = Guid.NewGuid(),
            CodigoQR = Guid.NewGuid(),
            FechaVisita = DateTime.Parse(_fechaVisita),
            CodigoIdentificacionUsuario = _codigoIdentificacionUsuario,
            FechaCompra = DateTime.Now
        };

        _mockUserRepository.Setup(r => r.ExistsByCodigoIdentificacion(_codigoIdentificacionUsuario))
            .Returns(false);

        _ticketsBusinessLogic.CreateTicket(inputTicket);
    }

    [TestMethod]
    [ExpectedException(typeof(BusinessLogicException))]
    public void CreateTicket_ShouldThrowBusinessLogicException_WhenEventDoesNotExist()
    {
        var eventoId = Guid.NewGuid();
        var inputTicket = new EventTicket
        {
            Id = Guid.NewGuid(),
            CodigoQR = Guid.NewGuid(),
            FechaVisita = DateTime.Parse(_fechaVisita),
            CodigoIdentificacionUsuario = _codigoIdentificacionUsuario,
            FechaCompra = DateTime.Now,
            EventoId = eventoId
        };

        _mockUserRepository.Setup(r => r.ExistsByCodigoIdentificacion(_codigoIdentificacionUsuario))
            .Returns(true);
        _mockEventRepository.Setup(r => r.Exists(eventoId))
            .Returns(false);

        _ticketsBusinessLogic.CreateTicket(inputTicket);
    }

    [TestMethod]
    [ExpectedException(typeof(EventCapacityExceededException))]
    public void CreateTicket_ShouldThrowEventCapacityExceededException_WhenEventIsFull()
    {
        var eventoId = Guid.NewGuid();
        var aforo = 2;
        var evento = new Entities.Event
        {
            Id = eventoId,
            Name = "Evento Test",
            Fecha = DateTime.Now.AddDays(1),
            Hora = TimeSpan.FromHours(18),
            Aforo = aforo,
            CostoAdicional = 10.0m
        };

        var inputTicket = new EventTicket
        {
            Id = Guid.NewGuid(),
            CodigoQR = Guid.NewGuid(),
            FechaVisita = DateTime.Parse(_fechaVisita),
            CodigoIdentificacionUsuario = _codigoIdentificacionUsuario,
            FechaCompra = DateTime.Now,
            EventoId = eventoId
        };

        _mockUserRepository.Setup(r => r.ExistsByCodigoIdentificacion(_codigoIdentificacionUsuario))
            .Returns(true);
        _mockEventRepository.Setup(r => r.Exists(eventoId))
            .Returns(true);
        _mockEventRepository.Setup(r => r.GetById(eventoId))
            .Returns(evento);
        _mockTicketsRepository.Setup(r => r.CountTicketsByEventId(eventoId))
            .Returns(aforo);

        _ticketsBusinessLogic.CreateTicket(inputTicket);
    }

    [TestMethod]
    public void GetMyTickets_ShouldReturnUserTickets()
    {
        var userId = Guid.NewGuid();
        var user = new ThemePark.Entities.User
        {
            Id = userId,
            CodigoIdentificacion = _codigoIdentificacionUsuario,
            Nombre = "Test",
            Apellido = "User",
            Email = "test@test.com",
            Contraseña = "pass123"
        };

        var tickets = new List<Ticket> { _expectedTicket };

        _mockSessionRepository.Setup(r => r.GetCurrentUserId()).Returns(userId);
        _mockUserRepository.Setup(r => r.GetById(userId)).Returns(user);
        _mockTicketsRepository.Setup(r => r.GetByCodigoIdentificacionUsuario(_codigoIdentificacionUsuario))
            .Returns(tickets);

        var result = _ticketsBusinessLogic.GetMyTickets();

        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Count);
        Assert.AreEqual(_expectedTicket.Id, result[0].Id);
    }
}
