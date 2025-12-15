using ThemePark.Exceptions;

namespace ThemePark.IDataAccess.Exceptions;

public sealed class IncidentNotFoundException(string id) : BaseCustomException($"Incidente con ID '{id}' no encontrado",
           $"Incident with ID '{id}' not found")
{
}
