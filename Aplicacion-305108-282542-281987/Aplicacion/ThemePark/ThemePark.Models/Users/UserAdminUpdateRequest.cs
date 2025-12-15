using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using ThemePark.Entities;
using ThemePark.Entities.Roles;
using ThemePark.Enums;
using ThemePark.Models.Enums;

namespace ThemePark.Models.Users;

[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
public class UserAdminUpdateRequest
{
    [Required(ErrorMessage = "El campo 'roles' es requerido")]
    public List<RolModel> Roles { get; set; } = [];

    public NivelMembresiaModel? NivelMembresia { get; set; }

    public User ToEntity(Guid userId)
    {
        var roles = new List<Entities.Roles.Rol>();

        foreach(var role in Roles)
        {
            switch(role)
            {
                case RolModel.Visitante:
                    if(NivelMembresia == null)
                    {
                        throw new ThemePark.Exceptions.InvalidRequestDataException("El campo 'nivelMembresia' es requerido cuando se asigna el rol 'Visitante'");
                    }

                    roles.Add(new RolVisitante { NivelMembresia = (NivelMembresia)NivelMembresia });
                    break;
                case RolModel.AdministradorParque:
                    roles.Add(new RolAdministradorParque());
                    break;
                case RolModel.OperadorAtraccion:
                    roles.Add(new RolOperadorAtraccion());
                    break;
            }
        }

        return new User
        {
            Id = userId,
            Roles = roles
        };
    }
}
