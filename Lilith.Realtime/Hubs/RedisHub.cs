namespace Lilith.Realtime.Hubs;

using Microsoft.AspNetCore.SignalR;

public class RedisHub : Hub
{
    public async Task SubscribeToTag(string tag)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, tag);
    }

    public async Task UnsubscribeFromTag(string tag)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, tag);
    }
}
