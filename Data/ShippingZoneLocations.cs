using System;
using System.Collections.Generic;

namespace Site.Data
{
    public partial class ShippingZoneLocations
    {
        public int Id { get; set; }
        public int ShippingZoneId { get; set; }
        public string Code { get; set; }
        public string Type { get; set; }

        public ShippingZones ShippingZone { get; set; }
    }
}
