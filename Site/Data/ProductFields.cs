using System;
using System.Collections.Generic;

namespace Site.Data
{
    public partial class ProductFields
    {
        public int Id { get; set; }
        public int ProductTemplateId { get; set; }
        public string CallName { get; set; }
        public string Heading { get; set; }
        public string Type { get; set; }
        public int CustomOrder { get; set; }
        public string DefaultValue { get; set; }

        public ProductTemplates ProductTemplate { get; set; }
    }
}
