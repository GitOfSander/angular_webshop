using System;
using System.Collections.Generic;

namespace Site.Data
{
    public partial class TaxRates
    {
        public TaxRates()
        {
            TaxRateLocations = new HashSet<TaxRateLocations>();
        }

        public int Id { get; set; }
        public int TaxClassId { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public decimal Rate { get; set; }
        public string Name { get; set; }
        public bool Compound { get; set; }
        public bool Shipping { get; set; }
        public int PriorityOrder { get; set; }
        public bool? Default { get; set; }

        public TaxClasses TaxClass { get; set; }
        public ICollection<TaxRateLocations> TaxRateLocations { get; set; }
    }
}
