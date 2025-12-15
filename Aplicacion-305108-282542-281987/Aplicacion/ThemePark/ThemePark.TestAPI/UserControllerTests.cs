using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Moq;
using ThemePark.Entities;
using ThemePark.Entities.Roles;
using ThemePark.Enums;
using ThemePark.Exceptions;
using ThemePark.IBusinessLogic;
using ThemePark.IBusinessLogic.Exceptions;
using ThemePark.IDataAccess.Exceptions;
using ThemePark.Models.Users;
using ThemeParkApi.Controllers;
using ThemeParkApi.Filters;

namespace ThemePark.TestAPI;

[TestClass]
public class UserControllerTests
{
    private UserController _controller = null!;
    private Mock<IUserService> _mockUserService = null!;
    private UserRegisterRequest _validRequest = null!;
    private UserWithAllResponse _expectedResponse = null!;
    private UserProfileUpdateRequest _profileUpdateRequest = null!;
    private UserProfileUpdateResponse _expectedProfileResponse = null!;
    private List<UserWithAllResponse> _expectedUsersList = null!;
    private UserWithAllResponse _expectedUserById = null!;
    private readonly Guid _testUserId = Guid.NewGuid();
    private readonly string _testCodigoIdentificacion = "ABC12345";
    private User _mockUser = null!;
    private User _mockRegisterUser = null!;
    private User _mockUpdateUser = null!;
    private ExceptionFilter _exceptionFilter = null!;
    private ExceptionContext _exceptionContext = null!;

    [TestInitialize]
    public void TestInitialize()
    {
        _mockUserService = new Mock<IUserService>(MockBehavior.Strict);
        _controller = new UserController(_mockUserService.Object);
        _exceptionFilter = new ExceptionFilter();

        _validRequest = new UserRegisterRequest
        {
            Nombre = "Juan",
            Apellido = "Pérez",
            Email = "juan@email.com",
            Contraseña = "password123",
            FechaNacimiento = "15/1/1990"
        };

        _expectedResponse = new UserWithAllResponse
        {
            Id = Guid.NewGuid(),
            Nombre = "Juan",
            Apellido = "Pérez",
            Email = "juan@email.com",
            FechaNacimiento = "1990-01-15",
            NivelMembresia = "Estándar",
            Roles = ["Visitante"],
            FechaRegistro = "2025-09-19T10:30:00"
        };

        _profileUpdateRequest = new UserProfileUpdateRequest
        {
            Nombre = "Carlos",
            Apellido = "González",
            Email = "carlos@email.com",
            Contraseña = "newpassword123",
            FechaNacimiento = "1985-05-20"
        };

        _expectedProfileResponse = new UserProfileUpdateResponse
        {
            Id = Guid.NewGuid(),
            Nombre = "Carlos",
            Apellido = "González",
            Email = "carlos@email.com",
            FechaNacimiento = "1985-05-20",
            NivelMembresia = "Estándar",
            FechaRegistro = "2025-09-19T10:30:00"
        };

        _expectedUsersList =
        [
            new UserWithAllResponse
            {
                Id = Guid.NewGuid(),
                Nombre = "Juan",
                Apellido = "Pérez",
                Email = "juan@email.com",
                FechaNacimiento = "1990-01-15",
                NivelMembresia = "Estándar",
                Roles = ["Visitante"]
            },
            new UserWithAllResponse
            {
                Id = Guid.NewGuid(),
                Nombre = "María",
                Apellido = "García",
                Email = "maria@email.com",
                FechaNacimiento = "1985-03-20",
                NivelMembresia = "Premium",
                Roles = ["AdministradorParque"]
            }

        ];

        _expectedUserById = new UserWithAllResponse
        {
            Id = _testUserId,
            Nombre = "Pedro",
            Apellido = "Martínez",
            Email = "pedro@email.com",
            FechaNacimiento = "1988-07-10",
            NivelMembresia = "Premium",
            Roles = ["Visitante"],
            FechaRegistro = "2025-09-18T15:20:00"
        };

        _mockUser = new User
        {
            Id = _testUserId,
            Nombre = "Pedro",
            Apellido = "Martínez",
            Email = "pedro@email.com",
            FechaNacimiento = DateTime.Parse("1988-07-10"),
            Roles = [new RolVisitante { NivelMembresia = NivelMembresia.Premium }],
            FechaRegistro = DateTime.Parse("2025-09-18T15:20:00"),
            CodigoIdentificacion = _testCodigoIdentificacion
        };

        _mockRegisterUser = new User
        {
            Id = Guid.NewGuid(),
            Nombre = "Juan",
            Apellido = "Pérez",
            Email = "juan@email.com",
            Contraseña = "password123",
            FechaNacimiento = DateTime.Parse("1990-01-15"),
            Roles = [new RolVisitante { NivelMembresia = NivelMembresia.Estándar }],
            FechaRegistro = DateTime.Parse("2025-09-19T10:30:00")
        };

        _mockUpdateUser = new User
        {
            Id = _testUserId,
            Nombre = "Carlos",
            Apellido = "González",
            Email = "juan@email.com",
            FechaNacimiento = DateParser.ParseDate("02/05/2021"),
            Roles = [new RolVisitante { NivelMembresia = NivelMembresia.Estándar }],
            FechaRegistro = DateTime.Parse("2025-09-19T10:30:00"),
            CodigoIdentificacion = _testCodigoIdentificacion
        };
    }

