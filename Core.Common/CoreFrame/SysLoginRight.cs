using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Common.CoreFrame
{
    /// <summary>
    /// 登录信息
    /// </summary>
    public class SysLoginRight
    {
        public SysLoginRight()
        {

        }

        public SysLoginRight(int workId)
        {
            _workId = workId;
        }
        private int _userId;

        public int UserId
        {
            get { return _userId; }
            set { _userId = value; }
        }

        private string _userName;
        /// <summary>
        /// 登录用户名称
        /// </summary>

        public string UserName
        {
            get { return _userName; }
            set { _userName = value; }
        }

        private int _empId;

        public int EmpId
        {
            get { return _empId; }
            set { _empId = value; }
        }
        private string _empName;
        /// <summary>
        /// 当前人员名称
        /// </summary>

        public string EmpName
        {
            get { return _empName; }
            set { _empName = value; }
        }
        private int _deptId;

        public int DeptId
        {
            get { return _deptId; }
            set { _deptId = value; }
        }
        private string _deptName;
        /// <summary>
        /// 当前登录科室
        /// </summary>

        public string DeptName
        {
            get { return _deptName; }
            set { _deptName = value; }
        }
        private int _workId;

        public int WorkId
        {
            get { return _workId; }
            set { _workId = value; }
        }

        private string _workName;
        /// <summary>
        /// 当前机构
        /// </summary>

        public string WorkName
        {
            get { return _workName; }
            set { _workName = value; }
        }
        private int _isAdmin;
        /// <summary>
        /// 是否管理员 0普通用户 1机构管理员 2超级管理员
        /// </summary>

        public int IsAdmin
        {
            get { return _isAdmin; }
            set { _isAdmin = value; }
        }

        /// <summary>
        /// token
        /// </summary>
        private string _token;
        public string Token
        {
            get { return _token; }
            set { _token = value; }
        }

    }
}
