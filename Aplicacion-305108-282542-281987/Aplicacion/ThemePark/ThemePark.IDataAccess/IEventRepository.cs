using System.Linq.Expressions;
using ThemePark.Entities;

namespace ThemePark.IDataAccess;

public interface IEventRepository
{
    Event Add(Event evento);
    Event Update(Event evento);
    Event GetById(Guid id);
    List<Event> GetAll();
    bool ExistsByName(string name);
    bool Exists(Guid id);
    void Delete(Guid id);
    List<Event> Get(Expression<Func<Event, bool>> predicate);
}
