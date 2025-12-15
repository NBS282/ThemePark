using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Moq;
using ThemePark.Entities.Tickets;
using ThemePark.Exceptions;
using ThemePark.IBusinessLogic;
using ThemePark.IDataAccess.Exceptions;
using ThemePark.Models.Tickets;
using ThemeParkApi.Controllers;
using ThemeParkApi.Filters;

namespace ThemePark.TestAPI;

[TestClass]
public class TicketsControllerTests
{
    private TicketsController _controller = null!;
    private Mock<ITicketsBusinessLogic> _mockTicketsService = null!;
    private CreateTicketRequest _validRequest = null!;
    private GeneralTicket _expectedTicket = null!;
    private string _codigoIdentificacionUsuario = null!;
    private ExceptionFilter _exceptionFilter = null!;
    private ExceptionContext _exceptionContext = null!;

    [TestInitialize]
    public void TestInitialize()
    {
        _mockTicketsService = new Mock<ITicketsBusinessLogic>(MockBehavior.Strict);
        _controller = new TicketsController(_mockTicketsService.Object);
        _exceptionFilter = new ExceptionFilter();

        _codigoIdentificacionUsuario = "USER123";

        var fechaFutura = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd");

        _validRequest = new CreateTicketRequest
        {
            FechaVisita = fechaFutura,
            TipoEntrada = "general",
            CodigoIdentificacionUsuario = _codigoIdentificacionUsuario
        };

        _expectedTicket = new GeneralTicket
        {
            Id = Guid.NewGuid(),
            CodigoQR = Guid.NewGuid(),
            FechaVisita = DateTime.Parse(fechaFutura),
            CodigoIdentificacionUsuario = _codigoIdentificacionUsuario,
            FechaCompra = DateTime.Now
        };
    }

    [TestCleanup]
    public void TestCleanup()
    {
        _controller = null!;
        _mockTicketsService = null!;
        _validRequest = null!;
        _expectedTicket = null!;
        _codigoIdentificacionUsuario = null!;
    }

    [TestMethod]
    public void TicketsController_ShouldInitializeCorrectly()
    {
        Assert.IsNotNull(_controller);
        Assert.IsInstanceOfType(_controller, typeof(ControllerBase));
    }

    [TestMethod]
    public void CreateTicket_WithValidRequest_ShouldReturnCreatedResult()
    {
        _mockTicketsService.Setup(s => s.CreateTicket(It.IsAny<Ticket>()))
                          .Returns(_expectedTicket);

        var result = _controller.CreateTicket(_validRequest);

        Assert.IsInstanceOfType(result, typeof(CreatedResult));
        var createdResult = (CreatedResult)result;

        var responseModel = (TicketResponseModel)createdResult.Value!;
        Assert.AreEqual(_expectedTicket.CodigoQR, responseModel.CodigoQR);
        Assert.AreEqual(_expectedTicket.CodigoIdentificacionUsuario, responseModel.CodigoIdentificacionUsuario);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidTicketException))]
    public void CreateTicket_ShouldThrowInvalidTicketException_WhenDateIsInPast()
    {
        var pastDateRequest = new CreateTicketRequest
        {
            FechaVisita = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd"),
            TipoEntrada = "general",
            CodigoIdentificacionUsuario = _codigoIdentificacionUsuario
        };

        _mockTicketsService.Setup(s => s.CreateTicket(It.IsAny<Ticket>()))
                          .Throws(new InvalidTicketException("La fecha de visita no puede ser en el pasado"));

        _controller.CreateTicket(pastDateRequest);
    }

    [TestMethod]
    public void ExceptionFilter_ShouldReturn404NotFound_WhenTicketNotFoundExceptionOccurs()
    {
        var exception = new TicketNotFoundException(Guid.NewGuid());
        SetupExceptionContext(exception);

        _exceptionFilter.OnException(_exceptionContext);

        var response = _exceptionContext.Result as ObjectResult;
        Assert.IsNotNull(response);
        Assert.AreEqual((int)HttpStatusCode.NotFound, response.StatusCode);
        Assert.AreEqual("Ticket No Encontrado", GetTitle(response.Value!));
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

    [TestMethod]
    public void GetMyTickets_ShouldReturnOkWithTicketsList()
    {
        var tickets = new List<Ticket> { _expectedTicket };
        _mockTicketsService.Setup(s => s.GetMyTickets())
                          .Returns(tickets);

        var result = _controller.GetMyTickets();

        Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        var okResult = (OkObjectResult)result;
        var responseList = okResult.Value as List<TicketResponseModel>;
        Assert.IsNotNull(responseList);
        Assert.AreEqual(1, responseList.Count);
    }
}
