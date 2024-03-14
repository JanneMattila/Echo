using Echo.Hubs;
using Echo.Models;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Echo.Controllers;

public class PagesController : Controller
{
    private readonly IHubContext<EchoHub, IEchoHub> _echoHub;

    public PagesController(IHubContext<EchoHub, IEchoHub> echoHub)
    {
        _echoHub = echoHub;
    }

    public IActionResult Index()
    {
        var scheme = Request.Scheme;
        if (Request.Headers.TryGetValue("x-arr-ssl", out Microsoft.Extensions.Primitives.StringValues value) &&
            value.Contains("true"))
        {
            scheme = Uri.UriSchemeHttps;
        }
        else if (Request.Headers.TryGetValue("X-AppService-Proto", out Microsoft.Extensions.Primitives.StringValues valueScheme) &&
                 valueScheme.Contains(Uri.UriSchemeHttps))
        {
            scheme = Uri.UriSchemeHttps;
        }
        return View(model: $"{scheme}://{this.Request.Host}{this.Request.PathBase}");
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

        var content = Request.Headers.FirstOrDefault(h => h.Key == "x-custom").Value.ToString();
        if (string.IsNullOrEmpty(content))
        {
            content = Guid.NewGuid().ToString("D");
        }

        ArgumentNullException.ThrowIfNull(content);
        var contentETag = Convert.ToBase64String(Encoding.UTF8.GetBytes(content));

        if (ifNoneMatch.Any() &&
            ifNoneMatch.Contains(contentETag))
        {
            return StatusCode((int)HttpStatusCode.NotModified);
        }

        Response.Headers.Append(HeaderNames.ETag, contentETag);
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
        var headers = new Dictionary<string, string>();
        foreach (var item in this.HttpContext.Request.Headers)
        {
            headers.Add(item.Key, item.Value.ToString());
        }

        var request = $"{this.Request.Method} {this.Request.Path}{this.Request.QueryString} {this.Request.Protocol}";
        await _echoHub.Clients.All.Echo(new EchoModel()
        {
            Request = request,
            Headers = headers,
            Body = string.Empty
        });

        var feature = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();
        var originalPath = feature?.OriginalPath;
        if (!string.IsNullOrEmpty(originalPath))
        {
            var originalRequest = $"Original request was: {feature?.OriginalPath}{feature?.OriginalQueryString}";
            request = request + Environment.NewLine + Environment.NewLine + originalRequest;
        }

        var sb = new StringBuilder();
        sb.AppendLine(request);
        sb.AppendLine();
        sb.AppendLine();
        foreach (var item in headers)
        {
            sb.AppendLine($"{item.Key}: {item.Value}");
        }
        return sb.ToString();
    }

    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
