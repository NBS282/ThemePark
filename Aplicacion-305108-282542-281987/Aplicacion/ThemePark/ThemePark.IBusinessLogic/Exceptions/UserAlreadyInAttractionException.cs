using ThemePark.Exceptions;

namespace ThemePark.IBusinessLogic.Exceptions;

public sealed class UserAlreadyInAttractionException(string attractionName) : BaseCustomException($"El usuario ya se encuentra en la atracción '{attractionName}'. Debe registrar la salida antes de ingresar a otra atracción",
       $"User is already in attraction '{attractionName}'. Must register exit before entering another attraction")
{
}
