using Moq;
using ThemePark.BusinessLogic.Attractions;
using ThemePark.Enums;
using ThemePark.IBusinessLogic;
using ThemePark.IBusinessLogic.Exceptions;
using ThemePark.IDataAccess;
using ThemePark.IDataAccess.Exceptions;
using AttractionEntity = ThemePark.Entities.Attraction;

namespace ThemePark.TestBusinessLogic.Attractions;

[TestClass]
public class AccessControlServiceTests
{
    private Mock<IAttractionRepository> _mockAttractionRepo = null!;
    private Mock<IDateTimeRepository> _mockDateTimeRepo = null!;
    private Mock<IUserRepository> _mockUserRepo = null!;
    private Mock<IVisitRepository> _mockVisitRepo = null!;
    private Mock<ITicketsRepository> _mockTicketsRepo = null!;
    private Mock<IEventRepository> _mockEventRepo = null!;
    private Mock<IScoringStrategyService> _mockScoringStrategyService = null!;
    private Mock<IScoringAlgorithmFactory> _mockAlgorithmFactory = null!;
    private AccessControlService _service = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockAttractionRepo = new Mock<IAttractionRepository>();
        _mockDateTimeRepo = new Mock<IDateTimeRepository>();
        _mockUserRepo = new Mock<IUserRepository>();
        _mockVisitRepo = new Mock<IVisitRepository>();
        _mockTicketsRepo = new Mock<ITicketsRepository>();
        _mockEventRepo = new Mock<IEventRepository>();
        _mockScoringStrategyService = new Mock<IScoringStrategyService>();
        _mockAlgorithmFactory = new Mock<IScoringAlgorithmFactory>();

