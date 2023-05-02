using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;
using Site.Data;
using Site.Models;
using Site.Models.App;
using Site.Services;
using System;
using System.Threading.Tasks;

namespace Site.Controllers
{
    public class SisowController : Controller
    {
        SiteContext _context;
        private readonly IOptions<AppSettings> _config;
        private IHostingEnvironment _env;
        private readonly IEmailSender _emailSender;
        private readonly IActionContextAccessor _actionContextAccessor;
        private readonly IDataProtectionProvider _provider;

        public SisowController(SiteContext context, IOptions<AppSettings> config, IHostingEnvironment env, IEmailSender emailSender, IActionContextAccessor actionContextAccessor, IDataProtectionProvider provider)
        {
            _context = context;
            _config = config;
            _env = env;
            _emailSender = emailSender;
            _actionContextAccessor = actionContextAccessor;
            _provider = provider;
        }

        [Route("/spine-api/sisow-return")]
        [HttpGet]
        public IActionResult SisowReturnApi(string trxid, string ec, string status, string sha1)
        {
            try
            {
                string alternateGuid = new Setting(_context).GetSettingValueByKey("confirmation", "website", _config.Value.WebsiteId);
                Page page = new Page(_context, _config);
                page.SetPageBundleAndWebsiteBundle(new Website(_context, _config).GetWebsiteLanguage().Id);
                return Redirect(page.GetPageUrlByAlternateGuid(alternateGuid) + "?id=" + trxid);
            }
            catch (Exception e)
            {
                new Error().ReportError(_config.Value.SystemWebsiteId, "SisowController#1", "Er ging wat mis bij het verwerken op de return pagina.", "", e.Message);

                return StatusCode(400);
            }
        }

        [Route("/spine-api/sisow-notify")]
        [HttpPost]
        public async Task<IActionResult> SisowNotifyApiAsync(string trxid, string ec, string status, string sha1, string notify)
        {
            try
            {
                return await new Commerce(_context, _config, _env, _emailSender, _actionContextAccessor, _provider).ProcessPaymentAsync(trxid);
            }
            catch (Exception e)
            {
                new Error().ReportError(_config.Value.SystemWebsiteId, "SisowController#2", "Er ging wat mis bij het verwerken van de melding van Sisow.", "", e.Message);

                return StatusCode(400);
            }
        }

        [Route("/spine-api/sisow-notify")]
        [HttpGet]
        public async Task<IActionResult> GetSisowNotifyApiAsync(string trxid, string ec, string status, string sha1, string notify)
        {
            try
            {
                return await new Commerce(_context, _config, _env, _emailSender, _actionContextAccessor, _provider).ProcessPaymentAsync(trxid);
            }
            catch (Exception e)
            {
                new Error().ReportError(_config.Value.SystemWebsiteId, "SisowController#3", "Er ging wat mis bij het verwerken van de melding van Sisow.", "", e.Message);

                return StatusCode(400);
            }
        }
    }
}
