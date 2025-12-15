namespace ThemePark.Models.Attractions;

public class RegisterVisitByNFCRequest
{
    public string CodigoIdentificacion { get; set; } = string.Empty;

    public string ToEntity() => CodigoIdentificacion;
}
