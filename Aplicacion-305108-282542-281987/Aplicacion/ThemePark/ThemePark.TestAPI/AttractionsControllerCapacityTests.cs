using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Moq;
using ThemePark.Entities;
using ThemePark.Enums;
using ThemePark.IBusinessLogic;
using ThemePark.Models.Attractions;
using ThemeParkApi.Controllers;
using ThemeParkApi.Filters;

namespace ThemePark.TestAPI;

[TestClass]
public class AttractionsControllerCapacityTests
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
    public void GetCapacity_ShouldReturnOkWithCapacityData_WhenValidNameProvided()
    {
        var nombre = "Montaña Rusa T-Rex";

        var domainAttraction = new Attraction("Montaña Rusa T-Rex", TipoAtraccion.MontañaRusa, 12, 50,
            "Descripción", new DateTime(2025, 9, 18, 14, 30, 0));
        domainAttraction.IncrementarAforo();
        domainAttraction.IncrementarAforo();
        domainAttraction.IncrementarAforo();

        _mockAttractionsBusinessLogic.Setup(x => x.GetCapacity(nombre))
            .Returns(domainAttraction);

        var result = _controller.GetCapacity(nombre);

        Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        var okResult = result as OkObjectResult;
        var response = okResult!.Value as CapacityInfoModel;
        Assert.AreEqual(50, response!.CapacidadMaxima);
        Assert.AreEqual(3, response.AforoActual);
    }

    [TestMethod]
    public void GetCapacity_ShouldReturnCorrectCalculations_WhenValidNameProvided()
    {
        var nombre = "Montaña Rusa T-Rex";

        var domainAttraction = new Attraction("Montaña Rusa T-Rex", TipoAtraccion.MontañaRusa, 12, 50,
            "Descripción", new DateTime(2025, 9, 18, 14, 30, 0));
        domainAttraction.IncrementarAforo();
        domainAttraction.IncrementarAforo();
        domainAttraction.IncrementarAforo();

        _mockAttractionsBusinessLogic.Setup(x => x.GetCapacity(nombre))
            .Returns(domainAttraction);

        var result = _controller.GetCapacity(nombre);

        var okResult = result as OkObjectResult;
        var response = okResult!.Value as CapacityInfoModel;
        Assert.AreEqual(47, response!.EspaciosDisponibles);
        Assert.AreEqual(6.0, response.PorcentajeOcupacion);
    }

    [TestMethod]
    public void GetUsageReport_ShouldReturnOkResponse_WhenValidDatesProvided()
    {
        var fechaInicio = new DateTime(2025, 9, 1);
        var fechaFin = new DateTime(2025, 9, 30);

        var usageData = new Dictionary<string, int>
        {
            { "Montaña Rusa", 50 },
            { "Torre de Caída", 30 },
            { "Carrusel", 20 }
        };

        _mockAttractionsBusinessLogic.Setup(x => x.GetUsageReport(fechaInicio, fechaFin))
            .Returns(usageData);

        var result = _controller.GetUsageReport("2025-09-01", "2025-09-30");

        Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        var okResult = result as OkObjectResult;
        Assert.AreEqual(200, okResult!.StatusCode);
        var response = okResult.Value as List<UsageReportModel>;
        Assert.IsNotNull(response);
    }

    [TestMethod]
    public void GetUsageReport_ShouldReturnCorrectCountAndFirstAttraction_WhenValidDatesProvided()
    {
        var fechaInicio = new DateTime(2025, 9, 1);
        var fechaFin = new DateTime(2025, 9, 30);

        var usageData = new Dictionary<string, int>
        {
            { "Montaña Rusa", 50 },
            { "Torre de Caída", 30 },
            { "Carrusel", 20 }
        };

        _mockAttractionsBusinessLogic.Setup(x => x.GetUsageReport(fechaInicio, fechaFin))
            .Returns(usageData);

        var result = _controller.GetUsageReport("2025-09-01", "2025-09-30");

        var okResult = result as OkObjectResult;
        var response = okResult!.Value as List<UsageReportModel>;
        Assert.AreEqual(3, response!.Count);
        Assert.IsTrue(response.Any(r => r.AtraccionNombre == "Montaña Rusa" && r.CantidadVisitas == 50));
    }

    [TestMethod]
    public void GetUsageReport_ShouldReturnAllAttractions_WhenValidDatesProvided()
    {
        var fechaInicio = new DateTime(2025, 9, 1);
        var fechaFin = new DateTime(2025, 9, 30);

        var usageData = new Dictionary<string, int>
        {
            { "Montaña Rusa", 50 },
            { "Torre de Caída", 30 },
            { "Carrusel", 20 }
        };

        _mockAttractionsBusinessLogic.Setup(x => x.GetUsageReport(fechaInicio, fechaFin))
            .Returns(usageData);

        var result = _controller.GetUsageReport("2025-09-01", "2025-09-30");

        var okResult = result as OkObjectResult;
        var response = okResult!.Value as List<UsageReportModel>;
        Assert.IsTrue(response!.Any(r => r.AtraccionNombre == "Torre de Caída" && r.CantidadVisitas == 30));
        Assert.IsTrue(response!.Any(r => r.AtraccionNombre == "Carrusel" && r.CantidadVisitas == 20));
    }
}
