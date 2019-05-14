﻿using Dapper;
using Dapper.Contrib.Extensions;
using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Core.Common.CoreFrame
{
    /// <summary>
    /// 数据库操作基类
    /// </summary>
    public class AbstractDao : AbstractCoreFrame
    {
        /// <summary>
        /// 根据主键获取实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id">主键ID</param>
        /// <returns></returns>
        public T GetModel<T>(int id) where T : class
        {
            return connection.Get<T>(id,transaction);
        }

        /// <summary>
        /// 保存更新实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">实体对象</param>
        /// <returns></returns>
        public bool Save<T>(T obj) where T : class
        {
            try
            {
                string value = GetKeyValue<T>(obj);
                if (string.IsNullOrEmpty(value))
                {
                    return false;
                }
                bool flag = false;
                if (0 == Convert.ToInt32(value))
                {
                    flag = connection.Insert<T>(obj) > 0;
                }
                else
                {
                    flag = connection.Update<T>(obj);
                }
                return flag;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 删除实体对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id">主键ID</param>
        /// <returns></returns>
        public bool Delete<T>(int id) where T : class
        {
            T model=GetModel<T>(id);
            return connection.Delete<T>(model, transaction);
        }

        /// <summary>
        /// 根据条件查询实体列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="where">查询条件</param>
        /// <returns></returns>
        public IEnumerable<T> GetList<T>(string where="")
        {
            Type currenttype = typeof(T);
            string tableName = GetTableName(currenttype);
            string strSql = string.Empty;
            if(!string.IsNullOrEmpty(where))
            {
                strSql = "SELECT * FROM " + tableName + " WHERE " + where;
            }
            else
            {
                strSql = "SELECT * FROM " + tableName;
            }
            return connection.Query<T>(strSql);
        }

        /// <summary>
        /// 获取表名
        /// 默认使用类名和覆盖，如果类有一个Table属性
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        private static string GetTableName(object entity)
        {
            Type type = entity.GetType();
            return GetTableName(type);
        }

        /// <summary>
        /// 获取表名
        /// 默认使用类名和覆盖，如果类有一个Table属性
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static string GetTableName(Type type)
        {
            var tableName = String.Format("[{0}]", type.Name);
            //string tableName = Encapsulate(type.Name);

            var tableattr =
                type.GetCustomAttributes(true).SingleOrDefault(attr => attr.GetType().Name == "TableAttribute") as
                    dynamic;
            if (tableattr != null)
            {
                tableName = String.Format("[{0}]", tableattr.Name);
                //tableName = Encapsulate(tableattr.Name);
                try
                {
                    if (!String.IsNullOrEmpty(tableattr.Schema))
                    {
                        tableName = String.Format("[{0}].[{1}]", tableattr.Schema, tableattr.Name);
                        //string schemaName = Encapsulate(tableattr.Schema);
                        //tableName = String.Format("{0}.{1}", schemaName, tableName);
                    }
                }
                catch (RuntimeBinderException)
                {
                    //架构不存在这个这个属性。
                }
            }
            return tableName;
        }

        /// <summary>
        /// 获取实体类中主键的值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        private static string GetKeyValue<T>(T obj) where T : class
        {
            Type currenttype = typeof(T);
            List<PropertyInfo> idProps = GetIdProperties(currenttype).ToList();
            if (!idProps.Any())
            {
                throw new ArgumentException("实体类没有任何主键属性");
            }

            if (idProps.Count() > 1)
            {
                throw new ArgumentException("实体类不能有多个主键属性");
            }

            PropertyInfo onlyKey = idProps.First();
            object o = onlyKey.GetValue(obj);
            if (o != null && !string.IsNullOrEmpty(o.ToString()))
            {
                return o.ToString();
            }
            return string.Empty;
        }

        /// <summary>
        /// 获取被命名ID的或Key特性的所有属性
        /// 为了插入和更新
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        private static IEnumerable<PropertyInfo> GetIdProperties(object entity)
        {
            Type type = entity.GetType();
            return GetIdProperties(type);
        }

        /// <summary>
        /// 获取被命名ID的或Key特性的所有属性
        /// </summary>
        /// <param name="type">实体类型</param>
        /// <returns></returns>
        private static IEnumerable<PropertyInfo> GetIdProperties(Type type)
        {
            List<PropertyInfo> tp =
                type.GetProperties()
                    .Where(p => p.GetCustomAttributes(true).Any(attr => attr.GetType().Name == "KeyAttribute"))
                    .ToList();
            return tp.Any() ? tp : type.GetProperties().Where(p => p.Name == "Id");
        }
    }
}