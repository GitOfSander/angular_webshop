using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;
using Site.Data;
using Site.Models.App;
using Site.Models.MailChimp;
using Site.Models.Sisow;
using Site.Models.SendCloud;
using Site.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static Site.Models.Order;
using Microsoft.AspNetCore.DataProtection;
using Newtonsoft.Json;

namespace Site.Models
{
    public class Commerce : Controller
    {
        private readonly SiteContext _context;
        private readonly IOptions<AppSettings> _config;
        private readonly IHostingEnvironment _env;
        private readonly IEmailSender _emailSender;
        private readonly IActionContextAccessor _actionContextAccessor;
        private readonly IDataProtectionProvider _provider;

        public Commerce(SiteContext context, IOptions<AppSettings> config = null, IHostingEnvironment env = null, IEmailSender emailSender = null, IActionContextAccessor actionContextAccessor = null, IDataProtectionProvider provider = null)
        {
            _context = context;
            _config = config;
            _env = env;
            _emailSender = emailSender;
            _actionContextAccessor = actionContextAccessor;
            _provider = provider;
        }

        private Orders _order;
        private string _nameRegex = @"[a-zA-Z\sàáâäãåèéêëìíîïòóôöõøùúûüÿýñçčšžÀÁÂÄÃÅÈÉÊËÌÍÎÏÒÓÔÖÕØÙÚÛÜŸÝÑßÇŒÆČŠŽ∂ð ,.'-]+$";
        private string _zipCodeRegex = @"[1-9][0-9]{3}[ ]?([A-RT-Za-rt-z][A-Za-z]|[sS][BCbcE-Re-rT-Zt-z])"; //Only validates zip codes from the netherlands
        private string _houseNrRegEx = @"[0-9]*";
        private string _priceSpace;
        private string _currencyPosition;
        private string _orderNumber;
        private List<Dictionary<string, object>> _paymentInfoErrors = new List<Dictionary<string, object>>();

        public Order Order;
        public PaymentInfo PI;

        public class PaymentInfo
        {
            public int websiteLanguageId { get; set; } = 0;
            public string ReserveGuid { get; set; } = "";
            public bool Newspaper { get; set; } = false;
            public string Client { get; set; } = "private";
            public string DeliveryClient { get; set; } = "private";
            public bool DifferentAddress { get; set; } = false;
            public string Email { get; set; } = "";
            //public string PhoneNumber { get; set; } = "";
            public string Company { get; set; } = "";
            public string City { get; set; } = "";
            public string FirstName { get; set; } = "";
            public string LastName { get; set; } = "";
            public string ZipCode { get; set; } = "";
            public string HouseNr { get; set; } = "";
            public string Addition { get; set; } = "";
            public string AddressLine1 { get; set; } = "";
            public string DeliveryCompany { get; set; } = "";
            public string DeliveryCity { get; set; } = "";
            public string DeliveryFirstName { get; set; } = "";
            public string DeliveryLastName { get; set; } = "";
            public string DeliveryZipCode { get; set; } = "";
            public string DeliveryHouseNr { get; set; } = "";
            public string DeliveryAddition { get; set; } = "";
            public string DeliveryAddressLine1 { get; set; } = "";
            public string GreetingCard { get; set; } = "";
            public string Issuer { get; set; } = "";
            public string Agreement { get; set; } = "";
        }

        public Dictionary<decimal, decimal> CalculatePercentageOfTaxWithPrice(Dictionary<decimal, decimal> prices, decimal total)
        {
            Dictionary<decimal, decimal> percentages = new Dictionary<decimal, decimal>();
            foreach (decimal key in prices.Keys)
            {
                percentages.Add(key, (100 / total) * prices[key]);
            }

            return percentages;
        }

        public string GetCurrency(string currency)
        {
            switch (currency.ToUpper())
            {
                case "EUR":
                    return "€";
                case "USD":
                case "AUD":
                case "BSD":
                case "XCD":
                case "ARS":
                case "BBD":
                case "BZD":
                case "BMD":
                case "BND":
                case "SGD":
                case "CAD":
                case "KYD":
                case "CLP":
                case "COP":
                case "NZD":
                case "CUC":
                case "CUP":
                    return "$";
                case "GBP":
                case "GGP":
                    return "£";
                default:
                    return "$";
            }
        }

        public void SetPriceFormatVariables()
        {
            Setting setting = new Setting(_context);
            _priceSpace = setting.GetSettingValueByKey("priceSpace", "website", _config.Value.WebsiteId).ToLower();
            _currencyPosition = setting.GetSettingValueByKey("currencyPosition", "website", _config.Value.WebsiteId).ToLower();
        }

        public string GetPriceFormat(string price, string currency)
        {
            currency = GetCurrency(currency);
            string space = _priceSpace == "true" ? " " : "";
            return _currencyPosition == "left" ? currency + space + price : price + space + currency;
        }

        public bool IsPromoEnabled(bool schedule, DateTime from, DateTime to, decimal promoPrice, decimal price)
        {
            //If schedule is enabled for the promo price
            if (schedule)
            {
                DateTime dateTime = DateTime.Now;
                if (dateTime.Ticks > from.Ticks && dateTime.Ticks < to.Ticks)
                {
                    //Set promo to true if promo price is filled
                    return (promoPrice != price) ? true : false;
                }
            }
            else
            {
                //Set promo to true if promo price is filled
                return (promoPrice != price) ? true : false;
            }

            return false;
        }

