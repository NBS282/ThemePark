using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ThemePark.Models.Users;

[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
public class UserLoginRequest
{
    [Required(ErrorMessage = "El campo 'email' es requerido")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "El campo 'password' es requerido")]
    public string Contraseña { get; set; } = string.Empty;
}
