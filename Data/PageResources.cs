using System;
using System.Collections.Generic;

namespace Site.Data
{
    public partial class PageResources
    {
        public int Id { get; set; }
        public int PageId { get; set; }
        public int PageTemplateFieldId { get; set; }
        public string Text { get; set; }

        public Pages Page { get; set; }
    }
}
