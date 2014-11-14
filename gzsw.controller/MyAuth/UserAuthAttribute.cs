using System.Web;
using gzsw.dal;
using gzsw.model;
using gzsw.util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace gzsw.controller.MyAuth
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class UserAuthAttribute : AuthorizeAttribute
    {
        [Ninject.Inject]
        public IDao<SYS_USEROLE> DaoUserole { get; set; }

        [Ninject.Inject]
        public IDao<SYS_ROLEFUNCTION> DaoRolefunction { get; set; }

        public string Funcs { get; set; }

        /// <summary>
        /// 构造
        /// </summary>
        public UserAuthAttribute()
        {
            //DaoUserole = new DaoTemplate<SYS_USEROLE>();
            //DaoRolefunction = new DaoTemplate<SYS_ROLEFUNCTION>();
            //DaoUserole = daourserrole;
            //DaoRolefunction = _DaoRolefunction;
        }

        /// <summary>
        /// 构造
        /// </summary>
        public UserAuthAttribute(string funs)
            : this()
        {
            Funcs = funs;
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var user = httpContext.User as MyFormsPrincipal<MyUserDataPrincipal>;
            if (user != null)
            {
                var rst = user.IsInFunc(Funcs);
                if (!rst)
                    chktype = 1;
                return rst;
            }
            chktype = 0;
            return false;
        }

        private int chktype = 0;
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (filterContext == null)
            {
                throw new ArgumentNullException("filterContext");
            }
            if (OutputCacheAttribute.IsChildActionCacheActive(filterContext))
            {
                throw new InvalidOperationException("MvcResources.AuthorizeAttribute_CannotUseWithinChildActionCache");
            }
            bool inherit = true;
            if (!filterContext.ActionDescriptor.IsDefined(typeof(AllowAnonymousAttribute), inherit) && !filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true))
            {
                if (!this.AuthorizeCore(filterContext.HttpContext))
                {
                    if (chktype == 0)
                        filterContext.Result = new RedirectResult("/Account/Login?unauth");
                    else
                        filterContext.Result = new RedirectResult("/Account/NoAuth?unauth");
                    //HttpCachePolicyBase cache = filterContext.HttpContext.Response.Cache;
                    //cache.SetProxyMaxAge(new TimeSpan(0L));
                    //cache.AddValidationCallback(new HttpCacheValidateHandler(this.CacheValidateHandler), null);
                }
                else
                {
                    // this.HandleUnauthorizedRequest(filterContext);
                }
            }
            //验证不通过,直接跳转到相应页面，注意：如果不使用以下跳转，则会继续执行Action方法
            //filterContext.Result = new RedirectResult("/Account/Login?unauth");
        }
    }

    /// <summary>
    /// 统一管理操作枚举
    /// </summary>
    public enum ActionEnum
    {
        NON,

        /// <summary>
        /// 所有
        /// </summary>
        ALL,
        /// <summary>
        /// 查看
        /// </summary>
        VIW,
        /// <summary>
        /// 添加
        /// </summary>
        ADD,
        /// <summary>
        /// 修改
        /// </summary>
        EDT,
        /// <summary>
        /// 删除
        /// </summary>
        DEL
    }
}
