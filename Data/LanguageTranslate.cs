using System;
using System.Collections.Generic;

namespace Site.Data
{
    public partial class LanguageTranslate
    {
        public int Id { get; set; }
        public int LanguageId { get; set; }
        public string Code { get; set; }
        public string Translate { get; set; }
    }
}
