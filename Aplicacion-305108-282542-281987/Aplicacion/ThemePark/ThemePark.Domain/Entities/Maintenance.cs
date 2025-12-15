using ThemePark.Exceptions;

namespace ThemePark.Entities;

public class Maintenance
{
    private string _attractionName = string.Empty;
    private string _descripcion = string.Empty;
    private int _duracionMinutos;
    private DateTime _fecha;

    public string AttractionName
    {
        get => _attractionName;
        private set
        {
            if(string.IsNullOrWhiteSpace(value))
            {
                throw new InvalidMaintenanceDataException(nameof(AttractionName));
            }

            _attractionName = value;
        }
    }

    public DateTime Fecha
    {
        get => _fecha;
        private set
        {
            if(value.Date < DateTime.Now.Date)
            {
                throw new InvalidMaintenanceDataException(nameof(Fecha), value.ToString("yyyy-MM-dd"), "no puede ser en el pasado");
            }

            _fecha = value;
        }
    }

    public TimeSpan HoraInicio { get; private set; }

    public int DuracionMinutos
    {
        get => _duracionMinutos;
        private set
        {
            if(value <= 0)
            {
                throw new InvalidMaintenanceDataException(nameof(DuracionMinutos), value.ToString(), "debe ser mayor a 0");
            }

            _duracionMinutos = value;
        }
    }

    public string Descripcion
    {
        get => _descripcion;
        private set
        {
            if(string.IsNullOrWhiteSpace(value))
            {
                throw new InvalidMaintenanceDataException(nameof(Descripcion));
            }

            _descripcion = value;
        }
    }

    public Guid IncidentId { get; private set; }
    public Guid Id { get; private set; }

    public Maintenance(string attractionName, DateTime fecha, TimeSpan horaInicio, int duracionMinutos, string descripcion, Guid incidentId)
    {
        AttractionName = attractionName;
        Fecha = fecha;
        HoraInicio = horaInicio;
        DuracionMinutos = duracionMinutos;
        Descripcion = descripcion;
        IncidentId = incidentId;
        Id = Guid.NewGuid();
    }

    public Maintenance(string attractionName, DateTime fecha, TimeSpan horaInicio, int duracionMinutos, string descripcion)
    {
        AttractionName = attractionName;
        Fecha = fecha;
        HoraInicio = horaInicio;
        DuracionMinutos = duracionMinutos;
        Descripcion = descripcion;
        Id = Guid.NewGuid();
    }

    public void AssignIncident(Guid incidentId)
    {
        IncidentId = incidentId;
    }
}
