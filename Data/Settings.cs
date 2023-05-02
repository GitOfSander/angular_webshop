using System;
using System.Collections.Generic;

namespace Site.Data
{
    public partial class Settings
    {
        public int Id { get; set; }
        public int LinkedToId { get; set; }
        public string LinkedToType { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
    }
}
