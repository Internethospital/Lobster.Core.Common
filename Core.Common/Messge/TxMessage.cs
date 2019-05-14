using qcloudsms_csharp;
using qcloudsms_csharp.json;
using qcloudsms_csharp.httpclient;
namespace Core.Common.Messge
{
    /// <summary>
    /// 腾讯云短信
    /// </summary>
    public class TxMessage : MessageConfig, IMessage
    {
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="appid"></param>
        /// <param name="appkey"></param>
        public TxMessage(string appid, string appkey) : base(appid, appkey)
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

            SmsSingleSender ssender = new SmsSingleSender(int.Parse(base.appid), base.appkey);
            var result = ssender.send(1, "86", phoneNumber, msg, "", "");
            return result;
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
            SmsSingleSender ssender = new SmsSingleSender(int.Parse(base.appid), base.appkey);
            var result = ssender.sendWithParam("86", phoneNumber, templateId, parameters, "", "", "");  // 签名参数未提供或者为空时，会使用默认签名发送短信
            return result;
        }
    }
}
