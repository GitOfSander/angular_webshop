using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Site.Models.App;
using Site.Models.SendCloud;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Site.Models.SendCloud
{
    public class SendCloudClient
    {
        private readonly IOptions<AppSettings> _config;

        const string hostUrl = "https://panel.sendcloud.sc/api/v2/";
        /*
            API information
            API Key: Your public api key
            API Secret: Your secret api key
            Partner uuid: Your SendCloud partner uuid
        */
        protected string ApiKey;
        protected string ApiSecret;
        protected string PartnerUuid;
        /**
	     * @var SendCloudApiParcelResource
	     */
        public SendCloudApiParcelsResource Parcels;
        /**
	     * @var SendCloudApiParcelStatusResource
	     */
        public SendCloudApiParcelStatusResource ParcelStatuses;
        /**
	     * @var SendCloudApiShippingResource
	     */
        public SendCloudApiShippingResource ShippingMethods;
        /**
	     * @var SendCloudApiUserResource
	     */
        public SendCloudApiUserResource User;
        /**
	     * @var SendCloudApiLabelResource
	     */
        public SendCloudApiLabelResource Label;
        /**
	     * @var SendCloudInvoicesResource
	     */
        public SendCloudApiInvoicesResource Invoices;
        /**
	     * @var SendCloudSenderAddressResource
	     */
        public SendCloudApiSenderAddressResource SenderAddresses;
        protected string ApiUrl;

        private string Mode;

        /**
	     * Making sure the right construct function gets called
	     */
        public SendCloudClient()
        {
            // Check if mode is set by extended class, if not fall back to naming
            if (!String.IsNullOrEmpty(Mode))
            {
                // Set mode to classname, strip namespaces :)
                string[] functionCall = this.GetType().Name.ToLower().Split('\\');
                Mode = functionCall.Last();
            }
            //      var a = func_get_args();
            //var i = func_num_args();
            //      var f = ""
            //      if (method_exists(this.GetType().Name.ToLower(), var f = 'SendCloud'.i))
            //      {
            //          call_user_func_array(array($this,$f),a);
            //      }

            //      string[] functionCall = this.GetType().Name.ToLower().Split('\\');
            //      functionCall.Last();
        }

        /**
         * Wrapper constructor
         * @param string $api_key
         * @param string $api_secret
         * @return void
         */
        public SendCloudClient(IOptions<AppSettings> config, string apiKey, string apiSecret, string partnerUuid = "")
        {
            _config = config;
            SetApiKeys(apiKey, apiSecret);
            PartnerUuid = partnerUuid;
            ApiUrl = hostUrl;

            SetupResources();
        }

        public bool HasMethod(Dictionary<string, object> objectToCheck, string methodName)
        {
            var type = objectToCheck.GetType();
            return type.GetMethod(methodName) != null;
        }

        public object CallUserFuncArray(string className, string methodName, params object[] args)
        {
            return Type.GetType(className).GetMethod(methodName).Invoke(null, args);
        }

        public void SetApiUrl(string url)
        {
            ApiUrl = url;
        }

        /**
        * Sets the API-Key and API-Secret
        * @param string $api_key
        * @param string $api_secret
        * @return void
        * @throws SendCloudApiException Exception thrown if one of the arguments are not passed
        */
        private void SetApiKeys(string apiKey = "", string apiSecret = "")
        {
            if (apiKey != "" || apiSecret != "")
            {
                ApiKey = apiKey;
                ApiSecret = apiSecret;
            }
            else
            {
                throw new SendCloudApiException("You must have an API key and an API secret key");
            }
        }

        /**
            * Internal method that initializes Objects
            * @return void
            */
        private void SetupResources()
        {
            Parcels = new SendCloudApiParcelsResource();
            ParcelStatuses = new SendCloudApiParcelStatusResource();
            ShippingMethods = new SendCloudApiShippingResource();
            User = new SendCloudApiUserResource();
            Label = new SendCloudApiLabelResource();
            Invoices = new SendCloudApiInvoicesResource();
            SenderAddresses = new SendCloudApiSenderAddressResource();
        }
        /**
        * Returns API-Key
        * @return string
        */
        public string GetApiKey()
        {
            return ApiKey;
        }

        /**
        * Returns API-Secret
        * @return string
        */
        public string GetApiSecret()
        {
            return ApiSecret;
        }

        /**
         * Sends the request to create the requested object on the server
         * @param array $object
         * @return array
         */
        public async Task<Dictionary<string, object>> CreateAsync(Dictionary<string, object> obj, string createResource, string resource)
        {
            Dictionary<string, object> data = new Dictionary<string, object>() {
                { createResource, obj }
            };
            return await CreateAsync(resource, data, createResource);
        }

        /**
        * Creates an object
        * @param string $url
        * @param array $post
        * @param array $return_object
        * @return object
        */
        public async Task<Dictionary<string, object>> CreateAsync(string url, Dictionary<string, object> post, object returnObject)
        {
            return await SendRequestAsync(url, "post", post, returnObject);
        }

        /**
        * Sends the request to get the requested object from the server
        * @param array $object
        * @return array
        */
        public async Task<Dictionary<string, object>> GetAsync(Dictionary<string, object> obj, string getResource, string resource)
        {
            Dictionary<string, object> data = new Dictionary<string, object>() {
                { getResource, obj }
            };
            return await GetAsync(resource, data, getResource);
        }

        /**
        * Sends the request to get the requested object from the server
        * @return array
        */
        public async Task<Dictionary<string, object>> GetAsync(string resource)
        {
            return await GetAsync(resource, null, null);
        }

        /**
        * Gets one or more objects
        * @param string $url
        * @param array $params
        * @param array $return_object
        * @return object
        */
        public async Task<Dictionary<string, object>> GetAsync(string url, Dictionary<string, object> par, object returnObjects)
        {
            return await SendRequestAsync(url, "get", par, returnObjects);
        }


        /**
        * Updates an object
        * @param string $url
        * @param array $params
        * @param array $return_object
        * @return object
        */
        public async Task<Dictionary<string, object>> UpdateAsync(string url, Dictionary<string, object> par, object returnObjects)
        {
            return await SendRequestAsync(url, "put", par, returnObjects);
        }

        public async Task<Dictionary<string, object>> CancelAsync(int objectId = 0)
        {
            if (objectId != 0)
            {
                return await CancelParcelAsync("parcels" + "/" + objectId + "/cancel/");
            }

            return null;
        }

        /**
        * Updates an object
        * @param string $url
        * @return object
        */
        public async Task<Dictionary<string, object>> CancelParcelAsync(string url)
        {
            return await SendRequestAsync(url, "post", new Dictionary<string, object>(), null);
        }

        /**
        * Generates the url that can be used for the call
        * @param string $url
        * @param array $params
        * @return string
        */
        public string GetUrl(string url, Dictionary<string, object> par = null)
        {
            string apiUrl = ApiUrl;
            Uri apiParsed = new Uri(apiUrl);
            Uri resourceUrl = new Uri(apiParsed.Scheme + "://" + apiParsed.Host + "/" + url);
            // When the port is in the parsed url, add the port.
            string port = (apiParsed.Port != 0 ? ":" + apiParsed.Port : "");
            apiUrl = apiParsed.Scheme + "://" + GetApiKey() + ":" + GetApiSecret() + "@" + apiParsed.Host + port + "/";
            char[] c = { '/' };
            if (!string.IsNullOrEmpty(apiParsed.LocalPath))
            {
                apiUrl += apiParsed.LocalPath.Trim(c);
            }

            apiUrl += resourceUrl.LocalPath;
            if (!string.IsNullOrEmpty(resourceUrl.Query))
            {
                apiUrl += "?" + resourceUrl.Query;
            }
            else if (par != null)
            {
                Dictionary<string, object> queryParameters = new Dictionary<string, object>();
                foreach (var item in par)
                {
                    Type t = item.Value.GetType();
                    bool isDict = t.IsConstructedGenericType && t.GetGenericTypeDefinition() == typeof(Dictionary<,>);
                    if (!t.IsConstructedGenericType && t.GetGenericTypeDefinition() != typeof(Dictionary<,>))
                    {
                        queryParameters.Add(item.Key, WebUtility.UrlEncode(item.Value.ToString()));
                    }
                }

                string query = String.Join("&", queryParameters.Values);
                if (!string.IsNullOrEmpty(query))
                {
                    apiUrl += "?" + query;
                }
            }

            return apiUrl;
        }

        /**
        * Sends the API Request to SendCloud and returns the response body
        * @param string $url
        * @param string $method
        * @param object $object
        * @param object $return_object
        * @return object
        * @throws SendCloudApiException Exception thrown if $object isn't an object
        * @throws SendCloudApiException Exception thrown if server returns an error
        */
        public async Task<Dictionary<string, object>> SendRequestAsync(string url, string method, Dictionary<string, object> obj, object returnObject)
        {
            object result;

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(GetUrl(url, obj));
            httpWebRequest.ContentType = "application/json; charset=utf-8";
            httpWebRequest.Method = method.ToUpper();
            string encoded = Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1").GetBytes(Regex.Replace(ApiKey, @"\s+", string.Empty) + ":" + Regex.Replace(ApiSecret, @"\s+", string.Empty)));
            httpWebRequest.Headers.Add("Authorization", "Basic " + encoded);
            if (method == "post" || method == "put")
            {
                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    string json = JsonConvert.SerializeObject(obj);

                    streamWriter.Write(json);
                    streamWriter.Flush();
                    streamWriter.Close();
                }

                try
                {
                    var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        result = streamReader.ReadToEnd();
                    }
                }
                catch (Exception e)
                {
                    Console.Write(e);
                    new Error().ReportError(_config.Value.SystemWebsiteId, "SENDCLOUDCLIENT#01", "SendRequestAsync kon niet uitgevoerd worden.", "", e.Message);

                    result = "";
                }

            }
            else
            {
                try
                {
                    using (WebResponse response = httpWebRequest.GetResponse())
                    {
                        using (var streamReader = new StreamReader(response.GetResponseStream()))
                        {
                            result = streamReader.ReadToEnd();
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.Write(e);
                    new Error().ReportError(_config.Value.SystemWebsiteId, "SENDCLOUDCLIENT#02", "SendRequestAsync kon niet uitgevoerd worden.", "", e.Message);

                    result = "";
                }
            }

            return JsonConvert.DeserializeObject<Dictionary<string, object>>(result.ToString());
        }

        /**
	     * Internal method that is fired when the server returns an error
	     * @param int $response_code
	     * @param string $response_body
	     * @return void
	     * @throws SendCloudApiException Exception containing the response error received from the server
	     */
        //private void HandleResponseError(int responseCode, string responseBody) {
        // $error = $response_body["error"];
        // if (!is_array($error) || !array_key_exists("message", $error)) {
        //  switch($response_code) {
        //   case 404:
        //    $message = "Page not found.";
        //    break;
        //   default:
        //    $message = "Unknown error";
        //  }
        // } else {
        //  $message = $error["message"];
        // }
        // if (!is_array($error) || !array_key_exists("code", $error)) {
        //  $code = -99;
        // } else {
        //  $code = $error["code"];
        // }
        // throw new SendCloudApiException($message, $code);
        //}
    }
}


