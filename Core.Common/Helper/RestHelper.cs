using Core.Common.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Core.Common.Helper
{
    /// <summary>
    /// RestHelper帮助类
    /// </summary>
    public class RestHelper
    {
        /// <summary>
        /// 执行Post请求
        /// <param name="serviceName">服务名称</param>
        /// <param name="request">Rest请求对象</param> 
        /// <param name="userName">BrowserStack User Name</param>
        /// <param name="accessKey">BrowserStack Access Key</param>
        /// </summary>
        public static T ExecutePost<T>(string serviceName, RestRequest request, string userName = "default", string accessKey = "default") where T : class, new()
        {
            var restClient = new RestClient(GetBaseUrl(serviceName))
            {
                Authenticator = new HttpBasicAuthenticator(userName, accessKey)
            };

            request.Method = Method.POST;

            request.AddHeader("Content-Type", "application/json");

            var response = restClient.Execute<T>(request);

            if (response.ErrorException != null)
            {
                const string message = "Error retrieving response.  Check inner details for more info.";
                var browserStackException = new ApplicationException(message, response.ErrorException);
                throw browserStackException;
            }
            return response.Data;
        }

        /// <summary>
        /// 异步执行Post请求
        /// </summary>
        /// <param name="serviceName">服务名称</param>
        /// <param name="request"></param>
        /// <param name="userName"></param>
        /// <param name="accessKey"></param>
        /// <returns></returns>
        public static Task<IRestResponse> ExecutePostAsync(string serviceName, RestRequest request, string userName = "default", string accessKey = "default")
        {
            var restClient = new RestClient(GetBaseUrl(serviceName))
            {
                Authenticator = new HttpBasicAuthenticator(userName, accessKey)
            };

            request.Method = Method.POST;

            request.AddHeader("Content-Type", "application/json");


            var tcs = new TaskCompletionSource<IRestResponse>();
            var rrah = restClient.ExecuteAsync(request, response => {
                tcs.SetResult(response);
            });

            return tcs.Task;
        }


        /// <summary>
        /// 执行Get请求
        /// <param name="serviceName">服务名称</param>
        /// <param name="request">Rest请求对象</param>
        /// <param name="userName">BrowserStack User Name</param>
        /// <param name="accessKey">BrowserStack Access Key</param>
        /// </summary>
        public static T ExecuteGet<T>(string serviceName, RestRequest request, string userName = "default", string accessKey = "default") where T : class, new()
        {
            var restClient = new RestClient(GetBaseUrl(serviceName))
            {
                Authenticator = new HttpBasicAuthenticator(userName, accessKey)
            };

            request.Method = Method.GET;

            request.AddHeader("Content-Type", "application/json");

            var response = restClient.Execute<T>(request);

            if (response.ErrorException != null)
            {
                const string message = "Error retrieving response.  Check inner details for more info.";
                var browserStackException = new ApplicationException(message, response.ErrorException);
                throw browserStackException;
            }
            return response.Data;
        }

        /// <summary>
        /// 异步执行Get请求
        /// </summary>
        /// <param name="serviceName">服务名称</param>
        /// <param name="request"></param>
        /// <param name="userName"></param>
        /// <param name="accessKey"></param>
        /// <returns></returns>
        public static Task<IRestResponse> ExecuteGetAsync(string serviceName, RestRequest request, string userName = "default", string accessKey = "default")
        {
            var restClient = new RestClient(GetBaseUrl(serviceName))
            {
                Authenticator = new HttpBasicAuthenticator(userName, accessKey)
            };

            request.Method = Method.GET;

            request.AddHeader("Content-Type", "application/json");

            var tcs = new TaskCompletionSource<IRestResponse>();
            var rrah = restClient.ExecuteAsync(request, response => {
                tcs.SetResult(response);
            });

            return tcs.Task;
        }

        /// <summary>
        /// 根据服务名获取服务地址
        /// </summary>
        /// <param name="serviceName">服务名称</param>
        /// <returns></returns>
        private static string GetBaseUrl(string serviceName)
        {
            return ConfigHelper.GetSetting("ConnectionStrings:" + serviceName).ToString();

            //using (var consul = new Consul.ConsulClient(c =>
            //{
            //    c.Address = new Uri("http://192.168.3.233:8500"); //Consul地址
            //}))
            //{
            //    var services = consul.Catalog.Service(serviceName).Result.Response;

            //    foreach (var s1 in services)
            //    {
            //        Console.WriteLine($"ID={s1.ServiceID},Service={s1.ServiceName},Addr={s1.Address},Port={s1.ServicePort}");
            //    }
            //}
        }

        /*
        /// <summary>
        /// 模拟请求API接口
        /// </summary>
        /// <param name="url">API接口地址</param>
        /// <param name="dic">字典类型入参</param>
        /// <param name="method">请求类型：GET/POST</param>
        /// <returns></returns>
        public static IRestResponse<Response> GetResponseData(string serviceName, string url, Dictionary<string, object> dic = null, Method method = Method.GET)
        {
            var client = new RestClient(ConfigHelper.GetSetting("ConnectionStrings:" + serviceName).ToString());
            //client.Encoding = Encoding.GetEncoding("gb2312");
            //client.AddDefaultHeader("Content-Type", "text/html;charset=gb2312");
            var request = new RestRequest(url, method);

            if (dic != null)
            {
                foreach (var item in dic)
                {
                    request.AddParameter(item.Key, item.Value);
                }
            }
            return client.Execute<Response>(request);
        }

        /// <summary>
        /// Generic Post method
        /// <param name="request">Restsharp request object</param> 
        /// <param name="obj">Object that you want to serialize</param>
        /// <param name="userName">BrowserStack User Name</param>
        /// <param name="accessKey">BrowserStack Access Key</param>
        /// </summary>
        public static T ExecutePost<T, K>(string serviceName, RestRequest request, K obj, string userName = "default", string accessKey = "default") where T : class, new()
        {
            var restClient = new RestClient(ConfigHelper.GetSetting("ConnectionStrings:" + serviceName).ToString())
            {
                Authenticator = new HttpBasicAuthenticator(userName, accessKey)
            };

            request.Method = Method.POST;

            request.AddHeader("Content-Type", "application/json");

            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,

                Converters = new List<JsonConverter>
                {
                    new IsoDateTimeConverter()
                    {
                        DateTimeFormat= "yyyy-MM-dd HH:mm:ss"
                    }
                }
            };

            var myContentJson = JsonConvert.SerializeObject(obj, settings);

            request.AddParameter("application/json", myContentJson, ParameterType.RequestBody);

            var response = restClient.Execute<T>(request);

            //To make the call async
            //var cancellationTokenSource = new CancellationTokenSource();
            //var response2 = restClient.ExecuteTaskAsync<T>(request, cancellationTokenSource.Token);

            if (response.ErrorException != null)
            {
                const string message = "Error retrieving response.  Check inner details for more info.";
                var browserStackException = new ApplicationException(message, response.ErrorException);
                throw browserStackException;
            }
            return response.Data;
        }
        */
    }
}
