namespace ThemePark.Exceptions;

public sealed class InvalidRequestDataException(string message) : BaseCustomException(message, $"Invalid request data: {message}")
{
}
