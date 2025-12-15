using ThemePark.Entities;
using ThemePark.Enums;
using ThemePark.Exceptions;
namespace ThemePark.TestDomain;

[TestClass]
public class ScoringStrategyTests
{
    private ScoringStrategy _scoringStrategy = null!;

    [TestInitialize]
    public void TestInitialize()
    {
        _scoringStrategy = new ScoringStrategy();
    }

    [TestCleanup]
    public void TestCleanup()
    {
        _scoringStrategy = null!;
    }

    [TestMethod]
    public void ScoringStrategy_ShouldInitializeWithEmptyName()
    {
        Assert.AreEqual(string.Empty, _scoringStrategy.Name);
    }

    [TestMethod]
    public void ScoringStrategy_ShouldSetValidName()
    {
        _scoringStrategy.Name = "Estrategia Verano";
        Assert.AreEqual("Estrategia Verano", _scoringStrategy.Name);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidUserDataException))]
    public void ScoringStrategy_ShouldThrowArgumentException_WhenNameIsEmpty()
    {
        _scoringStrategy.Name = string.Empty;
    }

    [TestMethod]
    public void ScoringStrategy_ShouldSetValidDescription()
    {
        _scoringStrategy.Description = "Estrategia especial para eventos";
        Assert.AreEqual("Estrategia especial para eventos", _scoringStrategy.Description);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidUserDataException))]
    public void ScoringStrategy_ShouldThrowArgumentException_WhenDescriptionIsEmpty()
    {
        _scoringStrategy.Description = string.Empty;
    }

    [TestMethod]
    public void ScoringStrategy_ShouldSetValidConfiguracionTyped()
    {
        var configuracion = new ConfiguracionPorAtraccion(new Dictionary<string, int> { ["montaña rusa"] = 100 });
        _scoringStrategy.ConfiguracionTyped = configuracion;
        Assert.AreEqual(configuracion, _scoringStrategy.ConfiguracionTyped);
    }

    [TestMethod]
    public void ScoringStrategy_ShouldSetValidActive()
    {
        _scoringStrategy.Active = true;
        Assert.AreEqual(true, _scoringStrategy.Active);
    }

    [TestMethod]
    public void ScoringStrategy_ShouldSetValidType()
    {
        _scoringStrategy.Type = TipoEstrategia.PuntuacionCombo;
        Assert.AreEqual(TipoEstrategia.PuntuacionCombo, _scoringStrategy.Type);
    }

    [TestMethod]
    public void ScoringStrategy_ShouldInitializeWithDefaultId()
    {
        Assert.AreEqual(Guid.Empty, _scoringStrategy.Id);
    }

    [TestMethod]
    public void ScoringStrategy_ShouldInitializeWithCreatedDate()
    {
        Assert.IsTrue(_scoringStrategy.CreatedDate > DateTime.MinValue);
    }

    [TestMethod]
    public void ScoringStrategy_ShouldSetBasicProperties_WhenCreatedWithValidParameters()
    {
        var configuracion = new ConfiguracionPorCombo(15, 2, 3);
        var strategy = new ScoringStrategy("Estrategia Test", TipoEstrategia.PuntuacionCombo, "Descripción test", configuracion);

        Assert.AreEqual("Estrategia Test", strategy.Name);
        Assert.AreEqual(TipoEstrategia.PuntuacionCombo, strategy.Type);
        Assert.AreEqual("Descripción test", strategy.Description);
    }

    [TestMethod]
    public void ScoringStrategy_ShouldSetConfigurationAndState_WhenCreatedWithValidParameters()
    {
        var configuracion = new ConfiguracionPorCombo(15, 2, 3);
        var strategy = new ScoringStrategy("Estrategia Test", TipoEstrategia.PuntuacionCombo, "Descripción test", configuracion);

        Assert.AreEqual(configuracion, strategy.ConfiguracionTyped);
        Assert.AreEqual(false, strategy.Active);
    }

    [TestMethod]
    public void ScoringStrategy_ShouldSetGeneratedValues_WhenCreatedWithValidParameters()
    {
        var configuracion = new ConfiguracionPorCombo(15, 2, 3);
        var strategy = new ScoringStrategy("Estrategia Test", TipoEstrategia.PuntuacionCombo, "Descripción test", configuracion);

        Assert.AreEqual(Guid.Empty, strategy.Id);
        Assert.IsTrue(strategy.CreatedDate > DateTime.MinValue);
    }

    [TestMethod]
    public void ScoringStrategy_ShouldSetValidConfiguracionObject()
    {
        var configuracion = new ConfiguracionPorCombo(15, 2, 3);
        _scoringStrategy.ConfiguracionTyped = configuracion;
        Assert.AreEqual(configuracion, _scoringStrategy.ConfiguracionTyped);
    }
}
