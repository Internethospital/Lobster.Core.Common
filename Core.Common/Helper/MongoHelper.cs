using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Text;

namespace Core.Common.Helper
{
    /// <summary>
    /// MongoDB操作类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MongoHelper<T>
    {
        public string conn;
        public string dbName;
        public string collectionName;

        private IMongoCollection<T> collection;

        public MongoHelper(string _conn, string _dbName)
        {
            conn = _conn;
            dbName = _dbName;
            collectionName = typeof(T).Name;
            SetCollection();
        }

        public MongoHelper(string _conn, string _dbName, string _collectionName)
        {
            conn = _conn;
            dbName = _dbName;
            collectionName = _collectionName;
            SetCollection();
        }

        /// <summary>
        /// 设置你的collection
        /// </summary>
        public void SetCollection()
        {
            MongoClient client = new MongoClient(conn);
            //var server = client.GetServer();
            var database = client.GetDatabase(dbName);
            collection = database.GetCollection<T>(collectionName);
        }

        /// <summary>
        /// 查找
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public T Find(string id)
        {
            ObjectId objid = new ObjectId(id);
            var filter = Builders<T>.Filter.Eq("_id", objid);
            Object model = this.collection.Find(filter).FirstAsync().Result;
            if (model != null)
                (model as AbstractMongoModel).id_string = (model as AbstractMongoModel).id.ToString();
            return (T)model;
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public List<T> FindAll(Expression<Func<T, bool>> filter)
        {
            List<T> list = this.collection.Find(filter).ToListAsync().Result;
            foreach (Object model in list)
            {
                (model as AbstractMongoModel).id_string = (model as AbstractMongoModel).id.ToString();
            }
            return list;
        }
        /// <summary>
        /// 查询全部
        /// </summary>
        /// <returns></returns>
        public List<T> FindAll()
        {
            List<T> list = this.collection.Find(Builders<T>.Filter.Empty).ToListAsync().Result;
            foreach (Object model in list)
            {
                (model as AbstractMongoModel).id_string = (model as AbstractMongoModel).id.ToString();
            }
            return list;
        }
        /// <summary>
        /// 记录条数
        /// </summary>
        /// <returns></returns>
        public long Count()
        {
            return this.collection.Count(Builders<T>.Filter.Empty);
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <returns></returns>
        public long Update(string id, string fieldname, object val)
        {
            ObjectId objid = new ObjectId(id);
            var filter = Builders<T>.Filter.Eq("_id", objid);
            var update = Builders<T>.Update.Set(fieldname, val);
            return this.collection.UpdateOne(filter, update).ModifiedCount;
        }

        public long Update(T model)
        {
            if ((model as AbstractMongoModel).id_string != null)
                (model as AbstractMongoModel).id = new ObjectId((model as AbstractMongoModel).id_string);
            ObjectId objid = new ObjectId((model as AbstractMongoModel).id_string);
            var filter = Builders<T>.Filter.Eq("_id", objid);
            return this.collection.ReplaceOne(filter, model).ModifiedCount;
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool Insert(T model)
        {
            this.collection.InsertOne(model);
            (model as AbstractMongoModel).id_string = (model as AbstractMongoModel).id.ToString();
            return true;
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public long Delete(string id)
        {
            ObjectId objid = new ObjectId(id);
            var filter = Builders<T>.Filter.Eq("_id", objid);
            return this.collection.DeleteOne(filter).DeletedCount;
        }

        /// 将 Stream 转成 byte[]
        public byte[] StreamToBytes(Stream stream)
        {
            byte[] bytes = new byte[stream.Length];
            stream.Read(bytes, 0, bytes.Length);
            // 设置当前流的位置为流的开始
            stream.Seek(0, SeekOrigin.Begin);
            return bytes;
        }

        /// 将 byte[] 转成 Stream
        public Stream BytesToStream(byte[] bytes)
        {
            Stream stream = new MemoryStream(bytes);
            return stream;
        }
    }

    [JsonObject(MemberSerialization.OptOut)]
    public abstract class AbstractMongoModel
    {
        [JsonIgnore]
        public ObjectId id { get; set; }
        //[BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        //public DateTime created_at { get; set; }
        //[BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        //public DateTime updated_at { get; set; }

        private string _id_string;
        public string id_string
        {
            get
            {
                return _id_string;
            }
            set
            {
                _id_string = value;
            }
        }
    }
}
