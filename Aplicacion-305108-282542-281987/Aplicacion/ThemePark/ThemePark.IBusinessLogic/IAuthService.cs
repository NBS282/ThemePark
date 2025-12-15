using ThemePark.Entities;

namespace ThemePark.IBusinessLogic;

public interface IAuthService
{
    (Session Session, User User) Login(string email, string password);
    bool ValidateToken(string token);
    User? GetUserByToken(string token);
}
