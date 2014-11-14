using System.Linq;
using System.Web;
using gzsw.controller;
using gzsw.model;
using gzsw.util;
using Ninject;
using System;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using gzsw.controller.MyAuth;
using gzsw.dal;
using System.Web.Optimization;
using gzsw.util.cache;
using System.Web.Security;

namespace gzsw.web
{
    public class MvcApplication : HttpApplication
    {
        [Ninject.Inject]
        public ICacheProvider cacheProvider { get; set; }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AuthConfig.RegisterAuth();
            DependencyResolver.SetResolver(new NinjectDependencyResolver());
        }

        protected void Session_Start(object sender, EventArgs e)
        {
            string sessionId = Session.SessionID;
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            #region 附件上传的验证参数

            try
            {
                string session_param_name = "ASPSESSID";
                string session_cookie_name = "ASP.NET_SessionId";

                if (HttpContext.Current.Request.Form[session_param_name] != null)
                {
                    UpdateCookie(session_cookie_name, HttpContext.Current.Request.Form[session_param_name]);
                }
                else if (HttpContext.Current.Request.QueryString[session_param_name] != null)
                {
                    UpdateCookie(session_cookie_name, HttpContext.Current.Request.QueryString[session_param_name]);
                }
            }
            catch
            {
            }
            try
            {
                string auth_param_name = "AUTHID";
                string auth_cookie_name = FormsAuthentication.FormsCookieName;

                if (HttpContext.Current.Request.Form[auth_param_name] != null)
                {
                    UpdateCookie(auth_cookie_name, HttpContext.Current.Request.Form[auth_param_name]);
                }
                else if (HttpContext.Current.Request.QueryString[auth_param_name] != null)
                {
                    UpdateCookie(auth_cookie_name, HttpContext.Current.Request.QueryString[auth_param_name]);
                }
            }
            catch
            {
            }
            #endregion
        }

        #region 附件上传的权限问题

        private void UpdateCookie(string cookieName, string cookieValue)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies.Get(cookieName);
            if (null == cookie)
            {
                cookie = new HttpCookie(cookieName);
            }
            cookie.Value = cookieValue;
            HttpContext.Current.Request.Cookies.Set(cookie);
        }
        #endregion

        protected void Application_PostAuthenticateRequest(object sender, System.EventArgs e)
        {
            var formsIdentity = HttpContext.Current.User.Identity as FormsIdentity;
            if (formsIdentity != null && formsIdentity.IsAuthenticated && formsIdentity.AuthenticationType == "Forms")
            {
                var principal = MyFormsAuthentication<MyUserDataPrincipal>.TryParsePrincipal(HttpContext.Current.Request);
                if (null != principal && principal.UserState.UserState.UserFuncs==null)
                {
                    principal.UserState.UserState.UserFuncs =
                        GetUserFuncsCache(principal.UserState.UserState.UserID);
                    principal.UserState.UserState.UserOrgs =
                        GetUserOrgsCache(principal.UserState.UserState.UserID);
                }
                HttpContext.Current.User = principal;
            }
        }

        void Application_Error(object sender, EventArgs e)
        {
            try
            {
                var objExp = HttpContext.Current.Server.GetLastError();
                LogHelper.ErrorLog("全局异常：\r\n客户机IP：" + Request.UserHostAddress + "\r\n错误地址：" + Request.Url, objExp);
                //Response.Redirect("/Home/Error");
            }
            catch (System.Exception ex)
            {
                LogHelper.ErrorLog("全局未能记录的异常", ex);
                throw ex;
            }
        }

        private List<UserFuncs> GetUserFuncsCache(string userid)
        {
            try
            {
                string key = "ACCOUNTCONTROLLLER_CACHEKEY_USERUNCS_" + userid.ToUpper();
                return new MemoryCacheProvider().Get(key, () =>
                    new SYS_USER_DAL().GetUserFunc(userid)
                    );
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("GetUserFuncsCache", ex);
                return null;
            }
        }
        private List<SYS_ORGANIZE> GetUserOrgsCache(string userid)
        {
            try
            {
                string key = "ACCOUNTCONTROLLLER_CACHEKEY_USERORGS_" + userid.ToUpper();
                return new MemoryCacheProvider().Get(key, () =>
                    new SYS_USER_DAL().GetUserORG(userid)
                    );
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("GetUserOrgsCache", ex);
                return null;
            }
        }
    }

    public class NinjectDependencyResolver : IDependencyResolver
    {
        private readonly IKernel _kernel;

        public NinjectDependencyResolver()
        {
            _kernel = new StandardKernel();
            _kernel.Settings.InjectNonPublic = true;
            AddBindings();
        }

        private void AddBindings()
        {
            _kernel.Bind(typeof(IDao<>)).To(typeof(DaoTemplate<>));
            _kernel.Bind<ICacheProvider>().To<MemoryCacheProvider>();
        }

        public object GetService(Type serviceType)
        {
            return _kernel.TryGet(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return _kernel.GetAll(serviceType);
        }

    }
}