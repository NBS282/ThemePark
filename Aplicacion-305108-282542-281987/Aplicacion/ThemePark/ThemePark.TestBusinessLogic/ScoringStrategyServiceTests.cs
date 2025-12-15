using Moq;
using ThemePark.BusinessLogic.ScoringStrategy;
using ThemePark.Entities;
using ThemePark.Enums;
using ThemePark.IBusinessLogic;
using ThemePark.IBusinessLogic.Exceptions;
using ThemePark.IDataAccess;
using ThemePark.IDataAccess.Exceptions;
using ScoringStrategyEntity = ThemePark.Entities.ScoringStrategy;

namespace ThemePark.TestBusinessLogic;

[TestClass]
public class ScoringStrategyServiceTests
{
    private Mock<IScoringStrategyRepository> _mockRepository = null!;
    private Mock<IScoringAlgorithmFactory> _mockAlgorithmFactory = null!;
    private Mock<IPluginLoader> _mockPluginLoader = null!;
    private IScoringStrategyService _service = null!;
    private ScoringStrategyEntity _testStrategy = null!;

    [TestInitialize]
    public void TestInitialize()
    {
        _mockRepository = new Mock<IScoringStrategyRepository>(MockBehavior.Strict);
        _mockAlgorithmFactory = new Mock<IScoringAlgorithmFactory>(MockBehavior.Strict);
        _mockPluginLoader = new Mock<IPluginLoader>(MockBehavior.Strict);
        _mockPluginLoader.Setup(p => p.GetAllPlugins()).Returns([]);
        _service = new ScoringStrategyService(_mockRepository.Object, _mockAlgorithmFactory.Object, _mockPluginLoader.Object);

        var configuracion = new ConfiguracionPorAtraccion(new Dictionary<string, int> { ["montaña rusa"] = 100 });
        _testStrategy = new ScoringStrategyEntity(
            "Estrategia Test",
            TipoEstrategia.PuntuacionPorAtraccion,
            "Descripción test",
            configuracion);
    }

    [TestCleanup]
    public void TestCleanup()
    {
        _mockRepository.VerifyAll();
    }

    [TestMethod]
    public void GetAllStrategies_ShouldReturnAllStrategiesFromRepository()
    {
        var expectedStrategies = new List<ScoringStrategyEntity> { _testStrategy };
        _mockRepository.Setup(r => r.GetAll()).Returns(expectedStrategies);

        var result = _service.GetAllStrategies();

        Assert.AreEqual(expectedStrategies, result);
        Assert.AreEqual(1, result.Count);
        Assert.AreEqual(_testStrategy.Name, result[0].Name);
    }

    [TestMethod]
    public void GetByName_ShouldReturnSpecificStrategyFromRepository()
    {
        var strategyName = "Estrategia Test";
        var allStrategies = new List<ScoringStrategyEntity> { _testStrategy };
        _mockRepository.Setup(r => r.GetAll()).Returns(allStrategies);

        var result = _service.GetByName(strategyName);

        Assert.AreEqual(_testStrategy, result);
        Assert.AreEqual(strategyName, result.Name);
    }

    [TestMethod]
    public void Create_ShouldAddNewStrategyToRepository()
    {
        var configuracionCombo = new ConfiguracionPorCombo(10, 50, 2);
        var newStrategy = new ScoringStrategyEntity(
            "Nueva Estrategia",
            TipoEstrategia.PuntuacionCombo,
            "Descripción nueva",
            configuracionCombo);
        _mockRepository.Setup(r => r.Add(newStrategy)).Returns(newStrategy);

        var result = _service.Create(newStrategy);

        Assert.AreEqual(newStrategy, result);
        Assert.AreEqual("Nueva Estrategia", result.Name);
    }

    [TestMethod]
    public void ToggleActive_ShouldActivateInactiveStrategy_WhenNoOtherStrategyIsActive()
    {
        var strategyName = "Estrategia Test";
        _testStrategy.Active = false;
        var allStrategies = new List<ScoringStrategyEntity> { _testStrategy };

        _mockRepository.Setup(r => r.GetAll()).Returns(allStrategies);
        _mockRepository.Setup(r => r.Update(It.IsAny<ScoringStrategyEntity>())).Returns((ScoringStrategyEntity s) => s);

        var result = _service.ToggleActive(strategyName);

        Assert.AreEqual(_testStrategy, result);
        Assert.IsTrue(result.Active);
    }

