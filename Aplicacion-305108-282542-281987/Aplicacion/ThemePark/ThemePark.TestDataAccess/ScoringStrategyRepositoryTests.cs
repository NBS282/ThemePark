using Microsoft.EntityFrameworkCore;
using ThemePark.DataAccess;
using ThemePark.DataAccess.Repositories;
using ThemePark.Entities;
using ThemePark.Enums;
using ThemePark.IDataAccess;

namespace ThemePark.TestDataAccess;

[TestClass]
public class ScoringStrategyRepositoryTests
{
    private ThemeParkDbContext _context = null!;
    private IScoringStrategyRepository _repository = null!;
    private ScoringStrategy _testStrategy = null!;

    [TestInitialize]
    public void TestInitialize()
    {
        var options = new DbContextOptionsBuilder<ThemeParkDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ThemeParkDbContext(options);
        _repository = new ScoringStrategyRepository(_context);

        var configuracion = new ConfiguracionPorAtraccion(new Dictionary<string, int> { ["montaña rusa"] = 100 });
        _testStrategy = new ScoringStrategy(
            "Estrategia Test",
            TipoEstrategia.PuntuacionPorAtraccion,
            "Descripción test",
            configuracion);
    }

    [TestCleanup]
    public void TestCleanup()
    {
        _context.Dispose();
    }

    [TestMethod]
    public void Add_ShouldAddStrategyToDatabase()
    {
        var result = _repository.Add(_testStrategy);

        Assert.AreEqual(_testStrategy, result);
        Assert.AreEqual(1, _context.ScoringStrategies.Count());
    }

    [TestMethod]
    public void Add_ShouldSaveStrategyWithCorrectProperties()
    {
        _ = _repository.Add(_testStrategy);

        var savedStrategy = _context.ScoringStrategies.First();
        Assert.AreEqual(_testStrategy.Name, savedStrategy.Name);
        Assert.AreEqual(_testStrategy.Type, savedStrategy.Type);
    }

    [TestMethod]
    public void Add_ShouldAddStrategyWithConfiguracionTyped()
    {
        var configuracion = new ConfiguracionPorCombo(15, 2, 3);
        var strategy = new ScoringStrategy("Test Strategy", TipoEstrategia.PuntuacionCombo, "Test Description", configuracion);

        var result = _repository.Add(strategy);
        _context.SaveChanges();

        Assert.AreEqual(strategy, result);
        var savedStrategy = _context.ScoringStrategies.Include(s => s.ConfiguracionTyped).First();
        Assert.IsNotNull(savedStrategy.ConfiguracionTyped);
    }

    [TestMethod]
    public void Add_ShouldSaveConfiguracionTypedWithCorrectType()
    {
        var configuracion = new ConfiguracionPorCombo(15, 2, 3);
        var strategy = new ScoringStrategy("Test Strategy", TipoEstrategia.PuntuacionCombo, "Test Description", configuracion);

        var result = _repository.Add(strategy);
        _context.SaveChanges();

        var savedStrategy = _context.ScoringStrategies.Include(s => s.ConfiguracionTyped).First();
        Assert.IsInstanceOfType(savedStrategy.ConfiguracionTyped, typeof(ConfiguracionPorCombo));
    }

    [TestMethod]
    public void Add_ShouldSaveConfiguracionTypedWithCorrectValues()
    {
        var configuracion = new ConfiguracionPorCombo(15, 2, 3);
        var strategy = new ScoringStrategy("Test Strategy", TipoEstrategia.PuntuacionCombo, "Test Description", configuracion);

        var result = _repository.Add(strategy);
        _context.SaveChanges();

        var savedStrategy = _context.ScoringStrategies.Include(s => s.ConfiguracionTyped).First();
        var savedConfig = (ConfiguracionPorCombo)savedStrategy.ConfiguracionTyped;
        Assert.AreEqual(15, savedConfig.VentanaTemporalMinutos);
        Assert.AreEqual(2, savedConfig.BonusMultiplicador);
        Assert.AreEqual(3, savedConfig.MinimoAtracciones);
    }

