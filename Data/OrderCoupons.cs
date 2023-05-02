using System;
using System.Collections.Generic;

namespace Site.Data
{
    public partial class OrderCoupons
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int CouponId { get; set; }
        public string Name { get; set; }
        public decimal Discount { get; set; }

        public Orders Order { get; set; }
    }
}
