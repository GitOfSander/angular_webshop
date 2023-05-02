using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Site.Models.MailChimp.Lists.Interests;
using Site.Models.MailChimp.Lists.Members.Notes;
using Newtonsoft.Json;

namespace Site.Models.MailChimp.Lists.Members
{
    public class Member
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("email_address")]
        public string EmailAddress { get; set; }
        [JsonProperty("unique_email_id")]
        public string UniqueEmailId { get; set; }
        [JsonProperty("email_type")]
        public string EmailType { get; set; }
        [JsonProperty("status")]
        public string Status { get; set; }
        [JsonProperty("merge_fields")]
        public Dictionary<string, string> MergeFields { get; set; }
        [JsonProperty("interests")]
        public Interest Interests { get; set; }
        [JsonProperty("stats")]
        public Stat Stats { get; set; }
        [JsonProperty("ip_signup")]
        public string IpSignup { get; set; }
        [JsonProperty("timestamp_signup")]
        public string TimestampSignup { get; set; }
        [JsonProperty("ip_opt")]
        public string IpOpt { get; set; }
        [JsonProperty("timestamp_opt")]
        public string TimestampOpt { get; set; }
        [JsonProperty("member_rating")]
        public byte? MemberRating { get; set; }
        [JsonProperty("last_changed")]
        public string LastChanged { get; set; }
        [JsonProperty("language")]
        public string Language { get; set; }
        [JsonProperty("vip")]
        public bool? Vip { get; set; }
        [JsonProperty("email_client")]
        public string EmailClient { get; set; }
        [JsonProperty("location")]
        public Location Location { get; set; }
        [JsonProperty("last_note")]
        public Note LastNote { get; set; }
        [JsonProperty("list_id")]
        public string ListId { get; set; }
        [JsonProperty("_links")]
        public List<Link> Links { get; set; }

        public string GetSubscriberHash()
        {
            using (var md5Hash = MD5.Create())
            {
                var data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(EmailAddress));
                var sb = new StringBuilder();
                for (var i = 0; i < data.Length; i++)
                {
                    sb.Append(data[i].ToString("x2"));
                }
                return sb.ToString();
            }
        }
    }
    public class MemberQuery
    {
        [JsonProperty("fields")]
        public string[] Fields { get; set; }
        [JsonProperty("exclude_fields")]
        public string[] ExcludeFields { get; set; }
    }
    public class CollectionMemberParent
    {
        [JsonProperty("list_id")]
        public string ListId { get; set; }
    }
    public class CollectionMember : CollectionMemberParent
    {
        [JsonProperty("members")]
        public List<Member> Members { get; set; }
        [JsonProperty("total_items")]
        public int TotalItems { get; set; }
    }
    public class CollectionMemberQuery : CollectionMemberParent
    {
        [JsonProperty("fields")]
        public string[] Fields { get; set; }
        [JsonProperty("exclude_fields")]
        public string[] ExcludeFields { get; set; }
        [JsonProperty("count")]
        public int Count { get; set; }
        [JsonProperty("offset")]
        public int Offset { get; set; }
        [JsonProperty("email_type")]
        public string EmailType { get; set; }
        [JsonProperty("status")]
        public string Status { get; set; }
        [JsonProperty("since_timestamp_opt")]
        public string SinceTimestampOpt { get; set; }
        [JsonProperty("before_timestamp_opt")]
        public string BeforeTimestampOpt { get; set; }
        [JsonProperty("since_last_changed")]
        public string SinceLastChanged { get; set; }
        [JsonProperty("before_last_changed")]
        public string BeforeLastChanged { get; set; }
    }
}
