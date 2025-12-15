using ThemePark.Exceptions;

namespace ThemePark.IBusinessLogic.Exceptions;

public sealed class CannotDeleteAttractionWithActiveIncidentsException(string attractionName) : BaseCustomException($"No se puede eliminar la atracción '{attractionName}' porque tiene incidencias activas. Debe resolver las incidencias antes de eliminar la atracción.",
       $"Cannot delete attraction '{attractionName}' because it has active incidents. You must resolve the incidents before deleting the attraction.")
{
}
