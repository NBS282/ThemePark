using ThemePark.Exceptions;

namespace ThemePark.IBusinessLogic.Exceptions;

public sealed class VisitAlreadyActiveException(string attractionName) : BaseCustomException($"Ya existe una visita activa en la atracción '{attractionName}'. Debe registrar la salida antes de ingresar nuevamente",
           $"An active visit already exists at attraction '{attractionName}'. Must register exit before entering again")
{
}
