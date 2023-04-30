using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Site.Data;
using System.Collections.Generic;
using System.Linq;
using static Site.Startup;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using System;
using Site.Models.App;
using static Site.Models.Page;

namespace Site.Models
{
    public class Routing : Controller
    {
        SiteContext _context;
        private readonly IOptions<AppSettings> _config;

        public Routing(SiteContext context = null, IOptions<AppSettings> config = null)
        {
            _context = context;
            _config = config;
        }

        public class PageRoutes
        {
            public Site.Data.Pages Page { get; set; }
            public PageTemplates PageTemplate { get; set; }
            public IEnumerable<PageTemplateSections> PageTemplateSections { get; set; }
            public WebsiteLanguages WebsiteLanguage { get; set; }
            public Languages Language { get; set; }
            public string FullUrl { get; set; }
        }

        public class DataRoutes
        {
            public DataTemplates DataTemplate { get; set; }
            public DataItems DataItem { get; set; }
            public IEnumerable<DataTemplateSections> DataTemplateSections { get; set; }
            public Site.Data.Pages Page { get; set; }
            public WebsiteLanguages WebsiteLanguage { get; set; }
            public Languages Language { get; set; }
        }

        public class ProductRoutes
        {
            public Site.Data.Pages Page { get; set; }
            public PageTemplates PageTemplate { get; set; }
            public ProductTemplates ProductTemplate { get; set; }
            public WebsiteLanguages WebsiteLanguage { get; set; }
            public Languages Language { get; set; }
        }

        public class PageLanguages
        {
            public WebsiteLanguages WebsiteLanguage { get; set; }
            public Languages Language { get; set; }
        }

        public List<DataRoutes> GetDataRoutesByWebsiteLanguageIdAndDetailPage(int websiteLanguageId, bool detailPage)
        {
            return _context.Pages.Join(_context.DataTemplates.Where(DataTemplates => DataTemplates.DetailPage == detailPage), Pages => Pages.AlternateGuid, DataTemplates => DataTemplates.PageAlternateGuid, (Pages, DataTemplates) => new { Pages, DataTemplates })
                                 .Join(_context.DataItems, x => x.DataTemplates.Id, DataItems => DataItems.DataTemplateId, (x, DataItems) => new { x.DataTemplates, x.Pages, DataItems })
                                 .Join(_context.WebsiteLanguages, x => x.Pages.WebsiteLanguageId, WebsiteLanguages => WebsiteLanguages.Id, (x, WebsiteLanguages) => new { x.DataItems, x.DataTemplates, x.Pages, WebsiteLanguages })
                                 .Join(_context.Languages, x => x.WebsiteLanguages.LanguageId, Languages => Languages.Id, (x, Languages) => new { x.DataItems, x.DataTemplates, x.Pages, x.WebsiteLanguages, Languages })
                                 .GroupJoin(_context.DataTemplateSections, x => x.DataTemplates.Id, DataTemplateSections => DataTemplateSections.DataTemplateId, (x, DataTemplateSections) => new { x.DataTemplates, x.DataItems, x.Pages, x.WebsiteLanguages, x.Languages, DataTemplateSections })
                                 .Where(x => x.DataTemplates.DetailPage == detailPage)
                                 .Where(x => x.WebsiteLanguages.Id == websiteLanguageId)
                                 .Where(x => x.WebsiteLanguages.Active == true)
                                 .Select(x => new DataRoutes()
                                 {
                                     DataItem = x.DataItems,
                                     DataTemplate = x.DataTemplates,
                                     DataTemplateSections = x.DataTemplateSections,
                                     Page = x.Pages,
                                     WebsiteLanguage = x.WebsiteLanguages,
                                     Language = x.Languages
                                 }).ToList();
        }