        public async Task<ObjectResult> ValidateAndCreatePaymentAsync()
        {
            if (ValidatePaymentInfo())
            {
                SisowClient sisowClient = null;
                Order order = new Order(_context, _config);
                _order = order.GetOrderByReserveGuid(PI.ReserveGuid);
                if (_order != null)
                {
                    //Do not continue if order is already processing
                    if (_order.Status.ToLower() != "processing")
                    {
                        //Add visitor to mailing list if requested
                        if (PI.Newspaper)
                        {
                            bool result = await new MailChimpManager(_config, _config.Value.OAuth.MailChimp.ApiKey, _config.Value.OAuth.MailChimp.ListId).AddMember("subscribed", PI.Email, PI.FirstName, PI.LastName);
                        }

                        try
                        {
                            //Get current available order number
                            Setting setting = new Setting(_context);
                            Settings _setting = await setting.IncrementSettingValueByKeyAsync("orderCurrent", "website", _config.Value.WebsiteId);
                            _orderNumber = setting.GetSettingValueByKey("orderPrefix", "website", _config.Value.WebsiteId) + _setting.Value + setting.GetSettingValueByKey("orderSuffix", "website", _config.Value.WebsiteId);

                            //Calculate total price
                            OrderBundle _orderBundle = order.GetOrderBundle(_order.Id);
                            order.Commerce = new Commerce(_context, _config);
                            order.DigitsAfterDecimal = Int32.Parse(setting.GetSettingValueByKey("digitsAfterDecimal", "website", _config.Value.WebsiteId));
                            order.Currency = setting.GetSettingValueByKey("currency", "website", _config.Value.WebsiteId);
                            order.DistributePricesToTaxes(_orderBundle.OrderLines);
                            decimal shippingCosts = order.GetTotalShippingCosts(_orderBundle.OrderLines, _orderBundle.OrderShippingZoneMethods);
                            decimal productsTotal = order.GetProductsTotal(_orderBundle.OrderLines);
                            string total = decimal.Round((productsTotal + shippingCosts), order.DigitsAfterDecimal, MidpointRounding.AwayFromZero).ToString(CultureInfo.GetCultureInfo("en-EN").NumberFormat);

                            string domain = new Website(_context, _config).GetWebsiteUrl(_config.Value.WebsiteId);

                            //Create payment
                            sisowClient = new SisowClient(_config.Value.OAuth.Sisow.ClientId, _config.Value.OAuth.Sisow.ClientSecret)
                            {
                                issuerId = PI.Issuer,
                                payment = "", //Empty = iDEAL
                                amount = double.Parse(total),
                                purchaseId = new string(_setting.Value.ToString().Take(16).ToArray()), //Max of 16 characters
                                description = new string(("Bestelnummer: " + _orderNumber).Take(32).ToArray()), //Max of 32 characters
                                notifyUrl = domain + "/spine-api/sisow-notify",
                                returnUrl = domain + "/spine-api/sisow-return",
                                //sr.entranceCode = ...;
                                testMode = _config.Value.OAuth.Sisow.TestMode
                            };

                            if (sisowClient.TransactionRequest() == 0)
                            {
                                //Update order
                                UpdateOrder(order, sisowClient.trxId);
                                return StatusCode(200, Json(sisowClient.issuerUrl));
                            }
                            else
                            {
                                return StatusCode(422, Json(new Dictionary<string, object>() { { "errorType", "errorMessage" } }));
                            }
                        }
                        catch (Exception e)
                        {
                            //Update order
                            _order.Status = "failed";
                            order.UpdateOrder(_order);

                            new Error().ReportError(_config.Value.SystemWebsiteId, "COMMERCE#01", "Er kon geen betaling worden aangemaakt bij Sisow.", "", e.Message);

                            return StatusCode(422, Json(new Dictionary<string, object>() { { "errorType", "errorMessage" } }));
                        }
                    }
                }
                else
                {
                    return StatusCode(422, Json(new Dictionary<string, object>() { { "errorType", "reserveMinutsExpired" } }));
                }
            }

            return StatusCode(422, Json(new Dictionary<string, object>() { { "fields", _paymentInfoErrors } }));
        }

        public void UpdateOrder(Order order, string trxId)
        {
            _order.OrderNumber = _orderNumber;
            _order.CreatedDate = DateTime.Now;
            _order.Status = "processing";
            _order.TransactionId = trxId;
            _order.Note = (PI.GreetingCard != null ? PI.GreetingCard : "");

            Encryptor encryptor = new Encryptor(_provider);
            _order.BillingAddressLine1 = encryptor.Encrypt(PI.AddressLine1 + " " + PI.HouseNr + (PI.Addition != null ? " " + PI.Addition : ""));
            _order.BillingCity = encryptor.Encrypt(PI.City);
            _order.BillingCompany = encryptor.Encrypt(PI.Company);
            _order.BillingEmail = encryptor.Encrypt(PI.Email);
            _order.BillingFirstName = encryptor.Encrypt(PI.FirstName);
            _order.BillingLastName = encryptor.Encrypt(PI.LastName);
            _order.BillingPhoneNumber = encryptor.Encrypt("");//encryptor.Encrypt((PI.PhoneNumber != null) ? PI.PhoneNumber : "");
            _order.BillingZipCode = encryptor.Encrypt(PI.ZipCode);
            _order.BillingCountry = encryptor.Encrypt("NL");
            _order.ShippingAddressLine1 = encryptor.Encrypt((!string.IsNullOrEmpty(PI.DeliveryAddressLine1) ? PI.DeliveryAddressLine1 + " " + PI.DeliveryHouseNr + (PI.DeliveryAddition != null ? " " + PI.DeliveryAddition : "") : ""));
            _order.ShippingCity = encryptor.Encrypt(PI.DeliveryCity);
            _order.ShippingCompany = encryptor.Encrypt(PI.DeliveryCompany);
            _order.ShippingFirstName = encryptor.Encrypt(PI.DeliveryFirstName);
            _order.ShippingLastName = encryptor.Encrypt(PI.DeliveryLastName);
            _order.ShippingZipCode = encryptor.Encrypt(PI.DeliveryZipCode);
            _order.ShippingCountry = encryptor.Encrypt((!string.IsNullOrWhiteSpace(PI.DeliveryAddressLine1) ? "NL" : ""));

            order.UpdateOrder(_order);
        }

