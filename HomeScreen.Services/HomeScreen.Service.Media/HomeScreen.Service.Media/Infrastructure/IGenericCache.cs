namespace HomeScreen.Service.Media.Infrastructure;

public interface IGenericCache
{
    Task WriteCache<T>(string key, T entry, CancellationToken cancellationToken = default);

    Task WriteCache(string key, string entry, CancellationToken cancellationToken = default);

    Task<T?> ReadCache<T>(string key, CancellationToken cancellationToken = default) where T : class;

    Task<string?> ReadCache(string key, CancellationToken cancellationToken = default);
}
