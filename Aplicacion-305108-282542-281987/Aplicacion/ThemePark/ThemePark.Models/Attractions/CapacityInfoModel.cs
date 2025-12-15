using ThemePark.Entities;

namespace ThemePark.Models.Attractions;

public class CapacityInfoModel
{
    public int CapacidadMaxima { get; set; }
    public int AforoActual { get; set; }
    public int EspaciosDisponibles { get; set; }
    public double PorcentajeOcupacion { get; set; }
    public bool TieneIncidencia { get; set; }

    public static CapacityInfoModel FromAttraction(Attraction attraction)
    {
        return new CapacityInfoModel
        {
            CapacidadMaxima = attraction.CapacidadMaxima,
            AforoActual = attraction.AforoActual,
            EspaciosDisponibles = attraction.CapacidadMaxima - attraction.AforoActual,
            PorcentajeOcupacion = Math.Round((double)attraction.AforoActual / attraction.CapacidadMaxima * 100, 2),
            TieneIncidencia = attraction.TieneIncidenciaActiva
        };
    }
}
