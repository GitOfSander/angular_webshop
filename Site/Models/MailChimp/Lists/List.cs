using System.Collections.Generic;
using Newtonsoft.Json;

namespace Site.Models.MailChimp.Lists
{
    public class List
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("contact")]
        public Contact Contact { get; set; }
        [JsonProperty("permission_reminder")]
        public string PermissionReminder { get; set; }
        [JsonProperty("use_archive_bar")]
        public bool? UseArchiveBar { get; set; }
        [JsonProperty("campaign_defaults")]
        public CampaignDefault CampaignDefaults { get; set; }
        [JsonProperty("notify_on_subscribe")]
        public string NotifyOnSubscribe { get; set; }
        [JsonProperty("notify_on_unsubscribe")]
        public string NotifyOnUnsubscribe { get; set; }
        [JsonProperty("date_created")]
        public string DateCreated { get; set; }
        [JsonProperty("list_rating")]
        public int? ListRating { get; set; }
        [JsonProperty("email_type_option")]
        public bool? EmailTypeOption { get; set; }
        [JsonProperty("subscribe_url_short")]
        public string SubscribeUrlShort { get; set; }
        [JsonProperty("subscribe_url_long")]
        public string SubscribeUrlLong { get; set; }
        [JsonProperty("beamer_address")]
        public string BeamerAddress { get; set; }
        [JsonProperty("visibility")]
        public string Visibility { get; set; }
        [JsonProperty("modules")]
        public string[] Modules { get; set; }
        [JsonProperty("stats")]
        public Stat Stats { get; set; }
        [JsonProperty("_links")]
        public List<Link> Links { get; set; }
    }
    public class ListQuery
    {
        [JsonProperty("fields")]
        public string[] Fields { get; set; }
        [JsonProperty("exclude_fields")]
        public string[] ExcludeFields { get; set; }
    }
    public class CollectionList
    {
        [JsonProperty("lists")]
        public List<List> Lists { get; set; }
        [JsonProperty("total_items")]
        public int TotalItems { get; set; }
    }
    public class CollectionListQuery
    {
        [JsonProperty("fields")]
        public string[] Fields { get; set; }
        [JsonProperty("exclude_fields")]
        public string[] ExcludeFields { get; set; }
        [JsonProperty("count")]
        public int Count { get; set; }
        [JsonProperty("offset")]
        public int Offset { get; set; }
        [JsonProperty("before_date_created")]
        public string BeforeDateCreated { get; set; }
        [JsonProperty("since_date_created")]
        public string SinceDateCreated { get; set; }
        [JsonProperty("before_campaign_last_sent")]
        public string BeforeCampaignLastSent { get; set; }
        [JsonProperty("since_campaign_last_sent")]
        public string SinceCampaignLastSent { get; set; }
    }
}