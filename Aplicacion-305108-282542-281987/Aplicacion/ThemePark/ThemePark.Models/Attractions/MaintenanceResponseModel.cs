using ThemePark.Entities;

namespace ThemePark.Models.Attractions;

public class MaintenanceResponseModel
{
    public string Id { get; set; } = string.Empty;
    public string Atraccion { get; set; } = string.Empty;
    public string Fecha { get; set; } = string.Empty;
    public string HoraInicio { get; set; } = string.Empty;
    public int DuracionMinutos { get; set; }
    public string Descripcion { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public string? IncidentId { get; set; }

    public static MaintenanceResponseModel FromMaintenanceData(
        string maintenanceId,
        string atraccionNombre,
        System.DateTime fecha,
        System.TimeSpan horaInicio,
        int duracionMinutos,
        string descripcion,
        string estado,
        string? incidentId = null)
    {
        return new MaintenanceResponseModel
        {
            Id = maintenanceId,
            Atraccion = atraccionNombre,
            Fecha = fecha.ToString("yyyy-MM-dd"),
            HoraInicio = horaInicio.ToString(@"hh\:mm"),
            DuracionMinutos = duracionMinutos,
            Descripcion = descripcion,
            Estado = estado,
            IncidentId = incidentId
        };
    }

    public static MaintenanceResponseModel FromEntity(Maintenance maintenance)
    {
        return new MaintenanceResponseModel
        {
            Id = maintenance.Id.ToString(),
            Atraccion = maintenance.AttractionName,
            Fecha = maintenance.Fecha.ToString("yyyy-MM-dd"),
            HoraInicio = maintenance.HoraInicio.ToString(@"hh\:mm"),
            DuracionMinutos = maintenance.DuracionMinutos,
            Descripcion = maintenance.Descripcion,
            Estado = "Programado",
            IncidentId = maintenance.IncidentId.ToString()
        };
    }

    public static List<MaintenanceResponseModel> FromEntities(List<Maintenance> maintenances)
    {
        return maintenances.Select(FromEntity).ToList();
    }
}
