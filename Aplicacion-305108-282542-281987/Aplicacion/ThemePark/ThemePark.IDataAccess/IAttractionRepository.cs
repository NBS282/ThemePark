using ThemePark.Entities;

namespace ThemePark.IDataAccess;

public interface IAttractionRepository
{
    bool ExistsByName(string nombre);
    void Save(Attraction attraction);
    Attraction GetByName(string nombre);
    void Delete(string nombre);
    List<Attraction> GetAll();
    Attraction? GetById(string nombre);
}
