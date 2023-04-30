using System;
using System.Collections.Generic;

namespace Site.Data
{
    public partial class OrderRefunds
    {
        public OrderRefunds()
        {
            OrderRefundLines = new HashSet<OrderRefundLines>();
        }

        public int Id { get; set; }
        public int OrderId { get; set; }
        public string UserId { get; set; }
        public string Name { get; set; }
        public decimal Refund { get; set; }
        public string Reason { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string InvoiceNumber { get; set; }

        public Orders Order { get; set; }
        public ICollection<OrderRefundLines> OrderRefundLines { get; set; }
    }
}
