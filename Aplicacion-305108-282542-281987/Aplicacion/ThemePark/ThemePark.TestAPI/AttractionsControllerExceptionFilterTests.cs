using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Moq;
using ThemePark.Exceptions;
using ThemePark.IBusinessLogic;
using ThemePark.IBusinessLogic.Exceptions;
using ThemePark.IDataAccess.Exceptions;
using ThemeParkApi.Controllers;
using ThemeParkApi.Filters;

namespace ThemePark.TestAPI;

[TestClass]
public class AttractionsControllerExceptionFilterTests
{
    private Mock<IAttractionsBusinessLogic> _mockAttractionsBusinessLogic = null!;
    private AttractionsController _controller = null!;
    private ExceptionFilter _exceptionFilter = null!;
    private ExceptionContext _exceptionContext = null!;
    private AuthenticationFilter _authenticationFilter = null!;
    private Mock<HttpContext> _httpContextMock = null!;
    private AuthorizationFilterContext _authorizationContext = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockAttractionsBusinessLogic = new Mock<IAttractionsBusinessLogic>(MockBehavior.Strict);
        _controller = new AttractionsController(_mockAttractionsBusinessLogic.Object);
        _exceptionFilter = new ExceptionFilter();
        _authenticationFilter = new AuthenticationFilter();
        _httpContextMock = new Mock<HttpContext>(MockBehavior.Strict);
        _authorizationContext = new AuthorizationFilterContext(
            new ActionContext(_httpContextMock.Object, new RouteData(), new ActionDescriptor()),
            []);
    }

    private void SetupExceptionContext(Exception exception)
    {
        _exceptionContext = new ExceptionContext(
            new ActionContext(
                new Mock<HttpContext>().Object,
                new RouteData(),
                new ActionDescriptor()),
            [])
        {
            Exception = exception
        };
    }

    private string GetMessage(object value)
    {
        return value.GetType().GetProperty("message")?.GetValue(value)?.ToString() ??
               value.GetType().GetProperty("Message")?.GetValue(value)?.ToString() ?? string.Empty;
    }

    private string GetTitle(object value)
    {
        return value.GetType().GetProperty("title")?.GetValue(value)?.ToString() ??
               value.GetType().GetProperty("Title")?.GetValue(value)?.ToString() ?? string.Empty;
    }

    [TestMethod]
    public void ExceptionFilter_ShouldReturnBadRequestResponse_WhenInvalidAttractionDataExceptionOccurs()
    {
        var exception = new InvalidAttractionDataException("Nombre", "vacío");
        SetupExceptionContext(exception);

        _exceptionFilter.OnException(_exceptionContext);

        var response = _exceptionContext.Result as ObjectResult;
        Assert.IsNotNull(response);
        Assert.AreEqual((int)HttpStatusCode.BadRequest, response.StatusCode);
    }

    [TestMethod]
    public void ExceptionFilter_ShouldReturnCorrectTitleAndMessage_WhenInvalidAttractionDataExceptionOccurs()
    {
        var exception = new InvalidAttractionDataException("Nombre", "vacío");
        SetupExceptionContext(exception);

        _exceptionFilter.OnException(_exceptionContext);

        var response = _exceptionContext.Result as ObjectResult;
        Assert.AreEqual("Datos de Atracción Inválidos", GetTitle(response!.Value!));
        Assert.AreEqual(exception.Message, GetMessage(response.Value!));
    }

    [TestMethod]
    public void ExceptionFilter_ShouldReturnNotFoundResponse_WhenAttractionNotFoundExceptionOccurs()
    {
        var attractionName = "Montaña Rusa";
        var exception = new AttractionNotFoundException(attractionName);
        SetupExceptionContext(exception);

        _exceptionFilter.OnException(_exceptionContext);

        var response = _exceptionContext.Result as ObjectResult;
        Assert.IsNotNull(response);
        Assert.AreEqual((int)HttpStatusCode.NotFound, response.StatusCode);
    }

    [TestMethod]
    public void ExceptionFilter_ShouldReturnCorrectTitleAndMessage_WhenAttractionNotFoundExceptionOccurs()
    {
        var attractionName = "Montaña Rusa";
        var exception = new AttractionNotFoundException(attractionName);
        SetupExceptionContext(exception);

        _exceptionFilter.OnException(_exceptionContext);

        var response = _exceptionContext.Result as ObjectResult;
        Assert.AreEqual("Atracción No Encontrada", GetTitle(response!.Value!));
        Assert.AreEqual(exception.Message, GetMessage(response.Value!));
    }

    [TestMethod]
    public void ExceptionFilter_ShouldReturnInternalServerErrorResponse_WhenUnknownExceptionOccurs()
    {
        var exception = new Exception("Unknown error");
        SetupExceptionContext(exception);

        _exceptionFilter.OnException(_exceptionContext);

        var response = _exceptionContext.Result as ObjectResult;
        Assert.IsNotNull(response);
        Assert.AreEqual((int)HttpStatusCode.InternalServerError, response.StatusCode);
    }

    [TestMethod]
    public void ExceptionFilter_ShouldReturnCorrectTitleAndMessage_WhenUnknownExceptionOccurs()
    {
        var exception = new Exception("Unknown error");
        SetupExceptionContext(exception);

        _exceptionFilter.OnException(_exceptionContext);

        var response = _exceptionContext.Result as ObjectResult;
        Assert.AreEqual("Error Interno del Servidor", GetTitle(response!.Value!));
        Assert.AreEqual("Ocurrió un error inesperado", GetMessage(response.Value!));
    }

    [TestMethod]
    public void ExceptionFilter_ShouldReturnBadRequestResponse_WhenBusinessLogicExceptionOccurs()
    {
        var exception = BusinessLogicException.AttractionAlreadyExists("Montaña Rusa");
        SetupExceptionContext(exception);

        _exceptionFilter.OnException(_exceptionContext);

        var response = _exceptionContext.Result as ObjectResult;
        Assert.IsNotNull(response);
        Assert.AreEqual((int)HttpStatusCode.BadRequest, response.StatusCode);
    }

    [TestMethod]
    public void ExceptionFilter_ShouldReturnCorrectTitleAndMessage_WhenBusinessLogicExceptionOccurs()
    {
        var exception = BusinessLogicException.AttractionAlreadyExists("Montaña Rusa");
        SetupExceptionContext(exception);

        _exceptionFilter.OnException(_exceptionContext);

        var response = _exceptionContext.Result as ObjectResult;
        Assert.AreEqual("Error de Lógica de Negocio", GetTitle(response!.Value!));
        Assert.AreEqual(exception.Message, GetMessage(response.Value!));
    }

    [TestMethod]
    public void ExceptionFilter_ShouldReturnBadRequestResponse_WhenUserRegistrationExceptionOccurs()
    {
        var exception = new UserRegistrationException("test@test.com");
        SetupExceptionContext(exception);

        _exceptionFilter.OnException(_exceptionContext);

        var response = _exceptionContext.Result as ObjectResult;
        Assert.IsNotNull(response);
        Assert.AreEqual(409, response.StatusCode);
    }

    [TestMethod]
    public void ExceptionFilter_ShouldReturnCorrectTitleAndMessage_WhenUserRegistrationExceptionOccurs()
    {
        var exception = new UserRegistrationException("test@test.com");
        SetupExceptionContext(exception);

        _exceptionFilter.OnException(_exceptionContext);

        var response = _exceptionContext.Result as ObjectResult;
        Assert.AreEqual("Email Duplicado", GetTitle(response!.Value!));
        Assert.AreEqual(exception.Message, GetMessage(response.Value!));
    }

    [TestMethod]
    public void ExceptionFilter_ShouldReturnNotFoundResponse_WhenEventNotFoundExceptionOccurs()
    {
        var exception = new EventNotFoundException(Guid.NewGuid());
        SetupExceptionContext(exception);

        _exceptionFilter.OnException(_exceptionContext);

        var response = _exceptionContext.Result as ObjectResult;
        Assert.IsNotNull(response);
        Assert.AreEqual((int)HttpStatusCode.NotFound, response.StatusCode);
    }

    [TestMethod]
    public void ExceptionFilter_ShouldReturnCorrectTitleAndMessage_WhenEventNotFoundExceptionOccurs()
    {
        var exception = new EventNotFoundException(Guid.NewGuid());
        SetupExceptionContext(exception);

        _exceptionFilter.OnException(_exceptionContext);

        var response = _exceptionContext.Result as ObjectResult;
        Assert.AreEqual("Evento No Encontrado", GetTitle(response!.Value!));
        Assert.AreEqual(exception.Message, GetMessage(response.Value!));
    }

    [TestMethod]
    public void ExceptionFilter_ShouldReturnNotFoundResponse_WhenIncidentNotFoundExceptionOccurs()
    {
        var exception = new IncidentNotFoundException(Guid.NewGuid().ToString());
        SetupExceptionContext(exception);

        _exceptionFilter.OnException(_exceptionContext);

        var response = _exceptionContext.Result as ObjectResult;
        Assert.IsNotNull(response);
        Assert.AreEqual((int)HttpStatusCode.NotFound, response.StatusCode);
    }

    [TestMethod]
    public void ExceptionFilter_ShouldReturnCorrectTitleAndMessage_WhenIncidentNotFoundExceptionOccurs()
    {
        var exception = new IncidentNotFoundException(Guid.NewGuid().ToString());
        SetupExceptionContext(exception);

        _exceptionFilter.OnException(_exceptionContext);

        var response = _exceptionContext.Result as ObjectResult;
        Assert.AreEqual("Incidencia No Encontrada", GetTitle(response!.Value!));
        Assert.AreEqual(exception.Message, GetMessage(response.Value!));
    }

    [TestMethod]
    public void ExceptionFilter_ShouldReturnBadRequestResponse_WhenInvalidIncidentDataExceptionOccurs()
    {
        var exception = new InvalidIncidentDataException("Descripcion");
        SetupExceptionContext(exception);

        _exceptionFilter.OnException(_exceptionContext);

        var response = _exceptionContext.Result as ObjectResult;
        Assert.IsNotNull(response);
        Assert.AreEqual((int)HttpStatusCode.BadRequest, response.StatusCode);
    }

    [TestMethod]
    public void ExceptionFilter_ShouldReturnCorrectTitleAndMessage_WhenInvalidIncidentDataExceptionOccurs()
    {
        var exception = new InvalidIncidentDataException("Descripcion");
        SetupExceptionContext(exception);

        _exceptionFilter.OnException(_exceptionContext);

        var response = _exceptionContext.Result as ObjectResult;
        Assert.AreEqual("Datos de Incidencia Inválidos", GetTitle(response!.Value!));
        Assert.AreEqual(exception.Message, GetMessage(response.Value!));
    }

    [TestMethod]
    public void ExceptionFilter_ShouldReturnNotFoundResponse_WhenTicketNotFoundExceptionOccurs()
    {
        var exception = new TicketNotFoundException(Guid.NewGuid());
        SetupExceptionContext(exception);

        _exceptionFilter.OnException(_exceptionContext);

        var response = _exceptionContext.Result as ObjectResult;
        Assert.IsNotNull(response);
        Assert.AreEqual((int)HttpStatusCode.NotFound, response.StatusCode);
    }

    [TestMethod]
    public void ExceptionFilter_ShouldReturnCorrectTitleAndMessage_WhenTicketNotFoundExceptionOccurs()
    {
        var exception = new TicketNotFoundException(Guid.NewGuid());
        SetupExceptionContext(exception);

        _exceptionFilter.OnException(_exceptionContext);

        var response = _exceptionContext.Result as ObjectResult;
        Assert.AreEqual("Ticket No Encontrado", GetTitle(response!.Value!));
        Assert.AreEqual(exception.Message, GetMessage(response.Value!));
    }

    [TestMethod]
    public void ExceptionFilter_ShouldReturnNotFoundResponse_WhenUserNotFoundExceptionOccurs()
    {
        var exception = new UserNotFoundException(Guid.NewGuid());
        SetupExceptionContext(exception);

        _exceptionFilter.OnException(_exceptionContext);

        var response = _exceptionContext.Result as ObjectResult;
        Assert.IsNotNull(response);
        Assert.AreEqual((int)HttpStatusCode.NotFound, response.StatusCode);
    }

    [TestMethod]
    public void ExceptionFilter_ShouldReturnCorrectTitleAndMessage_WhenUserNotFoundExceptionOccurs()
    {
        var exception = new UserNotFoundException(Guid.NewGuid());
        SetupExceptionContext(exception);

        _exceptionFilter.OnException(_exceptionContext);

        var response = _exceptionContext.Result as ObjectResult;
        Assert.AreEqual("Usuario No Encontrado", GetTitle(response!.Value!));
        Assert.AreEqual(exception.Message, GetMessage(response.Value!));
    }

    [TestMethod]
    public void ExceptionFilter_ShouldReturnNotFoundResponse_WhenVisitNotFoundExceptionOccurs()
    {
        var exception = new VisitNotFoundException(Guid.NewGuid());
        SetupExceptionContext(exception);

        _exceptionFilter.OnException(_exceptionContext);

        var response = _exceptionContext.Result as ObjectResult;
        Assert.IsNotNull(response);
        Assert.AreEqual((int)HttpStatusCode.NotFound, response.StatusCode);
    }

    [TestMethod]
    public void ExceptionFilter_ShouldReturnCorrectTitleAndMessage_WhenVisitNotFoundExceptionOccurs()
    {
        var exception = new VisitNotFoundException(Guid.NewGuid());
        SetupExceptionContext(exception);

        _exceptionFilter.OnException(_exceptionContext);

        var response = _exceptionContext.Result as ObjectResult;
        Assert.AreEqual("Visita No Encontrada", GetTitle(response!.Value!));
        Assert.AreEqual(exception.Message, GetMessage(response.Value!));
    }

    [TestMethod]
    public void ExceptionFilter_ShouldReturnConflictResponse_WhenActiveIncidentExceptionOccurs()
    {
        var exception = new ActiveIncidentException("Montaña Rusa");
        SetupExceptionContext(exception);

        _exceptionFilter.OnException(_exceptionContext);

        var response = _exceptionContext.Result as ObjectResult;
        Assert.IsNotNull(response);
        Assert.AreEqual((int)HttpStatusCode.Conflict, response.StatusCode);
    }

    [TestMethod]
    public void ExceptionFilter_ShouldReturnCorrectTitleAndMessage_WhenActiveIncidentExceptionOccurs()
    {
        var exception = new ActiveIncidentException("Montaña Rusa");
        SetupExceptionContext(exception);

        _exceptionFilter.OnException(_exceptionContext);

        var response = _exceptionContext.Result as ObjectResult;
        Assert.AreEqual("Incidencia Activa", GetTitle(response!.Value!));
        Assert.AreEqual(exception.Message, GetMessage(response.Value!));
    }

    [TestMethod]
    public void ExceptionFilter_ShouldReturnConflictResponse_WhenCapacityExceededExceptionOccurs()
    {
        var exception = new CapacityExceededException("Montaña Rusa", 50, 50);
        SetupExceptionContext(exception);

        _exceptionFilter.OnException(_exceptionContext);

        var response = _exceptionContext.Result as ObjectResult;
        Assert.IsNotNull(response);
        Assert.AreEqual((int)HttpStatusCode.Conflict, response.StatusCode);
    }

    [TestMethod]
    public void ExceptionFilter_ShouldReturnCorrectTitleAndMessage_WhenCapacityExceededExceptionOccurs()
    {
        var exception = new CapacityExceededException("Montaña Rusa", 50, 50);
        SetupExceptionContext(exception);

        _exceptionFilter.OnException(_exceptionContext);

        var response = _exceptionContext.Result as ObjectResult;
        Assert.AreEqual("Capacidad Excedida", GetTitle(response!.Value!));
        Assert.AreEqual(exception.Message, GetMessage(response.Value!));
    }

    [TestMethod]
    public void ExceptionFilter_ShouldReturnBadRequestResponse_WhenInvalidEventDataExceptionOccurs()
    {
        var exception = new InvalidEventDataException("Nombre", "vacío");
        SetupExceptionContext(exception);

        _exceptionFilter.OnException(_exceptionContext);

        var response = _exceptionContext.Result as ObjectResult;
        Assert.IsNotNull(response);
        Assert.AreEqual((int)HttpStatusCode.BadRequest, response.StatusCode);
    }

    [TestMethod]
    public void ExceptionFilter_ShouldReturnCorrectTitleAndMessage_WhenInvalidEventDataExceptionOccurs()
    {
        var exception = new InvalidEventDataException("Nombre", "vacío");
        SetupExceptionContext(exception);

        _exceptionFilter.OnException(_exceptionContext);

        var response = _exceptionContext.Result as ObjectResult;
        Assert.AreEqual("Datos de Evento Inválidos", GetTitle(response!.Value!));
        Assert.AreEqual(exception.Message, GetMessage(response.Value!));
    }

    [TestMethod]
    public void ExceptionFilter_ShouldReturn401_WhenInvalidTokenFormatExceptionOccurs()
    {
        var exception = new InvalidTokenFormatException();
        SetupExceptionContext(exception);

        _exceptionFilter.OnException(_exceptionContext);

        var response = _exceptionContext.Result as ObjectResult;
        Assert.IsNotNull(response);
        Assert.AreEqual(401, response.StatusCode);
        Assert.AreEqual("Formato de Token Inválido", GetTitle(response.Value!));
    }

    [TestMethod]
    public void ExceptionFilter_ShouldReturn401_WhenInvalidTokenExceptionOccurs()
    {
        var exception = new InvalidTokenException();
        SetupExceptionContext(exception);

        _exceptionFilter.OnException(_exceptionContext);

        var response = _exceptionContext.Result as ObjectResult;
        Assert.IsNotNull(response);
        Assert.AreEqual(401, response.StatusCode);
        Assert.AreEqual("Token Inválido", GetTitle(response.Value!));
    }

    [TestMethod]
    public void ExceptionFilter_ShouldReturn401_WhenInvalidCredentialsExceptionOccurs()
    {
        var exception = new InvalidCredentialsException();
        SetupExceptionContext(exception);

        _exceptionFilter.OnException(_exceptionContext);

        var response = _exceptionContext.Result as ObjectResult;
        Assert.IsNotNull(response);
        Assert.AreEqual(401, response.StatusCode);
        Assert.AreEqual("Credenciales Inválidas", GetTitle(response.Value!));
    }

    [TestMethod]
    public void ExceptionFilter_ShouldReturn401_WhenInvalidAuthenticationExceptionOccurs()
    {
        var exception = new InvalidAuthenticationException("Auth failed");
        SetupExceptionContext(exception);

        _exceptionFilter.OnException(_exceptionContext);

        var response = _exceptionContext.Result as ObjectResult;
        Assert.IsNotNull(response);
        Assert.AreEqual(401, response.StatusCode);
        Assert.AreEqual("Error de Autenticación", GetTitle(response.Value!));
    }

    [TestMethod]
    public void ExceptionFilter_ShouldReturn409_WhenVisitAlreadyActiveExceptionOccurs()
    {
        var exception = new VisitAlreadyActiveException("Visit already active");
        SetupExceptionContext(exception);

        _exceptionFilter.OnException(_exceptionContext);

        var response = _exceptionContext.Result as ObjectResult;
        Assert.IsNotNull(response);
        Assert.AreEqual(409, response.StatusCode);
        Assert.AreEqual("Visita Ya Activa", GetTitle(response.Value!));
    }

    [TestMethod]
    public void ExceptionFilter_ShouldReturn400_WhenInvalidTicketExceptionOccurs()
    {
        var exception = new InvalidTicketException("Ticket is invalid");
        SetupExceptionContext(exception);

        _exceptionFilter.OnException(_exceptionContext);

        var response = _exceptionContext.Result as ObjectResult;
        Assert.IsNotNull(response);
        Assert.AreEqual(400, response.StatusCode);
        Assert.AreEqual("Ticket Inválido", GetTitle(response.Value!));
    }

    [TestMethod]
    public void ExceptionFilter_ShouldReturn410_WhenExpiredTicketExceptionOccurs()
    {
        var ticketId = "TICKET123";
        var expirationDate = new DateTime(2025, 9, 30);
        var exception = new ExpiredTicketException(ticketId, expirationDate);
        SetupExceptionContext(exception);

        _exceptionFilter.OnException(_exceptionContext);

        var response = _exceptionContext.Result as ObjectResult;
        Assert.IsNotNull(response);
        Assert.AreEqual(410, response.StatusCode);
        Assert.AreEqual("Ticket Expirado", GetTitle(response.Value!));
    }

    [TestMethod]
    public void ExceptionFilter_ShouldReturn410_WhenTicketAlreadyUsedExceptionOccurs()
    {
        var ticketId = "TICKET123";
        var attractionName = "Montaña Rusa";
        var exception = new TicketAlreadyUsedException(ticketId, attractionName);
        SetupExceptionContext(exception);

        _exceptionFilter.OnException(_exceptionContext);

        var response = _exceptionContext.Result as ObjectResult;
        Assert.IsNotNull(response);
        Assert.AreEqual(410, response.StatusCode);
        Assert.AreEqual("Ticket Ya Utilizado", GetTitle(response.Value!));
    }

    [TestMethod]
    public void ExceptionFilter_ShouldReturn422_WhenWrongAttractionExceptionOccurs()
    {
        var exception = new WrongAttractionException("Wrong attraction");
        SetupExceptionContext(exception);

        _exceptionFilter.OnException(_exceptionContext);

        var response = _exceptionContext.Result as ObjectResult;
        Assert.IsNotNull(response);
        Assert.AreEqual(422, response.StatusCode);
        Assert.AreEqual("Ticket para Otra Atracción", GetTitle(response.Value!));
    }

    [TestMethod]
    public void ExceptionFilter_ShouldReturn422_WhenTicketNotValidForDateExceptionOccurs()
    {
        var ticketId = "TICKET123";
        var validDate = new DateTime(2025, 10, 1);
        var currentDate = new DateTime(2025, 10, 2);
        var exception = new TicketNotValidForDateException(ticketId, validDate, currentDate);
        SetupExceptionContext(exception);

        _exceptionFilter.OnException(_exceptionContext);

        var response = _exceptionContext.Result as ObjectResult;
        Assert.IsNotNull(response);
        Assert.AreEqual(422, response.StatusCode);
        Assert.AreEqual("Ticket No Válido para la Fecha", GetTitle(response.Value!));
    }

    [TestMethod]
    public void ExceptionFilter_ShouldReturn422_WhenAgeLimitExceptionOccurs()
    {
        var exception = new AgeLimitException("Montaña Rusa", 12, 10);
        SetupExceptionContext(exception);

        _exceptionFilter.OnException(_exceptionContext);

        var response = _exceptionContext.Result as ObjectResult;
        Assert.IsNotNull(response);
        Assert.AreEqual(422, response.StatusCode);
        Assert.AreEqual("Edad Mínima No Cumplida", GetTitle(response.Value!));
    }

    [TestMethod]
    public void ExceptionFilter_ShouldReturn422_WhenNoActiveVisitExceptionOccurs()
    {
        var attractionName = "Montaña Rusa";
        var userIdentification = "USER123";
        var exception = new NoActiveVisitException(attractionName, userIdentification);
        SetupExceptionContext(exception);

        _exceptionFilter.OnException(_exceptionContext);

        var response = _exceptionContext.Result as ObjectResult;
        Assert.IsNotNull(response);
        Assert.AreEqual(422, response.StatusCode);
        Assert.AreEqual("Sin Visita Activa", GetTitle(response.Value!));
    }

    [TestMethod]
    public void ExceptionFilter_ShouldReturn400_WhenInvalidUserDataExceptionOccurs()
    {
        var exception = new InvalidUserDataException("Invalid user data");
        SetupExceptionContext(exception);

        _exceptionFilter.OnException(_exceptionContext);

        var response = _exceptionContext.Result as ObjectResult;
        Assert.IsNotNull(response);
        Assert.AreEqual(400, response.StatusCode);
        Assert.AreEqual("Datos de Usuario Inválidos", GetTitle(response.Value!));
    }

    [TestMethod]
    public void ExceptionFilter_ShouldReturn400_WhenDateTimeBusinessLogicExceptionOccurs()
    {
        var exception = new DateTimeBusinessLogicException("DateTime validation error");
        SetupExceptionContext(exception);

        _exceptionFilter.OnException(_exceptionContext);

        var response = _exceptionContext.Result as ObjectResult;
        Assert.IsNotNull(response);
        Assert.AreEqual(400, response.StatusCode);
        Assert.AreEqual("Error de Validación de Fecha", GetTitle(response.Value!));
    }

    [TestMethod]
    public void ExceptionFilter_ShouldReturn400_WhenScoringStrategyExceptionOccurs()
    {
        var exception = new ScoringStrategyException("Scoring error");
        SetupExceptionContext(exception);

        _exceptionFilter.OnException(_exceptionContext);

        var response = _exceptionContext.Result as ObjectResult;
        Assert.IsNotNull(response);
        Assert.AreEqual(400, response.StatusCode);
        Assert.AreEqual("Error de Estrategia de Puntuación", GetTitle(response.Value!));
    }

    [TestMethod]
    public void ExceptionFilter_ShouldReturn400_WhenJsonExceptionOccurs()
    {
        var exception = new System.Text.Json.JsonException("Invalid JSON");
        SetupExceptionContext(exception);

        _exceptionFilter.OnException(_exceptionContext);

        var response = _exceptionContext.Result as ObjectResult;
        Assert.IsNotNull(response);
        Assert.AreEqual(400, response.StatusCode);
        Assert.AreEqual("Error de Formato JSON", GetTitle(response.Value!));
        Assert.AreEqual("El request contiene campos no reconocidos o tiene formato inválido", GetMessage(response.Value!));
    }

    [TestMethod]
    public void ExceptionFilter_ShouldReturn404_WhenScoringStrategyNotFoundExceptionOccurs()
    {
        var exception = new ScoringStrategyNotFoundException("Estrategia 1");
        SetupExceptionContext(exception);

        _exceptionFilter.OnException(_exceptionContext);

        var response = _exceptionContext.Result as ObjectResult;
        Assert.IsNotNull(response);
        Assert.AreEqual(404, response.StatusCode);
        Assert.AreEqual("Estrategia de Puntuación No Encontrada", GetTitle(response.Value!));
    }

    [TestMethod]
    public void ExceptionFilter_ShouldReturn409_WhenUserAlreadyInAttractionExceptionOccurs()
    {
        var exception = new UserAlreadyInAttractionException("Montaña Rusa");
        SetupExceptionContext(exception);

        _exceptionFilter.OnException(_exceptionContext);

        var response = _exceptionContext.Result as ObjectResult;
        Assert.IsNotNull(response);
        Assert.AreEqual(409, response.StatusCode);
        Assert.AreEqual("Usuario en Otra Atracción", GetTitle(response.Value!));
    }

    [TestMethod]
    public void ExceptionFilter_ShouldReturn409_WhenEventCapacityExceededExceptionOccurs()
    {
        var exception = new EventCapacityExceededException("Evento Especial", 100);
        SetupExceptionContext(exception);

        _exceptionFilter.OnException(_exceptionContext);

        var response = _exceptionContext.Result as ObjectResult;
        Assert.IsNotNull(response);
        Assert.AreEqual(409, response.StatusCode);
        Assert.AreEqual("Aforo Completo", GetTitle(response.Value!));
    }

    [TestMethod]
    public void ExceptionFilter_ShouldReturn409_WhenCannotDeleteAttractionExceptionOccurs()
    {
        var exception = new CannotDeleteAttractionException("Montaña Rusa");
        SetupExceptionContext(exception);

        _exceptionFilter.OnException(_exceptionContext);

        var response = _exceptionContext.Result as ObjectResult;
        Assert.IsNotNull(response);
        Assert.AreEqual(409, response.StatusCode);
        Assert.AreEqual("Atracción con Registros Asociados", GetTitle(response.Value!));
    }

    [TestMethod]
    public void ExceptionFilter_ShouldReturn409_WhenCannotDeleteAttractionWithActiveIncidentsExceptionOccurs()
    {
        var exception = new CannotDeleteAttractionWithActiveIncidentsException("Montaña Rusa");
        SetupExceptionContext(exception);

        _exceptionFilter.OnException(_exceptionContext);

        var response = _exceptionContext.Result as ObjectResult;
        Assert.IsNotNull(response);
        Assert.AreEqual(409, response.StatusCode);
        Assert.AreEqual("Atracción con Incidencias Activas", GetTitle(response.Value!));
    }

    [TestMethod]
    public void ExceptionFilter_ShouldReturn409_WhenCannotDeleteAttractionWithScheduledMaintenancesExceptionOccurs()
    {
        var exception = new CannotDeleteAttractionWithScheduledMaintenancesException("Montaña Rusa");
        SetupExceptionContext(exception);

        _exceptionFilter.OnException(_exceptionContext);

        var response = _exceptionContext.Result as ObjectResult;
        Assert.IsNotNull(response);
        Assert.AreEqual(409, response.StatusCode);
        Assert.AreEqual("Atracción con Mantenimientos Programados", GetTitle(response.Value!));
    }

    [TestMethod]
    public void ExceptionFilter_ShouldReturn400_WhenInvalidMaintenanceDataExceptionOccurs()
    {
        var exception = new InvalidMaintenanceDataException("Campo inválido");
        SetupExceptionContext(exception);

        _exceptionFilter.OnException(_exceptionContext);

        var response = _exceptionContext.Result as ObjectResult;
        Assert.IsNotNull(response);
        Assert.AreEqual(400, response.StatusCode);
        Assert.AreEqual("Datos de Mantenimiento Inválidos", GetTitle(response.Value!));
    }

    [TestMethod]
    public void ExceptionFilter_ShouldReturn400_WhenInvalidRequestDataExceptionOccurs()
    {
        var exception = new InvalidRequestDataException("Request inválido");
        SetupExceptionContext(exception);

        _exceptionFilter.OnException(_exceptionContext);

        var response = _exceptionContext.Result as ObjectResult;
        Assert.IsNotNull(response);
        Assert.AreEqual(400, response.StatusCode);
        Assert.AreEqual("Error de Validación", GetTitle(response.Value!));
    }

    [TestMethod]
    public void ExceptionFilter_ShouldReturn422_WhenEventTicketNotValidForTimeExceptionOccurs()
    {
        var eventName = "Evento Nocturno";
        var eventStart = DateTime.Now.Date.AddHours(20);
        var eventEnd = eventStart.AddHours(4);
        var currentTime = DateTime.Now.Date.AddHours(19);
        var exception = new EventTicketNotValidForTimeException(eventName, eventStart, eventEnd, currentTime);
        SetupExceptionContext(exception);

        _exceptionFilter.OnException(_exceptionContext);

        var response = _exceptionContext.Result as ObjectResult;
        Assert.IsNotNull(response);
        Assert.AreEqual(422, response.StatusCode);
        Assert.AreEqual("Ticket de Evento Fuera de Horario", GetTitle(response.Value!));
    }
}
