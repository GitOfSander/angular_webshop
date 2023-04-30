using System;
using System.Collections.Generic;

namespace Site.Data
{
    public partial class ApiKeys
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public string Permissions { get; set; }
        public DateTime LastAccess { get; set; }
        public int? WebsiteId { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string TruncatedKey { get; set; }

        public Websites Website { get; set; }
    }
}
