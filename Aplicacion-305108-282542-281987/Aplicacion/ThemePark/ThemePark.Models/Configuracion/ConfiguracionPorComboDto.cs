using System.ComponentModel.DataAnnotations;
using ThemePark.Models.Enums;

namespace ThemePark.Models.Configuracion;

public class ConfiguracionPorComboDto : ConfiguracionDto
{
    public override TipoAlgoritmoDto Tipo => TipoAlgoritmoDto.PuntuacionCombo;

    [Required(ErrorMessage = "El campo 'ventanaTemporalMinutos' es requerido")]
    public int? VentanaTemporalMinutos { get; set; }

    [Required(ErrorMessage = "El campo 'bonusMultiplicador' es requerido")]
    public int? BonusMultiplicador { get; set; }

    [Required(ErrorMessage = "El campo 'minimoAtracciones' es requerido")]
    public int? MinimoAtracciones { get; set; }

    public ConfiguracionPorComboDto()
    {
    }

    public ConfiguracionPorComboDto(int ventanaTemporalMinutos, int bonusMultiplicador, int minimoAtracciones)
    {
        VentanaTemporalMinutos = ventanaTemporalMinutos;
        BonusMultiplicador = bonusMultiplicador;
        MinimoAtracciones = minimoAtracciones;
    }

    public override Entities.Configuracion ToEntity()
    {
        return new Entities.ConfiguracionPorCombo(VentanaTemporalMinutos!.Value, BonusMultiplicador!.Value, MinimoAtracciones!.Value);
    }

    public static ConfiguracionPorComboDto FromEntity(Entities.ConfiguracionPorCombo entity)
    {
        return new ConfiguracionPorComboDto(entity.VentanaTemporalMinutos, entity.BonusMultiplicador, entity.MinimoAtracciones);
    }
}