//if (!function_exists('json_last_error_msg')) {
//	function json_last_error_msg()
//{
//    static $ERRORS = array(
//        JSON_ERROR_NONE => 'No error',
//        JSON_ERROR_DEPTH => 'Maximum stack depth exceeded',
//        JSON_ERROR_STATE_MISMATCH => 'State mismatch (invalid or malformed JSON)',
//        JSON_ERROR_CTRL_CHAR => 'Control character error, possibly incorrectly encoded',
//        JSON_ERROR_SYNTAX => 'Syntax error',
//        JSON_ERROR_UTF8 => 'Malformed UTF-8 characters, possibly incorrectly encoded'
//    );
//		$error = json_last_error();
//    return isset($ERRORS[$error]) ? $ERRORS[$error] : 'Unknown error';
//}
//}


/**
 * @version 2.0.0
 * @package SendCloud
 * @see https://docs.sendcloud.sc/api/v2/index.html
 */
public class SendCloudApiException : Exception
{
    public string Msg;
    public int Code;

    public SendCloudApiException(string msg, int code = 0)
    {
        Msg = msg;
        Code = code;
    }
}

/**
 * @version 2.0.0
 * @package SendCloud
 * @see https://docs.sendcloud.sc/api/v2/index.html
 */
public class SendCloudApiAbstractResource
{
    /**
	 * @var WebshopappApiClient
	 */
    protected Dictionary<string, object> _client;
    /**
	* Settings to other classes
	**/
    protected bool _createRequest = true;
    protected bool _getRequest = true;
    protected bool _updateRequest = true;
    protected string _singleResource = "";
    protected string _listResource = "";
    protected string _createResource = "";
    protected string _updateResource = "";
    protected string _resource = "";

