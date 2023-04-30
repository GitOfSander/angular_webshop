using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Site.Data;
using Site.Models.App;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using static Site.Models.Product;
using static Site.Models.Shipping;

namespace Site.Models
{
    public class ApiKey : Controller
    {
        private readonly SiteContext _context;
        private readonly IOptions<AppSettings> _config;

        public ApiKey(SiteContext context, IOptions<AppSettings> config = null)
        {
            _context = context;
            _config = config;
        }

        public void UpdateApiKey(ApiKeys apiKey)
        {
            _context.ApiKeys.Update(apiKey);
            _context.SaveChanges();
        }
    }
}