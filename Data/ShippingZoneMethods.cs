using System;
using System.Collections.Generic;

namespace Site.Data
{
    public partial class ShippingZoneMethods
    {
        public ShippingZoneMethods()
        {
            ShippingZoneMethodClasses = new HashSet<ShippingZoneMethodClasses>();
        }

        public int Id { get; set; }
        public int ShippingZoneId { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public bool Taxable { get; set; }
        public string Cost { get; set; }
        public string CalculationType { get; set; }
        public string FreeShippingType { get; set; }
        public decimal MinimumAmount { get; set; }
        public bool Active { get; set; }
        public int? CustomOrder { get; set; }

        public ShippingZones ShippingZone { get; set; }
        public ICollection<ShippingZoneMethodClasses> ShippingZoneMethodClasses { get; set; }
    }
}
