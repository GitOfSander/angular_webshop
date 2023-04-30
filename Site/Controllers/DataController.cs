using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Site.Data;
using Site.Models;
using Site.Models.App;
using System;

namespace Site.Controllers
{
    public class DataController : Controller
    {
        SiteContext _context;
        private readonly IOptions<AppSettings> _config;
        private IHostingEnvironment _env;

        public DataController(SiteContext context, IOptions<AppSettings> config, IHostingEnvironment env)
        {
            _context = context;
            _config = config;
            _env = env;
        }

        [Route("/spine-api/data-bundle-with-childs")]
        [HttpGet]
        public IActionResult GetDataBundlesWithCategorieByUrlApi(int websiteLanguageId, string callName, string url, string fieldCallName, string type, bool breadcrumbs)
        {
            try
            {
                Models.Data data = new Models.Data(_context, _config);
                data.AddBreadcrumbsJson = true;

                return data.GetDataBundleWithChilds(websiteLanguageId, callName, url, fieldCallName, type);
            }
            catch (Exception e)
            {
                new Error().ReportError(_config.Value.SystemWebsiteId, "DATACONTROLLER#01", "Kan databundels en gelinkte databundels niet ophalen.", "", e.Message);

                return StatusCode(400);
            }
        }

        [Route("/spine-api/data-bundles")]
        [HttpGet]
        public IActionResult GetDataBundlesApi(int websiteLanguageId, string callName)
        {
            try
            {
                return new Models.Data(_context, _config).GetDataBundlesJson(websiteLanguageId, callName);
            }
            catch (Exception e)
            {
                new Error().ReportError(_config.Value.SystemWebsiteId, "DATACONTROLLER#02", "Kan databundels niet ophalen.", "", e.Message);

                return StatusCode(400);
            }
        }

        [Route("/spine-api/data-bundle")]
        [HttpGet]
        public IActionResult GetDataBundleApi(int websiteLanguageId, string callName, string url)
        {
            try
            {
                return new Models.Data(_context, _config).GetDataBundleJson(websiteLanguageId, callName, url);
            }
            catch (Exception e)
            {
                new Error().ReportError(_config.Value.SystemWebsiteId, "DATACONTROLLER#03", "Kan databundel niet ophalen.", "", e.Message);

                return StatusCode(400);
            }
        }

        [Route("/spine-api/data-bundle-with-categories")]
        [HttpGet]
        public IActionResult GetDataBundleWithCategoriesApi(int websiteLanguageId, string callName, string url)
        {
            try
            {
                return new Models.Data(_context, _config).GetDataBundleWithCategoriesJson(websiteLanguageId, callName, url);
            }
            catch (Exception e)
            {
                new Error().ReportError(_config.Value.SystemWebsiteId, "DATACONTROLLER#04", "Kan databundle en categorieën niet ophalen.", "", e.Message);

                return StatusCode(400);
            }
        }

        [Route("/spine-api/data-bundles-with-categories")]
        [HttpGet]
        public IActionResult GetDataBundlesWithCategorieApi(int websiteLanguageId, string callName)
        {
            try
            {
                return new Models.Data(_context, _config).GetDataBundlesWithCategorieJson(websiteLanguageId, callName);
            }
            catch (Exception e)
            {
                new Error().ReportError(_config.Value.SystemWebsiteId, "DATACONTROLLER#05", "Kan databundels met categorieën niet ophalen.", "", e.Message);

                return StatusCode(400);
            }
        }

        [Route("/spine-api/max-data-bundles")]
        [HttpGet]
        public IActionResult GetMaxDataBundlesApi(int websiteLanguageId, string callName, int max)
        {
            try
            {
                return new Models.Data(_context, _config).GetMaxDataBundlesJson(websiteLanguageId, callName, max);
            }
            catch (Exception e)
            {
                new Error().ReportError(_config.Value.SystemWebsiteId, "DATACONTROLLER#06", "Kan max aantal databundels niet ophalen.", "", e.Message);

                return StatusCode(400);
            }
        }

        [Route("/spine-api/max-data-bundles-order-by-publish-date")]
        [HttpGet]
        public IActionResult GetMaxDataBundlesOrderByPublishDateApi(int websiteLanguageId, string callName, int max)
        {
            try
            {
                return new Models.Data(_context, _config).GetMaxDataBundlesOrderByPublishDateJson(websiteLanguageId, callName, max);
            }
            catch (Exception e)
            {
                new Error().ReportError(_config.Value.SystemWebsiteId, "DATACONTROLLER#07", "Kan max aantal databundles geordend op basis van 'PublishData' niet ophalen.", "", e.Message);

                return StatusCode(400);
            }
        }
    }
}
