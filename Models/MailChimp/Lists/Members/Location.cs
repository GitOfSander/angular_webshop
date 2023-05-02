using Newtonsoft.Json;

namespace Site.Models.MailChimp.Lists.Members
{
    public class Location
    {
        [JsonProperty("latitude")]
        public double Latitude { get; set; }
        [JsonProperty("longitude")]
        public double Longitude { get; set; }
        [JsonProperty("gmtoff")]
        public int Gmtoff { get; set; }
        [JsonProperty("dstoff")]
        public int Dstoff { get; set; }
        [JsonProperty("country_code")]
        public string CountryCode { get; set; }
        [JsonProperty("timezone")]
        public string Timezone { get; set; }
    }
}
