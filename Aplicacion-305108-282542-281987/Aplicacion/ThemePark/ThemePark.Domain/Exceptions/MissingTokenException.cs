namespace ThemePark.Exceptions;

public sealed class MissingTokenException : BaseCustomException
{
    public MissingTokenException()
        : base("El token de autenticación es requerido en el header Authorization con formato 'Bearer {token}'",
               "Authentication token is required in Authorization header with format 'Bearer {token}'")
    {
    }
}
