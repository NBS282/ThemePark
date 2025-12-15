using ThemePark.Entities;
using ThemePark.Enums;

namespace ThemePark.TestDomain;

[TestClass]
public class ConfiguracionTests
{
    [TestMethod]
    public void Configuracion_ShouldBeAbstractClass()
    {
        var configuracionType = typeof(Configuracion);

        Assert.IsTrue(configuracionType.IsAbstract, "Configuracion should be an abstract class");
    }

    [TestMethod]
    public void Configuracion_ShouldHaveTipoProperty()
    {
        var configuracionType = typeof(Configuracion);
        var tipoProperty = configuracionType.GetProperty("Tipo");

        Assert.IsNotNull(tipoProperty, "Configuracion should have a Tipo property");
        Assert.AreEqual(typeof(TipoEstrategia?), tipoProperty.PropertyType, "Tipo property should be of type TipoEstrategia");
        Assert.IsTrue(tipoProperty.GetGetMethod()?.IsAbstract == true, "Tipo property should be abstract");
    }

    [TestMethod]
    public void ConfiguracionPorAtraccion_ShouldInheritFromConfiguracion()
    {
        var configuracionPorAtraccionType = typeof(ConfiguracionPorAtraccion);

        Assert.IsTrue(configuracionPorAtraccionType.IsSubclassOf(typeof(Configuracion)), "ConfiguracionPorAtraccion should inherit from Configuracion");
    }

    [TestMethod]
    public void ConfiguracionPorAtraccion_ShouldHaveValoresProperty()
    {
        var configuracionPorAtraccionType = typeof(ConfiguracionPorAtraccion);
        var valoresProperty = configuracionPorAtraccionType.GetProperty("Valores");

        Assert.IsNotNull(valoresProperty, "ConfiguracionPorAtraccion should have a Valores property");
        Assert.AreEqual(typeof(Dictionary<string, int>), valoresProperty.PropertyType, "Valores property should be of type Dictionary<string, int>");
    }

    [TestMethod]
    public void ConfiguracionPorAtraccion_Constructor_ShouldSetValores()
    {
        var valores = new Dictionary<string, int> { { "montaña rusa", 100 }, { "simulador", 50 } };

        var config = new ConfiguracionPorAtraccion(valores);

        Assert.AreEqual(valores, config.Valores);
    }

    [TestMethod]
    public void ConfiguracionPorEvento_ShouldInheritFromConfiguracion()
    {
        var configuracionPorEventoType = typeof(ConfiguracionPorEvento);

        Assert.IsTrue(configuracionPorEventoType.IsSubclassOf(typeof(Configuracion)), "ConfiguracionPorEvento should inherit from Configuracion");
    }

    [TestMethod]
    public void ConfiguracionPorCombo_ShouldInheritFromConfiguracion()
    {
        var configuracionPorComboType = typeof(ConfiguracionPorCombo);

        Assert.IsTrue(configuracionPorComboType.IsSubclassOf(typeof(Configuracion)), "ConfiguracionPorCombo should inherit from Configuracion");
    }

    [TestMethod]
    public void ConfiguracionPorEvento_ShouldHavePuntosProperty()
    {
        var configuracionPorEventoType = typeof(ConfiguracionPorEvento);
        var puntosProperty = configuracionPorEventoType.GetProperty("Puntos");

        Assert.IsNotNull(puntosProperty, "ConfiguracionPorEvento should have a Puntos property");
        Assert.AreEqual(typeof(int), puntosProperty.PropertyType, "Puntos property should be of type int");
    }

    [TestMethod]
    public void ConfiguracionPorEvento_ShouldHaveEventoProperty()
    {
        var configuracionPorEventoType = typeof(ConfiguracionPorEvento);
        var eventoProperty = configuracionPorEventoType.GetProperty("Evento");

        Assert.IsNotNull(eventoProperty, "ConfiguracionPorEvento should have an Evento property");
        Assert.AreEqual(typeof(string), eventoProperty.PropertyType, "Evento property should be of type string");
    }

