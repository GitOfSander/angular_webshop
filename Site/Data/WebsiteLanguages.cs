using System;
using System.Collections.Generic;

namespace Site.Data
{
    public partial class WebsiteLanguages
    {
        public WebsiteLanguages()
        {
            Pages = new HashSet<Pages>();
            Reviews = new HashSet<Reviews>();
            WebsiteFiles = new HashSet<WebsiteFiles>();
            WebsiteResources = new HashSet<WebsiteResources>();
        }

        public int Id { get; set; }
        public int WebsiteId { get; set; }
        public int LanguageId { get; set; }
        public bool DefaultLanguage { get; set; }
        public bool Active { get; set; }

        public Languages Language { get; set; }
        public Websites Website { get; set; }
        public ICollection<Pages> Pages { get; set; }
        public ICollection<Reviews> Reviews { get; set; }
        public ICollection<WebsiteFiles> WebsiteFiles { get; set; }
        public ICollection<WebsiteResources> WebsiteResources { get; set; }
    }
}
