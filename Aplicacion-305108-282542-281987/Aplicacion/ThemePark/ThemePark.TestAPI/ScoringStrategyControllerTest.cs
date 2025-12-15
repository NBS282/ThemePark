using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Moq;
using ThemePark.Exceptions;
using ThemePark.IBusinessLogic;
using ThemePark.IBusinessLogic.Exceptions;
using ThemePark.Models.Enums;
using ThemePark.Models.ScoringStrategy;
using ThemeParkApi.Controllers;
using ThemeParkApi.Filters;

namespace ThemePark.TestAPI;

[TestClass]
public class ScoringStrategyControllerTest
{
    private Mock<IScoringStrategyService> _mockService = null!;
    private Mock<IPluginLoader> _mockPluginLoader = null!;
    private ScoringStrategyController _controller = null!;
    [TestInitialize]
    public void TestInitialize()
    {
        _mockService = new Mock<IScoringStrategyService>(MockBehavior.Strict);
        _mockPluginLoader = new Mock<IPluginLoader>(MockBehavior.Strict);
        _controller = new ScoringStrategyController(_mockService.Object, _mockPluginLoader.Object);
    }

    [TestCleanup]
    public void TestCleanup()
    {
        _mockService.VerifyAll();
        _mockPluginLoader.VerifyAll();
    }

    [TestMethod]
    public void GetStrategies_Should_Return_OkResult_With_All_Strategies()
    {
        var config1 = new ThemePark.Entities.ConfiguracionPorAtraccion([]);
        var config2 = new ThemePark.Entities.ConfiguracionPorCombo(10, 50, 2);
        var expectedEntities = new List<Entities.ScoringStrategy>
        {
            new Entities.ScoringStrategy("PuntuacionPorAtraccion", ThemePark.Enums.TipoEstrategia.PuntuacionPorAtraccion, "Otorga puntos fijos por tipo de atracción", config1) { Active = true },
            new Entities.ScoringStrategy("PuntuacionCombo", ThemePark.Enums.TipoEstrategia.PuntuacionCombo, "Bonifica cadenas de atracciones diferentes", config2) { Active = false }
        };
        _mockService.Setup(x => x.GetAllStrategies())
            .Returns(expectedEntities);
        var result = _controller.GetStrategies();
        Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
    }

    [TestMethod]
    public void GetStrategies_Should_Return_ListWithCorrectCount()
    {
        var config1 = new ThemePark.Entities.ConfiguracionPorAtraccion([]);
        var config2 = new ThemePark.Entities.ConfiguracionPorCombo(15, 50, 3);
        var strategies = new List<ThemePark.Entities.ScoringStrategy>
        {
            new ThemePark.Entities.ScoringStrategy("PuntuacionPorAtraccion", ThemePark.Enums.TipoEstrategia.PuntuacionPorAtraccion, "desc", config1),
            new ThemePark.Entities.ScoringStrategy("PuntuacionCombo", ThemePark.Enums.TipoEstrategia.PuntuacionCombo, "desc", config2)
        };
        _mockService.Setup(x => x.GetAllStrategies()).Returns(strategies);

        var result = _controller.GetStrategies();
        var okResult = result as OkObjectResult;
        var strategiesDto = okResult!.Value as List<ScoringStrategyDto>;

        Assert.IsNotNull(strategiesDto);
        Assert.AreEqual(2, strategiesDto.Count);
    }

    [TestMethod]
    public void GetStrategies_Should_Return_StrategiesWithCorrectNames()
    {
        var config1 = new ThemePark.Entities.ConfiguracionPorAtraccion([]);
        var config2 = new ThemePark.Entities.ConfiguracionPorCombo(15, 50, 3);
        var strategies = new List<ThemePark.Entities.ScoringStrategy>
        {
            new ThemePark.Entities.ScoringStrategy("PuntuacionPorAtraccion", ThemePark.Enums.TipoEstrategia.PuntuacionPorAtraccion, "desc", config1),
            new ThemePark.Entities.ScoringStrategy("PuntuacionCombo", ThemePark.Enums.TipoEstrategia.PuntuacionCombo, "desc", config2)
        };
        _mockService.Setup(x => x.GetAllStrategies()).Returns(strategies);

        var result = _controller.GetStrategies();
        var okResult = result as OkObjectResult;
        var strategiesDto = okResult!.Value as List<ScoringStrategyDto>;

        Assert.AreEqual("PuntuacionPorAtraccion", strategiesDto![0].Nombre);
        Assert.AreEqual("PuntuacionCombo", strategiesDto[1].Nombre);
    }

