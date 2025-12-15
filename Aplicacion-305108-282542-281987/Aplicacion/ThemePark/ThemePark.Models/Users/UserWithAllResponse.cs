using ThemePark.Entities;
using ThemePark.Enums;

namespace ThemePark.Models.Users;

public class UserWithAllResponse
{
    public Guid Id { get; set; }
    public string CodigoIdentificacion { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FechaNacimiento { get; set; } = string.Empty;
    public string NivelMembresia { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = [];
    public string FechaRegistro { get; set; } = string.Empty;

    public static UserWithAllResponse FromEntity(User user)
    {
        var hasVisitanteRole = user.Roles.Any(r => r.TipoRol == Rol.Visitante);
        var nivelMembresia = hasVisitanteRole ? user.GetMembresia().ToString() : string.Empty;

        return new UserWithAllResponse
        {
            Id = user.Id,
            CodigoIdentificacion = user.CodigoIdentificacion,
            Nombre = user.Nombre,
            Apellido = user.Apellido,
            Email = user.Email,
            FechaNacimiento = user.FechaNacimiento.ToString("yyyy-MM-dd"),
            NivelMembresia = nivelMembresia,
            Roles = user.Roles.Select(r => r.TipoRol.ToString()).ToList(),
            FechaRegistro = user.FechaRegistro.ToString("yyyy-MM-dd")
        };
    }
}
