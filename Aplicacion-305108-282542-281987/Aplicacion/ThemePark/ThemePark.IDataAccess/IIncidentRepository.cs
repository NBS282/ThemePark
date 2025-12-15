using ThemePark.Entities;

namespace ThemePark.IDataAccess;

public interface IIncidentRepository
{
    Incident Save(Incident incident);
    Incident GetById(Guid id);
    List<Incident> GetActiveByAttractionName(string attractionName);
    Incident Update(Incident incident);
    List<Incident> GetAll();
}
