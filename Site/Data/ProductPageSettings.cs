using System;
using System.Collections.Generic;

namespace Site.Data
{
    public partial class ProductPageSettings
    {
        public int Id { get; set; }
        public int WebsiteLanguageId { get; set; }
        public int ProductId { get; set; }
        public string Url { get; set; }
        public string Title { get; set; }
        public string Keywords { get; set; }
        public string Description { get; set; }
        public bool Active { get; set; }

        public Products Product { get; set; }
    }
}
