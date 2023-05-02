using System;
using System.Collections.Generic;

namespace Site.Data
{
    public partial class WebsiteFiles
    {
        public int Id { get; set; }
        public int WebsiteUploadId { get; set; }
        public string OriginalPath { get; set; }
        public string CompressedPath { get; set; }
        public string Alt { get; set; }
        public int CustomOrder { get; set; }
        public bool Active { get; set; }
        public int? WebsiteLanguageId { get; set; }

        public WebsiteLanguages WebsiteLanguage { get; set; }
    }
}
