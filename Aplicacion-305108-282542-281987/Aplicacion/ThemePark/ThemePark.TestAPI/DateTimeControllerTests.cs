using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Moq;
using ThemePark.IBusinessLogic;
using ThemePark.IBusinessLogic.Exceptions;
using ThemePark.Models.DateTime;
using ThemeParkApi.Controllers;
using ThemeParkApi.Filters;

namespace ThemePark.TestAPI;

[TestClass]
public class DateTimeControllerTests
{
    private Mock<IDateTimeBusinessLogic> _mockDateTimeBusinessLogic = null!;
    private DateTimeController _controller = null!;
    private ExceptionFilter _exceptionFilter = null!;
    private ExceptionContext _exceptionContext = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockDateTimeBusinessLogic = new Mock<IDateTimeBusinessLogic>(MockBehavior.Strict);
        _controller = new DateTimeController(_mockDateTimeBusinessLogic.Object);
        _exceptionFilter = new ExceptionFilter();
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

    [TestMethod]
    public void GetDateTime_ShouldReturnCurrentDateTime_WhenCalledForFirstTime()
    {
        var expectedDateTime = "2025-09-17T14:30";
        _mockDateTimeBusinessLogic.Setup(x => x.GetCurrentDateTime())
            .Returns(expectedDateTime);

        var result = _controller.GetDateTime();

        Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        _mockDateTimeBusinessLogic.Verify(x => x.GetCurrentDateTime(), Times.Once);
    }

    [TestMethod]
    public void PostDateTime_ShouldUpdateDateTime_WhenValidDateTimeProvided()
    {
        var newDateTime = "2025-09-17T15:45";
        var request = new DateTimeRequest { FechaHora = newDateTime };
        _mockDateTimeBusinessLogic.Setup(x => x.SetCurrentDateTime(newDateTime))
            .Returns(newDateTime);

        var result = _controller.PostDateTime(request);

        Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        _mockDateTimeBusinessLogic.Verify(x => x.SetCurrentDateTime(newDateTime), Times.Once);
    }

    [TestMethod]
    public void ExceptionFilter_ShouldReturn400BadRequest_WhenDateTimeBusinessLogicExceptionOccurs()
    {
        var exception = new DateTimeBusinessLogicException("2025-13-45T99:99");
        SetupExceptionContext(exception);

        _exceptionFilter.OnException(_exceptionContext);

        var response = _exceptionContext.Result as ObjectResult;
        Assert.IsNotNull(response);
        Assert.AreEqual((int)HttpStatusCode.BadRequest, response.StatusCode);
    }

    [TestMethod]
    public void ExceptionFilter_ShouldReturnCorrectTitleAndMessage_WhenDateTimeBusinessLogicExceptionOccurs()
    {
        var exception = new DateTimeBusinessLogicException("Invalid date format");
        SetupExceptionContext(exception);

        _exceptionFilter.OnException(_exceptionContext);

        var response = _exceptionContext.Result as ObjectResult;
        Assert.AreEqual("Error de Validación de Fecha", GetTitle(response!.Value!));
        Assert.AreEqual(exception.Message, GetMessage(response.Value!));
    }

    [TestMethod]
    public void ExceptionFilter_ShouldReturn500InternalServerError_WhenUnknownExceptionOccurs()
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
