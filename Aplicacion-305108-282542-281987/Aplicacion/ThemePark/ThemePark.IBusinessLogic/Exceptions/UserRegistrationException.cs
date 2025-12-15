using ThemePark.Exceptions;

namespace ThemePark.IBusinessLogic.Exceptions;

public sealed class UserRegistrationException(string email) : BaseCustomException($"Ya existe un usuario registrado con el email: {email}", $"User with email '{email}' already exists")
{
    public static UserRegistrationException EmailInUse(string email)
        => new($"El email {email} ya está en uso por otro usuario");
}
