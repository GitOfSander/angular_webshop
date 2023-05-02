using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;

namespace Site.Models
{
    public class Error
    {
        private const string URL = "https://system.unveil.nl/api/error";

        public void ReportError(int websiteId, string name, string description, string data, string exceptionData)
        {
            string urlParameters = "?websiteId=" + websiteId + "&name=" + name + "&description=" + description + "&data=" + data + "&exceptionData=" + exceptionData;
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(URL);
            // Add an Accept header for JSON format.
            client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));

            Dictionary<string, object> dic = new Dictionary<string, object>() {
                { "websiteId", websiteId },
                { "name", name },
                { "description", description },
                { "data", data },
                { "exceptionData", exceptionData }
            };

            // List data response.
            var content = new StringContent(JsonConvert.SerializeObject(dic).ToString(), Encoding.UTF8, "application/json");
            HttpResponseMessage response = client.PostAsync(URL, content).Result;  // Blocking call! Program will wait here until a response is received or a timeout occurs.
            if (response.IsSuccessStatusCode)
            {
                //// Parse the response body.
                //var dataObjects = response.Content.ReadAsAsync<IEnumerable<DataObject>>().Result;  //Make sure to add a reference to System.Net.Http.Formatting.dll
                //foreach (var d in dataObjects)
                //{
                //    Console.WriteLine("{0}", d);
                //}
            }
            else
            {
                //Console.WriteLine("{0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
            }

            //Make any other calls using HttpClient here.

            //Dispose once all HttpClient calls are complete. This is not necessary if the containing object will be disposed of; for example in this case the HttpClient instance will be disposed automatically when the application terminates so the following call is superfluous.
            client.Dispose();
        }
    }
}

