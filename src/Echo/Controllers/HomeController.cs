using Echo.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Echo.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View(model: $"{this.Request.Scheme}://{this.Request.Host}");
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
