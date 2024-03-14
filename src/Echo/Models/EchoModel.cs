using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Echo.Models;

public class EchoModel
{
    [JsonPropertyName("request")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string Request { get; internal set; } = string.Empty;

    [JsonPropertyName("uptime")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public DateTime Uptime { get; internal set; }

    [JsonPropertyName("headers")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public Dictionary<string, string> Headers { get; internal set; } = [];

    [JsonPropertyName("environmentVariables")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public Dictionary<string, string>? EnvironmentVariables { get; internal set; }

    [JsonPropertyName("body")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string Body { get; set; } = string.Empty;
}
