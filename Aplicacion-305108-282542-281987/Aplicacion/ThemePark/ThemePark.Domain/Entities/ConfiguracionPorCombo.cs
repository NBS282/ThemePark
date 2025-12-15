using ThemePark.Enums;

namespace ThemePark.Entities;

public class ConfiguracionPorCombo : Configuracion
{
    public override TipoEstrategia? Tipo => TipoEstrategia.PuntuacionCombo;

    public int VentanaTemporalMinutos { get; set; }
    public int BonusMultiplicador { get; set; }
    public int MinimoAtracciones { get; set; }

    public ConfiguracionPorCombo(int ventanaTemporalMinutos, int bonusMultiplicador, int minimoAtracciones)
    {
        VentanaTemporalMinutos = ventanaTemporalMinutos;
        BonusMultiplicador = bonusMultiplicador;
        MinimoAtracciones = minimoAtracciones;
    }

    public ConfiguracionPorCombo()
    {
    }
}
