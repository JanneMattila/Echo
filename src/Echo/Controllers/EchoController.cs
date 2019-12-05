using Echo.Hubs;
using Echo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Echo.Controllers
{
    [Produces("application/json")]
    [Route("api/Echo")]
    public class EchoController : Controller
    {
        private readonly IHubContext<EchoHub, IEchoHub> _echoHub;

        public EchoController(IHubContext<EchoHub, IEchoHub> echoHubShared)
        {
            _echoHub = echoHubShared;
        }

        public async Task<ActionResult> ExecuteAsync()
        {
            using (var stream = new StreamReader(this.HttpContext.Request.Body))
            {
                var body = await stream.ReadToEndAsync();
                var headers = new StringBuilder();
                foreach (var item in this.HttpContext.Request.Headers)
                {
                    headers.AppendLine($"{item.Key}: {item.Value.ToString()}");
                }

                await _echoHub.Clients.All.Echo(new EchoModel()
                {
                    Request = $"{this.Request.Method} {this.Request.Path}{this.Request.QueryString} {this.Request.Protocol}",
                    Headers = headers.ToString(),
                    Body = body
                });
            }

            return Ok();
        }
    }
}