using Echo.Models;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace Echo.Hubs
{
    public class EchoHubShared
    {
        private readonly IHubContext<EchoHub> _hub;

        public EchoHubShared(IHubContext<EchoHub> hub)
        {
            _hub = hub;
        }

        public async Task Echo(EchoModel echoModel)
        {
            await _hub.Clients.All.SendAsync("echo", echoModel);
        }
    }
}
