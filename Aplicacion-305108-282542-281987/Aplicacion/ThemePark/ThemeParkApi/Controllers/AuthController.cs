using Microsoft.AspNetCore.Mvc;
using ThemePark.IBusinessLogic;
using ThemePark.Models.Users;

namespace ThemeParkApi.Controllers;

[ApiController]
[Route("auth")]
public class AuthController(IAuthService authService) : ControllerBase
{
    private readonly IAuthService _authService = authService;

    [HttpPost("login")]
    public IActionResult Login([FromBody] UserLoginRequest request)
    {
        return Ok(UserLoginResponse.FromEntity(_authService.Login(request.Email, request.Contraseña)));
    }
}
