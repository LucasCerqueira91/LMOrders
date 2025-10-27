using StackExchange.Redis;
using System.Text.Json;
using LM.Orders.Application.Abstractions;

namespace LM.Orders.Infrastructure.Cache;

public sealed class RedisCacheService : ICacheService, IDisposable
{
    private readonly ConnectionMultiplexer _conn;
    private readonly IDatabase _db;

    public RedisCacheService(string configuration)
    {
        _conn = ConnectionMultiplexer.Connect(configuration);
        _db = _conn.GetDatabase();
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken ct)
    {
        var val = await _db.StringGetAsync(key);
        return val.HasValue ? JsonSerializer.Deserialize<T>(val!) : default;
    }

    public Task SetAsync<T>(string key, T value, TimeSpan ttl, CancellationToken ct) =>
        _db.StringSetAsync(key, JsonSerializer.Serialize(value), ttl);

    public Task RemoveAsync(string key, CancellationToken ct) => _db.KeyDeleteAsync(key);

    public void Dispose() => _conn.Dispose();
}
