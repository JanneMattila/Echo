using System.Text.Json.Serialization;

namespace Echo.Models
{
    public class EchoModel
    {
        [JsonPropertyName("request")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string Request { get; internal set; } = string.Empty;

        [JsonPropertyName("headers")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string Headers { get; internal set; } = string.Empty;

        [JsonPropertyName("body")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string Body { get; set; } = string.Empty;
    }
}
