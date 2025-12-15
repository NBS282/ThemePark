using ThemePark.Entities;

namespace ThemePark.BusinessLogic.Attractions;

public interface IAttractionManagementService
{
    Attraction CreateAttraction(string nombre, int tipo, int edadMinima, int capacidadMaxima, string descripcion, int points);
    void DeleteAttraction(string nombre);
    Attraction UpdateAttraction(string nombre, string descripcion, int? capacidadMaxima, int? edadMinima);
    Attraction GetCapacity(string nombre);
    List<Attraction> GetAll();
    Attraction GetById(string nombre);
}