    [TestCleanup]
    public void TestCleanup()
    {
        _controller = null!;
        _mockUserService = null!;
        _validRequest = null!;
        _expectedResponse = null!;
        _mockUser = null!;
        _mockRegisterUser = null!;
        _mockUpdateUser = null!;
        _profileUpdateRequest = null!;
        _expectedProfileResponse = null!;
        _expectedUsersList = null!;
        _expectedUserById = null!;
    }

    [TestMethod]
    public void Register_ShouldReturnCreatedWithCorrectStatusCode_WhenValidRequest()
    {
        _mockUserService.Setup(x => x.Register(It.IsAny<User>()))
                       .Returns(_mockRegisterUser);

        var result = _controller.Register(_validRequest);

        Assert.IsInstanceOfType(result, typeof(CreatedResult));
        var createdResult = (CreatedResult)result;
        Assert.AreEqual(201, createdResult.StatusCode);
    }

    [TestMethod]
    public void Register_ShouldReturnResponseWithCorrectProperties_WhenValidRequest()
    {
        _mockUserService.Setup(x => x.Register(It.IsAny<User>()))
                       .Returns(_mockRegisterUser);

        var result = _controller.Register(_validRequest);
        var createdResult = (CreatedResult)result;
        var response = (UserWithAllResponse)createdResult.Value!;

        Assert.AreEqual(_expectedResponse.Nombre, response.Nombre);
        Assert.AreEqual("Estándar", response.NivelMembresia);
        Assert.AreEqual("Visitante", response.Roles[0]);
    }