        public bool ValidatePaymentInfo()
        {
            if (PI.Agreement.ToLower() != "true")
            {
                _paymentInfoErrors.Add(new Dictionary<string, object>()
                {
                    { "property", "agreement" },
                    { "message", " U dient akkoord te gaan met de privacy verklaring en algemene voorwaarden." }
                });
            }

            ValidateShippingInputs();

            //Only validate delivery inputs if checkbox is enabled
            if (PI.DifferentAddress)
            {
                ValidateDeliveryInputs();
            }

            //Validate greeting card
            if (PI.GreetingCard != "" && PI.GreetingCard != null)
            {
                if (PI.GreetingCard.Length > 140)
                {
                    _paymentInfoErrors.Add(new Dictionary<string, object>()
                    {
                        { "property", "greetingCard" },
                        { "message", " Bericht voor het wenskaartje mag maximaal 140 karakters lang zijn." }
                    });
                }
            }

            if (PI.Issuer == "" || PI.Issuer == null)
            {
                _paymentInfoErrors.Add(new Dictionary<string, object>() {
                    { "property", "issuer" },
                    { "message", "Er is geen bank geselecteerd." }
                });
            }

            return (_paymentInfoErrors.Count > 0) ? false : true;
        }

        public void ValidateShippingInputs()
        {
            //Validate email address
            if (PI.Email != "" && PI.Email != null)
            {
                if (PI.Email.Length > 250)
                {
                    _paymentInfoErrors.Add(new Dictionary<string, object>()
                    {
                        { "property", "email" },
                        { "message", "E-mailadres mag maximaal 250 karakters lang zijn." }
                    });
                }
                else if (!new EmailAddressAttribute().IsValid(PI.Email))
                {
                    _paymentInfoErrors.Add(new Dictionary<string, object>()
                    {
                        { "property", "email" },
                        { "message", "Dit is geen geldig e-mailadres." }
                    });
                }
            }
            else
            {
                _paymentInfoErrors.Add(new Dictionary<string, object>() {
                    { "property", "email" },
                    { "message", "Er is geen e-mailadres ingevuld." }
                });
            }

            ////Validate phone number
            //if (PI.PhoneNumber != "" && PI.PhoneNumber != null)
            //{
            //    if (PI.PhoneNumber.Length > 250)
            //    {
            //        _paymentInfoErrors.Add(new Dictionary<string, object>()
            //        {
            //            { "property", "phoneNumber" },
            //            { "message", "Telefoonnummer mag maximaal 250 karakters lang zijn." }
            //        });
            //    }
            //    if (!new PhoneAttribute().IsValid(PI.PhoneNumber))
            //    {
            //        _paymentInfoErrors.Add(new Dictionary<string, object>()
            //        {
            //            { "property", "phoneNumber" },
            //            { "message", "Dit is geen geldig telefoonnummer." }
            //        });
            //    }
            //}

            //Validate company name
            if (PI.Client == "company")
            {
                if (PI.Company != "" && PI.Company != null)
                {
                    if (PI.Company.Length > 250)
                    {
                        _paymentInfoErrors.Add(new Dictionary<string, object>()
                        {
                            { "property", "company" },
                            { "message", "Bedrijfsnaam mag maximaal 250 karakters lang zijn." }
                        });
                    }
                }
                else
                {
                    _paymentInfoErrors.Add(new Dictionary<string, object>() {
                        { "property", "company" },
                        { "message", "Er is geen bedrijfsnaam ingevuld." }
                    });
                }
            }

            //Validate city
            if (PI.City != "" && PI.City != null)
            {
                if (PI.City.Length > 250)
                {
                    _paymentInfoErrors.Add(new Dictionary<string, object>()
                    {
                        { "property", "city" },
                        { "message", "Plaats mag maximaal 250 karakters lang zijn." }
                    });
                }
            }
            else
            {
                _paymentInfoErrors.Add(new Dictionary<string, object>() {
                    { "property", "city" },
                    { "message", "Er is geen plaats ingevuld." }
                });
            }

            //Validate first name
            if (PI.FirstName != "" && PI.FirstName != null)
            {
                if (PI.FirstName.Length > 250)
                {
                    _paymentInfoErrors.Add(new Dictionary<string, object>()
                    {
                        { "property", "firstName" },
                        { "message", "Voornaam mag maximaal 250 karakters lang zijn." }
                    });
                }
                else if (!Regex.Match(PI.FirstName, _nameRegex, RegexOptions.IgnoreCase).Success)
                {
                    _paymentInfoErrors.Add(new Dictionary<string, object>()
                    {
                        { "property", "firstName" },
                        { "message", "Voornaam kan alleen bestaan uit letters van het alfabet." }
                    });
                }
            }
            else
            {
                _paymentInfoErrors.Add(new Dictionary<string, object>() {
                    { "property", "firstName" },
                    { "message", "Er is geen voornaam ingevuld." }
                });
            }

            //Validate last name
            if (PI.LastName != "" && PI.LastName != null)
            {
                if (PI.LastName.Length > 250)
                {
                    _paymentInfoErrors.Add(new Dictionary<string, object>()
                    {
                        { "property", "firstName" },
                        { "message", "Achternaam mag maximaal 250 karakters lang zijn." }
                    });
                }
                else if (!Regex.Match(PI.LastName, _nameRegex, RegexOptions.IgnoreCase).Success)
                {
                    _paymentInfoErrors.Add(new Dictionary<string, object>()
                    {
                        { "property", "lastName" },
                        { "message", "Achternaam kan alleen bestaan uit letters van het alfabet." }
                    });
                }
            }
            else
            {
                _paymentInfoErrors.Add(new Dictionary<string, object>() {
                    { "property", "lastName" },
                    { "message", "Er is geen achternaam ingevuld." }
                });
            }

            //Validate zip code
            if (PI.ZipCode != "" && PI.ZipCode != null)
            {
                if (PI.ZipCode.Length > 32)
                {
                    _paymentInfoErrors.Add(new Dictionary<string, object>()
                    {
                        { "property", "zipCode" },
                        { "message", "Postcode mag maximaal 32 karakters lang zijn." }
                    });
                }
                else if (!Regex.Match(PI.ZipCode, _zipCodeRegex, RegexOptions.IgnoreCase).Success)
                {
                    _paymentInfoErrors.Add(new Dictionary<string, object>()
                    {
                        { "property", "zipCode" },
                        { "message", "Dit is geen geldig nederlands postcode." }
                    });
                }
            }
            else
            {
                _paymentInfoErrors.Add(new Dictionary<string, object>() {
                    { "property", "zipCode" },
                    { "message", "Er is geen postcode ingevuld." }
                });
            }

            //Validate house number
            if (PI.HouseNr != "" && PI.HouseNr != null)
            {
                if (PI.HouseNr.Length > 15)
                {
                    _paymentInfoErrors.Add(new Dictionary<string, object>()
                    {
                        { "property", "houseNr" },
                        { "message", "Huisnummer mag maximaal 15 karakters lang zijn." }
                    });
                }
                else if (!Regex.Match(PI.HouseNr, _houseNrRegEx, RegexOptions.IgnoreCase).Success)
                {
                    _paymentInfoErrors.Add(new Dictionary<string, object>()
                    {
                        { "property", "houseNr" },
                        { "message", "Huisnummer mag alleen bestaan uit cijfers." }
                    });
                }
            }
            else
            {
                _paymentInfoErrors.Add(new Dictionary<string, object>() {
                    { "property", "houseNr" },
                    { "message", "Er is geen huisnummer ingevuld." }
                });
            }

            //Validate addition
            if (PI.Addition != "" && PI.Addition != null)
            {
                if (PI.Addition.Length > 10)
                {
                    _paymentInfoErrors.Add(new Dictionary<string, object>()
                    {
                        { "property", "addition" },
                        { "message", "Toevoeging mag maximaal 10 karakters lang zijn." }
                    });
                }
            }

            //Validate address line 1
            if (PI.AddressLine1 != "" && PI.AddressLine1 != null)
            {
                if (PI.AddressLine1.Length > 220)
                {
                    _paymentInfoErrors.Add(new Dictionary<string, object>()
                    {
                        { "property", "addressLine1" },
                        { "message", "Straatnaam mag maximaal 220 karakters lang zijn." }
                    });
                }
            }
            else
            {
                _paymentInfoErrors.Add(new Dictionary<string, object>() {
                    { "property", "addressLine1" },
                    { "message", "Er is geen straatnaam ingevuld." }
                });
            }
        }

