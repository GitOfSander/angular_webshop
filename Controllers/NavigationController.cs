using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Site.Data;
using Site.Models;
using Site.Models.App;

namespace Site.Controllers
{
    public class NavigationController : Controller
    {
        SiteContext _context;
        private readonly IOptions<AppSettings> _config;
        private IHostingEnvironment _env;

        public NavigationController(SiteContext context, IOptions<AppSettings> config, IHostingEnvironment env)
        {
            _context = context;
            _config = config;
            _env = env;
        }

        [Route("/spine-api/navigation")]
        [HttpGet]
        public IActionResult GetNavigationLinksApi(int websiteLanguageId, string callName)
        {
            try
            {
                return new Navigation(_context, _config).GetNavigationLinksJson(websiteLanguageId, callName);
            }
            catch (Exception e)
            {
                new Error().ReportError(_config.Value.SystemWebsiteId, "NAVIGATIONCONTROLLER#01", "Kan navigatie niet ophalen.", "", e.Message);

                return StatusCode(400);
            }
        }

        [Route("/spine-api/navigations")]
        [HttpGet]
        public IActionResult GetNavigationLinksApi(int websiteLanguageId, string[] callNames)
        {
            try
            {
                return new Navigation(_context, _config).GetNavigationsAndConvertToJson(websiteLanguageId, callNames);
            }
            catch (Exception e)
            {
                new Error().ReportError(_config.Value.SystemWebsiteId, "NAVIGATIONCONTROLLER#02", "Kan navigaties niet ophalen.", "", e.Message);

                return StatusCode(400);
            }
        }
    }
}
