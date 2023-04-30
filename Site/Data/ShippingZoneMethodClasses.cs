using System;
using System.Collections.Generic;

namespace Site.Data
{
    public partial class ShippingZoneMethodClasses
    {
        public int Id { get; set; }
        public int ShippingZoneMethodId { get; set; }
        public int ShippingClassId { get; set; }
        public string Cost { get; set; }

        public ShippingZoneMethods ShippingZoneMethod { get; set; }
    }
}
