using ThemePark.Exceptions;

namespace ThemePark.IBusinessLogic.Exceptions;

public sealed class NoActiveVisitException(string attractionName, string userIdentification) : BaseCustomException($"No hay ninguna visita activa para el usuario '{userIdentification}' en la atracción '{attractionName}'",
           $"No active visit exists for user '{userIdentification}' at attraction '{attractionName}'")
{
}