    public SendCloudApiAbstractResource(Dictionary<string, object> client = null)
    {
        _client = client;
    }

    /**
	 * Sends the request to create the requested object on the server
	 * @param array $object
	 * @return array
	 */
    public async Task<Dictionary<string, object>> CreateAsync(Dictionary<string, object> obj)
    {
        if (_createRequest)
        {
            Dictionary<string, object> data = new Dictionary<string, object>() {
                { _createResource, obj }
            };
            return await new SendCloudClient().CreateAsync(_resource, data, _createResource);
        }

        return null;
    }



    /**
	 * Sends the request to update an object on the server
	 * @param int $object_id
	 * @param array $data
	 * @return array
	 */

    //  public async Task<Dictionary<string, object>> UpdateAsync(Dictionary<string, object> data, int objectId = 0)
    //  {
    //if (_updateRequest) {
    //	if (objectId  != 0) {
    //              Dictionary<string, object> fields = new Dictionary<string, object>() {
    //                  { _updateResource, data }
    //              };
    //		return await new SendCloud().UpdateAsync(_resource + "/" + objectId, fields, _updateResource, new Dictionary<string, object>(), "");
    //	}
    //}

    //      return null;
    //  }
}

/**
 * @version 2.0.0
 * @package SendCloud
 * @see https://docs.sendcloud.sc/api/v2/index.html
 */