    [TestMethod]
    public void GetAll_ShouldReturnAllStrategiesFromDatabase()
    {
        _context.ScoringStrategies.Add(_testStrategy);
        var configuracionCombo = new ConfiguracionPorCombo(10, 50, 2);
        var secondStrategy = new ScoringStrategy(
            "Segunda Estrategia",
            TipoEstrategia.PuntuacionCombo,
            "Segunda descripción",
            configuracionCombo);
        _context.ScoringStrategies.Add(secondStrategy);
        _context.SaveChanges();

        var result = _repository.GetAll();

        Assert.AreEqual(2, result.Count);
        Assert.IsTrue(result.Any(s => s.Name == "Estrategia Test"));
        Assert.IsTrue(result.Any(s => s.Name == "Segunda Estrategia"));
    }

    [TestMethod]
    public void GetActiveStrategy_ShouldReturnActiveStrategyFromDatabase()
    {
        var configInactiva = new ConfiguracionPorAtraccion(new Dictionary<string, int> { ["montaña rusa"] = 50 });
        var inactiveStrategy = new ScoringStrategy(
            "Estrategia Inactiva",
            TipoEstrategia.PuntuacionPorAtraccion,
            "Descripción inactiva",
            configInactiva)
        {
            Active = false
        };
        var configActiva = new ConfiguracionPorCombo(15, 75, 3);
        var activeStrategy = new ScoringStrategy(
            "Estrategia Activa",
            TipoEstrategia.PuntuacionCombo,
            "Descripción activa",
            configActiva)
        {
            Active = true
        };

        _context.ScoringStrategies.Add(inactiveStrategy);
        _context.ScoringStrategies.Add(activeStrategy);
        _context.SaveChanges();

        var result = _repository.GetActiveStrategy();

        Assert.IsNotNull(result);
        Assert.AreEqual("Estrategia Activa", result.Name);
        Assert.IsTrue(result.Active);
    }

    [TestMethod]
    public void GetActiveStrategy_ShouldReturnNull_WhenNoActiveStrategyExists()
    {
        var configInactiva2 = new ConfiguracionPorAtraccion(new Dictionary<string, int> { ["montaña rusa"] = 50 });
        var inactiveStrategy = new ScoringStrategy(
            "Estrategia Inactiva",
            TipoEstrategia.PuntuacionPorAtraccion,
            "Descripción inactiva",
            configInactiva2)
        {
            Active = false
        };

        _context.ScoringStrategies.Add(inactiveStrategy);
        _context.SaveChanges();

        var result = _repository.GetActiveStrategy();

        Assert.IsNull(result);
    }

    [TestMethod]
    public void Update_ShouldUpdateExistingStrategyInDatabase()
    {
        _context.ScoringStrategies.Add(_testStrategy);
        _context.SaveChanges();

        _testStrategy.Description = "Descripción actualizada";
        var nuevaConfiguracion = new ConfiguracionPorAtraccion(new Dictionary<string, int> { ["montaña rusa"] = 200 });
        _testStrategy.ConfiguracionTyped = nuevaConfiguracion;
        _testStrategy.Active = true;

        var result = _repository.Update(_testStrategy);

        Assert.AreEqual(_testStrategy, result);
        Assert.AreEqual("Descripción actualizada", result.Description);
        Assert.IsInstanceOfType(result.ConfiguracionTyped, typeof(ConfiguracionPorAtraccion));
    }

    [TestMethod]
    public void Update_ShouldUpdateActiveStatus()
    {
        _context.ScoringStrategies.Add(_testStrategy);
        _context.SaveChanges();

        _testStrategy.Description = "Descripción actualizada";
        var nuevaConfiguracion = new ConfiguracionPorAtraccion(new Dictionary<string, int> { ["montaña rusa"] = 200 });
        _testStrategy.ConfiguracionTyped = nuevaConfiguracion;
        _testStrategy.Active = true;

        var result = _repository.Update(_testStrategy);

        Assert.IsTrue(result.Active);
    }

