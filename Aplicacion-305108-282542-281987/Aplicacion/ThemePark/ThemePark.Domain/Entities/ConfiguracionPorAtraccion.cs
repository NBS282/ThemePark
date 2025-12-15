using ThemePark.Enums;

namespace ThemePark.Entities;

public class ConfiguracionPorAtraccion(Dictionary<string, int> valores) : Configuracion
{
    public override TipoEstrategia? Tipo => TipoEstrategia.PuntuacionPorAtraccion;

    public Dictionary<string, int> Valores { get; set; } = valores ?? [];
}
