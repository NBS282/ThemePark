using ThemePark.Entities;
using ThemePark.IBusinessLogic.Exceptions;
using ThemePark.IDataAccess;
using ThemePark.IDataAccess.Exceptions;

namespace ThemePark.BusinessLogic.Attractions;

public class MaintenanceManagementService(
    IAttractionRepository attractionRepository,
    IMaintenanceRepository maintenanceRepository,
    IIncidentRepository incidentRepository,
    IDateTimeRepository dateTimeRepository) : IMaintenanceManagementService
{
    private readonly IAttractionRepository _attractionRepository = attractionRepository;
    private readonly IMaintenanceRepository _maintenanceRepository = maintenanceRepository;
    private readonly IIncidentRepository _incidentRepository = incidentRepository;
    private readonly IDateTimeRepository _dateTimeRepository = dateTimeRepository;

    public (string MaintenanceId, string IncidentId) CreatePreventiveMaintenance(Maintenance maintenance)
    {
        if(!_attractionRepository.ExistsByName(maintenance.AttractionName))
        {
            throw new AttractionNotFoundException(maintenance.AttractionName);
        }

        _ = _attractionRepository.GetByName(maintenance.AttractionName);
        var fechaCreacion = _dateTimeRepository.GetCurrentDateTime() ?? DateTime.Now;
        var incident = new Incident(maintenance.AttractionName, maintenance.Descripcion, fechaCreacion, maintenance.Id, maintenance.Fecha, maintenance.HoraInicio);

        _incidentRepository.Save(incident);

        maintenance.AssignIncident(incident.Id);
        _maintenanceRepository.Save(maintenance);

        return (maintenance.Id.ToString(), incident.Id.ToString());
    }

    public List<Maintenance> GetMaintenancesByAttraction(string attractionName)
    {
        if(!_attractionRepository.ExistsByName(attractionName))
        {
            throw new AttractionNotFoundException(attractionName);
        }

        return _maintenanceRepository.GetByAttractionName(attractionName);
    }

    public void CancelPreventiveMaintenance(string attractionName, string maintenanceId)
    {
        if(!_attractionRepository.ExistsByName(attractionName))
        {
            throw new AttractionNotFoundException(attractionName);
        }

        var attraction = _attractionRepository.GetByName(attractionName);
        var maintenanceGuid = Guid.Parse(maintenanceId);
        var maintenance = _maintenanceRepository.GetById(maintenanceGuid);

        if(maintenance == null)
        {
            throw BusinessLogicException.MaintenanceNotFound(maintenanceId);
        }

        var fechaResolucion = _dateTimeRepository.GetCurrentDateTime() ?? DateTime.Now;
        var incident = _incidentRepository.GetById(maintenance.IncidentId);
        incident.Resolve(fechaResolucion);
        attraction.DesactivarIncidencia();

        _attractionRepository.Save(attraction);
        _incidentRepository.Update(incident);
        _maintenanceRepository.Delete(maintenanceGuid);
    }
}
