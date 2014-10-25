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
        }

        [TestMethod]
        public void TestMethod2()
        {
            double v = 0;
            Console.WriteLine(3 / v);
        }


    }
}
