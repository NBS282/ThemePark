using ThemePark.Exceptions;

namespace ThemePark.IDataAccess.Exceptions;

public sealed class AttractionNotFoundException(string nombre) : BaseCustomException($"Atracción '{nombre}' no encontrada",
           $"Attraction '{nombre}' not found")
{
}
