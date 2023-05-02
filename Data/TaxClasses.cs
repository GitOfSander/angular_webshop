using System;
using System.Collections.Generic;

namespace Site.Data
{
    public partial class TaxClasses
    {
        public TaxClasses()
        {
            TaxRates = new HashSet<TaxRates>();
        }

        public int Id { get; set; }
        public int WebsiteId { get; set; }
        public string Name { get; set; }
        public bool Default { get; set; }

        public Websites Website { get; set; }
        public ICollection<TaxRates> TaxRates { get; set; }
    }
}
