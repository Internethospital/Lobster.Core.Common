using Senparc.Weixin.Entities.TemplateMessage;
using Senparc.Weixin.MP.AdvancedAPIs.TemplateMessage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Common.Weixin
{
    /// <summary>
    /// 微信模板类
    /// </summary>
    public class WeiXinTemplate : TemplateMessageBase
    {
        public TemplateDataItem first { get; set; }
        /// <summary>
        ///  
        /// </summary>
        public TemplateDataItem keyword1 { get; set; }
        /// <summary>
        ///  
        /// </summary>
        public TemplateDataItem keyword2 { get; set; }

        /// <summary>
        ///  
        /// </summary>
        public TemplateDataItem keyword3 { get; set; }
        /// <summary>
        ///  
        /// </summary>
        public TemplateDataItem keyword4 { get; set; }
        /// <summary>
        ///  
        /// </summary>
        public TemplateDataItem keyword5 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public TemplateDataItem remark { get; set; }

        /// <summary>
        /// 排队叫号通知模板
        /// </summary>
        /// <param name="templateId">模板ID</param>
        /// <param name="url"></param>
        /// <param name="room">诊室</param>
        /// <param name="doctor">医生</param>
        public WeiXinTemplate(string templateId, string url, string title, string _first, string _remark, string[] _params)
            : base(templateId, url, title)
        {
            first = new TemplateDataItem(_first);
            if (_params.Length > 0)
                keyword1 = new TemplateDataItem(_params[0]);
            if (_params.Length > 1)
                keyword2 = new TemplateDataItem(_params[1]);
            if (_params.Length > 2)
                keyword3 = new TemplateDataItem(_params[2]);
            if (_params.Length > 3)
                keyword4 = new TemplateDataItem(_params[3]);
            if (_params.Length > 4)
                keyword5 = new TemplateDataItem(_params[4]);
            remark = new TemplateDataItem(_remark);
        }
    }
}
