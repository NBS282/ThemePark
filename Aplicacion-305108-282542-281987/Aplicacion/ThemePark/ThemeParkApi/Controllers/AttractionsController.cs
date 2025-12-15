using Microsoft.AspNetCore.Mvc;
using ThemePark.IBusinessLogic;
using ThemePark.Models.Attractions;
using ThemeParkApi.Filters;

namespace ThemeParkApi.Controllers;

[ApiController]
[Route("attractions")]
public class AttractionsController(IAttractionsBusinessLogic attractionsBusinessLogic) : ControllerBase
{
    private readonly IAttractionsBusinessLogic _attractionsBusinessLogic = attractionsBusinessLogic;

    [HttpPost]
    [AuthenticationFilter]
    [AuthorizationFilter("AdministradorParque")]
    public IActionResult CreateAttraction(CreateAttractionRequest request)
    {
        var response = AttractionResponseModel.FromAttraction(
            _attractionsBusinessLogic.CreateAttraction(request.Nombre, request.Tipo!.Value, request.EdadMinima!.Value, request.CapacidadMaxima!.Value, request.Descripcion, request.Points ?? 0));
        return Created(string.Empty, response);
    }

    [HttpDelete("{nombre}")]
    [AuthenticationFilter]
    [AuthorizationFilter("AdministradorParque")]
    public IActionResult DeleteAttraction(string nombre)
    {
        _attractionsBusinessLogic.DeleteAttraction(nombre);
        return NoContent();
    }

    [HttpPost("{nombre}/access")]
    [AuthenticationFilter]
    [AuthorizationFilter("OperadorAtraccion")]
    public IActionResult ValidateTicketAndAccess(string nombre, AccessAttractionRequest request)
    {
        var (tipoEntrada, codigo) = request.ToEntity();
        return Created(string.Empty, VisitResponseModel.FromVisit(
            _attractionsBusinessLogic.ValidateTicketAndRegisterAccess(nombre, tipoEntrada, codigo)));
    }

    [HttpPost("{nombre}/exit")]
    [AuthenticationFilter]
    [AuthorizationFilter("OperadorAtraccion")]
    public IActionResult RegisterExit(string nombre, AccessAttractionRequest request)
    {
        var (tipoEntrada, codigo) = request.ToEntity();
        var response = VisitResponseModel.FromVisit(
            _attractionsBusinessLogic.RegisterExit(nombre, tipoEntrada, codigo));
        return Ok(response);
    }

    [HttpGet("{nombre}/capacity")]
    [AuthenticationFilter]
    public IActionResult GetCapacity(string nombre)
    {
        var response = CapacityInfoModel.FromAttraction(
            _attractionsBusinessLogic.GetCapacity(nombre));
        return Ok(response);
    }

    [HttpPost("{nombre}/incidents")]
    [AuthenticationFilter]
    [AuthorizationFilter("OperadorAtraccion")]
    public IActionResult CreateIncident(string nombre, CreateIncidentRequest request)
    {
        var incidentId = _attractionsBusinessLogic.CreateIncident(nombre, request.Descripcion);
        var response = IncidentResponseModel.FromIncidentData(
            incidentId, nombre, request.Descripcion, System.DateTime.Now);
        return Created(string.Empty, response);
    }

    [HttpDelete("{nombre}/incidents/{id}")]
    [AuthenticationFilter]
    [AuthorizationFilter("OperadorAtraccion")]
    public IActionResult ResolveIncident(string nombre, string id)
    {
        _attractionsBusinessLogic.ResolveIncident(nombre, id);
        return NoContent();
    }

    [HttpPut("{nombre}")]
    [AuthenticationFilter]
    [AuthorizationFilter("AdministradorParque")]
    public IActionResult UpdateAttraction(string nombre, UpdateAttractionRequest request)
    {
        request.Validate();
        var response = AttractionResponseModel.FromAttraction(
            _attractionsBusinessLogic.UpdateAttraction(nombre, request.Descripcion ?? string.Empty, request.CapacidadMaxima, request.EdadMinima));
        return Ok(response);
    }

    [HttpGet("report")]
    [AuthenticationFilter]
    [AuthorizationFilter("AdministradorParque")]
    public IActionResult GetUsageReport([FromQuery] string fechaInicio, [FromQuery] string fechaFin)
    {
        var request = new UsageReportRequest { FechaInicio = fechaInicio, FechaFin = fechaFin };
        var (parsedFechaInicio, parsedFechaFin) = request.ToDateTimes();
        return Ok(UsageReportModel.FromDictionary(
            _attractionsBusinessLogic.GetUsageReport(parsedFechaInicio, parsedFechaFin)));
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        var attractions = _attractionsBusinessLogic.GetAll();
        var response = attractions.Select(AttractionResponseModel.FromAttraction).ToList();
        return Ok(response);
    }

    [HttpGet("{nombre}")]
    [ExceptionFilter]
    public IActionResult GetById(string nombre)
    {
        var attraction = _attractionsBusinessLogic.GetById(nombre);
        var response = AttractionResponseModel.FromAttraction(attraction);
        return Ok(response);
    }

    [HttpGet("incidents")]
    public IActionResult GetAllIncidents()
    {
        var response = _attractionsBusinessLogic.GetAllIncidents().Select(IncidentResponseModel.FromIncident).ToList();
        return Ok(response);
    }

    [HttpGet("{nombre}/incidents")]
    [ExceptionFilter]
    public IActionResult GetIncidentsByAttraction(string nombre)
    {
        var incidents = _attractionsBusinessLogic.GetIncidentsByAttraction(nombre);
        var response = incidents.Select(IncidentResponseModel.FromIncident).ToList();
        return Ok(response);
    }
}
