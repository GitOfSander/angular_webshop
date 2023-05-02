using System;
using System.Collections.Generic;

namespace Site.Data
{
    public partial class ProductFiles
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int ProductUploadId { get; set; }
        public string OriginalPath { get; set; }
        public string CompressedPath { get; set; }
        public string Alt { get; set; }
        public int CustomOrder { get; set; }
        public bool Active { get; set; }

        public Products Product { get; set; }
    }
}
