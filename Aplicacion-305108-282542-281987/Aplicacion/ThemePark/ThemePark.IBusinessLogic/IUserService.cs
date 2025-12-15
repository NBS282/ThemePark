using ThemePark.Entities;

namespace ThemePark.IBusinessLogic;

public interface IUserService
{
    User Register(User user);
    User UpdateProfile(User user);
    User UpdateUserPrivileges(User user);
    List<User> GetAllUsers();
    User GetUserById(Guid id);
    User GetUserByCodigoIdentificacion(string codigoIdentificacion);
    void Delete(Guid id);
    List<object> GetUserPointHistory();
    object GetUserPoints();
}
