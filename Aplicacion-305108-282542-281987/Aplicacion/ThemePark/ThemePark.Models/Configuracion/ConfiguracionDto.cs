using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using ThemePark.Entities;
using ThemePark.Models.Enums;

namespace ThemePark.Models.Configuracion;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "tipo")]
[JsonDerivedType(typeof(ConfiguracionPorAtraccionDto), typeDiscriminator: "PuntuacionPorAtraccion")]
[JsonDerivedType(typeof(ConfiguracionPorEventoDto), typeDiscriminator: "PuntuacionPorEvento")]
[JsonDerivedType(typeof(ConfiguracionPorComboDto), typeDiscriminator: "PuntuacionCombo")]
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
public abstract class ConfiguracionDto
{
    [Required(ErrorMessage = "El campo 'tipo' es requerido")]
    public abstract TipoAlgoritmoDto Tipo { get; }

    public abstract Entities.Configuracion ToEntity();

    public static ConfiguracionDto FromEntity(Entities.Configuracion configuracion)
    {
        return configuracion switch
        {
            ConfiguracionPorAtraccion porAtraccion => ConfiguracionPorAtraccionDto.FromEntity(porAtraccion),
            ConfiguracionPorEvento porEvento => ConfiguracionPorEventoDto.FromEntity(porEvento),
            ConfiguracionPorCombo porCombo => ConfiguracionPorComboDto.FromEntity(porCombo),
            _ => throw new ThemePark.Exceptions.UnsupportedConfigurationTypeException(configuracion.GetType().Name)
        };
    }
}
