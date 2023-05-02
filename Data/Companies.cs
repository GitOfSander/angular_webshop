using System;
using System.Collections.Generic;

namespace Site.Data
{
    public partial class Companies
    {
        public Companies()
        {
            CompanyUsers = new HashSet<CompanyUsers>();
            Websites = new HashSet<Websites>();
        }

        public int Id { get; set; }
        public string Company { get; set; }
        public string Vat { get; set; }
        public string Coc { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }

        public ICollection<CompanyUsers> CompanyUsers { get; set; }
        public ICollection<Websites> Websites { get; set; }
    }
}
