using System.Collections.Generic;
using Newtonsoft.Json;

namespace Site.Models.MailChimp.Lists.Members.Notes
{
    public class Note
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("note_id")]
        public int NoteId { get; set; }
        [JsonProperty("created_at")]
        public string CreatedAt { get; set; }
        [JsonProperty("created_by")]
        public string CreatedBy { get; set; }
        [JsonProperty("updated_at")]
        public string UpdatedAt { get; set; }
        [JsonProperty("note")]
        public string Content { get; set; }
        [JsonProperty("list_id")]
        public string ListId { get; set; }
        [JsonProperty("email_id")]
        public string EmailId { get; set; }
        [JsonProperty("_links")]
        public List<Link> Link { get; set; }
    }
}
