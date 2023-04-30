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
using Microsoft.Extensions.Options;
using static Site.Startup;
using Microsoft.EntityFrameworkCore.Internal;
using System.Collections;
using System.Reflection;
using static Site.Models.Routing;
using Site.Models.App;
using System.Text.RegularExpressions;

namespace Site.Models
{
    public class Data : Controller
    {
        private readonly SiteContext _context;
        private readonly IOptions<AppSettings> _config;

        public Data(SiteContext context = null, IOptions<AppSettings> config = null)
        {
            _context = context;
            _config = config;
        }

        public class DataBundle
        {
            public DataItemFiles DataItemFile { get; set; }
            public IEnumerable<DataItemFiles> DataItemFiles { get; set; }
            public DataItemResources DataItemResource { get; set; }
            public IEnumerable<DataItemResources> DataItemResources { get; set; }
            public DataItems DataItem { get; set; }
            public DataTemplateFields DataTemplateField { get; set; }
            public IEnumerable<DataTemplateFields> DataTemplateFields { get; set; }
            public DataTemplates DataTemplate { get; set; }
            public DataTemplateUploads DataTemplateUpload { get; set; }
            public IEnumerable<DataTemplateUploads> DataTemplateUploads { get; set; }
            public DataTemplates LinkedToDataTemplate { get; set; }
            public IEnumerable<DataItems> LinkedToDataItems { get; set; }
        }

        private Page _page;
        public bool AddBreadcrumbsJson = false;

        public string GetUrlBeforeDataItemByPageAlternateGuid(int websiteLanguageId, string pageAlternateGuid)
        {
            DataRoutes _dataRoute = _context.Pages.Join(_context.WebsiteLanguages, Pages => Pages.WebsiteLanguageId, WebsiteLanguages => WebsiteLanguages.Id, (Pages, WebsiteLanguages) => new { Pages, WebsiteLanguages })
                                                  .Join(_context.Languages, x => x.WebsiteLanguages.LanguageId, Languages => Languages.Id, (x, Languages) => new { x.Pages, x.WebsiteLanguages, Languages })
                                                  .Where(x => x.WebsiteLanguages.Id == websiteLanguageId)
                                                  .Where(x => x.Pages.AlternateGuid == pageAlternateGuid)
                                                  .Where(x => x.WebsiteLanguages.Active == true)
                                                  .Select(x => new DataRoutes()
                                                  {
                                                      DataTemplate = null,
                                                      Page = x.Pages,
                                                      WebsiteLanguage = x.WebsiteLanguages,
                                                      Language = x.Languages
                                                  }).FirstOrDefault();

            List<Site.Data.Pages> _pages = new Routing(_context, _config).GetPageRoutes(websiteLanguageId);

            string url = "";
            if (_dataRoute != null)
            {
                return url = new Routing().FindParentUrl(_dataRoute.Page.Parent, _dataRoute.Page.Url, _pages, _dataRoute.WebsiteLanguage.DefaultLanguage, _dataRoute.Language.Code);
            }

            return "/" + url;
        }

        public DataBundle GetPreviousDataBundle(int websiteLanguageId, string callName, int customOrder)
        {
            return _context.DataTemplates.Join(_context.DataItems.OrderBy(DataItems => DataItems.CustomOrder), DataTemplates => DataTemplates.Id, DataItems => DataItems.DataTemplateId, (DataTemplates, DataItems) => new { DataTemplates, DataItems })
                                         .Where(x => x.DataTemplates.DetailPage == true)
                                         .Where(x => x.DataTemplates.CallName == callName)
                                         .Where(x => x.DataItems.WebsiteLanguageId == websiteLanguageId)
                                         .Where(x => x.DataItems.Active == true)
                                         .Select(x => new DataBundle()
                                         {
                                             DataItemFiles = null,
                                             DataItemResources = null,
                                             DataItem = x.DataItems,
                                             DataTemplateFields = null,
                                             DataTemplate = x.DataTemplates,
                                             DataTemplateUploads = null
                                         })
                                         .OrderByDescending(x => x.DataItem.CustomOrder)
                                         .FirstOrDefault(x => x.DataItem.CustomOrder < customOrder);
        }