    [TestMethod]
    public void GetStrategy_Should_Return_OkResult_With_Specific_Strategy()
    {
        const string strategyName = "PuntuacionPorAtraccion";
        var getConfig = new ThemePark.Entities.ConfiguracionPorAtraccion([]);
        var expectedEntity = new ThemePark.Entities.ScoringStrategy("PuntuacionPorAtraccion", ThemePark.Enums.TipoEstrategia.PuntuacionPorAtraccion, "Otorga puntos fijos por tipo de atracción", getConfig) { Active = true };
        _mockService.Setup(x => x.GetByName(strategyName))
                   .Returns(expectedEntity);
        var result = _controller.GetStrategy(strategyName);
        Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
    }

    [TestMethod]
    public void GetStrategy_Should_Return_StrategyWithCorrectName()
    {
        const string strategyName = "PuntuacionPorAtraccion";
        var getConfig = new ThemePark.Entities.ConfiguracionPorAtraccion([]);
        var expectedEntity = new ThemePark.Entities.ScoringStrategy("PuntuacionPorAtraccion", ThemePark.Enums.TipoEstrategia.PuntuacionPorAtraccion, "Otorga puntos fijos por tipo de atracción", getConfig) { Active = true };
        _mockService.Setup(x => x.GetByName(strategyName))
                   .Returns(expectedEntity);

        var result = _controller.GetStrategy(strategyName);
        var okResult = result as OkObjectResult;
        var strategyDto = okResult!.Value as ScoringStrategyDto;

        Assert.IsNotNull(strategyDto);
        Assert.AreEqual("PuntuacionPorAtraccion", strategyDto.Nombre);
    }

    [TestMethod]
    public void CreateStrategy_Should_Return_CreatedResult_With_New_Strategy()
    {
        var createDto = new CreateScoringStrategyDto
        {
            Nombre = "PuntuacionCombo",
            Descripcion = "Bonifica cadenas de atracciones diferentes",
            Algoritmo = TipoAlgoritmoDto.PuntuacionCombo,
            Configuracion = new ThemePark.Models.Configuracion.ConfiguracionPorComboDto { VentanaTemporalMinutos = 15, BonusMultiplicador = 50, MinimoAtracciones = 3 }
        };
        var expectedConfig = new ThemePark.Entities.ConfiguracionPorCombo(15, 50, 3);
        var expectedEntity = new ThemePark.Entities.ScoringStrategy("PuntuacionCombo", ThemePark.Enums.TipoEstrategia.PuntuacionCombo, "Bonifica cadenas de atracciones diferentes", expectedConfig) { Id = Guid.NewGuid(), Active = false };
        _mockService.Setup(x => x.CreateStrategy(
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<ThemePark.Enums.TipoEstrategia?>(),
            It.IsAny<ThemePark.Entities.Configuracion>(),
            It.IsAny<string>(),
            It.IsAny<string>()))
            .Returns(expectedEntity);
        var result = _controller.CreateStrategy(createDto);
        Assert.IsInstanceOfType(result, typeof(CreatedAtActionResult));
        var createdResult = result as CreatedAtActionResult;
        Assert.IsNotNull(createdResult);
    }

    [TestMethod]
    public void CreateStrategy_Should_Return_CreatedWithCorrectAction()
    {
        var createDto = new CreateScoringStrategyDto
        {
            Nombre = "PuntuacionCombo",
            Descripcion = "Bonifica cadenas de atracciones diferentes",
            Algoritmo = TipoAlgoritmoDto.PuntuacionCombo,
            Configuracion = new ThemePark.Models.Configuracion.ConfiguracionPorComboDto { VentanaTemporalMinutos = 15, BonusMultiplicador = 50, MinimoAtracciones = 3 }
        };
        var expectedConfig = new ThemePark.Entities.ConfiguracionPorCombo(15, 50, 3);
        var expectedEntity = new ThemePark.Entities.ScoringStrategy("PuntuacionCombo", ThemePark.Enums.TipoEstrategia.PuntuacionCombo, "Bonifica cadenas de atracciones diferentes", expectedConfig) { Id = Guid.NewGuid(), Active = false };
        _mockService.Setup(x => x.CreateStrategy(
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<ThemePark.Enums.TipoEstrategia?>(),
            It.IsAny<ThemePark.Entities.Configuracion>(),
            It.IsAny<string>(),
            It.IsAny<string>()))
            .Returns(expectedEntity);

        var result = _controller.CreateStrategy(createDto);
        var createdResult = result as CreatedAtActionResult;

        Assert.AreEqual("GetStrategy", createdResult!.ActionName);
        Assert.AreEqual(expectedEntity.Name, createdResult.RouteValues!["nombre"]);
    }

