using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Site.Data;
using Site.Models.App;
using System.Collections.Generic;
using System.Linq;

namespace Site.Models
{
    public class Shipping
    {
        private readonly SiteContext _context;
        private readonly IOptions<AppSettings> _config;

        public Shipping(SiteContext context, IOptions<AppSettings> config = null)
        {
            _context = context;
            _config = config;
        }

        public class ShippingZoneMethodAndClasses
        {
            public IEnumerable<ShippingZoneMethodClasses> ShippingZoneMethodClasses { get; set; }
            public ShippingZoneMethods ShippingZoneMethod { get; set; }
        }

        public ShippingClasses GetShippingClassById(int id)
        {
            return _context.ShippingClasses.FirstOrDefault(ShippingClass => ShippingClass.Id == id);
        }

        public IQueryable<ShippingClasses> GetShippingClassesByWebsiteId(int websiteId)
        {
            return _context.ShippingClasses.OrderBy(ShippingClass => ShippingClass.Name).Where(ShippingClass => ShippingClass.WebsiteId == websiteId);
        }

        public List<ShippingZoneMethodAndClasses> GetShippingZoneMethodAndClassesByDefaultAndWebsiteId()
        {
            return _context.ShippingZoneMethods.GroupJoin(_context.ShippingZoneMethodClasses, ShippingZoneMethod => ShippingZoneMethod.Id, ShippingZoneMethodClass => ShippingZoneMethodClass.ShippingZoneMethodId, (ShippingZoneMethod, ShippingZoneMethodClasses) => new { ShippingZoneMethod, ShippingZoneMethodClasses })
                                               .Join(_context.ShippingZones, x => x.ShippingZoneMethod.ShippingZoneId, ShippingZone => ShippingZone.Id, (x, ShippingZone) => new { x.ShippingZoneMethod, x.ShippingZoneMethodClasses, ShippingZone })
                                               .Where(x => x.ShippingZone.WebsiteId == _config.Value.WebsiteId && x.ShippingZone.Default == true)
                                               .Select(x => new ShippingZoneMethodAndClasses() {
                                                   ShippingZoneMethod = x.ShippingZoneMethod,
                                                   ShippingZoneMethodClasses = x.ShippingZoneMethodClasses
                                               })
                                               .ToList();
        }
    }
}