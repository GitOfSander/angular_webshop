using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Site.Data;
using Site.Models;
using Site.Models.App;
using Site.Models.Sisow;
using Site.Services;
using System;
using System.Collections.Generic;
using static Site.Models.Commerce;

namespace Site.Controllers
{
    public class CommerceController : Controller
    {
        private readonly SiteContext _context;
        private readonly IOptions<AppSettings> _config;
        private readonly IHostingEnvironment _env;
        private readonly IDataProtectionProvider _provider;
        private readonly IEmailSender _emailSender;

        public CommerceController(SiteContext context, IOptions<AppSettings> config, IHostingEnvironment env, IDataProtectionProvider provider, IEmailSender emailSender)
        {
            _context = context;
            _config = config;
            _env = env;
            _provider = provider;
            _emailSender = emailSender;
        }

        [Route("/spine-api/payment")]
        [HttpPost]
        public async System.Threading.Tasks.Task<IActionResult> SetSisowPaymentAsync(PaymentInfo paymentInfo)
        {
            try
            {
                Commerce commerce = new Commerce(_context, _config, null, _emailSender, null, _provider);
                commerce.PI = paymentInfo;
                return await commerce.ValidateAndCreatePaymentAsync();
            }
            catch (Exception e)
            {
                new Error().ReportError(_config.Value.SystemWebsiteId, "COMMERCECONTROLLER#01", "Er ging iets mis met de bestelling gereed maken voor Sisow.", "", e.Message);

                return StatusCode(400, Json(new
                {
                    result = "Er is iets fout gegaan!",
                }));
            }
        }

        [Route("/spine-api/issuers")]
        [HttpGet]
        public IActionResult GetIssuersApi()
        {
            try
            {
                string[] iss;
                SisowClient sisowClient = new SisowClient(_config.Value.OAuth.Sisow.ClientId, _config.Value.OAuth.Sisow.ClientSecret);

                List<Dictionary<string, string>> issuers = new List<Dictionary<string, string>>();
                //Get available iDEAL banks
                if (sisowClient.DirectoryRequest(false, out iss) == 0)
                {
                    for (int i = 0; i < iss.Length; i += 2)
                    {
                        issuers.Add(new Dictionary<string, string>() { { "key", iss[i + 1] }, { "value", iss[i] } });
                    }
                }

                return Ok(issuers);
            }
            catch (Exception e)
            {
                new Error().ReportError(_config.Value.SystemWebsiteId, "COMMERCECONTROLLER#02", "Issuers konden niet worden opgehaald bij Sisow.", "", e.Message);

                return StatusCode(400);
            }
        }

    }
}
