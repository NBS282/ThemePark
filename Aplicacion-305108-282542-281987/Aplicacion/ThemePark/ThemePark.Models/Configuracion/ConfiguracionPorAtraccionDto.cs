using System.ComponentModel.DataAnnotations;
using ThemePark.Models.Enums;

namespace ThemePark.Models.Configuracion;

public class ConfiguracionPorAtraccionDto : ConfiguracionDto
{
    [Required(ErrorMessage = "El campo 'tipo' es requerido")]
    public override TipoAlgoritmoDto Tipo => TipoAlgoritmoDto.PuntuacionPorAtraccion;

    [Required(ErrorMessage = "El campo 'valores' es requerido")]
    public Dictionary<string, int> Valores { get; set; } = [];

    public ConfiguracionPorAtraccionDto()
    {
    }

    public ConfiguracionPorAtraccionDto(Dictionary<string, int> valores)
    {
        Valores = valores;
    }

    public override Entities.Configuracion ToEntity()
    {
        var tiposRequeridos = new[] { "MontañaRusa", "Simulador", "Espectaculo", "ZonaInteractiva" };
        var tiposFaltantes = tiposRequeridos.Where(t => !Valores.ContainsKey(t)).ToList();

        if(tiposFaltantes.Any())
        {
            var mensaje = $"Faltan los siguientes tipos de atracción en 'valores': {string.Join(", ", tiposFaltantes)}";
            throw new ThemePark.Exceptions.InvalidRequestDataException(mensaje);
        }

        return new Entities.ConfiguracionPorAtraccion(Valores);
    }

    public static ConfiguracionPorAtraccionDto FromEntity(Entities.ConfiguracionPorAtraccion entity)
    {
        return new ConfiguracionPorAtraccionDto(entity.Valores);
    }
}