        public void ValidateDeliveryInputs()
        {
            //Validate company name
            if (PI.DeliveryClient == "company")
            {
                if (PI.DeliveryCompany != "" && PI.DeliveryCompany != null)
                {
                    if (PI.DeliveryCompany.Length > 250)
                    {
                        _paymentInfoErrors.Add(new Dictionary<string, object>()
                        {
                            { "property", "deliveryCompany" },
                            { "message", "Bedrijfsnaam mag maximaal 250 karakters lang zijn." }
                        });
                    }
                }
                else
                {
                    _paymentInfoErrors.Add(new Dictionary<string, object>() {
                        { "property", "deliveryCompany" },
                        { "message", "Er is geen bedrijfsnaam ingevuld." }
                    });
                }
            }

            //Validate city
            if (PI.DeliveryCity != "" && PI.DeliveryCity != null)
            {
                if (PI.City.Length > 250)
                {
                    _paymentInfoErrors.Add(new Dictionary<string, object>()
                    {
                        { "property", "deliveryCity" },
                        { "message", "Plaats mag maximaal 250 karakters lang zijn." }
                    });
                }
            }
            else
            {
                _paymentInfoErrors.Add(new Dictionary<string, object>() {
                    { "property", "deliveryCity" },
                    { "message", "Er is geen plaats ingevuld." }
                });
            }

            //Validate first name
            if (PI.DeliveryFirstName != "" && PI.DeliveryFirstName != null)
            {
                if (PI.DeliveryFirstName.Length > 250)
                {
                    _paymentInfoErrors.Add(new Dictionary<string, object>()
                    {
                        { "property", "deliveryFirstName" },
                        { "message", "Voornaam mag maximaal 250 karakters lang zijn." }
                    });
                }
                else if (!Regex.Match(PI.DeliveryFirstName, _nameRegex, RegexOptions.IgnoreCase).Success)
                {
                    _paymentInfoErrors.Add(new Dictionary<string, object>()
                    {
                        { "property", "deliveryFirstName" },
                        { "message", "Voornaam kan alleen bestaan uit letters van het alfabet." }
                    });
                }
            }
            else
            {
                _paymentInfoErrors.Add(new Dictionary<string, object>() {
                    { "property", "deliveryFirstName" },
                    { "message", "Er is geen voornaam ingevuld." }
                });
            }

            //Validate last name
            if (PI.DeliveryLastName != "" && PI.DeliveryLastName != null)
            {
                if (PI.DeliveryLastName.Length > 250)
                {
                    _paymentInfoErrors.Add(new Dictionary<string, object>()
                    {
                        { "property", "deliveryLastName" },
                        { "message", "Achternaam mag maximaal 250 karakters lang zijn." }
                    });
                }
                else if (!Regex.Match(PI.DeliveryLastName, _nameRegex, RegexOptions.IgnoreCase).Success)
                {
                    _paymentInfoErrors.Add(new Dictionary<string, object>()
                    {
                        { "property", "deliveryLastName" },
                        { "message", "Achternaam kan alleen bestaan uit letters van het alfabet." }
                    });
                }
            }
            else
            {
                _paymentInfoErrors.Add(new Dictionary<string, object>() {
                    { "property", "deliveryLastName" },
                    { "message", "Er is geen achternaam ingevuld." }
                });
            }

            //Validate zip code
            if (PI.DeliveryZipCode != "" && PI.DeliveryZipCode != null)
            {
                if (PI.DeliveryZipCode.Length > 32)
                {
                    _paymentInfoErrors.Add(new Dictionary<string, object>()
                    {
                        { "property", "deliveryZipCode" },
                        { "message", "Postcode mag maximaal 32 karakters lang zijn." }
                    });
                }
                else if (!Regex.Match(PI.DeliveryZipCode, _zipCodeRegex, RegexOptions.IgnoreCase).Success)
                {
                    _paymentInfoErrors.Add(new Dictionary<string, object>()
                    {
                        { "property", "deliveryZipCode" },
                        { "message", "Dit is geen geldig nederlands postcode." }
                    });
                }
            }
            else
            {
                _paymentInfoErrors.Add(new Dictionary<string, object>() {
                    { "property", "deliveryZipCode" },
                    { "message", "Er is geen postcode ingevuld." }
                });
            }

            //Validate house number
            if (PI.DeliveryHouseNr != "" && PI.DeliveryHouseNr != null)
            {
                if (PI.DeliveryHouseNr.Length > 15)
                {
                    _paymentInfoErrors.Add(new Dictionary<string, object>()
                    {
                        { "property", "deliveryHouseNr" },
                        { "message", "Huisnummer mag maximaal 15 karakters lang zijn." }
                    });
                }
                else if (!Regex.Match(PI.DeliveryHouseNr, _houseNrRegEx, RegexOptions.IgnoreCase).Success)
                {
                    _paymentInfoErrors.Add(new Dictionary<string, object>()
                    {
                        { "property", "deliveryHouseNr" },
                        { "message", "Huisnummer mag alleen bestaan uit cijfers." }
                    });
                }
            }
            else
            {
                _paymentInfoErrors.Add(new Dictionary<string, object>() {
                    { "property", "deliveryHouseNr" },
                    { "message", "Er is geen huisnummer ingevuld." }
                });
            }

            //Validate addition
            if (PI.DeliveryAddition != "" && PI.DeliveryAddition != null)
            {
                if (PI.DeliveryAddition.Length > 10)
                {
                    _paymentInfoErrors.Add(new Dictionary<string, object>()
                    {
                        { "property", "deliveryAddition" },
                        { "message", "Toevoeging mag maximaal 10 karakters lang zijn." }
                    });
                }
            }

            //Validate address line 1
            if (PI.DeliveryAddressLine1 != "" && PI.DeliveryAddressLine1 != null)
            {
                if (PI.DeliveryAddressLine1.Length > 220)
                {
                    _paymentInfoErrors.Add(new Dictionary<string, object>()
                    {
                        { "property", "deliveryAddressLine1" },
                        { "message", "Straatnaam mag maximaal 220 karakters lang zijn." }
                    });
                }
            }
            else
            {
                _paymentInfoErrors.Add(new Dictionary<string, object>() {
                    { "property", "deliveryAddressLine1" },
                    { "message", "Er is geen straatnaam ingevuld." }
                });
            }
        }

