namespace ThemePark.Models.DateTime;

public class DateTimeResponse(string fechaHora)
{
    public string FechaHora { get; set; } = fechaHora;
}
