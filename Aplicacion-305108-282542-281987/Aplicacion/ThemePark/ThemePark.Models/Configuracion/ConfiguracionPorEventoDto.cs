using System.ComponentModel.DataAnnotations;
using ThemePark.Models.Enums;

namespace ThemePark.Models.Configuracion;

public class ConfiguracionPorEventoDto : ConfiguracionDto
{
    public override TipoAlgoritmoDto Tipo => TipoAlgoritmoDto.PuntuacionPorEvento;

    [Required(ErrorMessage = "El campo 'puntos' es requerido")]
    public int? Puntos { get; set; }

    [Required(ErrorMessage = "El campo 'evento' es requerido")]
    public string Evento { get; set; } = string.Empty;

    public ConfiguracionPorEventoDto()
    {
    }

    public ConfiguracionPorEventoDto(int puntos, string evento)
    {
        Puntos = puntos;
        Evento = evento;
    }

    public override Entities.Configuracion ToEntity()
    {
        return new Entities.ConfiguracionPorEvento(Puntos!.Value, Evento);
    }

    public static ConfiguracionPorEventoDto FromEntity(Entities.ConfiguracionPorEvento entity)
    {
        return new ConfiguracionPorEventoDto(entity.Puntos, entity.Evento);
    }
}
