using Site.Data;
using Site.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using static Site.Models.Website;
using static Site.Models.Routing;
using Site.Models.App;
using Microsoft.Extensions.Options;
using static Site.Models.Navigation;
using System.Text.RegularExpressions;
using static Site.Models.Data;
using static Site.Models.Product;

namespace Site.Models
{
    public class Page : Controller
    {
        SiteContext _context;
        private readonly IOptions<AppSettings> _config;

        public Page(SiteContext context, IOptions<AppSettings> config)
        {
            _context = context;
            _config = config;
        }

        public class PageBundle
        {
            public PageFiles PageFile { get; set; }
            public IEnumerable<PageFiles> PageFiles { get; set; }
            public PageResources PageResource { get; set; }
            public IEnumerable<PageResources> PageResources { get; set; }
            public Site.Data.Pages Page { get; set; }
            public PageTemplateFields PageTemplateField { get; set; }
            public IEnumerable<PageTemplateFields> PageTemplateFields { get; set; }
            public PageTemplates PageTemplate { get; set; }
            public PageTemplateUploads PageTemplateUpload { get; set; }
            public IEnumerable<PageTemplateUploads> PageTemplateUploads { get; set; }
        }

        private List<Site.Data.Pages> _pages;
        private WebsiteBundle _websiteBundle;
        public bool AddProductJson = false;
        public bool AddBreadcrumbsJson = false;

