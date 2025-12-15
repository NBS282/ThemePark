using ThemePark.Enums;

namespace ThemePark.Entities;

public abstract class Configuracion
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public abstract TipoEstrategia? Tipo { get; }
}
