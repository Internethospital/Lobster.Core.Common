using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Common.Messge
{
    /// <summary>
    /// 请求方式
    /// </summary>
    public enum HTTPMethod
    {
        GET,
        POST,
        HEAD,
        PATCH,
        PUT,
        DLETE,
        OPTIONS
    }


    /// <summary>
    /// 短信平台配置
    /// </summary>
    public class MessageConfig
    {
        public string appid;
        public string appkey;
        public string url;
        public string method;
        /// <summary>
        /// 短信配置
        /// </summary>
        /// <param name="appid">appid</param>
        /// <param name="appkey">appkey</param>
        public MessageConfig(string appid, string appkey)
        {
            this.appid = appid;
            this.appkey = appkey;
        }
        /// <summary>
        /// 短信配置
        /// </summary>
        /// <param name="appid">appid</param>
        /// <param name="appkey">appkey</param>
        /// <param name="httpMethod">请求方式</param>
        /// <param name="url">发送地址</param>
        public MessageConfig(string appid, string appkey, HTTPMethod httpMethod, string url)
        {
            this.appid = appid;
            this.appkey = appkey;
            this.url = url;
            this.method = httpMethod.ToString();
        }
    }
}
