using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Primitives;
using Moq;
using ThemePark.Exceptions;
using ThemePark.IBusinessLogic;
using ThemeParkApi.Controllers;
using ThemeParkApi.Filters;

namespace ThemePark.TestAPI;

[TestClass]
public class AttractionsControllerAuthenticationTests
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

    private string GetTitle(object value)
    {
        return value.GetType().GetProperty("title")?.GetValue(value)?.ToString() ??
               value.GetType().GetProperty("Title")?.GetValue(value)?.ToString() ?? string.Empty;
    }

    [TestMethod]
    public void AuthenticationFilter_ShouldReturn401Unauthenticated_WhenEmptyHeaders()
    {
        _httpContextMock.Setup(h => h.Request.Headers).Returns(new HeaderDictionary());

        _authenticationFilter.OnAuthorization(_authorizationContext);

        var response = _authorizationContext.Result;
        _httpContextMock.VerifyAll();
        var concreteResponse = response as ObjectResult;
        concreteResponse!.StatusCode.Should().Be((int)HttpStatusCode.Unauthorized);
        GetTitle(concreteResponse.Value!).Should().Be("Token Faltante");
    }

    [TestMethod]
    public void AuthenticationFilter_ShouldReturn401Unauthenticated_WhenEmptyAuthorizationHeader()
    {
        _httpContextMock.Setup(h => h.Request.Headers).Returns(new HeaderDictionary(
            new Dictionary<string, StringValues> { { "Authorization", string.Empty } }));

        _authenticationFilter.OnAuthorization(_authorizationContext);

        var response = _authorizationContext.Result;
        _httpContextMock.VerifyAll();
        var concreteResponse = response as ObjectResult;
        concreteResponse!.StatusCode.Should().Be((int)HttpStatusCode.Unauthorized);
        GetTitle(concreteResponse.Value!).Should().Be("Token Faltante");
    }

    [TestMethod]
    public void AuthenticationFilter_ShouldReturn401InvalidAuthorization_WhenInvalidTokenFormat()
    {
        _httpContextMock.Setup(h => h.Request.Headers).Returns(new HeaderDictionary(
            new Dictionary<string, StringValues> { { "Authorization", "InvalidToken" } }));

        _authenticationFilter.OnAuthorization(_authorizationContext);

        var response = _authorizationContext.Result;
        _httpContextMock.VerifyAll();
        var concreteResponse = response as ObjectResult;
        concreteResponse!.StatusCode.Should().Be((int)HttpStatusCode.Unauthorized);
        GetTitle(concreteResponse.Value!).Should().Be("Formato de Token Inválido");
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
    public void AuthenticationFilter_ShouldReturn401InvalidToken_WhenAuthServiceIsNull()
    {
        var mockServiceProvider = new Mock<IServiceProvider>();

        mockServiceProvider.Setup(sp => sp.GetService(typeof(IAuthService)))
                          .Returns((IAuthService?)null);

        _httpContextMock.Setup(h => h.Request.Headers).Returns(new HeaderDictionary(
            new Dictionary<string, StringValues> { { "Authorization", "Bearer valid_token_123" } }));
        _httpContextMock.Setup(h => h.RequestServices).Returns(mockServiceProvider.Object);

        _authenticationFilter.OnAuthorization(_authorizationContext);

        var response = _authorizationContext.Result;
        _httpContextMock.VerifyAll();
        var concreteResponse = response as ObjectResult;
        concreteResponse!.StatusCode.Should().Be((int)HttpStatusCode.Unauthorized);
        GetTitle(concreteResponse.Value!).Should().Be("Token Inválido");
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

        var response = _authorizationContext.Result;
        _httpContextMock.VerifyAll();
        var concreteResponse = response as ObjectResult;
        concreteResponse!.StatusCode.Should().Be((int)HttpStatusCode.Unauthorized);
        GetTitle(concreteResponse.Value!).Should().Be("Token Inválido");
    }

    [TestMethod]
    public void ExceptionFilter_ShouldReturn403_WhenMissingRoleExceptionOccurs()
    {
        var requiredRoles = new[] { "Admin", "Operator" };
        var currentRoles = new[] { "Visitor" };
        var exception = new MissingRoleException(requiredRoles, currentRoles);
        SetupExceptionContext(exception);

        _exceptionFilter.OnException(_exceptionContext);

        var response = _exceptionContext.Result as ObjectResult;
        Assert.IsNotNull(response);
        Assert.AreEqual(403, response.StatusCode);
        Assert.AreEqual("Rol Faltante", GetTitle(response.Value!));
    }
}