        _service = new AccessControlService(
            _mockAttractionRepo.Object,
            _mockDateTimeRepo.Object,
            _mockUserRepo.Object,
            _mockVisitRepo.Object,
            _mockTicketsRepo.Object,
            _mockEventRepo.Object,
            _mockScoringStrategyService.Object,
            _mockAlgorithmFactory.Object);
    }

    [TestMethod]
    [ExpectedException(typeof(BusinessLogicException))]
    public void ValidateTicketAndRegisterAccess_WhenInvalidEntryType_ShouldThrowException()
    {
        var attraction = new AttractionEntity("Test", TipoAtraccion.MontañaRusa, 10, 100, "Test", DateTime.Now, 100);
        _mockAttractionRepo.Setup(r => r.ExistsByName("Test")).Returns(true);
        _mockAttractionRepo.Setup(r => r.GetByName("Test")).Returns(attraction);

        _service.ValidateTicketAndRegisterAccess("Test", "INVALID", "123");
    }

    [TestMethod]
    [ExpectedException(typeof(AttractionNotFoundException))]
    public void RegisterExit_WhenAttractionNotFound_ShouldThrowException()
    {
        _mockAttractionRepo.Setup(r => r.ExistsByName("NonExistent")).Returns(false);

        _service.RegisterExit("NonExistent", "NFC", "123");
    }

    [TestMethod]
    [ExpectedException(typeof(BusinessLogicException))]
    public void RegisterExit_WhenInvalidQRCode_ShouldThrowException()
    {
        var attraction = new AttractionEntity("Test", TipoAtraccion.MontañaRusa, 10, 100, "Test", DateTime.Now, 100);
        _mockAttractionRepo.Setup(r => r.ExistsByName("Test")).Returns(true);
        _mockAttractionRepo.Setup(r => r.GetByName("Test")).Returns(attraction);

        _service.RegisterExit("Test", "QR", "invalid-guid");
    }

    [TestMethod]
    [ExpectedException(typeof(BusinessLogicException))]
    public void RegisterExit_WhenInvalidEntryType_ShouldThrowException()
    {
        var attraction = new AttractionEntity("Test", TipoAtraccion.MontañaRusa, 10, 100, "Test", DateTime.Now, 100);
        _mockAttractionRepo.Setup(r => r.ExistsByName("Test")).Returns(true);
        _mockAttractionRepo.Setup(r => r.GetByName("Test")).Returns(attraction);

        _service.RegisterExit("Test", "INVALID", "123");
    }

    [TestMethod]
    [ExpectedException(typeof(UserAlreadyInAttractionException))]
    public void ValidateTicketAndRegisterAccess_ShouldThrowException_WhenUserHasActiveVisitInAnotherAttraction()
    {
        var attractionName = "Montaña Rusa";
        var otherAttractionName = "Carrusel";
        var userId = Guid.NewGuid();
        var qrCode = Guid.NewGuid();

        var attraction = new AttractionEntity(attractionName, TipoAtraccion.MontañaRusa, 10, 100, "Test", DateTime.Now, 100);
        var otherAttraction = new AttractionEntity(otherAttractionName, TipoAtraccion.Simulador, 5, 50, "Test", DateTime.Now, 50);
        var user = new ThemePark.Entities.User
        {
            Id = userId,
            CodigoIdentificacion = "USER123",
            Nombre = "Test",
            Apellido = "User",
            Email = "test@test.com",
            Contraseña = "pass",
            FechaNacimiento = DateTime.Now.AddYears(-20)
        };
        var ticket = new ThemePark.Entities.Tickets.GeneralTicket
        {
            Id = Guid.NewGuid(),
            CodigoQR = qrCode,
            CodigoIdentificacionUsuario = "USER123",
            FechaVisita = DateTime.Now.Date,
            FechaCompra = DateTime.Now.AddDays(-1)
        };
        var activeVisitInOtherAttraction = new ThemePark.Entities.Visit(userId, otherAttraction, DateTime.Now.AddHours(-1));

        _mockAttractionRepo.Setup(r => r.ExistsByName(attractionName)).Returns(true);
        _mockAttractionRepo.Setup(r => r.GetByName(attractionName)).Returns(attraction);
        _mockTicketsRepo.Setup(r => r.GetByQRCode(qrCode)).Returns(ticket);
        _mockUserRepo.Setup(r => r.GetByCodigoIdentificacion("USER123")).Returns(user);
        _mockDateTimeRepo.Setup(r => r.GetCurrentDateTime()).Returns(DateTime.Now);
        _mockVisitRepo.Setup(r => r.GetActiveVisitByUserAndAttraction(userId, attractionName)).Returns((ThemePark.Entities.Visit?)null);
        _mockVisitRepo.Setup(r => r.GetActiveVisitByUser(userId)).Returns(activeVisitInOtherAttraction);

        _service.ValidateTicketAndRegisterAccess(attractionName, "QR", qrCode.ToString());
    }

    [TestMethod]
    [ExpectedException(typeof(EventTicketNotValidForTimeException))]
    public void ValidateTicketAndRegisterAccess_ShouldThrowException_WhenEventTicketUsedBeforeEventStartTime()
    {
        var attractionName = "Montaña Rusa";
        var userId = Guid.NewGuid();
        var eventId = Guid.NewGuid();
        var qrCode = Guid.NewGuid();
        var eventStartTime = DateTime.Now.Date.AddHours(20); // Evento comienza a las 20:00
        var currentTime = DateTime.Now.Date.AddHours(19); // Intenta acceder a las 19:00 (1 hora antes)

        var attraction = new AttractionEntity(attractionName, TipoAtraccion.MontañaRusa, 10, 100, "Test", DateTime.Now, 100);
        var user = new ThemePark.Entities.User
        {
            Id = userId,
            CodigoIdentificacion = "USER123",
            Nombre = "Test",
            Apellido = "User",
            Email = "test@test.com",
            Contraseña = "pass",
            FechaNacimiento = DateTime.Now.AddYears(-20)
        };
        var eventTicket = new ThemePark.Entities.Tickets.EventTicket
        {
            Id = Guid.NewGuid(),
            CodigoQR = qrCode,
            CodigoIdentificacionUsuario = "USER123",
            FechaVisita = DateTime.Now.Date,
            FechaCompra = DateTime.Now.AddDays(-1),
            EventoId = eventId
        };
        var evento = new ThemePark.Entities.Event
        {
            Id = eventId,
            Name = "Evento Nocturno",
            Fecha = DateTime.Now.Date,
            Hora = new TimeSpan(20, 0, 0), // Evento de 20:00 a 00:00
            Aforo = 100,
            CostoAdicional = 50m,
            Atracciones = [attraction]
        };

        _mockAttractionRepo.Setup(r => r.ExistsByName(attractionName)).Returns(true);
        _mockAttractionRepo.Setup(r => r.GetByName(attractionName)).Returns(attraction);
        _mockTicketsRepo.Setup(r => r.GetByQRCode(qrCode)).Returns(eventTicket);
        _mockUserRepo.Setup(r => r.GetByCodigoIdentificacion("USER123")).Returns(user);
        _mockEventRepo.Setup(r => r.GetById(eventId)).Returns(evento);
        _mockDateTimeRepo.Setup(r => r.GetCurrentDateTime()).Returns(currentTime);

        _service.ValidateTicketAndRegisterAccess(attractionName, "QR", qrCode.ToString());
    }

    [TestMethod]
    [ExpectedException(typeof(EventTicketNotValidForTimeException))]
    public void ValidateTicketAndRegisterAccess_ShouldThrowException_WhenEventTicketUsedAfterEventEndTime()
    {
        var attractionName = "Montaña Rusa";
        var userId = Guid.NewGuid();
        var eventId = Guid.NewGuid();
        var qrCode = Guid.NewGuid();
        var eventStartTime = DateTime.Now.Date.AddHours(20); // Evento comienza a las 20:00
        var currentTime = DateTime.Now.Date.AddHours(24).AddMinutes(1); // Intenta acceder a las 00:01 (después de las 4 horas)

        var attraction = new AttractionEntity(attractionName, TipoAtraccion.MontañaRusa, 10, 100, "Test", DateTime.Now, 100);
        var user = new ThemePark.Entities.User
        {
            Id = userId,
            CodigoIdentificacion = "USER123",
            Nombre = "Test",
            Apellido = "User",
            Email = "test@test.com",
            Contraseña = "pass",
            FechaNacimiento = DateTime.Now.AddYears(-20)
        };
        var eventTicket = new ThemePark.Entities.Tickets.EventTicket
        {
            Id = Guid.NewGuid(),
            CodigoQR = qrCode,
            CodigoIdentificacionUsuario = "USER123",
            FechaVisita = DateTime.Now.Date,
            FechaCompra = DateTime.Now.AddDays(-1),
            EventoId = eventId
        };
        var evento = new ThemePark.Entities.Event
        {
            Id = eventId,
            Name = "Evento Nocturno",
            Fecha = DateTime.Now.Date,
            Hora = new TimeSpan(20, 0, 0), // Evento de 20:00 a 00:00 (4 horas)
            Aforo = 100,
            CostoAdicional = 50m,
            Atracciones = [attraction]
        };

        _mockAttractionRepo.Setup(r => r.ExistsByName(attractionName)).Returns(true);
        _mockAttractionRepo.Setup(r => r.GetByName(attractionName)).Returns(attraction);
        _mockTicketsRepo.Setup(r => r.GetByQRCode(qrCode)).Returns(eventTicket);
        _mockUserRepo.Setup(r => r.GetByCodigoIdentificacion("USER123")).Returns(user);
        _mockEventRepo.Setup(r => r.GetById(eventId)).Returns(evento);
        _mockDateTimeRepo.Setup(r => r.GetCurrentDateTime()).Returns(currentTime);

        _service.ValidateTicketAndRegisterAccess(attractionName, "QR", qrCode.ToString());
    }

    [TestMethod]
    public void ValidateTicketAndRegisterAccess_ShouldSucceed_WhenEventTicketUsedDuringEventTimeWindow()
    {
        var attractionName = "Montaña Rusa";
        var userId = Guid.NewGuid();
        var eventId = Guid.NewGuid();
        var qrCode = Guid.NewGuid();
        var currentTime = DateTime.Now.Date.AddHours(21); // Accede a las 21:00 (1 hora después del inicio)

        var attraction = new AttractionEntity(attractionName, TipoAtraccion.MontañaRusa, 10, 100, "Test", DateTime.Now, 100);
        var user = new ThemePark.Entities.User
        {
            Id = userId,
            CodigoIdentificacion = "USER123",
            Nombre = "Test",
            Apellido = "User",
            Email = "test@test.com",
            Contraseña = "pass",
            FechaNacimiento = DateTime.Now.AddYears(-20)
        };
        var eventTicket = new ThemePark.Entities.Tickets.EventTicket
        {
            Id = Guid.NewGuid(),
            CodigoQR = qrCode,
            CodigoIdentificacionUsuario = "USER123",
            FechaVisita = DateTime.Now.Date,
            FechaCompra = DateTime.Now.AddDays(-1),
            EventoId = eventId
        };
        var evento = new ThemePark.Entities.Event
        {
            Id = eventId,
            Name = "Evento Nocturno",
            Fecha = DateTime.Now.Date,
            Hora = new TimeSpan(20, 0, 0), // Evento de 20:00 a 00:00
            Aforo = 100,
            CostoAdicional = 50m,
            Atracciones = [attraction]
        };

        _mockAttractionRepo.Setup(r => r.ExistsByName(attractionName)).Returns(true);
        _mockAttractionRepo.Setup(r => r.GetByName(attractionName)).Returns(attraction);
        _mockTicketsRepo.Setup(r => r.GetByQRCode(qrCode)).Returns(eventTicket);
        _mockUserRepo.Setup(r => r.GetByCodigoIdentificacion("USER123")).Returns(user);
        _mockEventRepo.Setup(r => r.GetById(eventId)).Returns(evento);
        _mockDateTimeRepo.Setup(r => r.GetCurrentDateTime()).Returns(currentTime);
        _mockVisitRepo.Setup(r => r.GetActiveVisitByUserAndAttraction(userId, attractionName)).Returns((ThemePark.Entities.Visit?)null);
        _mockVisitRepo.Setup(r => r.GetActiveVisitByUser(userId)).Returns((ThemePark.Entities.Visit?)null);
        _mockScoringStrategyService.Setup(s => s.GetActiveStrategy()).Returns((ThemePark.Entities.ScoringStrategy?)null);
        _mockVisitRepo.Setup(r => r.Save(It.IsAny<ThemePark.Entities.Visit>())).Returns(It.IsAny<ThemePark.Entities.Visit>());
        _mockAttractionRepo.Setup(r => r.Save(It.IsAny<AttractionEntity>()));

        var result = _service.ValidateTicketAndRegisterAccess(attractionName, "QR", qrCode.ToString());

        Assert.IsNotNull(result);
        _mockVisitRepo.Verify(r => r.Save(It.IsAny<ThemePark.Entities.Visit>()), Times.Once);
    }
}