        public async Task<ObjectResult> ProcessPaymentAsync(string trxId)
        {
            SisowClient sisowClient = new SisowClient(_config.Value.OAuth.Sisow.ClientId, _config.Value.OAuth.Sisow.ClientSecret);
            if (sisowClient.StatusRequest(trxId) == 0)
            {
                //Convert Sisow status to the status of our system
                string status = GetStatus(sisowClient.status);

                //Get order bundle and update order
                Order = new Order(_context, _config);
                OrderBundle _orderBundle = Order.GetOrderBundleByTransactionId(trxId);

                //Continue if the status is not completed
                if (_orderBundle.Order.Status.ToLower() != "completed")
                {
                    _orderBundle.Order.Status = status;

                    //Update invoice number and create label in Sendcloud if completed
                    if (status == "completed")
                    {
                        //Create label in SendCloud
                        try
                        {
                            await CreateLabelAsync(_orderBundle);
                        }
                        catch (Exception e)
                        {
                            new Error().ReportError(_config.Value.SystemWebsiteId, "COMMERCE#02", "Bij een betaling kon er geen label worden aangemaakt bij SendCloud.", "", e.Message);
                        }


                        if (string.IsNullOrWhiteSpace(_orderBundle.Order.InvoiceNumber))
                        {
                            //Get current available invoice number
                            Setting setting = new Setting(_context);
                            Settings _setting = await setting.IncrementSettingValueByKeyAsync("invoiceCurrent", "website", _config.Value.WebsiteId);
                            string _invoiceNumber = setting.GetSettingValueByKey("invoicePrefix", "website", _config.Value.WebsiteId) + _setting.Value + setting.GetSettingValueByKey("invoiceSuffix", "website", _config.Value.WebsiteId);

                            _orderBundle.Order.InvoiceNumber = _invoiceNumber;
                        }
                    }

                    Order.UpdateOrder(_orderBundle.Order);

                    //Update stock quantity
                    if (status == "completed") Order.UpdateProductsStockQuantity(_orderBundle.OrderLines);

                    //Send mail to customer if status is completed
                    await SendMailAsync(status, _orderBundle);
                }

                return Ok("");
            }

            return StatusCode(400, "");
        }

