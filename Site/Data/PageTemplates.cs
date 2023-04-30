using System;
using System.Collections.Generic;

namespace Site.Data
{
    public partial class PageTemplates
    {
        public PageTemplates()
        {
            PageTemplateFields = new HashSet<PageTemplateFields>();
            PageTemplateSections = new HashSet<PageTemplateSections>();
            PageTemplateUploads = new HashSet<PageTemplateUploads>();
            Pages = new HashSet<Pages>();
        }

        public int Id { get; set; }
        public int WebsiteId { get; set; }
        public string Name { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public string Type { get; set; }

        public Websites Website { get; set; }
        public ICollection<PageTemplateFields> PageTemplateFields { get; set; }
        public ICollection<PageTemplateSections> PageTemplateSections { get; set; }
        public ICollection<PageTemplateUploads> PageTemplateUploads { get; set; }
        public ICollection<Pages> Pages { get; set; }
    }
}