        public DataBundle GetNextDataBundle(int websiteLanguageId, string callName, int customOrder)
        {
            return _context.DataTemplates.Join(_context.DataItems.OrderBy(DataItems => DataItems.CustomOrder), DataTemplates => DataTemplates.Id, DataItems => DataItems.DataTemplateId, (DataTemplates, DataItems) => new { DataTemplates, DataItems })
                                         .Where(x => x.DataTemplates.DetailPage == true)
                                         .Where(x => x.DataTemplates.CallName == callName)
                                         .Where(x => x.DataItems.WebsiteLanguageId == websiteLanguageId)
                                         .Where(x => x.DataItems.Active == true)
                                         .Select(x => new DataBundle()
                                         {
                                             DataItemFiles = null,
                                             DataItemResources = null,
                                             DataItem = x.DataItems,
                                             DataTemplateFields = null,
                                             DataTemplate = x.DataTemplates,
                                             DataTemplateUploads = null
                                         })
                                         .OrderBy(x => x.DataItem.CustomOrder)
                                         .FirstOrDefault(x => x.DataItem.CustomOrder > customOrder);
        }

        public DataBundle GetDataBundle(int websiteLanguageId, string callName, string url)
        {
            return _context.DataTemplates.Join(_context.DataItems.OrderBy(DataItems => DataItems.CustomOrder), DataTemplates => DataTemplates.Id, DataItems => DataItems.DataTemplateId, (DataTemplates, DataItems) => new { DataTemplates, DataItems })
                                         .GroupJoin(_context.DataItemFiles.Where(DataItemFiles => DataItemFiles.Active == true)
                                                                          .Join(_context.DataTemplateUploads, DataItemFiles => DataItemFiles.DataTemplateUploadId, DataTemplateUploads => DataTemplateUploads.Id, (DataItemFiles, DataTemplateUploads) => new { DataItemFiles, DataTemplateUploads }).OrderBy(x => x.DataItemFiles.CustomOrder), x => x.DataItems.Id, x => x.DataItemFiles.DataItemId, (x, DataItemFiles) => new { x.DataItems, x.DataTemplates, DataItemFiles })
                                         .GroupJoin(_context.DataItemResources.Join(_context.DataTemplateFields, DataItemResources => DataItemResources.DataTemplateFieldId, DataTemplateFields => DataTemplateFields.Id, (DataItemResources, DataTemplateFields) => new { DataItemResources, DataTemplateFields }), x => x.DataItems.Id, x => x.DataItemResources.DataItemId, (x, DataItemResources) => new { x.DataItems, x.DataTemplates, x.DataItemFiles, DataItemResources })
                                         .Where(x => x.DataTemplates.DetailPage == true)
                                         .Where(x => x.DataTemplates.CallName == callName)
                                         .Where(x => x.DataItems.WebsiteLanguageId == websiteLanguageId)
                                         .Where(x => x.DataItems.Active == true)
                                         .Select(x => new DataBundle()
                                         {
                                             DataItemFiles = x.DataItemFiles.Select(y => y.DataItemFiles),
                                             DataItemResources = x.DataItemResources.Select(y => y.DataItemResources),
                                             DataItem = x.DataItems,
                                             DataTemplateFields = x.DataItemResources.Select(y => y.DataTemplateFields),
                                             DataTemplate = x.DataTemplates,
                                             DataTemplateUploads = x.DataItemFiles.Select(y => y.DataTemplateUploads)
                                         })
                                         .FirstOrDefault(x => x.DataItem.PageUrl == url);
        }

        public List<DataBundle> GetDataBundles(int websiteLanguageId, string callName)
        {
            return _context.DataTemplates.Join(_context.DataItems.OrderBy(DataItems => DataItems.CustomOrder), DataTemplates => DataTemplates.Id, DataItems => DataItems.DataTemplateId, (DataTemplates, DataItems) => new { DataTemplates, DataItems })
                                         .GroupJoin(_context.DataItemFiles.Where(DataItemFiles => DataItemFiles.Active == true).Join(_context.DataTemplateUploads, DataItemFiles => DataItemFiles.DataTemplateUploadId, DataTemplateUploads => DataTemplateUploads.Id, (DataItemFiles, DataTemplateUploads) => new { DataItemFiles, DataTemplateUploads }).OrderBy(x => x.DataItemFiles.CustomOrder), x => x.DataItems.Id, x => x.DataItemFiles.DataItemId, (x, DataItemFiles) => new { x.DataItems, x.DataTemplates, DataItemFiles })
                                         .GroupJoin(_context.DataItemResources.Join(_context.DataTemplateFields, DataItemResources => DataItemResources.DataTemplateFieldId, DataTemplateFields => DataTemplateFields.Id, (DataItemResources, DataTemplateFields) => new { DataItemResources, DataTemplateFields }), x => x.DataItems.Id, x => x.DataItemResources.DataItemId, (x, DataItemResources) => new { x.DataItems, x.DataTemplates, x.DataItemFiles, DataItemResources })
                                         .Where(x => x.DataTemplates.CallName == callName)
                                         .Where(x => x.DataItems.WebsiteLanguageId == websiteLanguageId)
                                         .Where(x => x.DataItems.Active == true)
                                         .Select(x => new DataBundle()
                                         {
                                             DataItemFiles = x.DataItemFiles.Select(y => y.DataItemFiles),
                                             DataItemResources = x.DataItemResources.Select(y => y.DataItemResources),
                                             DataItem = x.DataItems,
                                             DataTemplateFields = x.DataItemResources.Select(y => y.DataTemplateFields),
                                             DataTemplate = x.DataTemplates,
                                             DataTemplateUploads = x.DataItemFiles.Select(y => y.DataTemplateUploads)
                                         }).ToList();
        }

