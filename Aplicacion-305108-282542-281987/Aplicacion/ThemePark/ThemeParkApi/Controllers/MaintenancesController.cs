using Microsoft.AspNetCore.Mvc;
using ThemePark.IBusinessLogic;
using ThemePark.Models.Attractions;
using ThemeParkApi.Filters;

namespace ThemeParkApi.Controllers;

[ApiController]
[ExceptionFilter]
[Route("attractions/{attractionName}/maintenances")]
public class MaintenancesController(IAttractionsBusinessLogic attractionsBusinessLogic) : ControllerBase
{
    private readonly IAttractionsBusinessLogic _attractionsBusinessLogic = attractionsBusinessLogic;

    [HttpPost]
    [AuthenticationFilter]
    [AuthorizationFilter("OperadorAtraccion")]
    public IActionResult CreateMaintenance(string attractionName, CreateMaintenanceRequest request)
    {
        var (maintenanceId, incidentId) = _attractionsBusinessLogic.CreatePreventiveMaintenance(request.ToEntity(attractionName));

        var (fecha, horaInicio) = request.ParseDates();
        var response = MaintenanceResponseModel.FromMaintenanceData(
            maintenanceId,
            attractionName,
            fecha,
            horaInicio,
            request.DuracionMinutos!.Value,
            request.Descripcion,
            "Programado",
            incidentId);

        return Created(string.Empty, response);
    }

    [HttpGet]
    [AuthenticationFilter]
    [AuthorizationFilter("OperadorAtraccion")]
    public IActionResult GetMaintenances(string attractionName)
    {
        var response = MaintenanceResponseModel.FromEntities(_attractionsBusinessLogic.GetMaintenancesByAttraction(attractionName));
        return Ok(response);
    }

    [HttpDelete("{maintenanceId}")]
    [AuthenticationFilter]
    [AuthorizationFilter("OperadorAtraccion")]
    public IActionResult DeleteMaintenance(string attractionName, string maintenanceId)
    {
        _attractionsBusinessLogic.CancelPreventiveMaintenance(attractionName, maintenanceId);
        return NoContent();
    }
}
