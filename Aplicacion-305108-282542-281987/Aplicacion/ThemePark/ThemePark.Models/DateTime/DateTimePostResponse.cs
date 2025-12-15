namespace ThemePark.Models.DateTime;

public class DateTimePostResponse(string fechaHora, string mensaje)
{
    public string FechaHora { get; set; } = fechaHora;
    public string Mensaje { get; set; } = mensaje;
}