    [TestMethod]
    public void ConfiguracionPorEvento_Constructor_ShouldSetProperties()
    {
        var puntos = 100;
        var evento = "Noche de Dinosaurios";

        var config = new ConfiguracionPorEvento(puntos, evento);

        Assert.AreEqual(puntos, config.Puntos);
        Assert.AreEqual(evento, config.Evento);
    }

    [TestMethod]
    public void ConfiguracionPorCombo_ShouldHaveVentanaTemporalProperty()
    {
        var configuracionPorComboType = typeof(ConfiguracionPorCombo);
        var ventanaTemporalProperty = configuracionPorComboType.GetProperty("VentanaTemporalMinutos");

        Assert.IsNotNull(ventanaTemporalProperty, "ConfiguracionPorCombo should have a VentanaTemporalMinutos property");
        Assert.AreEqual(typeof(int), ventanaTemporalProperty.PropertyType, "VentanaTemporalMinutos property should be of type int");
    }

    [TestMethod]
    public void ConfiguracionPorCombo_ShouldHaveBonusMultiplicadorProperty()
    {
        var configuracionPorComboType = typeof(ConfiguracionPorCombo);
        var bonusMultiplicadorProperty = configuracionPorComboType.GetProperty("BonusMultiplicador");

        Assert.IsNotNull(bonusMultiplicadorProperty, "ConfiguracionPorCombo should have a BonusMultiplicador property");
        Assert.AreEqual(typeof(int), bonusMultiplicadorProperty.PropertyType, "BonusMultiplicador property should be of type int");
    }

    [TestMethod]
    public void ConfiguracionPorCombo_ShouldHaveMinimoAtraccionesProperty()
    {
        var configuracionPorComboType = typeof(ConfiguracionPorCombo);
        var minimoAtraccionesProperty = configuracionPorComboType.GetProperty("MinimoAtracciones");

        Assert.IsNotNull(minimoAtraccionesProperty, "ConfiguracionPorCombo should have a MinimoAtracciones property");
        Assert.AreEqual(typeof(int), minimoAtraccionesProperty.PropertyType, "MinimoAtracciones property should be of type int");
    }

    [TestMethod]
    public void ConfiguracionPorCombo_Constructor_ShouldSetProperties()
    {
        var ventanaTemporalMinutos = 15;
        var bonusMultiplicador = 2;
        var minimoAtracciones = 3;

        var config = new ConfiguracionPorCombo(ventanaTemporalMinutos, bonusMultiplicador, minimoAtracciones);

        Assert.AreEqual(ventanaTemporalMinutos, config.VentanaTemporalMinutos);
        Assert.AreEqual(bonusMultiplicador, config.BonusMultiplicador);
        Assert.AreEqual(minimoAtracciones, config.MinimoAtracciones);
    }

    [TestMethod]
    public void ConfiguracionPorCombo_DefaultConstructor_ShouldCreateInstance()
    {
        var config = new ConfiguracionPorCombo();

        Assert.IsNotNull(config);
        Assert.AreEqual(TipoEstrategia.PuntuacionCombo, config.Tipo);
    }

    [TestMethod]
    public void ConfiguracionPorEvento_DefaultConstructor_ShouldCreateInstance()
    {
        var config = new ConfiguracionPorEvento();

        Assert.IsNotNull(config);
        Assert.AreEqual(TipoEstrategia.PuntuacionPorEvento, config.Tipo);
    }

    [TestMethod]
    public void ConfiguracionPorAtraccion_Tipo_ShouldReturnPuntuacionPorAtraccion()
    {
        var valores = new Dictionary<string, int> { { "montaña rusa", 100 } };
        var config = new ConfiguracionPorAtraccion(valores);

        Assert.AreEqual(TipoEstrategia.PuntuacionPorAtraccion, config.Tipo);
    }
}
