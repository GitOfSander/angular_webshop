using System;
using System.Collections.Generic;

namespace Site.Data
{
    public partial class Cultures
    {
        public int Id { get; set; }
        public string LanguageCode { get; set; }
        public string CultureName { get; set; }
        public string DisplayName { get; set; }
        public string CultureCode { get; set; }
    }
}
