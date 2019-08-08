
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
        /// 数据库服务
        /// </summary>
        OServer server { get; set; }
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
    public class OrientDbHelper : IOrientDbHelper
    {
        /// <summary>
        /// 构造方法
        /// </summary>
        public OrientDbHelper()
        {
            _server = new OServer(ConfigHelper.GetSetting("ConnectionStrings:OrientDBServer").ToString(),
                 int.Parse(ConfigHelper.GetSetting("ConnectionStrings:OrientDBPort").ToString()),
                 ConfigHelper.GetSetting("ConnectionStrings:OrientDBUsername").ToString(),
                 ConfigHelper.GetSetting("ConnectionStrings:OrientDBPassword").ToString());

            _odatabase = new ODatabase(ConfigHelper.GetSetting("ConnectionStrings:OrientDBServer").ToString(),
                int.Parse(ConfigHelper.GetSetting("ConnectionStrings:OrientDBPort").ToString()),
                ConfigHelper.GetSetting("ConnectionStrings:OrientDBDefaultDB").ToString(), ODatabaseType.Graph,
                ConfigHelper.GetSetting("ConnectionStrings:OrientDBUsername").ToString(),
                ConfigHelper.GetSetting("ConnectionStrings:OrientDBPassword").ToString());
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
        private OServer _server = null;           //数据库服务
        /// <summary>
        /// 数据库服务
        /// </summary>
        public OServer server
        {
            get
            {
                return _server;
            }
            set
            {
                _server = value;
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
