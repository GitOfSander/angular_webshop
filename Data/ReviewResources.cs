using System;
using System.Collections.Generic;

namespace Site.Data
{
    public partial class ReviewResources
    {
        public int Id { get; set; }
        public int ReviewId { get; set; }
        public int ReviewTemplateFieldId { get; set; }
        public string Text { get; set; }

        public Reviews Review { get; set; }
    }
}
