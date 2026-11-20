using Echo.Hubs;
using Echo.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using ModelContextProtocol.Server;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Echo.Tools;

[McpServerToolType]
public class EchoTools
{
    private readonly IHubContext<EchoHub, IEchoHub> _echoHub;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public EchoTools(IHubContext<EchoHub, IEchoHub> echoHub, IHttpContextAccessor httpContextAccessor)
    {
        _echoHub = echoHub;
        _httpContextAccessor = httpContextAccessor;
    }

    [McpServerTool(Name = "echo"), Description("Echoes the message back to the caller and broadcasts it to all connected Echo web page clients.")]
    public async Task<string> EchoAsync([Description("Message to echo back.")] string message)
    {
        var echo = CreateEchoModel(message);
        await _echoHub.Clients.All.Echo(echo);
        return message;
    }

    [McpServerTool(Name = "get_request_info", ReadOnly = true), Description("Returns information about the HTTP request used to call this MCP server, including request line, headers and server start time.")]
    public EchoModel GetRequestInfo()
    {
        var echo = CreateEchoModel(string.Empty);
        echo.Uptime = Program.Started;
        return echo;
    }

    private EchoModel CreateEchoModel(string body)
    {
        var headers = new Dictionary<string, string>();
        var request = _httpContextAccessor.HttpContext?.Request;
        if (request == null)
        {
            return new EchoModel() { Request = "MCP", Headers = headers, Body = body };
        }

        foreach (var item in request.Headers)
        {
            headers.Add(item.Key, item.Value.ToString());
        }

        return new EchoModel()
        {
            Request = $"{request.Method} {request.Path}{request.QueryString} {request.Protocol}",
            Headers = headers,
            Body = body
        };
    }
}
