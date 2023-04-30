using System;
using System.Collections.Generic;

namespace Site.Data
{
    public partial class DataTemplateSections
    {
        public int Id { get; set; }
        public int DataTemplateId { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public string Section { get; set; }
        public int LinkedToDataTemplateId { get; set; }

        public DataTemplates DataTemplate { get; set; }
    }
}
