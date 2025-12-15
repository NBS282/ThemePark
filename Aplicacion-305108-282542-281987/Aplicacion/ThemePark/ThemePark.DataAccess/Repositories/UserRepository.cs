using Microsoft.EntityFrameworkCore;
using ThemePark.Entities;
using ThemePark.Entities.Roles;
using ThemePark.Enums;
using ThemePark.IDataAccess;
using ThemePark.IDataAccess.Exceptions;

namespace ThemePark.DataAccess.Repositories;

public class UserRepository(ThemeParkDbContext context) : IUserRepository
{
    private readonly ThemeParkDbContext _context = context;

    public User Add(User user)
    {
        if(user.Roles == null || !user.Roles.Any())
        {
            user.Roles = [new RolVisitante { Id = Guid.NewGuid(), UserId = user.Id }];
        }
        else
        {
            foreach(var rol in user.Roles)
            {
                if(rol.Id == Guid.Empty)
                {
                    rol.Id = Guid.NewGuid();
                }

                rol.UserId = user.Id;
            }
        }

        if(!_context.Users.Any() && user.Roles.Count == 1 && user.Roles.Any(r => r is RolVisitante))
        {
            var adminRol = new RolAdministradorParque { Id = Guid.NewGuid(), UserId = user.Id };
            var visitanteRol = new RolVisitante { Id = Guid.NewGuid(), UserId = user.Id, NivelMembresia = NivelMembresia.Premium };
            user.Roles = [adminRol, visitanteRol];
        }

        _context.Users.Add(user);
        _context.SaveChanges();
        return user;
    }

    public bool ExistsByEmail(string email)
    {
        return _context.Users.Any(u => u.Email == email);
    }

    public bool ExistsByEmailExcludingUser(string email, Guid userId)
    {
        return _context.Users.Any(u => u.Email == email && u.Id != userId);
    }

    public bool ExistsByCodigoIdentificacion(string codigoIdentificacion)
    {
        return _context.Users.Any(u => u.CodigoIdentificacion == codigoIdentificacion);
    }

    public bool Exists(Guid id)
    {
        return _context.Users.Any(u => u.Id == id);
    }

    public User GetByEmailAndPassword(string email, string password)
    {
        return _context.Users.Include(u => u.Roles).FirstOrDefault(u => u.Email == email && u.Contraseña == password);
    }

    public List<User> GetAll()
    {
        return _context.Users.Include(u => u.Roles).ToList();
    }

    public User? GetById(Guid id)
    {
        var user = _context.Users.Include(u => u.Roles).FirstOrDefault(u => u.Id == id);
        if(user == null)
        {
            throw new UserNotFoundException(id);
        }

        return user;
    }

    public User Update(User user)
    {
        var existingUser = GetById(user.Id);
        existingUser.Nombre = user.Nombre;
        existingUser.Apellido = user.Apellido;
        existingUser.Email = user.Email;
        existingUser.Contraseña = user.Contraseña;
        existingUser.FechaNacimiento = user.FechaNacimiento;

        _context.SaveChanges();
        return existingUser;
    }

    public User UpdateAdmin(User user)
    {
        var existingUser = GetById(user.Id);
        _context.Roles.RemoveRange(existingUser.Roles);
        foreach(var rol in user.Roles)
        {
            if(rol.Id == Guid.Empty)
            {
                rol.Id = Guid.NewGuid();
            }

            rol.UserId = user.Id;
        }

        existingUser.Roles = user.Roles;

        _context.SaveChanges();
        return existingUser;
    }

    public User GetByCodigoIdentificacion(string codigoIdentificacion)
    {
        var user = _context.Users.Include(u => u.Roles).FirstOrDefault(u => u.CodigoIdentificacion == codigoIdentificacion);
        if(user == null)
        {
            throw new UserNotFoundException(codigoIdentificacion);
        }

        return user;
    }

    public void Delete(Guid id)
    {
        var user = GetById(id);
        _context.Users.Remove(user!);
        _context.SaveChanges();
    }

    public Guid? GetFounderAdminId()
    {
        var founderAdmin = _context.Users
            .Include(u => u.Roles)
            .Where(u => u.Roles.Any(r => r is RolAdministradorParque))
            .OrderBy(u => u.FechaRegistro)
            .FirstOrDefault();

        return founderAdmin?.Id;
    }
}