        public async Task CreateLabelAsync(OrderBundle orderBundle)
        {
            ApiKeys _apiKey = _context.ApiKeys.FirstOrDefault(ApiKey => ApiKey.Description.ToLower() == "sendcloud" && ApiKey.WebsiteId == _config.Value.WebsiteId);
            if (_apiKey != null)
            {
                decimal totalWeight = 0.00m;
                //decimal totalInsured = 0.00m;
                foreach (OrderLines orderLine in orderBundle.OrderLines)
                {
                    Products _product = new Product(_context, _config).GetProductById(orderLine.ProductId);
                    totalWeight = totalWeight + (_product != null ? _product.Weight : 0);
                    //totalInsured = totalInsured + orderLine.Price;
                }

                //Set total weight to 0.001 if it is 0
                if (totalWeight == 0) totalWeight = 0.001m;

                Orders _order = orderBundle.Order;
                SendCloudClient sendCloud = new SendCloudClient(_config, _apiKey.ClientId, _apiKey.ClientSecret);
                Encryptor encryptor = new Encryptor(_provider);

                //Dictionary<string, object> dic = await sendCloud.GetAsync("shipping_methods");
                //Dictionary<string, object> shippingMethod = null;
                //
                //foreach (KeyValuePair<string, object> parcels in dic)
                //{
                //    var regex = new Regex(Regex.Escape("{["));
                //    var obj = regex.Replace(parcels.Value.ToString(), "", 1);
                //    regex = new Regex(Regex.Escape("]}"));
                //    obj = regex.Replace(obj, "", obj.LastIndexOf("]}"));
                //
                //    List<Dictionary<string, object>> list = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(obj);
                //    foreach (Dictionary<string, object> shipping in list)
                //    {
                //        if (totalWeight <= decimal.Parse(shipping.FirstOrDefault(s => s.Key == "max_weight").Value.ToString()) && totalWeight >= decimal.Parse(shipping.FirstOrDefault(s => s.Key == "min_weight").Value.ToString())) 
                //        {
                //            KeyValuePair<string, object> countries = shipping.FirstOrDefault(s => s.Key == "countries");
                //            if (countries.Value != null)
                //            {
                //                List<Dictionary<string, object>> countriesList = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(countries.Value.ToString());
                //                foreach (Dictionary<string, object> countrie in countriesList)
                //                {
                //                    if (countrie.FirstOrDefault(s => s.Key == "iso_2").Value.ToString() == (!string.IsNullOrWhiteSpace(encryptor.Decrypt(_order.ShippingAddressLine1)) ? encryptor.Decrypt(_order.ShippingCountry) : encryptor.Decrypt(_order.BillingCountry)).ToUpper())
                //                    {
                //                        shippingMethod = new Dictionary<string, object>() {
                //                            { "id",  Int32.Parse(shipping.FirstOrDefault(s => s.Key == "id").Value.ToString()) },
                //                            { "name",  shipping.FirstOrDefault(s => s.Key == "name").Value.ToString() }
                //                        };
                //
                //                        break;
                //                    }
                //                }
                //            }
                //        }
                //    }
                //}

                Dictionary<string, object> dic = new Dictionary<string, object>()
                {
                    { "name", (!string.IsNullOrWhiteSpace(encryptor.Decrypt(_order.ShippingAddressLine1)) ? encryptor.Decrypt(_order.ShippingFirstName) + " " + encryptor.Decrypt(_order.ShippingLastName) : encryptor.Decrypt(_order.BillingFirstName) + " " + encryptor.Decrypt(_order.BillingLastName)) },
                    { "address", (!string.IsNullOrWhiteSpace(encryptor.Decrypt(_order.ShippingAddressLine1)) ? encryptor.Decrypt(_order.ShippingAddressLine1) :  encryptor.Decrypt(_order.BillingAddressLine1)) },
                    { "city", (!string.IsNullOrWhiteSpace(encryptor.Decrypt(_order.ShippingAddressLine1)) ? encryptor.Decrypt(_order.ShippingCity) :  encryptor.Decrypt(_order.BillingCity)) },
                    { "postal_code", (!string.IsNullOrWhiteSpace(encryptor.Decrypt(_order.ShippingAddressLine1)) ? encryptor.Decrypt(_order.ShippingZipCode)  :  encryptor.Decrypt(_order.BillingZipCode)) },
                    //{ "telephone", encryptor.Decrypt(_order.BillingPhoneNumber) },
                    { "email", encryptor.Decrypt(_order.BillingEmail) },
                    { "country", (!string.IsNullOrWhiteSpace(encryptor.Decrypt(_order.ShippingAddressLine1)) ? encryptor.Decrypt(_order.ShippingCountry)  :  encryptor.Decrypt(_order.BillingCountry)).ToUpper() },
                    //{ "weight", totalWeight },
                    //{ "data", null},
                    { "order_number", _order.OrderNumber }
                };

                //Request label if shipping method is found
                //if(shippingMethod != null)
                //{
                //    dic.Add("request_label", true);
                //    dic.Add("shipment", shippingMethod);
                //}

                if (!string.IsNullOrWhiteSpace(encryptor.Decrypt(_order.ShippingAddressLine1)))
                {
                    if (!string.IsNullOrWhiteSpace(encryptor.Decrypt(_order.ShippingCompany)))
                    {
                        dic.Add("company_name", encryptor.Decrypt(_order.ShippingCompany));
                    }
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(encryptor.Decrypt(_order.BillingCompany)))
                    {
                        dic.Add("company_name", encryptor.Decrypt(_order.BillingCompany));
                    }
                }

                await sendCloud.CreateAsync(dic, "parcel", "parcels");
            }

