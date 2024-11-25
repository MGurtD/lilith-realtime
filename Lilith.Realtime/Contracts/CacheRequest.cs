using System.Text.Json;

namespace Lilith.Realtime.Contracts;

public class CacheRequest
{
    public required string Key { get; set; }
    public required JsonElement Value { get; set; }
    public required string Tag { get; set; }
}