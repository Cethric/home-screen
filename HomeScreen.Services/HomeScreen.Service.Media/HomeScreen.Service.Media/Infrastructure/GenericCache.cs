using System.Diagnostics;
using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;

namespace HomeScreen.Service.Media.Infrastructure;

public class GenericCache(IDistributedCache distributedCache) : IGenericCache
{
    private static ActivitySource ActivitySource => new(nameof(GenericCache));

public async Task WriteCache<T>(string key, T entry, CancellationToken cancellationToken = default)
{
    using var activity = ActivitySource.StartActivity();
    using var writeStream = new MemoryStream();
    var result = JsonSerializer.Serialize(entry, JsonSerializerOptions.Default);
    await distributedCache.SetStringAsync(key, result, cancellationToken);
}

public async Task WriteCache(string key, string entry, CancellationToken cancellationToken = default) =>
    await distributedCache.SetStringAsync(key, entry, cancellationToken);

public async Task<T?> ReadCache<T>(string key, CancellationToken cancellationToken = default) where T : class
{
    using var activity = ActivitySource.StartActivity();
    var cache = await distributedCache.GetStringAsync(key, cancellationToken);
    if (string.IsNullOrEmpty(cache))
    {
        return null;
    }

    var result = JsonSerializer.Deserialize<T>(cache, JsonSerializerOptions.Default);
    return result ?? null;
}

public async Task<string?> ReadCache(string key, CancellationToken cancellationToken = default) =>
    await distributedCache.GetStringAsync(key, cancellationToken);
}