        public List<DataBundle> GetMaxDataBundles(int websiteLanguageId, string callName, int max)
        {
            return _context.DataTemplates.Join(_context.DataItems.OrderBy(DataItems => DataItems.CustomOrder), DataTemplates => DataTemplates.Id, DataItems => DataItems.DataTemplateId, (DataTemplates, DataItems) => new { DataTemplates, DataItems })
                                         .GroupJoin(_context.DataItemFiles.Where(DataItemFiles => DataItemFiles.Active == true)
                                                                          .Join(_context.DataTemplateUploads, DataItemFiles => DataItemFiles.DataTemplateUploadId, DataTemplateUploads => DataTemplateUploads.Id, (DataItemFiles, DataTemplateUploads) => new { DataItemFiles, DataTemplateUploads }).OrderBy(x => x.DataItemFiles.CustomOrder), x => x.DataItems.Id, x => x.DataItemFiles.DataItemId, (x, DataItemFiles) => new { x.DataItems, x.DataTemplates, DataItemFiles })
                                         .GroupJoin(_context.DataItemResources.Join(_context.DataTemplateFields, DataItemResources => DataItemResources.DataTemplateFieldId, DataTemplateFields => DataTemplateFields.Id, (DataItemResources, DataTemplateFields) => new { DataItemResources, DataTemplateFields }), x => x.DataItems.Id, x => x.DataItemResources.DataItemId, (x, DataItemResources) => new { x.DataItems, x.DataTemplates, x.DataItemFiles, DataItemResources })
                                         .Where(x => x.DataTemplates.CallName == callName)
                                         .Where(x => x.DataItems.WebsiteLanguageId == websiteLanguageId)
                                         .Where(x => x.DataItems.Active == true)
                                         .Select(x => new DataBundle()
                                         {
                                             DataItemFiles = x.DataItemFiles.Select(y => y.DataItemFiles),
                                             DataItemResources = x.DataItemResources.Select(y => y.DataItemResources),
                                             DataItem = x.DataItems,
                                             DataTemplateFields = x.DataItemResources.Select(y => y.DataTemplateFields),
                                             DataTemplate = x.DataTemplates,
                                             DataTemplateUploads = x.DataItemFiles.Select(y => y.DataTemplateUploads)
                                         })
                                         .Take(max)
                                         .ToList();
        }

        public List<DataBundle> GetMaxDataBundlesOrderByPublishDate(int websiteLanguageId, string callName, int max)
        {
            return _context.DataTemplates.Join(_context.DataItems.OrderByDescending(DataItems => DataItems.PublishDate), DataTemplates => DataTemplates.Id, DataItems => DataItems.DataTemplateId, (DataTemplates, DataItems) => new { DataTemplates, DataItems })
                                         .GroupJoin(_context.DataItemFiles.Where(DataItemFiles => DataItemFiles.Active == true)
                                                                          .Join(_context.DataTemplateUploads, DataItemFiles => DataItemFiles.DataTemplateUploadId, DataTemplateUploads => DataTemplateUploads.Id, (DataItemFiles, DataTemplateUploads) => new { DataItemFiles, DataTemplateUploads }).OrderBy(x => x.DataItemFiles.CustomOrder), x => x.DataItems.Id, x => x.DataItemFiles.DataItemId, (x, DataItemFiles) => new { x.DataItems, x.DataTemplates, DataItemFiles })
                                         .GroupJoin(_context.DataItemResources.Join(_context.DataTemplateFields, DataItemResources => DataItemResources.DataTemplateFieldId, DataTemplateFields => DataTemplateFields.Id, (DataItemResources, DataTemplateFields) => new { DataItemResources, DataTemplateFields }), x => x.DataItems.Id, x => x.DataItemResources.DataItemId, (x, DataItemResources) => new { x.DataItems, x.DataTemplates, x.DataItemFiles, DataItemResources })
                                         .Where(x => x.DataTemplates.CallName == callName)
                                         .Where(x => x.DataItems.WebsiteLanguageId == websiteLanguageId)
                                         .Where(x => x.DataItems.Active == true)
                                         .Select(x => new DataBundle()
                                         {
                                             DataItemFiles = x.DataItemFiles.Select(y => y.DataItemFiles),
                                             DataItemResources = x.DataItemResources.Select(y => y.DataItemResources),
                                             DataItem = x.DataItems,
                                             DataTemplateFields = x.DataItemResources.Select(y => y.DataTemplateFields),
                                             DataTemplate = x.DataTemplates,
                                             DataTemplateUploads = x.DataItemFiles.Select(y => y.DataTemplateUploads)
                                         }).Take(max).ToList();
        }