    [TestMethod]
    public void Update_ShouldPersistChangesInDatabase()
    {
        _context.ScoringStrategies.Add(_testStrategy);
        _context.SaveChanges();

        _testStrategy.Description = "Descripción actualizada";
        var nuevaConfiguracion = new ConfiguracionPorAtraccion(new Dictionary<string, int> { ["montaña rusa"] = 200 });
        _testStrategy.ConfiguracionTyped = nuevaConfiguracion;
        _testStrategy.Active = true;

        var result = _repository.Update(_testStrategy);

        var updatedInDb = _context.ScoringStrategies.First(s => s.Id == _testStrategy.Id);
        Assert.AreEqual("Descripción actualizada", updatedInDb.Description);
        Assert.IsTrue(updatedInDb.Active);
    }

    [TestMethod]
    public void GetById_ShouldReturnStrategyWithMatchingId()
    {
        _context.ScoringStrategies.Add(_testStrategy);
        _context.SaveChanges();

        var result = _repository.GetById(_testStrategy.Id);

        Assert.AreEqual(_testStrategy.Id, result.Id);
        Assert.AreEqual(_testStrategy.Name, result.Name);
        Assert.AreEqual(_testStrategy.Type, result.Type);
    }

    [TestMethod]
    public void Delete_ShouldRemoveStrategyFromDatabase()
    {
        _context.ScoringStrategies.Add(_testStrategy);
        _context.SaveChanges();

        Assert.AreEqual(1, _context.ScoringStrategies.Count());

        var result = _repository.Delete(_testStrategy.Id);

        Assert.IsTrue(result);
        Assert.AreEqual(0, _context.ScoringStrategies.Count());
    }

    [TestMethod]
    public void ExistsByName_ShouldReturnTrue_WhenStrategyWithNameExists()
    {
        _context.ScoringStrategies.Add(_testStrategy);
        _context.SaveChanges();

        var result = _repository.ExistsByName("Estrategia Test");

        Assert.IsTrue(result);
    }

    [TestMethod]
    public void ExistsByName_ShouldReturnFalse_WhenStrategyWithNameDoesNotExist()
    {
        var result = _repository.ExistsByName("Estrategia Inexistente");

        Assert.IsFalse(result);
    }

    [TestMethod]
    public void Add_ShouldHandlePluginStrategy_WithoutSavingTypedConfiguration()
    {
        var config = new ConfiguracionPorAtraccion(new Dictionary<string, int> { ["test"] = 100 });
        var pluginStrategy = new ScoringStrategy(
            "Plugin Test",
            "PuntuacionPorHora",
            "Test plugin",
            config,
            "{\"HourMultipliers\":{\"18\":2.0}}");

        var result = _repository.Add(pluginStrategy);

        Assert.IsNotNull(result);
        Assert.AreEqual("PuntuacionPorHora", result.PluginTypeIdentifier);
        Assert.IsNotNull(result.ConfiguracionTyped);

        var savedStrategy = _context.ScoringStrategies.First(s => s.Id == result.Id);
        Assert.AreEqual("PuntuacionPorHora", savedStrategy.PluginTypeIdentifier);
        Assert.AreEqual("{\"HourMultipliers\":{\"18\":2.0}}", savedStrategy.ConfigurationJson);
    }

    [TestMethod]
    public void Update_ShouldHandlePluginStrategy_PreservingConfigurationInMemory()
    {
        var config = new ConfiguracionPorAtraccion(new Dictionary<string, int> { ["test"] = 100 });
        var pluginStrategy = new ScoringStrategy(
            "Plugin Test",
            "PuntuacionPorHora",
            "Test plugin",
            config,
            "{\"HourMultipliers\":{\"18\":2.0}}");

        _context.ScoringStrategies.Add(pluginStrategy);
        _context.SaveChanges();

        var updatedConfig = new ConfiguracionPorAtraccion(new Dictionary<string, int> { ["test"] = 200 });
        pluginStrategy.ConfiguracionTyped = updatedConfig;
        pluginStrategy.ConfigurationJson = "{\"HourMultipliers\":{\"18\":3.0}}";
        pluginStrategy.Description = "Updated plugin";

        var result = _repository.Update(pluginStrategy);

        Assert.IsNotNull(result);
        Assert.AreEqual("Updated plugin", result.Description);
        Assert.IsNotNull(result.ConfiguracionTyped, "ConfiguracionTyped should be preserved in memory");
        Assert.AreEqual("{\"HourMultipliers\":{\"18\":3.0}}", result.ConfigurationJson);
    }