        public List<Site.Data.Pages> GetProductRoutesByWebsiteId()
        {

            //return _context.ProductTemplates.Select(ProductTemplate => new ProductTemplates() {
            //    Action =ProductTemplate.Action,
            //    Attributes = ProductTemplate.Attributes,
            //    Controller = ProductTemplate.Controller,
            //    CrossSells = ProductTemplate.CrossSells,
            //    Downloadable = ProductTemplate.Downloadable,
            //    ExternalProduct = ProductTemplate.ExternalProduct,
            //    GroupedProduct = ProductTemplate.GroupedProduct,
            //    Id = ProductTemplate.Id,
            //    Name = ProductTemplate.Name,
            //    Reviews = ProductTemplate.Reviews,
            //    SimpleProduct = ProductTemplate.SimpleProduct,
            //    Upsells = ProductTemplate.Upsells,
            //    VariableProduct = ProductTemplate.VariableProduct,
            //    Virtual = ProductTemplate.Virtual,
            //    WebsiteId = ProductTemplate.WebsiteId,
            //    Website = ProductTemplate.Websites.Select(Website => new Websites() {
            //        Active = Website.Active,
            //        CompanyId = Website.CompanyId,
            //        Domain = Website.Domain,
            //        Extension = Website.Extension,
            //        Folder = Website.Folder,
            //        Id = Website.Id,
            //        WebsiteLanguages = Website.WebsiteLanguages.Select(WebsiteLanguage => new WebsiteLanguages() {
            //            Active = WebsiteLanguage.Active,
            //            DefaultLanguage = WebsiteLanguage.DefaultLanguage,
            //            Id = WebsiteLanguage.Id,
            //            LanguageId = WebsiteLanguage.LanguageId,
            //            WebsiteId = WebsiteLanguage.WebsiteId,
            //            Language = WebsiteLanguage.Language,
            //        }).ToList()
            //}).Where(PageTemplate => PageTemplate.Type == "eCommerceCategory").ToList();
            return _context.Pages.Select(Page => new Site.Data.Pages()
            {
                Active = Page.Active,
                AlternateGuid = Page.AlternateGuid,
                CustomOrder = Page.CustomOrder,
                Description = Page.Description,
                Id = Page.Id,
                Keywords = Page.Keywords,
                Name = Page.Name,
                Parent = Page.Parent,
                Title = Page.Title,
                Url = Page.Url,
                WebsiteLanguageId = Page.WebsiteLanguageId,
                PageTemplateId = Page.PageTemplateId,
                PageTemplate = Page.PageTemplate,
                WebsiteLanguage = _context.WebsiteLanguages.Select(WebsiteLanguage => new WebsiteLanguages() {
                    Active = WebsiteLanguage.Active,
                    DefaultLanguage = WebsiteLanguage.DefaultLanguage,
                    Id = WebsiteLanguage.Id,
                    LanguageId = WebsiteLanguage.LanguageId,
                    WebsiteId = WebsiteLanguage.WebsiteId,
                    Language = WebsiteLanguage.Language,
                    Website = _context.Websites.Select(Website => new Websites() {
                        Active = Website.Active,
                        CompanyId = Website.CompanyId,
                        Domain = Website.Domain,
                        Extension = Website.Extension,
                        Folder = Website.Folder,
                        Id = Website.Id,
                        ProductTemplates = Website.ProductTemplates
                    }).FirstOrDefault(Website => Website.Id == WebsiteLanguage.WebsiteId)
                }).FirstOrDefault(WebsiteLanguage => WebsiteLanguage.WebsiteId == _config.Value.WebsiteId && WebsiteLanguage.Id == Page.WebsiteLanguageId && WebsiteLanguage.Active == true)
            }).Where(Page => Page.PageTemplate.Type == "eCommerceCategory").ToList();

            //return _context.Pages.Join(_context.PageTemplates.Where(PageTemplate => PageTemplate.Type == "eCommerceCategory"), Page => Page.PageTemplateId, PageTemplate => PageTemplate.Id, (Page, PageTemplate) => new { Page, PageTemplate })
            //                     .Join(_context.WebsiteLanguages, x => x.Page.WebsiteLanguageId, WebsiteLanguage => WebsiteLanguage.Id, (x, WebsiteLanguage) => new { x.PageTemplate, x.Page, WebsiteLanguage })
            //                     .Join(_context.Languages, x => x.WebsiteLanguage.LanguageId, Language => Language.Id, (x, Language) => new { x.PageTemplate, x.Page, x.WebsiteLanguage, Language })
            //                     .Join(_context.ProductTemplates, x => x.WebsiteLanguage.WebsiteId, ProductTemplate => ProductTemplate.WebsiteId, (x, ProductTemplate) => new { x.PageTemplate, x.Page, x.WebsiteLanguage, x.Language, ProductTemplate })
            //                     .Where(x => x.WebsiteLanguage.WebsiteId == _config.Value.WebsiteId)
            //                     .Where(x => x.WebsiteLanguage.Active == true)
            //                     .Select(x => new ProductRoutes()
            //                     {
            //                         Page = x.Page,
            //                         PageTemplate = x.PageTemplate,
            //                         ProductTemplate = x.ProductTemplate,
            //                         WebsiteLanguage = x.WebsiteLanguage,
            //                         Language = x.Language
            //                     }).ToList();
        }

