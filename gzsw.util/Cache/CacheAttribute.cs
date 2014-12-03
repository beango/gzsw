using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace gzsw.util.Cache
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class CacheAttribute : Attribute
    {
        public string CacheName
        { get; set; }
    }
}
