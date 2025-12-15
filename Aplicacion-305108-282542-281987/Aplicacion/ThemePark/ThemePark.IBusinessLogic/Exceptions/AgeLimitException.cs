using ThemePark.Exceptions;

namespace ThemePark.IBusinessLogic.Exceptions;

public sealed class AgeLimitException(string attractionName, int requiredAge, int userAge) : BaseCustomException($"La atracción '{attractionName}' requiere edad mínima de {requiredAge} años. Edad actual: {userAge} años",
           $"Attraction '{attractionName}' requires minimum age of {requiredAge} years. Current age: {userAge} years")
{
}
