using System;
using Newtonsoft.Json;

namespace gzsw.util
{
    public class JsonHelper
    {
        public static string ToJson<T>(T t)
        {
           return JsonConvert.SerializeObject(t);
        }

        public static T ToObject<T>(string strJson)
        {
            if (!string.IsNullOrEmpty(strJson))
            {
                return default(T);
            }
            return JsonConvert.DeserializeObject<T>(strJson); 
        }
    }
}
