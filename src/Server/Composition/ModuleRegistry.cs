namespace GameNet.Server.Composition;

public sealed class ModuleRegistry
{
    private readonly List<IGameNetModule> _modules = [];

    public IReadOnlyList<IGameNetModule> Modules => _modules;

    public void Register(IGameNetModule module)
    {
        ArgumentNullException.ThrowIfNull(module);

        if (_modules.Any(x => string.Equals(x.Name, module.Name, StringComparison.Ordinal)))
            throw new InvalidOperationException($"Duplicate GameNet module: {module.Name}");

        _modules.Add(module);
    }
}
