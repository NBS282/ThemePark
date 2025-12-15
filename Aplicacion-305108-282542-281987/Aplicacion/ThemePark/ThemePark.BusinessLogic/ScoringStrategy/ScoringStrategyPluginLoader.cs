using System.Reflection;
using System.Runtime.Loader;
using ThemePark.IBusinessLogic;
using ThemePark.IDataAccess;

namespace ThemePark.BusinessLogic.ScoringStrategy;

public class ScoringStrategyPluginLoader : IPluginLoader, IDisposable
{
    private readonly List<IScoringStrategyPlugin> _plugins = [];
    private readonly Dictionary<string, IScoringStrategyPlugin> _pluginsByIdentifier = [];
    private readonly List<WeakReference> _loadContexts = [];
    private readonly object _lock = new();
    private FileSystemWatcher? _watcher;
    private string? _pluginsPath;
    private Timer? _reloadTimer;
    private IScoringStrategyRepository? _repository;

    public List<IScoringStrategyPlugin> GetAllPlugins()
    {
        lock(_lock)
        {
            return new List<IScoringStrategyPlugin>(_plugins);
        }
    }

    public IScoringStrategyPlugin? GetPlugin(string typeIdentifier)
    {
        lock(_lock)
        {
            _pluginsByIdentifier.TryGetValue(typeIdentifier, out var plugin);
            return plugin;
        }
    }

    public void SetRepository(IScoringStrategyRepository repository)
    {
        _repository = repository;
    }

    public void LoadPlugins(string pluginsPath)
    {
        lock(_lock)
        {
            _pluginsPath = pluginsPath;

            if(!Directory.Exists(pluginsPath))
            {
                Directory.CreateDirectory(pluginsPath);
                return;
            }

            LoadPluginsInternal(pluginsPath);
        }
    }

    public void StartWatching(string pluginsPath)
    {
        _pluginsPath = pluginsPath;

        if(!Directory.Exists(pluginsPath))
        {
            Directory.CreateDirectory(pluginsPath);
        }

        LoadPlugins(pluginsPath);

        _watcher = new FileSystemWatcher(pluginsPath)
        {
            Filter = "*.dll",
            NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite | NotifyFilters.CreationTime,
            EnableRaisingEvents = true
        };

        _watcher.Changed += OnPluginFileChanged;
        _watcher.Created += OnPluginFileChanged;
        _watcher.Deleted += OnPluginFileChanged;
        _watcher.Renamed += OnPluginFileChanged;
    }

    private void OnPluginFileChanged(object sender, FileSystemEventArgs e)
    {
        _reloadTimer?.Dispose();
        _reloadTimer = new Timer(_ =>
        {
            try
            {
                ReloadPlugins();
            }
            catch(Exception)
            {
            }
        }, null, TimeSpan.FromSeconds(2), Timeout.InfiniteTimeSpan);
    }

    public void ReloadPlugins()
    {
        if(string.IsNullOrEmpty(_pluginsPath))
        {
            return;
        }

        lock(_lock)
        {
            var previousPluginIdentifiers = new HashSet<string>(_pluginsByIdentifier.Keys);

            UnloadPluginsInternal();

            LoadPluginsInternal(_pluginsPath);

            var currentPluginIdentifiers = new HashSet<string>(_pluginsByIdentifier.Keys);
            var removedPluginIdentifiers = previousPluginIdentifiers.Except(currentPluginIdentifiers).ToList();
            if(_repository != null && removedPluginIdentifiers.Any())
            {
                foreach(var removedPluginId in removedPluginIdentifiers)
                {
                    try
                    {
                        _repository.DeactivateStrategiesByPluginTypeIdentifier(removedPluginId);
                    }
                    catch(Exception)
                    {
                    }
                }
            }
        }
    }

