namespace ThemePark.Exceptions;

public sealed class InvalidUserDataException : BaseCustomException
{
    public InvalidUserDataException(string fieldName, string value)
        : base($"Los datos del usuario son inválidos: {fieldName} no puede ser '{value}'",
               $"Invalid user data: {fieldName} cannot be '{value}'")
    {
    }

    public InvalidUserDataException(string fieldName)
        : base($"Los datos del usuario son inválidos: {fieldName} es requerido",
               $"Invalid user data: {fieldName} is required")
    {
    }
}
