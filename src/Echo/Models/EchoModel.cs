using Newtonsoft.Json;

namespace Echo.Models
{
    public class EchoModel
    {
        [JsonProperty(PropertyName = "request", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public string Request { get; internal set; } = string.Empty;

        [JsonProperty(PropertyName = "headers", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public string Headers { get; internal set; } = string.Empty;

        [JsonProperty(PropertyName = "body", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public string Body { get; set; } = string.Empty;
    }
}
