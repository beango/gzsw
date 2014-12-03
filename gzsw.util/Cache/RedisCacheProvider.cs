using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace gzsw.util.cache
{
    public class RedisCacheProvider : ICacheProvider
    {
        public object Get(string key)
        {
            return null;
        }

        public T Get<T>(string key, Expression<Func<T>> expression, double expiry)
        {
            var func = expression.Compile();
            var result = func();
            //var cacheItem = _database.Get<T>(key);
            //if (EqualityComparer<T>.Default.Equals(cacheItem, default(T)))
            //    cacheItem = SetAndGet(key, expression, expiry);
            //return cacheItem;
            return result;
        }

        public void Set(string key, object data, double expiry)
        {

        }

        public T Get<T>(string key, Expression<Func<T>> expression, TimeSpan? expiry = null)
        {
            var func = expression.Compile();
            var result = func();
            //var cacheItem = _database.Get<T>(key);
            //if (EqualityComparer<T>.Default.Equals(cacheItem, default(T)))
            //    cacheItem = SetAndGet(key, expression, expiry);
            //return cacheItem;
            return result;
        }

        private T SetAndGet<T>(string key, Expression<Func<T>> expression, TimeSpan? expiry = null)
        {
            var func = expression.Compile();
            var result = func();
            //if (!ReferenceEquals(result, null))
            //    _database.Set(key, result, expiry ?? TimeSpan.FromMinutes(5));
            return result;
        }

        public void Bust(string key)
        {
            //var database = _connectionMultiplexer.GetDatabase();
            //database.KeyDelete(key);
        }
    }
}
