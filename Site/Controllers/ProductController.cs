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
    public class ProductController : Controller
    {
        SiteContext _context;
        private readonly IOptions<AppSettings> _config;
        private IHostingEnvironment _env;

        public ProductController(SiteContext context, IOptions<AppSettings> config, IHostingEnvironment env)
        {
            _context = context;
            _config = config;
            _env = env;
        }

        [Route("/spine-api/product-bundle")]
        [HttpGet]
        public IActionResult GetProductBundleApi(int websiteLanguageId, string alternateGuid, string url)
        {
            try
            {
                Product product = new Product(_context, _config);
                product.AddBreadcrumbsJson = true;

                return product.GetProductBundleJson(websiteLanguageId, alternateGuid, url, true);
            }
            catch (Exception e)
            {
                new Error().ReportError(_config.Value.SystemWebsiteId, "PRODUCTCONTROLLER#1", "Kan productbundel niet ophalen.", "", e.Message);

                return StatusCode(400);
            }
        }

        [Route("/spine-api/product-bundles")]
        [HttpGet]
        public IActionResult GetProductBundlesApi(int websiteLanguageId, string alternateGuid)
        {
            try
            {
                return new Product(_context, _config).GetProductBundlesJson(websiteLanguageId, alternateGuid, true);
            }
            catch (Exception e)
            {
                new Error().ReportError(_config.Value.SystemWebsiteId, "PRODUCTCONTROLLER#2", "Kan productbundels niet ophalen.", "", e.Message);

                return StatusCode(400);
            }
        }
    }
}
