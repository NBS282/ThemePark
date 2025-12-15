using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using ThemePark.Exceptions;
using static ThemePark.DateParser;

namespace ThemePark.Models.Events;

[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
public class CreateEventRequest
{
    [Required(ErrorMessage = "El campo 'nombre' es requerido")]
    public string Nombre { get; set; } = string.Empty;

    private string _fechaString = string.Empty;

    [Required(ErrorMessage = "El campo 'fecha' es requerido")]
    [JsonPropertyName("fecha")]
    public string FechaString
    {
        get => _fechaString;
        set
        {
            _fechaString = value;
            if(!string.IsNullOrEmpty(value))
            {
                Fecha = ParseDate(value, "fecha");
            }
        }
    }

    [JsonIgnore]
    public System.DateTime Fecha { get; set; }

    private string _horaString = string.Empty;

    [Required(ErrorMessage = "El campo 'hora' es requerido")]
    [JsonPropertyName("hora")]
    public string HoraString
    {
        get => _horaString;
        set
        {
            _horaString = value;
            if(!string.IsNullOrEmpty(value))
            {
                if(!System.TimeSpan.TryParse(value, out var timeSpan))
                {
                    throw new InvalidTimeFormatException(value);
                }

                Hora = timeSpan;
            }
        }
    }

    [JsonIgnore]
    public System.TimeSpan Hora { get; set; }

    [Required(ErrorMessage = "El campo 'aforo' es requerido")]
    public int Aforo { get; set; }

    [Required(ErrorMessage = "El campo 'costoAdicional' es requerido")]
    public decimal CostoAdicional { get; set; }

    [Required(ErrorMessage = "El campo 'atraccionesIncluidas' es requerido")]
    public List<string> AtraccionesIncluidas { get; set; } = [];
}
