using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using ThemePark.Entities;
using ThemePark.IDataAccess;
using ThemePark.IDataAccess.Exceptions;

namespace ThemePark.DataAccess.Repositories;

public class EventRepository(ThemeParkDbContext context) : IEventRepository
{
    private readonly ThemeParkDbContext _context = context;

    public Event Add(Event evento)
    {
        _context.Events.Add(evento);
        _context.SaveChanges();
        return evento;
    }

    public Event Update(Event evento)
    {
        _context.Events.Update(evento);
        _context.SaveChanges();
        return evento;
    }

    public Event GetById(Guid id)
    {
        var evento = _context.Events
            .Include(e => e.Atracciones)
            .FirstOrDefault(e => e.Id == id);
        if(evento == null)
        {
            throw new EventNotFoundException(id);
        }

        return evento;
    }

    public List<Event> GetAll()
    {
        return _context.Events
            .Include(e => e.Atracciones)
            .ToList();
    }

    public bool ExistsByName(string name)
    {
        return _context.Events.Any(e => e.Name == name);
    }

    public bool Exists(Guid id)
    {
        return _context.Events.Any(e => e.Id == id);
    }

    public void Delete(Guid id)
    {
        var evento = _context.Events.Find(id);
        if(evento == null)
        {
            throw new EventNotFoundException(id);
        }

        _context.Events.Remove(evento);
        _context.SaveChanges();
    }

    public List<Event> Get(Expression<Func<Event, bool>> predicate)
    {
        return _context.Events
            .Include(e => e.Atracciones)
            .Where(predicate)
            .ToList();
    }
}
