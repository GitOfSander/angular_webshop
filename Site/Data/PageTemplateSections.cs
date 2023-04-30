using System;
using System.Collections.Generic;

namespace Site.Data
{
    public partial class PageTemplateSections
    {
        public int Id { get; set; }
        public int PageTemplateId { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public string Section { get; set; }
        public int LinkedToDataTemplateId { get; set; }

        public PageTemplates PageTemplate { get; set; }
    }
}
