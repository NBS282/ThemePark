namespace ThemePark.Exceptions;

public sealed class InvalidMaintenanceDataException : BaseCustomException
{
    public InvalidMaintenanceDataException(string fieldName, string value)
        : base($"Los datos del mantenimiento son inválidos: {fieldName} no puede ser '{value}'",
               $"Invalid maintenance data: {fieldName} cannot be '{value}'")
    {
    }

    public InvalidMaintenanceDataException(string fieldName)
        : base($"Los datos del mantenimiento son inválidos: {fieldName} es requerido",
               $"Invalid maintenance data: {fieldName} is required")
    {
    }

    public InvalidMaintenanceDataException(string fieldName, string value, string constraint)
        : base($"Los datos del mantenimiento son inválidos: {fieldName} {constraint}, valor proporcionado: '{value}'",
               $"Invalid maintenance data: {fieldName} {constraint}, provided value: '{value}'")
    {
    }
}
