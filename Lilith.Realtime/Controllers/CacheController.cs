namespace Lilith.Realtime.Controllers;

using Lilith.Realtime.Contracts;
using Lilith.Realtime.Services;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/cache")]
public class CacheController : ControllerBase
{
    private readonly RedisService _redisService;

    public CacheController(RedisService redisService)
    {
        _redisService = redisService;
    }

    [HttpPost]
    public async Task<IActionResult> SetCache([FromBody] CacheRequest request)
    {
        await _redisService.SetCacheAsync(request.Key, request.Value);
        return Ok();
    }

    [HttpGet("{key}")]
    public async Task<IActionResult> GetCache(string key)
    {
        var value = await _redisService.GetCacheAsync(key);
        return value is not null ? Ok(value) : NotFound();
    }

    [HttpDelete("{key}")]
    public async Task<IActionResult> RemoveCache(string key)
    {
        await _redisService.RemoveCacheAsync(key);
        return NoContent();
    }
}


