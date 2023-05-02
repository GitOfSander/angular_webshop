using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

/// <summary>
/// Summary description for sisowrest
/// </summary>

namespace Site.Models.Sisow
{
    public class SisowClient
    {
        private string[] issuerid;
        private string[] issuername;
        private DateTime lastcheck;

        private string response;

        // Merchant data
        public string merchantId;
        public string merchantKey;

        // Transaction data
        public string payment;          // empty=iDEAL; sofort=SofortBanking; mistercash=MisterCash; overboeking=OverBoeking; ecare=Sisow ecare; ...
        public string issuerId;         // mandatory for iDEAL; Sisow iDEAL bank code
        public string purchaseId;       // mandatory; max 16 alphanumeric
        public string entranceCode;     // max 40 strict alphanumeric (letters and numbers only)
        public string description;      // mandatory; max 32 alphanumeric
        public double amount;           // mandatory; min 0.45
        public string notifyUrl;
        public string returnUrl;        // mandatory
        public string cancelUrl;
        public string callbackUrl;
        public bool testMode;

        // Invoice data
        public string invoiceNo;
        public long documentId;
        public string documentUrl;

        // Status data
        public string status;
        public DateTime timeStamp;
        public string consumerAccount;
        public string consumerName;
        public string consumerCity;

        // Result/check data
        public string trxId;
        public string issuerUrl;

        // Error data
        public string errorCode;
        public string errorMessage;

        // Status
        public const string statusSuccess = "Success";
        public const string statusCancelled = "Cancelled";
        public const string statusExpired = "Expired";
        public const string statusFailure = "Failure";
        public const string statusOpen = "Open";

        public SisowClient(string merchantid, string merchantkey)
        {
            this.merchantId = merchantid;
            this.merchantKey = merchantkey;
            testMode = false;
        }

        private void error()
        {
            errorCode = parse("errorcode");
            errorMessage = System.Web.HttpUtility.UrlDecode(parse("errormessage"));
        }

        private string parse(string search)
        {
            return parse(response, search);
        }

        private string parse(string xml, string search)
        {
            int start, end;

            if ((start = xml.IndexOf("<" + search + ">")) < 0)
                return null;
            start += search.Length + 2;
            if ((end = xml.IndexOf("</" + search + ">", start)) < 0)
                return null;
            return xml.Substring(start, end - start);
        }

        private bool send(string method)
        {
            return send(method, null, null);
        }

        private bool send(string method, string[] keyvalue)
        {
            return send(method, keyvalue, null);
        }

        private bool send(string method, string[] keyvalue, string[] extra)
        {
            string parms = "";
            string url = "https://www.sisow.nl/Sisow/iDeal/RestHandler.ashx/" + method;
            try
            {
                if (keyvalue != null && keyvalue.Length > 0)
                {
                    for (int i = 0; i + 1 < keyvalue.Length; i += 2)
                    {
                        if (string.IsNullOrEmpty(keyvalue[i + 1]))
                            continue;
                        if (!string.IsNullOrEmpty(parms))
                            parms += "&";
                        parms += keyvalue[i] + "=" + System.Web.HttpUtility.UrlEncode(keyvalue[i + 1]);
                    }
                }
                if (extra != null && extra.Length > 0)
                {
                    for (int i = 0; i + 1 < extra.Length; i += 2)
                    {
                        if (string.IsNullOrEmpty(extra[i + 1]))
                            continue;
                        if (!string.IsNullOrEmpty(parms))
                            parms += "&";
                        parms += extra[i] + "=" + System.Web.HttpUtility.UrlEncode(extra[i + 1]);
                    }
                }
                System.Net.HttpWebRequest hwr = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(url);
                hwr.Timeout = 120000;
                hwr.ContentType = "application/x-www-form-urlencoded";
                hwr.Method = "POST";
                hwr.ContentLength = parms.Length;
                System.IO.StreamWriter sw = new System.IO.StreamWriter(hwr.GetRequestStream());
                sw.Write(parms);
                sw.Flush();
                sw.Close();
                System.Net.HttpWebResponse hws = (System.Net.HttpWebResponse)hwr.GetResponse();
                System.IO.StreamReader sr = new System.IO.StreamReader(hws.GetResponseStream());
                response = sr.ReadToEnd();
                hws.Close();
                return true;
            }
            catch (Exception ex)
            {
                response = "";
                errorMessage = ex.Message;
                return false;
            }
        }

