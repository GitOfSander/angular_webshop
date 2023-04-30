using System;
using System.Collections.Generic;

namespace Site.Data
{
    public partial class Navigations
    {
        public Navigations()
        {
            NavigationItems = new HashSet<NavigationItems>();
        }

        public int Id { get; set; }
        public int WebsiteId { get; set; }
        public string CallName { get; set; }
        public string Name { get; set; }
        public byte MaxDepth { get; set; }

        public Websites Website { get; set; }
        public ICollection<NavigationItems> NavigationItems { get; set; }
    }
}
