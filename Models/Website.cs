using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Options;
using Site.Data;
using Site.Models.App;
using System.Collections.Generic;
using System.Linq;
using static Site.Startup;

namespace Site.Models
{
    public class Website : Controller
    {
        SiteContext _context;
        private readonly IOptions<AppSettings> _config;

        public Website(SiteContext context, IOptions<AppSettings> config)
        {
            _context = context;
            _config = config;
        }

        public class WebsiteBundle
        {
            public Languages Language { get; set; }
            public IEnumerable<Languages> Languages { get; set; }
            public IEnumerable<WebsiteFields> WebsiteFields { get; set; }
            public WebsiteFields WebsiteField { get; set; }
            public IEnumerable<WebsiteFiles> WebsiteFiles { get; set; }
            public WebsiteLanguages WebsiteLanguage { get; set; }
            public IEnumerable<WebsiteLanguages> WebsiteLanguages { get; set; }
            public IEnumerable<WebsiteResources> WebsiteResources { get; set; }
            public WebsiteResources WebsiteResource { get; set; }
            public Websites Website { get; set; }
            public IEnumerable<WebsiteUploads> WebsiteUploads { get; set; }
        }

        public string GetWebsiteUrl(int websiteId)
        {
            Websites _website = _context.Websites.FirstOrDefault(Website => Website.Id == websiteId);

            string Subdomain = "";
            if (_website.Subdomain != "")
            {
                Subdomain = _website.Subdomain + ".";
            }

            return _website.TypeClient + "://" + Subdomain + _website.Domain + "." + _website.Extension;
        }

        public WebsiteBundle GetWebsiteBundle(int WebsiteLanguageId)
        {
            return _context.Websites.Join(_context.WebsiteLanguages, Websites => Websites.Id, WebsiteLanguages => WebsiteLanguages.WebsiteId, (Websites, WebsiteLanguages) => new { Websites, WebsiteLanguages })
                                    .GroupJoin(_context.WebsiteFiles.Join(_context.WebsiteUploads, WebsiteFiles => WebsiteFiles.WebsiteUploadId, WebsiteUploads => WebsiteUploads.Id, (WebsiteFiles, WebsiteUploads) => new { WebsiteFiles, WebsiteUploads }), x => x.WebsiteLanguages.Id, x => x.WebsiteFiles.WebsiteLanguageId, (x, WebsiteFiles) => new { x.WebsiteLanguages, x.Websites, WebsiteFiles })
                                    .GroupJoin(_context.WebsiteResources.Join(_context.WebsiteFields, WebsiteResources => WebsiteResources.WebsiteFieldId, WebsiteFields => WebsiteFields.Id, (WebsiteResources, WebsiteFields) => new { WebsiteResources, WebsiteFields }), x => x.WebsiteLanguages.Id, x => x.WebsiteResources.WebsiteLanguageId, (x, WebsiteResources) => new { x.WebsiteLanguages, x.Websites, x.WebsiteFiles, WebsiteResources })
                                    .Where(x => x.WebsiteLanguages.Active == true)
                                    .Where(x => x.WebsiteLanguages.Id == WebsiteLanguageId)
                                    .Select(x => new WebsiteBundle()
                                    {
                                        Languages = null,
                                        WebsiteFields = x.WebsiteResources.Select(y => y.WebsiteFields),
                                        WebsiteFiles = x.WebsiteFiles.Select(y => y.WebsiteFiles),
                                        WebsiteLanguage = x.WebsiteLanguages,
                                        WebsiteLanguages = null,
                                        WebsiteResources = x.WebsiteResources.Select(y => y.WebsiteResources),
                                        Website = x.Websites,
                                        WebsiteUploads = x.WebsiteFiles.Select(y => y.WebsiteUploads)
                                    })
                                    .FirstOrDefault();
        }