        private int getDirectory()
        {
            if (issuerid != null && lastcheck.AddDays(1).CompareTo(DateTime.Now) >= 0)
                return 0;
            if (!send("DirectoryRequest"))
                return -1;
            string search = parse("directory");
            if (string.IsNullOrEmpty(search))
            {
                error();
                return -2;
            }
            string[] iss = search.Replace("<issuer>", "").Split(new string[] { "</issuer>" }, StringSplitOptions.RemoveEmptyEntries);
            issuerid = new string[iss.Length];
            issuername = new string[iss.Length];
            for (int i = 0; i < iss.Length; i++)
            {
                issuerid[i] = parse(iss[i], "issuerid");
                issuername[i] = parse(iss[i], "issuername");
            }
            lastcheck = DateTime.Now;
            return 0;
        }

        // DirectoryRequest
        public int DirectoryRequest(bool test, out string select)
        {
            int ex;

            select = "<select id=\"sisowbank\" name=\"issuerid\">";
            ex = getDirectory();
            if (ex < 0)
                return ex;
            for (int i = 0; i < issuerid.Length; i++)
            {
                select += "<option value=\"" + issuerid[i] + "\">" + issuername[i] + "</option>";
            }
            select += "</select>";
            return 0;
        }

        // DirectoryRequest
        public int DirectoryRequest(bool test, out string[] issuers)
        {
            int ex;

            issuers = null;
            ex = getDirectory();
            if (ex < 0)
                return ex;
            issuers = new string[issuerid.Length * 2];
            for (int i = 0; i < issuerid.Length; i++)
            {
                issuers[i * 2] = issuerid[i];
                issuers[i * 2 + 1] = issuername[i];
            }
            return 0;
        }

        // compute SHA1
        private static string GetSHA1(string key)
        {
            System.Security.Cryptography.SHA1Managed sha = new System.Security.Cryptography.SHA1Managed();
            System.Text.UTF8Encoding enc = new System.Text.UTF8Encoding();
            byte[] bytes = sha.ComputeHash(enc.GetBytes(key));
            //string sha1 = System.BitConverter.ToString(sha1).Replace("-", "");
            string sha1 = "";
            for (int j = 0; j < bytes.Length; j++)
                sha1 += bytes[j].ToString("x2");
            return sha1;
        }

        // TransactionRequest
        public int TransactionRequest(params string[] keyvalue)
        {
            trxId = issuerUrl = "";
            if (string.IsNullOrEmpty(merchantId))
            {
                errorMessage = "No Merchant ID";
                return -1;
            }
            if (string.IsNullOrEmpty(merchantKey))
            {
                errorMessage = "No Merchant Key";
                return -2;
            }
            if (string.IsNullOrEmpty(purchaseId))
            {
                errorMessage = "No purchaseid";
                return -3;
            }
            if (amount < 0.45)
            {
                errorMessage = "amount < 0.45";
                return -4;
            }
            if (string.IsNullOrEmpty(description))
            {
                errorMessage = "No description";
                return -5;
            }
            if (string.IsNullOrEmpty(returnUrl))
            {
                errorMessage = "No returnurl";
                return -6;
            }
            if (string.IsNullOrEmpty(issuerId) && string.IsNullOrEmpty(payment))
            {
                errorMessage = "No iDEAL issuerid or no payment";
                return -7;
            }
            if (string.IsNullOrEmpty(entranceCode))
                entranceCode = purchaseId;
            string sha1 = GetSHA1(purchaseId + entranceCode + (amount * 100).ToString() + Regex.Replace(merchantId, @"\s+", string.Empty) + Regex.Replace(merchantKey, @"\s+", string.Empty));
            string[] pars = { "merchantid", Regex.Replace(merchantId, @"\s+", string.Empty), "payment", payment, "issuerid", issuerId, "purchaseid", purchaseId,
                "amount", Math.Round(amount * 100).ToString(), "description", description, "entrancecode", entranceCode, "returnurl", returnUrl,
                "cancelurl", cancelUrl, "callbackurl", callbackUrl, "notifyurl", notifyUrl, "testmode", (testMode ? "true" : "false"), "sha1", sha1 };
            if (!send("TransactionRequest", pars, keyvalue))
                return -8;
            trxId = parse("trxid");
            if (string.IsNullOrEmpty(trxId))
            {
                error();
                return -2;
            }
            issuerUrl = System.Web.HttpUtility.UrlDecode(parse("issuerurl"));
            invoiceNo = parse("invoiceno");
            long.TryParse(parse("documentid"), out documentId);
            documentUrl = System.Web.HttpUtility.UrlDecode(parse("documenturl"));
            return 0;
        }

        private int GetStatus()
        {
            status = parse("status");
            if (string.IsNullOrEmpty(status))
            {
                error();
                return -5;
            }
            timeStamp = DateTime.Parse(parse("timestamp"));
            amount = long.Parse(parse("amount")) / 100.0;
            consumerAccount = parse("consumeraccount");
            consumerName = parse("consumername");
            consumerCity = parse("consumercity");
            purchaseId = parse("purchaseid");
            description = parse("description");
            entranceCode = parse("entrancecode");
            return 0;
        }

