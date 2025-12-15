using ThemePark.BusinessLogic.Attractions;
using ThemePark.Entities;
using ThemePark.IBusinessLogic;

namespace ThemePark.BusinessLogic;

public class AttractionsBusinessLogic(
    IAttractionManagementService managementService,
    IAccessControlService accessControlService,
    IIncidentManagementService incidentService,
    IAttractionReportingService reportingService,
    IMaintenanceManagementService maintenanceService) : IAttractionsBusinessLogic
{
    private readonly IAttractionManagementService _managementService = managementService;
    private readonly IAccessControlService _accessControlService = accessControlService;
    private readonly IIncidentManagementService _incidentService = incidentService;
    private readonly IAttractionReportingService _reportingService = reportingService;
    private readonly IMaintenanceManagementService _maintenanceService = maintenanceService;

    public Attraction CreateAttraction(string nombre, int tipo, int edadMinima, int capacidadMaxima, string descripcion, int points)
    {
        return _managementService.CreateAttraction(nombre, tipo, edadMinima, capacidadMaxima, descripcion, points);
    }

    public void DeleteAttraction(string nombre)
    {
        _managementService.DeleteAttraction(nombre);
    }

    public Visit ValidateTicketAndRegisterAccess(string nombre, string tipoEntrada, string codigo)
    {
        return _accessControlService.ValidateTicketAndRegisterAccess(nombre, tipoEntrada, codigo);
    }

    public Visit RegisterExit(string nombre, string tipoEntrada, string codigo)
    {
        return _accessControlService.RegisterExit(nombre, tipoEntrada, codigo);
    }

    public Attraction GetCapacity(string nombre)
    {
        return _managementService.GetCapacity(nombre);
    }

    public string CreateIncident(string nombre, string descripcion)
    {
        return _incidentService.CreateIncident(nombre, descripcion);
    }

    public void ResolveIncident(string nombre, string id)
    {
        _incidentService.ResolveIncident(nombre, id);
    }

    public Attraction UpdateAttraction(string nombre, string descripcion, int? capacidadMaxima, int? edadMinima)
    {
        return _managementService.UpdateAttraction(nombre, descripcion, capacidadMaxima, edadMinima);
    }

    public Dictionary<string, int> GetUsageReport(DateTime fechaInicio, DateTime fechaFin)
    {
        return _reportingService.GetUsageReport(fechaInicio, fechaFin);
    }

    public (string MaintenanceId, string IncidentId) CreatePreventiveMaintenance(Maintenance maintenance)
    {
        return _maintenanceService.CreatePreventiveMaintenance(maintenance);
    }

    public List<Maintenance> GetMaintenancesByAttraction(string attractionName)
    {
        return _maintenanceService.GetMaintenancesByAttraction(attractionName);
    }

    public void CancelPreventiveMaintenance(string attractionName, string maintenanceId)
    {
        _maintenanceService.CancelPreventiveMaintenance(attractionName, maintenanceId);
    }

    public List<Attraction> GetAll()
    {
        return _managementService.GetAll();
    }

    public Attraction GetById(string nombre)
    {
        return _managementService.GetById(nombre);
    }

    public List<Incident> GetAllIncidents()
    {
        return _incidentService.GetAllIncidents();
    }

    public List<Incident> GetIncidentsByAttraction(string attractionName)
    {
        return _incidentService.GetIncidentsByAttraction(attractionName);
    }
}
