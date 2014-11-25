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
                    var req = filterContext.ActionDescriptor.ActionName;
                    if (chktype == 0)
                        filterContext.Result = new RedirectResult("/Account/Login?redirect=" + req);
                    else
                        filterContext.Result = new RedirectResult("/Account/NoAuth?redirect=" + req);
                }
                else
                {
                }
            }
        }
    }

    /// <summary>
    /// 统一管理操作枚举
    /// </summary>
    public enum ActionEnum
    {
        /// <summary>
        /// 所有
        /// </summary>
        ALL=0,
        /// <summary>
        /// 查看
        /// </summary>
        VIW=1,
        /// <summary>
        /// 添加
        /// </summary>
        ADD=2,
        /// <summary>
        /// 修改
        /// </summary>
        EDT=3,
        /// <summary>
        /// 删除
        /// </summary>
        DEL=4
    }
}
