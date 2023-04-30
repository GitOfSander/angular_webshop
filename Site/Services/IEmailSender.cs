using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Site.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string[] email, string subject, string message, string fromName, string fromEmail, string[] bccEmails, string[] ccEmails, Dictionary<string, string> attachments = null);
    }
}
