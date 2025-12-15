using ThemePark.Exceptions;

namespace ThemePark.IDataAccess.Exceptions;

public sealed class VisitNotFoundException(Guid visitId) : BaseCustomException($"Visita con ID {visitId} no encontrada",
           $"Visit with ID {visitId} not found")
{
}
