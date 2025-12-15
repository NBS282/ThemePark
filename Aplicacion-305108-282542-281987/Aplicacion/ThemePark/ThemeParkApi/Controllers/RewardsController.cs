using Microsoft.AspNetCore.Mvc;
using ThemePark.IBusinessLogic;
using ThemePark.Models.Rewards;
using ThemeParkApi.Filters;

namespace ThemeParkApi.Controllers;

[ApiController]
[Route("rewards")]
public class RewardsController(IRewardsBusinessLogic rewardsBusinessLogic) : ControllerBase
{
    private readonly IRewardsBusinessLogic _rewardsBusinessLogic = rewardsBusinessLogic;

    [HttpPost]
    [AuthenticationFilter]
    [AuthorizationFilter("AdministradorParque")]
    public IActionResult CreateReward(CreateRewardRequest request)
    {
        var response = RewardResponseModel.FromReward(
            _rewardsBusinessLogic.CreateReward(request.Nombre, request.Descripcion, request.CostoPuntos!.Value, request.CantidadDisponible!.Value, request.NivelMembresiaRequerido));
        return Created(string.Empty, response);
    }

    [HttpDelete("{id}")]
    [AuthenticationFilter]
    [AuthorizationFilter("AdministradorParque")]
    public IActionResult DeleteReward(int id)
    {
        _rewardsBusinessLogic.DeleteReward(id);
        return NoContent();
    }

    [HttpGet]
    [AuthenticationFilter]
    public IActionResult GetAllRewards()
    {
        var response = _rewardsBusinessLogic.GetAllRewards().Select(RewardResponseModel.FromReward).ToList();
        return Ok(response);
    }

    [HttpGet("{id}")]
    [AuthenticationFilter]
    public IActionResult GetRewardById(int id)
    {
        var response = RewardResponseModel.FromReward(_rewardsBusinessLogic.GetRewardById(id));
        return Ok(response);
    }

    [HttpPut("{id}")]
    [AuthenticationFilter]
    [AuthorizationFilter("AdministradorParque")]
    public IActionResult UpdateReward(int id, UpdateRewardRequest request)
    {
        var response = RewardResponseModel.FromReward(
            _rewardsBusinessLogic.UpdateReward(id, request.Descripcion, request.CostoPuntos, request.CantidadDisponible, request.NivelMembresiaRequerido));
        return Ok(response);
    }

    [HttpGet("user")]
    [AuthenticationFilter]
    public IActionResult GetAvailableRewardsForUser()
    {
        var response = _rewardsBusinessLogic.GetAvailableRewardsForUser().Select(RewardResponseModel.FromReward).ToList();
        return Ok(response);
    }

    [HttpPost("{id}/exchange")]
    [AuthenticationFilter]
    public IActionResult ExchangeReward(int id)
    {
        var response = RewardExchangeResponseModel.FromRewardExchange(_rewardsBusinessLogic.ExchangeReward(id));
        return Created(string.Empty, response);
    }

    [HttpGet("user/exchanges")]
    [AuthenticationFilter]
    public IActionResult GetUserExchanges()
    {
        var response = _rewardsBusinessLogic.GetUserExchanges().Select(RewardExchangeResponseModel.FromRewardExchange).ToList();
        return Ok(response);
    }
}
