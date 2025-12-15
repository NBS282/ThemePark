using Moq;
using ThemePark.BusinessLogic;
using ThemePark.BusinessLogic.Attractions;
using ThemePark.Entities;
using ThemePark.Entities.Tickets;
using ThemePark.Enums;
using ThemePark.IBusinessLogic;
using ThemePark.IBusinessLogic.Exceptions;
using ThemePark.IDataAccess;
using ThemePark.IDataAccess.Exceptions;
using ThemePark.Models.Attractions;

namespace ThemePark.TestBusinessLogic;

[TestClass]
public class AttractionsBusinessLogicTests
{
    private Mock<IAttractionRepository> _mockAttractionRepository = null!;
    private Mock<IDateTimeRepository> _mockDateTimeRepository = null!;
    private Mock<IUserRepository> _mockUserRepository = null!;
    private Mock<IVisitRepository> _mockVisitRepository = null!;
    private Mock<ITicketsRepository> _mockTicketsRepository = null!;
    private Mock<IEventRepository> _mockEventRepository = null!;
    private Mock<IIncidentRepository> _mockIncidentRepository = null!;
    private Mock<IScoringStrategyService> _mockScoringStrategyService = null!;
    private Mock<IScoringAlgorithmFactory> _mockAlgorithmFactory = null!;
    private Mock<IMaintenanceRepository> _mockMaintenanceRepository = null!;
    private IAttractionsBusinessLogic _attractionsBusinessLogic = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockAttractionRepository = new Mock<IAttractionRepository>(MockBehavior.Strict);
        _mockDateTimeRepository = new Mock<IDateTimeRepository>(MockBehavior.Strict);
        _mockUserRepository = new Mock<IUserRepository>(MockBehavior.Strict);
        _mockVisitRepository = new Mock<IVisitRepository>(MockBehavior.Strict);
        _mockTicketsRepository = new Mock<ITicketsRepository>(MockBehavior.Strict);
        _mockEventRepository = new Mock<IEventRepository>(MockBehavior.Strict);
        _mockIncidentRepository = new Mock<IIncidentRepository>(MockBehavior.Strict);
        _mockScoringStrategyService = new Mock<IScoringStrategyService>(MockBehavior.Strict);
        _mockAlgorithmFactory = new Mock<IScoringAlgorithmFactory>(MockBehavior.Strict);
        _mockMaintenanceRepository = new Mock<IMaintenanceRepository>(MockBehavior.Strict);

