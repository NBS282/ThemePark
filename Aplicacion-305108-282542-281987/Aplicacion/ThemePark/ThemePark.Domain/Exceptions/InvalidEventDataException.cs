namespace ThemePark.Exceptions;

public sealed class InvalidEventDataException : BaseCustomException
{
    public InvalidEventDataException(string fieldName, string value)
        : base($"Los datos del evento son inválidos: {fieldName} no puede ser '{value}'",
               $"Invalid event data: {fieldName} cannot be '{value}'")
    {
    }

    public InvalidEventDataException(string fieldName)
        : base($"Los datos del evento son inválidos: {fieldName} es requerido",
               $"Invalid event data: {fieldName} is required")
    {
    }
}
