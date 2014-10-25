using gzsw.util.cache;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading;

namespace gzsw.test
{
    [TestClass]
    public class CacheHelperTest
    {
        [TestMethod]
        public void Get()
        {
            MemoryCacheProvider cachePro = new MemoryCacheProvider();
            var ch1 = cachePro.Get<int>("tetcache",()=>123,new TimeSpan(0,0,3));
            
            Assert.AreEqual(123, ch1, "默认缓存");
            Thread.Sleep(4);

            Assert.AreEqual(false, MemoryCache.Default.Contains("tetcache"), "缓存已经过期");
            

        }
    }
}
