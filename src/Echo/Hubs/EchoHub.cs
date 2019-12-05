using Microsoft.AspNetCore.SignalR;

namespace Echo.Hubs
{
    public class EchoHub : Hub<IEchoHub>
    {
    }
}