        public Site.Data.Pages GetPageBundle(int pageId)
        {
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
                PageFiles = Page.PageFiles.Where(PageFile => PageFile.PageId == Page.Id && PageFile.Active == true).OrderBy(PageFile => PageFile.CustomOrder).ToList(),
                PageResources = Page.PageResources.Where(PageResource => PageResource.PageId == Page.Id).ToList(),
                PageTemplate = _context.PageTemplates.Select(PageTemplate => new PageTemplates() {
                    Id = PageTemplate.Id,
                    Action = PageTemplate.Action,
                    Controller = PageTemplate.Controller,
                    Name = PageTemplate.Name,
                    Type = PageTemplate.Type,
                    Website = PageTemplate.Website,
                    WebsiteId = PageTemplate.WebsiteId,
                    PageTemplateFields = PageTemplate.PageTemplateFields.Where(PageTemplateField => PageTemplateField.PageTemplateId == PageTemplate.Id).ToList(),
                    PageTemplateUploads = PageTemplate.PageTemplateUploads.Where(PageTemplateUpload => PageTemplateUpload.PageTemplateId == PageTemplate.Id).ToList()
                }).FirstOrDefault(PageTemplate => PageTemplate.Id == Page.PageTemplateId)
            }).FirstOrDefault(Page => Page.Id == pageId && Page.Active == true);
        }

        public List<Site.Data.Pages> GetPageBundlesByType(int websiteLanguageId, string type, bool active)
        {
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
                PageFiles = Page.PageFiles.Where(PageFile => PageFile.PageId == Page.Id && PageFile.Active == true).OrderBy(PageFile => PageFile.CustomOrder).ToList(),
                PageResources = Page.PageResources.Where(PageResource => PageResource.PageId == Page.Id).ToList(),
                PageTemplate = _context.PageTemplates.Select(PageTemplate => new PageTemplates()
                {
                    Id = PageTemplate.Id,
                    Action = PageTemplate.Action,
                    Controller = PageTemplate.Controller,
                    Name = PageTemplate.Name,
                    Type = PageTemplate.Type,
                    Website = PageTemplate.Website,
                    WebsiteId = PageTemplate.WebsiteId,
                    PageTemplateFields = PageTemplate.PageTemplateFields.Where(PageTemplateField => PageTemplateField.PageTemplateId == PageTemplate.Id).ToList(),
                    PageTemplateUploads = PageTemplate.PageTemplateUploads.Where(PageTemplateUpload => PageTemplateUpload.PageTemplateId == PageTemplate.Id).ToList()
                }).FirstOrDefault(PageTemplate => PageTemplate.Id == Page.PageTemplateId)
            }).Where(Page => Page.PageTemplate.Type == type && Page.Active == active).ToList();
        }

        public Site.Data.Pages GetRootPage(int websiteLanguageId)
        {
            return _context.WebsiteLanguages.Join(_context.Websites, WebsiteLanguages => WebsiteLanguages.WebsiteId, Websites => Websites.Id, (WebsiteLanguages, Websites) => new { WebsiteLanguages, Websites })
                                            .Join(_context.Pages, x => x.WebsiteLanguages.Id, Pages => Pages.WebsiteLanguageId, (x, Pages) => new { x.WebsiteLanguages, x.Websites, Pages })
                                            .Where(x => x.Websites.RootPageAlternateGuid == x.Pages.AlternateGuid)
                                            .Select(x => x.Pages)
                                            .FirstOrDefault(x => x.WebsiteLanguageId == websiteLanguageId);
        }

        public Site.Data.Pages GetPage(int websiteLanguageId, string alternateGuid)
        {
            return _context.Pages.Where(Pages => Pages.AlternateGuid == alternateGuid).FirstOrDefault(Pages => Pages.WebsiteLanguageId == websiteLanguageId);
        }

        public bool SetPageBundleAndWebsiteBundle(int websiteLanguageId)
        {
            _pages = new Routing(_context, _config).GetPageRoutes(websiteLanguageId);

            _websiteBundle = _context.WebsiteLanguages.Join(_context.Languages, WebsiteLanguages => WebsiteLanguages.LanguageId, Languages => Languages.Id, (WebsiteLanguages, Languages) => new { WebsiteLanguages, Languages })
                                                      .Where(x => x.WebsiteLanguages.Id == websiteLanguageId)
                                                      .Select(x => new WebsiteBundle()
                                                      {
                                                          Language = x.Languages,
                                                          WebsiteLanguage = x.WebsiteLanguages
                                                      }).FirstOrDefault();

            if (_pages != null && _websiteBundle != null) { return true; }

            return false;
        }

        public string GetPageUrlByAlternateGuid(string alternateGuid)
        {
            Site.Data.Pages _page = _pages.FirstOrDefault(Page => Page.AlternateGuid == alternateGuid);

            string url = "";
            if (_page != null)
            {
                url = new Routing(_context).FindParentUrl(_page.Parent, _page.Url, _pages, _websiteBundle.WebsiteLanguage.DefaultLanguage, _websiteBundle.Language.Code);
            }

            return url;
        }

        public List<PageRoutes> GetPageRoutes()
        {
            return _context.Pages.Join(_context.PageTemplates, Pages => Pages.PageTemplateId, PageTemplates => PageTemplates.Id, (Pages, PageTemplates) => new { Pages, PageTemplates })
                                 .Join(_context.WebsiteLanguages, x => x.Pages.WebsiteLanguageId, WebsiteLanguages => WebsiteLanguages.Id, (x, WebsiteLanguages) => new { x.PageTemplates, x.Pages, WebsiteLanguages })
                                 .Join(_context.Languages, x => x.WebsiteLanguages.LanguageId, Languages => Languages.Id, (x, Languages) => new { x.PageTemplates, x.Pages, x.WebsiteLanguages, Languages })
                                 .Where(x => x.WebsiteLanguages.WebsiteId == _config.Value.WebsiteId)
                                 .Where(x => x.WebsiteLanguages.Active == true)
                                 .Select(x => new PageRoutes()
                                 {
                                     Page = x.Pages,
                                     PageTemplate = x.PageTemplates,
                                     WebsiteLanguage = x.WebsiteLanguages,
                                     Language = x.Languages
                                 }).ToList();
        }

        public string GetPageUrl(List<Site.Data.Pages> pages, Site.Data.Pages page)
        {
            if (page != null)
            {
                return new Routing(_context, _config).FindParentUrl(page.Parent, page.Url, pages, page.WebsiteLanguage.DefaultLanguage, page.WebsiteLanguage.Language.Code);
            }

            return null;
        }

        public string GetSectionOrFilter(Site.Data.Pages page, NavigationItems navigationItem)
        {
            PageTemplateSections pageTemplateSection = page.PageTemplate.PageTemplateSections.FirstOrDefault(PageTemplateSections => PageTemplateSections.Id == navigationItem.LinkedToSectionId);
            switch (pageTemplateSection.Type.ToLower())
            {
                case "section":
                    return pageTemplateSection.Section;
                default: // "datafilter"
                    DataItems _dataItem = _context.DataItems.FirstOrDefault(DataItems => DataItems.AlternateGuid == navigationItem.FilterAlternateGuid);
                    return Regex.Replace(_dataItem.Id + "_" + _dataItem.Title, @"[^A-Za-z0-9_\.~]+", "-");
            }
        }

        public PageBundle ChangeTextByType(PageBundle pageBundle)
        {
            foreach (PageResources pageResource in pageBundle.PageResources)
            {
                string type = pageBundle.PageTemplateFields.FirstOrDefault(PageTemplateFields => PageTemplateFields.Id == pageResource.PageTemplateFieldId).Type;
                string text = pageResource.Text;
                if (type.ToLower() == "textarea")
                {
                    pageBundle.PageResources.FirstOrDefault(PageResources => PageResources.Id == pageResource.Id).Text = text.Replace("\r\n", "\n").Replace("\n", "<br />");
                }
            }

            return pageBundle;
        }

        public Dictionary<string, object> ConvertPageBundleToJson(Site.Data.Pages page, int websiteLanguageId = 0, string url = "")
        {
            if (url == "") { url = new Website(_context, _config).GetWebsiteUrl(_config.Value.WebsiteId); }

            if (_pages == null && _websiteBundle == null && websiteLanguageId != 0)
            {
                SetPageBundleAndWebsiteBundle(websiteLanguageId);
            }

            Dictionary<string, object> uploads = new Dictionary<string, object>();
            foreach (PageTemplateUploads pageTemplateUpload in page.PageTemplate.PageTemplateUploads)
            {
                List<Dictionary<string, object>> files = new List<Dictionary<string, object>>();
                foreach (PageFiles pageFile in page.PageFiles.Where(PageFiles => PageFiles.PageTemplateUploadId == pageTemplateUpload.Id && PageFiles.Active == true))
                {
                    files.Add(new Dictionary<string, object>()
                    {
                        { "originalPath", url + pageFile.OriginalPath.Replace("~/", "/").Replace(" ", "%20") },
                        { "compressedPath", url + pageFile.CompressedPath.Replace("~/", "/").Replace(" ", "%20") },
                        { "alt", pageFile.Alt }
                    });
                }

                uploads.Add(pageTemplateUpload.CallName, files);
            }

            Dictionary<string, object> fields = new Dictionary<string, object>();
            foreach (PageTemplateFields pageTemplateField in page.PageTemplate.PageTemplateFields)
            {
                if (pageTemplateField.Type.ToLower() != "selectlinkedto") //selectlinkedto is not available yet for pages
                {
                    string text = page.PageResources.FirstOrDefault(PageResources => PageResources.PageTemplateFieldId == pageTemplateField.Id).Text;
                    if (pageTemplateField.Type.ToLower() == "textarea")
                    {
                        text = text.Replace("\r\n", "\n").Replace("\n", "<br />");
                    }

                    fields.Add(pageTemplateField.CallName, text);
                }
            }

            Dictionary<string, object> result = new Dictionary<string, object>() {
                { "title", page.Title },
                { "keywords", page.Keywords },
                { "description", page.Description },
                { "alternateGuid", page.AlternateGuid },
                { "active", page.Active },
                { "files", uploads },
                { "resources", fields },
                { "url", _pages != null ? GetPageUrlByAlternateGuid(page.AlternateGuid) : "" }
            };

            if (AddProductJson) { result = AddProductToPageBundleJson(result, page, websiteLanguageId); }
            if (AddBreadcrumbsJson && _pages != null) { result = AddBreadcrumbsToJson(result, page, websiteLanguageId); }

            return result;
        }

        public Dictionary<string, object> AddProductToPageBundleJson(Dictionary<string, object> result, Site.Data.Pages page, int websiteLanguageId)
        {
            Product product = new Product(_context, _config);
            List<Products> _products = product.GetProductsByPageAlternateGuidAndWebsiteLanguageId(page.AlternateGuid, websiteLanguageId);
            Products _product = _products.OrderBy(x => x.Price).FirstOrDefault();
            Products _productPromo = _products.OrderBy(x => x.PromoPrice).FirstOrDefault();

            if (_product != null) { 
                _product = (_product.Price <= _productPromo.PromoPrice) ? _product : _productPromo;

                result.Add("product", product.ConvertProductBundleToJson(product.GetProductBundleByProductId(websiteLanguageId, page.AlternateGuid, _product.Id, true), websiteLanguageId));
            }

            return result;
        }

        public Dictionary<string, object> AddBreadcrumbsToJson(Dictionary<string, object> result, Site.Data.Pages page, int websiteLanguageId, DataBundle dataBundle = null, ProductBundle productBundle = null)
        {
            List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();

            //Add detail page
            if (dataBundle != null)
            {
                list.Add(new Dictionary<string, object>()
                {
                    { "name", dataBundle.DataItem.Title },
                    { "url", GetPageUrlByAlternateGuid(dataBundle.DataTemplate.PageAlternateGuid) + "/" + dataBundle.DataItem.PageUrl }
                });
            }

            //Add product page
            if (productBundle != null)
            {
                list.Add(new Dictionary<string, object>()
                {
                    { "name", new Product(_context, _config).GetProductResourceTextByCallName(productBundle, "title") },
                    { "url", GetPageUrlByAlternateGuid(productBundle.ProductPage.PageAlternateGuid) + "/" + productBundle.ProductPageSetting.Url }
                });
            }

            string alternateGuid = "";
            if (dataBundle != null)
            {
                alternateGuid = dataBundle.DataTemplate.PageAlternateGuid;
            }
            else if (productBundle != null)
            {
                alternateGuid = productBundle.ProductPage.PageAlternateGuid;
            }

            //Create list of breadcrumbs
            list = CreateBreadcrumbPageList(list, page != null ? page : GetPage(websiteLanguageId, alternateGuid));

            //Get root page for the first breadcrumb
            Site.Data.Pages _page = GetRootPage(websiteLanguageId);

            //Add root page to breadcrumbs
            list.Add(new Dictionary<string, object>()
            {
                { "name", _page.Name },
                { "url", GetPageUrlByAlternateGuid(_page.AlternateGuid) }
            });

            //Reverse list and add to result
            result.Add("breadcrumbs", list.AsEnumerable().Reverse().ToList());

            return result;
        }

        public List<Dictionary<string, object>> CreateBreadcrumbPageList(List<Dictionary<string, object>> list, Site.Data.Pages page)
        {
            list.Add(new Dictionary<string, object>()
            {
                { "name", page.Name },
                { "url", GetPageUrlByAlternateGuid(page.AlternateGuid) }
            });

            return page.Parent != 0 ? CreateBreadcrumbPageList(list, _pages.FirstOrDefault(Page => Page.Id == page.Parent)) : list;
        }

        public List<Dictionary<string, object>> ConvertPageBundlesToJson(List<Site.Data.Pages> pages, int websiteLanguageId = 0)
        {
            string url = new Website(_context, _config).GetWebsiteUrl(_config.Value.WebsiteId);

            if (websiteLanguageId != 0)
            {
                SetPageBundleAndWebsiteBundle(websiteLanguageId);
            }

            List<Dictionary<string, object>> pageBundleList = new List<Dictionary<string, object>>();
            foreach (Site.Data.Pages page in pages)
            {
                pageBundleList.Add(ConvertPageBundleToJson(page, websiteLanguageId, url));
            }

            return pageBundleList;
        }


        public Dictionary<string, string> CreatePageUrlsAndConvertToJson(int websiteLanguageId, string[] alternateGuids, string[] settingValues)
        {
            SetPageBundleAndWebsiteBundle(websiteLanguageId);

            Dictionary<string, string> pageUrls = new Dictionary<string, string>();
            foreach (string alternateGuid in alternateGuids)
            {
                pageUrls.Add(alternateGuid, GetPageUrlByAlternateGuid(alternateGuid));
            }

            if (settingValues.Count() != 0) {
                Setting setting = new Setting(_context);
                foreach (string settingValue in settingValues)
                {
                    pageUrls.Add(settingValue, GetPageUrlByAlternateGuid(setting.GetSettingValueByKey(settingValue, "website", _config.Value.WebsiteId)));
                }
            }

            return pageUrls;
        }

        public ObjectResult GetPageBundlesByTypeJson(int websiteLanguageId, string type, bool active)
        {
            return Ok(ConvertPageBundlesToJson(GetPageBundlesByType(websiteLanguageId, type, active), websiteLanguageId));
        }

        public ObjectResult GetPageBundleJson(int pageId, int websiteLanguageId)
        {
            return Ok(ConvertPageBundleToJson(GetPageBundle(pageId), websiteLanguageId));
        }

        public ObjectResult GetPageUrlsJson(int websiteLanguageId, string[] alternateGuids, string[] settingValues)
        {
            return Ok(CreatePageUrlsAndConvertToJson(websiteLanguageId, alternateGuids, settingValues));
        }
    }
}
