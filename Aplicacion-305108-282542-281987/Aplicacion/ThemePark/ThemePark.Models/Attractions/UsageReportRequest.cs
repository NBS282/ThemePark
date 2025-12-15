using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using static ThemePark.DateParser;

namespace ThemePark.Models.Attractions;

[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
public class UsageReportRequest
{
    [Required(ErrorMessage = "El campo 'fechaInicio' es requerido")]
    public string FechaInicio { get; set; } = string.Empty;

    [Required(ErrorMessage = "El campo 'fechaFin' es requerido")]
    public string FechaFin { get; set; } = string.Empty;

    public (System.DateTime FechaInicio, System.DateTime FechaFin) ToDateTimes()
    {
        var fechaInicio = ParseDate(FechaInicio, "fechaInicio");
        var fechaFin = ParseDate(FechaFin, "fechaFin");
        return (fechaInicio, fechaFin);
    }
}