        public DataBundle GetDataBundleWithCategories(int websiteLanguageId, string callName, string url)
        {
            return _context.DataTemplates.Join(_context.DataItems.OrderBy(DataItems => DataItems.CustomOrder), DataTemplates => DataTemplates.Id, DataItems => DataItems.DataTemplateId, (DataTemplates, DataItems) => new { DataTemplates, DataItems })
                                         .GroupJoin(_context.DataItemFiles.Where(DataItemFiles => DataItemFiles.Active == true)
                                                                          .Join(_context.DataTemplateUploads, DataItemFiles => DataItemFiles.DataTemplateUploadId, DataTemplateUploads => DataTemplateUploads.Id, (DataItemFiles, DataTemplateUploads) => new { DataItemFiles, DataTemplateUploads })
                                                                          .OrderBy(x => x.DataItemFiles.CustomOrder), x => x.DataItems.Id, x => x.DataItemFiles.DataItemId, (x, DataItemFiles) => new { x.DataItems, x.DataTemplates, DataItemFiles })
                                         .GroupJoin(_context.DataItemResources.Join(_context.DataTemplateFields, DataItemResources => DataItemResources.DataTemplateFieldId, DataTemplateFields => DataTemplateFields.Id, (DataItemResources, DataTemplateFields) => new { DataItemResources, DataTemplateFields }) //Here comes the DataTemplateFields.LinkedToDataTemplateId
                                                                              .GroupJoin(_context.DataItems.OrderBy(DataItems => DataItems.CustomOrder).Where(DataItems => DataItems.Active == true).Join(_context.DataTemplates, DataItems => DataItems.DataTemplateId, DataTemplates => DataTemplates.Id, (DataItems, DataTemplates) => new { DataItems, DataTemplates }), x => x.DataItemResources.Text, y => y.DataItems.Id.ToString(), (x, DataItems) => new { x.DataItemResources, x.DataTemplateFields, DataItems }),
                                         x => x.DataItems.Id, x => x.DataItemResources.DataItemId, (x, DataItemResources) => new { x.DataItems, x.DataTemplates, x.DataItemFiles, DataItemResources })
                                         .Where(x => x.DataTemplates.DetailPage == true)
                                         .Where(x => x.DataTemplates.CallName == callName)
                                         .Where(x => x.DataItems.WebsiteLanguageId == websiteLanguageId)
                                         .Where(x => x.DataItems.Active == true)
                                         .Select(x => new DataBundle()
                                         {
                                             DataItemFiles = x.DataItemFiles.Select(y => y.DataItemFiles),
                                             DataItemResources = x.DataItemResources.Select(y => y.DataItemResources),
                                             DataItem = x.DataItems,
                                             DataTemplateFields = x.DataItemResources.Select(y => y.DataTemplateFields),
                                             DataTemplate = x.DataTemplates,
                                             DataTemplateUploads = x.DataItemFiles.Select(y => y.DataTemplateUploads),
                                             LinkedToDataTemplate = null,
                                             LinkedToDataItems = x.DataItemResources.Select(y => y.DataItems.FirstOrDefault().DataItems)
                                         })
                                         .FirstOrDefault(x => x.DataItem.PageUrl == url);
        }

