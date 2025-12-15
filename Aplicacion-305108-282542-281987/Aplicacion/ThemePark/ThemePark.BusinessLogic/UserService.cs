using ThemePark.Entities;
using ThemePark.IBusinessLogic;
using ThemePark.IBusinessLogic.Exceptions;
using ThemePark.IDataAccess;

namespace ThemePark.BusinessLogic;

public class UserService(IUserRepository userRepository, IVisitRepository visitRepository, ISessionRepository sessionRepository, IRewardExchangeRepository rewardExchangeRepository) : IUserService
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IVisitRepository _visitRepository = visitRepository;
    private readonly ISessionRepository _sessionRepository = sessionRepository;
    private readonly IRewardExchangeRepository _rewardExchangeRepository = rewardExchangeRepository;

    public User Register(User user)
    {
        if(_userRepository.ExistsByEmail(user.Email))
        {
            throw new UserRegistrationException(user.Email);
        }

        if(string.IsNullOrEmpty(user.CodigoIdentificacion))
        {
            user.CodigoIdentificacion = GenerateUniqueCodigoIdentificacion();
        }

        return _userRepository.Add(user);
    }

    public User UpdateProfile(User user)
    {
        _userRepository.GetById(user.Id);
        if(!string.IsNullOrEmpty(user.Email) && _userRepository.ExistsByEmailExcludingUser(user.Email, user.Id))
        {
            throw UserRegistrationException.EmailInUse(user.Email);
        }

        return _userRepository.Update(user);
    }

    public List<User> GetAllUsers()
    {
        return _userRepository.GetAll();
    }

    public User GetUserById(Guid id)
    {
        return _userRepository.GetById(id);
    }

    public User GetUserByCodigoIdentificacion(string codigoIdentificacion)
    {
        return _userRepository.GetByCodigoIdentificacion(codigoIdentificacion);
    }

    public User UpdateUserPrivileges(User user)
    {
        _userRepository.GetById(user.Id);
        if(user.Roles.Count == 0)
        {
            throw BusinessLogicException.InvalidRoleType("valor desconocido");
        }

        foreach(var role in user.Roles)
        {
            if(!Enum.IsDefined(typeof(Enums.Rol), role.TipoRol))
            {
                throw BusinessLogicException.InvalidRoleType(((int)role.TipoRol).ToString());
            }
        }

        foreach(var role in user.Roles)
        {
            if(role is Entities.Roles.RolVisitante visitante)
            {
                if(!Enum.IsDefined(typeof(Enums.NivelMembresia), visitante.NivelMembresia))
                {
                    throw BusinessLogicException.InvalidMembershipType(((int)visitante.NivelMembresia).ToString());
                }
            }
        }

        var founderId = _userRepository.GetFounderAdminId();
        if(founderId.HasValue && founderId.Value == user.Id)
        {
            var hasAdminRole = user.Roles.Any(r => r is Entities.Roles.RolAdministradorParque);
            if(!hasAdminRole)
            {
                throw new CannotModifyFounderAdminException();
            }
        }

        return _userRepository.UpdateAdmin(user);
    }

    public void Delete(Guid id)
    {
        _userRepository.GetById(id);

        var founderId = _userRepository.GetFounderAdminId();
        if(founderId.HasValue && founderId.Value == id)
        {
            throw new CannotModifyFounderAdminException();
        }

        _userRepository.Delete(id);
    }

    public List<object> GetUserPointHistory()
    {
        var userId = _sessionRepository.GetCurrentUserId();
        var visits = _visitRepository.GetByUserId(userId);
        var exchanges = _rewardExchangeRepository.GetByUserId(userId);

        var history = new List<object>();
        history.AddRange(visits);
        history.AddRange(exchanges);

        return history.OrderByDescending(item =>
        {
            if(item is Visit visit)
            {
                return visit.EntryTime;
            }
            else if(item is RewardExchange exchange)
            {
                return exchange.FechaCanje;
            }

            return DateTime.MinValue;
        }).ToList();
    }

    public object GetUserPoints()
    {
        var userId = _sessionRepository.GetCurrentUserId();
        var userVisits = _visitRepository.GetByUserId(userId);
        var puntosAcumulados = userVisits.Sum(v => v.Points);

        var previousExchanges = _rewardExchangeRepository.GetByUserId(userId);
        var puntosGastados = previousExchanges.Sum(e => e.PuntosDescontados);

        var puntosDisponibles = puntosAcumulados - puntosGastados;

        return new
        {
            PuntosDisponibles = puntosDisponibles,
            PuntosAcumulados = puntosAcumulados,
            PuntosGastados = puntosGastados
        };
    }

    private string GenerateUniqueCodigoIdentificacion()
    {
        string codigo;
        do
        {
            codigo = Guid.NewGuid().ToString("N")[..8].ToUpper();
        }
        while(_userRepository.ExistsByCodigoIdentificacion(codigo));

        return codigo;
    }
}
