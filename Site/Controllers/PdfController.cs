using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Site.Data;
using Site.Models;
using System.IO;
using Rotativa.AspNetCore;
using Rotativa.AspNetCore.Options;
using Microsoft.AspNetCore.Authorization;
using Site.Models.ViewModels;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Options;
using Site.Models.App;
using static Site.Models.Order;
using System.Globalization;
using Microsoft.AspNetCore.Routing;

namespace Site.Controllers
{
    public class PdfController : Controller
    {
        private readonly SiteContext _context;
        private readonly IOptions<AppSettings> _config;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHostingEnvironment _env;
        private readonly IActionContextAccessor _actionContextAccessor;
        private readonly IDataProtectionProvider _provider;

        public PdfController(SiteContext context, IOptions<AppSettings> config, UserManager<ApplicationUser> userManager = null, IHostingEnvironment env = null, IActionContextAccessor actionContextAccessor = null, IDataProtectionProvider provider = null)
        {
            _context = context;
            _config = config;
            _userManager = userManager;
            _env = env;
            _actionContextAccessor = actionContextAccessor;
            _provider = provider;
        }

        [HttpGet]
        public ViewAsPdf Index(int id, bool test = false)
        {
            string filePath = "";
            PdfViewModel model;
            Orders _order = new Orders();

            if (!test)
            {
                int orderId = id;

                Setting setting = new Setting(_context);

                Order order = new Order(_context, _config);
                OrderBundle orderBundle = order.GetOrderBundle(orderId);
                order.Commerce = new Commerce(_context, _config);
                order.Commerce.SetPriceFormatVariables();
                order.DigitsAfterDecimal = Int32.Parse(setting.GetSettingValueByKey("digitsAfterDecimal", "website", _config.Value.WebsiteId));
                order.Currency = setting.GetSettingValueByKey("currency", "website", _config.Value.WebsiteId);
                order.DistributePricesToTaxes(orderBundle.OrderLines);
                decimal shippingCosts = order.GetTotalShippingCosts(orderBundle.OrderLines, orderBundle.OrderShippingZoneMethods);
                decimal productsTotal = order.GetProductsTotal(orderBundle.OrderLines);
                string total = decimal.Round((productsTotal + shippingCosts), order.DigitsAfterDecimal, MidpointRounding.AwayFromZero).ToString(CultureInfo.GetCultureInfo("nl-NL").NumberFormat);

                Encryptor encryptor = new Encryptor(_provider);
                _order = orderBundle.Order;
                model = new PdfViewModel
                {
                    Website = new Website(_context, _config).GetCleanWebsiteUrlByWebsiteId(_config.Value.WebsiteId) + "/",
                    AddressLine1 = setting.GetSettingValueByKey("addressLine1", "website", _config.Value.WebsiteId),
                    ZipCode = setting.GetSettingValueByKey("zipCode", "website", _config.Value.WebsiteId),
                    Vat = setting.GetSettingValueByKey("vat", "website", _config.Value.WebsiteId),
                    Coc = setting.GetSettingValueByKey("coc", "website", _config.Value.WebsiteId),
                    Country = setting.GetSettingValueByKey("country", "website", _config.Value.WebsiteId),
                    City = setting.GetSettingValueByKey("city", "website", _config.Value.WebsiteId),
                    Date = _order.CreatedDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),
                    OrderNumber = _order.OrderNumber,
                    InvoiceNumber = _order.InvoiceNumber,
                    OrderLines = orderBundle.OrderLines,
                    ShippingAddressLine1 = (!string.IsNullOrWhiteSpace(encryptor.Decrypt(_order.ShippingAddressLine1)) ? encryptor.Decrypt(_order.ShippingAddressLine1) : encryptor.Decrypt(_order.BillingAddressLine1)),
                    ShippingCity = (!string.IsNullOrWhiteSpace(encryptor.Decrypt(_order.ShippingAddressLine1)) ? encryptor.Decrypt(_order.ShippingCity) : encryptor.Decrypt(_order.BillingCity)),
                    ShippingCompany = (!string.IsNullOrWhiteSpace(encryptor.Decrypt(_order.ShippingAddressLine1)) ? encryptor.Decrypt(_order.ShippingCompany) : encryptor.Decrypt(_order.BillingCompany)),
                    ShippingCountry = "Nederland",//(!string.IsNullOrWhiteSpace(_order.ShippingAddressLine1) ? _order.ShippingCountry : _order.BillingCountry),
                    ShippingName = (!string.IsNullOrWhiteSpace(encryptor.Decrypt(_order.ShippingAddressLine1)) ? encryptor.Decrypt(_order.ShippingFirstName) + " " + encryptor.Decrypt(_order.ShippingLastName) : encryptor.Decrypt(_order.BillingFirstName) + " " + encryptor.Decrypt(_order.BillingLastName)),
                    ShippingZipCode = (!string.IsNullOrWhiteSpace(encryptor.Decrypt(_order.ShippingAddressLine1)) ? encryptor.Decrypt(_order.ShippingZipCode) : encryptor.Decrypt(_order.BillingZipCode)),
                    BillingAddressLine1 = encryptor.Decrypt(_order.BillingAddressLine1),
                    BillingCity = encryptor.Decrypt(_order.BillingCity),
                    BillingCompany = encryptor.Decrypt(_order.BillingCompany),
                    BillingCountry = "Nederland",
                    BillingZipCode = encryptor.Decrypt(_order.BillingZipCode),
                    BillingEmail = encryptor.Decrypt(_order.BillingEmail),
                    BillingName = encryptor.Decrypt(_order.BillingFirstName) + " " + encryptor.Decrypt(_order.BillingLastName),
                    BillingPhoneNumber = encryptor.Decrypt(_order.BillingPhoneNumber),
                    BillingVatNumber = encryptor.Decrypt(_order.BillingVatNumber),
                    Status = "Betaald",
                    Note = _order.Note,
                    TaxClasses = order.TaxClasses,
                    ShippingCosts = shippingCosts,
                    Subtotal = order.Commerce.GetPriceFormat(decimal.Round((productsTotal), order.DigitsAfterDecimal, MidpointRounding.AwayFromZero).ToString(CultureInfo.GetCultureInfo("nl-NL").NumberFormat), order.Currency),
                    Tax = order.Commerce.GetPriceFormat(decimal.Round(order.GetTotalTax(), order.DigitsAfterDecimal, MidpointRounding.AwayFromZero).ToString(CultureInfo.GetCultureInfo("nl-NL").NumberFormat), order.Currency),
                    TotalExclusive = order.Commerce.GetPriceFormat(decimal.Round((shippingCosts + productsTotal) - order.GetTotalTax(), order.DigitsAfterDecimal, MidpointRounding.AwayFromZero).ToString(CultureInfo.GetCultureInfo("nl-NL").NumberFormat), order.Currency),
                    Total = order.Commerce.GetPriceFormat(decimal.Round((shippingCosts + productsTotal), order.DigitsAfterDecimal, MidpointRounding.AwayFromZero).ToString(CultureInfo.GetCultureInfo("nl-NL").NumberFormat), order.Currency),
                    PriceFormat = order.Commerce.GetPriceFormat("price", order.Currency),
                    DigitsFormat = "0.".PadRight(order.DigitsAfterDecimal + "0.".Length, '0'),
                    DigitsAfterDecimal = order.DigitsAfterDecimal
                };

                filePath = Path.GetFullPath(Path.Combine(_env.ContentRootPath + $@"\Files\Pdf", "Invoice_" + _order.Id + ".pdf"));
            }
            else
            {
                filePath = Path.GetFullPath(Path.Combine(_env.ContentRootPath + $@"\Files\Pdf", "Invoice_Test.pdf"));

                model = new PdfViewModel
                {

                };
            }