        // StatusRequest
        public int StatusRequest()
        {
            if (string.IsNullOrEmpty(merchantId))
            {
                errorMessage = "No Merchant ID";
                return -1;
            }
            if (string.IsNullOrEmpty(merchantKey))
            {
                errorMessage = "No Merchant Key";
                return -2;
            }
            if (string.IsNullOrEmpty(trxId))
            {
                errorMessage = "No trxid";
                return -3;
            }
            string sha1 = GetSHA1(trxId + Regex.Replace(merchantId, @"\s+", string.Empty) + Regex.Replace(merchantKey, @"\s+", string.Empty));
            string[] pars = { "merchantid", Regex.Replace(merchantId, @"\s+", string.Empty), "trxid", trxId, "sha1", sha1 };
            if (!send("StatusRequest", pars))
                return -4;
            return GetStatus();
        }

        // StatusRequest
        public int StatusRequest(string trxid)
        {
            if (string.IsNullOrEmpty(merchantId))
            {
                errorMessage = "No Merchant ID";
                return -1;
            }
            if (string.IsNullOrEmpty(merchantKey))
            {
                errorMessage = "No Merchant Key";
                return -2;
            }
            if (string.IsNullOrEmpty(trxid))
            {
                errorMessage = "No trxid";
                return -3;
            }
            trxId = trxid;
            string sha1 = GetSHA1(trxId + Regex.Replace(merchantId, @"\s+", string.Empty) + Regex.Replace(merchantKey, @"\s+", string.Empty));
            string[] pars = { "merchantid", Regex.Replace(merchantId, @"\s+", string.Empty), "trxid", trxId, "sha1", sha1 };
            if (!send("StatusRequest", pars))
                return -4;
            return GetStatus();
        }

        // RefundRequest (Sisow iDEAL)
        public long RefundRequest(string trxid)
        {
            trxId = trxid;
            string sha1 = GetSHA1(trxId + Regex.Replace(merchantId, @"\s+", string.Empty) + Regex.Replace(merchantKey, @"\s+", string.Empty));
            string[] pars = { "merchantid", merchantId, "trxid", trxId, "sha1", sha1 };
            if (!send("RefundRequest", pars))
                return -1;
            if (long.TryParse(parse("documentid"), out documentId))
                return documentId;
            return -2;
        }

        // InvoiceRequest (Sisow ecare)
        public int InvoiceRequest(string trxid, params string[] keyvalue)
        {
            trxId = trxid;
            string sha1 = GetSHA1(trxId + Regex.Replace(merchantId, @"\s+", string.Empty) + Regex.Replace(merchantKey, @"\s+", string.Empty));
            string[] pars = { "merchantid", merchantId, "trxid", trxId, "sha1", sha1 };
            if (!send("InvoiceRequest", pars, keyvalue))
                return -1;
            invoiceNo = parse("invoiceno");
            if (string.IsNullOrEmpty(invoiceNo))
            {
                error();
                return -2;
            }
            long.TryParse(parse("documentid"), out documentId);
            documentUrl = System.Web.HttpUtility.UrlDecode(parse("documenturl"));
            return 0;
        }

        // CancelReservationRequest (Sisow ecare)
        public int CancelReservationRequest(string trxid)
        {
            trxId = trxid;
            string sha1 = GetSHA1(trxId + Regex.Replace(merchantId, @"\s+", string.Empty) + Regex.Replace(merchantKey, @"\s+", string.Empty));
            string[] pars = { "merchantid", merchantId, "trxid", trxId, "sha1", sha1 };
            if (!send("CancelReservationRequest", pars))
                return -1;
            status = parse("status");
            if (string.IsNullOrEmpty(status))
            {
                error();
                return -2;
            }
            return 0;
        }

        // CreditInvoiceRequest (Sisow ecare)
        public int CreditInvoiceRequest(string trxid, params string[] keyvalue)
        {
            trxId = trxid;
            string sha1 = GetSHA1(trxId + Regex.Replace(merchantId, @"\s+", string.Empty) + Regex.Replace(merchantKey, @"\s+", string.Empty));
            string[] pars = { "merchantid", merchantId, "trxid", trxId, "sha1", sha1 };
            if (!send("CreditInvoiceRequest", pars, keyvalue))
                return -1;
            invoiceNo = parse("invoiceno");
            if (string.IsNullOrEmpty(invoiceNo))
            {
                error();
                return -2;
            }
            long.TryParse(parse("documentid"), out documentId);
            documentUrl = System.Web.HttpUtility.UrlDecode(parse("documenturl"));
            return 0;
        }
    }
}