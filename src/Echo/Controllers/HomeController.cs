using Echo.Hubs;
using Echo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace Echo.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHubContext<EchoHub, IEchoHub> _echoHub;

        public HomeController(IHubContext<EchoHub, IEchoHub> echoHub)
        {
            _echoHub = echoHub;
        }

        public IActionResult Index()
        {
            return View(model: $"{this.Request.Scheme}://{this.Request.Host}");
        }

        public async Task<IActionResult> Echo()
        {
            var headers = new StringBuilder();
            foreach (var item in this.HttpContext.Request.Headers)
            {
                headers.AppendLine($"{item.Key}: {item.Value}");
            }

            await _echoHub.Clients.All.Echo(new EchoModel()
            {
                Request = $"{this.Request.Method} {this.Request.Path}{this.Request.QueryString} {this.Request.Protocol}",
                Headers = headers.ToString(),
                Body = string.Empty
            });

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
