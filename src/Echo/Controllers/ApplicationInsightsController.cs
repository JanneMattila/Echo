using Echo.Hubs;
using Echo.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Echo.Controllers;

[Produces("application/json")]
[Route("v2/track")]
[Route("v2.1/track")]
public class ApplicationInsightsController : Controller
{
    private readonly IHubContext<EchoHub, IEchoHub> _echoHub;
    private readonly EchoOptions _options;
    private readonly JsonSerializerOptions _jsonSerializerOptions = new() { WriteIndented = true };

    public ApplicationInsightsController(IHubContext<EchoHub, IEchoHub> echoHub, IOptions<EchoOptions> options)
    {
        _options = options.Value;
        _echoHub = echoHub;
    }

    [HttpPost]
    [HttpOptions]
    public async Task<ActionResult> PostAsync()
    {
        using (var stream = new MemoryStream())
        {
            await this.HttpContext.Request.Body.CopyToAsync(stream);
            byte[] bodyBytes = stream.ToArray();

            var echo = FillEchoModel(bodyBytes);
            await _echoHub.Clients.All.Echo(echo);
        }

        return Ok();
    }

    private EchoModel FillEchoModel(byte[] bodyBytes)
    {
        var headers = new Dictionary<string, string>();
        foreach (var item in this.HttpContext.Request.Headers)
        {
            headers.Add(item.Key, item.Value.ToString());
        }

        if (headers.TryGetValue("Content-Encoding", out string? value) && value == "gzip")
        {
            bodyBytes = GzipDecompress(bodyBytes);
        }

        var body = Encoding.UTF8.GetString(bodyBytes);

        if (string.IsNullOrEmpty(body) == false &&
            this.HttpContext.Request.HasJsonContentType())
        {
            body = FormatJson(body);
        }
        else if (string.IsNullOrEmpty(body) == false &&
                 this.HttpContext.Request.ContentType == "application/x-json-stream")
        {
            var sb = new StringBuilder();
            body.Split('\n').ToList().ForEach(line =>
            {
                sb.AppendLine(FormatJson(line));
            });
            body = sb.ToString();
        }

        return new EchoModel()
        {
            Request = $"{this.Request.Method} {this.Request.Path}{this.Request.QueryString} {this.Request.Protocol}",
            Headers = headers,
            Body = body
        };
    }

    private static byte[] GzipDecompress(byte[] bodyBytes)
    {
        using var memoryStream = new MemoryStream(bodyBytes);
        using var gzipStream = new GZipStream(memoryStream, CompressionMode.Decompress);
        using var resultStream = new MemoryStream();
        gzipStream.CopyTo(resultStream);
        return resultStream.ToArray();
    }

    private string FormatJson(string json)
    {
        try
        {
            using var document = JsonDocument.Parse(json);
            return JsonSerializer.Serialize(document, _jsonSerializerOptions);
        }
        catch (Exception)
        {
            return json;
        }
    }
}
