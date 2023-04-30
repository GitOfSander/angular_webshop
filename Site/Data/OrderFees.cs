using System;
using System.Collections.Generic;

namespace Site.Data
{
    public partial class OrderFees
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public decimal? TaxRate { get; set; }

        public Orders Order { get; set; }
    }
}
