using System;
using System.Collections.Generic;

namespace Site.Data
{
    public partial class ShippingClasses
    {
        public int Id { get; set; }
        public int WebsiteId { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public string Description { get; set; }

        public Websites Website { get; set; }
    }
}
