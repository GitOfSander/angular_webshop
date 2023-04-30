using Newtonsoft.Json;

namespace Site.Models.MailChimp.Lists.Interests
{
    public class Interest
    {
        [JsonProperty("list_id")]
        public string ListId { get; set; }
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("display_order")]
        public string DisplayOrder { get; set; }
        [JsonProperty("type")]
        public string Type { get; set; }
    }
}
