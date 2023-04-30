using System;
using System.Collections.Generic;

namespace Site.Data
{
    public partial class Websites
    {
        public Websites()
        {
            ApiKeys = new HashSet<ApiKeys>();
            DataTemplates = new HashSet<DataTemplates>();
            Navigations = new HashSet<Navigations>();
            OauthTokens = new HashSet<OauthTokens>();
            Orders = new HashSet<Orders>();
            PageTemplates = new HashSet<PageTemplates>();
            ProductTemplates = new HashSet<ProductTemplates>();
            ReviewTemplates = new HashSet<ReviewTemplates>();
            ShippingClasses = new HashSet<ShippingClasses>();
            ShippingZones = new HashSet<ShippingZones>();
            TaxClasses = new HashSet<TaxClasses>();
            WebsiteFields = new HashSet<WebsiteFields>();
            WebsiteLanguages = new HashSet<WebsiteLanguages>();
            WebsiteUploads = new HashSet<WebsiteUploads>();
        }

        public int Id { get; set; }
        public int CompanyId { get; set; }
        public string Domain { get; set; }
        public string Extension { get; set; }
        public string Folder { get; set; }
        public string Subdomain { get; set; }
        public string TypeClient { get; set; }
        public string RootPageAlternateGuid { get; set; }
        public string Subtitle { get; set; }
        public bool? Active { get; set; }

        public Companies Company { get; set; }
        public ICollection<ApiKeys> ApiKeys { get; set; }
        public ICollection<DataTemplates> DataTemplates { get; set; }
        public ICollection<Navigations> Navigations { get; set; }
        public ICollection<OauthTokens> OauthTokens { get; set; }
        public ICollection<Orders> Orders { get; set; }
        public ICollection<PageTemplates> PageTemplates { get; set; }
        public ICollection<ProductTemplates> ProductTemplates { get; set; }
        public ICollection<ReviewTemplates> ReviewTemplates { get; set; }
        public ICollection<ShippingClasses> ShippingClasses { get; set; }
        public ICollection<ShippingZones> ShippingZones { get; set; }
        public ICollection<TaxClasses> TaxClasses { get; set; }
        public ICollection<WebsiteFields> WebsiteFields { get; set; }
        public ICollection<WebsiteLanguages> WebsiteLanguages { get; set; }
        public ICollection<WebsiteUploads> WebsiteUploads { get; set; }
    }
}
