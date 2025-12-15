using ThemePark.Entities;

namespace ThemePark.IDataAccess;

public interface IMaintenanceRepository
{
    Maintenance Save(Maintenance maintenance);
    List<Maintenance> GetByAttractionName(string attractionName);
    Maintenance? GetById(Guid id);
    void Delete(Guid id);
}
