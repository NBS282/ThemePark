using Microsoft.AspNetCore.Mvc;
using ThemePark.IBusinessLogic;
using ThemePark.Models.Events;
using ThemeParkApi.Filters;

namespace ThemeParkApi.Controllers;

[ApiController]
[Route("events")]
public class EventsController(IEventsService eventsBusinessLogic) : ControllerBase
{
    private readonly IEventsService _eventsBusinessLogic = eventsBusinessLogic;

    [HttpPost]
    [AuthenticationFilter]
    [AuthorizationFilterAttribute("AdministradorParque")]
    public IActionResult CreateEvent(CreateEventRequest request)
    {
        return Created(string.Empty, EventResponseModel.FromEvent(
            _eventsBusinessLogic.CreateEvent(
                request.Nombre,
                request.Fecha,
                request.Hora,
                request.Aforo,
                request.CostoAdicional,
                request.AtraccionesIncluidas)));
    }

    [HttpPut("{id}")]
    [AuthenticationFilter]
    [AuthorizationFilterAttribute("AdministradorParque")]
    public IActionResult UpdateEvent(string id, CreateEventRequest request)
    {
        return Ok(EventResponseModel.FromEvent(
            _eventsBusinessLogic.UpdateEvent(
                id,
                request.Nombre,
                request.Fecha,
                request.Hora,
                request.Aforo,
                request.CostoAdicional,
                request.AtraccionesIncluidas)));
    }

    [HttpDelete("{id}")]
    [AuthenticationFilter]
    [AuthorizationFilterAttribute("AdministradorParque")]
    public IActionResult DeleteEvent(string id)
    {
        _eventsBusinessLogic.DeleteEvent(id);
        return NoContent();
    }

    [HttpGet]
    [AuthenticationFilter]
    public IActionResult GetAllEvents()
    {
        var response = _eventsBusinessLogic.GetAllEvents()?.Select(EventResponseModel.FromEvent).ToList() ?? [];
        return Ok(response);
    }

    [HttpGet("{id}")]
    [AuthenticationFilter]
    public IActionResult GetEventById(string id)
    {
        var response = EventResponseModel.FromEvent(_eventsBusinessLogic.GetEventById(id));
        return Ok(response);
    }
}