        public List<DataBundle> GetDataBundlesWithCategorie(int WebsiteLanguageId, string CallName)
        {
            return _context.DataTemplates.Join(_context.DataItems.OrderBy(DataItems => DataItems.CustomOrder), DataTemplates => DataTemplates.Id, DataItems => DataItems.DataTemplateId, (DataTemplates, DataItems) => new { DataTemplates, DataItems })
                                         .GroupJoin(_context.DataItemFiles.Where(DataItemFiles => DataItemFiles.Active == true)
                                                    .Join(_context.DataTemplateUploads, DataItemFiles => DataItemFiles.DataTemplateUploadId, DataTemplateUploads => DataTemplateUploads.Id, (DataItemFiles, DataTemplateUploads) => new { DataItemFiles, DataTemplateUploads })
                                                    .OrderBy(x => x.DataItemFiles.CustomOrder),
                                         x => x.DataItems.Id, x => x.DataItemFiles.DataItemId, (x, DataItemFiles) => new { x.DataItems, x.DataTemplates, DataItemFiles })
                                         .GroupJoin(_context.DataItemResources
                                                    .Join(_context.DataTemplateFields, DataItemResources => DataItemResources.DataTemplateFieldId, DataTemplateFields => DataTemplateFields.Id, (DataItemResources, DataTemplateFields) => new { DataItemResources, DataTemplateFields })
                                                    .GroupJoin(_context.DataItems.OrderBy(DataItems => DataItems.CustomOrder).Where(DataItems => DataItems.Active == true)
                                                               .Join(_context.DataTemplates, DataItems => DataItems.DataTemplateId, DataTemplates => DataTemplates.Id, (DataItems, DataTemplates) => new { DataItems, DataTemplates }),
                                                    x => x.DataItemResources.Text, y => y.DataItems.Id.ToString(), (x, DataItems) => new { x.DataItemResources, x.DataTemplateFields, DataItems }),
                                         x => x.DataItems.Id, x => x.DataItemResources.DataItemId, (x, DataItemResources) => new { x.DataItems, x.DataTemplates, x.DataItemFiles, DataItemResources })
                                         .Where(x => x.DataTemplates.CallName == CallName)
                                         .Where(x => x.DataItems.WebsiteLanguageId == WebsiteLanguageId)
                                         .Where(x => x.DataItems.Active == true)
                                         .Select(x => new DataBundle()
                                         {
                                             DataItemFiles = x.DataItemFiles.Select(y => y.DataItemFiles),
                                             DataItemResources = x.DataItemResources.Select(y => y.DataItemResources),
                                             DataItem = x.DataItems,
                                             DataTemplateFields = x.DataItemResources.Select(y => y.DataTemplateFields),
                                             DataTemplate = x.DataTemplates,
                                             DataTemplateUploads = x.DataItemFiles.Select(y => y.DataTemplateUploads),
                                             LinkedToDataTemplate = null,
                                             LinkedToDataItems = x.DataItemResources.Select(y => y.DataItems.FirstOrDefault().DataItems)
                                         }).ToList();
        }

        public List<DataBundle> GetDataBundlesByResourceTextAndFieldTypeAndResourceCallName(int websiteLanguageId, string text, string type, string callName, bool active)
        {
            return _context.DataTemplates.Join(_context.DataItems.OrderBy(DataItems => DataItems.CustomOrder), DataTemplates => DataTemplates.Id, DataItems => DataItems.DataTemplateId, (DataTemplates, DataItems) => new { DataTemplates, DataItems })
                                         .GroupJoin(_context.DataItemFiles.Where(DataItemFiles => DataItemFiles.Active == true).Join(_context.DataTemplateUploads, DataItemFiles => DataItemFiles.DataTemplateUploadId, DataTemplateUploads => DataTemplateUploads.Id, (DataItemFiles, DataTemplateUploads) => new { DataItemFiles, DataTemplateUploads }).OrderBy(x => x.DataItemFiles.CustomOrder), x => x.DataItems.Id, x => x.DataItemFiles.DataItemId, (x, DataItemFiles) => new { x.DataItems, x.DataTemplates, DataItemFiles })
                                         .GroupJoin(_context.DataItemResources.Join(_context.DataTemplateFields, DataItemResources => DataItemResources.DataTemplateFieldId, DataTemplateFields => DataTemplateFields.Id, (DataItemResources, DataTemplateFields) => new { DataItemResources, DataTemplateFields }), x => x.DataItems.Id, x => x.DataItemResources.DataItemId, (x, DataItemResources) => new { x.DataItems, x.DataTemplates, x.DataItemFiles, DataItemResources })
                                         .Join(_context.DataItemResources.Where(DataItemResource => DataItemResource.Text == text)
                                            .Join(_context.DataTemplateFields.Where(DataTemplateField => DataTemplateField.Type.ToLower() == type && DataTemplateField.CallName.ToLower() == callName), DataItemResource => DataItemResource.DataTemplateFieldId, DataTemplateField => DataTemplateField.Id, (DataItemResource, DataTemplateField) => new { DataItemResource, DataTemplateField })
                                         , x => x.DataItems.Id, y => y.DataItemResource.DataItemId, (x, y) => new { x.DataItems, x.DataTemplates, x.DataItemFiles, x.DataItemResources, y })
                                         .Where(x => x.DataItems.Active == active)
                                         .Select(x => new DataBundle()
                                         {
                                             DataItemFiles = x.DataItemFiles.Select(y => y.DataItemFiles),
                                             DataItemResources = x.DataItemResources.Select(y => y.DataItemResources),
                                             DataItem = x.DataItems,
                                             DataTemplateFields = x.DataItemResources.Select(y => y.DataTemplateFields),
                                             DataTemplate = x.DataTemplates,
                                             DataTemplateUploads = x.DataItemFiles.Select(y => y.DataTemplateUploads)
                                         }).ToList();
        }

