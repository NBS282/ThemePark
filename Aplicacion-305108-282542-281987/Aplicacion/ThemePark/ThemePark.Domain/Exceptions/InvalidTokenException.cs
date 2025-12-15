namespace ThemePark.Exceptions;

public sealed class InvalidTokenException : BaseCustomException
{
    public InvalidTokenException()
        : base("El token es inválido o ha expirado",
               "Token is invalid or has expired")
    {
    }
}
