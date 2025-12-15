using ThemePark.Entities;
using ThemePark.IDataAccess;
using ThemePark.IDataAccess.Exceptions;

namespace ThemePark.DataAccess.Repositories;

public class IncidentRepository(ThemeParkDbContext context) : IIncidentRepository
{
    private readonly ThemeParkDbContext _context = context;

    public Incident Save(Incident incident)
    {
        _context.Incidents.Add(incident);
        _context.SaveChanges();
        return incident;
    }

    public Incident GetById(Guid id)
    {
        var incident = _context.Incidents.FirstOrDefault(i => i.Id == id);
        if(incident == null)
        {
            throw new IncidentNotFoundException(id.ToString());
        }

        return incident;
    }

    public List<Incident> GetActiveByAttractionName(string attractionName)
    {
        return _context.Incidents
            .Where(i => i.AttractionName == attractionName && i.IsActive)
            .ToList();
    }

    public Incident Update(Incident incident)
    {
        _context.Incidents.Update(incident);
        _context.SaveChanges();
        return incident;
    }

    public List<Incident> GetAll()
    {
        return _context.Incidents.ToList();
    }
}
