using Microsoft.AspNetCore.Mvc;
using ThemePark.IBusinessLogic;
using ThemePark.Models.Users;
using ThemeParkApi.Filters;

namespace ThemeParkApi.Controllers;

[ApiController]
[Route("users")]
public class UserController(IUserService userService) : ControllerBase
{
    private readonly IUserService _userService = userService;

    [HttpPost("register")]
    public IActionResult Register([FromBody] UserRegisterRequest request)
    {
        var response = UserWithAllResponse.FromEntity(_userService.Register(request.ToEntity()));
        return Created(string.Empty, response);
    }

    [HttpPut("{codigoIdentificacion}")]
    [AuthenticationFilter]
    [AuthorizationFilterAttribute]
    public IActionResult UpdateProfile(string codigoIdentificacion, [FromBody] UserProfileUpdateRequest request)
    {
        var response = UserProfileUpdateResponse.FromEntity(
            _userService.UpdateProfile(request.ToEntity(_userService.GetUserByCodigoIdentificacion(codigoIdentificacion).Id)));
        return Ok(response);
    }

    [HttpGet]
    [AuthenticationFilter]
    [AuthorizationFilterAttribute("AdministradorParque")]
    public IActionResult GetAllUsers()
    {
        var response = _userService.GetAllUsers().Select(UserWithAllResponse.FromEntity).ToList();
        return Ok(response);
    }

    [HttpGet("{id}")]
    [AuthenticationFilter]
    [AuthorizationFilterAttribute("AdministradorParque")]
    public IActionResult GetUserById(Guid id)
    {
        var response = UserWithAllResponse.FromEntity(_userService.GetUserById(id));
        return Ok(response);
    }

    [HttpPut("{id}/admin")]
    [AuthenticationFilter]
    [AuthorizationFilterAttribute("AdministradorParque")]
    public IActionResult UpdateUserAdmin(Guid id, [FromBody] UserAdminUpdateRequest request)
    {
        var response = UserWithAllResponse.FromEntity(_userService.UpdateUserPrivileges(request.ToEntity(id)));
        return Ok(response);
    }

    [HttpDelete("{id}")]
    [AuthenticationFilter]
    [AuthorizationFilterAttribute("AdministradorParque")]
    public IActionResult DeleteUser(Guid id)
    {
        _userService.Delete(id);
        return NoContent();
    }

    [HttpGet("points/history")]
    [AuthenticationFilter]
    public IActionResult GetUserPointHistory()
    {
        var response = _userService.GetUserPointHistory().Select(PointHistoryResponseModel.FromDynamic).ToList();
        return Ok(response);
    }

    [HttpGet("points")]
    [AuthenticationFilter]
    public IActionResult GetUserPoints()
    {
        var response = UserPointsResponseModel.FromDynamic(_userService.GetUserPoints());
        return Ok(response);
    }
}
