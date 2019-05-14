using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Common.Helper
{
    public class ConfigHelper
    {
        public static IConfiguration t;

        /// <summary>
        /// 获取某个配置节点的值
        /// </summary>
        /// <param name="key">配置节点</param>
        /// <returns>配置节点的值</returns>
        public static object GetSetting(string key)
        {
            return t.GetSection(key).Value;
        }
    }
}
