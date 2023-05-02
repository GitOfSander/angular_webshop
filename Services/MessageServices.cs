using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static Site.Startup;

namespace Site.Services
{
    // This class is used by the application to send Email and SMS
    // when you turn on two-factor authentication in ASP.NET Identity.
    // For more details see this link http://go.microsoft.com/fwlink/?LinkID=532713
    public class AuthMessageSender : IEmailSender, ISmsSender
    {
        public AuthMessageSenderOptions _options { get; } //set only via Secret Manager

        public AuthMessageSender(IOptions<AuthMessageSenderOptions> optionsAccessor)
        {
            _options = optionsAccessor.Value;
        }

        public Task SendEmailAsync(string[] emails, string subject, string message, string fromName, string fromEmail, string[] bccEmails, string[] ccEmails, Dictionary<string, string> attachments = null)
        {
            // Plug in your email service here to send an email.
            return Execute(_options.SendGridKey, subject, message, emails, fromName, fromEmail, bccEmails, ccEmails, attachments);
        }

        public async Task Execute(string apiKey, string subject, string message, string[] emails, string fromName, string fromEmail, string[] bccEmails, string[] ccEmails, Dictionary<string, string> attachments = null)
        {
            SendGridClient client = new SendGridClient(apiKey);
            EmailAddress from = new EmailAddress(fromEmail, fromName);
            List<EmailAddress> emailAddresses = new List<EmailAddress>();
            foreach (string email in emails)
            {
                emailAddresses.Add(new EmailAddress(email));
            }
            List<EmailAddress> bccEmailAddresses = new List<EmailAddress>();
            foreach (string bccEmail in bccEmails)
            {
                bccEmailAddresses.Add(new EmailAddress(bccEmail));
            }
            List<EmailAddress> ccEmailAddresses = new List<EmailAddress>();
            foreach (string ccEmail in ccEmails)
            {
                ccEmailAddresses.Add(new EmailAddress(ccEmail));
            }
            string plainTextContent = Regex.Replace(message, "<[^>]*>", "");
            SendGridMessage msg = MailHelper.CreateSingleEmailToMultipleRecipients(from, emailAddresses, subject, plainTextContent, message);
            if (bccEmailAddresses.Count > 0) msg.AddBccs(bccEmailAddresses);
            if (ccEmailAddresses.Count > 0) msg.AddCcs(ccEmailAddresses);
            if (attachments != null)
            {
                foreach (KeyValuePair<string, string> attachment in attachments)
                {
                    msg.AddAttachment(attachment.Key, attachment.Value);
                }
            }
            Response response = await client.SendEmailAsync(msg);
        }

        public Task SendSmsAsync(string number, string message)
        {
            // Plug in your SMS service here to send a text message.
            return Task.FromResult(0);
        }
    }
}
