using System.ComponentModel.DataAnnotations.Schema;
using ThemePark.Enums;
using ThemePark.Exceptions;

namespace ThemePark.Entities;

public class ScoringStrategy
{
    public Guid Id { get; set; } = Guid.Empty;

    private string _name = string.Empty;

    public string Name
    {
        get => _name;
        set
        {
            if(value == string.Empty)
            {
                throw new InvalidUserDataException("Name");
            }

            _name = value;
        }
    }

    private string _description = string.Empty;

    public string Description
    {
        get => _description;
        set
        {
            if(value == string.Empty)
            {
                throw new InvalidUserDataException("Description");
            }

            _description = value;
        }
    }

    private Configuracion? _configuracionTyped = null;

    public Configuracion? ConfiguracionTyped
    {
        get => _configuracionTyped;
        set
        {
            _configuracionTyped = value;
        }
    }

    public bool Active { get; set; } = false;

    public TipoEstrategia? Type { get; set; } = TipoEstrategia.PuntuacionPorAtraccion;

    public string? PluginTypeIdentifier { get; set; } = null;

    public string ConfigurationJson { get; set; } = string.Empty;

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    [NotMapped]
    public bool IsPluginAvailable { get; set; } = true;

    public ScoringStrategy()
    {
    }

    public ScoringStrategy(string name, TipoEstrategia? type, string description, Configuracion configuracionTyped)
    {
        Name = name;
        Type = type;
        Description = description;
        ConfiguracionTyped = configuracionTyped;
        PluginTypeIdentifier = null;
        ConfigurationJson = System.Text.Json.JsonSerializer.Serialize(configuracionTyped);
    }

    public ScoringStrategy(string name, string pluginTypeIdentifier, string description, Configuracion configuracionTyped, string configurationJson)
    {
        Name = name;
        Type = null;
        PluginTypeIdentifier = pluginTypeIdentifier;
        Description = description;
        ConfiguracionTyped = configuracionTyped;
        ConfigurationJson = configurationJson;
    }
}
