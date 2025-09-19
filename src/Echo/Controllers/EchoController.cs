using Echo.Hubs;
using Echo.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace Echo.Controllers;

[Produces("application/json")]
[Route("api/Echo")]
public class EchoController : Controller
{
    private readonly IHubContext<EchoHub, IEchoHub> _echoHub;
    private readonly EchoOptions _options;
    private readonly JsonSerializerOptions _jsonSerializerOptions = new() { WriteIndented = true };

    public EchoController(IHubContext<EchoHub, IEchoHub> echoHub, IOptions<EchoOptions> options)
    {
        _options = options.Value;
        _echoHub = echoHub;
    }

    /// <summary>
    /// Gets echo information for a simple GET request.
    /// </summary>
    /// <returns>Echo model containing request metadata.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(EchoModel), StatusCodes.Status200OK)]
    public IActionResult Get()
    {
        var echo = FillEchoModel(string.Empty);
        echo.Uptime = Program.Started;

        if (_options.ShowEnvironmentVariables)
        {
            var environmentVariables = new Dictionary<string, string>();
            foreach (DictionaryEntry item in Environment.GetEnvironmentVariables())
            {
                if (item.Key is string key && item.Value is string value)
                {
                    environmentVariables.Add(key, value);
                }
            }
            echo.EnvironmentVariables = environmentVariables;
        }
        return Ok(echo);
    }

    /// <summary>
    /// Echoes back the request headers and body (if JSON, pretty printed) and pushes the payload to SignalR clients.
    /// </summary>
    /// <remarks>
    /// Send any payload in the body. If Content-Type is application/json, it will be formatted.
    /// </remarks>
    /// <returns>Status 200 OK.</returns>
    [HttpPost]
    [HttpOptions]
    [Consumes("text/plain", "application/x-www-form-urlencoded", "application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> PostAsync()
    {
        using (var stream = new StreamReader(this.HttpContext.Request.Body))
        {
            var body = await stream.ReadToEndAsync();
            var echo = FillEchoModel(body);
            await _echoHub.Clients.All.Echo(echo);
        }

        if (this.HttpContext.Request.Method == "OPTIONS" &&
            this.HttpContext.Request.Headers.ContainsKey("WebHook-Request-Origin"))
        {
            // CloudEvents spec:
            // https://github.com/cloudevents/spec/blob/v1.0/http-webhook.md#41-validation-request
            // We let all the webhooks use this endpoint freely without limits
            this.HttpContext.Response.Headers.Append("WebHook-Allowed-Origin", new StringValues("*"));
            this.HttpContext.Response.Headers.Append("WebHook-Allowed-Rate", new StringValues("*"));
        }
        return Ok();
    }

    private EchoModel FillEchoModel(string body)
    {
        var headers = new Dictionary<string, string>();
        foreach (var item in this.HttpContext.Request.Headers)
        {
            headers.Add(item.Key, item.Value.ToString());
        }

        if (string.IsNullOrEmpty(body) == false &&
            this.HttpContext.Request.HasJsonContentType())
        {
            body = FormatJson(body);
        }

        return new EchoModel()
        {
            Request = $"{this.Request.Method} {this.Request.Path}{this.Request.QueryString} {this.Request.Protocol}",
            Headers = headers,
            Body = body
        };
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