    [TestMethod]
    public void CreateStrategy_Should_Return_StrategyWithCorrectName()
    {
        var createDto = new CreateScoringStrategyDto
        {
            Nombre = "PuntuacionCombo",
            Descripcion = "Bonifica cadenas de atracciones diferentes",
            Algoritmo = TipoAlgoritmoDto.PuntuacionCombo,
            Configuracion = new ThemePark.Models.Configuracion.ConfiguracionPorComboDto { VentanaTemporalMinutos = 15, BonusMultiplicador = 50, MinimoAtracciones = 3 }
        };
        var expectedConfig = new ThemePark.Entities.ConfiguracionPorCombo(15, 50, 3);
        var expectedEntity = new ThemePark.Entities.ScoringStrategy("PuntuacionCombo", ThemePark.Enums.TipoEstrategia.PuntuacionCombo, "Bonifica cadenas de atracciones diferentes", expectedConfig) { Id = Guid.NewGuid(), Active = false };
        _mockService.Setup(x => x.CreateStrategy(
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<ThemePark.Enums.TipoEstrategia?>(),
            It.IsAny<ThemePark.Entities.Configuracion>(),
            It.IsAny<string>(),
            It.IsAny<string>()))
            .Returns(expectedEntity);

        var result = _controller.CreateStrategy(createDto);
        var createdResult = result as CreatedAtActionResult;
        var createdDto = createdResult!.Value as ScoringStrategyDto;

        Assert.IsNotNull(createdDto);
        Assert.AreEqual("PuntuacionCombo", createdDto.Nombre);
    }

    [TestMethod]
    public void ToggleActiveStrategy_Should_Return_OkResult_With_Updated_Strategy()
    {
        const string strategyName = "PuntuacionPorAtraccion";
        var toggleConfig = new ThemePark.Entities.ConfiguracionPorAtraccion(new Dictionary<string, int> { ["montanaRusa"] = 50, ["carrusel"] = 20 });
        var expectedEntity = new ThemePark.Entities.ScoringStrategy("PuntuacionPorAtraccion", ThemePark.Enums.TipoEstrategia.PuntuacionPorAtraccion, "Otorga puntos fijos por tipo de atracción", toggleConfig) { Id = Guid.NewGuid(), Active = true };
        _mockService.Setup(x => x.ToggleActive(strategyName))
                   .Returns(expectedEntity);
        var result = _controller.ToggleActiveStrategy(strategyName);
        Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
    }

    [TestMethod]
    public void ToggleActiveStrategy_Should_Return_StrategyWithCorrectName()
    {
        const string strategyName = "PuntuacionPorAtraccion";
        var toggleConfig = new ThemePark.Entities.ConfiguracionPorAtraccion(new Dictionary<string, int> { ["montanaRusa"] = 50, ["carrusel"] = 20 });
        var expectedEntity = new ThemePark.Entities.ScoringStrategy("PuntuacionPorAtraccion", ThemePark.Enums.TipoEstrategia.PuntuacionPorAtraccion, "Otorga puntos fijos por tipo de atracción", toggleConfig) { Id = Guid.NewGuid(), Active = true };
        _mockService.Setup(x => x.ToggleActive(strategyName))
                   .Returns(expectedEntity);

        var result = _controller.ToggleActiveStrategy(strategyName);
        var okResult = result as OkObjectResult;
        var toggledDto = okResult!.Value as ScoringStrategyDto;

        Assert.IsNotNull(toggledDto);
        Assert.AreEqual("PuntuacionPorAtraccion", toggledDto.Nombre);
    }

    [TestMethod]
    public void UpdateStrategy_Should_Return_OkResult_With_Updated_Strategy()
    {
        const string strategyName = "PuntuacionCombo";
        var updateDto = new UpdateScoringStrategyDto
        {
            Descripcion = "Nueva descripción actualizada",
            Algoritmo = TipoAlgoritmoDto.PuntuacionCombo,
            Configuracion = new ThemePark.Models.Configuracion.ConfiguracionPorComboDto { VentanaTemporalMinutos = 20, BonusMultiplicador = 75, MinimoAtracciones = 2 }
        };
        var updateConfig = new ThemePark.Entities.ConfiguracionPorCombo(20, 75, 2);
        var expectedEntity = new ThemePark.Entities.ScoringStrategy("PuntuacionCombo", ThemePark.Enums.TipoEstrategia.PuntuacionCombo, "Nueva descripción actualizada", updateConfig) { Id = Guid.NewGuid(), Active = false };
        _mockService.Setup(x => x.Update(strategyName, It.IsAny<ThemePark.Entities.ScoringStrategy>()))
                   .Returns(expectedEntity);

        var result = _controller.UpdateStrategy(strategyName, updateDto);
        Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
    }

