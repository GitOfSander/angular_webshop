using Newtonsoft.Json;

namespace Site.Models.MailChimp.Lists.Members
{
    public class Stat
    {
        [JsonProperty("avg_open_rate")]
        public double AvgOpenRate { get; set; }
        [JsonProperty("avg_click_rate")]
        public double AvgClickRate { get; set; }
    }
}
