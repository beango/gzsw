using System.Collections.Generic;
using System.Linq;
using System.Text;
using gzsw.controller.MyAuth;
using gzsw.dal;
using gzsw.util.Enum;
using Ninject;
using System.Web.Mvc;
using gzsw.util;
using System.Web.Routing;
using System;
using gzsw.model;

namespace gzsw.controller
{
    public class BaseController : Controller
    {
        [Inject]
        public IDao<SYS_LOG> DaoLog { get; set; }

        #region 权限

        /// <summary>
        /// 用户信息
        /// </summary>
        //[Inject]
        public UserState UserState
        {
            get;
            set;
        }

        protected override IAsyncResult BeginExecute(RequestContext requestContext, AsyncCallback callback, object state)
        {
            try
            {
                var user = requestContext.HttpContext.User as MyFormsPrincipal<MyUserDataPrincipal>;
                if (null != user)
                    UserState = user.UserState.UserState;
                var formkeys = requestContext.HttpContext.Request.Form.AllKeys;
                var formvals = new List<string>();
                foreach (var key in formkeys)
                {
                    formvals.Add(key + "：" + requestContext.HttpContext.Request.Form[key]);
                }
                var log = new SYS_LOG()
                {
                    LOG_DTIME = DateTime.Now,
                    MENU_NAM = requestContext.HttpContext.Request.Url.ToString(),
                    USER_ID = UserState.UserID,
                    LOG_INFO = string.Join(",", formvals)
                };

                DaoLog.AddObject(log);
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("记录日志出错！", ex);
            }
            return base.BeginExecute(requestContext, callback, state);
        }
        #endregion
    }

    public class BaseController<T> : BaseController where T : class
    {
        #region dao
        public BaseController()
        {
        }
        [Inject]
        public IDao<T> dao { get; set; }

        public BaseController(IDao<T> dao)
        {
            this.dao = dao;
        }

        public JsonResult JsonResult(bool rst, string desc, string area = "AUTH", string action = "/", bool reload = true)
        {
            var co = RouteData.Values["controller"];
            var redirecturl = "/" + area + "/" + co + action;
            if (area == "")
                redirecturl = "/" + co + action;

            var errlist = new List<KeyValuePair<string, string>>();
            foreach (var key in ModelState.Keys)
            {
                if (ModelState[key].Errors.Count > 0)
                {
                    var firstOrDefault = ModelState[key].Errors.FirstOrDefault();
                    if (firstOrDefault != null)
                        errlist.Add(new KeyValuePair<string, string>(key, firstOrDefault.ErrorMessage));
                }
            }
            return Json(new { result = rst, desc = desc, reload = reload, url = redirecturl, validmsg = errlist }, "text/html");
        }

        #endregion

        #region 提示

        /// <summary>
        /// 提示
        /// </summary>
        /// <param name="msg">消息</param>
        /// <param name="alterType">消息类型</param>
        /// <param name="isReload">是否刷新当前页</param>
        /// <param name="isCloseDialog">是否关闭当前窗体</param>
        protected void Alter(
            string msg,
            AlterTypeEnum alterType,
            bool isReload = false,
            bool isCloseDialog = false)
        {
            TempData["alert"] = CreateAlter(msg, alterType, isReload, isCloseDialog);
        }

        private MvcHtmlString CreateAlter(string msg,
           AlterTypeEnum alterType = AlterTypeEnum.Success,
           bool isReload = false,
            bool isCloseDialog = false)
        {
            var index = (int)alterType;
            var script = new StringBuilder();
            script.Append("<script type=\"text/javascript\">$(function(){");
            script.Append(string.Format("layer.alert('{0}',{1}", msg, index));
            if (isReload || isCloseDialog)
            {
                script.Append(",function(index){  ");
                if (isReload)
                {
                    script.Append(" location.href=location.href; ");
                }
                if (isCloseDialog)
                {
                    script.Append(" parent.layer.closeAll(); ");
                }
                script.Append(" layer.close(index);}");
            }
            script.Append(");});");
            script.Append("</script>");
            return new MvcHtmlString(script.ToString());
        }
        #endregion
    }
}