            //Create directory is it does not exist
            if (!Directory.Exists(_env.ContentRootPath + $@"\Files\Pdf"))
            {
                Directory.CreateDirectory(_env.ContentRootPath + $@"\Files\Pdf");
            }

            RouteValueDictionary routeValueDictionary = new RouteValueDictionary()
            {
                { "id", id },
                { "test", test }
            };

            string cusomtSwitches = string.Format("--print-media-type --header-spacing 2 --header-html {1} --footer-html {0}", Url.Action("Footer", "Pdf", routeValueDictionary, "https"), Url.Action("Master", "Pdf", routeValueDictionary, "https"));

            return new ViewAsPdf("Invoice", model)
            {
                FileName = test ? "Invoice_Test.pdf" : "Invoice_" + _order.Id + ".pdf",
                CustomSwitches = cusomtSwitches,
                PageSize = Size.A4,
                PageOrientation = Orientation.Portrait,
                PageMargins = { Top = 109, Right = 0, Bottom = 30, Left = 0 },
                SaveOnServerPath = filePath
            };
        }

        [HttpGet]
        [Route("spine-api/create-master")]
        public ActionResult Master(int id, bool test = false)
        {
            PdfViewModel model;

            if (!test)
            {
                int orderId = id;

                Setting setting = new Setting(_context);

                Order order = new Order(_context, _config);
                OrderBundle orderBundle = order.GetOrderBundle(orderId);
                order.Commerce = new Commerce(_context, _config);
                order.Commerce.SetPriceFormatVariables();
                order.DigitsAfterDecimal = Int32.Parse(setting.GetSettingValueByKey("digitsAfterDecimal", "website", _config.Value.WebsiteId));
                order.Currency = setting.GetSettingValueByKey("currency", "website", _config.Value.WebsiteId);
                order.DistributePricesToTaxes(orderBundle.OrderLines);
                decimal shippingCosts = order.GetTotalShippingCosts(orderBundle.OrderLines, orderBundle.OrderShippingZoneMethods);
                decimal productsTotal = order.GetProductsTotal(orderBundle.OrderLines);
                string total = decimal.Round((productsTotal + shippingCosts), order.DigitsAfterDecimal, MidpointRounding.AwayFromZero).ToString(CultureInfo.GetCultureInfo("nl-NL").NumberFormat);

                Encryptor encryptor = new Encryptor(_provider);
                Orders _order = orderBundle.Order;
                model = new PdfViewModel
                {
                    Website = new Website(_context, _config).GetCleanWebsiteUrlByWebsiteId(_config.Value.WebsiteId) + "/",
                    AddressLine1 = setting.GetSettingValueByKey("addressLine1", "website", _config.Value.WebsiteId),
                    ZipCode = setting.GetSettingValueByKey("zipCode", "website", _config.Value.WebsiteId),
                    Vat = setting.GetSettingValueByKey("vat", "website", _config.Value.WebsiteId),
                    Coc = setting.GetSettingValueByKey("coc", "website", _config.Value.WebsiteId),
                    Country = setting.GetSettingValueByKey("country", "website", _config.Value.WebsiteId),
                    City = setting.GetSettingValueByKey("city", "website", _config.Value.WebsiteId),
                    Date = _order.CreatedDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),
                    OrderNumber = _order.OrderNumber,
                    InvoiceNumber = _order.InvoiceNumber,
                    OrderLines = orderBundle.OrderLines,
                    ShippingAddressLine1 = (!string.IsNullOrWhiteSpace(encryptor.Decrypt(_order.ShippingAddressLine1)) ? encryptor.Decrypt(_order.ShippingAddressLine1) : encryptor.Decrypt(_order.BillingAddressLine1)),
                    ShippingCity = (!string.IsNullOrWhiteSpace(encryptor.Decrypt(_order.ShippingAddressLine1)) ? encryptor.Decrypt(_order.ShippingCity) : encryptor.Decrypt(_order.BillingCity)),
                    ShippingCompany = (!string.IsNullOrWhiteSpace(encryptor.Decrypt(_order.ShippingAddressLine1)) ? encryptor.Decrypt(_order.ShippingCompany) : encryptor.Decrypt(_order.BillingCompany)),
                    ShippingCountry = "Nederland",//(!string.IsNullOrWhiteSpace(_order.ShippingAddressLine1) ? _order.ShippingCountry : _order.BillingCountry),
                    ShippingName = (!string.IsNullOrWhiteSpace(encryptor.Decrypt(_order.ShippingAddressLine1)) ? encryptor.Decrypt(_order.ShippingFirstName) + " " + encryptor.Decrypt(_order.ShippingLastName) : encryptor.Decrypt(_order.BillingFirstName) + " " + encryptor.Decrypt(_order.BillingLastName)),
                    ShippingZipCode = (!string.IsNullOrWhiteSpace(encryptor.Decrypt(_order.ShippingAddressLine1)) ? encryptor.Decrypt(_order.ShippingZipCode) : encryptor.Decrypt(_order.BillingZipCode)),
                    BillingAddressLine1 = encryptor.Decrypt(_order.BillingAddressLine1),
                    BillingCity = encryptor.Decrypt(_order.BillingCity),
                    BillingCompany = encryptor.Decrypt(_order.BillingCompany),
                    BillingCountry = "Nederland",
                    BillingZipCode = encryptor.Decrypt(_order.BillingZipCode),
                    BillingEmail = encryptor.Decrypt(_order.BillingEmail),
                    BillingName = encryptor.Decrypt(_order.BillingFirstName) + " " + encryptor.Decrypt(_order.BillingLastName),
                    BillingPhoneNumber = encryptor.Decrypt(_order.BillingPhoneNumber),
                    BillingVatNumber = encryptor.Decrypt(_order.BillingVatNumber),
                    Status = "Betaald",
                    Note = _order.Note,
                    ShippingCosts = shippingCosts,
                    Subtotal = order.Commerce.GetPriceFormat(decimal.Round((productsTotal), order.DigitsAfterDecimal, MidpointRounding.AwayFromZero).ToString(CultureInfo.GetCultureInfo("nl-NL").NumberFormat), order.Currency),
                    Tax = order.Commerce.GetPriceFormat(decimal.Round(order.GetTotalTax(), order.DigitsAfterDecimal, MidpointRounding.AwayFromZero).ToString(CultureInfo.GetCultureInfo("nl-NL").NumberFormat), order.Currency),
                    TotalExclusive = order.Commerce.GetPriceFormat(decimal.Round((shippingCosts + productsTotal) - order.GetTotalTax(), order.DigitsAfterDecimal, MidpointRounding.AwayFromZero).ToString(CultureInfo.GetCultureInfo("nl-NL").NumberFormat), order.Currency),
                    Total = order.Commerce.GetPriceFormat(decimal.Round((shippingCosts + productsTotal), order.DigitsAfterDecimal, MidpointRounding.AwayFromZero).ToString(CultureInfo.GetCultureInfo("nl-NL").NumberFormat), order.Currency),
                    PriceFormat = order.Commerce.GetPriceFormat("price", order.Currency),
                    DigitsFormat = "0.".PadRight(order.DigitsAfterDecimal + "0.".Length, '0'),
                    DigitsAfterDecimal = order.DigitsAfterDecimal
                };
            }
            else
            {
                model = new PdfViewModel
                {

                };
            }

