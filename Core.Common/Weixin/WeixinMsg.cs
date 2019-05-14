using Newtonsoft.Json;
using Senparc.Weixin.MP.AdvancedAPIs;
using Senparc.Weixin.MP.AdvancedAPIs.TemplateMessage;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Common.Weixin
{
    /// <summary>
    /// 发送微信公众号消息
    /// </summary>
    public class WeixinMsg
    {
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="weixinAppId">小程序AppID</param>
        /// <param name="appId">开发者ID（微信公众号）</param>
        /// <param name="openId">接收者ID（微信公众号）</param>
        /// <param name="pagepath">小程序跳转链接</param>
        /// <param name="templateId">消息模板</param>
        /// <param name="url"></param>
        /// <param name="title">标题</param>
        /// <param name="_first"></param>
        /// <param name="_remark"></param>
        /// <param name="_params">参数替换</param>
        /// <returns></returns>
        public string Send(string weixinAppId, string appId, string openId, string pagepath, string templateId, string url, string title, string _first, string _remark, string[] _params)
        {
            TempleteModel_MiniProgram mini;
            SendTemplateMessageResult result = new SendTemplateMessageResult();
            var data = new WeiXinTemplate(templateId, "", title, _first, _remark, _params);
            mini = new TempleteModel_MiniProgram();
            mini.appid = weixinAppId;
            mini.pagepath = pagepath;
            result = TemplateApi.SendTemplateMessage(appId, openId, data);
            return JsonConvert.SerializeObject(result);
        }
    }
}
