using Microsoft.AspNetCore.Mvc;
using ThemePark.IBusinessLogic;
using ThemePark.Models.Tickets;
using ThemeParkApi.Filters;

namespace ThemeParkApi.Controllers;

[ApiController]
[Route("tickets")]
public class TicketsController(ITicketsBusinessLogic ticketsBusinessLogic) : ControllerBase
{
    private readonly ITicketsBusinessLogic _ticketsBusinessLogic = ticketsBusinessLogic;

    [HttpPost]
    [AuthenticationFilter]
    public IActionResult CreateTicket([FromBody] CreateTicketRequest request)
    {
        var response = TicketResponseModel.FromEntity(_ticketsBusinessLogic.CreateTicket(request.ToEntity()));
        return Created(string.Empty, response);
    }

    [HttpGet("")]
    [AuthenticationFilter]
    public IActionResult GetMyTickets()
    {
        var response = _ticketsBusinessLogic.GetMyTickets().Select(TicketResponseModel.FromEntity).ToList();
        return Ok(response);
    }
}
