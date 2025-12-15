using ThemePark.Entities;

namespace ThemePark.Models.Attractions;

public class VisitResponseModel
{
    public string Id { get; set; } = string.Empty;
    public string IdUsuario { get; set; } = string.Empty;
    public string NombreAtraccion { get; set; } = string.Empty;
    public string HoraEntrada { get; set; } = string.Empty;
    public string? HoraSalida { get; set; }
    public bool EstaActiva { get; set; }
    public int PuntosObtenidos { get; set; }

    public static VisitResponseModel FromVisit(Visit visit)
    {
        return new VisitResponseModel
        {
            Id = visit.Id.ToString(),
            IdUsuario = visit.UserId.ToString(),
            NombreAtraccion = visit.AttractionName,
            HoraEntrada = visit.EntryTime.ToString("yyyy-MM-ddTHH:mm:ss"),
            HoraSalida = visit.ExitTime?.ToString("yyyy-MM-ddTHH:mm:ss"),
            EstaActiva = visit.IsActive,
            PuntosObtenidos = visit.Points
        };
    }
}
