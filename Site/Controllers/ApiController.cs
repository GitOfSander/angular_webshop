using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Internal;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using MimeKit;
using Org.BouncyCastle.Asn1.Ocsp;
using Site.Data;
using Site.Models;
using Site.Models.App;
using Site.Models.MailChimp;
using Site.Models.SendCloud;
using Site.Models.Sisow;
using Site.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using static Site.Startup;

namespace Site.Controllers
{
    public class ApiController : Controller
    {
        SiteContext _context;
        private readonly IOptions<AppSettings> _config;
        private IHostingEnvironment _env;
        private readonly IEmailSender _emailSender;
        private readonly IActionContextAccessor _actionContextAccessor;
        private readonly IDataProtectionProvider _provider;

        public ApiController(SiteContext context, IOptions<AppSettings> config, IHostingEnvironment env, IEmailSender emailSender, IActionContextAccessor actionContextAccessor, IDataProtectionProvider provider)
        {
            _context = context;
            _config = config;
            _env = env;
            _emailSender = emailSender;
            _actionContextAccessor = actionContextAccessor;
            _provider = provider;
        }

        //[Route("/spine-api/review")]
        //[HttpPost]
        //public IActionResult SendReview([FromBody] MailInformation MailInformation)
        //{
        //    //List<Dictionary<string, object>> PfParentRow = new List<Dictionary<string, object>>();
        //    Dictionary<string, object> Errors;
        //    Errors = new Dictionary<string, object>();
        //    //PfParentRow.Add(PfChildRow);

        //    ArrayList errors = new ArrayList();
        //    if (MailInformation.Name == "")
        //    {
        //        Errors.Add("name", "Er is geen naam ingevuld.");
        //    }

        //    if (MailInformation.Email != "")
        //    {
        //        if (!new EmailAddressAttribute().IsValid(MailInformation.Email))
        //        {
        //            Errors.Add("email", "Dit is geen geldig e-mailadres.");
        //        }
        //    }
        //    else
        //    {
        //        Errors.Add("email", "Er is geen e-mailadres ingevuld.");
        //    }

        //    if (MailInformation.Rating == 0)
        //    {
        //        Errors.Add("rating", "Er is geen beoordeling achtergelaten.");
        //    }

        //    if (MailInformation.Message == "")
        //    {
        //        Errors.Add("message", "Er is geen recensie ingevuld.");
        //    }

        //    if (Errors.Count > 0)
        //    {
        //        return Json(new
        //        {
        //            success = false,
        //            errors = Errors,
        //        });
        //    }
        //    var s = RouteData.Values["websiteLanguageId"];
        //    bool addedReview = new Review(_context, _config).InsertReview("General", 1, 0, "", MailInformation.Name, MailInformation.Email, MailInformation.Message, MailInformation.Rating);

        //    string Message = MailInformation.Message + Environment.NewLine + Environment.NewLine + MailInformation.Name + Environment.NewLine + MailInformation.Email + Environment.NewLine + MailInformation.PhoneNumber;

        //    bool SendMail = new Mailing().SmtpMail(MailInformation.Name, MailInformation.Email, Message, "SiSana", "info@unveil.nl", "Nieuwe recensie via sisana.nl", "", "", null);

        //    if (!SendMail)
        //    {
        //        Errors.Add("error", "Er is iets mis gegaan. Probeer het later opnieuw!");
        //        return Json(new
        //        {
        //            success = false,
        //            errors = Errors
        //        });
        //    }

        //    return Json(new
        //    {
        //        success = true,
        //        result = "Hey " + MailInformation.Name + ", uw recensie is succesvol verzonden!",
        //    });
        //}