        public WebsiteLanguages GetWebsiteLanguage()
        {
            return _context.WebsiteLanguages.FirstOrDefault(WebsiteLanguage => WebsiteLanguage.DefaultLanguage == true && WebsiteLanguage.Active && WebsiteLanguage.WebsiteId == _config.Value.WebsiteId);
        }

        public WebsiteBundle GetWebsiteLanguages()
        {
            return _context.Websites.Where(Websites => Websites.Id == _config.Value.WebsiteId)
                                    .GroupJoin(_context.WebsiteLanguages.Where(WebsiteLanguages => WebsiteLanguages.Active == true)
                                                                        .Join(_context.Languages, WebsiteLanguages => WebsiteLanguages.LanguageId, Languages => Languages.Id, (WebsiteLanguages, Languages) => new { WebsiteLanguages, Languages }), Websites => Websites.Id, x => x.WebsiteLanguages.WebsiteId, (Websites, WebsiteLanguages) => new { Websites, WebsiteLanguages })
                                    .Select(x => new WebsiteBundle()
                                    {
                                        Website = x.Websites,
                                        WebsiteLanguages = x.WebsiteLanguages.Select(y => y.WebsiteLanguages),
                                        Languages = x.WebsiteLanguages.Select(y => y.Languages),
                                    }).FirstOrDefault();
        }

        public ObjectResult GetWebsiteLanguageByDefaultLanguage(bool defaultLanguage)
        {
            return Ok(_context.WebsiteLanguages.FirstOrDefault(x => x.DefaultLanguage == defaultLanguage && x.WebsiteId == _config.Value.WebsiteId));
        }

        public WebsiteBundle ChangeWebsiteBundleTextByType(WebsiteBundle websiteBundle)
        {
            if (websiteBundle.WebsiteResources != null)
            {
                foreach (WebsiteResources websiteResource in websiteBundle.WebsiteResources)
                {
                    string type = websiteBundle.WebsiteFields.FirstOrDefault(WebsiteFields => WebsiteFields.Id == websiteResource.WebsiteFieldId).Type;
                    string text = websiteResource.Text;
                    if (type.ToLower() == "textarea")
                    {
                        websiteBundle.WebsiteResources.FirstOrDefault(WebsiteResources => WebsiteResources.Id == websiteResource.Id).Text = text.Replace("\r\n", "\n").Replace("\n", "<br />");
                    }
                }
            }

            return websiteBundle;
        }

        public WebsiteBundle ChangeWebsiteBundleFilePaths(WebsiteBundle websiteBundle)
        {
            string url = new Website(_context, _config).GetWebsiteUrl(_config.Value.WebsiteId);

            foreach (WebsiteFiles websiteFile in websiteBundle.WebsiteFiles)
            {
                int websiteFileId = websiteFile.Id;
                string compressedPath = websiteFile.CompressedPath.Replace("~/", url + "/").Replace(" ", "%20");
                string originalPath = websiteFile.OriginalPath.Replace("~/", url + "/").Replace(" ", "%20");

                websiteBundle.WebsiteFiles.FirstOrDefault(WebsiteFiles => WebsiteFiles.Id == websiteFileId).CompressedPath = compressedPath;
                websiteBundle.WebsiteFiles.FirstOrDefault(WebsiteFiles => WebsiteFiles.Id == websiteFileId).OriginalPath = originalPath;
            }

            return websiteBundle;
        }

        public string GetCleanWebsiteUrlByWebsiteId(int websiteId)
        {
            Websites _website = _context.Websites.FirstOrDefault(Website => Website.Id == websiteId);

            string Subdomain = "";
            if (_website.Subdomain != "")
            {
                Subdomain = _website.Subdomain + ".";
            }

            return Subdomain + _website.Domain + "." + _website.Extension;
        }

