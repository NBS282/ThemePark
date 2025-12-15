using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ThemePark.Models.Attractions;

[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
public class CreateAttractionRequest
{
    [Required(ErrorMessage = "El campo 'nombre' es requerido")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "El campo 'tipo' es requerido")]
    public int? Tipo { get; set; }

    [Required(ErrorMessage = "El campo 'edadMinima' es requerido")]
    public int? EdadMinima { get; set; }

    [Required(ErrorMessage = "El campo 'capacidadMaxima' es requerido")]
    public int? CapacidadMaxima { get; set; }

    [Required(ErrorMessage = "El campo 'descripcion' es requerido")]
    public string Descripcion { get; set; } = string.Empty;

    public int? Points { get; set; }
}
