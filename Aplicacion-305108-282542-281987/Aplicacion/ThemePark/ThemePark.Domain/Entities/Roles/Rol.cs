namespace ThemePark.Entities.Roles;

public abstract class Rol
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public abstract Enums.Rol TipoRol { get; }
}
