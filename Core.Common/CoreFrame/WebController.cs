using Core.Common.Helper;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Data.Common;

namespace Core.Common.CoreFrame
{
    /// <summary>
    /// 自定义Web基类
    /// </summary>
    [Controller]
    public class WebControllerBase: LayUIController
    {
        private SysLoginRight _LoginUserInfo;
        /// <summary>
        /// 登录用户信息
        /// </summary>
        public SysLoginRight LoginUserInfo
        {
            get
            {
                if (_LoginUserInfo == null)
                {
                    return new SysLoginRight(1);
                }
                return _LoginUserInfo;
            }
            set
            {
                _LoginUserInfo = value;
            }
        }
        
    }
}
