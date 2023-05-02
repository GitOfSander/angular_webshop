using System;
using System.Collections.Generic;

namespace Site.Data
{
    public partial class ReviewTemplates
    {
        public ReviewTemplates()
        {
            ReviewTemplateFields = new HashSet<ReviewTemplateFields>();
            Reviews = new HashSet<Reviews>();
        }

        public int Id { get; set; }
        public int WebsiteId { get; set; }
        public string CallName { get; set; }
        public string Name { get; set; }
        public bool Active { get; set; }
        public bool? CheckBeforeOnline { get; set; }
        public string LinkedToType { get; set; }

        public Websites Website { get; set; }
        public ICollection<ReviewTemplateFields> ReviewTemplateFields { get; set; }
        public ICollection<Reviews> Reviews { get; set; }
    }
}