    [TestMethod]
    public void Update_ShouldHandleNativeStrategy_PersistingConfiguration()
    {
        _context.ScoringStrategies.Add(_testStrategy);
        _context.SaveChanges();

        var newConfig = new ConfiguracionPorAtraccion(new Dictionary<string, int> { ["test"] = 150 });
        _testStrategy.ConfiguracionTyped = newConfig;
        _testStrategy.Description = "Updated native";

        var result = _repository.Update(_testStrategy);

        Assert.IsNotNull(result);
        Assert.AreEqual("Updated native", result.Description);
        Assert.IsNotNull(result.ConfiguracionTyped);
    }

    [TestMethod]
    public void DeactivateStrategiesByPluginTypeIdentifier_ShouldDeactivateMatchingStrategies()
    {
        var config1 = new ConfiguracionPorAtraccion(new Dictionary<string, int> { ["test"] = 100 });
        var pluginStrategy1 = new ScoringStrategy(
            "Plugin Test 1",
            "PuntuacionPorHora",
            "Test plugin 1",
            config1,
            "{\"HourMultipliers\":{\"18\":2.0}}")
        {
            Active = true
        };

        var config2 = new ConfiguracionPorAtraccion(new Dictionary<string, int> { ["test"] = 100 });
        var pluginStrategy2 = new ScoringStrategy(
            "Plugin Test 2",
            "PuntuacionPorHora",
            "Test plugin 2",
            config2,
            "{\"HourMultipliers\":{\"19\":2.0}}")
        {
            Active = false
        };

        var config3 = new ConfiguracionPorAtraccion(new Dictionary<string, int> { ["test"] = 100 });
        var otherPluginStrategy = new ScoringStrategy(
            "Other Plugin",
            "OtroPlugin",
            "Other plugin",
            config3,
            "{}")
        {
            Active = true
        };

        _context.ScoringStrategies.Add(pluginStrategy1);
        _context.ScoringStrategies.Add(pluginStrategy2);
        _context.ScoringStrategies.Add(otherPluginStrategy);
        _context.SaveChanges();

        _repository.DeactivateStrategiesByPluginTypeIdentifier("PuntuacionPorHora");

        var strategies = _context.ScoringStrategies.ToList();
        var deactivated1 = strategies.First(s => s.Name == "Plugin Test 1");
        var deactivated2 = strategies.First(s => s.Name == "Plugin Test 2");
        var other = strategies.First(s => s.Name == "Other Plugin");

        Assert.IsFalse(deactivated1.Active, "Plugin Test 1 should be deactivated");
        Assert.IsFalse(deactivated2.Active, "Plugin Test 2 should remain inactive");
        Assert.IsTrue(other.Active, "Other plugin should remain active");
    }

    [TestMethod]
    public void DeactivateStrategiesByPluginTypeIdentifier_ShouldNotAffectNativeStrategies()
    {
        _testStrategy.Active = true;
        _context.ScoringStrategies.Add(_testStrategy);
        _context.SaveChanges();

        _repository.DeactivateStrategiesByPluginTypeIdentifier("PuntuacionPorHora");

        var result = _context.ScoringStrategies.First(s => s.Id == _testStrategy.Id);
        Assert.IsTrue(result.Active, "Native strategy should not be affected");
    }

    [TestMethod]
    public void DeactivateStrategiesByPluginTypeIdentifier_WhenNoMatches_ShouldNotThrow()
    {
        _context.ScoringStrategies.Add(_testStrategy);
        _context.SaveChanges();

        _repository.DeactivateStrategiesByPluginTypeIdentifier("NonExistentPlugin");

        Assert.AreEqual(1, _context.ScoringStrategies.Count());
    }
}
