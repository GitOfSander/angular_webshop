using Newtonsoft.Json;

namespace Site.Models.MailChimp.Lists
{
    public class Stat
    {
        [JsonProperty("member_count")]
        public int MemberCount { get; set; }
        [JsonProperty("unsubscribe_count")]
        public int UnsubscribeCount { get; set; }
        [JsonProperty("cleaned_count")]
        public int CleanedCount { get; set; }
        [JsonProperty("member_count_since_send")]
        public int MemberCountSinceSend { get; set; }
        [JsonProperty("unsubscribe_count_since_send")]
        public int UnsubscribeCountSinceSend { get; set; }
        [JsonProperty("cleaned_count_since_send")]
        public int CleanedCountSinceSend { get; set; }
        [JsonProperty("campaign_count")]
        public int CampaignCount { get; set; }
        [JsonProperty("campaign_last_sent")]
        public string CampaignLastSent { get; set; }
        [JsonProperty("merge_field_count")]
        public int MergeFieldCount { get; set; }
        [JsonProperty("avg_sub_rate")]
        public byte AvgSubRate { get; set; }
        [JsonProperty("avg_unsub_rate")]
        public byte AvgUnsubRate { get; set; }
        [JsonProperty("target_sub_rate")]
        public byte TargetSubRate { get; set; }
        [JsonProperty("open_rate")]
        public byte OpenRate { get; set; }
        [JsonProperty("click_rate")]
        public byte ClickRate { get; set; }
        [JsonProperty("last_sub_date")]
        public string LastSubDate { get; set; }
        [JsonProperty("last_unsub_date")]
        public string LastUnsubDate { get; set; }
    }
}