public class SendCloudApiParcelsResource : SendCloudApiAbstractResource
{
    protected new string _resource = "parcels";
    protected new string _createResource = "parcel";
    protected new string _updateResource = "parcel";
    protected new string _listResource = "parcels";
    protected new string _singleResource = "parcel";
    /**
	 * Sends the request to create multiple parcels to the Server
	 *
	 * Example $object:
	 * array(
	 * 		array(
	 * 			'name' => 'John Doe',
	 * 			'company_name' => 'Stationsweg 20'
	 * 			....
	 * 		),
	 * 		array(
	 * 			'name' => 'Jan Smit',
	 * 			'company_name' => 'Stadhuisplein 10'
	 * 			....
	 * 		)
	 * );
	 *
	 * @param array $object
	 * @return array
	 */

    public async Task<Dictionary<string, object>> CreateBulkAsync(Dictionary<string, object> obj)
    {
        if (_createRequest)
        {
            Dictionary<string, object> data = new Dictionary<string, object>() {
                { "parcels", obj }
            };
            return await new SendCloudClient().CreateAsync("parcels", data, "parcels");
        }

        return null;
    }

    /**
	 * Override the default update method from the `SendCloudApiAbstractResource`
	 * in order to match the API documentation. Parcel updates should use the
	 * `/parcels/` endpoint with the resource ID included in the payload.
	 */
    public async Task<Dictionary<string, object>> UpdateAsync(Dictionary<string, object> data, int objectId = 0)
    {
        if (_updateRequest && objectId != 0)
        {
            data.Add("id", objectId);
            Dictionary<string, object> fields = new Dictionary<string, object>() {
                { "parcel", data }
            };
            return await new SendCloudClient().UpdateAsync("parcels", fields, "parcel");
        }

        return null;
    }
}
/**
 * @version 2.0.0
 * @package SendCloud
 * @see https://docs.sendcloud.sc/api/v2/index.html
 */
public class SendCloudApiLabelResource : SendCloudApiAbstractResource
{
    protected new string _resource = "labels";
    protected new string _listResource = "label";
    protected new string _singleResource = "label";
    protected new string _createResource = "label";
    protected new bool _createRequest = true;
}


/**
 * @version 2.0.0
 * @package SendCloud
 * @see https://docs.sendcloud.sc/api/v2/index.html
 */
public class SendCloudApiUserResource : SendCloudApiAbstractResource
{
    protected new string _resource = "user";
    protected new string _listResource = "user";
    protected new string _singleResource = "user";
    protected new bool _createRequest = false;
    protected new bool _updateRequest = false;
}


/**
 * @version 2.0.0
 * @package SendCloud
 * @see https://docs.sendcloud.sc/api/v2/index.html
 */
public class SendCloudApiShippingResource : SendCloudApiAbstractResource
{
    protected new string _resource = "shipping_methods";
    protected new string _listResource = "shipping_methods";
    protected new string _singleResource = "shipping_method";
    protected new bool _createRequest = false;
    protected new bool _updateRequest = false;
}


/**
 * @version 2.0.0
 * @package SendCloud
 * @see https://docs.sendcloud.sc/api/v2/index.html
 */
public class SendCloudApiParcelStatusResource : SendCloudApiAbstractResource
{
    protected new string _resource = "parcels/statuses";
    protected new string _listResource = "";
    protected new string _singleResource = "";
    protected new bool _createRequest = false;
    protected new bool _updateRequest = false;
}


/**
 * @version 2.0.0
 * @package SendCloud
 * @see https://docs.sendcloud.sc/api/v2/index.html
 */
public class SendCloudApiInvoicesResource : SendCloudApiAbstractResource
{
    protected new string _resource = "user/invoices";
    protected new string _listResource = "invoices";
    protected new string _singleResource = "invoice";
    protected new bool _createRequest = false;
    protected new bool _updateRequest = false;
}

/**
 * @version 2.0.0
 * @package SendCloud
 * @see https://docs.sendcloud.sc/api/v2/index.html
 */
public class SendCloudApiSenderAddressResource : SendCloudApiAbstractResource
{
    protected new string _resource = "user/addresses/sender";
    protected new string _listResource = "sender_addresses";
    protected new string _singleResource = "sender_address";
    protected new bool _createRequest = false;
    protected new bool _updateRequest = false;
}