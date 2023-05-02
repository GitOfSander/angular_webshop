using System;
using System.IO;
using System.Net;
using System.Text;
using System.Net.Http;
using System.Net.Http.Headers;
using Site.Models.MailChimp.Lists;
using Site.Models.MailChimp.Lists.Members;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using static Site.Startup;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Site.Models.App;
using System.Text.RegularExpressions;

namespace Site.Models.MailChimp
{
    public class MailChimpManager : Controller
    {
        private readonly IOptions<AppSettings> _config;

        #region Fields
        private string _dataCenter;
        private const string ApiUrl = "api.mailchimp.com/3.0/";
        private const string Get = "GET";
        private const string Post = "POST";
        private const string Patch = "PATCH";
        private const string Delete = "DELETE";
        private string _ApiKey;
        private string _ListId;
        #endregion

        public MailChimpManager(IOptions<AppSettings> config, string ApiKey, string ListId)
        {
            _config = config;
            GetApiKey = ApiKey;
            _ListId = ListId;
        }

        #region Properties
        public Error Error { get; private set; }
        public string GetApiKey
        {
            get { return _ApiKey; }
            private set
            {
                _ApiKey = value;
                _dataCenter = value.Split('-')[1];
            }
        }
        #endregion

        public async Task<bool> AddMember(string Status, string Email, string FirstName, string LastName)
        {
            var member = new Member()
            {
                EmailAddress = Email,
                Status = Status,
                MergeFields = new Dictionary<string, string>()
                {
                    { "FNAME", FirstName},
                    { "LNAME", LastName},
                },
            };

            var memberNew = await CreateMember(Regex.Replace(_ListId, @"\s+", string.Empty), member);
            if (memberNew)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #region Method
        private async Task<T> DoRequests<T>(string endPoint, string httpMethod, string payload = null)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", string.Format("{0} {1}", "whatever", ApiKey));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Regex.Replace(GetApiKey, @"\s+", string.Empty));
                    client.BaseAddress = new Uri(endPoint);


                    HttpRequestMessage request = new HttpRequestMessage();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    switch (httpMethod)
                    {
                        case Patch:
                            request.Method = HttpMethod.Post;
                            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("X-Http-Method-Override"));
                            break;
                        case Post:
                            request.Method = HttpMethod.Post;
                            break;
                        case Get:
                            request.Method = HttpMethod.Post;
                            break;
                        case Delete:
                            request.Method = HttpMethod.Post;
                            break;
                    }

                    //request.Cont
                    //SET ApiUrl AND CONTENT TYPE
                    HttpResponseMessage response = new HttpResponseMessage();

                    if (!string.IsNullOrEmpty(payload) && (httpMethod == Post || httpMethod == Patch))
                    {
                        request.Content = new StringContent(payload, Encoding.UTF8, "application/json");

                        //using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                        //{
                        //    streamWriter.Write(payload);
                        //}
                    }

