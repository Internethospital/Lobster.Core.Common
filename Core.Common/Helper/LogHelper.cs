using NLog;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Common.Helper
{
    /// <summary>
    /// 日志处理类
    /// </summary>
    public class LogHelper
    {
        public static void Trace(Type t, string msg)
        {
            LogManager.GetCurrentClassLogger(t.GetType()).Trace(msg);
        }
        public static void Info(string title, string msg)
        {
            LogManager.GetLogger(title).Info(msg);
        }
        public static void Warn(string title, string msg)
        {
            LogManager.GetLogger(title).Warn(msg);
        }
        public static void Error(string title, string msg)
        {
            LogManager.GetLogger(title).Error(msg);
        }
        public static void Error(string title, Exception ex)
        {
            LogManager.GetLogger(title).Error(ex);
        }
    }
}