    [TestMethod]
    [ExpectedException(typeof(ScoringStrategyException))]
    public void ToggleActive_ShouldThrowScoringStrategyException_WhenActivatingAndAnotherStrategyIsActive()
    {
        var configActive = new ConfiguracionPorAtraccion(new Dictionary<string, int> { ["test"] = 100 });
        var activeStrategy = new ScoringStrategyEntity("Estrategia Activa", TipoEstrategia.PuntuacionPorAtraccion, "Descripción", configActive)
        {
            Active = true
        };
        var configInactive = new ConfiguracionPorCombo(10, 50, 2);
        var inactiveStrategy = new ScoringStrategyEntity("Estrategia Inactiva", TipoEstrategia.PuntuacionCombo, "Descripción", configInactive)
        {
            Active = false
        };
        var allStrategies = new List<ScoringStrategyEntity> { activeStrategy, inactiveStrategy };

        _mockRepository.Setup(r => r.GetAll()).Returns(allStrategies);

        _service.ToggleActive("Estrategia Inactiva");
    }

    [TestMethod]
    [ExpectedException(typeof(ScoringStrategyException))]
    public void ToggleActive_ShouldThrowException_WhenTryingToDeactivateActiveStrategy()
    {
        _testStrategy.Active = true;
        var allStrategies = new List<ScoringStrategyEntity> { _testStrategy };

        _mockRepository.Setup(r => r.GetAll()).Returns(allStrategies);

        _service.ToggleActive(_testStrategy.Name);
    }

    [TestMethod]
    [ExpectedException(typeof(ScoringStrategyException))]
    public void Deactivate_ShouldThrowException_WhenNoActiveStrategy()
    {
        _mockRepository.Setup(r => r.GetAll()).Returns([]);

        _service.Deactivate();
    }

    [TestMethod]
    public void Deactivate_ShouldDeactivateActiveStrategy_WhenStrategyExists()
    {
        _testStrategy.Active = true;
        var allStrategies = new List<ScoringStrategyEntity> { _testStrategy };
        _mockRepository.Setup(r => r.GetAll()).Returns(allStrategies);
        _mockRepository.Setup(r => r.Update(It.IsAny<ScoringStrategyEntity>())).Returns((ScoringStrategyEntity s) => s);

        _service.Deactivate();

        Assert.IsFalse(_testStrategy.Active);
    }

    [TestMethod]
    public void Update_ShouldMergePartialUpdateAndUpdateRepository()
    {
        var strategyName = "Estrategia Test";
        var allStrategies = new List<ScoringStrategyEntity> { _testStrategy };
        var newConfig = new ConfiguracionPorAtraccion(new Dictionary<string, int> { ["new"] = 150 });
        var partialUpdate = new ScoringStrategyEntity
        {
            Description = "Nueva descripción actualizada",
            ConfiguracionTyped = newConfig
        };
        var expectedUpdated = new ScoringStrategyEntity(_testStrategy.Name, _testStrategy.Type, partialUpdate.Description, newConfig);

        _mockRepository.Setup(r => r.GetAll()).Returns(allStrategies);
        _mockRepository.Setup(r => r.Update(It.IsAny<ScoringStrategyEntity>())).Returns(expectedUpdated);

        var result = _service.Update(strategyName, partialUpdate);

        Assert.AreEqual(expectedUpdated, result);
        Assert.AreEqual("Nueva descripción actualizada", result.Description);
        Assert.IsInstanceOfType(result.ConfiguracionTyped, typeof(ConfiguracionPorAtraccion));
    }

    [TestMethod]
    public void GetActiveStrategy_ShouldReturnActiveStrategyFromRepository()
    {
        var config = new ConfiguracionPorAtraccion(new Dictionary<string, int> { ["test"] = 100 });
        var activeStrategy = new ScoringStrategyEntity("Estrategia Activa", TipoEstrategia.PuntuacionPorAtraccion, "Descripción", config)
        {
            Active = true
        };

        _mockRepository.Setup(r => r.GetActiveStrategy()).Returns(activeStrategy);
        var result = _service.GetActiveStrategy();

        Assert.AreEqual(activeStrategy, result);
        Assert.IsTrue(result!.Active);
    }

    [TestMethod]
    public void Delete_ShouldRemoveStrategyFromRepository()
    {
        var strategyName = "Estrategia Test";
        var allStrategies = new List<ScoringStrategyEntity> { _testStrategy };
        _mockRepository.Setup(r => r.GetAll()).Returns(allStrategies);
        _mockRepository.Setup(r => r.Delete(_testStrategy.Id)).Returns(true);

        _service.Delete(strategyName);

        _mockRepository.Verify(r => r.Delete(_testStrategy.Id), Times.Once);
    }

