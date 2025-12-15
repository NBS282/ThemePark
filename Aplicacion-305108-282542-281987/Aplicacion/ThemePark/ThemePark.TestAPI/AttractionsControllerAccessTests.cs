using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Moq;
using ThemePark.Entities;
using ThemePark.IBusinessLogic;
using ThemePark.Models.Attractions;
using ThemeParkApi.Controllers;
using ThemeParkApi.Filters;

namespace ThemePark.TestAPI;

[TestClass]
public class AttractionsControllerAccessTests
{
    private Mock<IAttractionsBusinessLogic> _mockAttractionsBusinessLogic = null!;
    private AttractionsController _controller = null!;
    private ExceptionFilter _exceptionFilter = null!;
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

    [TestMethod]
    public void RegisterVisit_ShouldReturnCreated_WhenValidNFCRequestProvided()
    {
        var nombre = "Montaña Rusa T-Rex";
        var request = new RegisterVisitByNFCRequest
        {
            CodigoIdentificacion = "123e4567-e89b-12d3-a456-426614174000"
        };

        var expectedVisit = new Visit(Guid.NewGuid(), nombre, DateTime.Now);

        _mockAttractionsBusinessLogic.Setup(x => x.ValidateTicketAndRegisterAccess(nombre, "NFC", request.CodigoIdentificacion))
            .Returns(expectedVisit);

        var result = _controller.ValidateTicketAndAccess(nombre, new AccessAttractionRequest { TipoEntrada = "NFC", Codigo = request.CodigoIdentificacion });

        Assert.IsInstanceOfType(result, typeof(CreatedResult));
        var createdResult = result as CreatedResult;
        Assert.AreEqual(201, createdResult!.StatusCode);
        _mockAttractionsBusinessLogic.Verify(x => x.ValidateTicketAndRegisterAccess(nombre, "NFC", request.CodigoIdentificacion), Times.Once);
    }

    [TestMethod]
    public void RegisterVisit_ShouldThrowArgumentException_WhenAttractionNotFound()
    {
        var nombre = "Atraccion Inexistente";
        var request = new RegisterVisitByNFCRequest
        {
            CodigoIdentificacion = "123e4567-e89b-12d3-a456-426614174000"
        };

        _mockAttractionsBusinessLogic.Setup(x => x.ValidateTicketAndRegisterAccess(nombre, "NFC", request.CodigoIdentificacion))
            .Throws(new ArgumentException("Attraction not found"));

        Assert.ThrowsException<ArgumentException>(() => _controller.ValidateTicketAndAccess(nombre, new AccessAttractionRequest { TipoEntrada = "NFC", Codigo = request.CodigoIdentificacion }));
        _mockAttractionsBusinessLogic.Verify(x => x.ValidateTicketAndRegisterAccess(nombre, "NFC", request.CodigoIdentificacion), Times.Once);
    }

    [TestMethod]
    public void ValidateTicketAndAccess_ShouldReturnCreated_WhenValidTicketProvided()
    {
        var nombre = "Montaña Rusa T-Rex";
        var request = new ValidateTicketAndRegisterAccessRequest
        {
            CodigoIdentificacion = "123e4567-e89b-12d3-a456-426614174000"
        };

        var expectedVisit = new Visit(Guid.NewGuid(), nombre, DateTime.Now);

        _mockAttractionsBusinessLogic.Setup(x => x.ValidateTicketAndRegisterAccess(nombre, "QR", request.CodigoIdentificacion))
            .Returns(expectedVisit);

        var result = _controller.ValidateTicketAndAccess(nombre, new AccessAttractionRequest { TipoEntrada = "QR", Codigo = request.CodigoIdentificacion });

        Assert.IsInstanceOfType(result, typeof(CreatedResult));
        var createdResult = result as CreatedResult;
        Assert.AreEqual(201, createdResult!.StatusCode);
        _mockAttractionsBusinessLogic.Verify(x => x.ValidateTicketAndRegisterAccess(nombre, "QR", request.CodigoIdentificacion), Times.Once);
    }

    [TestMethod]
    public void RegisterExit_ShouldReturnOk_WhenValidRequestProvided()
    {
        var nombre = "Montaña Rusa T-Rex";
        var request = new AccessAttractionRequest
        {
            TipoEntrada = "NFC",
            Codigo = "123e4567-e89b-12d3-a456-426614174000"
        };

        var expectedVisit = new Visit(Guid.NewGuid(), nombre, DateTime.Now.AddHours(-1));
        expectedVisit.MarkExit(DateTime.Now);

        _mockAttractionsBusinessLogic.Setup(x => x.RegisterExit(nombre, request.TipoEntrada, request.Codigo))
            .Returns(expectedVisit);

        var result = _controller.RegisterExit(nombre, request);

        Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult!.Value);
        _mockAttractionsBusinessLogic.Verify(x => x.RegisterExit(nombre, request.TipoEntrada, request.Codigo), Times.Once);
    }

    [TestMethod]
    public void CreateIncident_ShouldReturnCreatedWithId_WhenValidRequestProvided()
    {
        var nombre = "Montaña Rusa T-Rex";
        var request = new CreateIncidentRequest
        {
            Descripcion = "Fuera de servicio por mantenimiento",
            Prioridad = "mantenimiento"
        };

        var incidentId = "987fcdeb-51a2-43d7-8f9e-123456789abc";

        _mockAttractionsBusinessLogic.Setup(x => x.CreateIncident(nombre, request.Descripcion))
            .Returns(incidentId);

        var result = _controller.CreateIncident(nombre, request);

        Assert.IsInstanceOfType(result, typeof(CreatedResult));
        var createdResult = result as CreatedResult;
        Assert.AreEqual(201, createdResult!.StatusCode);
        var response = createdResult.Value as IncidentResponseModel;
        Assert.AreEqual(incidentId, response!.Id);
    }

    [TestMethod]
    public void CreateIncident_ShouldReturnIncidentData_WhenValidRequestProvided()
    {
        var nombre = "Montaña Rusa T-Rex";
        var request = new CreateIncidentRequest
        {
            Descripcion = "Fuera de servicio por mantenimiento",
            Prioridad = "mantenimiento"
        };

        var incidentId = "987fcdeb-51a2-43d7-8f9e-123456789abc";

        _mockAttractionsBusinessLogic.Setup(x => x.CreateIncident(nombre, request.Descripcion))
            .Returns(incidentId);

        var result = _controller.CreateIncident(nombre, request);

        var createdResult = result as CreatedResult;
        var response = createdResult!.Value as IncidentResponseModel;
        Assert.AreEqual(nombre, response!.Atraccion);
        Assert.AreEqual(request.Descripcion, response.Descripcion);
        Assert.AreEqual("Activa", response.Estado);
    }

    [TestMethod]
    public void ResolveIncident_ShouldReturnNoContent_WhenValidIdProvided()
    {
        var nombre = "Montaña Rusa T-Rex";
        var id = "987fcdeb-51a2-43d7-8f9e-123456789abc";

        _mockAttractionsBusinessLogic.Setup(x => x.ResolveIncident(nombre, id));

        var result = _controller.ResolveIncident(nombre, id);

        Assert.IsInstanceOfType(result, typeof(NoContentResult));
        _mockAttractionsBusinessLogic.Verify(x => x.ResolveIncident(nombre, id), Times.Once);
    }
}
