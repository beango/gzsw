using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Runtime.Caching;

namespace gzsw.util.cache
{
    public class MemoryCacheProvider : ICacheProvider
    {
        public T Get<T>(string key, Expression<Func<T>> expression, TimeSpan? expiry = null)
        {
            if (MemoryCache.Default.Contains(key))
            {
                return (T)MemoryCache.Default.Get(key);
            }
            return SetAndReturn(key, expression, expiry);
        }

        private static T SetAndReturn<T>(string key, Expression<Func<T>> expression, TimeSpan? expiry = null)
        {
            var func = expression.Compile();
            var response = func();
            if (!ReferenceEquals(response, null))
                MemoryCache.Default.Add(key, response, new CacheItemPolicy { SlidingExpiration = expiry ?? TimeSpan.FromMinutes(5) });
            return response;
        }

        public void Bust(string key)
        {
            MemoryCache.Default.Remove(key);
        }
    }
}
