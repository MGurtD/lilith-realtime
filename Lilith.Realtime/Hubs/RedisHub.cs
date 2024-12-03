namespace Lilith.Realtime.Hubs;

using Microsoft.AspNetCore.SignalR;

public class RedisHub : Hub
{
    public async Task SubscribeToKey(string key)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, key);
    }

    public async Task UnsubscribeFromTag(string key)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, key);
    }
}
