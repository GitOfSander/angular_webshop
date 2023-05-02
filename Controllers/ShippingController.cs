using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Site.Data;
using Site.Models;
using Site.Models.App;
using System;

namespace Site.Controllers
{
    public class ShippingController : Controller
    {
        SiteContext _context;
        private readonly IOptions<AppSettings> _config;
        private IHostingEnvironment _env;

        public ShippingController(SiteContext context, IOptions<AppSettings> config, IHostingEnvironment env)
        {
            _context = context;
            _config = config;
            _env = env;
        }

        public class ShippingMethod
        {
            public int ShippingMethodId { get; set; }
            public string ReserveGuid { get; set; }
        }

        [Route("/spine-api/shipping-method")]
        [HttpPost]
        public IActionResult UpdateShippingMethodApi([FromBody] ShippingMethod shippingMethod)
        {
            try
            {
                return new Order(_context, _config).insertOrUpdateOrderShippingZoneMethod(shippingMethod.ShippingMethodId , shippingMethod.ReserveGuid);
            }
            catch (Exception e)
            {
                new Error().ReportError(_config.Value.SystemWebsiteId, "SHIPPINGCONTROLLER#01", "Kan verzendmethode niet updaten.", "", e.Message);

                return StatusCode(400);
            }
        }
    }
}
