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

    public async Task SetCacheAsync(string key, JsonElement value, string tag)
    {
        var db = _redis.GetDatabase();

        // Serializar el objeto a JSON
        var serializedValue = JsonSerializer.Serialize(value); 
        // Guardar en Redis
        await db.StringSetAsync(key, serializedValue);

        // Asociar la clave al tag
        await db.SetAddAsync(tag, key);

        // Notificar a los suscriptores del tag
        await _hubContext.Clients.Group(tag).SendAsync("TagUpdated", tag, key, value);
    }

    public async Task<string?> GetCacheAsync(string key)
    {
        var db = _redis.GetDatabase();
        return await db.StringGetAsync(key);
    }

    public async Task RemoveCacheAsync(string key, string tag)
    {
        var db = _redis.GetDatabase();

        // Eliminar el elemento de Redis
        await db.KeyDeleteAsync(key);

        // Notificar a los suscriptores del tag
        await _hubContext.Clients.Group(tag).SendAsync("TagDeleted", tag, key);
    }
}