    [TestMethod]
    public void CalculateVisitPoints_ShouldUseActiveStrategyAndFactory_WhenActiveStrategyExists()
    {
        var attraction = new Attraction("Montaña Rusa", TipoAtraccion.MontañaRusa, 12, 50, "Emocionante montaña rusa", DateTime.Now, 100);
        var visit = new Visit(Guid.NewGuid(), attraction, DateTime.Now);
        var userVisits = new List<Visit>();

        var configuracion = new ConfiguracionPorAtraccion(new Dictionary<string, int> { ["montaña rusa"] = 150 });
        var activeStrategy = new ScoringStrategyEntity("Estrategia Activa", TipoEstrategia.PuntuacionPorAtraccion, "Descripción", configuracion)
        {
            Active = true
        };

        var mockAlgorithm = new Mock<IScoringAlgorithm>(MockBehavior.Strict);
        var mockFactory = new Mock<IScoringAlgorithmFactory>(MockBehavior.Strict);

        _mockRepository.Setup(r => r.GetActiveStrategy()).Returns(activeStrategy);
        mockFactory.Setup(f => f.CreateAlgorithm(It.IsAny<ScoringStrategyEntity>())).Returns(mockAlgorithm.Object);
        mockAlgorithm.Setup(a => a.CalculatePoints(visit, activeStrategy.ConfiguracionTyped!, userVisits)).Returns(150);

        var mockPluginLoader = new Mock<IPluginLoader>(MockBehavior.Strict);
        mockPluginLoader.Setup(p => p.GetAllPlugins()).Returns([]);

        var service = new ScoringStrategyService(_mockRepository.Object, mockFactory.Object, mockPluginLoader.Object);
        var result = service.CalculateVisitPoints(visit, userVisits);
        Assert.AreEqual(150, result);
        mockFactory.Verify(f => f.CreateAlgorithm(It.IsAny<ScoringStrategyEntity>()), Times.Once);
        mockAlgorithm.Verify(a => a.CalculatePoints(visit, activeStrategy.ConfiguracionTyped!, userVisits), Times.Once);
    }

    [TestMethod]
    public void ScoringStrategyException_NoActiveStrategy_ShouldHaveCorrectMessageAndInheritance()
    {
        var exception = ScoringStrategyException.NoActiveStrategy();

        Assert.IsInstanceOfType(exception, typeof(ScoringStrategyException));
        Assert.AreEqual("No se encontró una estrategia de puntuación activa", exception.Message);
        Assert.IsTrue(exception.TechnicalDetails.Contains("Scoring strategy error"));
    }

    [TestMethod]
    public void ScoringStrategyException_NoActiveStrategyToDeactivate_ShouldBeThrowable()
    {
        try
        {
            throw ScoringStrategyException.NoActiveStrategyToDeactivate();
        }
        catch(ScoringStrategyException ex)
        {
            Assert.AreEqual("No hay ninguna estrategia activa para desactivar", ex.Message);
            Assert.IsNotNull(ex.TechnicalDetails);
        }
    }

    [TestMethod]
    public void ScoringStrategyException_CannotActivateStrategy_ShouldHaveCorrectMessage()
    {
        var exception = ScoringStrategyException.CannotActivateStrategy();

        Assert.AreEqual("No se puede activar la estrategia: ya hay otra estrategia activa", exception.Message);
        Assert.IsInstanceOfType(exception, typeof(ScoringStrategyException));
    }

    [TestMethod]
    public void ScoringStrategyException_UnsupportedAlgorithmType_ShouldContainAlgorithmType()
    {
        var exception = ScoringStrategyException.UnsupportedAlgorithmType("AlgoritmoDesconocido");

        Assert.AreEqual("Tipo de algoritmo AlgoritmoDesconocido no compatible", exception.Message);
        Assert.IsTrue(exception.TechnicalDetails.Contains("Scoring strategy error"));
    }

    [TestMethod]
    public void ScoringStrategyException_NoTypedConfiguration_ShouldHaveCorrectMessageAndType()
    {
        var exception = ScoringStrategyException.NoTypedConfiguration();

        Assert.IsInstanceOfType(exception, typeof(ScoringStrategyException));
        Assert.AreEqual("La estrategia activa no tiene configuración tipada", exception.Message);
    }

    [TestMethod]
    public void ScoringStrategyException_InvalidConfigurationType_ShouldContainExpectedType()
    {
        var exception = ScoringStrategyException.InvalidConfigurationType("ConfiguracionPorCombo");

        Assert.AreEqual("La configuración debe ser de tipo ConfiguracionPorCombo para este algoritmo", exception.Message);
        Assert.IsNotNull(exception.TechnicalDetails);
    }

    [TestMethod]
    public void ScoringStrategyException_CannotDeleteActiveStrategy_ShouldBeThrowableWithCorrectMessage()
    {
        try
        {
            throw ScoringStrategyException.CannotDeleteActiveStrategy();
        }
        catch(ScoringStrategyException ex)
        {
            Assert.AreEqual("No se puede eliminar una estrategia de puntuación activa", ex.Message);
            Assert.IsInstanceOfType(ex, typeof(ScoringStrategyException));
        }
    }