        public List<DataRoutes> GetDataRoutesByWebsiteId()
        {
            return _context.Pages.Join(_context.DataTemplates.Where(DataTemplates => DataTemplates.DetailPage == true), Pages => Pages.AlternateGuid, DataTemplates => DataTemplates.PageAlternateGuid, (Pages, DataTemplates) => new { Pages, DataTemplates })
                                 .Join(_context.WebsiteLanguages, x => x.Pages.WebsiteLanguageId, WebsiteLanguages => WebsiteLanguages.Id, (x, WebsiteLanguages) => new { x.DataTemplates, x.Pages, WebsiteLanguages })
                                 .Join(_context.Languages, x => x.WebsiteLanguages.LanguageId, Languages => Languages.Id, (x, Languages) => new { x.DataTemplates, x.Pages, x.WebsiteLanguages, Languages })
                                 .Where(x => x.DataTemplates.DetailPage == true)
                                 .Where(x => x.WebsiteLanguages.WebsiteId == _config.Value.WebsiteId)
                                 .Where(x => x.WebsiteLanguages.Active == true)
                                 .Select(x => new DataRoutes()
                                 {
                                     DataTemplate = x.DataTemplates,
                                     Page = x.Pages,
                                     WebsiteLanguage = x.WebsiteLanguages,
                                     Language = x.Languages
                                 }).ToList();
        }

        public List<Site.Data.Pages> GetPageRoutesByWebsiteLanguageId(int websiteLanguageId)
        {
            return _context.Pages.Select(Page => new Site.Data.Pages
            {
                Active = Page.Active,
                AlternateGuid = Page.AlternateGuid,
                CustomOrder = Page.CustomOrder,
                Description = Page.Description,
                Id = Page.Id,
                Keywords = Page.Keywords,
                Name = Page.Name,
                Parent = Page.Parent,
                Title = Page.Title,
                Url = Page.Url,
                WebsiteLanguageId = Page.WebsiteLanguageId,
                PageTemplateId = Page.PageTemplateId,
                PageTemplate = _context.PageTemplates.Select(PageTemplate => new PageTemplates() {
                    Id = PageTemplate.Id,
                    Action = PageTemplate.Action,
                    Controller = PageTemplate.Controller,
                    Name = PageTemplate.Name,
                    Type = PageTemplate.Type,
                    Website = PageTemplate.Website,
                    WebsiteId = PageTemplate.WebsiteId,
                    PageTemplateSections = PageTemplate.PageTemplateSections.Where(PageTemplateSection => PageTemplateSection.PageTemplateId == PageTemplate.Id).ToList()
                }).FirstOrDefault(PageTemplate => PageTemplate.Id == Page.PageTemplateId),
                WebsiteLanguage = _context.WebsiteLanguages.Select(WebsiteLanguage => new WebsiteLanguages() {
                    Active = WebsiteLanguage.Active,
                    DefaultLanguage = WebsiteLanguage.DefaultLanguage,
                    Id = WebsiteLanguage.Id,
                    LanguageId = WebsiteLanguage.LanguageId,
                    WebsiteId = WebsiteLanguage.WebsiteId,
                    Language = WebsiteLanguage.Language
                }).FirstOrDefault(WebsiteLanguage => WebsiteLanguage.Id == websiteLanguageId && WebsiteLanguage.Active == true)
            }).ToList();
        }

