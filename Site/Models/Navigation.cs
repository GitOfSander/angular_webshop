using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Options;
using Site.Data;
using Site.Models.App;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using static Site.Models.Routing;

namespace Site.Models
{
    public class Navigation : Controller
    {
        SiteContext _context;
        private readonly IOptions<AppSettings> _config;

        public Navigation(SiteContext context, IOptions<AppSettings> config)
        {
            _context = context;
            _config = config;
        }

        public class NavigationBundle
        {
            public NavigationItems NavigationItem { get; set; }
            public Navigations Navigation { get; set; }
        }

        public class NavigationLinks
        {
            public string FullUrl { get; set; }
            public string Fragment { get; set; }
            public Site.Data.Pages Page { get; set; }
            public PageTemplates PageTemplate { get; set; }
            public NavigationItems NavigationItem { get; set; }
        }

        public IEnumerable<NavigationBundle> GetNavigationBundlesByWebsiteLanguageIdAndCallName(int websiteLanguageId, string callName)
        {
            return _context.NavigationItems.Where(x => x.WebsiteLanguageId == websiteLanguageId)
                                           .Join(_context.Navigations.Where(x => x.CallName == callName), NavigationItems => NavigationItems.NavigationId, Navigations => Navigations.Id, (NavigationItems, Navigations) => new { NavigationItems, Navigations })
                                           .Select(x => new NavigationBundle()
                                           {
                                               NavigationItem = x.NavigationItems,
                                               Navigation = x.Navigations
                                           })
                                           .OrderBy(x => x.NavigationItem.CustomOrder)
                                           .ToList();
        }

        public List<NavigationLinks> GetNavigationLinks(int websiteLanguageId, string callName)
        {
            Routing routing = new Routing(_context);
            List<Site.Data.Pages> _pages = routing.GetPageRoutesByWebsiteLanguageId(websiteLanguageId);
            List<DataRoutes> _dataRoutes = routing.GetDataRoutesByWebsiteLanguageIdAndDetailPage(websiteLanguageId, true);
            IEnumerable<NavigationBundle> _navigationBundles = new Navigation(_context, _config).GetNavigationBundlesByWebsiteLanguageIdAndCallName(websiteLanguageId, callName);

            List<NavigationLinks> navigationLinks = new List<NavigationLinks>();
            foreach (var i in _navigationBundles)
            {
                string url = "#";
                string fragment = "";
                Site.Data.Pages _page = new Site.Data.Pages();
                Page page = new Page(_context, _config);

                switch (i.NavigationItem.LinkedToType.ToLower())
                {
                    case "page":
                    case "category":
                        _page = _pages.Where(Page => Page.AlternateGuid == i.NavigationItem.LinkedToAlternateGuid).FirstOrDefault();
                        url = page.GetPageUrl(_pages, _page) + ((i.NavigationItem.LinkedToSectionId != 0) ? "#" + page.GetSectionOrFilter(_page, i.NavigationItem) : ""); ;
                        if (i.NavigationItem.LinkedToSectionId != 0) { fragment = page.GetSectionOrFilter(_page, i.NavigationItem); };
                        break;
                    case "product":
                        var productPageAndProductPageSetting = _context.ProductPages.Join(_context.ProductPageSettings.Where(ProductPageSetting => ProductPageSetting.WebsiteLanguageId == websiteLanguageId), ProductPage => ProductPage.ProductId, ProductPageSetting => ProductPageSetting.ProductId, (ProductPage, ProductPageSetting) => new { ProductPage, ProductPageSetting })
                                                                                    .FirstOrDefault(x => x.ProductPage.ProductId == Int32.Parse(i.NavigationItem.LinkedToAlternateGuid) && x.ProductPage.WebsiteLanguageId == websiteLanguageId);
                        _page = _pages.Where(Page => Page.AlternateGuid == productPageAndProductPageSetting.ProductPage.PageAlternateGuid).FirstOrDefault();
                        url = page.GetPageUrl(_pages, _page) + "/" + productPageAndProductPageSetting.ProductPageSetting.Url;
                        break;
                    case "dataitem":
                        DataRoutes _dataRoute = _dataRoutes.FirstOrDefault(DataRoutes => DataRoutes.DataItem.AlternateGuid == i.NavigationItem.LinkedToAlternateGuid);
                        _page = _pages.Where(Page => Page.AlternateGuid == _dataRoute.DataTemplate.PageAlternateGuid).FirstOrDefault();
                        url = page.GetPageUrl(_pages, _page) + "/" + _dataRoute.DataItem.PageUrl;
                        if (i.NavigationItem.LinkedToSectionId != 0) { fragment = new Data(_context).GetSectionOrFilter(_dataRoute, i.NavigationItem); };
                        break;
                    case "external":
                        url = i.NavigationItem.CustomUrl;
                        break;
                    default: // "nothing"
                        break;
                }

                // Something went wrong and the url couldn't be found. Cancel this operation and continue.
                if (url == null) { continue; };

                NavigationLinks navigationLink = new NavigationLinks()
                {
                    FullUrl = url,
                    Fragment = fragment,
                    Page = _page,
                    //new Dictionary<string, object>() {
                    //    { "Active", _page.Active },
                    //    { "AlternateGuid", _page.AlternateGuid },
                    //    { "CustomOrder", _page.CustomOrder },
                    //    { "Description", _page.Description },
                    //    { "Id", _page.Id },
                    //    { "Keywords", _page.Keywords },
                    //    { "Name", _page.Name },
                    //    { "Parent", _page.Parent },
                    //    { "Url", _page.Url }
                    //}
                    PageTemplate = (i.NavigationItem.LinkedToType.ToLower() == "product") ? null : (_page != null) ? _page.PageTemplate : null,
                    NavigationItem = i.NavigationItem
                };
                navigationLinks.Add(navigationLink);
            }

            return navigationLinks;
        }

