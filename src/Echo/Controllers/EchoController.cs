using Echo.Hubs;
using Echo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Primitives;
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

        public EchoController(IHubContext<EchoHub, IEchoHub> echoHub)
        {
            _echoHub = echoHub;
        }

        public async Task<ActionResult> ExecuteAsync()
        {
            using (var stream = new StreamReader(this.HttpContext.Request.Body))
            {
                var body = await stream.ReadToEndAsync();
                var headers = new StringBuilder();
                foreach (var item in this.HttpContext.Request.Headers)
                {
                    headers.AppendLine($"{item.Key}: {item.Value}");
                }

                await _echoHub.Clients.All.Echo(new EchoModel()
                {
                    Request = $"{this.Request.Method} {this.Request.Path}{this.Request.QueryString} {this.Request.Protocol}",
                    Headers = headers.ToString(),
                    Body = body
                });
            }

            if (this.HttpContext.Request.Method == "OPTIONS" &&
                this.HttpContext.Request.Headers.ContainsKey("WebHook-Request-Origin"))
            {
                // CloudEvents spec: 
                // https://github.com/cloudevents/spec/blob/v1.0/http-webhook.md#41-validation-request
                // We let all the webhooks use this endpoint freely without limits
                this.HttpContext.Response.Headers.Add("WebHook-Allowed-Origin", new StringValues("*"));
                this.HttpContext.Response.Headers.Add("WebHook-Allowed-Rate", new StringValues("*"));
            }
            return Ok();
        }
    }
}