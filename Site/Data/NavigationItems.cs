using System;
using System.Collections.Generic;

namespace Site.Data
{
    public partial class NavigationItems
    {
        public int Id { get; set; }
        public int WebsiteLanguageId { get; set; }
        public int NavigationId { get; set; }
        public int Parent { get; set; }
        public string LinkedToType { get; set; }
        public int LinkedToSectionId { get; set; }
        public string LinkedToAlternateGuid { get; set; }
        public string FilterAlternateGuid { get; set; }
        public string Name { get; set; }
        public string Target { get; set; }
        public string CustomUrl { get; set; }
        public int CustomOrder { get; set; }

        public Navigations Navigation { get; set; }
    }
}
