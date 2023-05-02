using System;
using System.Collections.Generic;

namespace Site.Data
{
    public partial class WebsiteResources
    {
        public int Id { get; set; }
        public int WebsiteLanguageId { get; set; }
        public int WebsiteFieldId { get; set; }
        public string Text { get; set; }

        public WebsiteLanguages WebsiteLanguage { get; set; }
    }
}
