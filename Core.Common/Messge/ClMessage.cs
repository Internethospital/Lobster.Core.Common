using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Common.Messge
{
    /// <summary>
    /// 创蓝短信
    /// </summary>
    public class ClMessage : MessageConfig, IMessage
    {
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="appid"></param>
        /// <param name="appkey"></param>
        /// <param name="httpMethod">请求方式</param>
        /// <param name="url"></param>
        public ClMessage(string appid, string appkey, HTTPMethod httpMethod, string url) : base(appid, appkey, httpMethod, url)
        {

        }
        /// <summary>
        /// 发送短信
        /// </summary>
        /// <param name="phoneNumber">手机号码</param>
        /// <param name="msg">消息</param>
        /// <param name="extend">扩展(选填参数)</param>
        /// <param name="ext">说明(选填参数)</param>
        /// <returns></returns>
        public dynamic send(string phoneNumber, string msg, string extend, string ext)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 发送短息
        /// </summary> 
        /// <param name="phoneNumber">手机号码</param>
        /// <param name="templateId">模板编号</param>
        /// <param name="parameters">参数</param>
        /// <param name="sign">验证(选填参数)</param>
        /// <param name="extend">扩展(选填参数)</param>
        /// <param name="ext">说明(选填参数)</param>
        /// <returns></returns>
        public dynamic sendWithParam(string phoneNumber, int templateId, string[] parameters, string sign, string extend, string ext)
        {
            throw new NotImplementedException();
        }
    }
}