    [TestMethod]
    [ExpectedException(typeof(ScoringStrategyNotFoundException))]
    public void GetByName_ShouldThrowScoringStrategyNotFoundException_WhenStrategyDoesNotExist()
    {
        var nombre = "Estrategia Inexistente";

        _mockRepository.Setup(x => x.GetAll()).Returns([]);

        _service.GetByName(nombre);
    }

    [TestMethod]
    [ExpectedException(typeof(ScoringStrategyNotFoundException))]
    public void ToggleActive_ShouldThrowScoringStrategyNotFoundException_WhenStrategyDoesNotExist()
    {
        var nombre = "Estrategia Inexistente";

        _mockRepository.Setup(x => x.GetAll()).Returns([]);

        _service.ToggleActive(nombre);
    }

    [TestMethod]
    [ExpectedException(typeof(ScoringStrategyNotFoundException))]
    public void Update_ShouldThrowScoringStrategyNotFoundException_WhenStrategyDoesNotExist()
    {
        var nombre = "Estrategia Inexistente";
        var configuracion = new ConfiguracionPorAtraccion(new Dictionary<string, int> { ["test"] = 50 });
        var partialUpdate = new ScoringStrategyEntity("Estrategia", TipoEstrategia.PuntuacionPorAtraccion, "Nueva descripcion", configuracion);

        _mockRepository.Setup(x => x.GetAll()).Returns([]);

        _service.Update(nombre, partialUpdate);
    }

    [TestMethod]
    [ExpectedException(typeof(ScoringStrategyNotFoundException))]
    public void Delete_ShouldThrowScoringStrategyNotFoundException_WhenStrategyDoesNotExist()
    {
        var nombre = "Estrategia Inexistente";

        _mockRepository.Setup(x => x.GetAll()).Returns([]);

        _service.Delete(nombre);
    }

    [TestMethod]
    public void GetAvailableStrategyTypes_ShouldReturnList()
    {
        var result = _service.GetAvailableStrategyTypes();

        Assert.IsNotNull(result);
        Assert.IsInstanceOfType(result, typeof(List<object>));
    }

    [TestMethod]
    public void GetAvailableStrategyTypes_ShouldIncludePluginsFromLoader()
    {
        var mockPluginLoader = new Mock<IPluginLoader>(MockBehavior.Strict);
        var mockPlugin = new Mock<IScoringStrategyPlugin>(MockBehavior.Strict);

        mockPlugin.Setup(p => p.StrategyTypeIdentifier).Returns("PuntuacionPorHora");
        mockPlugin.Setup(p => p.StrategyName).Returns("Puntuación por Hora");
        mockPlugin.Setup(p => p.Description).Returns("Multiplica puntos durante ciertas horas del día");

        var plugins = new List<IScoringStrategyPlugin> { mockPlugin.Object };
        mockPluginLoader.Setup(l => l.GetAllPlugins()).Returns(plugins);

        var service = new ScoringStrategyService(_mockRepository.Object, _mockAlgorithmFactory.Object, mockPluginLoader.Object);
        var result = service.GetAvailableStrategyTypes();

        Assert.IsTrue(result.Count >= 4, "Debe contener al menos 3 nativos + 1 plugin");
        mockPluginLoader.Verify(l => l.GetAllPlugins(), Times.Once);
    }

    [TestMethod]
    public void CreateStrategy_ShouldCreatePluginStrategyWithDeserializedConfiguration()
    {
        var pluginTypeId = "PuntuacionPorHora";
        var configJson = "{\"HourMultipliers\":{\"18\":2.0,\"19\":2.0}}";

        var mockConfig = new ConfiguracionPorAtraccion([]); // placeholder
        var mockPlugin = new Mock<IScoringStrategyPlugin>();
        mockPlugin.Setup(p => p.StrategyTypeIdentifier).Returns(pluginTypeId);
        mockPlugin.Setup(p => p.CreateConfigurationFromJson(configJson)).Returns(mockConfig);
        mockPlugin.Setup(p => p.ValidateConfiguration(mockConfig)).Returns(true);

        var mockPluginLoader = new Mock<IPluginLoader>(MockBehavior.Strict);
        mockPluginLoader.Setup(l => l.GetPlugin(pluginTypeId)).Returns(mockPlugin.Object);
        mockPluginLoader.Setup(l => l.GetAllPlugins()).Returns([]);

        var expectedStrategy = new ScoringStrategyEntity("Test Plugin", pluginTypeId, "Descripción", mockConfig, configJson);
        _mockRepository.Setup(r => r.Add(It.IsAny<ScoringStrategyEntity>())).Returns(expectedStrategy);

        var service = new ScoringStrategyService(_mockRepository.Object, _mockAlgorithmFactory.Object, mockPluginLoader.Object);
        var result = service.CreateStrategy("Test Plugin", "Descripción", (TipoEstrategia?)null, null, pluginTypeId, configJson);

        Assert.IsNotNull(result);
        Assert.AreEqual(pluginTypeId, result.PluginTypeIdentifier);
        Assert.AreEqual(configJson, result.ConfigurationJson);
        mockPlugin.Verify(p => p.CreateConfigurationFromJson(configJson), Times.Once);
        mockPlugin.Verify(p => p.ValidateConfiguration(mockConfig), Times.Once);
    }

