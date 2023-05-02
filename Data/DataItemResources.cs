using System;
using System.Collections.Generic;

namespace Site.Data
{
    public partial class DataItemResources
    {
        public int Id { get; set; }
        public int DataItemId { get; set; }
        public int DataTemplateFieldId { get; set; }
        public string Text { get; set; }

        public DataItems DataItem { get; set; }
    }
}
