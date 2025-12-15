using ThemePark.Entities;
using ThemePark.IBusinessLogic.Exceptions;
using ThemePark.IDataAccess;
using ThemePark.IDataAccess.Exceptions;

namespace ThemePark.BusinessLogic.Attractions;

public class IncidentManagementService(
    IAttractionRepository attractionRepository,
    IIncidentRepository incidentRepository,
    IDateTimeRepository dateTimeRepository,
    IMaintenanceRepository maintenanceRepository) : IIncidentManagementService
{
    private readonly IAttractionRepository _attractionRepository = attractionRepository;
    private readonly IIncidentRepository _incidentRepository = incidentRepository;
    private readonly IDateTimeRepository _dateTimeRepository = dateTimeRepository;
    private readonly IMaintenanceRepository _maintenanceRepository = maintenanceRepository;

    public string CreateIncident(string nombre, string descripcion)
    {
        if(!_attractionRepository.ExistsByName(nombre))
        {
            throw new AttractionNotFoundException(nombre);
        }

        var activeIncidents = _incidentRepository.GetActiveByAttractionName(nombre);
        if(activeIncidents.Any())
        {
            throw BusinessLogicException.ActiveIncidentAlreadyExists(nombre);
        }

        var attraction = _attractionRepository.GetByName(nombre);
        var fechaCreacion = _dateTimeRepository.GetCurrentDateTime() ?? DateTime.Now;
        var incident = new Incident(nombre, descripcion, fechaCreacion);

        attraction.ActivarIncidencia();
        _attractionRepository.Save(attraction);
        _incidentRepository.Save(incident);

        return incident.Id.ToString();
    }

    public void ResolveIncident(string nombre, string id)
    {
        if(!_attractionRepository.ExistsByName(nombre))
        {
            throw new AttractionNotFoundException(nombre);
        }

        var attraction = _attractionRepository.GetByName(nombre);
        var incidentId = Guid.Parse(id);
        var incident = _incidentRepository.GetById(incidentId);
        var fechaResolucion = _dateTimeRepository.GetCurrentDateTime() ?? DateTime.Now;

        incident.Resolve(fechaResolucion);
        attraction.DesactivarIncidencia();

        _attractionRepository.Save(attraction);
        _incidentRepository.Update(incident);

        if(incident.IsMaintenanceIncident())
        {
            _maintenanceRepository.Delete(incident.MaintenanceId!.Value);
        }
    }

    public List<Incident> GetAllIncidents()
    {
        return _incidentRepository.GetAll();
    }

    public List<Incident> GetIncidentsByAttraction(string attractionName)
    {
        if(!_attractionRepository.ExistsByName(attractionName))
        {
            throw new AttractionNotFoundException(attractionName);
        }

        return _incidentRepository.GetActiveByAttractionName(attractionName);
    }
}