    [TestMethod]
    [ExpectedException(typeof(ScoringStrategyException))]
    public void CalculateVisitPoints_WhenNoActiveStrategy_ShouldThrowException()
    {
        _mockRepository.Setup(r => r.GetActiveStrategy()).Returns((ScoringStrategyEntity?)null);

        var visit = new Visit(Guid.NewGuid(), "Montaña Rusa", DateTime.Now);
        _service.CalculateVisitPoints(visit, []);
    }

    [TestMethod]
    [ExpectedException(typeof(ScoringStrategyException))]
    public void CalculateVisitPoints_WhenNoTypedConfiguration_ShouldThrowException()
    {
        var strategyWithoutConfig = new ScoringStrategyEntity(
            "Test",
            TipoEstrategia.PuntuacionPorAtraccion,
            "Test",
            null!);

        _mockRepository.Setup(r => r.GetActiveStrategy()).Returns(strategyWithoutConfig);

        var visit = new Visit(Guid.NewGuid(), "Montaña Rusa", DateTime.Now);
        _service.CalculateVisitPoints(visit, []);
    }

    [TestMethod]
    [ExpectedException(typeof(ScoringStrategyException))]
    public void Delete_WhenStrategyIsActive_ShouldThrowException()
    {
        _testStrategy.Active = true;
        _mockRepository.Setup(r => r.GetAll()).Returns([_testStrategy]);

        _service.Delete(_testStrategy.Name);
    }

    [TestMethod]
    [ExpectedException(typeof(ScoringStrategyException))]
    public void CreateStrategy_WhenBothAlgoritmoAndPluginSpecified_ShouldThrowException()
    {
        var configuracion = new ConfiguracionPorAtraccion(new Dictionary<string, int>
        {
            ["MontañaRusa"] = 100,
            ["Simulador"] = 80,
            ["Espectaculo"] = 60,
            ["ZonaInteractiva"] = 40
        });

        _service.CreateStrategy(
            "Test",
            "Descripción",
            TipoEstrategia.PuntuacionPorAtraccion,
            configuracion,
            "PluginId",
            "{}");
    }

    [TestMethod]
    [ExpectedException(typeof(ScoringStrategyException))]
    public void CreateStrategy_WhenNeitherAlgoritmoNorPluginSpecified_ShouldThrowException()
    {
        _service.CreateStrategy(
            "Test",
            "Descripción",
            null,
            null,
            null,
            null);
    }

    [TestMethod]
    [ExpectedException(typeof(ScoringStrategyException))]
    public void CreateStrategy_WhenPluginSpecifiedWithoutConfigJson_ShouldThrowException()
    {
        var mockPluginLoader = new Mock<IPluginLoader>(MockBehavior.Strict);
        mockPluginLoader.Setup(l => l.GetAllPlugins()).Returns([]);

        var service = new ScoringStrategyService(_mockRepository.Object, _mockAlgorithmFactory.Object, mockPluginLoader.Object);
        service.CreateStrategy(
            "Test",
            "Descripción",
            null,
            null,
            "PluginId",
            null);
    }

    [TestMethod]
    [ExpectedException(typeof(ScoringStrategyException))]
    public void CreateStrategy_WhenPluginNotFound_ShouldThrowException()
    {
        var mockPluginLoader = new Mock<IPluginLoader>(MockBehavior.Strict);
        mockPluginLoader.Setup(l => l.GetPlugin("NonExistentPlugin")).Returns((IScoringStrategyPlugin?)null);
        mockPluginLoader.Setup(l => l.GetAllPlugins()).Returns([]);

        var service = new ScoringStrategyService(_mockRepository.Object, _mockAlgorithmFactory.Object, mockPluginLoader.Object);
        service.CreateStrategy(
            "Test",
            "Descripción",
            null,
            null,
            "NonExistentPlugin",
            "{}");
    }