        public List<Site.Data.Pages> GetPageRoutes(int websiteLanguageId)
        {
            return _context.Pages.Select(Page => new Site.Data.Pages
            {
                Active = Page.Active,
                AlternateGuid = Page.AlternateGuid,
                CustomOrder = Page.CustomOrder,
                Description = Page.Description,
                Id = Page.Id,
                Keywords = Page.Keywords,
                Name = Page.Name,
                Parent = Page.Parent,
                Title = Page.Title,
                Url = Page.Url,
                WebsiteLanguageId = Page.WebsiteLanguageId,
                PageTemplateId = Page.PageTemplateId,
                PageTemplate = _context.PageTemplates.Select(PageTemplate => new PageTemplates()
                {
                    Id = PageTemplate.Id,
                    Action = PageTemplate.Action,
                    Controller = PageTemplate.Controller,
                    Name = PageTemplate.Name,
                    Type = PageTemplate.Type,
                    Website = PageTemplate.Website,
                    WebsiteId = PageTemplate.WebsiteId
                }).FirstOrDefault(PageTemplate => PageTemplate.Id == Page.PageTemplateId),
                WebsiteLanguage = _context.WebsiteLanguages.Select(WebsiteLanguage => new WebsiteLanguages()
                {
                    Active = WebsiteLanguage.Active,
                    DefaultLanguage = WebsiteLanguage.DefaultLanguage,
                    Id = WebsiteLanguage.Id,
                    LanguageId = WebsiteLanguage.LanguageId,
                    WebsiteId = WebsiteLanguage.WebsiteId,
                    Language = _context.Languages.FirstOrDefault(Language => Language.Id == WebsiteLanguage.LanguageId)
                }).FirstOrDefault(WebsiteLanguage => WebsiteLanguage.Id == websiteLanguageId && WebsiteLanguage.Active == true)
            }).ToList();
        }

        public List<Site.Data.Pages> GetPageRoutesByWebsiteId()
        {
            return _context.Pages.Select(Page => new Site.Data.Pages
            {
                Active = Page.Active,
                AlternateGuid = Page.AlternateGuid,
                CustomOrder = Page.CustomOrder,
                Description = Page.Description,
                Id = Page.Id,
                Keywords = Page.Keywords,
                Name = Page.Name,
                Parent = Page.Parent,
                Title = Page.Title,
                Url = Page.Url,
                WebsiteLanguageId = Page.WebsiteLanguageId,
                PageTemplateId = Page.PageTemplateId,
                PageTemplate = _context.PageTemplates.Select(PageTemplate => new PageTemplates()
                {
                    Id = PageTemplate.Id,
                    Action = PageTemplate.Action,
                    Controller = PageTemplate.Controller,
                    Name = PageTemplate.Name,
                    Type = PageTemplate.Type,
                    Website = PageTemplate.Website,
                    WebsiteId = PageTemplate.WebsiteId,
                }).FirstOrDefault(PageTemplate => PageTemplate.Id == Page.PageTemplateId),
                WebsiteLanguage = _context.WebsiteLanguages.Select(WebsiteLanguage => new WebsiteLanguages()
                {
                    Active = WebsiteLanguage.Active,
                    DefaultLanguage = WebsiteLanguage.DefaultLanguage,
                    Id = WebsiteLanguage.Id,
                    LanguageId = WebsiteLanguage.LanguageId,
                    WebsiteId = WebsiteLanguage.WebsiteId,
                    Language = WebsiteLanguage.Language
                }).FirstOrDefault(WebsiteLanguage => WebsiteLanguage.WebsiteId == _config.Value.WebsiteId && WebsiteLanguage.Active == true)
            }).ToList();
        }

        public string GetPageRouteUrlByAlternateGuidAndWebsiteLanguageId(string alternateGuid, int websiteLanguageId)
        {
            List<Site.Data.Pages> _pages = GetPageRoutesByWebsiteId();

            Site.Data.Pages _page = _pages.FirstOrDefault(Page => Page.AlternateGuid == alternateGuid && Page.WebsiteLanguage.Id == websiteLanguageId);
            if (_page == null)
            {
                Site.Data.Pages _rootPage = new Page(_context, _config).GetRootPage(websiteLanguageId);
                _page = _pages.FirstOrDefault(Page => Page.AlternateGuid == _rootPage.AlternateGuid && Page.WebsiteLanguage.Id == websiteLanguageId);
            }
            return FindParentUrl(_page.Parent, _page.Url, _pages, _page.WebsiteLanguage.DefaultLanguage, _page.WebsiteLanguage.Language.Code);
        }

