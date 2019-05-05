using Newtonsoft.Json;

namespace SmartDoctor.Data.Models
{
    public class MksResponse
    {
        public bool Success { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Data { get; set; }
    }
}
