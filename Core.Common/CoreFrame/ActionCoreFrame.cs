﻿using Core.Common.Helper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Text;

namespace Core.Common.CoreFrame
{
    /// <summary>
    /// 
    /// </summary>
    public class ActionCoreFrame : ActionFilterAttribute
    {
        private static string TimeKey = "TimeKey";

        /// <summary>
        /// 在Action方法调用前
        /// </summary>
        /// <param name="context"></param>
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            context.HttpContext.Items[TimeKey] = Stopwatch.StartNew();
            //实例化数据库连接
            if (context.Controller is ApiControllerBase)
            {
                ApiControllerBase controller = (context.Controller as ApiControllerBase);
                controller.connection = new SqlConnection(ConfigHelper.GetSetting("ConnectionStrings:DefaultConnection").ToString());
            }
        }

        /// <summary>
        ///  在Action方法调用后
        /// </summary>
        /// <param name="context"></param>
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            var timer = context.HttpContext.Items[TimeKey] as Stopwatch;
            timer.Stop();
            if (timer.Elapsed.TotalSeconds > 3)
                LogHelper.Warn(context.ActionDescriptor.DisplayName, "耗时：" + timer.Elapsed.TotalSeconds);
            //关闭连接
            if (context.Controller is ApiControllerBase)
            {
                ApiControllerBase controller = (context.Controller as ApiControllerBase);
                controller.connection.Close();
            }
        }
    }

    public class ExceptionCoreFrame : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            context.ExceptionHandled = true;
            context.Result = new ObjectResult(context.Exception);
            LogHelper.Error(context.ActionDescriptor.DisplayName, context.Exception);
        }
    }
}
