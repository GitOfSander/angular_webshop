using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Site.Data;
using System.Collections.Generic;
using System.Linq;

namespace Site.Models
{
    public class Tax
    {
        private readonly SiteContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private ISession _session => _httpContextAccessor.HttpContext.Session;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public Tax(SiteContext context, UserManager<ApplicationUser> userManager = null, IHttpContextAccessor httpContextAccessor = null, SignInManager<ApplicationUser> signInManager = null)
        {
            _context = context;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _signInManager = signInManager;
        }

        public TaxClasses GetTaxClassById(int id)
        {
            return _context.TaxClasses.FirstOrDefault(TaxClass => TaxClass.Id == id);
        }

        public IEnumerable<TaxClasses> GetTaxClassesByWebsiteId(int websiteId)
        {
            return _context.TaxClasses.OrderBy(TaxClass => TaxClass.Name)
                                      .Where(TaxClass => TaxClass.WebsiteId == websiteId);

        }
    }
}
