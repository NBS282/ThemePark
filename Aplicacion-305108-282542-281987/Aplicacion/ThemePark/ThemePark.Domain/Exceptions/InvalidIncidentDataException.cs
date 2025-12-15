namespace ThemePark.Exceptions;

public sealed class InvalidIncidentDataException(string fieldName) : BaseCustomException($"Los datos del incidente son inválidos: {fieldName} es requerido",
           $"Invalid incident data: {fieldName} is required")
{
}
