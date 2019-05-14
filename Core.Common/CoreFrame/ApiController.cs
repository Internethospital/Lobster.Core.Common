﻿using Core.Common.Helper;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Data.Common;

namespace Core.Common.CoreFrame
{
    /// <summary>
    /// 自定义API基类
    /// </summary>
    [Controller]
    public class ApiControllerBase : ControllerBase, IApiHelper
    {

        private DbConnection _connection = null;//数据库连接
        /// <summary>
        /// 数据库连接
        /// </summary>
        public DbConnection connection
        {
            get
            {
                return _connection;
            }
            set
            {
                _connection = value;
            }
        }
        private DbTransaction _transaction = null;            //数据库事务
        /// <summary>
        /// 数据库事务
        /// </summary>
        public DbTransaction transaction
        {
            get
            {
                return _transaction;
            }
            set
            {
                _transaction = value;
            }
        }

        /// <summary>
        /// 是否处于事务中
        /// </summary>
        private bool IsTransaction = false;
        /// <summary>
        /// 启动一个事务
        /// </summary>
        [NonAction]
        public void BeginTransaction()
        {
            try
            {
                if (IsTransaction == false)
                {
                    connection.Open();
                    transaction = connection.BeginTransaction();
                    IsTransaction = true;
                }
                else
                {
                    throw new Exception("事务正在进行，一个对象不能同时开启多个事务！");
                }
            }
            catch (Exception e)
            {
                connection.Close();
                IsTransaction = false;
                throw new Exception("事务启动失败，请再试一次！\n" + e.Message);
            }
        }
        /// <summary>
        /// 提交一个事务
        /// </summary>
        [NonAction]
        public void CommitTransaction()
        {
            if (transaction != null)
            {
                transaction.Commit();
                IsTransaction = false;
                connection.Close();
            }
            else
            {
                throw new Exception("无可用事务！");
            }
        }
        /// <summary>
        /// 回滚一个事务
        /// </summary>
        [NonAction]
        public void RollbackTransaction()
        {
            if (transaction != null)
            {
                transaction.Rollback();
                IsTransaction = false;
                connection.Close();
            }
            else
            {
                throw new Exception("无可用事务！");
            }
        }

        /// <summary>
        /// 创建Object对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T NewObject<T>()
        {
            T obj = (T)System.Activator.CreateInstance(typeof(T));
            if (obj is AbstractCoreFrame)
            {
                (obj as AbstractCoreFrame).connection = connection;
                (obj as AbstractCoreFrame).transaction = transaction;
            }

            return obj;
        }
        /// <summary>
        /// 创建Dao对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T NewDao<T>()
        {
            T obj = (T)System.Activator.CreateInstance(typeof(T));
            if (obj is AbstractCoreFrame)
            {
                (obj as AbstractCoreFrame).connection = connection;
                (obj as AbstractCoreFrame).transaction = transaction;
            }

            return obj;
        }
    }
}
