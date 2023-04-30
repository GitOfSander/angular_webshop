using System;
using System.Collections.Generic;

namespace Site.Data
{
    public partial class ProductPages
    {
        public int Id { get; set; }
        public int WebsiteLanguageId { get; set; }
        public int ProductId { get; set; }
        public string PageAlternateGuid { get; set; }

        public Products Product { get; set; }
    }
}
