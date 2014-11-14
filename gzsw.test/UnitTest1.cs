using System;
using System.Linq;
using System.Reflection;
using System.Web.Script.Serialization;
using System.Web.Security;
using gzsw.controller.MyAuth;
using gzsw.model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using gzsw.util;
using gzsw.model.Enums;
using gzsw.dal;

namespace gzsw.test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            string pat = @"^(([0-1][0-9])|([2][0-3])):([0-5]?[0-9])?$";
            Regex r = new Regex(pat, RegexOptions.IgnoreCase);

            Assert.AreEqual(true, r.Match("12:15").Success);
            Assert.AreEqual(true, r.Match("12:15").Success);
            Assert.AreEqual(true, r.Match("22:15").Success);
            Assert.AreEqual(false, r.Match("24:15").Success);
            //Assert.AreEqual(true, r.Match("4:15").Success);
            Assert.AreEqual(true, r.Match("04:15").Success);
            Assert.AreEqual(true, r.Match("04:00").Success);
            Assert.AreEqual(true, r.Match("04:59").Success);
            //Assert.AreEqual(true, r.Match("2:05").Success);
            //Assert.AreEqual(true, r.Match("2:5").Success);

            Assert.AreEqual(false, r.Match("12:15:11").Success);
            Assert.AreEqual(false, r.Match("24:15").Success);
            Assert.AreEqual(false, r.Match("2:151").Success);
            Assert.AreEqual(false, r.Match("2:051").Success);
            Assert.AreEqual(false, r.Match("").Success);
            //Assert.AreEqual(false, r.Match("2:").Success);
            Assert.AreEqual(false, r.Match(":").Success);
            Assert.AreEqual(false, r.Match(":4").Success);
            Assert.AreEqual(false, r.Match("2").Success);
            Assert.AreEqual(false, r.Match("").Success);

            var param = new Dictionary<string, object>
            {
                {"USER_ID like", 123},
                {"USER_ID in", 223}, 
                {"USER_NAM like", 222},
                {"CREATE_ID", 576}
            };
            var arr = param.ToArray();
            Assert.AreEqual(4, arr.Count());
        }

        [TestMethod]
        public void TestMethod2()
        {
            double v = 0;
            Console.WriteLine(3 / v);
        }

        [TestMethod]
        public void GetHighLV()
        {
            var e1 = EnumHelper.ConvertToEnum<UserLV_ENUM>("6");
            Assert.AreEqual(UserLV_ENUM.省级, (UserLV_ENUM)e1);
        }

        [TestMethod]
        public void T3()
        {
            var log = new SYS_LOG()
            {
                LOG_DTIME = DateTime.Now,
                MENU_NAM = "",
                USER_ID = "tte"
            };
            log.LOG_INFO = "java实现页面最小化后桌面右下角出现提示框(跟QQ提示一样) - xu_...java实现页面最小化后桌面右下角出现提示框(跟QQ提示一样) - xu_...java实现页面最小化后桌面右下角出现提示框(跟QQ提示一样) - xu_...java实现页面最小化后桌面右下角出现提示框(跟QQ提示一样) - xu_...java实现页面最小化后桌面右下角出现提示框(跟QQ提示一样) - xu_...java实现页面最小化后桌面右下角出现提示框(跟QQ提示一样) - xu_...java实现页面最小化后桌面右下角出现提示框(跟QQ提示一样) - xu_...java实现页面最小化后桌面右下角出现提示框(跟QQ提示一样) - xu_...java实现页面最小化后桌面右下角出现提示框(跟QQ提示一样) - xu_...java实现页面最小化后桌面右下角出现提示框(跟QQ提示一样) - xu_...java实现页面最小化后桌面右下角出现提示框(跟QQ提示一样) - xu_...java实现页面最小化后桌面右下角出现提示框(跟QQ提示一样) - xu_...java实现页面最小化后桌面右下角出现提示框(跟QQ提示一样) - xu_...java实现页面最小化后桌面右下角出现提示框(跟QQ提示一样) - xu_...java实现页面最小化后桌面右下角出现提示框(跟QQ提示一样) - xu_...java实现页面最小化后桌面右下角出现提示框(跟QQ提示一样) - xu_...java实现页面最小化后桌面右下角出现提示框(跟QQ提示一样) - xu_...java实现页面最小化后桌面右下角出现提示框(跟QQ提示一样) - xu_...java实现页面最小化后桌面右下角出现提示框(跟QQ提示一样) - xu_...java实现页面最小化后桌面右下角出现提示框(跟QQ提示一样) - xu_...java实现页面最小化后桌面右下角出现提示框(跟QQ提示一样) - xu_...java实现页面最小化后桌面右下角出现提示框(跟QQ提示一样) - xu_...java实现页面最小化后桌面右下角出现提示框(跟QQ提示一样) - xu_...java实现页面最小化后桌面右下角出现提示u_...java实现页面最小化后桌面右下角出现提示框(跟QQ提示一样) - xu_...java实现页面最小化后桌面右下角出现提示框(跟QQ提示一样) - xu_...java实现页面最小化后桌面右下角出现提示框(跟QQ提示一样) - xu_...java实现页面最小化后桌面右下角出现提示框(跟QQ提示一样) - xu_...java实现页面最小化后桌面右下角出现提示框(跟QQ提示一样) - xu_...java实现页面最小化后桌面右下角出现提示框(跟QQ提示一样) - xu_...java实现页面最小化后桌面右下角出现提示";
            log.LOG_INFO = log.LOG_INFO.Substring(0, 1025);
            Console.WriteLine(log.LOG_INFO.Length);
            if (!string.IsNullOrEmpty(log.LOG_INFO) && log.LOG_INFO.Length > 1024)
                log.LOG_INFO = log.LOG_INFO.Substring(0, 1024);
            new DaoTemplate<SYS_LOG>().AddObject(log);
        }
    }
}
