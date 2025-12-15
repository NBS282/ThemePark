namespace ThemePark.Exceptions;

public sealed class InvalidTimeFormatException(string value) : BaseCustomException($"El formato de hora '{value}' es inválido. Use el formato HH:mm o HH:mm:ss (ejemplo: 18:00 o 18:30:45)",
           $"Invalid time format: '{value}'. Use HH:mm or HH:mm:ss format (example: 18:00 or 18:30:45)")
{
}
