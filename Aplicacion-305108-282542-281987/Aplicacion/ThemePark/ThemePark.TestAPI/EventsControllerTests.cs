using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Moq;
using ThemePark.Entities;
using ThemePark.Exceptions;
using ThemePark.IBusinessLogic;
using ThemePark.IDataAccess.Exceptions;
using ThemePark.Models.Events;
using ThemeParkApi.Controllers;
using ThemeParkApi.Filters;

namespace ThemePark.TestAPI;

[TestClass]
public class EventsControllerTests
{
    private Mock<IEventsService> _mockEventsBusinessLogic = null!;
    private EventsController _controller = null!;
    private ExceptionFilter _exceptionFilter = null!;
    private ExceptionContext _exceptionContext = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockEventsBusinessLogic = new Mock<IEventsService>(MockBehavior.Strict);
        _controller = new EventsController(_mockEventsBusinessLogic.Object);
        _exceptionFilter = new ExceptionFilter();
    }

    [TestCleanup]
    public void Cleanup()
    {
        _mockEventsBusinessLogic = null!;
        _controller = null!;
    }

    [TestMethod]
    public void CreateEvent_ShouldReturnCreated_WhenValidEventProvided()
    {
        var request = new CreateEventRequest
        {
            Nombre = "Noche de Dinosaurios",
            Fecha = System.DateTime.Parse("2027-10-31"),
            Hora = System.TimeSpan.Parse("20:00"),
            Aforo = 100,
            CostoAdicional = 50.0m,
            AtraccionesIncluidas = ["Jurassic Adventure", "T-Rex Simulator"]
        };

        var expectedEvent = new Event
        {
            Id = Guid.NewGuid(),
            Name = "Noche de Dinosaurios",
            Fecha = request.Fecha,
            Hora = request.Hora,
            Aforo = request.Aforo,
            CostoAdicional = request.CostoAdicional
        };

        _mockEventsBusinessLogic.Setup(bl => bl.CreateEvent(
            request.Nombre,
            request.Fecha,
            request.Hora,
            request.Aforo,
            request.CostoAdicional,
            request.AtraccionesIncluidas))
            .Returns(expectedEvent);

        var result = _controller.CreateEvent(request);

        var createdResult = result as CreatedResult;
        Assert.IsNotNull(createdResult);
        Assert.AreEqual(201, createdResult.StatusCode);
        Assert.IsNotNull(createdResult.Value);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidEventDataException))]
    public void CreateEvent_ShouldThrowInvalidEventDataException_WhenNameIsEmpty()
    {
        var request = new CreateEventRequest
        {
            Nombre = string.Empty,
            Fecha = System.DateTime.Parse("2025-10-31"),
            Hora = System.TimeSpan.Parse("20:00"),
            Aforo = 100,
            CostoAdicional = 50.0m,
            AtraccionesIncluidas = ["Jurassic Adventure"]
        };

        _mockEventsBusinessLogic.Setup(bl => bl.CreateEvent(
            request.Nombre,
            request.Fecha,
            request.Hora,
            request.Aforo,
            request.CostoAdicional,
            request.AtraccionesIncluidas))
            .Throws(new InvalidEventDataException("Nombre", "vacío"));

        _controller.CreateEvent(request);
    }

    [TestMethod]
    public void UpdateEvent_ShouldReturnOk_WhenValidEventProvided()
    {
        var eventId = Guid.NewGuid().ToString();
        var request = new CreateEventRequest
        {
            Nombre = "Noche de Dinosaurios Actualizada",
            Fecha = System.DateTime.Parse("2027-11-01"),
            Hora = System.TimeSpan.Parse("21:00"),
            Aforo = 150,
            CostoAdicional = 60.0m,
            AtraccionesIncluidas = ["Jurassic Adventure", "T-Rex Simulator"]
        };

        var expectedEvent = new Event
        {
            Id = Guid.Parse(eventId),
            Name = request.Nombre,
            Fecha = request.Fecha,
            Hora = request.Hora,
            Aforo = request.Aforo,
            CostoAdicional = request.CostoAdicional
        };

        _mockEventsBusinessLogic.Setup(bl => bl.UpdateEvent(
            eventId,
            request.Nombre,
            request.Fecha,
            request.Hora,
            request.Aforo,
            request.CostoAdicional,
            request.AtraccionesIncluidas))
            .Returns(expectedEvent);

        var result = _controller.UpdateEvent(eventId, request);

        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);
        Assert.IsNotNull(okResult.Value);
    }

    [TestMethod]
    [ExpectedException(typeof(EventNotFoundException))]
    public void UpdateEvent_ShouldThrowEventNotFoundException_WhenEventDoesNotExist()
    {
        var eventId = Guid.NewGuid().ToString();
        var request = new CreateEventRequest
        {
            Nombre = "Evento No Existente",
            Fecha = System.DateTime.Parse("2027-11-01"),
            Hora = System.TimeSpan.Parse("21:00"),
            Aforo = 150,
            CostoAdicional = 60.0m,
            AtraccionesIncluidas = ["Jurassic Adventure"]
        };

        _mockEventsBusinessLogic.Setup(bl => bl.UpdateEvent(
            eventId,
            request.Nombre,
            request.Fecha,
            request.Hora,
            request.Aforo,
            request.CostoAdicional,
            request.AtraccionesIncluidas))
            .Throws(new EventNotFoundException(Guid.Parse(eventId)));

        _controller.UpdateEvent(eventId, request);
    }

    [TestMethod]
    public void DeleteEvent_ShouldReturnNoContent_WhenValidIdProvided()
    {
        var eventId = "event-123";

        _mockEventsBusinessLogic.Setup(bl => bl.DeleteEvent(eventId));

        var result = _controller.DeleteEvent(eventId);

        var noContentResult = result as NoContentResult;
        Assert.IsNotNull(noContentResult);
        Assert.AreEqual(204, noContentResult.StatusCode);
    }

    [TestMethod]
    public void GetAllEvents_ShouldReturnOk_WhenEventsExist()
    {
        var expectedEvents = new List<Event>
        {
            new Event
            {
                Id = Guid.NewGuid(),
                Name = "Noche de Dinosaurios",
                Fecha = DateTime.Parse("2027-10-31"),
                Hora = TimeSpan.Parse("20:00"),
                Aforo = 100,
                CostoAdicional = 50.0m
            },
            new Event
            {
                Id = Guid.NewGuid(),
                Name = "Festival de Halloween",
                Fecha = DateTime.Parse("2027-10-31"),
                Hora = TimeSpan.Parse("18:00"),
                Aforo = 200,
                CostoAdicional = 30.0m
            }
        };

        _mockEventsBusinessLogic.Setup(bl => bl.GetAllEvents())
            .Returns(expectedEvents);

        var result = _controller.GetAllEvents();

        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);
        Assert.IsNotNull(okResult.Value);
    }

    [TestMethod]
    public void GetEventById_ShouldReturnOk_WhenValidIdProvided()
    {
        var eventId = Guid.NewGuid().ToString();
        var expectedEvent = new Event
        {
            Id = Guid.Parse(eventId),
            Name = "Noche de Dinosaurios",
            Fecha = DateTime.Parse("2027-10-31"),
            Hora = TimeSpan.Parse("20:00"),
            Aforo = 100,
            CostoAdicional = 50.0m
        };

        _mockEventsBusinessLogic.Setup(bl => bl.GetEventById(eventId))
            .Returns(expectedEvent);

        var result = _controller.GetEventById(eventId);

        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);
        Assert.IsNotNull(okResult.Value);
    }

    [TestMethod]
    [ExpectedException(typeof(EventNotFoundException))]
    public void GetEventById_ShouldThrowEventNotFoundException_WhenEventDoesNotExist()
    {
        var eventId = Guid.NewGuid().ToString();

        _mockEventsBusinessLogic.Setup(bl => bl.GetEventById(eventId))
            .Throws(new EventNotFoundException(Guid.Parse(eventId)));

        _controller.GetEventById(eventId);
    }

    [TestMethod]
    public void ExceptionFilter_ShouldReturn404NotFound_WhenEventNotFoundExceptionOccurs()
    {
        var exception = new EventNotFoundException(Guid.NewGuid());
        SetupExceptionContext(exception);

        _exceptionFilter.OnException(_exceptionContext);

        var response = _exceptionContext.Result as ObjectResult;
        Assert.IsNotNull(response);
        Assert.AreEqual((int)HttpStatusCode.NotFound, response.StatusCode);
        Assert.AreEqual("Evento No Encontrado", GetTitle(response.Value!));
        Assert.AreEqual(exception.Message, GetMessage(response.Value!));
    }

    [TestMethod]
    public void ExceptionFilter_ShouldReturn400BadRequest_WhenInvalidEventDataExceptionOccurs()
    {
        var exception = new InvalidEventDataException("Nombre", "vacío");
        SetupExceptionContext(exception);

        _exceptionFilter.OnException(_exceptionContext);

        var response = _exceptionContext.Result as ObjectResult;
        Assert.IsNotNull(response);
        Assert.AreEqual((int)HttpStatusCode.BadRequest, response.StatusCode);
        Assert.AreEqual("Datos de Evento Inválidos", GetTitle(response.Value!));
        Assert.AreEqual(exception.Message, GetMessage(response.Value!));
    }

    [TestMethod]
    public void ExceptionFilter_ShouldReturn400BadRequest_WhenInvalidTimeFormatExceptionOccurs()
    {
        var exception = new InvalidTimeFormatException("25:00");
        SetupExceptionContext(exception);

        _exceptionFilter.OnException(_exceptionContext);

        var response = _exceptionContext.Result as ObjectResult;
        Assert.IsNotNull(response);
        Assert.AreEqual((int)HttpStatusCode.BadRequest, response.StatusCode);
        Assert.AreEqual("Formato de Hora Inválido", GetTitle(response.Value!));
        Assert.AreEqual(exception.Message, GetMessage(response.Value!));
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
}
