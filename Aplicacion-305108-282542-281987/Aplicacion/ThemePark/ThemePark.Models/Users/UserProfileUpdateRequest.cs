using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using ThemePark.Entities;
using static ThemePark.DateParser;

namespace ThemePark.Models.Users;

[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
public class UserProfileUpdateRequest
{
    [Required(ErrorMessage = "El campo 'nombre' es requerido")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "El campo 'apellido' es requerido")]
    public string Apellido { get; set; } = string.Empty;

    [Required(ErrorMessage = "El campo 'email' es requerido")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "El campo 'contraseña' es requerido")]
    public string Contraseña { get; set; } = string.Empty;

    [Required(ErrorMessage = "El campo 'fechaNacimiento' es requerido")]
    public string FechaNacimiento { get; set; } = string.Empty;

    public User ToEntity(Guid userId)
    {
        return new User
        {
            Id = userId,
            Nombre = Nombre,
            Apellido = Apellido,
            Email = Email,
            Contraseña = Contraseña,
            FechaNacimiento = ParseDate(FechaNacimiento, "fechaNacimiento")
        };
    }
}