        public string GetSectionOrFilter(DataRoutes dataRoute, NavigationItems navigationItem)
        {
            DataTemplateSections dataTemplateSection = dataRoute.DataTemplateSections.FirstOrDefault(DataTemplateSections => DataTemplateSections.Id == navigationItem.LinkedToSectionId);
            switch (dataTemplateSection.Type.ToLower())
            {
                case "section":
                    return dataTemplateSection.Section;
                default: // "datafilter"
                    DataItems _dataItem = _context.DataItems.FirstOrDefault(DataItems => DataItems.AlternateGuid == navigationItem.FilterAlternateGuid);
                    return Regex.Replace(_dataItem.Id + "_" + _dataItem.Title, @"[^A-Za-z0-9_\.~]+", "-");
            }
        }

        public DataBundle ChangeDataBundleTextByType(DataBundle dataBundle)
        {
            dataBundle.DataItem.Text = dataBundle.DataItem.Text.Replace("\r\n", "\n").Replace("\n", "<br />");

            if (dataBundle.DataItemResources != null)
            {
                foreach (DataItemResources dataItemResource in dataBundle.DataItemResources)
                {
                    string type = dataBundle.DataTemplateFields.FirstOrDefault(DataTemplateFields => DataTemplateFields.Id == dataItemResource.DataTemplateFieldId).Type;
                    string text = dataItemResource.Text;
                    if (type.ToLower() == "textarea")
                    {
                        dataBundle.DataItemResources.FirstOrDefault(DataItemResources => DataItemResources.Id == dataItemResource.Id).Text = text.Replace("\r\n", "\n").Replace("\n", "<br />");
                    }
                }
            }

            return dataBundle;
        }

        public List<DataBundle> ChangeDataBundlesTextByType(List<DataBundle> dataBundles)
        {
            foreach (DataBundle dataBundle in dataBundles)
            {
                dataBundles.FirstOrDefault(x => x.DataItem.Id == dataBundle.DataItem.Id).DataItem.Text = dataBundle.DataItem.Text.Replace("\r\n", "\n").Replace("\n", "<br />");

                if (dataBundle.DataItemResources != null)
                {
                    foreach (DataItemResources dataItemResource in dataBundle.DataItemResources)
                    {
                        string type = dataBundle.DataTemplateFields.FirstOrDefault(DataTemplateFields => DataTemplateFields.Id == dataItemResource.DataTemplateFieldId).Type;
                        string text = dataItemResource.Text;
                        if (type.ToLower() == "textarea")
                        {
                            dataBundles.FirstOrDefault(x => x.DataItem.Id == dataBundle.DataItem.Id).DataItemResources.FirstOrDefault(DataItemResources => DataItemResources.Id == dataItemResource.Id).Text = text.Replace("\r\n", "\n").Replace("\n", "<br />");
                        }
                    }
                }
            }

            return dataBundles;
        }

        public DataBundle ChangeDataBundleFilePaths(DataBundle dataBundle)
        {
            string url = new Website(_context, _config).GetWebsiteUrl(_config.Value.WebsiteId);

            foreach (DataItemFiles dataItemFile in dataBundle.DataItemFiles)
            {
                int dataItemFileId = dataItemFile.Id;
                string compressedPath = dataItemFile.CompressedPath.Replace("~/", url + "/").Replace(" ", "%20");
                string originalPath = dataItemFile.OriginalPath.Replace("~/", url + "/").Replace(" ", "%20");

                dataBundle.DataItemFiles.FirstOrDefault(DataItemFiles => DataItemFiles.Id == dataItemFileId).CompressedPath = compressedPath;
                dataBundle.DataItemFiles.FirstOrDefault(DataItemFiles => DataItemFiles.Id == dataItemFileId).OriginalPath = originalPath;
            }

            return dataBundle;
        }

        public List<DataBundle> ChangeDataBundlesFilePaths(List<DataBundle> dataBundles)
        {
            string url = new Website(_context, _config).GetWebsiteUrl(_config.Value.WebsiteId);

            foreach (DataBundle dataBundle in dataBundles)
            {
                foreach (DataItemFiles dataItemFile in dataBundle.DataItemFiles)
                {
                    int dataItemFileId = dataItemFile.Id;
                    int dataItemId = dataBundle.DataItem.Id;
                    string compressedPath = dataItemFile.CompressedPath.Replace("~/", url + "/");
                    string originalPath = dataItemFile.OriginalPath.Replace("~/", url + "/");

                    dataBundles.FirstOrDefault(x => x.DataItem.Id == dataItemId).DataItemFiles.FirstOrDefault(DataItemFiles => DataItemFiles.Id == dataItemFileId).CompressedPath = compressedPath;
                    dataBundles.FirstOrDefault(x => x.DataItem.Id == dataItemId).DataItemFiles.FirstOrDefault(DataItemFiles => DataItemFiles.Id == dataItemFileId).OriginalPath = originalPath;
                }
            }

            return dataBundles;
        }