            return View(model);
        }

        [HttpGet]
        [Route("spine-api/create-footer")]
        public ActionResult Footer(int id, bool test = false)
        {
            PdfViewModel model;

            if (!test)
            {
                int orderId = id;

                Setting setting = new Setting(_context);

                Order order = new Order(_context, _config);
                OrderBundle orderBundle = order.GetOrderBundle(orderId);
                order.Commerce = new Commerce(_context, _config);
                order.Commerce.SetPriceFormatVariables();
                order.DigitsAfterDecimal = Int32.Parse(setting.GetSettingValueByKey("digitsAfterDecimal", "website", _config.Value.WebsiteId));
                order.Currency = setting.GetSettingValueByKey("currency", "website", _config.Value.WebsiteId);
                order.DistributePricesToTaxes(orderBundle.OrderLines);
                decimal shippingCosts = order.GetTotalShippingCosts(orderBundle.OrderLines, orderBundle.OrderShippingZoneMethods);
                decimal productsTotal = order.GetProductsTotal(orderBundle.OrderLines);
                string total = decimal.Round((productsTotal + shippingCosts), order.DigitsAfterDecimal, MidpointRounding.AwayFromZero).ToString(CultureInfo.GetCultureInfo("nl-NL").NumberFormat);

                Encryptor encryptor = new Encryptor(_provider);
                Orders _order = orderBundle.Order;
                model = new PdfViewModel
                {
                    Website = new Website(_context, _config).GetCleanWebsiteUrlByWebsiteId(_config.Value.WebsiteId),
                    AddressLine1 = setting.GetSettingValueByKey("addressLine1", "website", _config.Value.WebsiteId),
                    ZipCode = setting.GetSettingValueByKey("zipCode", "website", _config.Value.WebsiteId),
                    Vat = setting.GetSettingValueByKey("vat", "website", _config.Value.WebsiteId),
                    Coc = setting.GetSettingValueByKey("coc", "website", _config.Value.WebsiteId),
                    Country = setting.GetSettingValueByKey("country", "website", _config.Value.WebsiteId),
                    City = setting.GetSettingValueByKey("city", "website", _config.Value.WebsiteId),
                    Date = _order.CreatedDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),
                    OrderNumber = _order.OrderNumber,
                    InvoiceNumber = _order.InvoiceNumber,
                    OrderLines = orderBundle.OrderLines,
                    ShippingAddressLine1 = (!string.IsNullOrWhiteSpace(encryptor.Decrypt(_order.ShippingAddressLine1)) ? encryptor.Decrypt(_order.ShippingAddressLine1) : encryptor.Decrypt(_order.BillingAddressLine1)),
                    ShippingCity = (!string.IsNullOrWhiteSpace(encryptor.Decrypt(_order.ShippingAddressLine1)) ? encryptor.Decrypt(_order.ShippingCity) : encryptor.Decrypt(_order.BillingCity)),
                    ShippingCompany = (!string.IsNullOrWhiteSpace(encryptor.Decrypt(_order.ShippingAddressLine1)) ? encryptor.Decrypt(_order.ShippingCompany) : encryptor.Decrypt(_order.BillingCompany)),
                    ShippingCountry = "Nederland",//(!string.IsNullOrWhiteSpace(_order.ShippingAddressLine1) ? _order.ShippingCountry : _order.BillingCountry),
                    ShippingName = (!string.IsNullOrWhiteSpace(encryptor.Decrypt(_order.ShippingAddressLine1)) ? encryptor.Decrypt(_order.ShippingFirstName) + " " + encryptor.Decrypt(_order.ShippingLastName) : encryptor.Decrypt(_order.BillingFirstName) + " " + encryptor.Decrypt(_order.BillingLastName)),
                    ShippingZipCode = (!string.IsNullOrWhiteSpace(encryptor.Decrypt(_order.ShippingAddressLine1)) ? encryptor.Decrypt(_order.ShippingZipCode) : encryptor.Decrypt(_order.BillingZipCode)),
                    BillingAddressLine1 = encryptor.Decrypt(_order.BillingAddressLine1),
                    BillingCity = encryptor.Decrypt(_order.BillingCity),
                    BillingCompany = encryptor.Decrypt(_order.BillingCompany),
                    BillingCountry = "Nederland",
                    BillingZipCode = encryptor.Decrypt(_order.BillingZipCode),
                    BillingEmail = encryptor.Decrypt(_order.BillingEmail),
                    BillingName = encryptor.Decrypt(_order.BillingFirstName) + " " + encryptor.Decrypt(_order.BillingLastName),
                    BillingPhoneNumber = encryptor.Decrypt(_order.BillingPhoneNumber),
                    BillingVatNumber = encryptor.Decrypt(_order.BillingVatNumber),
                    Status = "Betaald",
                    Note = _order.Note,
                    ShippingCosts = shippingCosts,
                    Subtotal = order.Commerce.GetPriceFormat(decimal.Round((productsTotal), order.DigitsAfterDecimal, MidpointRounding.AwayFromZero).ToString(CultureInfo.GetCultureInfo("nl-NL").NumberFormat), order.Currency),
                    Tax = order.Commerce.GetPriceFormat(decimal.Round(order.GetTotalTax(), order.DigitsAfterDecimal, MidpointRounding.AwayFromZero).ToString(CultureInfo.GetCultureInfo("nl-NL").NumberFormat), order.Currency),
                    TotalExclusive = order.Commerce.GetPriceFormat(decimal.Round((shippingCosts + productsTotal) - order.GetTotalTax(), order.DigitsAfterDecimal, MidpointRounding.AwayFromZero).ToString(CultureInfo.GetCultureInfo("nl-NL").NumberFormat), order.Currency),
                    Total = order.Commerce.GetPriceFormat(decimal.Round((shippingCosts + productsTotal), order.DigitsAfterDecimal, MidpointRounding.AwayFromZero).ToString(CultureInfo.GetCultureInfo("nl-NL").NumberFormat), order.Currency),
                    PriceFormat = order.Commerce.GetPriceFormat("price", order.Currency),
                    DigitsFormat = "0.".PadRight(order.DigitsAfterDecimal + "0.".Length, '0'),
                    DigitsAfterDecimal = order.DigitsAfterDecimal
                };
            }
            else
            {
                model = new PdfViewModel
                {

                };
            }

            return View(model);
        }
    }
}