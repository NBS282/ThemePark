using ThemePark.Entities;
using ThemePark.Enums;

namespace ThemePark.Models.Users;

public class UserProfileUpdateResponse
{
    public Guid Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FechaNacimiento { get; set; } = string.Empty;
    public string NivelMembresia { get; set; } = string.Empty;
    public string FechaRegistro { get; set; } = string.Empty;

    public static UserProfileUpdateResponse FromEntity(User user)
    {
        var hasVisitanteRole = user.Roles.Any(r => r.TipoRol == Rol.Visitante);
        var nivelMembresia = hasVisitanteRole ? user.GetMembresia().ToString() : string.Empty;

        return new UserProfileUpdateResponse
        {
            Id = user.Id,
            Nombre = user.Nombre,
            Apellido = user.Apellido,
            Email = user.Email,
            FechaNacimiento = user.FechaNacimiento.ToString("yyyy-MM-dd"),
            NivelMembresia = nivelMembresia,
            FechaRegistro = user.FechaRegistro.ToString("yyyy-MM-dd")
        };
    }
}
