using NodaTime;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Common.Helper
{
    /// <summary>
    /// 时间扩展类
    /// 解决Linux与Windows时间一致性问题
    /// </summary>
    public static class TimeHelper
    {
        public static DateTime GetCstDateTime()
        {
            Instant now = SystemClock.Instance.GetCurrentInstant();
            var shanghaiZone = DateTimeZoneProviders.Tzdb["Asia/Shanghai"];
            return now.InZone(shanghaiZone).ToDateTimeUnspecified();
        }

        /// <summary>
        /// 转换时间格式到东八区，以防docker与windows时区不一致问题
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static DateTime ToCstTime(this DateTime time)
        {
            return GetCstDateTime();
        }
    }
}
