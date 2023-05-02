using System;
using System.Collections.Generic;

namespace Site.Data
{
    public partial class DataItemFiles
    {
        public int Id { get; set; }
        public int DataItemId { get; set; }
        public int DataTemplateUploadId { get; set; }
        public string OriginalPath { get; set; }
        public string CompressedPath { get; set; }
        public string Alt { get; set; }
        public int CustomOrder { get; set; }
        public bool? Active { get; set; }

        public DataItems DataItem { get; set; }
    }
}