    [TestMethod]
    public void Register_ShouldCallUserService_WhenValidRequest()
    {
        _mockUserService.Setup(x => x.Register(It.IsAny<User>()))
                       .Returns(_mockRegisterUser);

        var result = _controller.Register(_validRequest);

        _mockUserService.Verify(x => x.Register(It.IsAny<User>()), Times.Once);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidUserDataException))]
    public void Register_ShouldThrowInvalidUserDataException_WhenEmailIsInvalid()
    {
        _mockUserService.Setup(x => x.Register(It.IsAny<User>()))
                       .Throws(new InvalidUserDataException("Email", "formato inválido"));

        _controller.Register(_validRequest);
    }

    [TestMethod]
    [ExpectedException(typeof(UserRegistrationException))]
    public void Register_ShouldThrowUserRegistrationException_WhenEmailAlreadyExists()
    {
        _mockUserService.Setup(x => x.Register(It.IsAny<User>()))
                       .Throws(new UserRegistrationException("El email ya está registrado"));

        _controller.Register(_validRequest);
    }

    [TestMethod]
    public void UpdateProfile_ShouldReturnOkWithCorrectStatusCode_WhenValidRequest()
    {
        _mockUserService.Setup(x => x.GetUserByCodigoIdentificacion(_testCodigoIdentificacion))
                       .Returns(_mockUser);
        _mockUserService.Setup(x => x.UpdateProfile(It.IsAny<User>()))
                       .Returns(_mockUpdateUser);

        var result = _controller.UpdateProfile(_testCodigoIdentificacion, _profileUpdateRequest);

        Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        var okResult = (OkObjectResult)result;
        Assert.AreEqual(200, okResult.StatusCode);
    }

    [TestMethod]
    public void UpdateProfile_ShouldReturnResponseWithCorrectProperties_WhenValidRequest()
    {
        _mockUserService.Setup(x => x.GetUserByCodigoIdentificacion(_testCodigoIdentificacion))
                       .Returns(_mockUser);
        _mockUserService.Setup(x => x.UpdateProfile(It.IsAny<User>()))
                       .Returns(_mockUpdateUser);

        var result = _controller.UpdateProfile(_testCodigoIdentificacion, _profileUpdateRequest);
        var okResult = (OkObjectResult)result;
        var response = (UserProfileUpdateResponse)okResult.Value!;

        Assert.AreEqual(_expectedProfileResponse.Nombre, response.Nombre);
        Assert.AreEqual(_expectedProfileResponse.Apellido, response.Apellido);
    }

    [TestMethod]
    public void UpdateProfile_ShouldCallUserServiceMethods_WhenValidRequest()
    {
        _mockUserService.Setup(x => x.GetUserByCodigoIdentificacion(_testCodigoIdentificacion))
                       .Returns(_mockUser);
        _mockUserService.Setup(x => x.UpdateProfile(It.IsAny<User>()))
                       .Returns(_mockUpdateUser);

        var result = _controller.UpdateProfile(_testCodigoIdentificacion, _profileUpdateRequest);

        _mockUserService.Verify(x => x.GetUserByCodigoIdentificacion(_testCodigoIdentificacion), Times.Once);
        _mockUserService.Verify(x => x.UpdateProfile(It.IsAny<User>()), Times.Once);
    }

    [TestMethod]
    public void GetAllUsers_ShouldReturnOk_WhenRequestedByAdmin()
    {
        var mockUser1 = new User
        {
            Id = Guid.NewGuid(),
            Nombre = "Juan",
            Apellido = "Pérez",
            Email = "juan@email.com",
            FechaNacimiento = DateTime.Parse("1990-01-15"),
            Roles = [new RolVisitante { NivelMembresia = NivelMembresia.Estándar }],
            FechaRegistro = DateTime.Now
        };

        var mockUser2 = new User
        {
            Id = Guid.NewGuid(),
            Nombre = "María",
            Apellido = "García",
            Email = "maria@email.com",
            FechaNacimiento = DateTime.Parse("1985-03-20"),
            Roles = [new RolAdministradorParque(), new RolVisitante { NivelMembresia = NivelMembresia.Premium }],
            FechaRegistro = DateTime.Now
        };

        _mockUserService.Setup(x => x.GetAllUsers())
                       .Returns([mockUser1, mockUser2]);

        var result = _controller.GetAllUsers();

        Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        var okResult = (OkObjectResult)result;
        Assert.AreEqual(200, okResult.StatusCode);
    }

    [TestMethod]
    public void GetAllUsers_ShouldReturnListWithCorrectCount_WhenRequestedByAdmin()
    {
        var mockUser1 = new User
        {
            Id = Guid.NewGuid(),
            Nombre = "Juan",
            Apellido = "Pérez",
            Email = "juan@email.com",
            FechaNacimiento = DateTime.Parse("1990-01-15"),
            Roles = [new RolVisitante { NivelMembresia = NivelMembresia.Estándar }],
            FechaRegistro = DateTime.Now
        };

        var mockUser2 = new User
        {
            Id = Guid.NewGuid(),
            Nombre = "María",
            Apellido = "García",
            Email = "maria@email.com",
            FechaNacimiento = DateTime.Parse("1985-03-20"),
            Roles = [new RolAdministradorParque(), new RolVisitante { NivelMembresia = NivelMembresia.Premium }],
            FechaRegistro = DateTime.Now
        };

        _mockUserService.Setup(x => x.GetAllUsers())
                       .Returns([mockUser1, mockUser2]);

        var result = _controller.GetAllUsers();
        var okResult = (OkObjectResult)result;
        var response = (List<UserWithAllResponse>)okResult.Value!;

        Assert.AreEqual(2, response.Count);
        Assert.AreEqual(_expectedUsersList[0].Nombre, response[0].Nombre);
        Assert.AreEqual(_expectedUsersList[1].Roles[0], response[1].Roles[0]);
    }

    [TestMethod]
    public void GetAllUsers_ShouldCallUserService_WhenRequestedByAdmin()
    {
        var mockUser1 = new User
        {
            Id = Guid.NewGuid(),
            Nombre = "Juan",
            Apellido = "Pérez",
            Email = "juan@email.com",
            FechaNacimiento = DateTime.Parse("1990-01-15"),
            Roles = [new RolVisitante { NivelMembresia = NivelMembresia.Estándar }],
            FechaRegistro = DateTime.Now
        };

        var mockUser2 = new User
        {
            Id = Guid.NewGuid(),
            Nombre = "María",
            Apellido = "García",
            Email = "maria@email.com",
            FechaNacimiento = DateTime.Parse("1985-03-20"),
            Roles = [new RolAdministradorParque(), new RolVisitante { NivelMembresia = NivelMembresia.Premium }],
            FechaRegistro = DateTime.Now
        };

        _mockUserService.Setup(x => x.GetAllUsers())
                       .Returns([mockUser1, mockUser2]);

        var result = _controller.GetAllUsers();

        _mockUserService.Verify(x => x.GetAllUsers(), Times.Once);
    }

    [TestMethod]
    public void GetUserById_ShouldReturnOkWithCorrectStatusCode_WhenRequestedByAdmin()
    {
        _mockUserService.Setup(x => x.GetUserById(It.IsAny<Guid>()))
                       .Returns(_mockUser);

        var result = _controller.GetUserById(_testUserId);

        Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        var okResult = (OkObjectResult)result;
        Assert.AreEqual(200, okResult.StatusCode);
    }

    [TestMethod]
    public void GetUserById_ShouldReturnResponseWithCorrectProperties_WhenRequestedByAdmin()
    {
        _mockUserService.Setup(x => x.GetUserById(It.IsAny<Guid>()))
                       .Returns(_mockUser);

        var result = _controller.GetUserById(_testUserId);
        var okResult = (OkObjectResult)result;
        var response = (UserWithAllResponse)okResult.Value!;

        Assert.AreEqual(_expectedUserById.Id, response.Id);
        Assert.AreEqual(_expectedUserById.Nombre, response.Nombre);
        Assert.AreEqual(_expectedUserById.Email, response.Email);
    }

    [TestMethod]
    public void GetUserById_ShouldCallUserService_WhenRequestedByAdmin()
    {
        _mockUserService.Setup(x => x.GetUserById(It.IsAny<Guid>()))
                       .Returns(_mockUser);

        var result = _controller.GetUserById(_testUserId);

        _mockUserService.Verify(x => x.GetUserById(_testUserId), Times.Once);
    }

    [TestMethod]
    [ExpectedException(typeof(UserNotFoundException))]
    public void GetUserById_ShouldThrowUserNotFoundException_WhenUserDoesNotExist()
    {
        _mockUserService.Setup(x => x.GetUserById(It.IsAny<Guid>()))
                       .Throws(new UserNotFoundException(_testUserId));

        _controller.GetUserById(_testUserId);
    }

    [TestMethod]
    public void UpdateUserAdmin_ShouldReturnOk_WhenValidRequest()
    {
        var adminRequest = new UserAdminUpdateRequest
        {
            Roles = [Models.Enums.RolModel.Visitante, Models.Enums.RolModel.AdministradorParque],
            NivelMembresia = Models.Enums.NivelMembresiaModel.Premium
        };

        _mockUserService.Setup(x => x.UpdateUserPrivileges(It.IsAny<User>()))
                       .Returns(_mockUser);

        var result = _controller.UpdateUserAdmin(_testUserId, adminRequest);

        Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        var okResult = (OkObjectResult)result;
        Assert.AreEqual(200, okResult.StatusCode);

        _mockUserService.Verify(x => x.UpdateUserPrivileges(It.IsAny<User>()), Times.Once);
    }

    [TestMethod]
    public void AuthorizationFilter_ShouldReturn401_WhenUserNotAuthenticated()
    {
        var filter = new AuthorizationFilterAttribute();
        var context = CreateAuthorizationFilterContext();
        context.HttpContext.Items["Token"] = null;

        filter.OnAuthorization(context);

        var response = context.Result as ObjectResult;
        Assert.IsNotNull(response);
        Assert.AreEqual(401, response.StatusCode);
    }

    [TestMethod]
    public void AuthorizationFilter_ShouldAllowAccess_WhenUserHasOneOfMultipleRequiredRoles()
    {
        var filter = new AuthorizationFilterAttribute("AdministradorParque", "OperadorAtraccion");
        var context = CreateAuthorizationFilterContext();

        var testToken = "valid_token_123";
        var mockUser = new User
        {
            Id = Guid.NewGuid(),
            Nombre = "Test",
            Apellido = "User",
            Email = "test@test.com",
            FechaNacimiento = DateTime.Now,
            Roles = [new RolOperadorAtraccion(), new RolVisitante()],
            FechaRegistro = DateTime.Now
        };

        var mockAuthService = new Mock<IAuthService>();
        mockAuthService.Setup(auth => auth.GetUserByToken(testToken))
                      .Returns(mockUser);

        var mockServiceProvider = new Mock<IServiceProvider>();
        mockServiceProvider.Setup(sp => sp.GetService(typeof(IAuthService)))
                          .Returns(mockAuthService.Object);

        context.HttpContext.Items["Token"] = testToken;
        context.HttpContext.RequestServices = mockServiceProvider.Object;

        filter.OnAuthorization(context);

        Assert.IsNull(context.Result);
    }

    [TestMethod]
    public void AuthorizationFilter_ShouldNotExecute_WhenResultIsAlreadySet()
    {
        var filter = new AuthorizationFilterAttribute("AdministradorParque");
        var context = CreateAuthorizationFilterContext();
        var existingResult = new OkResult();
        context.Result = existingResult;

        filter.OnAuthorization(context);

        Assert.AreSame(existingResult, context.Result);
    }

    [TestMethod]
    public void AuthorizationFilter_ShouldReturn403_WhenUserDoesNotHaveRequiredRole()
    {
        var filter = new AuthorizationFilterAttribute("AdministradorParque");
        var context = CreateAuthorizationFilterContext();

        var testToken = "valid_token_456";
        var mockUser = new User
        {
            Id = Guid.NewGuid(),
            Nombre = "Test",
            Apellido = "User",
            Email = "test@test.com",
            FechaNacimiento = DateTime.Now,
            Roles = [new RolVisitante()],
            FechaRegistro = DateTime.Now
        };

        var mockAuthService = new Mock<IAuthService>();
        mockAuthService.Setup(auth => auth.GetUserByToken(testToken))
                      .Returns(mockUser);

        var mockServiceProvider = new Mock<IServiceProvider>();
        mockServiceProvider.Setup(sp => sp.GetService(typeof(IAuthService)))
                          .Returns(mockAuthService.Object);

        context.HttpContext.Items["Token"] = testToken;
        context.HttpContext.RequestServices = mockServiceProvider.Object;

        filter.OnAuthorization(context);

        var response = context.Result as ObjectResult;
        Assert.IsNotNull(response);
        Assert.AreEqual(403, response.StatusCode);
    }

    [TestMethod]
    public void DeleteUser_ShouldReturnNoContent_WhenValidIdProvided()
    {
        var userId = Guid.NewGuid();

        _mockUserService.Setup(x => x.Delete(userId));

        var result = _controller.DeleteUser(userId);

        Assert.IsInstanceOfType(result, typeof(NoContentResult));
        var noContentResult = (NoContentResult)result;
        Assert.AreEqual(204, noContentResult.StatusCode);

        _mockUserService.Verify(x => x.Delete(userId), Times.Once);
    }

    [TestMethod]
    public void ExceptionFilter_ShouldReturn409Conflict_WhenUserRegistrationExceptionOccurs()
    {
        var exception = new UserRegistrationException("test@test.com");
        SetupExceptionContext(exception);

        if(!_exceptionContext.ExceptionHandled)
        {
            _exceptionFilter.OnException(_exceptionContext);
        }

        var response = _exceptionContext.Result as ObjectResult;
        Assert.IsNotNull(response);
        Assert.AreEqual((int)HttpStatusCode.Conflict, response.StatusCode);
        Assert.AreEqual("Email Duplicado", GetTitle(response.Value!));
        Assert.AreEqual(exception.Message, GetMessage(response.Value!));
    }

    private AuthorizationFilterContext CreateAuthorizationFilterContext()
    {
        var httpContext = new DefaultHttpContext();
        var actionContext = new ActionContext(httpContext, new(), new());
        return new AuthorizationFilterContext(actionContext, []);
    }

    private void SetupExceptionContext(Exception exception)
    {
        _exceptionContext = new ExceptionContext(
            new ActionContext(
                new Mock<HttpContext>().Object,
                new Microsoft.AspNetCore.Routing.RouteData(),
                new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor()),
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
    public void GetUserPointHistory_ShouldReturnOk_WhenUserAuthenticated()
    {
        var mockHistoryList = new List<object>();

        _mockUserService.Setup(x => x.GetUserPointHistory())
                       .Returns(mockHistoryList);

        var result = _controller.GetUserPointHistory();

        Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        var okResult = (OkObjectResult)result;
        Assert.AreEqual(200, okResult.StatusCode);
    }

    [TestMethod]
    public void GetUserPointHistory_ShouldReturnCorrectProperties()
    {
        var userId = Guid.NewGuid();
        var mockHistoryList = new List<object>
        {
            new Visit(userId, "Atracción Test", DateTime.Parse("2025-01-15T10:30:00Z"), 50, "Por Tiempo")
        };

        _mockUserService.Setup(x => x.GetUserPointHistory())
                       .Returns(mockHistoryList);

        var result = _controller.GetUserPointHistory();

        var okResult = (OkObjectResult)result;
        var historyList = okResult.Value as List<PointHistoryResponseModel>;

        Assert.IsNotNull(historyList);
        Assert.AreEqual(1, historyList.Count);
        Assert.AreEqual("2025-01-15T10:30:00Z", historyList[0].Fecha);
        Assert.AreEqual("Atracción Test", historyList[0].Origen);
        Assert.AreEqual("Ganancia", historyList[0].Tipo);
        Assert.AreEqual(50, historyList[0].Puntos);
        Assert.AreEqual("Por Tiempo", historyList[0].EstrategiaPuntaje);
    }

    [TestMethod]
    public void GetUserPointHistory_ShouldCallUserService()
    {
        var mockHistoryList = new List<object>();

        _mockUserService.Setup(x => x.GetUserPointHistory())
                       .Returns(mockHistoryList);

        _controller.GetUserPointHistory();

        _mockUserService.Verify(x => x.GetUserPointHistory(), Times.Once);
    }

    [TestMethod]
    public void GetUserPointHistory_ShouldHaveAuthenticationFilter()
    {
        var method = typeof(UserController).GetMethod("GetUserPointHistory");
        var attributes = method?.GetCustomAttributes(typeof(AuthenticationFilter), false);

        Assert.IsNotNull(attributes);
        Assert.IsTrue(attributes.Length > 0);
    }

    [TestMethod]
    public void GetUserPoints_ShouldReturnOk_WhenUserAuthenticated()
    {
        var mockPoints = new { PuntosDisponibles = 100, PuntosAcumulados = 500, PuntosGastados = 400 };

        _mockUserService.Setup(x => x.GetUserPoints())
                       .Returns(mockPoints);

        var result = _controller.GetUserPoints();

        Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        var okResult = (OkObjectResult)result;
        Assert.AreEqual(200, okResult.StatusCode);
    }

    [TestMethod]
    public void GetUserPoints_ShouldReturnCorrectProperties()
    {
        var mockPoints = new { PuntosDisponibles = 100, PuntosAcumulados = 500, PuntosGastados = 400 };

        _mockUserService.Setup(x => x.GetUserPoints())
                       .Returns(mockPoints);

        var result = _controller.GetUserPoints();

        var okResult = (OkObjectResult)result;
        var points = okResult.Value as UserPointsResponseModel;

        Assert.IsNotNull(points);
        Assert.AreEqual(100, points.PuntosDisponibles);
        Assert.AreEqual(500, points.PuntosAcumulados);
        Assert.AreEqual(400, points.PuntosGastados);
    }

    [TestMethod]
    public void GetUserPoints_ShouldCallUserService()
    {
        var mockPoints = new { PuntosDisponibles = 0, PuntosAcumulados = 0, PuntosGastados = 0 };

        _mockUserService.Setup(x => x.GetUserPoints())
                       .Returns(mockPoints);

        _controller.GetUserPoints();

        _mockUserService.Verify(x => x.GetUserPoints(), Times.Once);
    }

    [TestMethod]
    public void GetUserPoints_ShouldHaveAuthenticationFilter()
    {
        var method = typeof(UserController).GetMethod("GetUserPoints");
        var attributes = method?.GetCustomAttributes(typeof(AuthenticationFilter), false);

        Assert.IsNotNull(attributes);
        Assert.IsTrue(attributes.Length > 0);
    }
}
