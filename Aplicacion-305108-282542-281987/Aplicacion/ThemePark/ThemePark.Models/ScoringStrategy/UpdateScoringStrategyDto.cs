using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using ThemePark.Enums;
using ThemePark.Models.Configuracion;
using ThemePark.Models.Enums;

namespace ThemePark.Models.ScoringStrategy;

[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
public class UpdateScoringStrategyDto
{
    [Required(ErrorMessage = "El campo 'descripcion' es requerido")]
    public string Descripcion { get; set; } = string.Empty;

    public TipoAlgoritmoDto? Algoritmo { get; set; }
    public ConfiguracionDto? Configuracion { get; set; }

    public string? ConfigurationJson { get; set; }

    public Entities.ScoringStrategy ToEntity()
    {
        var entity = new Entities.ScoringStrategy
        {
            Description = Descripcion
        };

        if(Algoritmo.HasValue && Configuracion != null)
        {
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

            entity.Type = MapToEntity(Algoritmo.Value);
            entity.ConfiguracionTyped = Configuracion.ToEntity();
        }

        if(!string.IsNullOrEmpty(ConfigurationJson))
        {
            entity.ConfigurationJson = ConfigurationJson;
        }

        return entity;
    }

    private static TipoEstrategia MapToEntity(TipoAlgoritmoDto dtoType)
    {
        return dtoType switch
        {
            TipoAlgoritmoDto.PuntuacionPorAtraccion => TipoEstrategia.PuntuacionPorAtraccion,
            TipoAlgoritmoDto.PuntuacionCombo => TipoEstrategia.PuntuacionCombo,
            TipoAlgoritmoDto.PuntuacionPorEvento => TipoEstrategia.PuntuacionPorEvento,
            _ => TipoEstrategia.PuntuacionPorAtraccion
        };
    }
}
