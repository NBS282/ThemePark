using ThemePark.Entities;

namespace ThemePark.BusinessLogic.Attractions;

public interface IMaintenanceManagementService
{
    (string MaintenanceId, string IncidentId) CreatePreventiveMaintenance(Maintenance maintenance);
    List<Maintenance> GetMaintenancesByAttraction(string attractionName);
    void CancelPreventiveMaintenance(string attractionName, string maintenanceId);
}