    [TestMethod]
    [ExpectedException(typeof(ScoringStrategyException))]
    public void CreateStrategy_WhenPluginConfigJsonInvalid_ShouldThrowException()
    {
        var mockPlugin = new Mock<IScoringStrategyPlugin>();
        mockPlugin.Setup(p => p.CreateConfigurationFromJson(It.IsAny<string>()))
            .Throws(new Exception("Invalid JSON"));

        var mockPluginLoader = new Mock<IPluginLoader>(MockBehavior.Strict);
        mockPluginLoader.Setup(l => l.GetPlugin("PluginId")).Returns(mockPlugin.Object);
        mockPluginLoader.Setup(l => l.GetAllPlugins()).Returns([]);

        var service = new ScoringStrategyService(_mockRepository.Object, _mockAlgorithmFactory.Object, mockPluginLoader.Object);
        service.CreateStrategy(
            "Test",
            "Descripción",
            null,
            null,
            "PluginId",
            "invalid json");
    }

    [TestMethod]
    [ExpectedException(typeof(ScoringStrategyException))]
    public void CreateStrategy_WhenPluginConfigInvalid_ShouldThrowException()
    {
        var mockConfig = new ConfiguracionPorAtraccion([]);
        var mockPlugin = new Mock<IScoringStrategyPlugin>();
        mockPlugin.Setup(p => p.CreateConfigurationFromJson(It.IsAny<string>()))
            .Returns(mockConfig);
        mockPlugin.Setup(p => p.ValidateConfiguration(mockConfig)).Returns(false);

        var mockPluginLoader = new Mock<IPluginLoader>(MockBehavior.Strict);
        mockPluginLoader.Setup(l => l.GetPlugin("PluginId")).Returns(mockPlugin.Object);
        mockPluginLoader.Setup(l => l.GetAllPlugins()).Returns([]);

        var service = new ScoringStrategyService(_mockRepository.Object, _mockAlgorithmFactory.Object, mockPluginLoader.Object);
        service.CreateStrategy(
            "Test",
            "Descripción",
            null,
            null,
            "PluginId",
            "{}");
    }

    [TestMethod]
    [ExpectedException(typeof(ScoringStrategyException))]
    public void CreateStrategy_WhenNativeStrategyWithoutConfiguration_ShouldThrowException()
    {
        _service.CreateStrategy(
            "Test",
            "Descripción",
            TipoEstrategia.PuntuacionPorAtraccion,
            null,
            null,
            null);
    }

    [TestMethod]
    public void EnsurePluginConfigurationLoaded_WhenPluginNotFound_ShouldMarkAsUnavailable()
    {
        var pluginStrategy = new ScoringStrategyEntity(
            "Plugin Strategy",
            "NonExistentPlugin",
            "Test",
            null!,
            "{}");

        var mockPluginLoader = new Mock<IPluginLoader>(MockBehavior.Strict);
        mockPluginLoader.Setup(l => l.GetPlugin("NonExistentPlugin")).Returns((IScoringStrategyPlugin?)null);
        mockPluginLoader.Setup(l => l.GetAllPlugins()).Returns([]);

        var service = new ScoringStrategyService(_mockRepository.Object, _mockAlgorithmFactory.Object, mockPluginLoader.Object);
        _mockRepository.Setup(r => r.GetAll()).Returns([pluginStrategy]);

        var strategies = service.GetAllStrategies();

        Assert.AreEqual(1, strategies.Count);
        Assert.IsFalse(strategies[0].IsPluginAvailable, "Plugin should be marked as unavailable");
        Assert.IsNull(strategies[0].ConfiguracionTyped, "Configuration should be null when plugin is not available");
    }

    [TestMethod]
    public void Update_ShouldUpdatePluginConfiguration_WhenPluginStrategyWithConfigJson()
    {
        var pluginTypeId = "PuntuacionPorHora";
        var oldConfigJson = "{\"HourMultipliers\":{\"18\":1.5}}";
        var newConfigJson = "{\"HourMultipliers\":{\"18\":2.0}}";

        var oldConfig = new ConfiguracionPorAtraccion([]);
        var newConfig = new ConfiguracionPorAtraccion([]);

        var existingStrategy = new ScoringStrategyEntity(
            "Plugin Strategy",
            pluginTypeId,
            "Old Description",
            oldConfig,
            oldConfigJson);

        var mockPlugin = new Mock<IScoringStrategyPlugin>();
        mockPlugin.Setup(p => p.CreateConfigurationFromJson(newConfigJson)).Returns(newConfig);
        mockPlugin.Setup(p => p.ValidateConfiguration(newConfig)).Returns(true);

        var mockPluginLoader = new Mock<IPluginLoader>(MockBehavior.Strict);
        mockPluginLoader.Setup(l => l.GetPlugin(pluginTypeId)).Returns(mockPlugin.Object);
        mockPluginLoader.Setup(l => l.GetAllPlugins()).Returns([]);

        var service = new ScoringStrategyService(_mockRepository.Object, _mockAlgorithmFactory.Object, mockPluginLoader.Object);

        var partialUpdate = new ScoringStrategyEntity
        {
            Description = "New Description",
            ConfigurationJson = newConfigJson
        };

        _mockRepository.Setup(r => r.GetAll()).Returns([existingStrategy]);
        _mockRepository.Setup(r => r.Update(It.IsAny<ScoringStrategyEntity>())).Returns(existingStrategy);

        var result = service.Update("Plugin Strategy", partialUpdate);

        Assert.AreEqual("New Description", existingStrategy.Description);
        Assert.AreEqual(newConfigJson, existingStrategy.ConfigurationJson);
        mockPlugin.Verify(p => p.CreateConfigurationFromJson(newConfigJson), Times.Once);
        mockPlugin.Verify(p => p.ValidateConfiguration(newConfig), Times.Once);
    }

