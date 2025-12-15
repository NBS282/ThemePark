using System.Text.Json.Serialization;
using ThemePark.Exceptions;

namespace ThemePark.Models.Attractions;

[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
public class UpdateAttractionRequest
{
    public string? Descripcion { get; set; }
    public int? CapacidadMaxima { get; set; }
    public int? EdadMinima { get; set; }

    public void Validate()
    {
        if(Descripcion == null && CapacidadMaxima == null && EdadMinima == null)
        {
            throw new InvalidRequestDataException("Debe proporcionar al menos un campo para actualizar: 'descripcion', 'capacidadMaxima' o 'edadMinima'");
        }
    }
}
