using ThemePark.Enums;

namespace ThemePark.Entities;

public class ConfiguracionPorEvento : Configuracion
{
    public override TipoEstrategia? Tipo => TipoEstrategia.PuntuacionPorEvento;

    public int Puntos { get; set; }
    public string Evento { get; set; } = string.Empty;

    public ConfiguracionPorEvento(int puntos, string evento)
    {
        Puntos = puntos;
        Evento = evento;
    }

    public ConfiguracionPorEvento()
    {
    }
}
