using System;
using System.Collections.Generic;

namespace Site.Data
{
    public partial class PageFiles
    {
        public int Id { get; set; }
        public int PageId { get; set; }
        public int PageTemplateUploadId { get; set; }
        public string OriginalPath { get; set; }
        public string CompressedPath { get; set; }
        public string Alt { get; set; }
        public int CustomOrder { get; set; }
        public bool? Active { get; set; }

        public Pages Page { get; set; }
    }
}
