using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using Dapper;

namespace Core.Common.Data
{
    /// <summary>
    /// SQL语句进行分页包装
    /// </summary>
    public class SqlPage
    {
        /// <summary>
        /// 格式化SQL语句
        /// </summary>
        /// <param name="strsql"></param>
        /// <param name="pageInfo"></param>
        /// <param name="oleDb"></param>
        /// <returns></returns>
        public static string FormatSql(DatabaseType type, string strsql, PageInfo pageInfo, DbConnection oleDb)
        {
            switch (type)
            {
                case DatabaseType.IbmDb2:
                    return Db2FormatSql(strsql, pageInfo, oleDb);
                case DatabaseType.MsAccess:
                    return MsAccessFormatSql(strsql, pageInfo, oleDb);
                case DatabaseType.MySQL:
                    return MySQLFormatSql(strsql, pageInfo, oleDb);
                case DatabaseType.Oracle:
                    return OracleFormatSql(strsql, pageInfo, oleDb);
                case DatabaseType.SqlServer:
                    return Sql2005FormatSql(strsql, pageInfo, oleDb);
            }
            return null;
        }

        private static string Db2FormatSql(string strsql, PageInfo pageInfo, DbConnection oleDb)
        {

            return null;
        }
        private static string Sql2000FormatSql(string strsql, PageInfo pageInfo, DbConnection oleDb)
        {
            return null;
        }
        private static string Sql2005FormatSql(string strsql, PageInfo pageInfo, DbConnection oleDb)
        {
            if (pageInfo.OrderBy == null || pageInfo.OrderBy.Length <= 0)
                throw new Exception("分页OrderBy属性不能为空，如：\"Id\" 或 \"Id Desc\"");

            int starRecordNum = pageInfo.startNum;
            int endRecordNum = pageInfo.endNum;

            string _strsql = strsql;


            string sql_totalRecord = "select count(*) from (" + _strsql + ") A";
            Object obj = oleDb.ExecuteScalar(sql_totalRecord);
            pageInfo.totalRecord = Convert.ToInt32(obj == DBNull.Value ? 0 : obj);

            string _sql = _strsql;

            string orderby = string.Join(",", pageInfo.OrderBy);


            strsql = @"select * from (
                                select row_number() over(order by {3}) as rowid,  t.* from ({0}) t
                            )as a where a.rowid >= {1} AND  a.rowid < {2}";

            strsql = String.Format(strsql, _sql, starRecordNum, endRecordNum, orderby);
            return strsql;
        }
        private static string MsAccessFormatSql(string strsql, PageInfo pageInfo, DbConnection oleDb)
        {
            return null;
        }
        private static string MySQLFormatSql(string strsql, PageInfo pageInfo, DbConnection oleDb)
        {
            if (pageInfo.OrderBy == null || pageInfo.OrderBy.Length <= 0)
                throw new Exception("分页OrderBy属性不能为空，如：\"Id\" 或 \"Id Desc\"");

            int starRecordNum = pageInfo.startNum;
            int endRecordNum = pageInfo.endNum;

            string _strsql = strsql;


            string sql_totalRecord = "select count(*) from (" + _strsql + ") A";
            Object obj = oleDb.ExecuteScalar(sql_totalRecord);
            pageInfo.totalRecord = Convert.ToInt32(obj == DBNull.Value ? 0 : obj);

            string _sql = _strsql;

            string orderby = string.Join(",", pageInfo.OrderBy);


            strsql = @"select * from (
                                select row_number() over(order by {3}) as rowid,  t.* from ({0}) t
                            )as a where a.rowid >= {1} AND  a.rowid < {2}";

            strsql = String.Format(strsql, _sql, starRecordNum, endRecordNum, orderby);
            return strsql;
        }

        private static string OracleFormatSql(string strsql, PageInfo pageInfo, DbConnection oleDb)
        {
            int starRecordNum = pageInfo.startNum;
            int endRecordNum = pageInfo.endNum;

            string sql_totalRecord = "select count(*) from (" + strsql + ") A";
            Object obj = oleDb.ExecuteScalar(sql_totalRecord);
            pageInfo.totalRecord = Convert.ToInt32(obj == DBNull.Value ? 0 : obj);

            strsql = " select * from( select a.*,rownum rn from ( " + strsql + " ) a )  where rn between " + starRecordNum.ToString() + " and " + endRecordNum.ToString();

            return strsql;
        }
    }

    /// <summary>
    /// 数据库类别
    /// </summary>
    public enum DatabaseType
    {
        /// <summary>未指定数据库</summary>
        UnKnown,
        /// <summary>IBMDB2数据库</summary>
        IbmDb2,
        /// <summary>SqlServer数据库</summary>
        SqlServer,
        /// <summary>Access数据库</summary>
        MsAccess,
        /// <summary>MySQL数据库</summary>
        MySQL,
        /// <summary>Oracle数据库</summary>
        Oracle
    }
}
