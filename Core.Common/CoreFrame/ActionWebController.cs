using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Text;
using Core.Common.Helper;

namespace Core.Common.CoreFrame
{
    public class ActionWebController : ActionFilterAttribute
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
            if (context.Controller is WebControllerBase)
            {
                WebControllerBase controller = (context.Controller as WebControllerBase);
                controller.LoginUserInfo = context.HttpContext.Session.Get<SysLoginRight>("SysLoginRight");
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
        }
    }

    public class ExceptionWebController : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            context.ExceptionHandled = true;
            context.Result = new ObjectResult(context.Exception);
            LogHelper.Error(context.ActionDescriptor.DisplayName, context.Exception);
        }
    }
}
