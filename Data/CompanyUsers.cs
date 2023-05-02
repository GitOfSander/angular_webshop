using System;
using System.Collections.Generic;

namespace Site.Data
{
    public partial class CompanyUsers
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public string UserId { get; set; }

        public Companies Company { get; set; }
    }
}
