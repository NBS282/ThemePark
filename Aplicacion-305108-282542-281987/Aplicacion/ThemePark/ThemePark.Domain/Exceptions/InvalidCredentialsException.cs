namespace ThemePark.Exceptions;

public sealed class InvalidCredentialsException : BaseCustomException
{
    public InvalidCredentialsException()
        : base("Las credenciales proporcionadas son inválidas",
               "Invalid credentials provided")
    {
    }
}
