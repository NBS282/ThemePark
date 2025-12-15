using ThemePark.Enums;

namespace ThemePark.Entities.Roles;

public class RolVisitante : Rol
{
    public override Enums.Rol TipoRol => Enums.Rol.Visitante;
    public NivelMembresia NivelMembresia { get; set; } = NivelMembresia.Estándar;
}
