using System;
using System.Collections.Generic;

namespace Site.Data
{
    public partial class OrderLines
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public string Name { get; set; } = "";
        public int Quantity { get; set; } = 0;
        public Decimal Price { get; set; } = 0.00M;
        public Decimal Discount { get; set; } = 0.00M;
        public Decimal TaxRate { get; set; } = 0.00M;
        public bool? TaxShipping { get; set; } = true;

        public Orders Order { get; set; }
    }
}
