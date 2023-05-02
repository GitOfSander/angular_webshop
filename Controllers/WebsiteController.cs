using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Site.Data;
using Site.Models;
using Site.Models.App;
using System;

namespace Site.Controllers
{
    public class WebsiteController : Controller
    {
        SiteContext _context;
        private readonly IOptions<AppSettings> _config;
        private IHostingEnvironment _env;

        public WebsiteController(SiteContext context, IOptions<AppSettings> config, IHostingEnvironment env)
        {
            _context = context;
            _config = config;
            _env = env;
        }

        [Route("/api/website-bundle")]
        [HttpGet]
        public IActionResult GetWebsiteBundleApi(int websiteLanguageId)
        {
            try
            {
                return new Website(_context, _config).GetWebsiteBundleJson(websiteLanguageId);
            }
            catch (Exception e)
            {
                new Error().ReportError(_config.Value.SystemWebsiteId, "WEBSITECONTROLLER#01", "Kan website bundle niet ophalen.", "", e.Message);

                return StatusCode(400);
            }
        }

        [Route("/api/website-language-by-default-language")]
        [HttpGet]
        public IActionResult GetWebsiteLanguageByDefaultLanguageApi(bool defaultLanguage)
        {
            try
            {
                return new Website(_context, _config).GetWebsiteLanguageByDefaultLanguage(defaultLanguage);
            }
            catch (Exception e)
            {
                new Error().ReportError(_config.Value.SystemWebsiteId, "WEBSITECONTROLLER#02", "Kan standaard website taal niet ophalen.", "", e.Message);

                return StatusCode(400);
            }
        }

        [Route("/api/website-languages")]
        [HttpGet]
        public IActionResult GetWebsiteLanguagesApi(int websiteLanguageId, string alternateGuid)
        {
            try
            {
                return new Website(_context, _config).GetWebsiteLanguagesJson(websiteLanguageId, alternateGuid);
            }
            catch (Exception e)
            {
                new Error().ReportError(_config.Value.SystemWebsiteId, "WEBSITECONTROLLER#03", "Kan website talen niet ophalen.", "", e.Message);

                return StatusCode(400);
            }
        }
    }
}
