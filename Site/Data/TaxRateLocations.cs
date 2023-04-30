using System;
using System.Collections.Generic;

namespace Site.Data
{
    public partial class TaxRateLocations
    {
        public int Id { get; set; }
        public int TaxRateId { get; set; }
        public string Code { get; set; }
        public string Type { get; set; }

        public TaxRates TaxRate { get; set; }
    }
}
