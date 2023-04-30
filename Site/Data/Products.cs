using System;
using System.Collections.Generic;

namespace Site.Data
{
    public partial class Products
    {
        public Products()
        {
            ProductFiles = new HashSet<ProductFiles>();
            ProductPageSettings = new HashSet<ProductPageSettings>();
            ProductPages = new HashSet<ProductPages>();
            ProductResources = new HashSet<ProductResources>();
        }

        public int Id { get; set; }
        public int ProductTemplateId { get; set; }
        public int TaxClassId { get; set; }
        public int ShippingClassId { get; set; }
        public decimal Price { get; set; }
        public decimal PromoPrice { get; set; }
        public DateTime PromoFromDate { get; set; }
        public DateTime PromoToDate { get; set; }
        public string TaxStatus { get; set; }
        public string Sku { get; set; }
        public bool ManageStock { get; set; }
        public string StockStatus { get; set; }
        public int StockQuantity { get; set; }
        public string Backorders { get; set; }
        public int MaxPerOrder { get; set; }
        public decimal Weight { get; set; }
        public decimal Length { get; set; }
        public decimal Width { get; set; }
        public decimal Height { get; set; }
        public int DownloadLimit { get; set; }
        public int DownloadExpire { get; set; }
        public bool Reviews { get; set; }
        public bool? Active { get; set; }
        public bool? PromoSchedule { get; set; }
        public string Name { get; set; }
        public int? CustomOrder { get; set; }

        public ProductTemplates ProductTemplate { get; set; }
        public ICollection<ProductFiles> ProductFiles { get; set; }
        public ICollection<ProductPageSettings> ProductPageSettings { get; set; }
        public ICollection<ProductPages> ProductPages { get; set; }
        public ICollection<ProductResources> ProductResources { get; set; }
    }
}
