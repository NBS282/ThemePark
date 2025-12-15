namespace ThemePark.Models.Attractions;

public class ValidateTicketAndRegisterAccessRequest
{
    public string CodigoIdentificacion { get; set; } = string.Empty;

    public string ToEntity() => CodigoIdentificacion;
}
