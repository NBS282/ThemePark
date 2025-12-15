using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;
using Moq;
using ThemePark.IBusinessLogic;
using ThemePark.Models.Users;
using ThemeParkApi.Controllers;
using ThemeParkApi.Filters;

namespace ThemePark.TestAPI;

[TestClass]
public class AuthControllerTests
{
    private Mock<IAuthService> _mockAuthService = null!;
    private AuthController _controller = null!;
    private AuthenticationFilter _authenticationFilter = null!;
    private Mock<HttpContext> _httpContextMock = null!;
    private AuthorizationFilterContext _authorizationContext = null!;

    [TestInitialize]
    public void TestInitialize()
    {
        _mockAuthService = new Mock<IAuthService>(MockBehavior.Strict);
        _controller = new AuthController(_mockAuthService.Object);
        _authenticationFilter = new AuthenticationFilter();
        _httpContextMock = new Mock<HttpContext>(MockBehavior.Strict);
        _authorizationContext = new AuthorizationFilterContext(
            new ActionContext(_httpContextMock.Object, new Microsoft.AspNetCore.Routing.RouteData(), new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor()),
            []);
    }

    [TestCleanup]
    public void TestCleanup()
    {
        _mockAuthService.VerifyAll();
    }

    [TestMethod]
    public void Login_ShouldCallAuthServiceWithCorrectParameters()
    {
        var request = new UserLoginRequest
        {
            Email = "test@example.com",
            Contraseña = "password123"
        };

        _mockAuthService.Setup(x => x.Login(request.Email, request.Contraseña))
            .Returns(default((ThemePark.Entities.Session, ThemePark.Entities.User)));

        Assert.ThrowsException<NullReferenceException>(() => _controller.Login(request));
    }

    [TestMethod]
    [ExpectedException(typeof(UnauthorizedAccessException))]
    public void Login_ShouldThrowUnauthorizedAccessException_WhenCredentialsAreInvalid()
    {
        var request = new UserLoginRequest
        {
            Email = "wrong@example.com",
            Contraseña = "wrongpassword"
        };

        _mockAuthService.Setup(x => x.Login(request.Email, request.Contraseña))
            .Throws(new UnauthorizedAccessException("Invalid credentials"));

        _controller.Login(request);
    }

    [TestMethod]
    public void AuthenticationFilter_ShouldReturn401Unauthenticated_WhenEmptyAuthorizationHeader()
    {
        _httpContextMock.Setup(h => h.Request.Headers).Returns(new HeaderDictionary(
            new Dictionary<string, StringValues> { { "Authorization", string.Empty } }));

        _authenticationFilter.OnAuthorization(_authorizationContext);

        var response = _authorizationContext.Result as ObjectResult;
        Assert.IsNotNull(response);
        Assert.AreEqual(401, response.StatusCode);
        Assert.AreEqual("Token Faltante", GetTitle(response.Value!));
    }

    [TestMethod]
    public void AuthenticationFilter_ShouldReturn401InvalidAuthorization_WhenInvalidTokenFormat()
    {
        _httpContextMock.Setup(h => h.Request.Headers).Returns(new HeaderDictionary(
            new Dictionary<string, StringValues> { { "Authorization", "InvalidToken" } }));

        _authenticationFilter.OnAuthorization(_authorizationContext);

        var response = _authorizationContext.Result as ObjectResult;
        Assert.IsNotNull(response);
        Assert.AreEqual(401, response.StatusCode);
        Assert.AreEqual("Formato de Token Inválido", GetTitle(response.Value!));
    }

    [TestMethod]
    public void AuthenticationFilter_ShouldSetUserInContext_WhenValidToken()
    {
        var mockItems = new Mock<IDictionary<object, object?>>();
        var mockAuthService = new Mock<IAuthService>();
        var mockServiceProvider = new Mock<IServiceProvider>();

        mockServiceProvider.Setup(sp => sp.GetService(typeof(IAuthService)))
            .Returns(mockAuthService.Object);
        mockAuthService.Setup(auth => auth.GetUserByToken("valid_token_123"))
            .Returns(new ThemePark.Entities.User { Id = Guid.NewGuid(), Nombre = "Test" });

        _httpContextMock.Setup(h => h.Request.Headers).Returns(new HeaderDictionary(
            new Dictionary<string, StringValues> { { "Authorization", "Bearer valid_token_123" } }));
        _httpContextMock.Setup(h => h.Items).Returns(mockItems.Object);
        _httpContextMock.Setup(h => h.RequestServices).Returns(mockServiceProvider.Object);

        _authenticationFilter.OnAuthorization(_authorizationContext);

        var response = _authorizationContext.Result;
        _httpContextMock.VerifyAll();
        response.Should().BeNull();
        mockItems.VerifySet(m => m["UserId"] = It.IsAny<object>(), Times.Once);
        mockItems.VerifySet(m => m["Token"] = It.IsAny<object>(), Times.Once);
    }

    [TestMethod]
    public void AuthenticationFilter_ShouldReturn401InvalidToken_WhenGetUserByTokenReturnsNull()
    {
        var mockAuthService = new Mock<IAuthService>();
        var mockServiceProvider = new Mock<IServiceProvider>();

        mockServiceProvider.Setup(sp => sp.GetService(typeof(IAuthService)))
            .Returns(mockAuthService.Object);
        mockAuthService.Setup(auth => auth.GetUserByToken("invalid_token_456"))
            .Returns((ThemePark.Entities.User?)null);

        _httpContextMock.Setup(h => h.Request.Headers).Returns(new HeaderDictionary(
            new Dictionary<string, StringValues> { { "Authorization", "Bearer invalid_token_456" } }));
        _httpContextMock.Setup(h => h.RequestServices).Returns(mockServiceProvider.Object);

        _authenticationFilter.OnAuthorization(_authorizationContext);

        var response = _authorizationContext.Result as ObjectResult;
        Assert.IsNotNull(response);
        Assert.AreEqual(401, response.StatusCode);
        Assert.AreEqual("Token Inválido", GetTitle(response.Value!));
    }

    private string GetTitle(object value)
    {
        return value.GetType().GetProperty("title")?.GetValue(value)?.ToString() ??
               value.GetType().GetProperty("Title")?.GetValue(value)?.ToString() ?? string.Empty;
    }
}
