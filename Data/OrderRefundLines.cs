using System;
using System.Collections.Generic;

namespace Site.Data
{
    public partial class OrderRefundLines
    {
        public int Id { get; set; }
        public int OrderRefundId { get; set; }
        public int OrderLineId { get; set; }
        public decimal TotalPrice { get; set; }
        public int Quantity { get; set; }

        public OrderRefunds OrderRefund { get; set; }
    }
}
