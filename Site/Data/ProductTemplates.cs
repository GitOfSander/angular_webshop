using System;
using System.Collections.Generic;

namespace Site.Data
{
    public partial class ProductTemplates
    {
        public ProductTemplates()
        {
            ProductFields = new HashSet<ProductFields>();
            ProductUploads = new HashSet<ProductUploads>();
            Products = new HashSet<Products>();
        }

        public int Id { get; set; }
        public int WebsiteId { get; set; }
        public string Name { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public bool? Attributes { get; set; }
        public bool? Reviews { get; set; }
        public bool? Virtual { get; set; }
        public bool? Downloadable { get; set; }
        public bool? Upsells { get; set; }
        public bool? CrossSells { get; set; }
        public bool? SimpleProduct { get; set; }
        public bool? GroupedProduct { get; set; }
        public bool? ExternalProduct { get; set; }
        public bool? VariableProduct { get; set; }

        public Websites Website { get; set; }
        public ICollection<ProductFields> ProductFields { get; set; }
        public ICollection<ProductUploads> ProductUploads { get; set; }
        public ICollection<Products> Products { get; set; }
    }
}
