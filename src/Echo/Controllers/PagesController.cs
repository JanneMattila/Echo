﻿using Echo.Hubs;
using Echo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Net.Http.Headers;
using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Echo.Controllers
{
    public class PagesController : Controller
    {
        private readonly IHubContext<EchoHub, IEchoHub> _echoHub;

        public PagesController(IHubContext<EchoHub, IEchoHub> echoHub)
        {
            _echoHub = echoHub;
        }

        public IActionResult Index()
        {
            return View(model: $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}");
        }

        public async Task<IActionResult> Echo()
        {
            var data = await PushToEcho();
            return View(model: data);
        }

        [ResponseCache(Duration = 10, Location = ResponseCacheLocation.Any, VaryByHeader = "x-custom")]
        public async Task<IActionResult> EchoCache()
        {
            var data = await PushToEcho();

            var ifNoneMatch = Request.Headers
                .FirstOrDefault(h => h.Key == HeaderNames.IfNoneMatch).Value;

            var content = Request.Headers
                .FirstOrDefault(h => h.Key == "x-custom").Value;
            if (string.IsNullOrEmpty(content))
            {
                content = Guid.NewGuid().ToString("D");
            }

            var contentETag = Convert.ToBase64String(Encoding.UTF8.GetBytes(content));

            if (ifNoneMatch.Any() && 
                ifNoneMatch.Contains(contentETag))
            {
                return StatusCode((int)HttpStatusCode.NotModified);
            }

            Response.Headers.Add(HeaderNames.ETag, contentETag);
            return View(model: data);
        }

        [ResponseCache(NoStore = true)]
        public async Task<IActionResult> EchoCacheNever()
        {
            var data = await PushToEcho();
            return View(model: data);
        }

        private async Task<string> PushToEcho()
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

            return headers.ToString();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
