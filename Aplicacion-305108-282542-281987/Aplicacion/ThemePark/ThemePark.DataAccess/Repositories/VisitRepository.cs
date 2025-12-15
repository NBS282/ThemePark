using Microsoft.EntityFrameworkCore;
using ThemePark.Entities;
using ThemePark.IDataAccess;
using ThemePark.IDataAccess.Exceptions;

namespace ThemePark.DataAccess.Repositories;

public class VisitRepository(ThemeParkDbContext context) : IVisitRepository
{
    private readonly ThemeParkDbContext _context = context;

    public Visit Save(Visit visit)
    {
        _context.Visits.Add(visit);
        _context.SaveChanges();
        return visit;
    }

    public Visit Update(Visit visit)
    {
        _context.Visits.Update(visit);
        _context.SaveChanges();
        return visit;
    }

    public Visit GetById(Guid visitId)
    {
        var visit = _context.Visits.FirstOrDefault(v => v.Id == visitId);
        if(visit == null)
        {
            throw new VisitNotFoundException(visitId);
        }

        return visit;
    }

    public List<Visit> GetByUserId(Guid userId)
    {
        return _context.Visits.Where(v => v.UserId == userId).ToList();
    }

    public List<Visit> GetActiveVisitsByAttraction(string attractionName)
    {
        return _context.Visits
            .Where(v => v.AttractionName == attractionName && v.IsActive)
            .ToList();
    }

    public Visit? GetActiveVisitByUserAndAttraction(Guid userId, string attractionName)
    {
        return _context.Visits
            .FirstOrDefault(v => v.UserId == userId && v.AttractionName == attractionName && v.IsActive);
    }

    public Visit? GetActiveVisitByUser(Guid userId)
    {
        return _context.Visits
            .Include(v => v.Attraction)
            .FirstOrDefault(v => v.UserId == userId && v.IsActive);
    }

    public List<Visit> GetVisitsByDate(DateTime date)
    {
        return _context.Visits
            .Where(v => v.EntryTime.Date == date.Date)
            .ToList();
    }

    public List<Visit> GetVisitsByDateRange(DateTime fechaInicio, DateTime fechaFin)
    {
        return _context.Visits
            .Where(v => v.EntryTime.Date >= fechaInicio.Date && v.EntryTime.Date <= fechaFin.Date)
            .ToList();
    }

    public List<Visit> GetVisitsByUserAndDate(Guid userId, DateTime date)
    {
        return _context.Visits
            .Where(v => v.UserId == userId && v.EntryTime.Date == date.Date)
            .ToList();
    }
}
