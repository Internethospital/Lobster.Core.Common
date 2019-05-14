using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Common.Messge
{
    /// <summary>
    /// 短信发送接口类
    /// </summary>
    interface IMessage
    {
        /// <summary>
        /// 发送短信
        /// </summary>
        /// <param name="phoneNumber">手机号码</param>
        /// <param name="msg">消息</param>
        /// <param name="extend">扩展(选填参数)</param>
        /// <param name="ext">说明(选填参数)</param>
        /// <returns></returns>
        dynamic send(string phoneNumber, string msg, string extend, string ext);
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
        dynamic sendWithParam( string phoneNumber, int templateId, string[] parameters, string sign, string extend, string ext);

    }
}