        [Route("/api/test-payment")]
        [HttpGet]
        public async Task<IActionResult> TestPaymentAsync(string password)
        {
            if (password != "TestPayment123") return Content("<html><body><h3>Incorrect password!</h3></body></html>", "text/html");

            bool CreatePaymentStatus = false;
            bool CreateSendcloudLabelStatus = false;
            bool AddToMailChimpStatus = false;
            bool EncryptStatus = false;
            bool CreatePdfStatus = false;
            bool SendMailStatus = false;
            string CreatePaymentMessage = "";
            string CreateSendcloudLabelMessage = "";
            string AddToMailChimpMessage = "";
            string EncryptMessage = "";
            string CreatePdfMessage = "";
            string SendMailMessage = "";

            //Create payment
            try
            {
                string domain = new Website(_context, _config).GetWebsiteUrl(_config.Value.WebsiteId);

                //Create payment
                SisowClient sisowClient = new SisowClient("2538117612", "71d49ed6357c78400b1432bd021043fb4649b931")
                {
                    issuerId = "01",
                    payment = "", //Empty = iDEAL
                    amount = double.Parse("10.00"),
                    purchaseId = new string(("PUR0001").Take(16).ToArray()), //Max of 16 characters
                    description = new string(("Bestelnummer: 0001").Take(32).ToArray()), //Max of 32 characters
                    returnUrl = domain + "/api/test-return-payment",
                    testMode = true
                };

                if (sisowClient.TransactionRequest() == 0)
                {
                    return Redirect(sisowClient.issuerUrl);
                }
                else
                {
                    CreatePaymentMessage = "Sisow TransactionRequest failed!"; 
                }
            }
            catch (Exception e)
            {
                CreatePaymentMessage = e.Message;
            }


            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("<html><body><h3>Results:</h3><br /><table><tbody>");
            stringBuilder.Append("<tr><td>Process</td><td>Status</td><td>Exception message</td></tr>");
            stringBuilder.Append("<tr><td>Create payment</td><td>" + (CreatePaymentStatus ? "<b style='color: green;'>Success</b>" : "<b style='color: red;'>Failed</b>") + "</td><td>" + CreatePaymentMessage + "</td></tr>");
            stringBuilder.Append("<tr><td>Add email address to MailChimp</td><td>" + (AddToMailChimpStatus ? "<b style='color: green;'>Success</b>" : "<b style='color: red;'>Failed</b>") + "</td><td>" + AddToMailChimpMessage + "</td></tr>");
            stringBuilder.Append("<tr><td>Encrypt and decrypt string</td><td>" + (EncryptStatus ? "<b style='color: green;'>Success</b>" : "<b style='color: red;'>Failed</b>") + "</td><td>" + EncryptMessage + "</td></tr>");
            stringBuilder.Append("<tr><td>Create Sendcloud label</td><td>" + (CreateSendcloudLabelStatus ? "<b style='color: green;'>Success</b>" : "<b style='color: red;'>Failed</b>") + "</td><td>" + CreateSendcloudLabelMessage + "</td></tr>");
            stringBuilder.Append("<tr><td>Create PDF</td><td>" + (CreatePdfStatus ? "<b style='color: green;'>Success</b>" : "<b style='color: red;'>Failed</b>") + "</td><td>" + CreatePdfMessage + "</td></tr>");
            stringBuilder.Append("<tr><td>Send mail</td><td>" + (SendMailStatus ? "<b style='color: green;'>Success</b>" : "<b style='color: red;'>Failed</b>") + "</td><td>" + SendMailMessage + "</td></tr>");
            stringBuilder.Append("</tbody></table></body></html>");

            return Content(stringBuilder.ToString(), "text/html");
        }

