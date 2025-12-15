using ThemePark.Entities;
using ThemePark.Exceptions;
using ThemePark.IBusinessLogic;
using ThemePark.IBusinessLogic.Exceptions;
using ThemePark.IDataAccess;

namespace ThemePark.BusinessLogic;

public class AuthService(IUserRepository userRepository, ISessionRepository sessionRepository) : IAuthService
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly ISessionRepository _sessionRepository = sessionRepository;

    public (Session Session, User User) Login(string email, string password)
    {
        ValidateLoginParameters(email, password);

        var user = _userRepository.GetByEmailAndPassword(email, password);
        if(user == null)
        {
            throw new InvalidCredentialsException();
        }

        var session = new Session
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Token = GenerateToken(),
            CreatedAt = DateTime.Now,
            ExpirationDate = DateTime.Now.AddDays(1)
        };

        _sessionRepository.Add(session);
        return (session, user);
    }

    public bool ValidateToken(string token)
    {
        if(string.IsNullOrWhiteSpace(token))
        {
            return false;
        }

        var session = _sessionRepository.GetByToken(token);
        return session != null && session.ExpirationDate > DateTime.Now;
    }

    public User? GetUserByToken(string token)
    {
        if(!ValidateToken(token))
        {
            return null;
        }

        var session = _sessionRepository.GetByToken(token);
        if(session == null)
        {
            return null;
        }

        return _userRepository.GetById(session.UserId);
    }

    private void ValidateLoginParameters(string email, string password)
    {
        if(string.IsNullOrWhiteSpace(email))
        {
            throw new InvalidAuthenticationException("El email");
        }

        if(string.IsNullOrWhiteSpace(password))
        {
            throw new InvalidAuthenticationException("La contraseña");
        }
    }

    private string GenerateToken()
    {
        return Guid.NewGuid().ToString();
    }
}
