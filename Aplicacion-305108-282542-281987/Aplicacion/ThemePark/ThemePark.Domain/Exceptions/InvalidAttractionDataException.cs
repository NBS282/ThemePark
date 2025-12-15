namespace ThemePark.Exceptions;

public sealed class InvalidAttractionDataException : BaseCustomException
{
    public InvalidAttractionDataException(string fieldName, string value)
        : base($"Los datos de la atracción son inválidos: {fieldName} no puede ser '{value}'",
               $"Invalid attraction data: {fieldName} cannot be '{value}'")
    {
    }

    public InvalidAttractionDataException(string fieldName)
        : base($"Los datos de la atracción son inválidos: {fieldName} es requerido",
               $"Invalid attraction data: {fieldName} is required")
    {
    }
}
