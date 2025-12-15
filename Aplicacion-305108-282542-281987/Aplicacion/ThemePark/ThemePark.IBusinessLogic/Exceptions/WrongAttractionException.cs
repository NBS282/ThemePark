using ThemePark.Exceptions;

namespace ThemePark.IBusinessLogic.Exceptions;

public sealed class WrongAttractionException(string attractionName) : BaseCustomException($"Este ticket no es válido para la atracción '{attractionName}'",
           $"This ticket is not valid for attraction '{attractionName}'")
{
}
