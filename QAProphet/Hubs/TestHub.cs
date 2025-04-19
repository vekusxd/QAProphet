using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using QAProphet.Extensions;

namespace QAProphet.Hubs;

[Authorize]
public class TestHub : Hub
{
    public async Task Send(string message)
    {
        var userName = Context!.User!.GetUserId();
        await Clients.All.SendAsync("ReceiveMessage", $"{userName}: {message}");
    }
}