using ThemePark.Exceptions;

namespace ThemePark.IDataAccess.Exceptions;

public sealed class CannotDeleteAttractionException(string attractionName) : BaseCustomException($"No se puede eliminar la atracción '{attractionName}' porque tiene registros asociados como visitas, eventos o mantenimientos. Debe eliminar primero estos registros relacionados.",
       $"Cannot delete attraction '{attractionName}' because it has associated records such as visits, events or maintenances. You must delete these related records first.")
{
}
