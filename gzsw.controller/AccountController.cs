using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Cotide.Framework.ActionResult;
using gzsw.controller.MyAuth;
using gzsw.dal;
using gzsw.model;
using gzsw.util.cache;
using Ninject.Activation;
using gzsw.util;
using gzsw.util.Cache;

namespace gzsw.controller
{
    public class AccountController : BaseController<SYS_USER>
    {
        [Ninject.Inject]
        public IDao<SYS_LOGINLOG> DaoLOGINLOG { get; set; }

        [Ninject.Inject]
        public ICacheProvider cacheProvider { get; set; }

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }
        [AllowAnonymous]
        public ActionResult Login1(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model, string returnUrl)
        { 
            try
            {
                #region 数据验证
                if (string.IsNullOrEmpty(model.UserID))
                {
                    Alter("用户名不能为空。", util.Enum.AlterTypeEnum.Warning);
                    return View();
                }
                if (string.IsNullOrEmpty(model.Password))
                {
                    Alter("密码不能为空。", util.Enum.AlterTypeEnum.Warning);
                    return View();
                }
                if (string.IsNullOrEmpty(model.Code))
                {
                    Alter("请输入验证码。", util.Enum.AlterTypeEnum.Warning);
                    return View();
                }
                if (Check(model.Code) == false)
                {
                    Alter("验证码输入错误。", util.Enum.AlterTypeEnum.Warning);
                    return View();
                }

                #endregion

                string pwd = CryptTools.Md5(model.Password);
                var user = dao.GetEntity("USER_ID", model.UserID);//, "USER_PASSWORD", pwd
                if (user == null)
                {
                    AddLoginLog(SYS_LOGINLOG.STATE_ENUM.用户不存在, model.UserID);
                    Alter("用户不存在！", util.Enum.AlterTypeEnum.Error);
                    ModelState.AddModelError("", "用户不存在！");
                    return View();
                }
                if (user.USER_PASSWORD!=pwd)
                {
                    AddLoginLog(SYS_LOGINLOG.STATE_ENUM.密码错误, model.UserID);
                    Alter("密码错误！", util.Enum.AlterTypeEnum.Error);
                    ModelState.AddModelError("", "密码错误！");
                    return View();
                }
                var us = new UserState();
                us.UserID = user.USER_ID;
                us.UserName = user.USER_NAM;
                #region
                
                //验证成功
                var userData = new MyUserDataPrincipal { UserState = us };
                MyFormsAuthentication<MyUserDataPrincipal>.SetAuthCookie(us.UserName, userData, model.RememberMe);
                GetUserFuncsCache(us.UserID);
                #endregion
                AddLoginLog(SYS_LOGINLOG.STATE_ENUM.登录成功, model.UserID);
                return Redirect("/");
            }
            catch (Exception e)
            {
                LogHelper.ErrorLog("登录出错。", e);
                ModelState.AddModelError("", "登录出错。");
                return View();
            } 
        }

        private void AddLoginLog(SYS_LOGINLOG.STATE_ENUM loginstats, string username)
        {
            try
            {
                DaoLOGINLOG.AddObject(new SYS_LOGINLOG()
                {
                    OPER_DTIME = DateTime.Now,
                    BROWSER = Request.Browser.Type,
                    OPER_IP = GetUserIp,
                    STATE = (byte)loginstats,
                    USER_ID = username
                });
            }
            catch (Exception e)
            {
                LogHelper.ErrorLog("记录登录日志出错", e);
            }
        }

        //[ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            var t = Guid.NewGuid().ToString("N");
            try
            {
                if (null != UserState)
                    RefleshUserFuncsCache(UserState.UserID);
                System.Web.Security.FormsAuthentication.SignOut();
                Session.Abandon();
                AddLoginLog(SYS_LOGINLOG.STATE_ENUM.退出成功, UserState.UserID);
            }
            catch
            {
                AddLoginLog(SYS_LOGINLOG.STATE_ENUM.退出失败, UserState.UserID);
            }
            return Redirect(Url.Action("Login", "Account", new { @t = t })); 
        }


        public ActionResult Change()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult NoAuth()
        {
            //ViewBag.F = new SYS_USER_DAL().GetUserFunc(UserState.UserID);
            //ViewBag.TD = dal.GetUserORG(UserState.UserID);
            return View();
        }

        [Ninject.Inject]
        public SYS_USER_DAL dal { get; set; }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Change(LocalPasswordModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var pwd = CryptTools.Md5(model.OldPassword);
                    var user = dao.GetEntity("USER_ID", UserState.UserID, "USER_PASSWORD", pwd);
                    if (user == null)
                    {
                        Alter("旧密码不正确。", util.Enum.AlterTypeEnum.Warning);
                        ModelState.AddModelError("", "旧密码不正确。");
                        return View(model);
                    }

                    var newPwd = CryptTools.Md5(model.NewPassword);
                    user.USER_PASSWORD = newPwd;

                    var id = dao.UpdateObject(user);
                    if (id > 0)
                    {
                        Alter("修改密码成功！", util.Enum.AlterTypeEnum.Success, false, true);
                        return View(model);
                    }
                    Alter("修改密码失败！", util.Enum.AlterTypeEnum.Success, false, true);
                    return View(model);
                }

                return View(model);
            }
            catch (Exception e)
            {
                LogHelper.ErrorLog("修改密码出错。", e);
                ModelState.AddModelError("", "修改密码出错。");
                return View(model);
            } 
        }

        /// <summary>
        /// 检查密码强度
        /// </summary>
        public ActionResult CheckPasswordIntensity(string NewPassword)
        {
            return null;
        }

        #region Helper
        /// <summary>
        /// 客户端ip(访问用户)
        /// </summary>
        private string GetUserIp
        {
            get
            {
                string realRemoteIP = "";
                if (System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] != null)
                {
                    realRemoteIP = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].Split(',')[0];
                }
                if (string.IsNullOrEmpty(realRemoteIP))
                {
                    realRemoteIP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                }
                if (string.IsNullOrEmpty(realRemoteIP))
                {
                    realRemoteIP = System.Web.HttpContext.Current.Request.UserHostAddress;
                }
                return realRemoteIP;
            }
        }

        #region Helper

        /// <summary>
        /// 检查验证码
        /// </summary>
        /// <param name="code">验证码</param>
        /// <returns></returns>
        private bool Check(string code)
        {
            if (HttpContext.Session[GuidKey.CodeSessionKey] == null)
                return false;
            if (!base.HttpContext.Session[GuidKey.CodeSessionKey].ToString().ToLower().Equals(code.ToLower()))
            {
                return false;
            }
            return true;
        }

        #endregion
        #endregion

        #region 辅助方法
        internal void RefleshUserFuncsCache(string userid)
        {
            try
            {
                string key = "ACCOUNTCONTROLLLER_CACHEKEY_USERUNCS_" + userid.ToUpper();
                cacheProvider.Bust(key);
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("RefleshUserFuncsCache", ex);
            }
        }
        public ICollection<UserFuncs> GetUserFuncsCache(string userid)
        {
            try
            {
                if (null != UserState)
                    RefleshUserFuncsCache(UserState.UserID);
                string key = "ACCOUNTCONTROLLLER_CACHEKEY_USERUNCS_" + userid.ToUpper();
                return new MemoryCacheProvider().Get<ICollection<UserFuncs>>(key, () =>
                    new SYS_USER_DAL().GetUserFunc(userid)
                    );
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("GetUserFuncsCache", ex);
                return null;
            }
        }
        #endregion
    }
}
