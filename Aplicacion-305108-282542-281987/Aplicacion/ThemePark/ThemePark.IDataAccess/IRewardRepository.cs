using ThemePark.Entities;

namespace ThemePark.IDataAccess;

public interface IRewardRepository
{
    bool ExistsByName(string nombre);
    void Save(Reward reward);
    Reward? GetById(int id);
    List<Reward> GetAll();
    void Delete(int id);
}
