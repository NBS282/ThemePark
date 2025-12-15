using ThemePark.Exceptions;

namespace ThemePark.IBusinessLogic.Exceptions;

public sealed class ForbiddenException(string message)
    : BaseCustomException(message, $"Forbidden: {message}")
{
}
