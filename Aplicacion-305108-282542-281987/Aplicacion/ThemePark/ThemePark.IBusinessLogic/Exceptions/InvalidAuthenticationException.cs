using ThemePark.Exceptions;

namespace ThemePark.IBusinessLogic.Exceptions;

public sealed class InvalidAuthenticationException(string field) : BaseCustomException($"{field} no puede estar vacío o nulo", $"{field} cannot be empty or null")
{
}