        public Dictionary<string, object> ConvertWebsiteBundleToJson(WebsiteBundle websiteBundle)
        {
            string url = GetWebsiteUrl(_config.Value.WebsiteId);

            Dictionary<string, object> uploads = new Dictionary<string, object>();
            foreach (WebsiteUploads websiteUpload in websiteBundle.WebsiteUploads.Distinct())
            {
                List<Dictionary<string, object>> files = new List<Dictionary<string, object>>();
                foreach (WebsiteFiles websiteFile in websiteBundle.WebsiteFiles.Where(WebsiteFiles => WebsiteFiles.WebsiteUploadId == websiteUpload.Id && WebsiteFiles.Active == true))
                {
                    files.Add(new Dictionary<string, object>()
                    {
                        { "originalPath", url + websiteFile.OriginalPath.Replace("~/", "/")},
                        { "compressedPath", url + websiteFile.CompressedPath.Replace("~/", "/")},
                        { "alt", websiteFile.Alt}
                    });
                }

                uploads.Add(websiteUpload.CallName, files);
            }

            Dictionary<string, object> fields = new Dictionary<string, object>();
            foreach (WebsiteFields websiteField in websiteBundle.WebsiteFields.Distinct())
            {
                if (websiteField.Type.ToLower() != "selectlinkedto") //selectlinkedto is not available yet for website
                {
                    string text = websiteBundle.WebsiteResources.FirstOrDefault(WebsiteResources => WebsiteResources.WebsiteFieldId == websiteField.Id).Text;
                    if (websiteField.Type.ToLower() == "textarea")
                    {
                        text = text.Replace("\r\n", "\n").Replace("\n", "<br />");
                    }

                    fields.Add(websiteField.CallName, text);
                }
            }

            return new Dictionary<string, object>() {
                { "rootPageUrl", new Page(_context, _config).GetRootPage(websiteBundle.WebsiteLanguage.Id)?.Url },
                { "files", uploads },
                { "resources", fields }
            };
        }

        public List<Dictionary<string, object>> ConvertWebsiteLanguagesToJson(WebsiteBundle websiteBundle, int websiteLanguageId, string alternateGuid)
        {
            List<Dictionary<string, object>> languages = new List<Dictionary<string, object>>();
            foreach (WebsiteLanguages websiteLanguage in websiteBundle.WebsiteLanguages)
            {
                Languages language = websiteBundle.Languages.FirstOrDefault(Languages => Languages.Id == websiteLanguage.LanguageId);
                Languages _currentLanguage = websiteBundle.WebsiteLanguages.Join(websiteBundle.Languages, WebsiteLanguage => WebsiteLanguage.LanguageId, Language => Language.Id, (WebsiteLanguage, Language) => new { WebsiteLanguage, Language }).FirstOrDefault(x => x.WebsiteLanguage.Id == websiteLanguageId).Language;

                Routing routing = new Routing(_context, _config);
                var alternateUrl = routing.GetPageRouteUrlByAlternateGuidAndWebsiteLanguageId(alternateGuid, websiteLanguage.Id);
                languages.Add(new Dictionary<string, object>()
                {
                    { "id", websiteLanguage.Id },
                    { "defaultLanguage", websiteLanguage.DefaultLanguage },
                    { "code", language.Code },
                    { "culture", language.Culture },
                    { "language", _context.LanguageTranslate.FirstOrDefault(x => x.LanguageId == websiteLanguage.LanguageId && x.Code == _currentLanguage.Code).Translate },
                    { "timeZoneId", language.TimeZoneId },
                    { "url", alternateUrl }
                });
            }

            return languages;
        }

        public ObjectResult GetWebsiteBundleJson(int websiteLanguageId)
        {
            WebsiteBundle _websiteBundle = GetWebsiteBundle(websiteLanguageId);
            if (_websiteBundle == null) return NotFound("");

            return Ok(ConvertWebsiteBundleToJson(_websiteBundle));
        }

        public ObjectResult GetWebsiteLanguagesJson(int websiteLanguageId, string alternateGuid)
        {
            return Ok(ConvertWebsiteLanguagesToJson(GetWebsiteLanguages(), websiteLanguageId, alternateGuid));
        }
    }
}