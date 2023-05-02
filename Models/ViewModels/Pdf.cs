using Site.Data;
using System.Collections.Generic;

namespace Site.Models.ViewModels
{
    public class PdfViewModel
    {
        public string Website { get; set; } = "";
        public string AddressLine1 { get; set; } = "";
        public string ZipCode { get; set; } = "";
        public string City { get; set; } = "";
        public string Country { get; set; } = "";
        public string Coc { get; set; } = "";
        public string Vat { get; set; } = "";
        public string BillingCompany { get; set; } = "";
        public string BillingEmail { get; set; } = "";
        public string BillingPhoneNumber { get; set; } = "";
        public string BillingVatNumber { get; set; } = "";
        public string BillingName { get; set; } = "";
        public string BillingAddressLine1 { get; set; } = "";
        public string BillingAddressLine2 { get; set; } = "";
        public string BillingZipCode { get; set; } = "";
        public string BillingCity { get; set; } = "";
        public string BillingCountry { get; set; } = "";
        public string BillingState { get; set; } = "";
        public string ShippingCompany { get; set; } = "";
        public string ShippingName { get; set; } = "";
        public string ShippingAddressLine1 { get; set; } = "";
        public string ShippingAddressLine2 { get; set; } = "";
        public string ShippingZipCode { get; set; } = "";
        public string ShippingCity { get; set; } = "";
        public string ShippingCountry { get; set; } = "";
        public string ShippingState { get; set; } = "";
        public string Date { get; set; } = "";
        public string OrderNumber { get; set; } = "";
        public string InvoiceNumber { get; set; } = "";
        public string Status { get; set; } = "";
        public string Subtotal { get; set; } = "";
        public string Tax { get; set; } = "";
        public decimal ShippingCosts { get; set; }
        public string TotalExclusive { get; set; } = "";
        public string Total { get; set; } = "";
        public string Note { get; set; } = "";
        public string PriceFormat { get; set; } = "";
        public string DigitsFormat { get; set; } = "";
        public int DigitsAfterDecimal { get; set; }
        public IEnumerable<OrderLines> OrderLines { get; set; }
        public IEnumerable<OrderFees> OrderFees { get; set; }
        public Dictionary<string, object> TaxClasses { get; set; }
    }
}