            //Update apiKey LastAccess
            _apiKey.LastAccess = DateTime.Now;
            new ApiKey(_context).UpdateApiKey(_apiKey);
        }

        public async Task SendMailAsync(string status, OrderBundle orderBundle)
        {
            switch (status)
            {
                case "completed":
                    await SendCompletedMailAsync(orderBundle);
                    return;
                case "cancelled":
                    return;
                case "failed":
                    return;
                case "pending":
                    return;
                default:
                    return;
            }
        }

        public async Task SendCompletedMailAsync(OrderBundle orderBundle)
        {
            try
            {
                await CreatePdfAsync(orderBundle);

                //Collect order information
                Setting setting = new Setting(_context);
                Order order = new Order(_context, _config);
                order.Commerce = new Commerce(_context, _config);
                order.Commerce.SetPriceFormatVariables();
                order.DigitsAfterDecimal = Int32.Parse(setting.GetSettingValueByKey("digitsAfterDecimal", "website", _config.Value.WebsiteId));
                order.Currency = setting.GetSettingValueByKey("currency", "website", _config.Value.WebsiteId);
                order.DistributePricesToTaxes(orderBundle.OrderLines);
                decimal shippingCosts = order.GetTotalShippingCosts(orderBundle.OrderLines, orderBundle.OrderShippingZoneMethods);
                decimal productsTotal = order.GetProductsTotal(orderBundle.OrderLines);
                string total = decimal.Round((productsTotal), order.DigitsAfterDecimal, MidpointRounding.AwayFromZero).ToString(CultureInfo.GetCultureInfo("de-DE").NumberFormat);

                Encryptor encryptor = new Encryptor(_provider);
                Orders _order = orderBundle.Order;
                string emailTemplateHtml = System.IO.File.ReadAllText(_env.WebRootPath + "\\templates\\email\\order\\index.html");
                emailTemplateHtml = emailTemplateHtml.Replace("!url!", string.Format("{0}://{1}{2}", _actionContextAccessor.ActionContext.HttpContext.Request.Scheme, _actionContextAccessor.ActionContext.HttpContext.Request.Host, "/assets/templates/email/order"));
                emailTemplateHtml = emailTemplateHtml.Replace("!order_billing_first_name!", encryptor.Decrypt(orderBundle.Order.BillingFirstName));
                emailTemplateHtml = emailTemplateHtml.Replace("!order_billing_last_name!", encryptor.Decrypt(orderBundle.Order.BillingLastName));
                emailTemplateHtml = emailTemplateHtml.Replace("!order_shipping_first_name!", (!string.IsNullOrWhiteSpace(encryptor.Decrypt(_order.ShippingAddressLine1)) ? encryptor.Decrypt(_order.ShippingFirstName) : encryptor.Decrypt(_order.BillingFirstName)));
                emailTemplateHtml = emailTemplateHtml.Replace("!order_shipping_last_name!", (!string.IsNullOrWhiteSpace(encryptor.Decrypt(_order.ShippingAddressLine1)) ? encryptor.Decrypt(_order.ShippingLastName) : encryptor.Decrypt(_order.BillingLastName)));
                emailTemplateHtml = emailTemplateHtml.Replace("!order_shipping_address_line_1!", (!string.IsNullOrWhiteSpace(encryptor.Decrypt(_order.ShippingAddressLine1)) ? encryptor.Decrypt(_order.ShippingAddressLine1) : encryptor.Decrypt(_order.BillingAddressLine1)));
                emailTemplateHtml = emailTemplateHtml.Replace("!order_shipping_zip_code!", (!string.IsNullOrWhiteSpace(encryptor.Decrypt(_order.ShippingAddressLine1)) ? encryptor.Decrypt(_order.ShippingZipCode) : encryptor.Decrypt(_order.BillingZipCode)));
                emailTemplateHtml = emailTemplateHtml.Replace("!order_shipping_city!", (!string.IsNullOrWhiteSpace(encryptor.Decrypt(_order.ShippingAddressLine1)) ? encryptor.Decrypt(_order.ShippingCity) : encryptor.Decrypt(_order.BillingCity)));
                emailTemplateHtml = emailTemplateHtml.Replace("!order_shipping_country!", "Nederland");
                emailTemplateHtml = emailTemplateHtml.Replace("!order_number!", _order.OrderNumber);
                emailTemplateHtml = emailTemplateHtml.Replace("!order_shipping_costs!", order.Commerce.GetPriceFormat(decimal.Round(shippingCosts, order.DigitsAfterDecimal, MidpointRounding.AwayFromZero).ToString(CultureInfo.GetCultureInfo("nl-NL").NumberFormat), order.Currency));
                emailTemplateHtml = emailTemplateHtml.Replace("!order_subtotal!", order.Commerce.GetPriceFormat(total, order.Currency));
                emailTemplateHtml = emailTemplateHtml.Replace("!order_tax!", order.Commerce.GetPriceFormat(decimal.Round(order.GetTotalTax(), order.DigitsAfterDecimal, MidpointRounding.AwayFromZero).ToString(CultureInfo.GetCultureInfo("nl-NL").NumberFormat), order.Currency));
                emailTemplateHtml = emailTemplateHtml.Replace("!order_total_excl!", order.Commerce.GetPriceFormat(decimal.Round(((shippingCosts + productsTotal) - order.GetTotalTax()), order.DigitsAfterDecimal, MidpointRounding.AwayFromZero).ToString(CultureInfo.GetCultureInfo("nl-NL").NumberFormat), order.Currency));
                emailTemplateHtml = emailTemplateHtml.Replace("!order_total!", order.Commerce.GetPriceFormat(decimal.Round((shippingCosts + productsTotal), order.DigitsAfterDecimal, MidpointRounding.AwayFromZero).ToString(CultureInfo.GetCultureInfo("nl-NL").NumberFormat), order.Currency));

                StringBuilder sb = new StringBuilder();
                string orderRowHtml = System.IO.File.ReadAllText(_env.WebRootPath + "\\templates\\email\\order\\order_row.html");
                foreach (OrderLines orderLine in orderBundle.OrderLines)
                {
                    sb.AppendLine(orderRowHtml);
                    sb.Replace("!product_name!", orderLine.Name);
                    sb.Replace("!product_quantity!", orderLine.Quantity.ToString());
                    sb.Replace("!product_total!", order.Commerce.GetPriceFormat((decimal.Round(orderLine.Price, order.DigitsAfterDecimal, MidpointRounding.AwayFromZero) * orderLine.Quantity).ToString(CultureInfo.GetCultureInfo("nl-NL").NumberFormat), order.Currency));
                }
                emailTemplateHtml = emailTemplateHtml.Replace("!order_rows!", sb.ToString());

                StringBuilder sb2 = new StringBuilder();
                string taxClassRowHtml = System.IO.File.ReadAllText(_env.WebRootPath + "\\templates\\email\\order\\tax_class_row.html");
                foreach (KeyValuePair<string, object> taxClass in order.TaxClasses)
                {
                    if (decimal.Parse(taxClass.Value.ToString()) > 0)
                    {
                        sb2.AppendLine(taxClassRowHtml);
                        sb2.Replace("!tax_class_tax!", decimal.Round(decimal.Parse(taxClass.Key.ToString()), order.DigitsAfterDecimal, MidpointRounding.AwayFromZero).ToString("G29", CultureInfo.GetCultureInfo("nl-NL").NumberFormat));
                        sb2.Replace("!tax_class_total!", order.Commerce.GetPriceFormat((decimal.Round(decimal.Parse(taxClass.Value.ToString()), order.DigitsAfterDecimal, MidpointRounding.AwayFromZero)).ToString(CultureInfo.GetCultureInfo("nl-NL").NumberFormat), order.Currency));
                    }
                }
                emailTemplateHtml = emailTemplateHtml.Replace("!order_tax_class_rows!", sb2.ToString());


                if (!string.IsNullOrWhiteSpace(_order.Note))
                {
                    StringBuilder sb3 = new StringBuilder();
                    string noteHtml = System.IO.File.ReadAllText(_env.WebRootPath + "\\templates\\email\\order\\order_note.html");
                    sb3.AppendLine(noteHtml);
                    sb3.Replace("!note!", _order.Note.Replace("\n", "<br />"));

                    emailTemplateHtml = emailTemplateHtml.Replace("!order_note!", sb3.ToString());
                }
                else
                {
                    emailTemplateHtml = emailTemplateHtml.Replace("!order_note!", "");
                }


                string path = Path.Combine(_env.ContentRootPath + $@"\Files\Pdf", "Invoice_" + orderBundle.Order.Id + ".pdf");
                byte[] pdfBytes = System.IO.File.ReadAllBytes(path);

                Dictionary<string, string> attachments = new Dictionary<string, string>();
                if (pdfBytes != null)
                {
                    attachments.Add(new File().RemoveInvalidCharacters("Factuur " + orderBundle.Order.OrderNumber + " van DOMEIN.nl.pdf"), Convert.ToBase64String(pdfBytes));
                };

                string[] emails = { encryptor.Decrypt(orderBundle.Order.BillingEmail) };
                string[] bccEmails = { _config.Value.Mail.Shop.Email };
                string[] ccEmails = { };
                await _emailSender.SendEmailAsync(emails, "Uw bestelling via DOMEIN.nl", emailTemplateHtml, _config.Value.Mail.Shop.Name, _config.Value.Mail.Shop.Email, bccEmails, ccEmails, attachments);

                //Delete file if exists
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }
            }
            catch (Exception e)
            {
                new Error().ReportError(_config.Value.SystemWebsiteId, "COMMERCE#03", "Bij een betaling kon er geen factuur aangemaakt en verzonden worden naar de koper.", "", e.Message);
            }
        }

        public async Task CreatePdfAsync(OrderBundle orderBundle)
        {
            UrlHelper urlHelper = new UrlHelper(_actionContextAccessor.ActionContext);

            RouteValueDictionary routeValueDictionary = new RouteValueDictionary()
            {
                { "id", orderBundle.Order.Id }
            };

            string baseUrl = _actionContextAccessor.ActionContext.HttpContext.Request.Scheme + "://" + _actionContextAccessor.ActionContext.HttpContext.Request.Host;
            string urlAction = urlHelper.Action("Index", "Pdf", routeValueDictionary);
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(baseUrl);
                    var result = await client.GetAsync(urlAction);
                    if (result.IsSuccessStatusCode && result.StatusCode == HttpStatusCode.OK)
                    {
                        await result.Content.ReadAsStringAsync();
                    }
                }

            }
            catch (Exception e)
            {
                Console.Write(e);
            }
        }

        public string GetStatus(string status)
        {
            switch (status.ToLower())
            {
                case "success":
                    return "completed";
                case "cancelled":
                    return "cancelled";
                case "expired":
                case "failure":
                    return "failed";
                case "pending":
                    return "pending";
                case "reversed":
                default:
                    return "processing";
            };
        }
    }
}