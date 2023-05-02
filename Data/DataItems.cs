using System;
using System.Collections.Generic;

namespace Site.Data
{
    public partial class DataItems
    {
        public DataItems()
        {
            DataItemFiles = new HashSet<DataItemFiles>();
            DataItemResources = new HashSet<DataItemResources>();
        }

        public int Id { get; set; }
        public int WebsiteLanguageId { get; set; }
        public int DataTemplateId { get; set; }
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string Text { get; set; }
        public string HtmlEditor { get; set; }
        public DateTime PublishDate { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public bool Active { get; set; }
        public string PageUrl { get; set; }
        public string PageTitle { get; set; }
        public string PageKeywords { get; set; }
        public string PageDescription { get; set; }
        public int CustomOrder { get; set; }
        public string AlternateGuid { get; set; }

        public DataTemplates DataTemplate { get; set; }
        public ICollection<DataItemFiles> DataItemFiles { get; set; }
        public ICollection<DataItemResources> DataItemResources { get; set; }
    }
}
