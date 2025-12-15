using ThemePark.Exceptions;

namespace ThemePark.IBusinessLogic.Exceptions;

public sealed class DateTimeBusinessLogicException(string message) : BaseCustomException(message, $"DateTime business logic error: {message}")
{
    public static DateTimeBusinessLogicException NullOrEmptyDateTime()
        => new("La fecha y hora no puede ser nula o vacía");

    public static DateTimeBusinessLogicException InvalidFormat()
        => new("Formato de fecha y hora inválido. Formato esperado: yyyy-MM-ddTHH:mm o yyyy-M-dd HH:mm");

    public static DateTimeBusinessLogicException CannotSetPastDate()
        => new("No se puede establecer una fecha anterior a la fecha actual del sistema");
}
