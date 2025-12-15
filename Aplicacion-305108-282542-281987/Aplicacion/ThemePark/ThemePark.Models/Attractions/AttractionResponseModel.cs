using ThemePark.Entities;

namespace ThemePark.Models.Attractions;

public class AttractionResponseModel
{
    public string Nombre { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public int EdadMinima { get; set; }
    public int CapacidadMaxima { get; set; }
    public string Descripcion { get; set; } = string.Empty;
    public string FechaCreacion { get; set; } = string.Empty;
    public string? FechaModificacion { get; set; }
    public bool TieneIncidencia { get; set; }

    public static AttractionResponseModel FromAttraction(Attraction attraction)
    {
        return new AttractionResponseModel
        {
            Nombre = attraction.Nombre,
            Tipo = attraction.Tipo.ToString(),
            EdadMinima = attraction.EdadMinima,
            CapacidadMaxima = attraction.CapacidadMaxima,
            Descripcion = attraction.Descripcion,
            FechaCreacion = attraction.FechaCreacion.ToString("yyyy-MM-ddTHH:mm:ss"),
            FechaModificacion = attraction.FechaModificacion?.ToString("yyyy-MM-ddTHH:mm:ss"),
            TieneIncidencia = attraction.TieneIncidenciaActiva
        };
    }
}
