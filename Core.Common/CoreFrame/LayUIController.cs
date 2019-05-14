using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Core.Common.CoreFrame
{
    public class LayUIController:Controller
    { 
        public   string ToJson(Hashtable hash)
        {
            throw new NotImplementedException();
        }

        public   string ToJson(DataTable dt)
        {
            string json = JsonConvert.SerializeObject(dt);
            return json;
        }

        public   string ToJson(object model)
        {
            string json = JsonConvert.SerializeObject(model);
            return json;
        }

        public   string ToTableJson(int count, DataTable dt)
        {
            string rowjson = ToJson(dt);
            string json = "{ \"code\":0,\"msg\":\"\",\"count\":" + count + ",\"data\":" + rowjson + "}";
            return json;
        }

        public   string ToTableJson(int count, object list)
        {
            string rowjson = ToJson(list);
            string json = "{ \"code\":0,\"msg\":\"\",\"count\":" + count + ",\"data\":" + rowjson + "}";
            return json;
        }

        public   string ToTreeJson(object list)
        {
            string rowjson = ToJson(list);
            string json = "{ \"code\":0,\"msg\":\"\",\"data\":" + rowjson + "}";
            return json;
        }

        public   string ToPhotoJson(string title, int id, int start, DataTable dt)
        {
            string rowjson = ToJson(dt);
            string json = "{ \"code\":0,\"msg\":\"\",\"title\":\"" + title + "\",\"id\":" + id + ",\"start\":" + start + ",\"data\":" + rowjson + "}";
            return json;
        }
    }
}