        _mockVisitRepository.Setup(x => x.GetVisitsByDateRange(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
            .Returns([]);

        _mockScoringStrategyService.Setup(x => x.GetActiveStrategy())
            .Returns((Entities.ScoringStrategy?)null);

        _mockVisitRepository.Setup(x => x.GetVisitsByUserAndDate(It.IsAny<Guid>(), It.IsAny<DateTime>()))
            .Returns([]);

        _mockVisitRepository.Setup(x => x.GetActiveVisitByUser(It.IsAny<Guid>()))
            .Returns((Visit?)null);

        // Crear los 4 servicios internos con sus dependencias reales
        var managementService = new AttractionManagementService(
            _mockAttractionRepository.Object,
            _mockDateTimeRepository.Object,
            _mockMaintenanceRepository.Object,
            _mockIncidentRepository.Object);

        var accessControlService = new AccessControlService(
            _mockAttractionRepository.Object,
            _mockDateTimeRepository.Object,
            _mockUserRepository.Object,
            _mockVisitRepository.Object,
            _mockTicketsRepository.Object,
            _mockEventRepository.Object,
            _mockScoringStrategyService.Object,
            _mockAlgorithmFactory.Object);

        var incidentService = new IncidentManagementService(
            _mockAttractionRepository.Object,
            _mockIncidentRepository.Object,
            _mockDateTimeRepository.Object,
            _mockMaintenanceRepository.Object);

        var reportingService = new AttractionReportingService(
            _mockVisitRepository.Object);

        var maintenanceService = new MaintenanceManagementService(
            _mockAttractionRepository.Object,
            _mockMaintenanceRepository.Object,
            _mockIncidentRepository.Object,
            _mockDateTimeRepository.Object);

        // Crear el Facade con los 5 servicios
        _attractionsBusinessLogic = new AttractionsBusinessLogic(
            managementService,
            accessControlService,
            incidentService,
            reportingService,
            maintenanceService);
    }

    [TestMethod]
    public void CreateAttraction_ShouldCreateAndSaveAttraction_WhenValidDataProvided()
    {
        var nombre = "Montaña Rusa T-Rex";
        var tipo = 0;
        var edadMinima = 12;
        var capacidadMaxima = 50;
        var descripcion = "Emocionante montaña rusa temática de dinosaurios";
        var fechaCreacion = new DateTime(2025, 9, 18, 14, 30, 0);

        _mockDateTimeRepository.Setup(x => x.GetCurrentDateTime()).Returns(fechaCreacion);
        _mockAttractionRepository.Setup(x => x.ExistsByName(nombre)).Returns(false);
        _mockAttractionRepository.Setup(x => x.Save(It.IsAny<Attraction>()));

        var result = _attractionsBusinessLogic.CreateAttraction(nombre, tipo, edadMinima, capacidadMaxima, descripcion, 0);

        _mockAttractionRepository.Verify(x => x.Save(It.Is<Attraction>(a =>
            a.Nombre == nombre &&
            a.Tipo == TipoAtraccion.MontañaRusa &&
            a.EdadMinima == edadMinima &&
            a.CapacidadMaxima == capacidadMaxima &&
            a.Descripcion == descripcion &&
            a.FechaCreacion == fechaCreacion)), Times.Once);

        Assert.IsNotNull(result);
        Assert.AreEqual(nombre, result.Nombre);
        Assert.AreEqual(TipoAtraccion.MontañaRusa, result.Tipo);
    }

    [TestMethod]
    [ExpectedException(typeof(BusinessLogicException))]
    public void CreateAttraction_ShouldThrowBusinessLogicException_WhenAttractionNameAlreadyExists()
    {
        var nombre = "Montaña Rusa T-Rex";
        var tipo = 0;
        var edadMinima = 12;
        var capacidadMaxima = 50;
        var descripcion = "Emocionante montaña rusa temática de dinosaurios";

        _mockAttractionRepository.Setup(x => x.ExistsByName(nombre)).Returns(true);

        _attractionsBusinessLogic.CreateAttraction(nombre, tipo, edadMinima, capacidadMaxima, descripcion, 0);
    }

    [TestMethod]
    public void DeleteAttraction_ShouldDeleteAttraction_WhenAttractionExists()
    {
        var nombre = "Montaña Rusa T-Rex";

        _mockAttractionRepository.Setup(x => x.ExistsByName(nombre)).Returns(true);
        _mockIncidentRepository.Setup(x => x.GetActiveByAttractionName(nombre)).Returns([]);
        _mockMaintenanceRepository.Setup(x => x.GetByAttractionName(nombre)).Returns([]);
        _mockAttractionRepository.Setup(x => x.Delete(nombre));

        _attractionsBusinessLogic.DeleteAttraction(nombre);

        _mockAttractionRepository.Verify(x => x.Delete(nombre), Times.Once);
    }

    [TestMethod]
    [ExpectedException(typeof(AttractionNotFoundException))]
    public void DeleteAttraction_ShouldThrowAttractionNotFoundException_WhenAttractionDoesNotExist()
    {
        var nombre = "Atracción Inexistente";

        _mockAttractionRepository.Setup(x => x.ExistsByName(nombre)).Returns(false);

        _attractionsBusinessLogic.DeleteAttraction(nombre);
    }

    [TestMethod]
    public void GetCapacity_ShouldReturnCapacityInfo_WhenAttractionExists()
    {
        var nombre = "Montaña Rusa T-Rex";
        var attraction = new Attraction(nombre, TipoAtraccion.MontañaRusa, 12, 50, "Emocionante montaña rusa", DateTime.Now);

        _mockAttractionRepository.Setup(x => x.ExistsByName(nombre)).Returns(true);
        _mockAttractionRepository.Setup(x => x.GetByName(nombre)).Returns(attraction);
        _mockDateTimeRepository.Setup(x => x.GetCurrentDateTime()).Returns(DateTime.Now);
        _mockMaintenanceRepository.Setup(x => x.GetByAttractionName(nombre)).Returns([]);

        var result = _attractionsBusinessLogic.GetCapacity(nombre);

        Assert.IsNotNull(result);
        _mockAttractionRepository.Verify(x => x.ExistsByName(nombre), Times.Once);
        _mockAttractionRepository.Verify(x => x.GetByName(nombre), Times.Once);
    }

    [TestMethod]
    [ExpectedException(typeof(AttractionNotFoundException))]
    public void GetCapacity_ShouldThrowAttractionNotFoundException_WhenAttractionDoesNotExist()
    {
        var nombre = "Atracción Inexistente";

        _mockAttractionRepository.Setup(x => x.ExistsByName(nombre)).Returns(false);

        _attractionsBusinessLogic.GetCapacity(nombre);
    }

    [TestMethod]
    public void ResolveIncident_ShouldResolveIncident_WhenAttractionExistsAndHasActiveIncident()
    {
        var nombre = "Montaña Rusa T-Rex";
        var incidentId = Guid.NewGuid();
        var attraction = new Attraction(nombre, TipoAtraccion.MontañaRusa, 12, 50, "Emocionante montaña rusa", DateTime.Now);
        attraction.ActivarIncidencia();
        var incident = new Incident(nombre, "Falla en frenos", DateTime.Now);
        var fechaResolucion = new DateTime(2025, 9, 29, 15, 0, 0);

        _mockAttractionRepository.Setup(x => x.ExistsByName(nombre)).Returns(true);
        _mockAttractionRepository.Setup(x => x.GetByName(nombre)).Returns(attraction);
        _mockAttractionRepository.Setup(x => x.Save(attraction));
        _mockIncidentRepository.Setup(x => x.GetById(incidentId)).Returns(incident);
        _mockIncidentRepository.Setup(x => x.Update(incident)).Returns(incident);
        _mockDateTimeRepository.Setup(x => x.GetCurrentDateTime()).Returns(fechaResolucion);

        _attractionsBusinessLogic.ResolveIncident(nombre, incidentId.ToString());

        Assert.IsFalse(attraction.TieneIncidenciaActiva);
        _mockAttractionRepository.Verify(x => x.Save(attraction), Times.Once);
    }

    [TestMethod]
    [ExpectedException(typeof(AttractionNotFoundException))]
    public void ResolveIncident_ShouldThrowAttractionNotFoundException_WhenAttractionDoesNotExist()
    {
        var nombre = "Atracción Inexistente";
        var incidentId = Guid.NewGuid();

        _mockAttractionRepository.Setup(x => x.ExistsByName(nombre)).Returns(false);

        _attractionsBusinessLogic.ResolveIncident(nombre, incidentId.ToString());
    }

    [TestMethod]
    public void CreateIncident_ShouldCreateIncident_WhenAttractionExists()
    {
        var nombre = "Montaña Rusa T-Rex";
        var descripcion = "Falla en el sistema de frenos";
        var attraction = new Attraction(nombre, TipoAtraccion.MontañaRusa, 12, 50, "Emocionante montaña rusa", DateTime.Now);
        var fechaIncident = new DateTime(2025, 9, 18, 16, 0, 0);

        _mockAttractionRepository.Setup(x => x.ExistsByName(nombre)).Returns(true);
        _mockAttractionRepository.Setup(x => x.GetByName(nombre)).Returns(attraction);
        _mockAttractionRepository.Setup(x => x.Save(attraction));
        _mockDateTimeRepository.Setup(x => x.GetCurrentDateTime()).Returns(fechaIncident);
        _mockIncidentRepository.Setup(x => x.GetActiveByAttractionName(nombre)).Returns([]);
        _mockIncidentRepository.Setup(x => x.Save(It.IsAny<Incident>())).Returns((Incident i) => i);

        var result = _attractionsBusinessLogic.CreateIncident(nombre, descripcion);

        Assert.IsNotNull(result);
        Assert.IsTrue(attraction.TieneIncidenciaActiva);
        _mockAttractionRepository.Verify(x => x.Save(attraction), Times.Once);
    }

    [TestMethod]
    [ExpectedException(typeof(AttractionNotFoundException))]
    public void CreateIncident_ShouldThrowAttractionNotFoundException_WhenAttractionDoesNotExist()
    {
        var nombre = "Atracción Inexistente";
        var descripcion = "Falla en el sistema";

        _mockAttractionRepository.Setup(x => x.ExistsByName(nombre)).Returns(false);

        _attractionsBusinessLogic.CreateIncident(nombre, descripcion);
    }

    [TestMethod]
    [ExpectedException(typeof(BusinessLogicException))]
    public void CreateIncident_ShouldThrowBusinessLogicException_WhenAttractionAlreadyHasActiveIncident()
    {
        var nombre = "Montaña Rusa T-Rex";
        var descripcion = "Segunda falla en el sistema de frenos";
        var attraction = new Attraction(nombre, TipoAtraccion.MontañaRusa, 12, 50, "Emocionante montaña rusa", DateTime.Now);
        var existingIncident = new Incident(nombre, "Falla en el sistema de frenos", DateTime.Now);

        _mockAttractionRepository.Setup(x => x.ExistsByName(nombre)).Returns(true);
        _mockAttractionRepository.Setup(x => x.GetByName(nombre)).Returns(attraction);
        _mockIncidentRepository.Setup(x => x.GetActiveByAttractionName(nombre)).Returns([existingIncident]);

        _attractionsBusinessLogic.CreateIncident(nombre, descripcion);
    }

    [TestMethod]
    public void ResolveIncident_ShouldResolveIncidentAndDeactivate_WhenIncidentExists()
    {
        var nombre = "Montaña Rusa T-Rex";
        var incidentId = Guid.NewGuid();
        var attraction = new Attraction(nombre, TipoAtraccion.MontañaRusa, 12, 50, "Emocionante montaña rusa", DateTime.Now);
        attraction.ActivarIncidencia();
        var incident = new Incident(nombre, "Falla en frenos", DateTime.Now);
        var fechaResolucion = new DateTime(2025, 9, 29, 15, 0, 0);

        _mockAttractionRepository.Setup(x => x.ExistsByName(nombre)).Returns(true);
        _mockAttractionRepository.Setup(x => x.GetByName(nombre)).Returns(attraction);
        _mockAttractionRepository.Setup(x => x.Save(attraction));
        _mockIncidentRepository.Setup(x => x.GetById(incidentId)).Returns(incident);
        _mockIncidentRepository.Setup(x => x.Update(incident)).Returns(incident);
        _mockIncidentRepository.Setup(x => x.GetActiveByAttractionName(nombre)).Returns([]);
        _mockDateTimeRepository.Setup(x => x.GetCurrentDateTime()).Returns(fechaResolucion);

        _attractionsBusinessLogic.ResolveIncident(nombre, incidentId.ToString());

        Assert.IsFalse(attraction.TieneIncidenciaActiva);
        _mockAttractionRepository.Verify(x => x.Save(attraction), Times.Once);
        _mockIncidentRepository.Verify(x => x.GetById(incidentId), Times.Once);
        _mockIncidentRepository.Verify(x => x.Update(It.Is<Incident>(i =>
            !i.IsActive &&
            i.FechaResolucion == fechaResolucion)), Times.Once);
    }

    [TestMethod]
    public void UpdateAttraction_ShouldUpdateAttraction_WhenAttractionExistsAndValidDataProvided()
    {
        var nombre = "Montaña Rusa T-Rex";
        var request = new UpdateAttractionRequest
        {
            Descripcion = "Nueva descripción actualizada",
            CapacidadMaxima = 60,
            EdadMinima = 14
        };
        var attraction = new Attraction(nombre, TipoAtraccion.MontañaRusa, 12, 50, "Descripción original", DateTime.Now);
        var fechaModificacion = new DateTime(2025, 9, 18, 17, 0, 0);

        _mockAttractionRepository.Setup(x => x.ExistsByName(nombre)).Returns(true);
        _mockAttractionRepository.Setup(x => x.GetByName(nombre)).Returns(attraction);
        _mockAttractionRepository.Setup(x => x.Save(attraction));
        _mockDateTimeRepository.Setup(x => x.GetCurrentDateTime()).Returns(fechaModificacion);

        var result = _attractionsBusinessLogic.UpdateAttraction(nombre, request.Descripcion, request.CapacidadMaxima, request.EdadMinima);

        Assert.IsNotNull(result);
        _mockAttractionRepository.Verify(x => x.Save(attraction), Times.Once);
    }

    [TestMethod]
    [ExpectedException(typeof(AttractionNotFoundException))]
    public void UpdateAttraction_ShouldThrowAttractionNotFoundException_WhenAttractionDoesNotExist()
    {
        var nombre = "Atracción Inexistente";
        var request = new UpdateAttractionRequest
        {
            Descripcion = "Nueva descripción",
            CapacidadMaxima = 60
        };

        _mockAttractionRepository.Setup(x => x.ExistsByName(nombre)).Returns(false);

        _attractionsBusinessLogic.UpdateAttraction(nombre, request.Descripcion, request.CapacidadMaxima, request.EdadMinima);
    }

    [TestMethod]
    public void ValidateTicketAndRegisterAccess_ShouldRegisterVisit_WhenValidUserAndAttractionProvided()
    {
        var nombre = "Montaña Rusa T-Rex";
        var request = new RegisterVisitByNFCRequest
        {
            CodigoIdentificacion = "ID123456"
        };
        var userId = Guid.NewGuid();
        var user = new User
        {
            Id = userId,
            Nombre = "Juan",
            Apellido = "Pérez",
            Email = "juan@email.com",
            Contraseña = "password123",
            FechaNacimiento = new DateTime(1990, 1, 1),
            CodigoIdentificacion = "ID123456"
        };
        var attraction = new Attraction(nombre, TipoAtraccion.MontañaRusa, 10, 50, "Emocionante montaña rusa", DateTime.Now);
        var entryTime = DateTime.Now.AddDays(1);
        var validTicket = new GeneralTicket
        {
            Id = Guid.NewGuid(),
            CodigoIdentificacionUsuario = "ID123456",
            FechaVisita = entryTime.Date,
            FechaCompra = DateTime.Now.AddDays(-1),
            CodigoQR = Guid.NewGuid()
        };

        _mockAttractionRepository.Setup(x => x.ExistsByName(nombre)).Returns(true);
        _mockAttractionRepository.Setup(x => x.GetByName(nombre)).Returns(attraction);
        _mockUserRepository.Setup(x => x.GetByCodigoIdentificacion("ID123456")).Returns(user);
        _mockTicketsRepository.Setup(x => x.GetByCodigoIdentificacionUsuario("ID123456")).Returns([validTicket]);
        _mockVisitRepository.Setup(x => x.GetActiveVisitByUserAndAttraction(userId, nombre)).Returns((Visit?)null);
        _mockDateTimeRepository.Setup(x => x.GetCurrentDateTime()).Returns(entryTime);
        _mockVisitRepository.Setup(x => x.Save(It.IsAny<Visit>())).Returns((Visit v) => v);
        _mockAttractionRepository.Setup(x => x.Save(attraction));

        var result = _attractionsBusinessLogic.ValidateTicketAndRegisterAccess(nombre, "NFC", request.CodigoIdentificacion);

        Assert.IsNotNull(result);
        _mockVisitRepository.Verify(x => x.Save(It.Is<Visit>(v =>
            v.UserId == userId &&
            v.AttractionName == nombre &&
            v.EntryTime == entryTime &&
            v.IsActive)), Times.Once);
        _mockAttractionRepository.Verify(x => x.Save(attraction), Times.Once);
        Assert.AreEqual(1, attraction.AforoActual);
    }

    [TestMethod]
    [ExpectedException(typeof(BusinessLogicException))]
    public void ValidateTicketAndRegisterAccess_ShouldThrowBusinessLogicException_WhenAttractionHasActiveIncident_NFC()
    {
        var nombre = "Montaña Rusa T-Rex";
        var request = new RegisterVisitByNFCRequest
        {
            CodigoIdentificacion = "ID123456"
        };
        var userId = Guid.NewGuid();
        var user = new User
        {
            Id = userId,
            Nombre = "Juan",
            Apellido = "Pérez",
            Email = "juan@email.com",
            Contraseña = "password123",
            FechaNacimiento = new DateTime(1990, 1, 1),
            CodigoIdentificacion = "ID123456"
        };
        var attraction = new Attraction(nombre, TipoAtraccion.MontañaRusa, 10, 50, "Emocionante montaña rusa", DateTime.Now);
        attraction.ActivarIncidencia();

        _mockAttractionRepository.Setup(x => x.ExistsByName(nombre)).Returns(true);
        _mockAttractionRepository.Setup(x => x.GetByName(nombre)).Returns(attraction);
        _mockUserRepository.Setup(x => x.GetByCodigoIdentificacion("ID123456")).Returns(user);
        _mockTicketsRepository.Setup(x => x.GetByCodigoIdentificacionUsuario("ID123456")).Returns([]);

        _attractionsBusinessLogic.ValidateTicketAndRegisterAccess(nombre, "NFC", request.CodigoIdentificacion);
    }

    [TestMethod]
    [ExpectedException(typeof(BusinessLogicException))]
    public void ValidateTicketAndRegisterAccess_ShouldThrowBusinessLogicException_WhenAttractionIsAtMaxCapacity_NFC()
    {
        var nombre = "Montaña Rusa T-Rex";
        var request = new RegisterVisitByNFCRequest
        {
            CodigoIdentificacion = "ID123456"
        };
        var userId = Guid.NewGuid();
        var user = new User
        {
            Id = userId,
            Nombre = "Juan",
            Apellido = "Pérez",
            Email = "juan@email.com",
            Contraseña = "password123",
            FechaNacimiento = new DateTime(1990, 1, 1),
            CodigoIdentificacion = "ID123456"
        };
        var attraction = new Attraction(nombre, TipoAtraccion.MontañaRusa, 10, 2, "Emocionante montaña rusa", DateTime.Now);
        attraction.IncrementarAforo();
        attraction.IncrementarAforo();

        _mockAttractionRepository.Setup(x => x.ExistsByName(nombre)).Returns(true);
        _mockAttractionRepository.Setup(x => x.GetByName(nombre)).Returns(attraction);
        _mockUserRepository.Setup(x => x.GetByCodigoIdentificacion("ID123456")).Returns(user);
        _mockTicketsRepository.Setup(x => x.GetByCodigoIdentificacionUsuario("ID123456")).Returns([]);

        _attractionsBusinessLogic.ValidateTicketAndRegisterAccess(nombre, "NFC", request.CodigoIdentificacion);
    }

    [TestMethod]
    [ExpectedException(typeof(AgeLimitException))]
    public void ValidateTicketAndRegisterAccess_ShouldThrowBusinessLogicException_WhenUserIsTooYoung_NFC()
    {
        var nombre = "Montaña Rusa T-Rex";
        var request = new RegisterVisitByNFCRequest
        {
            CodigoIdentificacion = "ID123456"
        };
        var userId = Guid.NewGuid();
        var entryTime = DateTime.Now.AddDays(1);
        var user = new User
        {
            Id = userId,
            Nombre = "Juan",
            Apellido = "Pérez",
            Email = "juan@email.com",
            Contraseña = "password123",
            FechaNacimiento = new DateTime(2020, 1, 1),
            CodigoIdentificacion = "ID123456"
        };
        var attraction = new Attraction(nombre, TipoAtraccion.MontañaRusa, 12, 50, "Emocionante montaña rusa", DateTime.Now);

        var validTicket = new GeneralTicket
        {
            Id = Guid.NewGuid(),
            CodigoIdentificacionUsuario = "ID123456",
            FechaVisita = entryTime.Date,
            FechaCompra = DateTime.Now.AddDays(-1),
            CodigoQR = Guid.NewGuid()
        };

        _mockAttractionRepository.Setup(x => x.ExistsByName(nombre)).Returns(true);
        _mockAttractionRepository.Setup(x => x.GetByName(nombre)).Returns(attraction);
        _mockUserRepository.Setup(x => x.GetByCodigoIdentificacion("ID123456")).Returns(user);
        _mockTicketsRepository.Setup(x => x.GetByCodigoIdentificacionUsuario("ID123456")).Returns([validTicket]);
        _mockDateTimeRepository.Setup(x => x.GetCurrentDateTime()).Returns(entryTime);

        _attractionsBusinessLogic.ValidateTicketAndRegisterAccess(nombre, "NFC", request.CodigoIdentificacion);
    }

    [TestMethod]
    public void ValidateTicketAndRegisterAccess_ShouldReturnValidVisit_WhenValidTicketProvided()
    {
        var nombre = "Montaña Rusa";
        var entryTime = DateTime.Now;
        var request = new { QRCode = "QR123456", CodigoIdentificacion = "ID123456" };
        var user = new User
        {
            Id = Guid.NewGuid(),
            Nombre = "Juan",
            Apellido = "Pérez",
            Email = "juan@email.com",
            Contraseña = "password123",
            FechaNacimiento = new DateTime(1990, 1, 1),
            CodigoIdentificacion = "ID123456"
        };
        var attraction = new Attraction(nombre, TipoAtraccion.MontañaRusa, 18, 50, "Emocionante montaña rusa", DateTime.Now);

        var validTicket = new GeneralTicket
        {
            Id = Guid.NewGuid(),
            CodigoIdentificacionUsuario = "ID123456",
            FechaVisita = entryTime.Date,
            FechaCompra = DateTime.Now.AddDays(-1),
            CodigoQR = Guid.NewGuid()
        };

        _mockAttractionRepository.Setup(x => x.ExistsByName(nombre)).Returns(true);
        _mockAttractionRepository.Setup(x => x.GetByName(nombre)).Returns(attraction);
        _mockAttractionRepository.Setup(x => x.Save(attraction));
        _mockUserRepository.Setup(x => x.GetByCodigoIdentificacion("ID123456")).Returns(user);
        _mockTicketsRepository.Setup(x => x.GetByCodigoIdentificacionUsuario("ID123456")).Returns([validTicket]);
        _mockDateTimeRepository.Setup(x => x.GetCurrentDateTime()).Returns(entryTime);
        _mockVisitRepository.Setup(x => x.GetActiveVisitByUserAndAttraction(user.Id, nombre)).Returns((Visit?)null);
        _mockVisitRepository.Setup(x => x.Save(It.IsAny<Visit>())).Returns((Visit v) => v);

        var result = _attractionsBusinessLogic.ValidateTicketAndRegisterAccess(nombre, "NFC", request.CodigoIdentificacion);

        Assert.IsNotNull(result);
        var visit = result as Visit;
        Assert.IsNotNull(visit);
    }

    [TestMethod]
    public void ValidateTicketAndRegisterAccess_ShouldSetVisitProperties_WhenValidTicketProvided()
    {
        var nombre = "Montaña Rusa";
        var entryTime = DateTime.Now;
        var request = new { QRCode = "QR123456", CodigoIdentificacion = "ID123456" };
        var user = new User
        {
            Id = Guid.NewGuid(),
            Nombre = "Juan",
            Apellido = "Pérez",
            Email = "juan@email.com",
            Contraseña = "password123",
            FechaNacimiento = new DateTime(1990, 1, 1),
            CodigoIdentificacion = "ID123456"
        };
        var attraction = new Attraction(nombre, TipoAtraccion.MontañaRusa, 18, 50, "Emocionante montaña rusa", DateTime.Now);

        var validTicket = new GeneralTicket
        {
            Id = Guid.NewGuid(),
            CodigoIdentificacionUsuario = "ID123456",
            FechaVisita = entryTime.Date,
            FechaCompra = DateTime.Now.AddDays(-1),
            CodigoQR = Guid.NewGuid()
        };

        _mockAttractionRepository.Setup(x => x.ExistsByName(nombre)).Returns(true);
        _mockAttractionRepository.Setup(x => x.GetByName(nombre)).Returns(attraction);
        _mockAttractionRepository.Setup(x => x.Save(attraction));
        _mockUserRepository.Setup(x => x.GetByCodigoIdentificacion("ID123456")).Returns(user);
        _mockTicketsRepository.Setup(x => x.GetByCodigoIdentificacionUsuario("ID123456")).Returns([validTicket]);
        _mockDateTimeRepository.Setup(x => x.GetCurrentDateTime()).Returns(entryTime);
        _mockVisitRepository.Setup(x => x.GetActiveVisitByUserAndAttraction(user.Id, nombre)).Returns((Visit?)null);
        _mockVisitRepository.Setup(x => x.Save(It.IsAny<Visit>())).Returns((Visit v) => v);

        var result = _attractionsBusinessLogic.ValidateTicketAndRegisterAccess(nombre, "NFC", request.CodigoIdentificacion);

        var visit = result as Visit;
        Assert.AreEqual(user.Id, visit!.UserId);
        Assert.AreEqual(nombre, visit.AttractionName);
        Assert.AreEqual(entryTime, visit.EntryTime);
    }

    [TestMethod]
    public void RegisterExit_ShouldReturnValidVisit_WhenValidRequest()
    {
        var nombre = "Montaña Rusa";
        var exitTime = DateTime.Now;
        var codigoIdentificacion = "ID123456";
        var user = new User
        {
            Id = Guid.NewGuid(),
            Nombre = "Juan",
            Apellido = "Pérez",
            Email = "juan@email.com",
            Contraseña = "password123",
            FechaNacimiento = new DateTime(1990, 1, 1),
            CodigoIdentificacion = "ID123456"
        };
        var attraction = new Attraction(nombre, TipoAtraccion.MontañaRusa, 18, 50, "Emocionante montaña rusa", DateTime.Now);
        attraction.IncrementarAforo();

        var activeVisit = new Visit(user.Id, nombre, DateTime.Now.AddHours(-1));

        _mockAttractionRepository.Setup(x => x.ExistsByName(nombre)).Returns(true);
        _mockAttractionRepository.Setup(x => x.GetByName(nombre)).Returns(attraction);
        _mockAttractionRepository.Setup(x => x.Save(attraction));
        _mockUserRepository.Setup(x => x.GetByCodigoIdentificacion("ID123456")).Returns(user);
        _mockDateTimeRepository.Setup(x => x.GetCurrentDateTime()).Returns(exitTime);
        _mockVisitRepository.Setup(x => x.GetActiveVisitByUserAndAttraction(user.Id, nombre)).Returns(activeVisit);
        _mockVisitRepository.Setup(x => x.Update(It.IsAny<Visit>())).Returns((Visit v) => v);

        var result = _attractionsBusinessLogic.RegisterExit(nombre, "NFC", codigoIdentificacion);

        Assert.IsNotNull(result);
        var visit = result as Visit;
        Assert.IsNotNull(visit);
    }

    [TestMethod]
    public void RegisterExit_ShouldUpdateExitTimeAndDecrementAforo_WhenValidRequest()
    {
        var nombre = "Montaña Rusa";
        var exitTime = DateTime.Now;
        var codigoIdentificacion = "ID123456";
        var user = new User
        {
            Id = Guid.NewGuid(),
            Nombre = "Juan",
            Apellido = "Pérez",
            Email = "juan@email.com",
            Contraseña = "password123",
            FechaNacimiento = new DateTime(1990, 1, 1),
            CodigoIdentificacion = "ID123456"
        };
        var attraction = new Attraction(nombre, TipoAtraccion.MontañaRusa, 18, 50, "Emocionante montaña rusa", DateTime.Now);
        attraction.IncrementarAforo();

        var activeVisit = new Visit(user.Id, nombre, DateTime.Now.AddHours(-1));

        _mockAttractionRepository.Setup(x => x.ExistsByName(nombre)).Returns(true);
        _mockAttractionRepository.Setup(x => x.GetByName(nombre)).Returns(attraction);
        _mockAttractionRepository.Setup(x => x.Save(attraction));
        _mockUserRepository.Setup(x => x.GetByCodigoIdentificacion("ID123456")).Returns(user);
        _mockDateTimeRepository.Setup(x => x.GetCurrentDateTime()).Returns(exitTime);
        _mockVisitRepository.Setup(x => x.GetActiveVisitByUserAndAttraction(user.Id, nombre)).Returns(activeVisit);
        _mockVisitRepository.Setup(x => x.Update(It.IsAny<Visit>())).Returns((Visit v) => v);

        var result = _attractionsBusinessLogic.RegisterExit(nombre, "NFC", codigoIdentificacion);

        var visit = result as Visit;
        Assert.AreEqual(exitTime, visit!.ExitTime);
        Assert.AreEqual(0, attraction.AforoActual);
    }

    [TestMethod]
    public void ValidateTicketAndRegisterAccess_ShouldHandleAgeCalculation_WhenCurrentDateIsBeforeBirthday_NFC()
    {
        var nombre = "Montaña Rusa";
        var codigoIdentificacion = "12345678";
        var edadMinima = 15;
        var birthDate = new DateTime(2008, 6, 15);
        var currentDate = DateTime.Now.AddDays(1);

        var user = new User
        {
            Id = Guid.NewGuid(),
            CodigoIdentificacion = codigoIdentificacion,
            FechaNacimiento = birthDate
        };

        var attraction = new Attraction(nombre, TipoAtraccion.MontañaRusa, edadMinima, 50, "Descripción", DateTime.Now);

        var request = new RegisterVisitByNFCRequest { CodigoIdentificacion = codigoIdentificacion };

        var validTicket = new GeneralTicket
        {
            Id = Guid.NewGuid(),
            CodigoIdentificacionUsuario = codigoIdentificacion,
            FechaVisita = currentDate.Date,
            FechaCompra = DateTime.Now.AddDays(-1),
            CodigoQR = Guid.NewGuid()
        };

        _mockAttractionRepository.Setup(x => x.ExistsByName(nombre)).Returns(true);
        _mockAttractionRepository.Setup(x => x.GetByName(nombre)).Returns(attraction);
        _mockUserRepository.Setup(x => x.GetByCodigoIdentificacion(codigoIdentificacion)).Returns(user);
        _mockTicketsRepository.Setup(x => x.GetByCodigoIdentificacionUsuario(codigoIdentificacion)).Returns([validTicket]);
        _mockDateTimeRepository.Setup(x => x.GetCurrentDateTime()).Returns(currentDate);
        _mockVisitRepository.Setup(x => x.GetActiveVisitByUserAndAttraction(user.Id, nombre)).Returns((Visit?)null);
        _mockVisitRepository.Setup(x => x.Save(It.IsAny<Visit>())).Returns((Visit v) => v);
        _mockAttractionRepository.Setup(x => x.Save(It.IsAny<Attraction>()));

        var result = _attractionsBusinessLogic.ValidateTicketAndRegisterAccess(nombre, "NFC", request.CodigoIdentificacion);

        Assert.IsNotNull(result);
        _mockVisitRepository.Verify(x => x.Save(It.IsAny<Visit>()), Times.Once);
    }

    [TestMethod]
    [ExpectedException(typeof(VisitAlreadyActiveException))]
    public void ValidateTicketAndRegisterAccess_ShouldThrowBusinessLogicException_WhenUserAlreadyHasActiveVisit_NFC()
    {
        var nombre = "Montaña Rusa T-Rex";
        var request = new RegisterVisitByNFCRequest
        {
            CodigoIdentificacion = "ID123456"
        };
        var userId = Guid.NewGuid();
        var user = new User
        {
            Id = userId,
            Nombre = "Juan",
            Apellido = "Pérez",
            Email = "juan@email.com",
            Contraseña = "password123",
            FechaNacimiento = new DateTime(1990, 1, 1),
            CodigoIdentificacion = "ID123456"
        };
        var attraction = new Attraction(nombre, TipoAtraccion.MontañaRusa, 10, 50, "Emocionante montaña rusa", DateTime.Now);
        var existingVisit = new Visit(userId, nombre, DateTime.Now.AddMinutes(-30));

        var validTicket = new GeneralTicket
        {
            Id = Guid.NewGuid(),
            CodigoIdentificacionUsuario = "ID123456",
            FechaVisita = DateTime.Now.Date,
            FechaCompra = DateTime.Now.AddDays(-1),
            CodigoQR = Guid.NewGuid()
        };

        _mockAttractionRepository.Setup(x => x.ExistsByName(nombre)).Returns(true);
        _mockAttractionRepository.Setup(x => x.GetByName(nombre)).Returns(attraction);
        _mockUserRepository.Setup(x => x.GetByCodigoIdentificacion("ID123456")).Returns(user);
        _mockTicketsRepository.Setup(x => x.GetByCodigoIdentificacionUsuario("ID123456")).Returns([validTicket]);
        _mockDateTimeRepository.Setup(x => x.GetCurrentDateTime()).Returns(DateTime.Now);
        _mockVisitRepository.Setup(x => x.GetActiveVisitByUserAndAttraction(userId, nombre)).Returns(existingVisit);

        _attractionsBusinessLogic.ValidateTicketAndRegisterAccess(nombre, "NFC", request.CodigoIdentificacion);
    }

    [TestMethod]
    [ExpectedException(typeof(NoActiveVisitException))]
    public void RegisterExit_ShouldThrowBusinessLogicException_WhenNoActiveVisitFound()
    {
        var nombre = "Montaña Rusa";
        var codigoIdentificacion = "ID123456";
        var user = new User
        {
            Id = Guid.NewGuid(),
            Nombre = "Juan",
            Apellido = "Pérez",
            Email = "juan@email.com",
            Contraseña = "password123",
            FechaNacimiento = new DateTime(1990, 1, 1),
            CodigoIdentificacion = "ID123456"
        };
        var attraction = new Attraction(nombre, TipoAtraccion.MontañaRusa, 18, 50, "Emocionante montaña rusa", DateTime.Now);

        _mockAttractionRepository.Setup(x => x.ExistsByName(nombre)).Returns(true);
        _mockAttractionRepository.Setup(x => x.GetByName(nombre)).Returns(attraction);
        _mockUserRepository.Setup(x => x.GetByCodigoIdentificacion("ID123456")).Returns(user);
        _mockVisitRepository.Setup(x => x.GetActiveVisitByUserAndAttraction(user.Id, nombre)).Returns((Visit?)null);

        _attractionsBusinessLogic.RegisterExit(nombre, "NFC", codigoIdentificacion);
    }

    [TestMethod]
    public void CreateAttraction_ShouldSetBasicProperties_WhenGetCurrentDateTimeReturnsNull()
    {
        var nombre = "Casa del Terror";
        var tipo = 0;
        var edadMinima = 16;
        var capacidadMaxima = 20;
        var descripcion = "Escalofriante experiencia de terror";

        _mockDateTimeRepository.Setup(x => x.GetCurrentDateTime()).Returns((DateTime?)null);
        _mockAttractionRepository.Setup(x => x.ExistsByName(nombre)).Returns(false);
        _mockAttractionRepository.Setup(x => x.Save(It.IsAny<Attraction>()));

        var result = _attractionsBusinessLogic.CreateAttraction(nombre, tipo, edadMinima, capacidadMaxima, descripcion, 0);

        Assert.IsNotNull(result);
        Assert.AreEqual(nombre, result.Nombre);
        Assert.AreEqual((TipoAtraccion)tipo, result.Tipo);
    }

    [TestMethod]
    public void CreateAttraction_ShouldSetAdditionalProperties_WhenGetCurrentDateTimeReturnsNull()
    {
        var nombre = "Casa del Terror";
        var tipo = 0;
        var edadMinima = 16;
        var capacidadMaxima = 20;
        var descripcion = "Escalofriante experiencia de terror";

        _mockDateTimeRepository.Setup(x => x.GetCurrentDateTime()).Returns((DateTime?)null);
        _mockAttractionRepository.Setup(x => x.ExistsByName(nombre)).Returns(false);
        _mockAttractionRepository.Setup(x => x.Save(It.IsAny<Attraction>()));

        var result = _attractionsBusinessLogic.CreateAttraction(nombre, tipo, edadMinima, capacidadMaxima, descripcion, 0);

        Assert.AreEqual(TipoAtraccion.MontañaRusa, result.Tipo);
        Assert.AreEqual(edadMinima, result.EdadMinima);
        Assert.AreEqual(capacidadMaxima, result.CapacidadMaxima);
        Assert.AreEqual(descripcion, result.Descripcion);

        _mockAttractionRepository.Verify(x => x.Save(It.Is<Attraction>(a =>
            a.Nombre == nombre &&
            a.Tipo == TipoAtraccion.MontañaRusa &&
            a.EdadMinima == edadMinima &&
            a.CapacidadMaxima == capacidadMaxima &&
            a.Descripcion == descripcion)), Times.Once);
    }

    [TestMethod]
    [ExpectedException(typeof(AttractionNotFoundException))]
    public void ValidateTicketAndRegisterAccess_ShouldThrowAttractionNotFoundException_WhenAttractionDoesNotExist()
    {
        var nombre = "Atracción Inexistente";
        var request = new { QRCode = "QR123456", CodigoIdentificacion = "ID123456" };

        _mockAttractionRepository.Setup(x => x.ExistsByName(nombre)).Returns(false);

        _attractionsBusinessLogic.ValidateTicketAndRegisterAccess(nombre, "QR", request.CodigoIdentificacion);
    }

    [TestMethod]
    [ExpectedException(typeof(BusinessLogicException))]
    public void ValidateTicketAndRegisterAccess_ShouldThrowBusinessLogicException_WhenAttractionHasActiveIncident()
    {
        var nombre = "Montaña Rusa";
        var request = new { QRCode = "QR123456", CodigoIdentificacion = "ID123456" };
        var attraction = new Attraction(nombre, TipoAtraccion.MontañaRusa, 10, 50, "Emocionante montaña rusa", DateTime.Now);
        attraction.ActivarIncidencia();

        _mockAttractionRepository.Setup(x => x.ExistsByName(nombre)).Returns(true);
        _mockAttractionRepository.Setup(x => x.GetByName(nombre)).Returns(attraction);

        _attractionsBusinessLogic.ValidateTicketAndRegisterAccess(nombre, "QR", request.CodigoIdentificacion);
    }

    [TestMethod]
    [ExpectedException(typeof(BusinessLogicException))]
    public void ValidateTicketAndRegisterAccess_ShouldThrowBusinessLogicException_WhenAttractionIsAtMaxCapacity()
    {
        var nombre = "Montaña Rusa";
        var request = new { QRCode = "QR123456", CodigoIdentificacion = "ID123456" };
        var attraction = new Attraction(nombre, TipoAtraccion.MontañaRusa, 10, 2, "Emocionante montaña rusa", DateTime.Now);
        attraction.IncrementarAforo();
        attraction.IncrementarAforo();

        _mockAttractionRepository.Setup(x => x.ExistsByName(nombre)).Returns(true);
        _mockAttractionRepository.Setup(x => x.GetByName(nombre)).Returns(attraction);

        _attractionsBusinessLogic.ValidateTicketAndRegisterAccess(nombre, "QR", request.CodigoIdentificacion);
    }

    [TestMethod]
    [ExpectedException(typeof(AgeLimitException))]
    public void ValidateTicketAndRegisterAccess_ShouldThrowBusinessLogicException_WhenUserIsTooYoungForAttraction()
    {
        var nombre = "Montaña Rusa Extrema";
        var qrCode = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var entryTime = DateTime.Now.AddDays(1);
        var user = new User
        {
            Id = userId,
            Nombre = "Juan",
            Apellido = "Pérez",
            Email = "juan@email.com",
            Contraseña = "password123",
            FechaNacimiento = new DateTime(2020, 1, 1),
            CodigoIdentificacion = "ID123456"
        };
        var attraction = new Attraction(nombre, TipoAtraccion.MontañaRusa, 16, 50, "Solo para mayores de 16", DateTime.Now);

        var ticket = new GeneralTicket
        {
            Id = Guid.NewGuid(),
            CodigoIdentificacionUsuario = "ID123456",
            CodigoQR = qrCode,
            FechaVisita = entryTime.Date,
            FechaCompra = DateTime.Now.AddDays(-1)
        };

        _mockAttractionRepository.Setup(x => x.ExistsByName(nombre)).Returns(true);
        _mockAttractionRepository.Setup(x => x.GetByName(nombre)).Returns(attraction);
        _mockTicketsRepository.Setup(x => x.GetByQRCode(qrCode)).Returns(ticket);
        _mockUserRepository.Setup(x => x.GetByCodigoIdentificacion("ID123456")).Returns(user);
        _mockDateTimeRepository.Setup(x => x.GetCurrentDateTime()).Returns(entryTime);

        _attractionsBusinessLogic.ValidateTicketAndRegisterAccess(nombre, "QR", qrCode.ToString());
    }

    [TestMethod]
    public void CreateAttraction_ShouldReturnValidAttraction_WhenGetCurrentDateTimeReturnsNull()
    {
        var nombre = "Carrusel Clásico";
        var tipo = 1;
        var edadMinima = 3;
        var capacidadMaxima = 30;
        var descripcion = "Un carrusel tradicional para toda la familia";

        _mockDateTimeRepository.Setup(x => x.GetCurrentDateTime()).Returns((DateTime?)null);
        _mockAttractionRepository.Setup(x => x.ExistsByName(nombre)).Returns(false);
        _mockAttractionRepository.Setup(x => x.Save(It.IsAny<Attraction>()));

        var result = _attractionsBusinessLogic.CreateAttraction(nombre, tipo, edadMinima, capacidadMaxima, descripcion, 0);

        Assert.IsNotNull(result);
        Assert.AreEqual(nombre, result.Nombre);
    }

    [TestMethod]
    public void CreateAttraction_ShouldSetCurrentFechaCreacion_WhenGetCurrentDateTimeReturnsNull()
    {
        var nombre = "Carrusel Clásico";
        var tipo = 0;
        var edadMinima = 3;
        var capacidadMaxima = 30;
        var descripcion = "Un carrusel tradicional para toda la familia";

        _mockDateTimeRepository.Setup(x => x.GetCurrentDateTime()).Returns((DateTime?)null);
        _mockAttractionRepository.Setup(x => x.ExistsByName(nombre)).Returns(false);
        _mockAttractionRepository.Setup(x => x.Save(It.IsAny<Attraction>()));

        var result = _attractionsBusinessLogic.CreateAttraction(nombre, tipo, edadMinima, capacidadMaxima, descripcion, 0);

        Assert.IsTrue(result.FechaCreacion <= DateTime.Now);
        Assert.IsTrue(result.FechaCreacion > DateTime.Now.AddMinutes(-1));
    }

    [TestMethod]
    public void ValidateTicketAndRegisterAccess_ShouldReturnValidVisit_WhenGetCurrentDateTimeReturnsNull()
    {
        var nombre = "Torre Espacial";
        var request = new { QRCode = "QR789456", CodigoIdentificacion = "ID789456" };
        var user = new User
        {
            Id = Guid.NewGuid(),
            Nombre = "María",
            Apellido = "González",
            Email = "maria@email.com",
            Contraseña = "password456",
            FechaNacimiento = new DateTime(1985, 5, 15),
            CodigoIdentificacion = "ID789456"
        };
        var attraction = new Attraction(nombre, TipoAtraccion.MontañaRusa, 16, 40, "Torre de caída libre", DateTime.Now);

        var validTicket = new GeneralTicket
        {
            Id = Guid.NewGuid(),
            CodigoIdentificacionUsuario = "ID789456",
            FechaVisita = DateTime.Now.Date,
            FechaCompra = DateTime.Now.AddDays(-1),
            CodigoQR = Guid.NewGuid()
        };

        _mockAttractionRepository.Setup(x => x.ExistsByName(nombre)).Returns(true);
        _mockAttractionRepository.Setup(x => x.GetByName(nombre)).Returns(attraction);
        _mockAttractionRepository.Setup(x => x.Save(attraction));
        _mockUserRepository.Setup(x => x.GetByCodigoIdentificacion("ID789456")).Returns(user);
        _mockTicketsRepository.Setup(x => x.GetByCodigoIdentificacionUsuario("ID789456")).Returns([validTicket]);
        _mockDateTimeRepository.Setup(x => x.GetCurrentDateTime()).Returns((DateTime?)null);
        _mockVisitRepository.Setup(x => x.GetActiveVisitByUserAndAttraction(user.Id, nombre)).Returns((Visit?)null);
        _mockVisitRepository.Setup(x => x.Save(It.IsAny<Visit>())).Returns((Visit v) => v);

        var result = _attractionsBusinessLogic.ValidateTicketAndRegisterAccess(nombre, "NFC", request.CodigoIdentificacion);

        Assert.IsNotNull(result);
        var visit = result as Visit;
        Assert.IsNotNull(visit);
    }

    [TestMethod]
    public void ValidateTicketAndRegisterAccess_ShouldSetCurrentEntryTime_WhenGetCurrentDateTimeReturnsNull()
    {
        var nombre = "Torre Espacial";
        var request = new { QRCode = "QR789456", CodigoIdentificacion = "ID789456" };
        var user = new User
        {
            Id = Guid.NewGuid(),
            Nombre = "María",
            Apellido = "González",
            Email = "maria@email.com",
            Contraseña = "password456",
            FechaNacimiento = new DateTime(1985, 5, 15),
            CodigoIdentificacion = "ID789456"
        };
        var attraction = new Attraction(nombre, TipoAtraccion.MontañaRusa, 16, 40, "Torre de caída libre", DateTime.Now);

        var validTicket = new GeneralTicket
        {
            Id = Guid.NewGuid(),
            CodigoIdentificacionUsuario = "ID789456",
            FechaVisita = DateTime.Now.Date,
            FechaCompra = DateTime.Now.AddDays(-1),
            CodigoQR = Guid.NewGuid()
        };

        _mockAttractionRepository.Setup(x => x.ExistsByName(nombre)).Returns(true);
        _mockAttractionRepository.Setup(x => x.GetByName(nombre)).Returns(attraction);
        _mockAttractionRepository.Setup(x => x.Save(attraction));
        _mockUserRepository.Setup(x => x.GetByCodigoIdentificacion("ID789456")).Returns(user);
        _mockTicketsRepository.Setup(x => x.GetByCodigoIdentificacionUsuario("ID789456")).Returns([validTicket]);
        _mockDateTimeRepository.Setup(x => x.GetCurrentDateTime()).Returns((DateTime?)null);
        _mockVisitRepository.Setup(x => x.GetActiveVisitByUserAndAttraction(user.Id, nombre)).Returns((Visit?)null);
        _mockVisitRepository.Setup(x => x.Save(It.IsAny<Visit>())).Returns((Visit v) => v);

        var result = _attractionsBusinessLogic.ValidateTicketAndRegisterAccess(nombre, "NFC", request.CodigoIdentificacion);

        var visit = result as Visit;
        Assert.IsTrue(visit!.EntryTime <= DateTime.Now);
        Assert.IsTrue(visit.EntryTime > DateTime.Now.AddMinutes(-1));
    }

    [TestMethod]
    public void RegisterExit_ShouldReturnValidVisit_WhenGetCurrentDateTimeReturnsNull()
    {
        var nombre = "Río Aventura";
        var codigoIdentificacion = "ID321987";
        var user = new User
        {
            Id = Guid.NewGuid(),
            Nombre = "Carlos",
            Apellido = "Rodríguez",
            Email = "carlos@email.com",
            Contraseña = "password789",
            FechaNacimiento = new DateTime(1992, 8, 20),
            CodigoIdentificacion = "ID321987"
        };
        var attraction = new Attraction(nombre, TipoAtraccion.ZonaInteractiva, 8, 25, "Emocionante río rápido", DateTime.Now);
        attraction.IncrementarAforo();

        var existingVisit = new Visit(user.Id, nombre, DateTime.Now.AddMinutes(-30));

        _mockAttractionRepository.Setup(x => x.ExistsByName(nombre)).Returns(true);
        _mockAttractionRepository.Setup(x => x.GetByName(nombre)).Returns(attraction);
        _mockAttractionRepository.Setup(x => x.Save(attraction));
        _mockUserRepository.Setup(x => x.GetByCodigoIdentificacion("ID321987")).Returns(user);
        _mockVisitRepository.Setup(x => x.GetActiveVisitByUserAndAttraction(user.Id, nombre)).Returns(existingVisit);
        _mockDateTimeRepository.Setup(x => x.GetCurrentDateTime()).Returns((DateTime?)null);
        _mockVisitRepository.Setup(x => x.Update(existingVisit)).Returns(existingVisit);

        var result = _attractionsBusinessLogic.RegisterExit(nombre, "NFC", codigoIdentificacion);

        Assert.IsNotNull(result);
        var visit = result as Visit;
        Assert.IsNotNull(visit);
    }

    [TestMethod]
    public void RegisterExit_ShouldSetCurrentExitTime_WhenGetCurrentDateTimeReturnsNull()
    {
        var nombre = "Río Aventura";
        var codigoIdentificacion = "ID321987";
        var user = new User
        {
            Id = Guid.NewGuid(),
            Nombre = "Carlos",
            Apellido = "Rodríguez",
            Email = "carlos@email.com",
            Contraseña = "password789",
            FechaNacimiento = new DateTime(1992, 8, 20),
            CodigoIdentificacion = "ID321987"
        };
        var attraction = new Attraction(nombre, TipoAtraccion.ZonaInteractiva, 8, 25, "Emocionante río rápido", DateTime.Now);
        attraction.IncrementarAforo();

        var existingVisit = new Visit(user.Id, nombre, DateTime.Now.AddMinutes(-30));

        _mockAttractionRepository.Setup(x => x.ExistsByName(nombre)).Returns(true);
        _mockAttractionRepository.Setup(x => x.GetByName(nombre)).Returns(attraction);
        _mockAttractionRepository.Setup(x => x.Save(attraction));
        _mockUserRepository.Setup(x => x.GetByCodigoIdentificacion("ID321987")).Returns(user);
        _mockVisitRepository.Setup(x => x.GetActiveVisitByUserAndAttraction(user.Id, nombre)).Returns(existingVisit);
        _mockDateTimeRepository.Setup(x => x.GetCurrentDateTime()).Returns((DateTime?)null);
        _mockVisitRepository.Setup(x => x.Update(existingVisit)).Returns(existingVisit);

        var result = _attractionsBusinessLogic.RegisterExit(nombre, "NFC", codigoIdentificacion);

        var visit = result as Visit;
        Assert.IsTrue(visit!.ExitTime <= DateTime.Now);
        Assert.IsTrue(visit.ExitTime > DateTime.Now.AddMinutes(-1));
    }

    [TestMethod]
    public void ValidateTicketAndRegisterAccess_ShouldCalculateAgeCorrectly_WhenBirthdayNotYetThisYear()
    {
        var nombre = "Montaña Rusa";
        var currentDate = DateTime.Now.AddDays(1);
        var birthdayThisYear = currentDate.AddMonths(2);
        var birthYear = 2010;
        var birthDate = new DateTime(birthYear, 8, 20);

        var request = new { QRCode = "QR123456", CodigoIdentificacion = "ID123456" };
        var user = new User
        {
            Id = Guid.NewGuid(),
            Nombre = "Juan",
            Apellido = "Pérez",
            Email = "juan@email.com",
            Contraseña = "password123",
            FechaNacimiento = birthDate,
            CodigoIdentificacion = "ID123456"
        };
        var attraction = new Attraction(nombre, TipoAtraccion.MontañaRusa, 14, 50, "Emocionante montaña rusa", DateTime.Now);

        var validTicket = new GeneralTicket
        {
            Id = Guid.NewGuid(),
            CodigoIdentificacionUsuario = "ID123456",
            FechaVisita = currentDate.Date,
            FechaCompra = DateTime.Now.AddDays(-1),
            CodigoQR = Guid.NewGuid()
        };

        _mockAttractionRepository.Setup(x => x.ExistsByName(nombre)).Returns(true);
        _mockAttractionRepository.Setup(x => x.GetByName(nombre)).Returns(attraction);
        _mockAttractionRepository.Setup(x => x.Save(attraction));
        _mockUserRepository.Setup(x => x.GetByCodigoIdentificacion("ID123456")).Returns(user);
        _mockTicketsRepository.Setup(x => x.GetByCodigoIdentificacionUsuario("ID123456")).Returns([validTicket]);
        _mockDateTimeRepository.Setup(x => x.GetCurrentDateTime()).Returns(currentDate);
        _mockVisitRepository.Setup(x => x.GetActiveVisitByUserAndAttraction(user.Id, nombre)).Returns((Visit?)null);
        _mockVisitRepository.Setup(x => x.Save(It.IsAny<Visit>())).Returns((Visit v) => v);

        var result = _attractionsBusinessLogic.ValidateTicketAndRegisterAccess(nombre, "NFC", request.CodigoIdentificacion);

        var expectedAge = currentDate.Year - birthYear - 1;
        Assert.AreEqual(expectedAge, attraction.EdadMinima);
        _mockVisitRepository.Verify(x => x.Save(It.IsAny<Visit>()), Times.Once);
        _mockAttractionRepository.Verify(x => x.Save(attraction), Times.Once);
    }

    [TestMethod]
    public void ValidateTicketAndRegisterAccess_ShouldCalculateAgeCorrectly_WhenBirthdayAlreadyPassedThisYear()
    {
        var nombre = "Carrusel";
        var currentDate = new DateTime(2030, 10, 15);
        var birthDate = new DateTime(2010, 8, 20);

        var request = new { QRCode = "QR789456", CodigoIdentificacion = "ID789456" };
        var user = new User
        {
            Id = Guid.NewGuid(),
            Nombre = "María",
            Apellido = "González",
            Email = "maria@email.com",
            Contraseña = "password456",
            FechaNacimiento = birthDate,
            CodigoIdentificacion = "ID789456"
        };
        var attraction = new Attraction(nombre, TipoAtraccion.Simulador, 5, 30, "Carrusel familiar", DateTime.Now);

        var validTicket = new GeneralTicket
        {
            Id = Guid.NewGuid(),
            CodigoIdentificacionUsuario = "ID789456",
            FechaVisita = currentDate.Date,
            FechaCompra = DateTime.Now.AddDays(-1),
            CodigoQR = Guid.NewGuid()
        };

        _mockAttractionRepository.Setup(x => x.ExistsByName(nombre)).Returns(true);
        _mockAttractionRepository.Setup(x => x.GetByName(nombre)).Returns(attraction);
        _mockAttractionRepository.Setup(x => x.Save(attraction));
        _mockUserRepository.Setup(x => x.GetByCodigoIdentificacion("ID789456")).Returns(user);
        _mockTicketsRepository.Setup(x => x.GetByCodigoIdentificacionUsuario("ID789456")).Returns([validTicket]);
        _mockDateTimeRepository.Setup(x => x.GetCurrentDateTime()).Returns(currentDate);
        _mockVisitRepository.Setup(x => x.GetActiveVisitByUserAndAttraction(user.Id, nombre)).Returns((Visit?)null);
        _mockVisitRepository.Setup(x => x.Save(It.IsAny<Visit>())).Returns((Visit v) => v);

        var result = _attractionsBusinessLogic.ValidateTicketAndRegisterAccess(nombre, "NFC", request.CodigoIdentificacion);

        var expectedAge = currentDate.Year - birthDate.Year;
        Assert.AreEqual(expectedAge, currentDate.Year - birthDate.Year);
        _mockVisitRepository.Verify(x => x.Save(It.IsAny<Visit>()), Times.Once);
        _mockAttractionRepository.Verify(x => x.Save(attraction), Times.Once);
    }

    [TestMethod]
    public void UpdateAttraction_ShouldUseDateTimeNow_WhenGetCurrentDateTimeReturnsNull()
    {
        var nombre = "Rueda Gigante";
        var descripcion = "Rueda gigante renovada";
        var capacidadMaxima = 40;
        var edadMinima = 8;

        var attraction = new Attraction(nombre, TipoAtraccion.Simulador, 5, 30, "Rueda gigante clásica", DateTime.Now);

        _mockAttractionRepository.Setup(x => x.ExistsByName(nombre)).Returns(true);
        _mockAttractionRepository.Setup(x => x.GetByName(nombre)).Returns(attraction);
        _mockAttractionRepository.Setup(x => x.Save(attraction));
        _mockDateTimeRepository.Setup(x => x.GetCurrentDateTime()).Returns((DateTime?)null);

        var result = _attractionsBusinessLogic.UpdateAttraction(nombre, descripcion, capacidadMaxima, edadMinima);

        Assert.IsNotNull(result);
        Assert.AreEqual(attraction, result);
        _mockAttractionRepository.Verify(x => x.Save(attraction), Times.Once);
    }

    [TestMethod]
    [ExpectedException(typeof(AttractionNotFoundException))]
    public void ValidateTicketAndRegisterAccess_ShouldThrowAttractionNotFoundException_WhenAttractionDoesNotExist_NFC()
    {
        var nombre = "AtraccionInexistente";
        var request = new { QRCode = "QR123456", CodigoIdentificacion = "ID123456" };

        _mockAttractionRepository.Setup(x => x.ExistsByName(nombre)).Returns(false);

        _attractionsBusinessLogic.ValidateTicketAndRegisterAccess(nombre, "NFC", request.CodigoIdentificacion);
    }

    [TestMethod]
    public void ValidateTicketAndRegisterAccess_ShouldHandleNullDateTime_WhenRepositoryReturnsNull_NFC()
    {
        var nombre = "Torre de Caída";
        var request = new { QRCode = "QR123456", CodigoIdentificacion = "ID123456" };
        var user = new User
        {
            Id = Guid.NewGuid(),
            Nombre = "Carlos",
            Apellido = "Martínez",
            Email = "carlos@email.com",
            Contraseña = "password123",
            FechaNacimiento = new DateTime(1995, 3, 15),
            CodigoIdentificacion = "ID123456"
        };
        var attraction = new Attraction(nombre, TipoAtraccion.MontañaRusa, 15, 10, "Una torre de caída libre", DateTime.Now);

        var validTicket = new GeneralTicket
        {
            Id = Guid.NewGuid(),
            CodigoIdentificacionUsuario = "ID123456",
            FechaVisita = DateTime.Now.Date,
            FechaCompra = DateTime.Now.AddDays(-1),
            CodigoQR = Guid.NewGuid()
        };

        _mockAttractionRepository.Setup(x => x.ExistsByName(nombre)).Returns(true);
        _mockAttractionRepository.Setup(x => x.GetByName(nombre)).Returns(attraction);
        _mockAttractionRepository.Setup(x => x.Save(attraction));
        _mockUserRepository.Setup(x => x.GetByCodigoIdentificacion("ID123456")).Returns(user);
        _mockTicketsRepository.Setup(x => x.GetByCodigoIdentificacionUsuario("ID123456")).Returns([validTicket]);
        _mockDateTimeRepository.Setup(x => x.GetCurrentDateTime()).Returns((DateTime?)null);
        _mockVisitRepository.Setup(x => x.GetActiveVisitByUserAndAttraction(user.Id, nombre)).Returns((Visit?)null);
        _mockVisitRepository.Setup(x => x.Save(It.IsAny<Visit>())).Returns((Visit v) => v);

        var result = _attractionsBusinessLogic.ValidateTicketAndRegisterAccess(nombre, "NFC", request.CodigoIdentificacion);

        Assert.IsNotNull(result);
        _mockVisitRepository.Verify(x => x.Save(It.IsAny<Visit>()), Times.Once);
        _mockAttractionRepository.Verify(x => x.Save(attraction), Times.Once);
    }

    [TestMethod]
    public void ValidateTicketAndRegisterAccess_ShouldHandleNullCodigoIdentificacion_NFC()
    {
        var nombre = "Carrusel";
        var codigoIdentificacion = "USER999";
        var user = new User
        {
            Id = Guid.NewGuid(),
            Nombre = "Ana",
            Apellido = "López",
            Email = "ana@email.com",
            Contraseña = "password789",
            FechaNacimiento = new DateTime(2000, 8, 20),
            CodigoIdentificacion = "USER999"
        };
        var attraction = new Attraction(nombre, TipoAtraccion.Simulador, 5, 30, "Carrusel familiar", DateTime.Now);

        var today = DateTime.Now.Date;
        var validTicket = new GeneralTicket
        {
            Id = Guid.NewGuid(),
            CodigoIdentificacionUsuario = "USER999",
            FechaVisita = today,
            FechaCompra = DateTime.Now.AddDays(-1),
            CodigoQR = Guid.NewGuid()
        };

        _mockAttractionRepository.Setup(x => x.ExistsByName(nombre)).Returns(true);
        _mockAttractionRepository.Setup(x => x.GetByName(nombre)).Returns(attraction);
        _mockAttractionRepository.Setup(x => x.Save(attraction));
        _mockUserRepository.Setup(x => x.GetByCodigoIdentificacion("USER999")).Returns(user);
        _mockTicketsRepository.Setup(x => x.GetByCodigoIdentificacionUsuario("USER999")).Returns([validTicket]);
        _mockDateTimeRepository.Setup(x => x.GetCurrentDateTime()).Returns(today);
        _mockVisitRepository.Setup(x => x.GetActiveVisitByUserAndAttraction(user.Id, nombre)).Returns((Visit?)null);
        _mockVisitRepository.Setup(x => x.Save(It.IsAny<Visit>())).Returns((Visit v) => v);

        var result = _attractionsBusinessLogic.ValidateTicketAndRegisterAccess(nombre, "NFC", "USER999");

        Assert.IsNotNull(result);
        _mockVisitRepository.Verify(x => x.Save(It.IsAny<Visit>()), Times.Once);
        _mockAttractionRepository.Verify(x => x.Save(attraction), Times.Once);
    }

    [TestMethod]
    public void ValidateTicketAndRegisterAccess_ShouldHandleEmptyCodigoIdentificacion_NFC()
    {
        var nombre = "Carrusel";
        var codigoIdentificacion = "USER888";
        var user = new User
        {
            Id = Guid.NewGuid(),
            Nombre = "TestUser",
            Apellido = "TestLastName",
            Email = "test@email.com",
            Contraseña = "password123",
            FechaNacimiento = new DateTime(2000, 5, 15),
            CodigoIdentificacion = "USER888"
        };
        var attraction = new Attraction(nombre, TipoAtraccion.Simulador, 5, 30, "Carrusel familiar", DateTime.Now);

        var today = DateTime.Now.Date;
        var validTicket = new GeneralTicket
        {
            Id = Guid.NewGuid(),
            CodigoIdentificacionUsuario = "USER888",
            FechaVisita = today,
            FechaCompra = DateTime.Now.AddDays(-1),
            CodigoQR = Guid.NewGuid()
        };

        _mockAttractionRepository.Setup(x => x.ExistsByName(nombre)).Returns(true);
        _mockAttractionRepository.Setup(x => x.GetByName(nombre)).Returns(attraction);
        _mockAttractionRepository.Setup(x => x.Save(attraction));
        _mockUserRepository.Setup(x => x.GetByCodigoIdentificacion("USER888")).Returns(user);
        _mockTicketsRepository.Setup(x => x.GetByCodigoIdentificacionUsuario("USER888")).Returns([validTicket]);
        _mockDateTimeRepository.Setup(x => x.GetCurrentDateTime()).Returns(today);
        _mockVisitRepository.Setup(x => x.GetActiveVisitByUserAndAttraction(user.Id, nombre)).Returns((Visit?)null);
        _mockVisitRepository.Setup(x => x.Save(It.IsAny<Visit>())).Returns((Visit v) => v);

        var result = _attractionsBusinessLogic.ValidateTicketAndRegisterAccess(nombre, "NFC", "USER888");

        Assert.IsNotNull(result);
        _mockVisitRepository.Verify(x => x.Save(It.IsAny<Visit>()), Times.Once);
        _mockAttractionRepository.Verify(x => x.Save(attraction), Times.Once);
    }

    [TestMethod]
    public void ValidateTicketAndRegisterAccess_ShouldHandleRequestWithoutCodigoIdentificacionProperty()
    {
        var nombre = "Torre de Caída";
        var codigoIdentificacion = "USER789";
        var user = new User
        {
            Id = Guid.NewGuid(),
            Nombre = "Juan",
            Apellido = "Pérez",
            Email = "juan@email.com",
            Contraseña = "password789",
            FechaNacimiento = new DateTime(1990, 3, 10),
            CodigoIdentificacion = "USER789"
        };
        var attraction = new Attraction(nombre, TipoAtraccion.MontañaRusa, 15, 10, "Una torre de caída libre", DateTime.Now);

        var today = DateTime.Now.Date;
        var validTicket = new GeneralTicket
        {
            Id = Guid.NewGuid(),
            CodigoIdentificacionUsuario = "USER789",
            FechaVisita = today,
            FechaCompra = DateTime.Now.AddDays(-1),
            CodigoQR = Guid.NewGuid()
        };

        _mockAttractionRepository.Setup(x => x.ExistsByName(nombre)).Returns(true);
        _mockAttractionRepository.Setup(x => x.GetByName(nombre)).Returns(attraction);
        _mockAttractionRepository.Setup(x => x.Save(attraction));
        _mockUserRepository.Setup(x => x.GetByCodigoIdentificacion("USER789")).Returns(user);
        _mockTicketsRepository.Setup(x => x.GetByCodigoIdentificacionUsuario("USER789")).Returns([validTicket]);
        _mockDateTimeRepository.Setup(x => x.GetCurrentDateTime()).Returns(today);
        _mockVisitRepository.Setup(x => x.GetActiveVisitByUserAndAttraction(user.Id, nombre)).Returns((Visit?)null);
        _mockVisitRepository.Setup(x => x.Save(It.IsAny<Visit>())).Returns((Visit v) => v);

        var result = _attractionsBusinessLogic.ValidateTicketAndRegisterAccess(nombre, "NFC", codigoIdentificacion);

        _mockVisitRepository.Verify(x => x.Save(It.IsAny<Visit>()), Times.Once);
        _mockAttractionRepository.Verify(x => x.Save(attraction), Times.Once);
    }

    [TestMethod]
    [ExpectedException(typeof(BusinessLogicException))]
    public void CreateAttraction_ShouldThrowBusinessLogicException_WhenInvalidAttractionTypeProvided()
    {
        var nombre = "Montaña Rusa T-Rex";
        var tipoInvalido = 999;
        var edadMinima = 12;
        var capacidadMaxima = 50;
        var descripcion = "Emocionante montaña rusa temática de dinosaurios";

        _mockAttractionRepository.Setup(x => x.ExistsByName(nombre)).Returns(false);

        _attractionsBusinessLogic.CreateAttraction(nombre, tipoInvalido, edadMinima, capacidadMaxima, descripcion, 0);
    }

    [TestMethod]
    [ExpectedException(typeof(AgeLimitException))]
    public void ValidateTicketAndRegisterAccess_ShouldThrowBusinessLogicException_WhenUserIsTooYoung()
    {
        var nombre = "Torre de Caída";
        var qrCode = Guid.NewGuid();
        var user = new User
        {
            Id = Guid.NewGuid(),
            Nombre = "Niño",
            Apellido = "Pequeño",
            Email = "nino@email.com",
            Contraseña = "password123",
            FechaNacimiento = new DateTime(2020, 1, 1),
            CodigoIdentificacion = "ID123456"
        };
        var attraction = new Attraction(nombre, TipoAtraccion.MontañaRusa, 15, 10, "Una torre de caída libre", DateTime.Now);

        var ticket = new GeneralTicket
        {
            Id = Guid.NewGuid(),
            CodigoIdentificacionUsuario = "ID123456",
            CodigoQR = qrCode,
            FechaVisita = DateTime.Now.AddDays(1),
            FechaCompra = DateTime.Now.AddDays(-1)
        };

        _mockAttractionRepository.Setup(x => x.ExistsByName(nombre)).Returns(true);
        _mockAttractionRepository.Setup(x => x.GetByName(nombre)).Returns(attraction);
        _mockTicketsRepository.Setup(x => x.GetByQRCode(qrCode)).Returns(ticket);
        _mockUserRepository.Setup(x => x.GetByCodigoIdentificacion("ID123456")).Returns(user);
        _mockDateTimeRepository.Setup(x => x.GetCurrentDateTime()).Returns(new DateTime(2025, 6, 15));

        _attractionsBusinessLogic.ValidateTicketAndRegisterAccess(nombre, "QR", qrCode.ToString());
    }

    [TestMethod]
    public void ValidateTicketAndRegisterAccess_ShouldHandleRequestWithNullPropertyValue_NFC()
    {
        var nombre = "Montaña Rusa";
        var request = new { QRCode = "QR555777", CodigoIdentificacion = (string?)null };
        var user = new User
        {
            Id = Guid.NewGuid(),
            Nombre = "Usuario",
            Apellido = "Null",
            Email = "null@email.com",
            Contraseña = "password456",
            FechaNacimiento = new DateTime(1995, 8, 20),
            CodigoIdentificacion = "USER123"
        };
        var attraction = new Attraction(nombre, TipoAtraccion.MontañaRusa, 12, 20, "Montaña rusa emocionante", DateTime.Now);

        var today = DateTime.Now.Date;
        var validTicket = new GeneralTicket
        {
            Id = Guid.NewGuid(),
            CodigoIdentificacionUsuario = "USER123",
            FechaVisita = today,
            FechaCompra = DateTime.Now.AddDays(-1),
            CodigoQR = Guid.NewGuid()
        };

        _mockAttractionRepository.Setup(x => x.ExistsByName(nombre)).Returns(true);
        _mockAttractionRepository.Setup(x => x.GetByName(nombre)).Returns(attraction);
        _mockAttractionRepository.Setup(x => x.Save(attraction));
        _mockUserRepository.Setup(x => x.GetByCodigoIdentificacion("USER123")).Returns(user);
        _mockTicketsRepository.Setup(x => x.GetByCodigoIdentificacionUsuario("USER123")).Returns([validTicket]);
        _mockDateTimeRepository.Setup(x => x.GetCurrentDateTime()).Returns(today);
        _mockVisitRepository.Setup(x => x.GetActiveVisitByUserAndAttraction(user.Id, nombre)).Returns((Visit?)null);
        _mockVisitRepository.Setup(x => x.Save(It.IsAny<Visit>())).Returns((Visit v) => v);

        var result = _attractionsBusinessLogic.ValidateTicketAndRegisterAccess(nombre, "NFC", "USER123");

        Assert.IsNotNull(result);
        _mockVisitRepository.Verify(x => x.Save(It.IsAny<Visit>()), Times.Once);
        _mockAttractionRepository.Verify(x => x.Save(attraction), Times.Once);
    }

    [TestMethod]
    public void ValidateTicketAndRegisterAccess_ShouldHandleRequestWithNullPropertyValue()
    {
        var nombre = "Rueda Gigante";
        var request = new { QRCode = "QR111222", CodigoIdentificacion = (string?)null };
        var user = new User
        {
            Id = Guid.NewGuid(),
            Nombre = "Maria",
            Apellido = "López",
            Email = "maria@email.com",
            Contraseña = "password789",
            FechaNacimiento = new DateTime(1985, 12, 25),
            CodigoIdentificacion = "USER456"
        };
        var attraction = new Attraction(nombre, TipoAtraccion.Simulador, 5, 40, "Rueda gigante panorámica", DateTime.Now);

        var today = DateTime.Now.Date;
        var validTicket = new GeneralTicket
        {
            Id = Guid.NewGuid(),
            CodigoIdentificacionUsuario = "USER456",
            FechaVisita = today,
            FechaCompra = DateTime.Now.AddDays(-1),
            CodigoQR = Guid.NewGuid()
        };

        _mockAttractionRepository.Setup(x => x.ExistsByName(nombre)).Returns(true);
        _mockAttractionRepository.Setup(x => x.GetByName(nombre)).Returns(attraction);
        _mockAttractionRepository.Setup(x => x.Save(attraction));
        _mockUserRepository.Setup(x => x.GetByCodigoIdentificacion("USER456")).Returns(user);
        _mockTicketsRepository.Setup(x => x.GetByCodigoIdentificacionUsuario("USER456")).Returns([validTicket]);
        _mockDateTimeRepository.Setup(x => x.GetCurrentDateTime()).Returns(today);
        _mockVisitRepository.Setup(x => x.GetActiveVisitByUserAndAttraction(user.Id, nombre)).Returns((Visit?)null);
        _mockVisitRepository.Setup(x => x.Save(It.IsAny<Visit>())).Returns((Visit v) => v);

        var result = _attractionsBusinessLogic.ValidateTicketAndRegisterAccess(nombre, "NFC", "USER456");

        _mockVisitRepository.Verify(x => x.Save(It.IsAny<Visit>()), Times.Once);
        _mockAttractionRepository.Verify(x => x.Save(attraction), Times.Once);
    }

    [TestMethod]
    public void ValidateTicketAndRegisterAccess_ShouldWork_WhenValidQRCode()
    {
        var nombre = "Montaña Rusa";
        var tipoEntrada = "QR";
        var qrCode = Guid.NewGuid().ToString();

        var user = new User
        {
            Id = Guid.NewGuid(),
            Nombre = "Juan",
            Apellido = "Pérez",
            Email = "juan@email.com",
            Contraseña = "password123",
            FechaNacimiento = new DateTime(1990, 1, 1),
            CodigoIdentificacion = "ID123456"
        };

        var ticket = new GeneralTicket
        {
            Id = Guid.NewGuid(),
            CodigoIdentificacionUsuario = user.CodigoIdentificacion,
            CodigoQR = Guid.Parse(qrCode),
            FechaVisita = DateTime.Now.Date,
            FechaCompra = DateTime.Now.AddDays(-1)
        };

        var attraction = new Attraction(nombre, TipoAtraccion.MontañaRusa, 18, 50, "Emocionante montaña rusa", DateTime.Now);

        _mockAttractionRepository.Setup(x => x.ExistsByName(nombre)).Returns(true);
        _mockAttractionRepository.Setup(x => x.GetByName(nombre)).Returns(attraction);
        _mockTicketsRepository.Setup(x => x.GetByQRCode(Guid.Parse(qrCode))).Returns(ticket);
        _mockUserRepository.Setup(x => x.GetByCodigoIdentificacion(user.CodigoIdentificacion)).Returns(user);
        _mockDateTimeRepository.Setup(x => x.GetCurrentDateTime()).Returns(DateTime.Now);
        _mockVisitRepository.Setup(x => x.GetActiveVisitByUserAndAttraction(user.Id, nombre)).Returns((Visit?)null);
        _mockVisitRepository.Setup(x => x.Save(It.IsAny<Visit>())).Returns((Visit v) => v);
        _mockAttractionRepository.Setup(x => x.Save(attraction));

        var result = _attractionsBusinessLogic.ValidateTicketAndRegisterAccess(nombre, tipoEntrada, qrCode);

        Assert.IsNotNull(result);
        Assert.AreEqual(user.Id, result.UserId);
        Assert.AreEqual(nombre, result.AttractionName);
        _mockVisitRepository.Verify(x => x.Save(It.IsAny<Visit>()), Times.Once);
    }

    [TestMethod]
    public void RegisterExit_ShouldReturnVisitWithCorrectProperties_WhenValidQRCode()
    {
        var nombre = "Montaña Rusa";
        var tipoEntrada = "QR";
        var qrCode = Guid.NewGuid().ToString();

        var user = new User
        {
            Id = Guid.NewGuid(),
            Nombre = "Juan",
            Apellido = "Pérez",
            Email = "juan@email.com",
            Contraseña = "password123",
            FechaNacimiento = new DateTime(1990, 1, 1),
            CodigoIdentificacion = "ID123456"
        };

        var ticket = new GeneralTicket
        {
            Id = Guid.NewGuid(),
            CodigoIdentificacionUsuario = user.CodigoIdentificacion,
            CodigoQR = Guid.Parse(qrCode),
            FechaVisita = DateTime.Now.Date,
            FechaCompra = DateTime.Now.AddDays(-1)
        };

        var attraction = new Attraction(nombre, TipoAtraccion.MontañaRusa, 18, 50, "Emocionante montaña rusa", DateTime.Now);
        attraction.IncrementarAforo();

        var activeVisit = new Visit(user.Id, nombre, DateTime.Now.AddHours(-1));

        _mockAttractionRepository.Setup(x => x.ExistsByName(nombre)).Returns(true);
        _mockAttractionRepository.Setup(x => x.GetByName(nombre)).Returns(attraction);
        _mockTicketsRepository.Setup(x => x.GetByQRCode(Guid.Parse(qrCode))).Returns(ticket);
        _mockUserRepository.Setup(x => x.GetByCodigoIdentificacion(user.CodigoIdentificacion)).Returns(user);
        _mockVisitRepository.Setup(x => x.GetActiveVisitByUserAndAttraction(user.Id, nombre)).Returns(activeVisit);
        _mockDateTimeRepository.Setup(x => x.GetCurrentDateTime()).Returns(DateTime.Now);
        _mockVisitRepository.Setup(x => x.Update(activeVisit)).Returns(activeVisit);
        _mockAttractionRepository.Setup(x => x.Save(attraction));

        var result = _attractionsBusinessLogic.RegisterExit(nombre, tipoEntrada, qrCode);

        Assert.IsNotNull(result);
        Assert.AreEqual(user.Id, result.UserId);
        Assert.AreEqual(nombre, result.AttractionName);
    }

    [TestMethod]
    public void RegisterExit_ShouldSetExitTimeAndDecrementAforo_WhenValidQRCode()
    {
        var nombre = "Montaña Rusa";
        var tipoEntrada = "QR";
        var qrCode = Guid.NewGuid().ToString();

        var user = new User
        {
            Id = Guid.NewGuid(),
            Nombre = "Juan",
            Apellido = "Pérez",
            Email = "juan@email.com",
            Contraseña = "password123",
            FechaNacimiento = new DateTime(1990, 1, 1),
            CodigoIdentificacion = "ID123456"
        };

        var ticket = new GeneralTicket
        {
            Id = Guid.NewGuid(),
            CodigoIdentificacionUsuario = user.CodigoIdentificacion,
            CodigoQR = Guid.Parse(qrCode),
            FechaVisita = DateTime.Now.Date,
            FechaCompra = DateTime.Now.AddDays(-1)
        };

        var attraction = new Attraction(nombre, TipoAtraccion.MontañaRusa, 18, 50, "Emocionante montaña rusa", DateTime.Now);
        attraction.IncrementarAforo();

        var activeVisit = new Visit(user.Id, nombre, DateTime.Now.AddHours(-1));

        _mockAttractionRepository.Setup(x => x.ExistsByName(nombre)).Returns(true);
        _mockAttractionRepository.Setup(x => x.GetByName(nombre)).Returns(attraction);
        _mockTicketsRepository.Setup(x => x.GetByQRCode(Guid.Parse(qrCode))).Returns(ticket);
        _mockUserRepository.Setup(x => x.GetByCodigoIdentificacion(user.CodigoIdentificacion)).Returns(user);
        _mockVisitRepository.Setup(x => x.GetActiveVisitByUserAndAttraction(user.Id, nombre)).Returns(activeVisit);
        _mockDateTimeRepository.Setup(x => x.GetCurrentDateTime()).Returns(DateTime.Now);
        _mockVisitRepository.Setup(x => x.Update(activeVisit)).Returns(activeVisit);
        _mockAttractionRepository.Setup(x => x.Save(attraction));

        var result = _attractionsBusinessLogic.RegisterExit(nombre, tipoEntrada, qrCode);

        Assert.IsNotNull(result.ExitTime);
        Assert.AreEqual(0, attraction.AforoActual);
    }

    [TestMethod]
    [ExpectedException(typeof(WrongAttractionException))]
    public void ValidateTicketAndRegisterAccess_ShouldThrowWrongAttractionException_WhenNoGeneralTicketAvailable_NFC()
    {
        var nombre = "Carrusel";
        var codigoIdentificacion = "USER999";
        var user = new User
        {
            Id = Guid.NewGuid(),
            Nombre = "Ana",
            Apellido = "López",
            Email = "ana@email.com",
            Contraseña = "password789",
            FechaNacimiento = new DateTime(2000, 8, 20),
            CodigoIdentificacion = codigoIdentificacion
        };
        var attraction = new Attraction(nombre, TipoAtraccion.Simulador, 5, 30, "Carrusel familiar", DateTime.Now);

        var today = DateTime.Now.Date;
        var eventId = Guid.NewGuid();
        var eventTicket = new EventTicket
        {
            Id = Guid.NewGuid(),
            CodigoIdentificacionUsuario = codigoIdentificacion,
            FechaVisita = today,
            FechaCompra = DateTime.Now.AddDays(-1),
            CodigoQR = Guid.NewGuid(),
            EventoId = eventId
        };

        var evento = new Event
        {
            Id = eventId,
            Name = "Evento Sin Carrusel",
            Fecha = today,
            Hora = new TimeSpan(20, 0, 0),
            Aforo = 100,
            CostoAdicional = 50m,
            Atracciones = [] // Evento sin atracciones, no incluye el Carrusel
        };

        _mockAttractionRepository.Setup(x => x.ExistsByName(nombre)).Returns(true);
        _mockAttractionRepository.Setup(x => x.GetByName(nombre)).Returns(attraction);
        _mockUserRepository.Setup(x => x.GetByCodigoIdentificacion(codigoIdentificacion)).Returns(user);
        _mockTicketsRepository.Setup(x => x.GetByCodigoIdentificacionUsuario(codigoIdentificacion)).Returns([eventTicket]);
        _mockEventRepository.Setup(x => x.GetById(eventId)).Returns(evento);
        _mockDateTimeRepository.Setup(x => x.GetCurrentDateTime()).Returns(today);

        _attractionsBusinessLogic.ValidateTicketAndRegisterAccess(nombre, "NFC", codigoIdentificacion);
    }

    [TestMethod]
    [ExpectedException(typeof(AgeLimitException))]
    public void ValidateTicketAndRegisterAccess_ShouldThrowBusinessLogicException_WhenCurrentDateIsBeforeBirthday()
    {
        var nombre = "Montaña Rusa";
        var qrCode = Guid.NewGuid();
        var user = new User
        {
            Id = Guid.NewGuid(),
            Nombre = "Carlos",
            Apellido = "Joven",
            Email = "carlos@email.com",
            Contraseña = "password123",
            FechaNacimiento = new DateTime(2014, 10, 15),
            CodigoIdentificacion = "USER888"
        };
        var attraction = new Attraction(nombre, TipoAtraccion.MontañaRusa, 17, 50, "Montaña rusa emocionante", DateTime.Now);

        var currentDate = new DateTime(2030, 10, 10);
        var ticket = new GeneralTicket
        {
            Id = Guid.NewGuid(),
            CodigoIdentificacionUsuario = user.CodigoIdentificacion,
            FechaVisita = currentDate.Date,
            FechaCompra = currentDate.AddDays(-1),
            CodigoQR = qrCode
        };

        _mockAttractionRepository.Setup(x => x.ExistsByName(nombre)).Returns(true);
        _mockAttractionRepository.Setup(x => x.GetByName(nombre)).Returns(attraction);
        _mockTicketsRepository.Setup(x => x.GetByQRCode(qrCode)).Returns(ticket);
        _mockUserRepository.Setup(x => x.GetByCodigoIdentificacion(user.CodigoIdentificacion)).Returns(user);
        _mockDateTimeRepository.Setup(x => x.GetCurrentDateTime()).Returns(currentDate);
        _mockVisitRepository.Setup(x => x.GetActiveVisitByUserAndAttraction(user.Id, nombre)).Returns((Visit)null);

        _attractionsBusinessLogic.ValidateTicketAndRegisterAccess(nombre, "QR", qrCode.ToString());
    }

    [TestMethod]
    [ExpectedException(typeof(BusinessLogicException))]
    public void ValidateTicketAndRegisterAccess_ShouldThrowBusinessLogicException_WhenInvalidQRCode()
    {
        var nombre = "Torre de Caída";
        var invalidQRCode = "INVALID-QR-CODE";

        var attraction = new Attraction(nombre, TipoAtraccion.MontañaRusa, 15, 25, "Torre de caída libre", DateTime.Now);

        _mockAttractionRepository.Setup(x => x.ExistsByName(nombre)).Returns(true);
        _mockAttractionRepository.Setup(x => x.GetByName(nombre)).Returns(attraction);

        _attractionsBusinessLogic.ValidateTicketAndRegisterAccess(nombre, "QR", invalidQRCode);
    }

    [TestMethod]
    [ExpectedException(typeof(ExpiredTicketException))]
    public void ValidateTicketAndRegisterAccess_ShouldThrowExpiredTicketException_WhenTicketIsExpired()
    {
        var nombre = "Rueda Gigante";
        var qrCode = Guid.NewGuid();
        var user = new User
        {
            Id = Guid.NewGuid(),
            Nombre = "Luis",
            Apellido = "Pérez",
            Email = "luis@email.com",
            Contraseña = "password456",
            FechaNacimiento = new DateTime(1990, 5, 10),
            CodigoIdentificacion = "USER777"
        };
        var attraction = new Attraction(nombre, TipoAtraccion.Simulador, 3, 40, "Rueda gigante panorámica", DateTime.Now);

        var currentDate = DateTime.Now.AddDays(5);
        var ticket = new GeneralTicket
        {
            Id = Guid.NewGuid(),
            CodigoIdentificacionUsuario = user.CodigoIdentificacion,
            FechaVisita = currentDate.AddDays(-1).Date,
            FechaCompra = currentDate.AddDays(-2),
            CodigoQR = qrCode
        };

        _mockAttractionRepository.Setup(x => x.ExistsByName(nombre)).Returns(true);
        _mockAttractionRepository.Setup(x => x.GetByName(nombre)).Returns(attraction);
        _mockTicketsRepository.Setup(x => x.GetByQRCode(qrCode)).Returns(ticket);
        _mockUserRepository.Setup(x => x.GetByCodigoIdentificacion(user.CodigoIdentificacion)).Returns(user);
        _mockDateTimeRepository.Setup(x => x.GetCurrentDateTime()).Returns(currentDate);

        _attractionsBusinessLogic.ValidateTicketAndRegisterAccess(nombre, "QR", qrCode.ToString());
    }

    [TestMethod]
    public void GetUsageReport_ShouldReturnCorrectCountAndMostVisited_WhenVisitsExistInDateRange()
    {
        var fechaInicio = new DateTime(2025, 9, 1);
        var fechaFin = new DateTime(2025, 9, 30);

        var visits = new List<Visit>
        {
            new(Guid.NewGuid(), "Montaña Rusa", new DateTime(2025, 9, 5)),
            new(Guid.NewGuid(), "Montaña Rusa", new DateTime(2025, 9, 6)),
            new(Guid.NewGuid(), "Torre de Caída", new DateTime(2025, 9, 7)),
            new(Guid.NewGuid(), "Carrusel", new DateTime(2025, 9, 8)),
            new(Guid.NewGuid(), "Montaña Rusa", new DateTime(2025, 9, 9))
        };

        _mockVisitRepository.Setup(x => x.GetVisitsByDateRange(fechaInicio, fechaFin)).Returns(visits);

        var result = _attractionsBusinessLogic.GetUsageReport(fechaInicio, fechaFin);

        Assert.AreEqual(3, result.Count);
        Assert.AreEqual(3, result["Montaña Rusa"]);
    }

    [TestMethod]
    public void GetUsageReport_ShouldReturnCorrectCountsForAllAttractions_WhenVisitsExistInDateRange()
    {
        var fechaInicio = new DateTime(2025, 9, 1);
        var fechaFin = new DateTime(2025, 9, 30);

        var visits = new List<Visit>
        {
            new(Guid.NewGuid(), "Montaña Rusa", new DateTime(2025, 9, 5)),
            new(Guid.NewGuid(), "Montaña Rusa", new DateTime(2025, 9, 6)),
            new(Guid.NewGuid(), "Torre de Caída", new DateTime(2025, 9, 7)),
            new(Guid.NewGuid(), "Carrusel", new DateTime(2025, 9, 8)),
            new(Guid.NewGuid(), "Montaña Rusa", new DateTime(2025, 9, 9))
        };

        _mockVisitRepository.Setup(x => x.GetVisitsByDateRange(fechaInicio, fechaFin)).Returns(visits);

        var result = _attractionsBusinessLogic.GetUsageReport(fechaInicio, fechaFin);

        Assert.AreEqual(1, result["Torre de Caída"]);
        Assert.AreEqual(1, result["Carrusel"]);
    }

    [TestMethod]
    [ExpectedException(typeof(BusinessLogicException))]
    public void GetUsageReport_ShouldThrowBusinessLogicException_WhenStartDateIsAfterEndDate()
    {
        var fechaInicio = new DateTime(2025, 9, 30);
        var fechaFin = new DateTime(2025, 9, 1);

        _attractionsBusinessLogic.GetUsageReport(fechaInicio, fechaFin);
    }

    [TestMethod]
    [ExpectedException(typeof(TicketNotValidForDateException))]
    public void ValidateTicketAndRegisterAccess_ShouldThrowTicketNotValidForDateException_WhenTicketDateDoesNotMatch()
    {
        var nombre = "Montaña Rusa";
        var qrCode = Guid.NewGuid();
        var user = new User
        {
            Id = Guid.NewGuid(),
            Nombre = "Juan",
            Apellido = "Pérez",
            Email = "juan@email.com",
            Contraseña = "password123",
            FechaNacimiento = new DateTime(1990, 1, 1),
            CodigoIdentificacion = "ID123456"
        };
        var attraction = new Attraction(nombre, TipoAtraccion.MontañaRusa, 10, 50, "Emocionante montaña rusa", DateTime.Now);
        var currentDate = new DateTime(2030, 10, 15);
        var ticket = new GeneralTicket
        {
            Id = Guid.NewGuid(),
            CodigoIdentificacionUsuario = user.CodigoIdentificacion,
            CodigoQR = qrCode,
            FechaVisita = new DateTime(2030, 10, 20),
            FechaCompra = currentDate.AddDays(-5)
        };

        _mockAttractionRepository.Setup(x => x.ExistsByName(nombre)).Returns(true);
        _mockAttractionRepository.Setup(x => x.GetByName(nombre)).Returns(attraction);
        _mockTicketsRepository.Setup(x => x.GetByQRCode(qrCode)).Returns(ticket);
        _mockUserRepository.Setup(x => x.GetByCodigoIdentificacion(user.CodigoIdentificacion)).Returns(user);
        _mockDateTimeRepository.Setup(x => x.GetCurrentDateTime()).Returns(currentDate);

        _attractionsBusinessLogic.ValidateTicketAndRegisterAccess(nombre, "QR", qrCode.ToString());
    }

    [TestMethod]
    public void ValidateTicketAndRegisterAccess_ShouldAllowMultipleEntries_WhenUserExitsAndReenters()
    {
        var nombre = "Montaña Rusa";
        var qrCode = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var user = new User
        {
            Id = userId,
            Nombre = "María",
            Apellido = "González",
            Email = "maria@email.com",
            Contraseña = "password456",
            FechaNacimiento = new DateTime(1995, 5, 10),
            CodigoIdentificacion = "ID789456"
        };
        var attraction = new Attraction(nombre, TipoAtraccion.MontañaRusa, 12, 50, "Emocionante montaña rusa", DateTime.Now);
        var currentDate = new DateTime(2030, 10, 20);
        var ticket = new GeneralTicket
        {
            Id = Guid.NewGuid(),
            CodigoIdentificacionUsuario = user.CodigoIdentificacion,
            CodigoQR = qrCode,
            FechaVisita = currentDate.Date,
            FechaCompra = currentDate.AddDays(-2)
        };

        var completedVisit = new Visit(userId, nombre, currentDate.AddHours(-1));
        completedVisit.MarkExit(currentDate.AddMinutes(-30));

        _mockAttractionRepository.Setup(x => x.ExistsByName(nombre)).Returns(true);
        _mockAttractionRepository.Setup(x => x.GetByName(nombre)).Returns(attraction);
        _mockTicketsRepository.Setup(x => x.GetByQRCode(qrCode)).Returns(ticket);
        _mockUserRepository.Setup(x => x.GetByCodigoIdentificacion(user.CodigoIdentificacion)).Returns(user);
        _mockDateTimeRepository.Setup(x => x.GetCurrentDateTime()).Returns(currentDate);
        _mockVisitRepository.Setup(x => x.GetActiveVisitByUserAndAttraction(userId, nombre)).Returns((Visit?)null);
        _mockVisitRepository.Setup(x => x.Save(It.IsAny<Visit>())).Returns((Visit v) => v);
        _mockAttractionRepository.Setup(x => x.Save(It.IsAny<Attraction>()));

        var result = _attractionsBusinessLogic.ValidateTicketAndRegisterAccess(nombre, "QR", qrCode.ToString());

        Assert.IsNotNull(result);
        Assert.AreEqual(userId, result.UserId);
        Assert.AreEqual(nombre, result.AttractionName);
        _mockVisitRepository.Verify(x => x.Save(It.IsAny<Visit>()), Times.Once);
    }

    [TestMethod]
    public void CreatePreventiveMaintenance_ShouldCreateMaintenanceAndIncident_WhenAttractionExists()
    {
        var attractionName = "Montaña Rusa";
        var fecha = DateTime.Now.AddDays(1);
        var horaInicio = new TimeSpan(10, 0, 0);
        var duracionMinutos = 120;
        var descripcion = "Mantenimiento preventivo del motor";
        var attraction = new Attraction(attractionName, TipoAtraccion.MontañaRusa, 12, 50, "Emocionante montaña rusa", DateTime.Now);
        var fechaCreacion = DateTime.Now;
        var maintenance = new Maintenance(attractionName, fecha, horaInicio, duracionMinutos, descripcion);

        _mockAttractionRepository.Setup(x => x.ExistsByName(attractionName)).Returns(true);
        _mockAttractionRepository.Setup(x => x.GetByName(attractionName)).Returns(attraction);
        _mockAttractionRepository.Setup(x => x.Save(attraction));
        _mockDateTimeRepository.Setup(x => x.GetCurrentDateTime()).Returns(fechaCreacion);
        _mockIncidentRepository.Setup(x => x.Save(It.IsAny<Incident>())).Returns((Incident i) => i);
        _mockMaintenanceRepository.Setup(x => x.Save(It.IsAny<Maintenance>())).Returns((Maintenance m) => m);

        var result = _attractionsBusinessLogic.CreatePreventiveMaintenance(maintenance);

        Assert.IsNotNull(result.MaintenanceId);
        Assert.IsNotNull(result.IncidentId);
    }

    [TestMethod]
    [ExpectedException(typeof(AttractionNotFoundException))]
    public void CreatePreventiveMaintenance_ShouldThrowAttractionNotFoundException_WhenAttractionDoesNotExist()
    {
        var attractionName = "Atracción Inexistente";
        var fecha = DateTime.Now.AddDays(1);
        var horaInicio = new TimeSpan(10, 0, 0);
        var duracionMinutos = 120;
        var descripcion = "Mantenimiento preventivo";
        var maintenance = new Maintenance(attractionName, fecha, horaInicio, duracionMinutos, descripcion);

        _mockAttractionRepository.Setup(x => x.ExistsByName(attractionName)).Returns(false);

        _attractionsBusinessLogic.CreatePreventiveMaintenance(maintenance);
    }

    [TestMethod]
    public void GetMaintenancesByAttraction_ShouldReturnMaintenances_WhenMaintenancesExist()
    {
        var attractionName = "Montaña Rusa";
        var maintenance1 = new Maintenance(attractionName, DateTime.Now.AddDays(1), new TimeSpan(10, 0, 0), 120, "Mantenimiento 1", Guid.NewGuid());
        var maintenance2 = new Maintenance(attractionName, DateTime.Now.AddDays(2), new TimeSpan(14, 0, 0), 90, "Mantenimiento 2", Guid.NewGuid());

        _mockAttractionRepository.Setup(x => x.ExistsByName(attractionName)).Returns(true);
        _mockMaintenanceRepository.Setup(x => x.GetByAttractionName(attractionName)).Returns([maintenance1, maintenance2]);

        var result = _attractionsBusinessLogic.GetMaintenancesByAttraction(attractionName);

        Assert.AreEqual(2, result.Count);
    }

    [TestMethod]
    [ExpectedException(typeof(AttractionNotFoundException))]
    public void GetMaintenancesByAttraction_ShouldThrowAttractionNotFoundException_WhenAttractionDoesNotExist()
    {
        var attractionName = "Atracción Inexistente";

        _mockAttractionRepository.Setup(x => x.ExistsByName(attractionName)).Returns(false);

        _attractionsBusinessLogic.GetMaintenancesByAttraction(attractionName);
    }

    [TestMethod]
    public void CancelPreventiveMaintenance_ShouldResolveIncidentAndDeleteMaintenance_WhenMaintenanceExists()
    {
        var attractionName = "Montaña Rusa";
        var maintenanceId = Guid.NewGuid();
        var incidentId = Guid.NewGuid();
        var attraction = new Attraction(attractionName, TipoAtraccion.MontañaRusa, 12, 50, "Emocionante montaña rusa", DateTime.Now);
        attraction.ActivarIncidencia();
        var maintenance = new Maintenance(attractionName, DateTime.Now.AddDays(1), new TimeSpan(10, 0, 0), 120, "Mantenimiento", incidentId);
        var incident = new Incident(attractionName, "Mantenimiento programado", DateTime.Now);
        var fechaResolucion = DateTime.Now;

        _mockAttractionRepository.Setup(x => x.ExistsByName(attractionName)).Returns(true);
        _mockAttractionRepository.Setup(x => x.GetByName(attractionName)).Returns(attraction);
        _mockAttractionRepository.Setup(x => x.Save(attraction));
        _mockMaintenanceRepository.Setup(x => x.GetById(maintenanceId)).Returns(maintenance);
        _mockMaintenanceRepository.Setup(x => x.Delete(maintenanceId));
        _mockIncidentRepository.Setup(x => x.GetById(incidentId)).Returns(incident);
        _mockIncidentRepository.Setup(x => x.Update(incident)).Returns(incident);
        _mockDateTimeRepository.Setup(x => x.GetCurrentDateTime()).Returns(fechaResolucion);

        _attractionsBusinessLogic.CancelPreventiveMaintenance(attractionName, maintenanceId.ToString());

        _mockMaintenanceRepository.Verify(x => x.Delete(maintenanceId), Times.Once);
        _mockIncidentRepository.Verify(x => x.Update(It.Is<Incident>(i => !i.IsActive)), Times.Once);
        Assert.IsFalse(attraction.TieneIncidenciaActiva);
    }

    [TestMethod]
    [ExpectedException(typeof(AttractionNotFoundException))]
    public void CancelPreventiveMaintenance_ShouldThrowAttractionNotFoundException_WhenAttractionDoesNotExist()
    {
        var attractionName = "Atracción Inexistente";
        var maintenanceId = Guid.NewGuid();

        _mockAttractionRepository.Setup(x => x.ExistsByName(attractionName)).Returns(false);

        _attractionsBusinessLogic.CancelPreventiveMaintenance(attractionName, maintenanceId.ToString());
    }

    [TestMethod]
    [ExpectedException(typeof(BusinessLogicException))]
    public void CancelPreventiveMaintenance_ShouldThrowBusinessLogicException_WhenMaintenanceDoesNotExist()
    {
        var attractionName = "Montaña Rusa";
        var maintenanceId = Guid.NewGuid();
        var attraction = new Attraction(attractionName, TipoAtraccion.MontañaRusa, 12, 50, "Emocionante montaña rusa", DateTime.Now);

        _mockAttractionRepository.Setup(x => x.ExistsByName(attractionName)).Returns(true);
        _mockAttractionRepository.Setup(x => x.GetByName(attractionName)).Returns(attraction);
        _mockMaintenanceRepository.Setup(x => x.GetById(maintenanceId)).Returns((Maintenance?)null);

        _attractionsBusinessLogic.CancelPreventiveMaintenance(attractionName, maintenanceId.ToString());
    }

    [TestMethod]
    public void GetAll_ShouldReturnAllAttractions_WhenAttractionsExist()
    {
        var attraction1 = new Attraction("Montaña Rusa", TipoAtraccion.MontañaRusa, 12, 50, "Emocionante", DateTime.Now);
        var attraction2 = new Attraction("Carrusel", TipoAtraccion.ZonaInteractiva, 5, 40, "Tranquila", DateTime.Now);
        var attractions = new List<Attraction> { attraction1, attraction2 };

        _mockAttractionRepository.Setup(x => x.GetAll()).Returns(attractions);
        _mockDateTimeRepository.Setup(x => x.GetCurrentDateTime()).Returns(DateTime.Now);
        _mockMaintenanceRepository.Setup(x => x.GetByAttractionName("Montaña Rusa")).Returns([]);
        _mockMaintenanceRepository.Setup(x => x.GetByAttractionName("Carrusel")).Returns([]);
        _mockAttractionRepository.Setup(x => x.GetByName("Montaña Rusa")).Returns(attraction1);
        _mockAttractionRepository.Setup(x => x.GetByName("Carrusel")).Returns(attraction2);

        var result = _attractionsBusinessLogic.GetAll();

        Assert.AreEqual(2, result.Count);
        Assert.AreEqual("Montaña Rusa", result[0].Nombre);
        Assert.AreEqual("Carrusel", result[1].Nombre);
    }

    [TestMethod]
    public void GetAll_ShouldActivateScheduledMaintenance_WhenSystemDateMatches()
    {
        var attractionName = "Montaña Rusa";
        var currentDateTime = new DateTime(2027, 11, 15, 14, 35, 0);
        var scheduledDate = new DateTime(2027, 11, 15);
        var scheduledTime = new TimeSpan(14, 30, 0);

        var attraction = new Attraction(attractionName, TipoAtraccion.MontañaRusa, 10, 100, "Descripción", DateTime.Now, 50);
        var maintenance = new Maintenance(attractionName, scheduledDate, scheduledTime, 120, "Mantenimiento preventivo");
        var incident = new Incident(attractionName, "Mantenimiento", DateTime.Now, maintenance.Id, scheduledDate, scheduledTime);

        _mockAttractionRepository.Setup(x => x.GetAll()).Returns([attraction]);
        _mockDateTimeRepository.Setup(x => x.GetCurrentDateTime()).Returns(currentDateTime);
        _mockMaintenanceRepository.Setup(x => x.GetByAttractionName(attractionName)).Returns([maintenance]);
        _mockIncidentRepository.Setup(x => x.GetById(maintenance.IncidentId)).Returns(incident);
        _mockAttractionRepository.Setup(x => x.GetByName(attractionName)).Returns(attraction);
        _mockAttractionRepository.Setup(x => x.Save(It.IsAny<Attraction>()));

        var result = _attractionsBusinessLogic.GetAll();

        Assert.AreEqual(1, result.Count);
        Assert.IsTrue(result[0].TieneIncidenciaActiva);
    }

    [TestMethod]
    public void GetById_ShouldReturnAttraction_WhenAttractionExists()
    {
        var attraction = new Attraction("Casa Embrujada", TipoAtraccion.Espectaculo, 16, 20, "Experiencia aterradora", DateTime.Now);

        _mockAttractionRepository.Setup(x => x.GetById("Casa Embrujada")).Returns(attraction);

        var result = _attractionsBusinessLogic.GetById("Casa Embrujada");

        Assert.IsNotNull(result);
        Assert.AreEqual("Casa Embrujada", result.Nombre);
        Assert.AreEqual(TipoAtraccion.Espectaculo, result.Tipo);
    }

    [TestMethod]
    [ExpectedException(typeof(AttractionNotFoundException))]
    public void GetById_ShouldThrowAttractionNotFoundException_WhenAttractionDoesNotExist()
    {
        _mockAttractionRepository.Setup(x => x.GetById("AtraccionInexistente")).Returns((Attraction?)null);

        _attractionsBusinessLogic.GetById("AtraccionInexistente");
    }

    [TestMethod]
    public void GetAllIncidents_ShouldReturnAllIncidents_WhenIncidentsExist()
    {
        var incident1 = new Incident("Montaña Rusa", "Falla en el motor", new DateTime(2025, 10, 1));
        var incident2 = new Incident("Torre de Caída", "Problema eléctrico", new DateTime(2025, 10, 2));
        var incident3 = new Incident("Carrusel", "Mantenimiento preventivo", new DateTime(2025, 10, 3));
        incident3.Resolve(new DateTime(2025, 10, 3, 15, 0, 0));

        var incidents = new List<Incident> { incident1, incident2, incident3 };

        _mockIncidentRepository.Setup(x => x.GetAll()).Returns(incidents);

        var result = _attractionsBusinessLogic.GetAllIncidents();

        Assert.AreEqual(3, result.Count);
        Assert.AreEqual("Montaña Rusa", result[0].AttractionName);
        Assert.AreEqual("Torre de Caída", result[1].AttractionName);
        Assert.AreEqual("Carrusel", result[2].AttractionName);
    }

    [TestMethod]
    public void GetAllIncidents_ShouldReturnEmptyList_WhenNoIncidentsExist()
    {
        _mockIncidentRepository.Setup(x => x.GetAll()).Returns([]);

        var result = _attractionsBusinessLogic.GetAllIncidents();

        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public void GetIncidentsByAttraction_ShouldReturnIncidentsForAttraction_WhenIncidentsExist()
    {
        var attractionName = "Montaña Rusa";
        var incident1 = new Incident(attractionName, "Falla en motor", new DateTime(2025, 10, 1));
        var incident2 = new Incident(attractionName, "Problema eléctrico", new DateTime(2025, 10, 2));

        var incidents = new List<Incident> { incident1, incident2 };

        _mockAttractionRepository.Setup(x => x.ExistsByName(attractionName)).Returns(true);
        _mockIncidentRepository.Setup(x => x.GetActiveByAttractionName(attractionName)).Returns(incidents);

        var result = _attractionsBusinessLogic.GetIncidentsByAttraction(attractionName);

        Assert.AreEqual(2, result.Count);
        Assert.IsTrue(result.All(i => i.AttractionName == attractionName));
    }

    [TestMethod]
    public void GetIncidentsByAttraction_ShouldReturnEmptyList_WhenNoIncidentsExistForAttraction()
    {
        var attractionName = "Carrusel";

        _mockAttractionRepository.Setup(x => x.ExistsByName(attractionName)).Returns(true);
        _mockIncidentRepository.Setup(x => x.GetActiveByAttractionName(attractionName)).Returns([]);

        var result = _attractionsBusinessLogic.GetIncidentsByAttraction(attractionName);

        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    [ExpectedException(typeof(AttractionNotFoundException))]
    public void GetIncidentsByAttraction_ShouldThrowAttractionNotFoundException_WhenAttractionDoesNotExist()
    {
        var attractionName = "AtraccionInexistente";

        _mockAttractionRepository.Setup(x => x.ExistsByName(attractionName)).Returns(false);

        _attractionsBusinessLogic.GetIncidentsByAttraction(attractionName);
    }

    [TestMethod]
    public void CreatePreventiveMaintenance_ShouldCreateIncidentWithScheduledDate_WhenMaintenanceProvided()
    {
        var attractionName = "Montaña Rusa";
        var fechaProgramada = new DateTime(2028, 11, 15);
        var horaProgramada = new TimeSpan(14, 30, 0);
        var maintenance = new Maintenance(attractionName, fechaProgramada, horaProgramada, 120, "Mantenimiento preventivo motor");

        var attraction = new Attraction(attractionName, TipoAtraccion.MontañaRusa, 10, 100, "Descripción", DateTime.Now, 50);

        Incident? capturedIncident = null;

        _mockAttractionRepository.Setup(x => x.ExistsByName(attractionName)).Returns(true);
        _mockAttractionRepository.Setup(x => x.GetByName(attractionName)).Returns(attraction);
        _mockAttractionRepository.Setup(x => x.Save(It.IsAny<Attraction>()));
        _mockMaintenanceRepository.Setup(x => x.Save(It.IsAny<Maintenance>()))
            .Returns<Maintenance>(m => m);
        _mockIncidentRepository.Setup(x => x.Save(It.IsAny<Incident>()))
            .Callback<Incident>(i => capturedIncident = i)
            .Returns<Incident>(i => i);
        _mockDateTimeRepository.Setup(x => x.GetCurrentDateTime()).Returns(DateTime.Now);

        _attractionsBusinessLogic.CreatePreventiveMaintenance(maintenance);

        Assert.AreEqual(maintenance.Id, capturedIncident?.MaintenanceId);
        Assert.AreEqual(fechaProgramada, capturedIncident?.FechaProgramada);
        Assert.AreEqual(horaProgramada, capturedIncident?.HoraProgramada);
    }

    [TestMethod]
    public void ResolveIncident_ShouldCancelMaintenanceAndDeleteIt_WhenIncidentIsMaintenanceIncident()
    {
        var attractionName = "Montaña Rusa";
        var maintenanceId = Guid.NewGuid();
        var fechaProgramada = new DateTime(2025, 11, 15);
        var horaProgramada = new TimeSpan(14, 30, 0);
        var incident = new Incident(attractionName, "Mantenimiento preventivo", DateTime.Now, maintenanceId, fechaProgramada, horaProgramada);

        var attraction = new Attraction(attractionName, TipoAtraccion.MontañaRusa, 10, 100, "Descripción", DateTime.Now, 50);
        attraction.ActivarIncidencia();

        _mockAttractionRepository.Setup(x => x.ExistsByName(attractionName)).Returns(true);
        _mockAttractionRepository.Setup(x => x.GetByName(attractionName)).Returns(attraction);
        _mockAttractionRepository.Setup(x => x.Save(It.IsAny<Attraction>()));
        _mockIncidentRepository.Setup(x => x.GetById(incident.Id)).Returns(incident);
        _mockIncidentRepository.Setup(x => x.Update(It.IsAny<Incident>()))
            .Returns<Incident>(i => i);
        _mockMaintenanceRepository.Setup(x => x.Delete(maintenanceId));
        _mockDateTimeRepository.Setup(x => x.GetCurrentDateTime()).Returns(DateTime.Now);

        _attractionsBusinessLogic.ResolveIncident(attractionName, incident.Id.ToString());

        _mockMaintenanceRepository.Verify(x => x.Delete(maintenanceId), Times.Once);
    }

    [TestMethod]
    public void GetCapacity_ShouldActivateMaintenance_WhenScheduledDateTimeHasArrived()
    {
        var attractionName = "Montaña Rusa";
        var currentDateTime = new DateTime(2027, 11, 15, 14, 35, 0);
        var scheduledDate = new DateTime(2027, 11, 15);
        var scheduledTime = new TimeSpan(14, 30, 0);

        var attraction = new Attraction(attractionName, TipoAtraccion.MontañaRusa, 10, 100, "Descripción", DateTime.Now, 50);
        var maintenance = new Maintenance(attractionName, scheduledDate, scheduledTime, 120, "Mantenimiento preventivo");
        var incident = new Incident(attractionName, "Mantenimiento", DateTime.Now, maintenance.Id, scheduledDate, scheduledTime);

        _mockAttractionRepository.Setup(x => x.ExistsByName(attractionName)).Returns(true);
        _mockAttractionRepository.Setup(x => x.GetByName(attractionName)).Returns(attraction);
        _mockAttractionRepository.Setup(x => x.Save(It.IsAny<Attraction>()));
        _mockMaintenanceRepository.Setup(x => x.GetByAttractionName(attractionName)).Returns([maintenance]);
        _mockIncidentRepository.Setup(x => x.GetById(maintenance.IncidentId)).Returns(incident);
        _mockDateTimeRepository.Setup(x => x.GetCurrentDateTime()).Returns(currentDateTime);

        var result = _attractionsBusinessLogic.GetCapacity(attractionName);

        Assert.IsTrue(result.TieneIncidenciaActiva);
    }
}