    [TestMethod]
    public void UpdateStrategy_Should_Return_StrategyWithCorrectName()
    {
        const string strategyName = "PuntuacionCombo";
        var updateDto = new UpdateScoringStrategyDto
        {
            Descripcion = "Nueva descripción actualizada",
            Algoritmo = TipoAlgoritmoDto.PuntuacionCombo,
            Configuracion = new ThemePark.Models.Configuracion.ConfiguracionPorComboDto { VentanaTemporalMinutos = 20, BonusMultiplicador = 75, MinimoAtracciones = 2 }
        };
        var updateConfig = new ThemePark.Entities.ConfiguracionPorCombo(20, 75, 2);
        var expectedEntity = new ThemePark.Entities.ScoringStrategy("PuntuacionCombo", ThemePark.Enums.TipoEstrategia.PuntuacionCombo, "Nueva descripción actualizada", updateConfig) { Id = Guid.NewGuid(), Active = false };
        _mockService.Setup(x => x.Update(strategyName, It.IsAny<ThemePark.Entities.ScoringStrategy>()))
                   .Returns(expectedEntity);

        var result = _controller.UpdateStrategy(strategyName, updateDto);
        var okResult = result as OkObjectResult;
        var updatedDto = okResult!.Value as ScoringStrategyDto;

        Assert.IsNotNull(updatedDto);
        Assert.AreEqual("PuntuacionCombo", updatedDto.Nombre);
    }

    [TestMethod]
    public void DeleteStrategy_Should_Return_NoContentResult()
    {
        const string strategyName = "PuntuacionPorAtraccion";
        _mockService.Setup(x => x.Delete(strategyName));
        var result = _controller.DeleteStrategy(strategyName);
        Assert.IsInstanceOfType(result, typeof(NoContentResult));
    }

    [TestMethod]
    public void ExceptionFilter_Should_Return_BadRequest_When_ScoringStrategyException_Occurs()
    {
        var exception = ScoringStrategyException.NoActiveStrategy();
        var exceptionFilter = new ExceptionFilter();
        var exceptionContext = new ExceptionContext(
            new ActionContext(
                new Mock<HttpContext>().Object,
                new RouteData(),
                new ActionDescriptor()),
            [])
        {
            Exception = exception
        };

        exceptionFilter.OnException(exceptionContext);

        var response = exceptionContext.Result as ObjectResult;
        Assert.IsNotNull(response);
        Assert.AreEqual(400, response.StatusCode);
    }

    [TestMethod]
    public void ExceptionFilter_Should_Return_CorrectTitleAndMessage_When_ScoringStrategyException_Occurs()
    {
        var exception = ScoringStrategyException.NoActiveStrategy();
        var exceptionFilter = new ExceptionFilter();
        var exceptionContext = new ExceptionContext(
            new ActionContext(
                new Mock<HttpContext>().Object,
                new RouteData(),
                new ActionDescriptor()),
            [])
        {
            Exception = exception
        };

        exceptionFilter.OnException(exceptionContext);

        var response = exceptionContext.Result as ObjectResult;
        Assert.AreEqual("Error de Estrategia de Puntuación", GetTitle(response!.Value!));
        Assert.AreEqual(exception.Message, GetMessage(response.Value!));
    }

    [TestMethod]
    public void ExceptionFilter_Should_Return_Unauthorized_When_MissingTokenException_Occurs()
    {
        var exception = new MissingTokenException();
        var exceptionFilter = new ExceptionFilter();
        var exceptionContext = new ExceptionContext(
            new ActionContext(
                new Mock<HttpContext>().Object,
                new RouteData(),
                new ActionDescriptor()),
            [])
        {
            Exception = exception
        };

        exceptionFilter.OnException(exceptionContext);

        var response = exceptionContext.Result as ObjectResult;
        Assert.IsNotNull(response);
        Assert.AreEqual(401, response.StatusCode);
    }

