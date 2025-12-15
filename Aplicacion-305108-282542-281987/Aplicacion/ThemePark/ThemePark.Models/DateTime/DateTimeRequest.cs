using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ThemePark.Models.DateTime;

[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
public class DateTimeRequest
{
    [Required(ErrorMessage = "El campo 'fechaHora' es requerido")]
    public string FechaHora { get; set; } = string.Empty;
}