                    string result;
                    try
                    {
                        //response
                        //var statusCode = (int)response.StatusCode;
                        //using (var streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
                        //{
                        //    result = streamReader.ReadToEnd();
                        //}
                        //if (statusCode == 200) { return JsonConvert.DeserializeObject<T>(result); }
                        //if (statusCode == 204) { return (T)Convert.ChangeType(true, typeof(T));

                        response = await client.SendAsync(request);
                        var statusCode = (int)response.StatusCode;
                        Stream streamTask = await response.Content.ReadAsStreamAsync();

                        using (var sr = new StreamReader(streamTask))
                        {
                            if (statusCode == 200) { return (T)Convert.ChangeType(true, typeof(T)); }
                            //if (statusCode == 200)
                            //{
                            //    using (var jsonTextReader = new JsonTextReader(sr))
                            //    {
                            //        return (T)Convert.ChangeType(true, typeof(T));
                            //    }
                            //}
                        }

                        if (statusCode == 204) { return (T)Convert.ChangeType(true, typeof(T)); }

                        //using (Task<Stream> streamTask = request.Content.ReadAsStreamAsync())
                        //{
                        //    var statusCode = (int)response.StatusCode;

                        //}
                    }
                    catch (WebException we)
                    {
                        new Models.Error().ReportError(_config.Value.SystemWebsiteId, "MAILCHIMPMANAGER#01", "DoRequests kon niet uitgevoerd worden.", "", we.Message);

                        using (var httpWebResponse = (HttpWebResponse)we.Response)
                        {
                            if (httpWebResponse == null)
                            {
                                Error = new Error { Type = we.Status.ToString(), Detail = we.Message };
                                return default(T);
                            }
                            using (var streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
                            {
                                result = streamReader.ReadToEnd();
                            }
                            Error = JsonConvert.DeserializeObject<Error>(result);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                new Models.Error().ReportError(_config.Value.SystemWebsiteId, "MAILCHIMPMANAGER#02", "DoRequests kon niet uitgevoerd worden.", "", e.Message);
            }
            return default(T);
        }

        private string GetQueryParams(string payload)
        {
            if (payload == null) { return ""; }
            var jObject = JObject.Parse(payload);
            var sbQueryString = new StringBuilder();
            foreach (var obj in jObject)
            {
                if (obj.Value.Type == JTokenType.Array)
                {
                    sbQueryString.AppendFormat("{0}=", obj.Key);
                    foreach (var arrayValue in obj.Value)
                    {
                        sbQueryString.AppendFormat("{0}{1}", arrayValue, arrayValue.Next != null ? "," : "&");
                    }
                    continue;
                }
                sbQueryString.AppendFormat("{0}={1}&", obj.Key, obj.Value);
            }
            if (sbQueryString.Length > 0)
            {
                sbQueryString.Insert(0, "?");
                sbQueryString.Remove(sbQueryString.Length - 1, 1);
            }
            return sbQueryString.ToString();
        }
        #endregion

        #region List:
        public async Task<List> ReadList(string listId, ListQuery listQuery = null)
        {
            string queryString = null;
            if (listQuery != null)
            {
                var payload = JsonConvert.SerializeObject(listQuery, Formatting.None, new JsonSerializerSettings
                {
                    DefaultValueHandling = DefaultValueHandling.Ignore
                });
                queryString = GetQueryParams(payload);
            }
            var endPoint = string.Format("https://{0}.{1}lists/{2}/{3}", _dataCenter, ApiUrl, listId, queryString);
            return await DoRequests<List>(endPoint, Get);
        }

        public async Task<List> EditList(string listId, List list)
        {
            var payload = JsonConvert.SerializeObject(list, Formatting.None, new JsonSerializerSettings
            {
                DefaultValueHandling = DefaultValueHandling.Ignore
            });
            var endPoint = string.Format("https://{0}.{1}lists/{2}", _dataCenter, ApiUrl, listId);
            return await DoRequests<List>(endPoint, Patch, payload);
        }

        public async Task<List> CreateList(List list)
        {
            var payload = JsonConvert.SerializeObject(list, Formatting.None, new JsonSerializerSettings
            {
                DefaultValueHandling = DefaultValueHandling.Ignore
            });
            var endPoint = string.Format("https://{0}.{1}lists/", _dataCenter, ApiUrl);
            return await DoRequests<List>(endPoint, Post, payload);
        }

        public async Task<bool> DeleteList(string listId)
        {
            var endPoint = string.Format("https://{0}.{1}lists/{2}/", _dataCenter, ApiUrl, listId);
            return await DoRequests<bool>(endPoint, Delete);
        }

        public async Task<CollectionList> ReadLists(CollectionListQuery listListsQuery = null)
        {
            string queryString = null;
            if (listListsQuery != null)
            {
                var payload = JsonConvert.SerializeObject(listListsQuery, Formatting.None, new JsonSerializerSettings
                {
                    DefaultValueHandling = DefaultValueHandling.Ignore
                });
                queryString = GetQueryParams(payload);
            }
            var endPoint = string.Format("https://{0}.{1}lists/{2}", _dataCenter, ApiUrl, queryString);
            return await DoRequests<CollectionList>(endPoint, Get);
        }
        #endregion

        #region Member
        public async Task<bool> ReadMember(string listId, string subscriberHash, MemberQuery memberQuery = null)
        {
            string queryString = null;
            if (memberQuery != null)
            {
                var payload = JsonConvert.SerializeObject(memberQuery, Formatting.None, new JsonSerializerSettings
                {
                    DefaultValueHandling = DefaultValueHandling.Ignore
                });
                queryString = GetQueryParams(payload);
            }
            var endPoint = string.Format("https://{0}.{1}lists/{2}/members/{3}/{4}", _dataCenter, ApiUrl, listId, subscriberHash, queryString);
            return await DoRequests<bool>(endPoint, Get);
        }
        public async Task<bool> DeleteMember(string listId, string subscriberHash)
        {
            var endPoint = string.Format("https://{0}.{1}lists/{2}/members/{3}", _dataCenter, ApiUrl, listId, subscriberHash);
            return await DoRequests<bool>(endPoint, Delete);
        }
        public async Task<bool> CreateMember(string listId, Member member)
        {
            var payload = JsonConvert.SerializeObject(member, Formatting.None, new JsonSerializerSettings
            {
                DefaultValueHandling = DefaultValueHandling.Ignore
            });
            var endPoint = string.Format("https://{0}.{1}lists/{2}/members/", _dataCenter, ApiUrl, listId);
            return await DoRequests<bool>(endPoint, Post, payload);
        }
        public async Task<Member> EditMember(string listId, string subscriberHash, Member member)
        {
            var payload = JsonConvert.SerializeObject(member, Formatting.None, new JsonSerializerSettings
            {
                DefaultValueHandling = DefaultValueHandling.Ignore
            });
            var endPoint = string.Format("https://{0}.{1}lists/{2}/members/{3}", _dataCenter, ApiUrl, listId, subscriberHash);
            return await DoRequests<Member>(endPoint, Patch, payload);
        }
        //public async Task<Member> CreateOrEditMember(string listId, Member member, bool isForce = false)
        //{
        //    var memberNew = await Task.Run(() => CreateMember(listId, member));
        //    if (memberNew == null && Error.Status == 400 && Error.Title == "Member Exists")
        //    {
        //        memberNew = await Task.Run(() => EditMember(listId, member.GetSubscriberHash(), member));
        //        if (memberNew != null)
        //        {
        //            return memberNew;
        //        }
        //    }
        //    return null;
        //}
        public async Task<CollectionMember> ReadMembers(string listId, CollectionMemberQuery memberListsQuery = null)
        {
            string queryString = null;
            if (memberListsQuery != null)
            {
                var payload = JsonConvert.SerializeObject(memberListsQuery, Formatting.None, new JsonSerializerSettings
                {
                    DefaultValueHandling = DefaultValueHandling.Ignore
                });
                queryString = GetQueryParams(payload);
            }
            var endPoint = string.Format("https://{0}.{1}lists/{2}/members/{3}", _dataCenter, ApiUrl, listId, queryString);
            return await DoRequests<CollectionMember>(endPoint, Get);
        }
        public async Task<CollectionMember> GetAllSubscribedMembers(string listId)
        {
            var memberListQuery = new CollectionMemberQuery()
            {
                Status = "subscribed"
            };

            return await ReadMembers(listId, memberListQuery);
        }
        public async Task<CollectionMember> CollectionMemberQuery(string listId)
        {
            var memberListQuery = new CollectionMemberQuery()
            {
                Status = "unsubscribed"
            };

            return await ReadMembers(listId, memberListQuery);
        }
        public async Task<CollectionMember> GetAllCleanedMembers(string listId)
        {
            var memberListQuery = new CollectionMemberQuery()
            {
                Status = "cleaned"
            };

            return await ReadMembers(listId, memberListQuery);
        }
        #endregion
    }
}