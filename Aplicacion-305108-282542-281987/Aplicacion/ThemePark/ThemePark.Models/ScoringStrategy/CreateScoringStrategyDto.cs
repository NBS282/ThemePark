using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using ThemePark.Enums;
using ThemePark.Models.Configuracion;
using ThemePark.Models.Enums;

namespace ThemePark.Models.ScoringStrategy;

[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
public class CreateScoringStrategyDto
{
    [Required(ErrorMessage = "El campo 'nombre' es requerido")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "El campo 'descripcion' es requerido")]
    public string Descripcion { get; set; } = string.Empty;

    public TipoAlgoritmoDto? Algoritmo { get; set; }
    public ConfiguracionDto? Configuracion { get; set; }

    public string? PluginTypeIdentifier { get; set; }
    public string? ConfigurationJson { get; set; }

    public Entities.ScoringStrategy ToEntity()
    {
        if(Algoritmo.HasValue && !string.IsNullOrEmpty(PluginTypeIdentifier))
        {
            throw new ThemePark.Exceptions.InvalidRequestDataException(
                "Debe especificar 'algoritmo' O 'pluginTypeIdentifier', no ambos");
        }

        if(!Algoritmo.HasValue && string.IsNullOrEmpty(PluginTypeIdentifier))
        {
            throw new ThemePark.Exceptions.InvalidRequestDataException(
                "Debe especificar 'algoritmo' o 'pluginTypeIdentifier'");
        }

        if(Algoritmo.HasValue)
        {
            if(Configuracion == null)
            {
                throw new ThemePark.Exceptions.InvalidRequestDataException(
                    "'configuracion' es requerido para estrategias nativas");
            }

            if(!Enum.IsDefined(typeof(TipoAlgoritmoDto), Algoritmo.Value))
            {
                throw new ThemePark.Exceptions.InvalidRequestDataException("El campo 'algoritmo' debe ser un valor válido (0: PuntuacionPorAtraccion, 1: PuntuacionCombo, 2: PuntuacionPorEvento)");
            }

            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(Configuracion);

            if(!Validator.TryValidateObject(Configuracion, validationContext, validationResults, validateAllProperties: true))
            {
                var errors = string.Join(", ", validationResults.Select(r => r.ErrorMessage));
                throw new ThemePark.Exceptions.InvalidRequestDataException(errors);
            }

            var type = MapToEntity(Algoritmo.Value);
            var configuration = Configuracion.ToEntity();
            return new Entities.ScoringStrategy(Nombre, type, Descripcion, configuration);
        }

        throw new ThemePark.Exceptions.InvalidRequestDataException(
            "Para crear estrategias de plugin, use el método ToEntityWithPlugin del Service");
    }

    public static TipoEstrategia? MapToEntity(TipoAlgoritmoDto? dtoType)
    {
        return dtoType switch
        {
            TipoAlgoritmoDto.PuntuacionPorAtraccion => TipoEstrategia.PuntuacionPorAtraccion,
            TipoAlgoritmoDto.PuntuacionCombo => TipoEstrategia.PuntuacionCombo,
            TipoAlgoritmoDto.PuntuacionPorEvento => TipoEstrategia.PuntuacionPorEvento,
            _ => null
        };
    }
}
