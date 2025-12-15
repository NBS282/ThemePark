using ThemePark.BusinessLogic.ScoringStrategy;

namespace ThemePark.TestBusinessLogic.ScoringStrategy;

[TestClass]
public class ScoringStrategyPluginLoaderTests
{
    private ScoringStrategyPluginLoader _loader = null!;
    private string _testPluginsPath = null!;

    [TestInitialize]
    public void Setup()
    {
        _loader = new ScoringStrategyPluginLoader();
        _testPluginsPath = Path.Combine(Path.GetTempPath(), $"TestPlugins_{Guid.NewGuid()}");
        Directory.CreateDirectory(_testPluginsPath);
    }

    [TestCleanup]
    public void Cleanup()
    {
        _loader?.Dispose();

        if(Directory.Exists(_testPluginsPath))
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            try
            {
                Directory.Delete(_testPluginsPath, true);
            }
            catch(UnauthorizedAccessException)
            {
                TryDeleteDirectory(_testPluginsPath);
            }
        }
    }

    private static void TryDeleteDirectory(string path)
    {
        try
        {
            foreach(var file in Directory.GetFiles(path))
            {
                try
                {
                    File.SetAttributes(file, FileAttributes.Normal);
                    File.Delete(file);
                }
                catch
                {
                }
            }

            foreach(var dir in Directory.GetDirectories(path))
            {
                TryDeleteDirectory(dir);
            }

            try
            {
                Directory.Delete(path, false);
            }
            catch
            {
            }
        }
        catch
        {
        }
    }

    [TestMethod]
    public void GetAllPlugins_WhenNoPluginsLoaded_ShouldReturnEmptyList()
    {
        var plugins = _loader.GetAllPlugins();

        Assert.IsNotNull(plugins);
        Assert.AreEqual(0, plugins.Count);
    }

    [TestMethod]
    public void GetPlugin_WhenPluginDoesNotExist_ShouldReturnNull()
    {
        var plugin = _loader.GetPlugin("NonExistentPlugin");

        Assert.IsNull(plugin);
    }

    [TestMethod]
    public void LoadPlugins_WhenDirectoryDoesNotExist_ShouldNotThrowException()
    {
        var nonExistentPath = Path.Combine(Path.GetTempPath(), $"NonExistent_{Guid.NewGuid()}");

        _loader.LoadPlugins(nonExistentPath);
        Assert.AreEqual(0, _loader.GetAllPlugins().Count);
    }

    [TestMethod]
    public void LoadPlugins_WhenDirectoryIsEmpty_ShouldNotLoadAnyPlugins()
    {
        _loader.LoadPlugins(_testPluginsPath);

        Assert.AreEqual(0, _loader.GetAllPlugins().Count);
    }

    [TestMethod]
    public void LoadPlugins_WhenDirectoryContainsNonDllFiles_ShouldNotThrowException()
    {
        File.WriteAllText(Path.Combine(_testPluginsPath, "test.txt"), "not a dll");

        _loader.LoadPlugins(_testPluginsPath);

        Assert.AreEqual(0, _loader.GetAllPlugins().Count);
    }

    [TestMethod]
    public void LoadPlugins_WhenDirectoryContainsInvalidDll_ShouldNotThrowException()
    {
        File.WriteAllText(Path.Combine(_testPluginsPath, "invalid.dll"), "not a valid dll");

        _loader.LoadPlugins(_testPluginsPath);

        Assert.AreEqual(0, _loader.GetAllPlugins().Count);
    }

    [TestMethod]
    public void LoadPlugins_WhenValidPluginDllExists_ShouldLoadPlugin()
    {
        var sourcePluginPath = FindPluginDll();
        if(sourcePluginPath != null)
        {
            var targetPath = Path.Combine(_testPluginsPath, Path.GetFileName(sourcePluginPath));
            File.Copy(sourcePluginPath, targetPath);

            _loader.LoadPlugins(_testPluginsPath);

            var plugins = _loader.GetAllPlugins();
            Assert.IsTrue(plugins.Count > 0, "Should have loaded at least one plugin");
        }
        else
        {
            Assert.Inconclusive("No plugin DLL found to test with");
        }
    }

    [TestMethod]
    public void GetPlugin_AfterLoadingValidPlugin_ShouldReturnPlugin()
    {
        var sourcePluginPath = FindPluginDll();
        if(sourcePluginPath != null)
        {
            var targetPath = Path.Combine(_testPluginsPath, Path.GetFileName(sourcePluginPath));
            File.Copy(sourcePluginPath, targetPath);
            _loader.LoadPlugins(_testPluginsPath);

            var plugin = _loader.GetPlugin("PuntuacionPorHora");

            Assert.IsNotNull(plugin);
            Assert.AreEqual("PuntuacionPorHora", plugin.StrategyTypeIdentifier);
        }
        else
        {
            Assert.Inconclusive("No plugin DLL found to test with");
        }
    }

    [TestMethod]
    public void GetAllPlugins_AfterLoadingMultiplePlugins_ShouldReturnAllPlugins()
    {
        var sourcePluginPath = FindPluginDll();
        if(sourcePluginPath != null)
        {
            var targetPath = Path.Combine(_testPluginsPath, Path.GetFileName(sourcePluginPath));
            File.Copy(sourcePluginPath, targetPath);
            _loader.LoadPlugins(_testPluginsPath);

            var plugins = _loader.GetAllPlugins();

            Assert.IsTrue(plugins.Count > 0);
            foreach(var plugin in plugins)
            {
                Assert.IsNotNull(plugin.StrategyTypeIdentifier);
                Assert.IsNotNull(plugin.StrategyName);
                Assert.IsNotNull(plugin.Description);
            }
        }
        else
        {
            Assert.Inconclusive("No plugin DLL found to test with");
        }
    }

    [TestMethod]
    public void LoadPlugins_WhenCalledMultipleTimes_ShouldAccumulatePlugins()
    {
        var sourcePluginPath = FindPluginDll();
        if(sourcePluginPath != null)
        {
            var targetPath = Path.Combine(_testPluginsPath, Path.GetFileName(sourcePluginPath));
            File.Copy(sourcePluginPath, targetPath);

            _loader.LoadPlugins(_testPluginsPath);
            var countAfterFirstLoad = _loader.GetAllPlugins().Count;

            _loader.LoadPlugins(_testPluginsPath);
            var countAfterSecondLoad = _loader.GetAllPlugins().Count;

            Assert.AreEqual(countAfterFirstLoad * 2, countAfterSecondLoad, "Plugins should accumulate on multiple loads");
        }
        else
        {
            Assert.Inconclusive("No plugin DLL found to test with");
        }
    }

    [TestMethod]
    public void ReloadPlugins_ShouldReplaceExistingPlugins()
    {
        var sourcePluginPath = FindPluginDll();
        if(sourcePluginPath != null)
        {
            var targetPath = Path.Combine(_testPluginsPath, Path.GetFileName(sourcePluginPath));
            File.Copy(sourcePluginPath, targetPath);

            _loader.LoadPlugins(_testPluginsPath);
            var countAfterFirstLoad = _loader.GetAllPlugins().Count;

            _loader.ReloadPlugins();
            var countAfterReload = _loader.GetAllPlugins().Count;

            Assert.AreEqual(countAfterFirstLoad, countAfterReload, "ReloadPlugins should replace, not accumulate plugins");
        }
        else
        {
            Assert.Inconclusive("No plugin DLL found to test with");
        }
    }

    [TestMethod]
    public void Dispose_ShouldClearAllPlugins()
    {
        var sourcePluginPath = FindPluginDll();
        if(sourcePluginPath != null)
        {
            var targetPath = Path.Combine(_testPluginsPath, Path.GetFileName(sourcePluginPath));
            File.Copy(sourcePluginPath, targetPath);

            _loader.LoadPlugins(_testPluginsPath);
            Assert.IsTrue(_loader.GetAllPlugins().Count > 0, "Should have loaded plugins");

            _loader.Dispose();

            Assert.AreEqual(0, _loader.GetAllPlugins().Count, "Dispose should clear all plugins");
        }
        else
        {
            Assert.Inconclusive("No plugin DLL found to test with");
        }
    }

    [TestMethod]
    public void GetPlugin_WithValidIdentifier_ShouldReturnCorrectPlugin()
    {
        var sourcePluginPath = FindPluginDll();
        if(sourcePluginPath != null)
        {
            var targetPath = Path.Combine(_testPluginsPath, Path.GetFileName(sourcePluginPath));
            File.Copy(sourcePluginPath, targetPath);
            _loader.LoadPlugins(_testPluginsPath);

            var plugins = _loader.GetAllPlugins();
            if(plugins.Count > 0)
            {
                var expectedPlugin = plugins[0];

                var retrievedPlugin = _loader.GetPlugin(expectedPlugin.StrategyTypeIdentifier);

                Assert.IsNotNull(retrievedPlugin);
                Assert.AreEqual(expectedPlugin.StrategyTypeIdentifier, retrievedPlugin.StrategyTypeIdentifier);
                Assert.AreEqual(expectedPlugin.StrategyName, retrievedPlugin.StrategyName);
            }
        }
        else
        {
            Assert.Inconclusive("No plugin DLL found to test with");
        }
    }

    [TestMethod]
    public void StartWatching_WhenDirectoryDoesNotExist_ShouldCreateDirectory()
    {
        var nonExistentPath = Path.Combine(Path.GetTempPath(), $"WatchTest_{Guid.NewGuid()}");

        try
        {
            _loader.StartWatching(nonExistentPath);

            Assert.IsTrue(Directory.Exists(nonExistentPath), "StartWatching should create directory if it doesn't exist");
        }
        finally
        {
            if(Directory.Exists(nonExistentPath))
            {
                Directory.Delete(nonExistentPath, true);
            }
        }
    }

    [TestMethod]
    public void StartWatching_ShouldLoadPluginsInitially()
    {
        var sourcePluginPath = FindPluginDll();
        if(sourcePluginPath != null)
        {
            var watchPath = Path.Combine(Path.GetTempPath(), $"WatchTest_{Guid.NewGuid()}");
            Directory.CreateDirectory(watchPath);

            try
            {
                var targetPath = Path.Combine(watchPath, Path.GetFileName(sourcePluginPath));
                File.Copy(sourcePluginPath, targetPath);

                _loader.StartWatching(watchPath);

                Assert.IsTrue(_loader.GetAllPlugins().Count > 0, "StartWatching should load existing plugins");
            }
            finally
            {
                if(Directory.Exists(watchPath))
                {
                    TryDeleteDirectory(watchPath);
                }
            }
        }
        else
        {
            Assert.Inconclusive("No plugin DLL found to test with");
        }
    }

    [TestMethod]
    public void StartWatching_WhenEmptyDirectory_ShouldNotThrowException()
    {
        var watchPath = Path.Combine(Path.GetTempPath(), $"WatchTest_{Guid.NewGuid()}");
        Directory.CreateDirectory(watchPath);

        try
        {
            _loader.StartWatching(watchPath);

            Assert.AreEqual(0, _loader.GetAllPlugins().Count);
        }
        finally
        {
            if(Directory.Exists(watchPath))
            {
                Directory.Delete(watchPath, true);
            }
        }
    }

    [TestMethod]
    public void StartWatching_WhenFileAdded_ShouldTriggerReload()
    {
        var sourcePluginPath = FindPluginDll();
        if(sourcePluginPath != null)
        {
            var watchPath = Path.Combine(Path.GetTempPath(), $"WatchTest_{Guid.NewGuid()}");
            Directory.CreateDirectory(watchPath);

            try
            {
                _loader.StartWatching(watchPath);
                Assert.AreEqual(0, _loader.GetAllPlugins().Count, "Should start with no plugins");

                var targetPath = Path.Combine(watchPath, Path.GetFileName(sourcePluginPath));
                File.Copy(sourcePluginPath, targetPath);

                Thread.Sleep(3000);

                Assert.IsTrue(_loader.GetAllPlugins().Count > 0, "Plugin should be loaded after file is added");
            }
            finally
            {
                if(Directory.Exists(watchPath))
                {
                    TryDeleteDirectory(watchPath);
                }
            }
        }
        else
        {
            Assert.Inconclusive("No plugin DLL found to test with");
        }
    }

    [TestMethod]
    public void ReloadPlugins_WhenPluginRemoved_ShouldCallDeactivateStrategies()
    {
        var sourcePluginPath = FindPluginDll();
        if(sourcePluginPath != null)
        {
            var targetPath = Path.Combine(_testPluginsPath, Path.GetFileName(sourcePluginPath));
            File.Copy(sourcePluginPath, targetPath);

            _loader.LoadPlugins(_testPluginsPath);
            var initialCount = _loader.GetAllPlugins().Count;
            Assert.IsTrue(initialCount > 0, "Should have loaded plugins");

            var mockRepository = new Moq.Mock<ThemePark.IDataAccess.IScoringStrategyRepository>();
            _loader.SetRepository(mockRepository.Object);

            File.Delete(targetPath);

            _loader.ReloadPlugins();

            mockRepository.Verify(r => r.DeactivateStrategiesByPluginTypeIdentifier(Moq.It.IsAny<string>()), Moq.Times.AtLeastOnce());
            Assert.AreEqual(0, _loader.GetAllPlugins().Count, "All plugins should be removed");
        }
        else
        {
            Assert.Inconclusive("No plugin DLL found to test with");
        }
    }

    [TestMethod]
    public void LoadPlugins_WhenFileIsLocked_ShouldSkipLockedFile()
    {
        var sourcePluginPath = FindPluginDll();
        if(sourcePluginPath != null)
        {
            var targetPath = Path.Combine(_testPluginsPath, "locked.dll");
            File.Copy(sourcePluginPath, targetPath);

            // Bloquear el archivo
            using(var fileStream = new FileStream(targetPath, FileMode.Open, FileAccess.Read, FileShare.None))
            {
                // Intentar cargar plugins mientras el archivo está bloqueado
                _loader.LoadPlugins(_testPluginsPath);

                // El loader debería manejar el archivo bloqueado sin lanzar excepción
                Assert.AreEqual(0, _loader.GetAllPlugins().Count, "Locked file should be skipped");
            }

            // Ahora que el archivo está desbloqueado, debería poder cargarse
            _loader.LoadPlugins(_testPluginsPath);
            Assert.IsTrue(_loader.GetAllPlugins().Count > 0, "File should load after being unlocked");
        }
        else
        {
            Assert.Inconclusive("No plugin DLL found to test with");
        }
    }

    [TestMethod]
    public void ReloadPlugins_WhenRepositoryThrowsException_ShouldHandleGracefully()
    {
        var sourcePluginPath = FindPluginDll();
        if(sourcePluginPath != null)
        {
            var targetPath = Path.Combine(_testPluginsPath, Path.GetFileName(sourcePluginPath));
            File.Copy(sourcePluginPath, targetPath);

            _loader.LoadPlugins(_testPluginsPath);
            var initialCount = _loader.GetAllPlugins().Count;
            Assert.IsTrue(initialCount > 0, "Should have loaded plugins");

            // Crear un mock del repository que lance excepción
            var mockRepository = new Moq.Mock<ThemePark.IDataAccess.IScoringStrategyRepository>();
            mockRepository.Setup(r => r.DeactivateStrategiesByPluginTypeIdentifier(Moq.It.IsAny<string>()))
                .Throws(new Exception("Database error"));
            _loader.SetRepository(mockRepository.Object);

            File.Delete(targetPath);

            // Recargar plugins - no debería lanzar excepción a pesar del error del repository
            _loader.ReloadPlugins();

            Assert.AreEqual(0, _loader.GetAllPlugins().Count, "Plugins should still be removed despite repository error");
        }
        else
        {
            Assert.Inconclusive("No plugin DLL found to test with");
        }
    }

    private static string? FindPluginDll()
    {
        var searchPaths = new[]
        {
            Path.Combine(Directory.GetCurrentDirectory(), "Plugins"),
            Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "..", "ThemeParkApi", "Plugins"),
            Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "..", "..", "ScoringStrategies", "ScoringStrategyPlugins", "ScoringStrategyPlugins", "bin", "Debug", "net8.0"),
            Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "..", "..", "ScoringStrategies", "ScoringStrategyPlugins", "ScoringStrategyPlugins", "bin", "Release", "net8.0")
        };

        foreach(var path in searchPaths)
        {
            if(Directory.Exists(path))
            {
                var dllFiles = Directory.GetFiles(path, "ScoringStrategyPlugins.dll", SearchOption.TopDirectoryOnly);
                if(dllFiles.Length > 0)
                {
                    return dllFiles[0];
                }
            }
        }

        return null;
    }
}
