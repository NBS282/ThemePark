using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ThemePark.Models.Attractions;

[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
public class AccessAttractionRequest
{
    [Required(ErrorMessage = "El campo 'tipoEntrada' es requerido")]
    public string TipoEntrada { get; set; } = string.Empty;

    [Required(ErrorMessage = "El campo 'codigo' es requerido")]
    public string Codigo { get; set; } = string.Empty;

    public (string TipoEntrada, string Codigo) ToEntity()
    {
        return (TipoEntrada, Codigo);
    }
}
