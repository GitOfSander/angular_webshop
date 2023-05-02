using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Site.Models;
using Site.Data;
using Site.Models.App;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Hosting;

namespace Site.Controllers
{
    public class RoutingController : Controller
    {
        private readonly SiteContext _context;
        private readonly IOptions<AppSettings> _config;
        private readonly IHostingEnvironment _env;

        public RoutingController(SiteContext context, IOptions<AppSettings> config, IHostingEnvironment env)
        {
            _context = context;
            _config = config;
            _env = env;
        }

        [Route("/spine-api/routes")]
        [HttpGet]
        public IActionResult GetRoutesApi()
        {
            try
            {
                return new Routing(_context, _config).GetRoutes();
            }
            catch (Exception e)
            {
                new Error().ReportError(_config.Value.SystemWebsiteId, "ROUTINGCONTROLLER#01", "Kan routes niet ophalen.", "", e.Message);

                return StatusCode(400);
            }
        }

        [Route("/spine-api/route-by-alternate-guid-and-website-language-id")]
        [HttpGet]
        public IActionResult GetPageRouteUrlByAlternateGuidAndWebsiteLanguageIdApi(string alternateGuid, int websiteLanguageId)
        {
            try
            {
                return new Routing(_context, _config).GetRoutesByAlternateGuidAndWebsiteLanguageId(alternateGuid, websiteLanguageId);
            }
            catch (Exception e)
            {
                new Error().ReportError(_config.Value.SystemWebsiteId, "ROUTINGCONTROLLER#02", "Kan route op basis van 'AlternateGuid' en 'WebsiteLanguageId' niet ophalen.", "", e.Message);

                return StatusCode(400);
            }
        }
    }
}