    [TestMethod]
    [ExpectedException(typeof(ScoringStrategyException))]
    public void Update_ShouldThrowException_WhenPluginNotFoundDuringUpdate()
    {
        var pluginTypeId = "NonExistentPlugin";
        var oldConfigJson = "{\"test\":1}";
        var newConfigJson = "{\"test\":2}";

        var oldConfig = new ConfiguracionPorAtraccion([]);
        var existingStrategy = new ScoringStrategyEntity(
            "Plugin Strategy",
            pluginTypeId,
            "Description",
            oldConfig,
            oldConfigJson);

        var mockPluginLoader = new Mock<IPluginLoader>(MockBehavior.Strict);
        mockPluginLoader.Setup(l => l.GetPlugin(pluginTypeId)).Returns((IScoringStrategyPlugin?)null);
        mockPluginLoader.Setup(l => l.GetAllPlugins()).Returns([]);

        var service = new ScoringStrategyService(_mockRepository.Object, _mockAlgorithmFactory.Object, mockPluginLoader.Object);

        var partialUpdate = new ScoringStrategyEntity
        {
            Description = "New Description",
            ConfigurationJson = newConfigJson
        };

        _mockRepository.Setup(r => r.GetAll()).Returns([existingStrategy]);

        service.Update("Plugin Strategy", partialUpdate);
    }

    [TestMethod]
    [ExpectedException(typeof(ScoringStrategyException))]
    public void Update_ShouldThrowException_WhenPluginConfigJsonDeserializationFails()
    {
        var pluginTypeId = "PuntuacionPorHora";
        var oldConfigJson = "{\"test\":1}";
        var invalidConfigJson = "invalid json";

        var oldConfig = new ConfiguracionPorAtraccion([]);
        var existingStrategy = new ScoringStrategyEntity(
            "Plugin Strategy",
            pluginTypeId,
            "Description",
            oldConfig,
            oldConfigJson);

        var mockPlugin = new Mock<IScoringStrategyPlugin>();
        mockPlugin.Setup(p => p.CreateConfigurationFromJson(invalidConfigJson))
            .Throws(new Exception("JSON deserialization failed"));

        var mockPluginLoader = new Mock<IPluginLoader>(MockBehavior.Strict);
        mockPluginLoader.Setup(l => l.GetPlugin(pluginTypeId)).Returns(mockPlugin.Object);
        mockPluginLoader.Setup(l => l.GetAllPlugins()).Returns([]);

        var service = new ScoringStrategyService(_mockRepository.Object, _mockAlgorithmFactory.Object, mockPluginLoader.Object);

        var partialUpdate = new ScoringStrategyEntity
        {
            Description = "New Description",
            ConfigurationJson = invalidConfigJson
        };

        _mockRepository.Setup(r => r.GetAll()).Returns([existingStrategy]);

        service.Update("Plugin Strategy", partialUpdate);
    }

    [TestMethod]
    [ExpectedException(typeof(ScoringStrategyException))]
    public void Update_ShouldThrowException_WhenPluginConfigValidationFails()
    {
        var pluginTypeId = "PuntuacionPorHora";
        var oldConfigJson = "{\"test\":1}";
        var newConfigJson = "{\"test\":2}";

        var oldConfig = new ConfiguracionPorAtraccion([]);
        var newConfig = new ConfiguracionPorAtraccion([]);
        var existingStrategy = new ScoringStrategyEntity(
            "Plugin Strategy",
            pluginTypeId,
            "Description",
            oldConfig,
            oldConfigJson);

        var mockPlugin = new Mock<IScoringStrategyPlugin>();
        mockPlugin.Setup(p => p.CreateConfigurationFromJson(newConfigJson)).Returns(newConfig);
        mockPlugin.Setup(p => p.ValidateConfiguration(newConfig)).Returns(false);

        var mockPluginLoader = new Mock<IPluginLoader>(MockBehavior.Strict);
        mockPluginLoader.Setup(l => l.GetPlugin(pluginTypeId)).Returns(mockPlugin.Object);
        mockPluginLoader.Setup(l => l.GetAllPlugins()).Returns([]);

        var service = new ScoringStrategyService(_mockRepository.Object, _mockAlgorithmFactory.Object, mockPluginLoader.Object);

        var partialUpdate = new ScoringStrategyEntity
        {
            Description = "New Description",
            ConfigurationJson = newConfigJson
        };

        _mockRepository.Setup(r => r.GetAll()).Returns([existingStrategy]);

        service.Update("Plugin Strategy", partialUpdate);
    }