        [Route("/api/test-return-payment")]
        [HttpGet]
        public async Task<IActionResult> TestReturnPaymentAsync(string trxid, string ec, string status, string sha1)
        {
            bool CreatePaymentStatus = true;
            bool CreateSendcloudLabelStatus = false;
            bool AddToMailChimpStatus = false;
            bool EncryptStatus = false;
            bool CreatePdfStatus = false;
            bool SendMailStatus = false;
            string CreatePaymentMessage = "";
            string CreateSendcloudLabelMessage = "";
            string AddToMailChimpMessage = "";
            string EncryptMessage = "";
            string CreatePdfMessage = "";
            string SendMailMessage = "";

            //Add email address to MailChimp
            try
            {
                bool result = await new MailChimpManager(_config, "a5c75d05d0cf0de135e3cf605eff3d2a-us16", "0a733d3c98").AddMember("subscribed", "kkk@unveil.nl", "Sander", "Pals");
                AddToMailChimpStatus = true;
            }
            catch (Exception e)
            {
                AddToMailChimpMessage = e.Message;
            }

            //Encrypt and decrypt string
            try
            {
                Encryptor encryptor = new Encryptor(_provider);
                string email = encryptor.Encrypt("sander@unveil.nl");
                email = encryptor.Decrypt(email);

                EncryptStatus = true;
            }
            catch (Exception e)
            {
                EncryptMessage = e.Message;
            }

            //Create Sendcloud label
            try
            {
                SendCloudClient sendCloud = new SendCloudClient(_config, "40df9de544264fa28cc48d4a151f1908", "75ab36df3c4e4fd9af05641dda210d72");

                Dictionary<string, object> dic = new Dictionary<string, object>()
                {
                    { "name", "NAME" },
                    { "address", "STREET 14" },
                    { "city", "Oosterhout" },
                    { "postal_code", "4904KH" },
                    //{ "telephone", encryptor.Decrypt(_order.BillingPhoneNumber) },
                    { "email", "sander@unveil.nl" },
                    { "country", "NL" },
                    //{ "weight", totalWeight },
                    //{ "data", null },
                    { "order_number", "ORD0001" },
                    { "company_name", "Unveil" }
                };

                await sendCloud.CreateAsync(dic, "parcel", "parcels");

                CreateSendcloudLabelStatus = true;
            }
            catch (Exception e)
            {
                CreateSendcloudLabelMessage = e.Message;
            }

            //Create PDF
            UrlHelper urlHelper = new UrlHelper(_actionContextAccessor.ActionContext);

            RouteValueDictionary routeValueDictionary = new RouteValueDictionary()
            {
                { "id", "0" },
                { "test", true }
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

                CreatePdfStatus = true;
            }
            catch (Exception e)
            {
                CreatePdfMessage = e.Message;
            }

            //Send mail
            if (CreatePdfStatus) { 
                try
                {
                    string emailTemplateHtml = System.IO.File.ReadAllText(_env.WebRootPath + "\\templates\\email\\order\\index.html");
                    emailTemplateHtml = emailTemplateHtml.Replace("!url!", string.Format("{0}://{1}{2}", _actionContextAccessor.ActionContext.HttpContext.Request.Scheme, _actionContextAccessor.ActionContext.HttpContext.Request.Host, "/assets/templates/email/order"));
                    emailTemplateHtml = emailTemplateHtml.Replace("!order_billing_first_name!", "Sander");
                    emailTemplateHtml = emailTemplateHtml.Replace("!order_billing_last_name!", "Pals");
                    emailTemplateHtml = emailTemplateHtml.Replace("!order_shipping_first_name!","Sander");
                    emailTemplateHtml = emailTemplateHtml.Replace("!order_shipping_last_name!", "Pals");
                    emailTemplateHtml = emailTemplateHtml.Replace("!order_shipping_address_line_1!", "Verzendstraat 83");
                    emailTemplateHtml = emailTemplateHtml.Replace("!order_shipping_zip_code!", "5678LP");
                    emailTemplateHtml = emailTemplateHtml.Replace("!order_shipping_city!", "Oosterhout");
                    emailTemplateHtml = emailTemplateHtml.Replace("!order_shipping_country!", "Nederland");
                    emailTemplateHtml = emailTemplateHtml.Replace("!order_number!", "ORD0001");
                    emailTemplateHtml = emailTemplateHtml.Replace("!order_shipping_costs!", "€5,00");
                    emailTemplateHtml = emailTemplateHtml.Replace("!order_subtotal!", "€7,00");
                    emailTemplateHtml = emailTemplateHtml.Replace("!order_tax!", "€1,00");
                    emailTemplateHtml = emailTemplateHtml.Replace("!order_total_excl!", "€6,00");
                    emailTemplateHtml = emailTemplateHtml.Replace("!order_total!", "€8,00");

                    StringBuilder sb = new StringBuilder();
                    string orderRowHtml = System.IO.File.ReadAllText(_env.WebRootPath + "\\templates\\email\\order\\order_row.html");
                    sb.AppendLine(orderRowHtml);
                    sb.Replace("!product_name!", "Social media post");
                    sb.Replace("!product_quantity!", "1");
                    sb.Replace("!product_total!", "8,00");
                    emailTemplateHtml = emailTemplateHtml.Replace("!order_rows!", sb.ToString());

                    StringBuilder sb2 = new StringBuilder();
                    string taxClassRowHtml = System.IO.File.ReadAllText(_env.WebRootPath + "\\templates\\email\\order\\tax_class_row.html");
                    sb2.AppendLine(taxClassRowHtml);
                    sb2.Replace("!tax_class_tax!", "21.00");
                    sb2.Replace("!tax_class_total!", "€1,00");
                    emailTemplateHtml = emailTemplateHtml.Replace("!order_tax_class_rows!", sb2.ToString());


                    emailTemplateHtml = emailTemplateHtml.Replace("!order_note!", "Test");


                    string path = Path.Combine(_env.ContentRootPath + $@"\Files\Pdf", "Invoice_Test.pdf");
                    byte[] pdfBytes = System.IO.File.ReadAllBytes(path);

                    Dictionary<string, string> attachments = new Dictionary<string, string>();
                    if (pdfBytes != null)
                    {
                        attachments.Add(new Models.File().RemoveInvalidCharacters("Factuur ORD0001 van DOMEIN.nl.pdf"), Convert.ToBase64String(pdfBytes));
                    };

                    string[] emails = { "sander@unveil.nl" };
                    string[] bccEmails = { };
                    string[] ccEmails = { };
                    await _emailSender.SendEmailAsync(emails, "Uw bestelling via DOMEIN.nl", emailTemplateHtml, _config.Value.Mail.Shop.Name, "test@unveil.nl", bccEmails, ccEmails, attachments);

                    //Delete file if exists
                    if (System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);
                    }

                    SendMailStatus = true;
                }
                catch (Exception e)
                {
                    SendMailMessage = e.Message;
                }
            }

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("<html><body><h3>Results:</h3><br /><table><tbody>");
            stringBuilder.Append("<tr><td>Process</td><td>Status</td><td>Exception message</td></tr>");
            stringBuilder.Append("<tr><td>Create payment</td><td>" + (CreatePaymentStatus ? "<b style='color: green;'>Success</b>" : "<b style='color: red;'>Failed</b>") + "</td><td>" + CreatePaymentMessage + "</td></tr>");
            stringBuilder.Append("<tr><td>Add email address to MailChimp</td><td>" + (AddToMailChimpStatus ? "<b style='color: green;'>Success</b>" : "<b style='color: red;'>Failed</b>") + "</td><td>" + AddToMailChimpMessage + "</td></tr>");
            stringBuilder.Append("<tr><td>Encrypt and decrypt string</td><td>" + (EncryptStatus ? "<b style='color: green;'>Success</b>" : "<b style='color: red;'>Failed</b>") + "</td><td>" + EncryptMessage + "</td></tr>");
            stringBuilder.Append("<tr><td>Create Sendcloud label</td><td>" + (CreateSendcloudLabelStatus ? "<b style='color: green;'>Success</b>" : "<b style='color: red;'>Failed</b>") + "</td><td>" + CreateSendcloudLabelMessage + "</td></tr>");
            stringBuilder.Append("<tr><td>Create PDF</td><td>" + (CreatePdfStatus ? "<b style='color: green;'>Success</b>" : "<b style='color: red;'>Failed</b>") + "</td><td>" + CreatePdfMessage + "</td></tr>");
            stringBuilder.Append("<tr><td>Send mail</td><td>" + (SendMailStatus ? "<b style='color: green;'>Success</b>" : "<b style='color: red;'>Failed</b>") + "</td><td>" + SendMailMessage + "</td></tr>");
            stringBuilder.Append("</tbody></table></body></html>");

            return Content(stringBuilder.ToString(), "text/html");
        }
    }
}
