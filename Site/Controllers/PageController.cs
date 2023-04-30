using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Site.Data;
using Site.Models;
using Site.Models.App;
using System;

namespace Site.Controllers
{
    public class PageController : Controller
    {
        SiteContext _context;
        private readonly IOptions<AppSettings> _config;
        private IHostingEnvironment _env;

        public PageController(SiteContext context, IOptions<AppSettings> config, IHostingEnvironment env)
        {
            _context = context;
            _config = config;
            _env = env;
        }

        [Route("/spine-api/page-bundle")]
        [HttpGet]
        public IActionResult GetPageBundleApi(int pageId, int websiteLanguageId, bool breadcrumbs, bool addProductJson)
        {
            try
            {
                Models.Page page = new Models.Page(_context, _config);
                page.AddBreadcrumbsJson = breadcrumbs;
                page.AddProductJson = addProductJson;

                return page.GetPageBundleJson(pageId, websiteLanguageId);
            }
            catch (Exception e)
            {
                new Error().ReportError(_config.Value.SystemWebsiteId, "PAGECONTROLLER#1", "Kan paginabundel niet ophalen.", "", e.Message);

                return StatusCode(400);
            }
        }

        [Route("/spine-api/page-bundles-by-type")]
        [HttpGet]
        public IActionResult GetPageBundlesByTypeApi(int websiteLanguageId, string type, bool addProductJson)
        {
            try
            {
                Models.Page page = new Models.Page(_context, _config);
                page.AddProductJson = addProductJson;

                return page.GetPageBundlesByTypeJson(websiteLanguageId, type, true);
            }
            catch (Exception e)
            {
                new Error().ReportError(_config.Value.SystemWebsiteId, "PAGECONTROLLER#2", "Kan paginabundles op basis van 'Type' niet ophalen.", "", e.Message);

                return StatusCode(400);
            }
        }

        [Route("/spine-api/page-urls-by-alternate-guids-and-setting-values")]
        [HttpGet]
        public IActionResult GetPageUrlsByAlternateGuidApi(int websiteLanguageId, string[] alternateGuids, string[] settingValues)
        {
            try
            {
                return new Models.Page(_context, _config).GetPageUrlsJson(websiteLanguageId, alternateGuids, settingValues);
            }
            catch (Exception e)
            {
                new Error().ReportError(_config.Value.SystemWebsiteId, "PAGECONTROLLER#3", "Kan pagina url's op basis van 'AlternateGuids' en 'SettingValues' niet ophalen.", "", e.Message);

                return StatusCode(400);
            }
        }
    }
}
