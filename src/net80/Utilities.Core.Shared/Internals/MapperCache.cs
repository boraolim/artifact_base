namespace Utilities.Core.Shared.Internals;

internal sealed class MapperCache
{
    private readonly object _lock = new();
    private readonly Dictionary<(Type, Type), Delegate> _cache = new();

    public Delegate GetOrAdd((Type, Type) key, Func<Delegate> factory)
    {
        if(_cache.TryGetValue(key, out var existing))
            return existing;

        lock(_lock)
        {
            if(!_cache.TryGetValue(key, out existing))
            {
                existing = factory();
                _cache[key] = existing;
            }
        }

        return existing;
    }
}