        public List<Dictionary<string, object>> ConvertDataBundlesToJson(List<DataBundle> dataBundles, int websiteLanguageId)
        {
            string url = new Website(_context, _config).GetWebsiteUrl(_config.Value.WebsiteId);

            _page = new Page(_context, _config);
            _page.SetPageBundleAndWebsiteBundle(websiteLanguageId);

            List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();
            foreach (DataBundle dataBundle in dataBundles)
            {
                list.Add(ConvertDataBundleToJson(dataBundle, websiteLanguageId, url));
            }

            return list;
        }

        public List<DataItems> GetLinkedDataItemsFromDataBundleByCallName(DataBundle dataBundle, string callName, int dataTemplateFieldId)
        {
            return dataBundle.DataTemplateFields.Join(dataBundle.DataItemResources.Where(DataItemResources => DataItemResources.DataTemplateFieldId == dataTemplateFieldId), DataTemplateFields => DataTemplateFields.Id, DataItemResources => DataItemResources.DataTemplateFieldId, (DataTemplateFields, DataItemResources) => new { DataTemplateFields, DataItemResources })
                                                .Join(dataBundle.LinkedToDataItems, x => x.DataItemResources.Text, LinkedToDataItems => (LinkedToDataItems != null) ? LinkedToDataItems.Id.ToString() : "0", (x, LinkedToDataItems) => new { x.DataTemplateFields, LinkedToDataItems })
                                                .Where(x => x.DataTemplateFields.CallName.ToLower() == callName)
                                                .Select(x => x.LinkedToDataItems).ToList();
        }

        public Dictionary<string, object> ConvertDataBundleToJson(DataBundle dataBundle, int websiteLanguageId, string url = "")
        {
            if (url == "") { url = new Website(_context, _config).GetWebsiteUrl(_config.Value.WebsiteId); }

            if (_page == null && websiteLanguageId != 0)
            {
                _page = new Page(_context, _config);
                _page.SetPageBundleAndWebsiteBundle(websiteLanguageId);
            }

            Dictionary<string, object> uploads = new Dictionary<string, object>();
            foreach (DataTemplateUploads dataTemplateUpload in dataBundle.DataTemplateUploads.Distinct())
            {
                List<Dictionary<string, object>> files = new List<Dictionary<string, object>>();
                foreach (DataItemFiles dataItemFile in dataBundle.DataItemFiles.Where(DataItemFiles => DataItemFiles.DataTemplateUploadId == dataTemplateUpload.Id && DataItemFiles.Active == true))
                {
                    files.Add(new Dictionary<string, object>()
                    {
                        { "originalPath", url + dataItemFile.OriginalPath.Replace("~/", "/")},
                        { "compressedPath", url + dataItemFile.CompressedPath.Replace("~/", "/")},
                        { "alt", dataItemFile.Alt}
                    });
                }

                uploads.Add(dataTemplateUpload.CallName, files);
            }

            Dictionary<string, object> fields = new Dictionary<string, object>();
            foreach (DataTemplateFields dataTemplateField in dataBundle.DataTemplateFields.Distinct())
            {
                if (dataTemplateField.Type.ToLower() != "selectlinkedto")
                {
                    string text = dataBundle.DataItemResources.FirstOrDefault(DataItemResources => DataItemResources.DataTemplateFieldId == dataTemplateField.Id).Text;
                    if (dataTemplateField.Type.ToLower() == "textarea")
                    {
                        text = text.Replace("\r\n", "\n").Replace("\n", "<br />");
                    }

                    fields.Add(dataTemplateField.CallName, text);
                }
                else
                {
                    if (dataBundle.LinkedToDataItems != null)
                    {
                        List<DataItems> _dataItems = GetLinkedDataItemsFromDataBundleByCallName(dataBundle, dataTemplateField.CallName, dataTemplateField.Id);

                        List<Dictionary<string, object>> linkedDataItems = new List<Dictionary<string, object>>();
                        foreach (DataItems dataItem in _dataItems.Distinct())
                        {
                            linkedDataItems.Add(new Dictionary<string, object>()
                            {
                                { "title", dataItem.Title },
                                { "subtitle", dataItem.Subtitle },
                                { "text", dataItem.Text },
                                { "htmlEditor", dataItem.HtmlEditor },
                                { "publishData", dataItem.PublishDate },
                                { "fromDate", dataItem.FromDate },
                                { "toDate", dataItem.ToDate },
                                { "active", dataItem.Active },
                                { "pageUrl", _page.GetPageUrlByAlternateGuid(dataBundle.DataTemplate.PageAlternateGuid) + "/" + dataItem.PageUrl },
                                { "pageTitle", dataItem.PageTitle },
                                { "pageKeywords", dataItem.PageKeywords },
                                { "pageDescription", dataItem.PageDescription },
                                { "alternateGuid", dataItem.AlternateGuid },
                                { "customOrder", dataItem.CustomOrder },
                                { "htmlIdentifier", Regex.Replace(dataItem.Id + "_" + dataItem.Title, @"[^A-Za-z0-9_\.~]+", "-") }
                            });
                        }

                        fields.Add(dataTemplateField.CallName, linkedDataItems);
                    }
                }
            }

            Dictionary<string, object> result = new Dictionary<string, object>() {
                { "title", dataBundle.DataItem.Title },
                { "subtitle", dataBundle.DataItem.Subtitle },
                { "text", dataBundle.DataItem.Text },
                { "htmlEditor", dataBundle.DataItem.HtmlEditor },
                { "publishData", dataBundle.DataItem.PublishDate },
                { "fromDate", dataBundle.DataItem.FromDate },
                { "toDate", dataBundle.DataItem.ToDate },
                { "active", dataBundle.DataItem.Active },
                { "pageUrl", _page.GetPageUrlByAlternateGuid(dataBundle.DataTemplate.PageAlternateGuid) + "/" + dataBundle.DataItem.PageUrl },
                { "pageTitle", dataBundle.DataItem.PageTitle },
                { "pageKeywords", dataBundle.DataItem.PageKeywords },
                { "pageDescription", dataBundle.DataItem.PageDescription },
                { "alternateGuid", dataBundle.DataItem.AlternateGuid },
                { "customOrder", dataBundle.DataItem.CustomOrder },
                { "htmlIdentifier", Regex.Replace(dataBundle.DataItem.Id + "_" + dataBundle.DataItem.Title, @"[^A-Za-z0-9_\.~]+", "-") },
                { "files", uploads },
                { "resources", fields }
            };

            if (AddBreadcrumbsJson && websiteLanguageId != 0) { result = _page.AddBreadcrumbsToJson(result, null, websiteLanguageId, dataBundle); }

            return result;
        }

