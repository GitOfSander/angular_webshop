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
    public class Language : Controller
    {
        SiteContext _context;
        private readonly IOptions<AppSettings> _config;

        public Language(SiteContext context, IOptions<AppSettings> config)
        {
            _context = context;
            _config = config;
        }

        public class LanguagesBundle
        {
            public Languages Language { get; set; }
            public WebsiteLanguages WebsiteLanguage { get; set; }
        }

        public IQueryable<LanguagesBundle> GetActiveLanguages(int websiteId)
        {
            return _context.WebsiteLanguages.Join(_context.Languages, WebsiteLanguages => WebsiteLanguages.LanguageId, Languages => Languages.Id, (WebsiteLanguages, Languages) => new { WebsiteLanguages, Languages })
                                            .Select(x => new LanguagesBundle()
                                            {
                                                Language = x.Languages,
                                                WebsiteLanguage = x.WebsiteLanguages
                                            })
                                            .Where(x => x.WebsiteLanguage.Active);
        }
    }
}