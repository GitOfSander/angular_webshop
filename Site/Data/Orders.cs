using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Site.Data
{
    public partial class Orders
    {
        public Orders()
        {
            OrderCoupons = new HashSet<OrderCoupons>();
            OrderFees = new HashSet<OrderFees>();
            OrderLines = new HashSet<OrderLines>();
            OrderRefunds = new HashSet<OrderRefunds>();
            OrderShippingZoneMethods = new HashSet<OrderShippingZoneMethods>();
        }

        public int Id { get; set; }
        public int WebsiteId { get; set; }
        public string BillingFirstName { get; set; } = "";
        public string BillingLastName { get; set; } = "";
        public string BillingCompany { get; set; } = "";
        public string BillingZipCode { get; set; } = "";
        public string BillingCity { get; set; } = "";
        public string BillingCountry { get; set; } = "";
        public string BillingState { get; set; } = "";
        public string BillingAddressLine1 { get; set; } = "";
        public string BillingAddressLine2 { get; set; } = "";
        public string BillingVatNumber { get; set; } = "";
        public string BillingEmail { get; set; } = "";
        public string BillingPhoneNumber { get; set; } = "";
        public string ShippingFirstName { get; set; } = "";
        public string ShippingLastName { get; set; } = "";
        public string ShippingCompany { get; set; } = "";
        public string ShippingZipCode { get; set; } = "";
        public string ShippingCity { get; set; } = "";
        public string ShippingCountry { get; set; } = "";
        public string ShippingState { get; set; } = "";
        public string ShippingAddressLine1 { get; set; } = "";
        public string ShippingAddressLine2 { get; set; } = "";
        public string Note { get; set; } = "";
        public string TransactionId { get; set; } = "";
        public string Status { get; set; } = "";
        public string Currency { get; set; } = "";
        public string OrderNumber { get; set; } = "";
        public string InvoiceNumber { get; set; } = "";

        [MaxLength(100)]
        public string RefInvoiceNumber { get; set; } = "";

        public DateTime CreatedDate { get; set; }
        public string ReserveGuid { get; set; } = "";

        public Websites Website { get; set; }
        public ICollection<OrderCoupons> OrderCoupons { get; set; }
        public ICollection<OrderFees> OrderFees { get; set; }
        public ICollection<OrderLines> OrderLines { get; set; }
        public ICollection<OrderRefunds> OrderRefunds { get; set; }
        public ICollection<OrderShippingZoneMethods> OrderShippingZoneMethods { get; set; }
    }
}
