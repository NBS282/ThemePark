using ThemePark.Entities;

namespace ThemePark.Models.Attractions;

public class IncidentResponseModel
{
    public string Id { get; set; } = string.Empty;
    public string Atraccion { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public string FechaCreacion { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public string? MaintenanceId { get; set; }
    public string? FechaProgramada { get; set; }
    public string? HoraProgramada { get; set; }

    public static IncidentResponseModel FromIncidentData(string incidentId, string atraccionNombre, string descripcion, System.DateTime fechaCreacion)
    {
        return new IncidentResponseModel
        {
            Id = incidentId,
            Atraccion = atraccionNombre,
            Descripcion = descripcion,
            FechaCreacion = fechaCreacion.ToString("yyyy-MM-ddTHH:mm:ss"),
            Estado = "Activa"
        };
    }

    public static IncidentResponseModel FromIncident(Incident incident)
    {
        return new IncidentResponseModel
        {
            Id = incident.Id.ToString(),
            Atraccion = incident.AttractionName,
            Descripcion = incident.Descripcion,
            FechaCreacion = incident.FechaCreacion.ToString("yyyy-MM-ddTHH:mm:ss"),
            Estado = incident.IsActive ? "Activa" : "Resuelta",
            MaintenanceId = incident.MaintenanceId?.ToString(),
            FechaProgramada = incident.FechaProgramada?.ToString("yyyy-MM-ddTHH:mm:ss"),
            HoraProgramada = incident.HoraProgramada?.ToString(@"hh\:mm")
        };
    }
}
