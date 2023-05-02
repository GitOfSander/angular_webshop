using System;
using System.Collections.Generic;

namespace Site.Data
{
    public partial class DataTemplates
    {
        public DataTemplates()
        {
            DataItems = new HashSet<DataItems>();
            DataTemplateFields = new HashSet<DataTemplateFields>();
            DataTemplateSections = new HashSet<DataTemplateSections>();
            DataTemplateUploads = new HashSet<DataTemplateUploads>();
        }

        public int Id { get; set; }
        public int WebsiteId { get; set; }
        public string CallName { get; set; }
        public string Name { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public string Description { get; set; }
        public bool DetailPage { get; set; }
        public string PageAlternateGuid { get; set; }
        public string TitleHeading { get; set; }
        public string SubtitleHeading { get; set; }
        public string TextHeading { get; set; }
        public string HtmlEditorHeading { get; set; }
        public string PublishDateHeading { get; set; }
        public string FromDateHeading { get; set; }
        public string ToDateHeading { get; set; }
        public bool? Active { get; set; }
        public string MenuType { get; set; }
        public string Icon { get; set; }
        public int? CustomOrder { get; set; }

        public Websites Website { get; set; }
        public ICollection<DataItems> DataItems { get; set; }
        public ICollection<DataTemplateFields> DataTemplateFields { get; set; }
        public ICollection<DataTemplateSections> DataTemplateSections { get; set; }
        public ICollection<DataTemplateUploads> DataTemplateUploads { get; set; }
    }
}
