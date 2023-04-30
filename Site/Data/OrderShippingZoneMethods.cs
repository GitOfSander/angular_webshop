using System;
using System.Collections.Generic;

namespace Site.Data
{
    public partial class OrderShippingZoneMethods
    {
        public int Id { get; set; }
        public int ShippingZoneMethodId { get; set; }
        public int OrderId { get; set; }
        public string Name { get; set; } = "";
        public Decimal Price { get; set; } = 0;
        public bool? Taxable { get; set; } = true;

        public Orders Order { get; set; }
    }
}
