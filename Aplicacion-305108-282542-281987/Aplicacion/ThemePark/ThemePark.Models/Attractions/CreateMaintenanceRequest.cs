using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using ThemePark.Entities;
using ThemePark.Exceptions;

namespace ThemePark.Models.Attractions;

[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
public class CreateMaintenanceRequest
{
    [Required(ErrorMessage = "El campo 'fecha' es requerido. Formato esperado: yyyy-MM-dd (ejemplo: 2025-12-31)")]
    public string Fecha { get; set; } = string.Empty;

    [Required(ErrorMessage = "El campo 'horaInicio' es requerido. Formato esperado: HH:mm (ejemplo: 14:30)")]
    public string HoraInicio { get; set; } = string.Empty;

    [Required(ErrorMessage = "El campo 'duracionMinutos' es requerido")]
    public int? DuracionMinutos { get; set; }

    [Required(ErrorMessage = "El campo 'descripcion' es requerido")]
    public string Descripcion { get; set; } = string.Empty;

    public (System.DateTime Fecha, System.TimeSpan HoraInicio) ParseDates()
    {
        try
        {
            var fecha = System.DateTime.Parse(Fecha);
            var horaInicio = System.TimeSpan.ParseExact(HoraInicio, @"hh\:mm", System.Globalization.CultureInfo.InvariantCulture);
            return (fecha, horaInicio);
        }
        catch(FormatException)
        {
            throw new InvalidRequestDataException("Formato de fecha u hora inválido. Use formato 'yyyy-MM-dd' para fecha (ejemplo: 2025-12-31) y 'HH:mm' para hora (ejemplo: 14:30)");
        }
    }

    public Maintenance ToEntity(string attractionName)
    {
        var (fecha, horaInicio) = ParseDates();
        return new Maintenance(attractionName, fecha, horaInicio, DuracionMinutos!.Value, Descripcion);
    }
}