        public List<Dictionary<string, object>> ConvertNavigationLinksToJson(List<NavigationLinks> navigationLinks, int parent)
        {
            Commerce commerce = new Commerce(_context, _config);
            commerce.SetPriceFormatVariables();

            Setting setting = new Setting(_context);
            int digitsAfterDecimal = Int32.Parse(setting.GetSettingValueByKey("digitsAfterDecimal", "website", _config.Value.WebsiteId));
            string currency = setting.GetSettingValueByKey("currency", "website", _config.Value.WebsiteId);

            List<Dictionary<string, object>> links = new List<Dictionary<string, object>>();
            foreach (NavigationLinks navigationLink in navigationLinks.Where(x => x.NavigationItem.Parent == parent))
            {
                Dictionary<string, object> dic = new Dictionary<string, object>()
                {
                    { "url", navigationLink.FullUrl },
                    { "name", navigationLink.NavigationItem.Name },
                    { "target", navigationLink.NavigationItem.Target },
                    { "fragment", navigationLink.Fragment },
                    { "childs", ConvertNavigationLinksToJson(navigationLinks, navigationLink.NavigationItem.Id) }
                };

                if (navigationLink.PageTemplate != null) { 
                    if (navigationLink.PageTemplate.Type.ToLower() == "ecommercecategory")
                    {
                        List<Products> _products = new Product(_context, _config).GetProductsByPageAlternateGuidAndWebsiteLanguageId(navigationLink.Page.AlternateGuid, navigationLink.Page.WebsiteLanguageId);
                        Products _product = _products.OrderBy(x => x.Price).FirstOrDefault();
                        Products _productPromo = _products.OrderBy(x => x.PromoPrice == 0).ThenBy(x => x.PromoPrice).FirstOrDefault();

                        if (_product != null)
                        {
                            bool promo = commerce.IsPromoEnabled(_product.PromoSchedule ?? false, _product.PromoFromDate, _product.PromoToDate, _product.PromoPrice, _product.Price);
                            decimal price = (promo ? _product.PromoPrice : _product.Price);
                            dic.Add("cheapestPrice", commerce.GetPriceFormat(decimal.Round(price, digitsAfterDecimal, MidpointRounding.AwayFromZero).ToString(CultureInfo.GetCultureInfo("nl-NL").NumberFormat), currency));
                        }
                    }
                }

                links.Add(dic);
            }

            return links;
        }


        public ObjectResult GetNavigationsAndConvertToJson(int websiteLanguageId, string[] callNames)
        {
            Dictionary<string, object> navigations = new Dictionary<string, object>();
            foreach (string callName in callNames)
            {
                navigations.Add(callName, ConvertNavigationLinksToJson(GetNavigationLinks(websiteLanguageId, callName), 0));
            }

            return Ok(navigations);
        }

        public ObjectResult GetNavigationLinksJson(int websiteLanguageId, string callName)
        {
            return Ok(ConvertNavigationLinksToJson(GetNavigationLinks(websiteLanguageId, callName), 0));
        }
    }
}
