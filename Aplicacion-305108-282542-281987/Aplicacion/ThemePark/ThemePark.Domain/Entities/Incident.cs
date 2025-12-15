using ThemePark.Exceptions;

namespace ThemePark.Entities;

public class Incident
{
    public Guid Id { get; private set; }
    public string AttractionName { get; private set; }
    public string Descripcion { get; private set; }
    public DateTime FechaCreacion { get; private set; }
    public DateTime? FechaResolucion { get; private set; }
    public bool IsActive { get; private set; }
    public Guid? MaintenanceId { get; private set; }
    public DateTime? FechaProgramada { get; private set; }
    public TimeSpan? HoraProgramada { get; private set; }

    public Incident(string attractionName, string descripcion, DateTime fechaCreacion)
    {
        if(string.IsNullOrWhiteSpace(attractionName))
        {
            throw new InvalidIncidentDataException(nameof(attractionName));
        }

        if(string.IsNullOrWhiteSpace(descripcion))
        {
            throw new InvalidIncidentDataException(nameof(descripcion));
        }

        Id = Guid.NewGuid();
        AttractionName = attractionName;
        Descripcion = descripcion;
        FechaCreacion = fechaCreacion;
        FechaResolucion = null;
        IsActive = true;
        MaintenanceId = null;
        FechaProgramada = null;
        HoraProgramada = null;
    }

    public Incident(string attractionName, string descripcion, DateTime fechaCreacion, Guid maintenanceId, DateTime fechaProgramada, TimeSpan horaProgramada)
    {
        if(string.IsNullOrWhiteSpace(attractionName))
        {
            throw new InvalidIncidentDataException(nameof(attractionName));
        }

        if(string.IsNullOrWhiteSpace(descripcion))
        {
            throw new InvalidIncidentDataException(nameof(descripcion));
        }

        Id = Guid.NewGuid();
        AttractionName = attractionName;
        Descripcion = descripcion;
        FechaCreacion = fechaCreacion;
        FechaResolucion = null;
        IsActive = true;
        MaintenanceId = maintenanceId;
        FechaProgramada = fechaProgramada;
        HoraProgramada = horaProgramada;
    }

    public void Resolve(DateTime fechaResolucion)
    {
        IsActive = false;
        FechaResolucion = fechaResolucion;
    }

    public bool IsMaintenanceIncident()
    {
        return MaintenanceId.HasValue;
    }
}
