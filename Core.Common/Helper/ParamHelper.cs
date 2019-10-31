using Core.Common.Data;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Common.Helper
{
    public class ParamHelper
    {
        /// <summary>
        /// 获取系统参数
        /// </summary>
        /// <param name="systemConfig">参数实体</param>
        /// <returns></returns>
        public static Response GetSystemConfigParam(int workId, string param)
        {
            RestRequest request = new RestRequest("/common/v1/SystemConfig/GetSystemConfigs_IH");
            request.AddParameter("workId", workId);
            request.AddParameter("param", param);

            Response response = RestHelper.ExecuteGet<Response>("ApiGateway", request);
            return response;
        }
    }
}
