using ThemePark.Entities;

namespace ThemePark.BusinessLogic.Attractions;

public interface IIncidentManagementService
{
    string CreateIncident(string nombre, string descripcion);
    void ResolveIncident(string nombre, string id);
    List<Incident> GetAllIncidents();
    List<Incident> GetIncidentsByAttraction(string attractionName);
}
