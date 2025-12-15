using ThemePark.Exceptions;

namespace ThemePark.IBusinessLogic.Exceptions;

public sealed class ActiveIncidentException(string attractionName)
    : BaseCustomException(
        $"Ya existe una incidencia activa para la atracción '{attractionName}'",
        $"Active incident already exists for attraction: {attractionName}")
{
}
