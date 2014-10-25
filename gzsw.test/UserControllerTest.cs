using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using gzsw.controller;
using System.Web.Mvc;
using gzsw.model;
using gzsw.util;

namespace gzsw.test
{
    [TestClass]
    public class UserControllerTest
    {
        private TestContext testContextInstance;

        /// <summary>
        ///获取或设置测试上下文，上下文提供
        ///有关当前测试运行及其功能的信息。
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region 附加测试属性
        // 
        //编写测试时，还可使用以下属性:
        //
        //使用 ClassInitialize 在运行类中的第一个测试前先运行代码
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //使用 ClassCleanup 在运行完类中的所有测试后再运行代码
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //使用 TestInitialize 在运行每个测试前先运行代码
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //使用 TestCleanup 在运行完每个测试后运行代码
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion

        [TestMethod]
        public void Login_Test()
        {
            AccountController target = new AccountController();

            Assert.AreNotEqual(target.Login(""), null);

            Assert.AreNotEqual(target.Login("/Home/Index"), null);
        }

        [TestMethod]
        public void LoginPost_Test()
        {
            AccountController target = new AccountController();

            LoginModel model = new LoginModel {
                UserID = "admin",
                Password = "admin1",
                RememberMe = true
            };
            ViewResult result = target.Login(model,"/Home/Index") as ViewResult;
            var _USCok = new CookieHelper().GetCookie("USERSTATE");
            //Assert.AreNotEqual(actual.ViewData.Model as List<Products>, null);
            Assert.AreNotEqual(_USCok, null);
        }
    }
}
