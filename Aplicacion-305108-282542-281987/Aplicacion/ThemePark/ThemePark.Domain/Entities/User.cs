using ThemePark.Entities.Roles;
using ThemePark.Enums;
using ThemePark.Exceptions;
using Rol = ThemePark.Entities.Roles.Rol;

namespace ThemePark.Entities;

public class User
{
    private string _nombre = string.Empty;

    private Guid _id;

    public Guid Id
    {
        get => _id;
        set
        {
            if(value == Guid.Empty)
            {
                throw new InvalidUserDataException("Id", "vacío");
            }

            _id = value;
        }
    }

    public string Nombre
    {
        get => _nombre;
        set
        {
            if(string.IsNullOrWhiteSpace(value))
            {
                throw new InvalidUserDataException("Nombre");
            }

            _nombre = value;
        }
    }

    private string _email = string.Empty;
    public string Email
    {
        get => _email;
        set
        {
            if(string.IsNullOrWhiteSpace(value))
            {
                throw new InvalidUserDataException("Email");
            }

            try
            {
                var mailAddress = new System.Net.Mail.MailAddress(value);
            }
            catch(FormatException)
            {
                throw new InvalidUserDataException("Email", value);
            }

            _email = value;
        }
    }

    private string _apellido = string.Empty;
    public string Apellido
    {
        get => _apellido;
        set
        {
            if(string.IsNullOrWhiteSpace(value))
            {
                throw new InvalidUserDataException("Apellido");
            }

            _apellido = value;
        }
    }

    private string _contraseña = string.Empty;
    public string Contraseña
    {
        get => _contraseña;
        set
        {
            if(string.IsNullOrWhiteSpace(value))
            {
                throw new InvalidUserDataException("Contraseña");
            }

            _contraseña = value;
        }
    }

    private DateTime _fechaNacimiento;

    public User()
    {
        Roles = [];
    }

    public User(List<Rol> roles)
    {
        Roles = roles;
    }

    public DateTime FechaNacimiento
    {
        get => _fechaNacimiento;
        set
        {
            if(value > DateTime.Now)
            {
                throw new InvalidUserDataException("FechaNacimiento", "en el futuro");
            }

            _fechaNacimiento = value;
        }
    }

    public List<Entities.Roles.Rol> Roles { get; set; } = [];
    public DateTime FechaRegistro { get; set; }
    public string CodigoIdentificacion { get; set; } = string.Empty;

    public T? ObtenerRol<T>()
        where T : Entities.Roles.Rol
    {
        return Roles.OfType<T>().FirstOrDefault();
    }

    public bool TieneRol<T>()
        where T : Entities.Roles.Rol
    {
        return Roles.OfType<T>().Any();
    }

    public NivelMembresia GetMembresia()
    {
        var rolVisitante = ObtenerRol<RolVisitante>();
        ValidarQueExisteRolVisitante(rolVisitante);

        return rolVisitante!.NivelMembresia;
    }

    private static void ValidarQueExisteRolVisitante(RolVisitante? rolVisitante)
    {
        if(rolVisitante == null)
        {
            throw new InvalidUserDataException("El usuario no tiene rol de visitante", "User does not have visitor role");
        }
    }
}
