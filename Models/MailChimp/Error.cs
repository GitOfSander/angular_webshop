using Newtonsoft.Json;

namespace Site.Models.MailChimp
{
    public class Error
    {
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("status")]
        public int Status { get; set; }
        [JsonProperty("detail")]
        public string Detail { get; set; }
        [JsonProperty("instance")]
        public string Instance { get; set; }
        [JsonProperty("errors")]
        public string Errors { get; set; }
    }
}
