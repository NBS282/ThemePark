using ThemePark.Entities;

namespace ThemePark.IBusinessLogic;

public interface IAttractionsBusinessLogic
{
    Attraction CreateAttraction(string nombre, int tipo, int edadMinima, int capacidadMaxima, string descripcion, int points);
    void DeleteAttraction(string nombre);
    Visit ValidateTicketAndRegisterAccess(string nombre, string tipoEntrada, string codigo);
    Visit RegisterExit(string nombre, string tipoEntrada, string codigo);
    Attraction GetCapacity(string nombre);
    string CreateIncident(string nombre, string descripcion);
    void ResolveIncident(string nombre, string id);
    Attraction UpdateAttraction(string nombre, string descripcion, int? capacidadMaxima, int? edadMinima);
    Dictionary<string, int> GetUsageReport(DateTime fechaInicio, DateTime fechaFin);
    (string MaintenanceId, string IncidentId) CreatePreventiveMaintenance(Maintenance maintenance);
    List<Maintenance> GetMaintenancesByAttraction(string attractionName);
    void CancelPreventiveMaintenance(string attractionName, string maintenanceId);
    List<Attraction> GetAll();
    Attraction GetById(string nombre);
    List<Incident> GetAllIncidents();
    List<Incident> GetIncidentsByAttraction(string attractionName);
}