    private void LoadPluginsInternal(string pluginsPath)
    {
        if(!Directory.Exists(pluginsPath))
        {
            try
            {
                Directory.CreateDirectory(pluginsPath);
            }
            catch(Exception)
            {
            }

            return;
        }

        var dllFiles = Directory.GetFiles(pluginsPath, "*.dll");

        foreach(var dllFile in dllFiles)
        {
            try
            {
                if(!WaitForFileAccess(dllFile, TimeSpan.FromSeconds(3)))
                {
                    continue;
                }

                var tempPath = Path.Combine(Path.GetTempPath(), "ThemeParkPlugins", Guid.NewGuid().ToString());
                Directory.CreateDirectory(tempPath);

                var tempDllPath = Path.Combine(tempPath, Path.GetFileName(dllFile));
                File.Copy(dllFile, tempDllPath, true);

                var dllDirectory = Path.GetDirectoryName(dllFile);
                if(dllDirectory != null)
                {
                    var dependencyFiles = Directory.GetFiles(dllDirectory, "*.dll")
                        .Where(f => !f.Equals(dllFile, StringComparison.OrdinalIgnoreCase));

                    foreach(var depFile in dependencyFiles)
                    {
                        var tempDepPath = Path.Combine(tempPath, Path.GetFileName(depFile));
                        File.Copy(depFile, tempDepPath, true);
                    }
                }

                var loadContext = new PluginLoadContext(tempDllPath);
                _loadContexts.Add(new WeakReference(loadContext));

                var assembly = loadContext.LoadFromAssemblyPath(tempDllPath);
                var pluginTypes = assembly.GetTypes()
                    .Where(t => typeof(IScoringStrategyPlugin).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

                foreach(var type in pluginTypes)
                {
                    var plugin = Activator.CreateInstance(type) as IScoringStrategyPlugin;
                    if(plugin != null)
                    {
                        _plugins.Add(plugin);
                        _pluginsByIdentifier[plugin.StrategyTypeIdentifier] = plugin;
                    }
                }
            }
            catch(Exception)
            {
            }
        }
    }

    private bool WaitForFileAccess(string filePath, TimeSpan timeout)
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        while(stopwatch.Elapsed < timeout)
        {
            try
            {
                using var stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                return true;
            }
            catch(IOException)
            {
                Thread.Sleep(100);
            }
            catch(Exception)
            {
                return false;
            }
        }

        return false;
    }

    private void UnloadPluginsInternal()
    {
        _plugins.Clear();
        _pluginsByIdentifier.Clear();

        foreach(var contextRef in _loadContexts)
        {
            if(contextRef.IsAlive && contextRef.Target is PluginLoadContext context)
            {
                context.Unload();
            }
        }

        _loadContexts.Clear();

        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
    }

    public void Dispose()
    {
        _reloadTimer?.Dispose();

        if(_watcher != null)
        {
            _watcher.EnableRaisingEvents = false;
            _watcher.Changed -= OnPluginFileChanged;
            _watcher.Created -= OnPluginFileChanged;
            _watcher.Deleted -= OnPluginFileChanged;
            _watcher.Renamed -= OnPluginFileChanged;
            _watcher.Dispose();
        }

        lock(_lock)
        {
            UnloadPluginsInternal();
        }
    }

    // Clase interna para manejar la carga de plugins en contextos aislados
    private sealed class PluginLoadContext(string pluginPath) : AssemblyLoadContext(isCollectible: true)
    {
        private readonly AssemblyDependencyResolver _resolver = new AssemblyDependencyResolver(pluginPath);

        protected override Assembly? Load(AssemblyName assemblyName)
        {
            var assemblyPath = _resolver.ResolveAssemblyToPath(assemblyName);
            if(assemblyPath != null)
            {
                return LoadFromAssemblyPath(assemblyPath);
            }

            return null;
        }

        protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
        {
            var libraryPath = _resolver.ResolveUnmanagedDllToPath(unmanagedDllName);
            if(libraryPath != null)
            {
                return LoadUnmanagedDllFromPath(libraryPath);
            }

            return IntPtr.Zero;
        }
    }
}