        public ObjectResult GetDataBundleWithChilds(int websiteLanguageId, string callName, string url, string fieldCallName, string type)
        {
            DataBundle _dataBundle = GetDataBundle(websiteLanguageId, callName, url);
            if (_dataBundle == null) return NotFound("");

            Dictionary<string, object> result = new Dictionary<string, object>() {
                { callName, ConvertDataBundleToJson(_dataBundle, websiteLanguageId, "") },
            };
            AddBreadcrumbsJson = false;
            result.Add(fieldCallName, ConvertDataBundlesToJson(GetDataBundlesByResourceTextAndFieldTypeAndResourceCallName(websiteLanguageId, _dataBundle.DataItem.Id.ToString(), type, fieldCallName, true), websiteLanguageId));

            return Ok(result);
        }

        public ObjectResult GetDataBundlesJson(int websiteLanguageId, string callName)
        {
            return Ok(ConvertDataBundlesToJson(GetDataBundles(websiteLanguageId, callName), websiteLanguageId));
        }

        public ObjectResult GetDataBundleJson(int websiteLanguageId, string callName, string url)
        {
            return Ok(ConvertDataBundlesToJson(GetDataBundles(websiteLanguageId, callName), websiteLanguageId));
        }

        public ObjectResult GetDataBundleWithCategoriesJson(int websiteLanguageId, string callName, string url)
        {
            return Ok(ConvertDataBundleToJson(GetDataBundleWithCategories(websiteLanguageId, callName, url), websiteLanguageId));
        }

        public ObjectResult GetDataBundlesWithCategorieJson(int websiteLanguageId, string callName)
        {
            return Ok(ConvertDataBundlesToJson(GetDataBundlesWithCategorie(websiteLanguageId, callName), websiteLanguageId));
        }

        public ObjectResult GetMaxDataBundlesJson(int websiteLanguageId, string callName, int max)
        {
            return Ok(ConvertDataBundlesToJson(GetMaxDataBundles(websiteLanguageId, callName, max), websiteLanguageId));
        }

        public ObjectResult GetMaxDataBundlesOrderByPublishDateJson(int websiteLanguageId, string callName, int max)
        {
            return Ok(ConvertDataBundlesToJson(GetMaxDataBundlesOrderByPublishDate(websiteLanguageId, callName, max), websiteLanguageId));
        }
    }
}
