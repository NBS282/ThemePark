using ThemePark.Enums;
using ThemePark.Models.Configuracion;
using ThemePark.Models.Enums;

namespace ThemePark.Models.ScoringStrategy;

public class ScoringStrategyDto
{
    public Guid Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public bool Activa { get; set; }
    public TipoAlgoritmoDto? Algoritmo { get; set; }
    public ConfiguracionDto? Configuracion { get; set; }
    public bool EsPlugin { get; set; }
    public string? PluginTypeIdentifier { get; set; }
    public string? ConfigurationJson { get; set; }
    public bool IsPluginAvailable { get; set; } = true;

    public static ScoringStrategyDto FromEntity(Entities.ScoringStrategy entity)
    {
        var esPlugin = !string.IsNullOrEmpty(entity.PluginTypeIdentifier);

        return new ScoringStrategyDto
        {
            Id = entity.Id,
            Nombre = entity.Name,
            Descripcion = entity.Description,
            Activa = entity.Active,
            EsPlugin = esPlugin,
            PluginTypeIdentifier = entity.PluginTypeIdentifier,
            ConfigurationJson = esPlugin ? entity.ConfigurationJson : null,
            Algoritmo = esPlugin ? null : MapToDto(entity.Type),
            Configuracion = esPlugin ? null : (entity.ConfiguracionTyped != null ? ConfiguracionDto.FromEntity(entity.ConfiguracionTyped) : null),
            IsPluginAvailable = entity.IsPluginAvailable
        };
    }

    public static List<ScoringStrategyDto> FromEntities(List<Entities.ScoringStrategy> entities)
    {
        return entities.Select(FromEntity).ToList();
    }

    public Entities.ScoringStrategy ToEntity()
    {
        if(Algoritmo == null || Configuracion == null)
        {
            throw new ArgumentException("Algoritmo y Configuracion son requeridos para estrategias no-plugin");
        }

        var type = MapToEntity(Algoritmo.Value);
        var configuration = Configuracion.ToEntity();
        var entity = new Entities.ScoringStrategy(Nombre, type, Descripcion, configuration)
        {
            Id = Id,
            Active = Activa
        };
        return entity;
    }

    private static TipoAlgoritmoDto MapToDto(TipoEstrategia? entityType)
    {
        return entityType switch
        {
            TipoEstrategia.PuntuacionPorAtraccion => TipoAlgoritmoDto.PuntuacionPorAtraccion,
            TipoEstrategia.PuntuacionCombo => TipoAlgoritmoDto.PuntuacionCombo,
            TipoEstrategia.PuntuacionPorEvento => TipoAlgoritmoDto.PuntuacionPorEvento,
            _ => TipoAlgoritmoDto.PuntuacionPorAtraccion
        };
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