    [TestMethod]
    public void ExceptionFilter_Should_Return_CorrectTitleAndMessage_When_UnauthorizedAccessException_Occurs()
    {
        var expectedMessage = "Authentication token is required";
        var exception = new UnauthorizedAccessException(expectedMessage);
        var exceptionFilter = new ExceptionFilter();
        var exceptionContext = new ExceptionContext(
            new ActionContext(
                new Mock<HttpContext>().Object,
                new RouteData(),
                new ActionDescriptor()),
            [])
        {
            Exception = exception
        };

        exceptionFilter.OnException(exceptionContext);

        var response = exceptionContext.Result as ObjectResult;
        Assert.AreEqual("Error Interno del Servidor", GetTitle(response!.Value!));
        Assert.AreEqual("Ocurrió un error inesperado", GetMessage(response.Value!));
    }

    [TestMethod]
    public void ExceptionFilter_Should_Return_Forbidden_When_ForbiddenException_Occurs()
    {
        var expectedMessage = "User does not have permission to perform this action";
        var exception = new ForbiddenException(expectedMessage);
        var exceptionFilter = new ExceptionFilter();
        var exceptionContext = new ExceptionContext(
            new ActionContext(
                new Mock<HttpContext>().Object,
                new RouteData(),
                new ActionDescriptor()),
            [])
        {
            Exception = exception
        };

        exceptionFilter.OnException(exceptionContext);

        var response = exceptionContext.Result as ObjectResult;
        Assert.IsNotNull(response);
        Assert.AreEqual(403, response.StatusCode);
    }

    [TestMethod]
    public void ExceptionFilter_Should_Return_CorrectTitleAndMessage_When_ForbiddenException_Occurs()
    {
        var expectedMessage = "User does not have permission to perform this action";
        var exception = new ForbiddenException(expectedMessage);
        var exceptionFilter = new ExceptionFilter();
        var exceptionContext = new ExceptionContext(
            new ActionContext(
                new Mock<HttpContext>().Object,
                new RouteData(),
                new ActionDescriptor()),
            [])
        {
            Exception = exception
        };

        exceptionFilter.OnException(exceptionContext);

        var response = exceptionContext.Result as ObjectResult;
        Assert.AreEqual("Acceso Prohibido", GetTitle(response!.Value!));
        Assert.AreEqual(expectedMessage, GetMessage(response.Value!));
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
    public void DeactivateStrategy_ShouldReturn200_WhenActiveStrategyExists()
    {
        _mockService.Setup(x => x.Deactivate());

        var result = _controller.DeactivateStrategy();

        Assert.IsInstanceOfType(result, typeof(OkResult));
    }

    [TestMethod]
    public void DeactivateStrategy_ShouldThrowException_WhenNoActiveStrategy()
    {
        _mockService.Setup(x => x.Deactivate()).Throws(ScoringStrategyException.NoActiveStrategyToDeactivate());

        Assert.ThrowsException<ScoringStrategyException>(_controller.DeactivateStrategy);
    }

    [TestMethod]
    public void GetAvailableTypes_Should_Return_OkResult()
    {
        _mockService.Setup(x => x.GetAvailableStrategyTypes())
            .Returns([]);

        var result = _controller.GetAvailableTypes();

        Assert.IsInstanceOfType(result, typeof(OkObjectResult));
    }

    [TestMethod]
    public void GetConfigSchema_ShouldReturnOkResult_WhenPluginExists()
    {
        var mockPlugin = new Mock<IScoringStrategyPlugin>();
        mockPlugin.Setup(p => p.StrategyTypeIdentifier).Returns("PuntuacionPorHora");
        mockPlugin.Setup(p => p.StrategyName).Returns("Puntuación por Hora");
        mockPlugin.Setup(p => p.Description).Returns("Multiplica puntos durante ciertas horas");
        mockPlugin.Setup(p => p.GetConfigurationSchema()).Returns(new { Type = "object" });

        _mockPluginLoader.Setup(l => l.GetPlugin("PuntuacionPorHora")).Returns(mockPlugin.Object);

        var result = _controller.GetConfigSchema("PuntuacionPorHora");

        Assert.IsInstanceOfType(result, typeof(OkObjectResult));
    }

    [TestMethod]
    public void GetConfigSchema_ShouldReturnOkResult_WhenPluginDoesNotExist()
    {
        _mockPluginLoader.Setup(l => l.GetPlugin("PuntuacionPorAtraccion")).Returns((IScoringStrategyPlugin?)null);

        var result = _controller.GetConfigSchema("PuntuacionPorAtraccion");

        Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.IsNotNull(okResult.Value);
    }
}
