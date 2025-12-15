using System.Text.Json.Serialization;
using ThemePark.Entities;
using ThemePark.Models.Attractions;

namespace ThemePark.Models.Events;

public class EventResponseModel
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Nombre { get; set; } = string.Empty;

    [JsonPropertyName("fecha")]
    public string Fecha { get; set; } = string.Empty;

    [JsonPropertyName("hora")]
    public string Hora { get; set; } = string.Empty;

    [JsonPropertyName("aforo")]
    public int Aforo { get; set; }

    [JsonPropertyName("costoAdicional")]
    public decimal CostoAdicional { get; set; }

    [JsonPropertyName("atracciones")]
    public List<AttractionResponseModel> AtraccionesIncluidas { get; set; } = [];

    public static EventResponseModel FromEvent(Event eventEntity)
    {
        return new EventResponseModel
        {
            Id = eventEntity.Id.ToString(),
            Nombre = eventEntity.Name,
            Fecha = eventEntity.Fecha.ToString("yyyy-MM-dd"),
            Hora = eventEntity.Hora.ToString(@"hh\:mm"),
            Aforo = eventEntity.Aforo,
            CostoAdicional = eventEntity.CostoAdicional,
            AtraccionesIncluidas = eventEntity.Atracciones?.Select(AttractionResponseModel.FromAttraction).ToList() ?? []
        };
    }
}