    [TestMethod]
    public void Update_ShouldUpdateComboConfiguration_WhenComboStrategyProvided()
    {
        var oldConfig = new ConfiguracionPorCombo(10, 50, 2);
        var existingStrategy = new ScoringStrategyEntity(
            "Combo Strategy",
            TipoEstrategia.PuntuacionCombo,
            "Old Description",
            oldConfig);

        var newConfig = new ConfiguracionPorCombo(15, 75, 3);
        var partialUpdate = new ScoringStrategyEntity
        {
            Description = "New Description",
            ConfiguracionTyped = newConfig
        };

        _mockRepository.Setup(r => r.GetAll()).Returns([existingStrategy]);
        _mockRepository.Setup(r => r.Update(It.IsAny<ScoringStrategyEntity>())).Returns(existingStrategy);

        var result = _service.Update("Combo Strategy", partialUpdate);

        Assert.AreEqual(15, ((ConfiguracionPorCombo)existingStrategy.ConfiguracionTyped).VentanaTemporalMinutos);
        Assert.AreEqual(75, ((ConfiguracionPorCombo)existingStrategy.ConfiguracionTyped).BonusMultiplicador);
        Assert.AreEqual(3, ((ConfiguracionPorCombo)existingStrategy.ConfiguracionTyped).MinimoAtracciones);
    }

    [TestMethod]
    public void Update_ShouldUpdateEventoConfiguration_WhenEventoStrategyProvided()
    {
        var oldConfig = new ConfiguracionPorEvento(100, "OldEvent");
        var existingStrategy = new ScoringStrategyEntity(
            "Evento Strategy",
            TipoEstrategia.PuntuacionPorEvento,
            "Old Description",
            oldConfig);

        var newConfig = new ConfiguracionPorEvento(200, "NewEvent");
        var partialUpdate = new ScoringStrategyEntity
        {
            Description = "New Description",
            ConfiguracionTyped = newConfig
        };

        _mockRepository.Setup(r => r.GetAll()).Returns([existingStrategy]);
        _mockRepository.Setup(r => r.Update(It.IsAny<ScoringStrategyEntity>())).Returns(existingStrategy);

        var result = _service.Update("Evento Strategy", partialUpdate);

        Assert.AreEqual(200, ((ConfiguracionPorEvento)existingStrategy.ConfiguracionTyped).Puntos);
        Assert.AreEqual("NewEvent", ((ConfiguracionPorEvento)existingStrategy.ConfiguracionTyped).Evento);
    }

    [TestMethod]
    public void CreateStrategy_ShouldCreateNativeComboStrategy()
    {
        var configuracion = new ConfiguracionPorCombo(30, 100, 3);

        var expectedStrategy = new ScoringStrategyEntity(
            "Combo Strategy",
            TipoEstrategia.PuntuacionCombo,
            "Combo description",
            configuracion);

        _mockRepository.Setup(r => r.Add(It.IsAny<ScoringStrategyEntity>())).Returns(expectedStrategy);

        var result = _service.CreateStrategy(
            "Combo Strategy",
            "Combo description",
            TipoEstrategia.PuntuacionCombo,
            configuracion,
            null,
            null);

        Assert.IsNotNull(result);
        Assert.AreEqual(TipoEstrategia.PuntuacionCombo, result.Type);
    }

    [TestMethod]
    public void CreateStrategy_ShouldCreateNativeEventoStrategy()
    {
        var configuracion = new ConfiguracionPorEvento(500, "TestEvent");

        var expectedStrategy = new ScoringStrategyEntity(
            "Evento Strategy",
            TipoEstrategia.PuntuacionPorEvento,
            "Evento description",
            configuracion);

        _mockRepository.Setup(r => r.Add(It.IsAny<ScoringStrategyEntity>())).Returns(expectedStrategy);

        var result = _service.CreateStrategy(
            "Evento Strategy",
            "Evento description",
            TipoEstrategia.PuntuacionPorEvento,
            configuracion,
            null,
            null);

        Assert.IsNotNull(result);
        Assert.AreEqual(TipoEstrategia.PuntuacionPorEvento, result.Type);
    }
}
