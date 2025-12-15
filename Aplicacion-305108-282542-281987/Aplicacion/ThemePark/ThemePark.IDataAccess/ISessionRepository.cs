using ThemePark.Entities;

namespace ThemePark.IDataAccess;

public interface ISessionRepository
{
    void Add(Session session);
    Session? GetByToken(string token);
    Guid GetCurrentUserId();
}
