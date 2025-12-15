using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Moq;
using ThemePark.Entities;
using ThemePark.Enums;
using ThemePark.IBusinessLogic;
using ThemePark.IDataAccess.Exceptions;
using ThemePark.Models.Attractions;
using ThemeParkApi.Controllers;
using ThemeParkApi.Filters;

namespace ThemePark.TestAPI;

[TestClass]
public class AttractionsControllerCrudTests
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
    public void CreateAttraction_ShouldReturnCreated_WhenValidRequestProvided()
    {
        var request = new CreateAttractionRequest
        {
            Nombre = "Montaña Rusa T-Rex",
            Tipo = 0,
            EdadMinima = 12,
            CapacidadMaxima = 50,
            Descripcion = "Emocionante montaña rusa temática de dinosaurios"
        };

        var domainAttraction = new Attraction("Montaña Rusa T-Rex", TipoAtraccion.MontañaRusa, 12, 50,
            "Emocionante montaña rusa temática de dinosaurios", new DateTime(2025, 9, 18, 14, 30, 0));

        var expectedResponse = new AttractionResponseModel
        {
            Nombre = "Montaña Rusa T-Rex",
            Tipo = "MontañaRusa",
            EdadMinima = 12,
            CapacidadMaxima = 50,
            Descripcion = "Emocionante montaña rusa temática de dinosaurios",
            FechaCreacion = "2025-09-18T14:30:00"
        };

        _mockAttractionsBusinessLogic.Setup(x => x.CreateAttraction(
            request.Nombre, request.Tipo!.Value, request.EdadMinima!.Value, request.CapacidadMaxima!.Value, request.Descripcion, It.IsAny<int>()))
            .Returns(domainAttraction);

        var result = _controller.CreateAttraction(request);

        Assert.IsInstanceOfType(result, typeof(CreatedResult));
        var createdResult = result as CreatedResult;
        Assert.AreEqual(201, createdResult!.StatusCode);
        _mockAttractionsBusinessLogic.Verify(x => x.CreateAttraction(
            request.Nombre, request.Tipo!.Value, request.EdadMinima!.Value, request.CapacidadMaxima!.Value, request.Descripcion, It.IsAny<int>()), Times.Once);
    }

    [TestMethod]
    public void CreateAttraction_ShouldThrowArgumentException_WhenAttractionNameAlreadyExists()
    {
        var request = new CreateAttractionRequest
        {
            Nombre = "Montaña Rusa T-Rex",
            Tipo = 0,
            EdadMinima = 12,
            CapacidadMaxima = 50,
            Descripcion = "Emocionante montaña rusa temática de dinosaurios"
        };

        _mockAttractionsBusinessLogic.Setup(x => x.CreateAttraction(
            request.Nombre, request.Tipo!.Value, request.EdadMinima!.Value, request.CapacidadMaxima!.Value, request.Descripcion, It.IsAny<int>()))
            .Throws(new ArgumentException("Attraction name already exists"));

        Assert.ThrowsException<ArgumentException>(() => _controller.CreateAttraction(request));
        _mockAttractionsBusinessLogic.Verify(x => x.CreateAttraction(
            request.Nombre, request.Tipo!.Value, request.EdadMinima!.Value, request.CapacidadMaxima!.Value, request.Descripcion, It.IsAny<int>()), Times.Once);
    }

    [TestMethod]
    public void DeleteAttraction_ShouldReturnNoContent_WhenValidNameProvided()
    {
        var nombre = "Montaña Rusa T-Rex";

        _mockAttractionsBusinessLogic.Setup(x => x.DeleteAttraction(nombre));

        var result = _controller.DeleteAttraction(nombre);

        Assert.IsInstanceOfType(result, typeof(NoContentResult));
        _mockAttractionsBusinessLogic.Verify(x => x.DeleteAttraction(nombre), Times.Once);
    }

    [TestMethod]
    public void DeleteAttraction_ShouldThrowArgumentException_WhenAttractionNotFound()
    {
        var nombre = "Atraccion Inexistente";

        _mockAttractionsBusinessLogic.Setup(x => x.DeleteAttraction(nombre))
            .Throws(new ArgumentException("Attraction not found"));

        Assert.ThrowsException<ArgumentException>(() => _controller.DeleteAttraction(nombre));
        _mockAttractionsBusinessLogic.Verify(x => x.DeleteAttraction(nombre), Times.Once);
    }

    [TestMethod]
    public void UpdateAttraction_ShouldReturnOkWithBasicProperties_WhenValidRequestProvided()
    {
        var nombre = "Montaña Rusa T-Rex";
        var request = new UpdateAttractionRequest
        {
            Descripcion = "Emocionante simulador temático de dinosaurios",
            EdadMinima = 8,
            CapacidadMaxima = 30
        };

        var domainAttraction = new Attraction("Montaña Rusa T-Rex", TipoAtraccion.Simulador, 8, 30,
            "Emocionante simulador temático de dinosaurios", new DateTime(2025, 9, 18, 14, 30, 0));

        _mockAttractionsBusinessLogic.Setup(x => x.UpdateAttraction(nombre, request.Descripcion, request.CapacidadMaxima, request.EdadMinima))
            .Returns(domainAttraction);

        var result = _controller.UpdateAttraction(nombre, request);

        Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        var okResult = result as OkObjectResult;
        var response = okResult!.Value as AttractionResponseModel;
        Assert.AreEqual("Montaña Rusa T-Rex", response!.Nombre);
        Assert.AreEqual("Simulador", response.Tipo);
    }

    [TestMethod]
    public void UpdateAttraction_ShouldReturnUpdatedProperties_WhenValidRequestProvided()
    {
        var nombre = "Montaña Rusa T-Rex";
        var request = new UpdateAttractionRequest
        {
            Descripcion = "Emocionante simulador temático de dinosaurios",
            EdadMinima = 8,
            CapacidadMaxima = 30
        };

        var domainAttraction = new Attraction("Montaña Rusa T-Rex", TipoAtraccion.Simulador, 8, 30,
            "Emocionante simulador temático de dinosaurios", new DateTime(2025, 9, 18, 14, 30, 0));

        _mockAttractionsBusinessLogic.Setup(x => x.UpdateAttraction(nombre, request.Descripcion, request.CapacidadMaxima, request.EdadMinima))
            .Returns(domainAttraction);

        var result = _controller.UpdateAttraction(nombre, request);

        var okResult = result as OkObjectResult;
        var response = okResult!.Value as AttractionResponseModel;
        Assert.AreEqual(8, response!.EdadMinima);
        Assert.AreEqual("Simulador", response.Tipo);
        Assert.AreEqual(8, response.EdadMinima);
        Assert.AreEqual(30, response.CapacidadMaxima);
        Assert.AreEqual("Emocionante simulador temático de dinosaurios", response.Descripcion);
    }

    [TestMethod]
    public void UpdateAttraction_ShouldReturnOkWithBasicProperties_WhenDescripcionIsNull()
    {
        var nombre = "Carrusel Clásico";
        var request = new UpdateAttractionRequest
        {
            Descripcion = null,
            EdadMinima = 5,
            CapacidadMaxima = 25
        };

        var domainAttraction = new Attraction("Carrusel Clásico", TipoAtraccion.Simulador, 5, 25,
            string.Empty, new DateTime(2025, 9, 18, 14, 30, 0));

        _mockAttractionsBusinessLogic.Setup(x => x.UpdateAttraction(nombre, string.Empty, request.CapacidadMaxima, request.EdadMinima))
            .Returns(domainAttraction);

        var result = _controller.UpdateAttraction(nombre, request);

        Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        var okResult = result as OkObjectResult;
        var response = okResult!.Value as AttractionResponseModel;
        Assert.AreEqual("Carrusel Clásico", response!.Nombre);
        Assert.AreEqual("Simulador", response.Tipo);
        Assert.AreEqual(5, response.EdadMinima);
    }

    [TestMethod]
    public void UpdateAttraction_ShouldUseEmptyString_WhenDescripcionIsNull()
    {
        var nombre = "Carrusel Clásico";
        var request = new UpdateAttractionRequest
        {
            Descripcion = null,
            EdadMinima = 5,
            CapacidadMaxima = 25
        };

        var domainAttraction = new Attraction("Carrusel Clásico", TipoAtraccion.MontañaRusa, 5, 25,
            string.Empty, new DateTime(2025, 9, 18, 14, 30, 0));

        _mockAttractionsBusinessLogic.Setup(x => x.UpdateAttraction(nombre, string.Empty, request.CapacidadMaxima, request.EdadMinima))
            .Returns(domainAttraction);

        var result = _controller.UpdateAttraction(nombre, request);

        var okResult = result as OkObjectResult;
        var response = okResult!.Value as AttractionResponseModel;
        Assert.AreEqual(5, response!.EdadMinima);
        Assert.AreEqual(25, response.CapacidadMaxima);
        Assert.AreEqual(string.Empty, response.Descripcion);
    }

    [TestMethod]
    public void GetAll_ShouldReturnOkWithAllAttractions_WhenAttractionsExist()
    {
        var attraction1 = new Attraction("Montaña Rusa", TipoAtraccion.MontañaRusa, 12, 50, "Emocionante", DateTime.Now);
        var attraction2 = new Attraction("Carrusel", TipoAtraccion.ZonaInteractiva, 5, 40, "Tranquila", DateTime.Now);
        var attractions = new List<Attraction> { attraction1, attraction2 };

        _mockAttractionsBusinessLogic.Setup(x => x.GetAll()).Returns(attractions);

        var result = _controller.GetAll();

        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        var response = okResult.Value as List<AttractionResponseModel>;
        Assert.AreEqual(2, response!.Count);
        Assert.AreEqual("Montaña Rusa", response[0].Nombre);
        Assert.AreEqual("Carrusel", response[1].Nombre);
    }

    [TestMethod]
    public void GetById_ShouldReturnOkWithAttraction_WhenAttractionExists()
    {
        var attraction = new Attraction("Casa Embrujada", TipoAtraccion.Espectaculo, 16, 20, "Experiencia aterradora", DateTime.Now);

        _mockAttractionsBusinessLogic.Setup(x => x.GetById("Casa Embrujada")).Returns(attraction);

        var result = _controller.GetById("Casa Embrujada");

        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        var response = okResult.Value as AttractionResponseModel;
        Assert.AreEqual("Casa Embrujada", response!.Nombre);
        Assert.AreEqual("Espectaculo", response.Tipo);
        Assert.AreEqual(16, response.EdadMinima);
    }

    [TestMethod]
    public void GetById_ShouldThrowAttractionNotFoundException_WhenAttractionDoesNotExist()
    {
        _mockAttractionsBusinessLogic.Setup(x => x.GetById("AtraccionInexistente"))
            .Throws(new AttractionNotFoundException("AtraccionInexistente"));

        Assert.ThrowsException<AttractionNotFoundException>(() => _controller.GetById("AtraccionInexistente"));
    }

    [TestMethod]
    public void GetAllIncidents_ShouldReturnOkWithAllIncidents_WhenIncidentsExist()
    {
        var incident1 = new Incident("Montaña Rusa", "Falla en el motor", new DateTime(2025, 10, 1));
        var incident2 = new Incident("Torre de Caída", "Problema eléctrico", new DateTime(2025, 10, 2));
        var incident3 = new Incident("Carrusel", "Mantenimiento preventivo", new DateTime(2025, 10, 3));
        incident3.Resolve(new DateTime(2025, 10, 3, 15, 0, 0));

        var incidents = new List<Incident> { incident1, incident2, incident3 };

        _mockAttractionsBusinessLogic.Setup(x => x.GetAllIncidents()).Returns(incidents);

        var result = _controller.GetAllIncidents();

        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        var response = okResult.Value as List<IncidentResponseModel>;
        Assert.AreEqual(3, response!.Count);
        Assert.AreEqual("Montaña Rusa", response[0].Atraccion);
        Assert.AreEqual("Torre de Caída", response[1].Atraccion);
        Assert.AreEqual("Carrusel", response[2].Atraccion);
    }

    [TestMethod]
    public void GetAllIncidents_ShouldReturnEmptyList_WhenNoIncidentsExist()
    {
        _mockAttractionsBusinessLogic.Setup(x => x.GetAllIncidents()).Returns([]);

        var result = _controller.GetAllIncidents();

        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        var response = okResult.Value as List<IncidentResponseModel>;
        Assert.AreEqual(0, response!.Count);
    }

    [TestMethod]
    public void GetIncidentsByAttraction_ShouldReturnOkWithIncidents_WhenIncidentsExist()
    {
        var attractionName = "Montaña Rusa";
        var incident1 = new Incident(attractionName, "Falla en motor", new DateTime(2025, 10, 1));
        var incident2 = new Incident(attractionName, "Problema eléctrico", new DateTime(2025, 10, 2));

        var incidents = new List<Incident> { incident1, incident2 };

        _mockAttractionsBusinessLogic.Setup(x => x.GetIncidentsByAttraction(attractionName)).Returns(incidents);

        var result = _controller.GetIncidentsByAttraction(attractionName);

        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        var response = okResult.Value as List<IncidentResponseModel>;
        Assert.AreEqual(2, response!.Count);
        Assert.IsTrue(response.All(i => i.Atraccion == attractionName));
    }

    [TestMethod]
    public void GetIncidentsByAttraction_ShouldReturnEmptyList_WhenNoIncidentsExist()
    {
        var attractionName = "Carrusel";

        _mockAttractionsBusinessLogic.Setup(x => x.GetIncidentsByAttraction(attractionName)).Returns([]);

        var result = _controller.GetIncidentsByAttraction(attractionName);

        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        var response = okResult.Value as List<IncidentResponseModel>;
        Assert.AreEqual(0, response!.Count);
    }
}
