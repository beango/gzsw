using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace gzsw.util.cache
{
    public interface ICacheProvider
    {
        T Get<T>(string key, Expression<Func<T>> expression, TimeSpan? expiry = null);
        void Bust(string key);

        //TV Get<TV>(TK cacheKey, Func<TV> getUncachedValue, DateTimeOffset dateTimeOffset);

        //TV Get<TV>(TK cacheKey, Func<TV> getUncachedValue, TimeSpan timeSpan);

        //void Remove(TK cacheKey);
    }
}
