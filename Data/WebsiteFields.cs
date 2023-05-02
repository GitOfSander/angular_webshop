using System;
using System.Collections.Generic;

namespace Site.Data
{
    public partial class WebsiteFields
    {
        public int Id { get; set; }
        public int WebsiteId { get; set; }
        public string CallName { get; set; }
        public string Heading { get; set; }
        public string Type { get; set; }
        public int CustomOrder { get; set; }
        public string DefaultValue { get; set; }
        public bool? DeveloperOnly { get; set; }

        public Websites Website { get; set; }
    }
}
