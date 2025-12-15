using System.Globalization;
using ThemePark.Exceptions;

namespace ThemePark;

public static class DateParser
{
    public static DateTime ParseDate(string dateString, string fieldName = "Fecha")
    {
        string[] formatosAceptados = ["d/M/yyyy", "yyyy-MM-dd"];

        if(!DateTime.TryParseExact(
            dateString,
            formatosAceptados,
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out var fecha))
        {
            throw new InvalidRequestDataException($"El campo '{fieldName}' tiene un formato inválido. Debe ser dd/MM/yyyy o yyyy-MM-dd");
        }

        return fecha;
    }

    public static DateTime ParseDateTime(string dateTimeString, string fieldName = "FechaHora")
    {
        string[] formatosAceptados = ["yyyy-MM-ddTHH:mm", "yyyy-M-dd HH:mm", "d/M/yyyy HH:mm"];

        if(!DateTime.TryParseExact(
            dateTimeString,
            formatosAceptados,
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out var fechaHora))
        {
            throw new InvalidRequestDataException($"El campo '{fieldName}' tiene un formato inválido. Debe ser yyyy-MM-ddTHH:mm, yyyy-M-dd HH:mm o dd/MM/yyyy HH:mm");
        }

        return fechaHora;
    }
}
