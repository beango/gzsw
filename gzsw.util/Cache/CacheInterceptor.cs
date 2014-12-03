using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ninject;
using Ninject.Extensions.Interception;
using Ninject.Extensions.Interception.Attributes;
using Ninject.Extensions.Interception.Request;
using gzsw.util.cache;

namespace gzsw.util.Cache
{
    public class InterceptCacheAttribute : InterceptAttribute
    {
        public double TimeOut { get; set; }

        public override IInterceptor CreateInterceptor(IProxyRequest request)
        {
            var cacheInterceptor = request.Kernel.Get<CacheInterceptor>();
            cacheInterceptor.TimeOut = TimeOut;
            return cacheInterceptor;
        }
    }

    public class CacheInterceptor : IInterceptor
    {
        [Inject]
        public ICacheProvider Cache { get; set; }

        public double TimeOut { get; set; }

        public void Intercept(IInvocation invocation)
        {
            var key = GenerateCacheKey(invocation.Request);
            object value = Cache.Get(key);
            if (null != value)
            {
                LogHelper.WriteLog("find from cache: " + key);
                invocation.ReturnValue = value;
                return;
            }
            LogHelper.WriteLog("find from db: " + key + ", timeout:" + TimeOut);
            invocation.Proceed();
            Cache.Set(key, invocation.ReturnValue, TimeOut);
        }

        private string GenerateCacheKey(IProxyRequest request)
        {
            var sb = new StringBuilder(request.Method.Name).Append(".");
            foreach (var argument in request.Arguments)
            {
                if (argument == null)
                {
                    sb.Append("null");
                }
                else if (argument is string && argument.ToString().Length < 50)
                {
                    sb.Append((string)argument);
                }
                else
                {
                    sb.Append(argument.GetHashCode());
                }
                sb.Append(".");
            }
            sb.Remove(sb.Length - 1, 1);
            return sb.ToString();

        }
    }
}
