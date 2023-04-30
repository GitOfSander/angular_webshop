using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Site.Models.App
{
    public class AppSettings
    {
        public int WebsiteId { get; set; }
        public int SystemWebsiteId { get; set; }
        public Mail Mail { get; set; }
        public OAuth OAuth { get; set; }
    }

    public class Mail
    {
        public MailInfo Shop { get; set; }
    }

    public class OAuth
    {
        public AuthenticationData Sisow { get; set; }
        public MailChimp MailChimp { get; set; }
    }

    public class MailInfo
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Subject { get; set; }
        public string BCC { get; set; }
        public string CC { get; set; }
    }

    public class AuthenticationData
    {
        public string Url { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string WebhookSecret { get; set; }
        public string CallbackPath { get; set; }
        public string AuthorizationEndpoint { get; set; }
        public string TokenEndpoint { get; set; }
        public bool TestMode { get; set; }
    }

    public class MailChimp
    {
        public string Url { get; set; }
        public string ListId { get; set; }
        public string ApiKey { get; set; }
        public string ClientSecret { get; set; }
    }

    public class WeFact
    {
        public string Url { get; set; }
        public string Key { get; set; }
    }
}
