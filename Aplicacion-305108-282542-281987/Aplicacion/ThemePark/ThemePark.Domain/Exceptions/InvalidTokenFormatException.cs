namespace ThemePark.Exceptions;

public sealed class InvalidTokenFormatException : BaseCustomException
{
    public InvalidTokenFormatException()
        : base("El formato del token es inválido. Debe ser 'Bearer {token}'",
               "Invalid token format. Must be 'Bearer {token}'")
    {
    }
}
