using System;
using System.Collections.Generic;

namespace Site.Data
{
    public partial class OauthTokens
    {
        public int Id { get; set; }
        public int WebsiteId { get; set; }
        public string CallName { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }

        public Websites Website { get; set; }
    }
}
