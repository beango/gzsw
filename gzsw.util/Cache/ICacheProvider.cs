using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace gzsw.util.cache
{
    public interface ICacheProvider
    {
        object Get(string key);

        void Set(string key, object data, double expiry);

        T Get<T>(string key, Expression<Func<T>> expression, TimeSpan? expiry = null);

        T Get<T>(string key, Expression<Func<T>> expression, double expiry);

        void Bust(string key);
    }
}
