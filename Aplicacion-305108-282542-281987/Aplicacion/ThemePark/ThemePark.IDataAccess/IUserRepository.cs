using ThemePark.Entities;

namespace ThemePark.IDataAccess;

public interface IUserRepository
{
    User Add(User user);
    User Update(User user);
    User UpdateAdmin(User user);
    User GetById(Guid id);
    List<User> GetAll();
    bool ExistsByEmail(string email);
    bool ExistsByEmailExcludingUser(string email, Guid userId);
    bool ExistsByCodigoIdentificacion(string codigoIdentificacion);
    bool Exists(Guid id);
    User GetByEmailAndPassword(string email, string password);
    User GetByCodigoIdentificacion(string codigoIdentificacion);
    void Delete(Guid id);
    Guid? GetFounderAdminId();
}
