namespace ThemePark.Exceptions;

public sealed class UnsupportedConfigurationTypeException(string configurationType) : BaseCustomException($"Tipo de configuración no soportado: {configurationType}", $"El tipo de configuración '{configurationType}' no es válido")
{
}
