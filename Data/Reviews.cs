using System;
using System.Collections.Generic;

namespace Site.Data
{
    public partial class Reviews
    {
        public Reviews()
        {
            ReviewResources = new HashSet<ReviewResources>();
        }

        public int Id { get; set; }
        public int WebsiteLanguageId { get; set; }
        public int LinkedToId { get; set; }
        public string UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Text { get; set; }
        public byte Rating { get; set; }
        public bool? Active { get; set; }
        public DateTime? CreatedAt { get; set; }
        public bool? ViewedByAdmin { get; set; }
        public int? ReviewTemplateId { get; set; }
        public bool? Anonymous { get; set; }

        public ReviewTemplates ReviewTemplate { get; set; }
        public WebsiteLanguages WebsiteLanguage { get; set; }
        public ICollection<ReviewResources> ReviewResources { get; set; }
    }
}
