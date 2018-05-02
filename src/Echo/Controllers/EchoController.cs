using Echo.Hubs;
using Echo.Models;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Echo.Controllers
{
    [Produces("application/json")]
    [Route("api/Echo")]
    public class EchoController : Controller
    {
        private readonly EchoHubShared _echoHubShared;

        public EchoController(EchoHubShared echoHubShared)
        {
            _echoHubShared = echoHubShared;
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

                await _echoHubShared.Echo(new EchoModel()
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