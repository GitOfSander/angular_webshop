using System;
using System.Collections.Generic;

namespace Site.Data
{
    public partial class ShippingZones
    {
        public ShippingZones()
        {
            ShippingZoneLocations = new HashSet<ShippingZoneLocations>();
            ShippingZoneMethods = new HashSet<ShippingZoneMethods>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public int? WebsiteId { get; set; }
        public bool? Default { get; set; }
        public int? PriorityOrder { get; set; }

        public Websites Website { get; set; }
        public ICollection<ShippingZoneLocations> ShippingZoneLocations { get; set; }
        public ICollection<ShippingZoneMethods> ShippingZoneMethods { get; set; }
    }
}
