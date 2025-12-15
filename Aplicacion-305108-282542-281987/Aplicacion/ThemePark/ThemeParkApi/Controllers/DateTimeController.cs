using Microsoft.AspNetCore.Mvc;
using ThemePark.IBusinessLogic;
using ThemePark.Models.DateTime;
using ThemeParkApi.Filters;

namespace ThemeParkApi.Controllers;

[ApiController]
[Route("datetime")]
public class DateTimeController(IDateTimeBusinessLogic dateTimeBusinessLogic) : ControllerBase
{
    private readonly IDateTimeBusinessLogic _dateTimeBusinessLogic = dateTimeBusinessLogic;

    [HttpGet]
    public IActionResult GetDateTime()
    {
        return Ok(new DateTimeResponse(_dateTimeBusinessLogic.GetCurrentDateTime()));
    }

    [HttpPost]
    [AuthenticationFilter]
    [AuthorizationFilterAttribute("AdministradorParque")]
    public IActionResult PostDateTime([FromBody] DateTimeRequest request)
    {
        return Ok(new DateTimePostResponse(
            _dateTimeBusinessLogic.SetCurrentDateTime(request.FechaHora),
            "Fecha y hora actualizada correctamente"));
    }
}
