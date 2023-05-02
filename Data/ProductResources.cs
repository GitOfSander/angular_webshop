using System;
using System.Collections.Generic;

namespace Site.Data
{
    public partial class ProductResources
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int ProductFieldId { get; set; }
        public string Text { get; set; }
        public int? WebsiteLanguageId { get; set; }

        public Products Product { get; set; }
    }
}