        public string FindParentUrl(int parent, string url, List<Site.Data.Pages> pages, bool defaultLanguage, string code)
        {
            if (parent != 0)
            {
                Site.Data.Pages _page = pages.Find(Page => Page.Id == parent);
                url = _page.Url + "/" + url;

                if (_page.Parent != 0)
                {
                    return FindParentUrl(_page.Parent, url, pages, defaultLanguage, code);
                }
            }

            if (!defaultLanguage)
            {
                return "/" + code.ToLower() + ((url != "") ? "/" + url : "");
            }

            return "/" + url;
        }

        public List<Dictionary<string, object>> ConvertRoutesToJson(List<Site.Data.Pages> pages, List<DataRoutes> dataRoutes, List<Site.Data.Pages> productPages)
        {
            List<Dictionary<string, object>> routes = new List<Dictionary<string, object>>();
            foreach (Site.Data.Pages page in pages)
            {
                routes.Add(ConvertPageRouteToJson(pages, page));
            }

            foreach (DataRoutes dataRoute in dataRoutes)
            {
                routes.Add(ConvertDataRouteToJson(pages, dataRoute));
            }

            foreach (Site.Data.Pages productPage in productPages)
            {
                routes.Add(ConvertProductRouteToJson(pages, productPage));
            }

            return routes;
        }

        public Dictionary<string, object> ConvertPageRouteToJson(List<Site.Data.Pages> pages, Site.Data.Pages page)
        {
            string url = FindParentUrl(page.Parent, page.Url, pages, page.WebsiteLanguage.DefaultLanguage, page.WebsiteLanguage.Language.Code);

            return new Dictionary<string, object>() {
                { "action", page.PageTemplate.Action },
                { "controller", page.PageTemplate.Controller },
                { "url",  url.Substring(1) },
                { "pageId", page.Id },
                { "websiteId", page.WebsiteLanguage.WebsiteId },
                { "websiteLanguageId", page.WebsiteLanguage.Id },
                { "alternateGuid", page.AlternateGuid }
            };
        }

        public Dictionary<string, object> ConvertProductRouteToJson(List<Site.Data.Pages> pages, Site.Data.Pages productPage)
        {
            string url = FindParentUrl(productPage.Parent, productPage.Url, pages, productPage.WebsiteLanguage.DefaultLanguage, productPage.WebsiteLanguage.Language.Code);

            return new Dictionary<string, object>() {
                { "action", productPage.WebsiteLanguage.Website.ProductTemplates.FirstOrDefault().Action },
                { "controller", productPage.WebsiteLanguage.Website.ProductTemplates.FirstOrDefault().Controller },
                { "url",  url.Substring(1) + "/:itemUrl" },
                { "pageId", productPage.Id },
                { "websiteId", productPage.WebsiteLanguage.WebsiteId },
                { "websiteLanguageId", productPage.WebsiteLanguage.Id },
                { "alternateGuid", productPage.AlternateGuid }
            };
        }

        public Dictionary<string, object> ConvertDataRouteToJson(List<Site.Data.Pages> pages, DataRoutes dataRoute)
        {
            string url = FindParentUrl(dataRoute.Page.Parent, dataRoute.Page.Url, pages, dataRoute.WebsiteLanguage.DefaultLanguage, dataRoute.Language.Code);

            return new Dictionary<string, object>() {
                { "action", dataRoute.DataTemplate.Action },
                { "controller", dataRoute.DataTemplate.Controller },
                { "url",  url.Substring(1) + "/:itemUrl"  },
                { "pageId", dataRoute.Page.Id },
                { "websiteId", dataRoute.WebsiteLanguage.WebsiteId },
                { "websiteLanguageId", dataRoute.WebsiteLanguage.Id },
                { "alternateGuid", dataRoute.DataTemplate.PageAlternateGuid }
            };
        }

        public ObjectResult GetRoutes()
        {
            return Ok(ConvertRoutesToJson(GetPageRoutesByWebsiteId(), GetDataRoutesByWebsiteId(), GetProductRoutesByWebsiteId()));
        }

        public ObjectResult GetRoutesByAlternateGuidAndWebsiteLanguageId(string alternateGuid, int websiteLanguageId)
        {
            return Ok(GetPageRouteUrlByAlternateGuidAndWebsiteLanguageId(alternateGuid, websiteLanguageId));
        }
    }
}