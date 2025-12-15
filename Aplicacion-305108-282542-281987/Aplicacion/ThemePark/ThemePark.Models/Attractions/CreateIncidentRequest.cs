using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ThemePark.Models.Attractions;

[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
public class CreateIncidentRequest
{
    [Required(ErrorMessage = "El campo 'descripcion' es requerido")]
    public string Descripcion { get; set; } = string.Empty;

    [Required(ErrorMessage = "El campo 'prioridad' es requerido")]
    public string Prioridad { get; set; } = string.Empty;
}
