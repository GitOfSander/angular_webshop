using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Site.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Site.Data;
using Site.Models;
using Site.Models.App;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Site.Controllers
{
    public class OrderControlller : Controller
    {
        private readonly SiteContext _context;
        private readonly IOptions<AppSettings> _config;
        private readonly IHostingEnvironment _env;
        private readonly IDataProtectionProvider _provider;
        private readonly IEmailSender _emailSender;

        public OrderControlller(SiteContext context, IOptions<AppSettings> config, IHostingEnvironment env, IDataProtectionProvider provider, IEmailSender emailSender)
        {
            _context = context;
            _config = config;
            _env = env;
            _provider = provider;
            _emailSender = emailSender;
        }

        public class ProductLine
        {
            public int ProductId { get; set; }
            public string ReserveGuid { get; set; }
            public int Quantity { get; set; }
            public bool Increment { get; set; }
            public int WebsiteLanguageId { get; set; }
        }

        [Route("/spine-api/order-bundle-by-reserve-guid")]
        [HttpGet]
        public async Task<IActionResult> GetOrderBundleByReserveGuidApiAsync(string reserveGuid, int websiteLanguageId, bool lockPrices)
        {
            try
            {
                Order order = new Order(_context, _config, null, _emailSender);
                order.LockPrices = lockPrices;
                return await order.GetOrderJsonAsync(reserveGuid, websiteLanguageId);
            }
            catch (Exception e) 
            {
                new Error().ReportError(_config.Value.SystemWebsiteId, "ORDERCONTROLLER#1", "kan orderbundel op basis van 'ReserveGuid' niet ophalen.", "", e.Message);
                return StatusCode(400);
            }
        }

        [Route("/spine-api/order-billing-email-and-status-by-transaction-id")]
        [HttpGet]
        public IActionResult GetOrderBillingEmailAndStatusByTransactionId(string transactionId)
        {
            if (!string.IsNullOrWhiteSpace(transactionId))
            {
                try
                {
                    Orders _order = new Order(_context, _config, _provider).GetOrderByTransactionId(transactionId);
                    Encryptor encryptor = new Encryptor(_provider);
                    return Ok(new Dictionary<string, object>() {
                    { "billingEmail", encryptor.Decrypt(_order.BillingEmail) },
                    { "status", _order.Status}
                });
                }
                catch (Exception e)
                {
                    new Error().ReportError(_config.Value.SystemWebsiteId, "ORDERCONTROLLER#2", transactionId + " Kan order 'BillingEmail' en 'Status' op basis van 'TransactionId' niet ophalen.", "", e.Message);

                    return StatusCode(400);
                }
            }

            return StatusCode(400); //TEST080533680534
        }

        [Route("/spine-api/order-line")]
        [HttpPost]
        public async Task<IActionResult> UpdateOrderLineApiAsync([FromBody] ProductLine productLine)
        {
            try
            {
                return await new Order(_context, _config, _provider).UpdateOrderLineAndGetOrderJsonAsync(productLine.ProductId, productLine.ReserveGuid, productLine.Quantity, productLine.Increment, productLine.WebsiteLanguageId);
            }
            catch (Exception e)
            {
                new Error().ReportError(_config.Value.SystemWebsiteId, "ORDERCONTROLLER#3", "Kan orderregel niet updaten.", "", e.Message);

                return StatusCode(400);
            }
        }

        [Route("/spine-api/delete-order-line")]
        [HttpPost]
        public async Task<IActionResult> DeleteOrderLineApiAsync([FromBody] ProductLine productLine)
        {
            try
            {
                return await new Order(_context, _config).DeleteOrderLineAndGetOrderJsonAsync(productLine.ProductId, productLine.ReserveGuid, productLine.WebsiteLanguageId);
            }
            catch (Exception e)
            {
                new Error().ReportError(_config.Value.SystemWebsiteId, "ORDERCONTROLLER#4", "Kan orderregel niet verwijderen.", "", e.Message);

                return StatusCode(400);
            }
        }
    }
}
