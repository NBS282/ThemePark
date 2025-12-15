using Microsoft.EntityFrameworkCore;
using ThemePark.Entities;
using ThemePark.IDataAccess;
using ThemePark.IDataAccess.Exceptions;

namespace ThemePark.DataAccess.Repositories;

public class AttractionRepository(ThemeParkDbContext context) : IAttractionRepository
{
    private readonly ThemeParkDbContext _context = context;

    public bool ExistsByName(string nombre)
    {
        return _context.Attractions.Any(a => a.Nombre == nombre);
    }

    public void Save(Attraction attraction)
    {
        var existing = _context.Attractions.Find(attraction.Nombre);
        if(existing == null)
        {
            _context.Attractions.Add(attraction);
        }
        else
        {
            _context.Entry(existing).CurrentValues.SetValues(attraction);
        }

        _context.SaveChanges();
    }

    public Attraction GetByName(string nombre)
    {
        return _context.Attractions.First(a => a.Nombre == nombre);
    }

    public void Delete(string nombre)
    {
        var attraction = _context.Attractions.First(a => a.Nombre == nombre);
        _context.Attractions.Remove(attraction);

        try
        {
            _context.SaveChanges();
        }
        catch(DbUpdateException)
        {
            throw new CannotDeleteAttractionException(nombre);
        }
    }

    public List<Attraction> GetAll()
    {
        return _context.Attractions.ToList();
    }

    public Attraction? GetById(string nombre)
    {
        return _context.Attractions.FirstOrDefault(a => a.Nombre == nombre);
    }
}
