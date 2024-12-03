namespace Lilith.Realtime.Services;

using StackExchange.Redis;
using Microsoft.AspNetCore.SignalR;
using Lilith.Realtime.Hubs;
using System.Text.Json;

public class RedisService
{
    private readonly IConnectionMultiplexer _redis;
    private readonly IHubContext<RedisHub> _hubContext;

    public RedisService(IConnectionMultiplexer redis, IHubContext<RedisHub> hubContext)
    {
        _redis = redis;
        _hubContext = hubContext;
    }

    public async Task SetCacheAsync(string key, JsonElement value)
    {
        var db = _redis.GetDatabase();

        // Serializar el objeto a JSON
        var serializedValue = JsonSerializer.Serialize(value); 

        // Guardar en Redis
        await db.StringSetAsync(key, serializedValue);

        // Notificar a los suscriptores del tag
        await _hubContext.Clients.Group(key).SendAsync("KeyUpdated", key, value);
    }

    public async Task<JsonElement?> GetCacheAsync(string key)
    {
        var db = _redis.GetDatabase();
        var value = await db.StringGetAsync(key);

        if (!value.HasValue)
        {
            return null;
        }

        return JsonSerializer.Deserialize<JsonElement>(value.ToString());
    }

    public async Task RemoveCacheAsync(string key)
    {
        var db = _redis.GetDatabase();

        // Eliminar el elemento de Redis
        await db.KeyDeleteAsync(key);

        // Notificar a los suscriptores del tag
        await _hubContext.Clients.Group(key).SendAsync("KeyDeleted", key);
    }
}
