using Microsoft.AspNetCore.Mvc;
using ThemePark.IBusinessLogic;
using ThemePark.Models.Scoring;
using ThemeParkApi.Filters;

namespace ThemeParkApi.Controllers;

[ApiController]
[Route("scoring")]
public class ScoringController(IRankingService rankingService) : ControllerBase
{
    private readonly IRankingService _rankingService = rankingService;

    [HttpGet("ranking")]
    [AuthenticationFilter]
    [AuthorizationFilterAttribute("AdministradorParque")]

    public IActionResult GetRanking()
    {
        var response = _rankingService.GetDailyRanking().Select(RankingResponseModel.FromEntity).ToList();
        return Ok(response);
    }
}
