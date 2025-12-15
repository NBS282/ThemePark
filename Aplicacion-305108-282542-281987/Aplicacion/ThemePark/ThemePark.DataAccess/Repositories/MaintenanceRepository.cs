using ThemePark.Entities;
using ThemePark.IDataAccess;

namespace ThemePark.DataAccess.Repositories;

public class MaintenanceRepository(ThemeParkDbContext context) : IMaintenanceRepository
{
    private readonly ThemeParkDbContext _context = context;

    public Maintenance Save(Maintenance maintenance)
    {
        _context.Maintenances.Add(maintenance);
        _context.SaveChanges();
        return maintenance;
    }

    public List<Maintenance> GetByAttractionName(string attractionName)
    {
        return _context.Maintenances
            .Where(m => m.AttractionName == attractionName)
            .ToList();
    }

    public Maintenance? GetById(Guid id)
    {
        return _context.Maintenances.FirstOrDefault(m => m.Id == id);
    }

    public void Delete(Guid id)
    {
        var maintenance = _context.Maintenances.FirstOrDefault(m => m.Id == id);
        if(maintenance != null)
        {
            _context.Maintenances.Remove(maintenance);
            _context.SaveChanges();
        }
    }
}
