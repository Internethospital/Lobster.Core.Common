
using Orient.Client;
using Orient.Client.API;

namespace Core.Common.Helper
{
    /// <summary>
    /// 接口
    /// </summary>
    public interface IOrientDbHelper
    {
        /// <summary>
        /// 数据库连接
        /// </summary>
        ODatabase odatabase { get; set; }
        /// <summary>
        /// 数据库事务
        /// </summary>
        OTransaction transaction { get; set; }
    }
    /// <summary>
    /// 实现类
    /// </summary>
    public class OrientDbHelper: IOrientDbHelper
    {
        /// <summary>
        /// 构造方法
        /// </summary>
        public OrientDbHelper()
        {
            _odatabase = new ODatabase("127.0.0.1", 2424, "xxx", ODatabaseType.Document, "root", "root");
        }

        private ODatabase _odatabase = null;           //数据库连接
        /// <summary>
        /// 数据库连接
        /// </summary>
        public ODatabase odatabase
        {
            get
            {
                return _odatabase;
            }
            set
            {
                _odatabase = value;
            }
        }
        private OTransaction _transaction = null;            //数据库事务
        /// <summary>
        /// 数据库事务
        /// </summary>
        public OTransaction transaction
        {
            get
            {
                return _odatabase.Transaction;
            }
            set
            {
                _transaction = value;
            }
        }
    }
}
