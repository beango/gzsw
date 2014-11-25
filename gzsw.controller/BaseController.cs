using System.Collections.Generic;
using System.Linq;
using System.Text;
using gzsw.controller.MyAuth;
using gzsw.dal;
using gzsw.dal.dao;
using gzsw.util.Enum;
using gzsw.util.Extensions;
using Ninject;
using System.Web.Mvc;
using gzsw.util;
using System.Web.Routing;
using System;
using gzsw.model;
using System.Data;
using gzsw.model.Enums;

namespace gzsw.controller
{
    public class BaseController : Controller
    {
        [Inject]
        public IDao<SYS_LOG> DaoLog { get; set; }

        [Inject]
        public IDao<SYS_HALL> _DaoHAll { get; set; }


        #region 用户资料扩展
        /// <summary>
        /// 获取用户有权限访问的营业厅
        /// </summary>
        protected virtual IList<SYS_HALL> UserHall
        {
            get
            {
                var orgs = UserState.UserOrgs;
                if (null != orgs)
                {
                    return _DaoHAll.FindList("", "ORG_ID in", orgs.Select(o => o.ORG_ID));
                }
                return new List<SYS_HALL>();
            }
        }

        /// <summary>
        /// 是否管理员
        /// </summary>
        protected virtual bool isAdmin
        {
            get
            {
                if (null == UserState)
                    return false;
                return UserState.UserID.ToLower() == "admin";
            }
        }

        /// <summary>
        /// 用户最高权限
        /// </summary>
        protected UserLV_ENUM GetHighLV
        {
            get
            {
                var lv = new SYS_USER_DAL().GetHighLV(UserState.UserID);
                return EnumHelper.ConvertToEnum<UserLV_ENUM>(lv.ToString());
            }
        }
        #endregion

        private int DefaultYears = 2010;

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

        /// <summary>
        /// 跳转到没有权限页
        /// </summary>
        public ActionResult NoAuth
        {
            get { return Redirect("/Account/NoAuth"); }
        }

        /// <summary>
        /// 记录访问日志
        /// </summary>
        /// <param name="requestContext"></param>
        /// <param name="callback"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        protected override IAsyncResult BeginExecute(RequestContext requestContext, AsyncCallback callback, object state)
        {
            try
            {
                var user = requestContext.HttpContext.User as MyFormsPrincipal<MyUserDataPrincipal>;
                if (null != user)
                    UserState = user.UserState.UserState;

                var action = requestContext.RouteData.Values["action"].ToString();
                if (null != action && ",tree,getcode,leftmenu,index,details,SendInfoTip,".IndexOf("," + action.ToLower() + ",") == -1)
                {
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
                        USER_ID = null != UserState ? UserState.UserID : "",
                        LOG_INFO = string.Join(",", formvals)
                    };
                    if (!string.IsNullOrEmpty(log.LOG_INFO) && log.LOG_INFO.Length > 1024)
                    {
                        log.LOG_INFO = log.LOG_INFO.Substring(0, 1024);
                    }
                    DaoLog.AddObject(log);
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("记录日志出错！", ex);
            }
            return base.BeginExecute(requestContext, callback, state);
        }
        #endregion

        #region 图表charts
        /// <summary>
        /// 多指标3D柱状图
        /// </summary>
        /// <returns></returns>
        protected string CreateMSColumn3DChart(string titleName, DataTable dt, int h = 580, string subtitle = "", bool islink = false, bool isformatscale = false, string DefaultNumberScale = "", string NumberScaleValue = "", string NumberScaleUnit = "")
        {
            FusionChartHelper help = new FusionChartHelper(FusionChartType.MSColumn3D);
            help.SetDataSource(dt);
            help.ChartHeight = h;
            help.ChartWidth = "100%";
            help.Caption = titleName;//主标题，将显示在图形顶端
            help.SubCaption = subtitle;//二级标题，将显示在主标题下方，没有不显示
            help.BgColor = "ffffff";//背景色
            help.CanvasBorderColor = "ffffff";
            help.Decimals = 2;//显示小数位
            help.IsFormatNumberScale = isformatscale;//是否格式化数字，如1000显示成1K；
            help.DefaultNumberScale = DefaultNumberScale;
            help.NumberScaleValue = NumberScaleValue;
            help.NumberScaleUnit = NumberScaleUnit;
            var cols = dt.Columns;
            for (int i = 1; i < cols.Count; i++)
            {
                help.SetSeriesName(cols[i].ColumnName, cols[i].ColumnName, null, false, islink);
            }
            return help.ToString();
        }

        /// <summary>
        /// 多指标3D柱状图
        /// </summary>
        /// <returns></returns>
        protected string CreateMSColumn3DLineDYChart(string titleName, DataSet ds, int h = 580, string subtitle = "", bool islink = false)
        {
            FusionChartHelper help = new FusionChartHelper(FusionChartType.MSColumn3DLineDY);
            help.SetDataSource(ds);
            help.ChartHeight = h;
            help.ChartWidth = "100%";
            help.Caption = titleName;//主标题，将显示在图形顶端
            help.SubCaption = subtitle;//二级标题，将显示在主标题下方，没有不显示
            help.BgColor = "ffffff";//背景色
            help.CanvasBorderColor = "ffffff";
            help.Decimals = 2;//显示小数位
            help.IsFormatNumberScale = false;//是否格式化数字，如1000显示成1K；
            var cols = ds.Tables[0].Columns;
            for (int i = 1; i < cols.Count; i++)
            {
                help.SetSeriesName(cols[i].ColumnName, cols[i].ColumnName, null, false, islink);
            }
            return help.ToString();
        }

        /// <summary>
        /// 3D柱状图
        /// </summary>
        /// <returns></returns>
        protected string CreateColumn3DChart(string caption, DataTable dt, string subtitle = "", bool isformatscale = false, string DefaultNumberScale = "", string NumberScaleValue = "", string NumberScaleUnit = "")
        {
            FusionChartHelper help = new FusionChartHelper(FusionChartType.Column3D);
            help.SetDataSource(dt);
            help.ChartHeight = 550;
            help.ChartWidth = "100%";
            help.Caption = caption;//主标题，将显示在图形顶端
            help.SubCaption = subtitle;//二级标题，将显示在主标题下方，没有不显示
            help.BgColor = "ffffff";//背景色
            help.CanvasBorderColor = "ffffff";
            help.Decimals = 2;//显示小数位
            help.IsFormatNumberScale = isformatscale;//是否格式化数字，如1000显示成1K；
            help.DefaultNumberScale = DefaultNumberScale;
            help.NumberScaleValue = NumberScaleValue;
            help.NumberScaleUnit = NumberScaleUnit;
            return help.ToString();
        }

        /// <summary>
        /// 单线性图
        /// </summary>
        /// <returns></returns>
        protected dynamic CreateSplineChart(string caption, DataSet ds, int h = 550, int? max = null, int? min = null, string subCaption = "")
        {
            return CreateSplineChart(caption, ds.Tables[0], h, max, min, subCaption);
        }

        /// <summary>
        /// 单线性图
        /// </summary>
        /// <returns></returns>
        protected dynamic CreateSplineChart(string caption, DataTable dt, int h = 550, int? max = null, int? min = null, string subCaption = "", bool isformatscale = false, string DefaultNumberScale = "", string NumberScaleValue = "", string NumberScaleUnit = "")
        {
            FusionChartHelper help = new FusionChartHelper(FusionChartType.Line);
            help.SetDataSource(dt);
            help.ChartHeight = h;
            help.ChartWidth = "100%";
            help.Caption = caption;//主标题，将显示在图形顶端
            help.SubCaption = subCaption;//二级标题，将显示在主标题下方，没有不显示
            help.BgColor = "ffffff";//背景色
            help.CanvasBorderColor = "ffffff";
            help.Decimals = 2;//显示小数位
            help.IsFormatNumberScale = isformatscale;//是否格式化数字，如1000显示成1K；
            help.DefaultNumberScale = DefaultNumberScale;
            help.NumberScaleValue = NumberScaleValue;
            help.NumberScaleUnit = NumberScaleUnit;
            if (max != null)
                help.YMaxValue = max.Value;
            if (min != null)
                help.YMinValue = min.Value;
            return help.ToString();
        }

        /// <summary>
        /// 3D饼图
        /// </summary>
        /// <returns></returns>
        protected dynamic CreatePie3DChart(string caption, DataSet ds, int h = 550, string subtitle = "")
        {
            return CreatePie3DChart(caption, ds.Tables[0], h, subtitle);
        }

        /// <summary>
        /// 3D饼图
        /// </summary>
        /// <returns></returns>
        protected dynamic CreatePie3DChart(string caption, DataTable dt, int h = 550, string subtitle = "")
        {
            FusionChartHelper help = new FusionChartHelper(FusionChartType.Pie2D);
            help.SetDataSource(dt);
            help.ChartHeight = h;
            help.ChartWidth = "100%";
            help.Caption = caption;//主标题，将显示在图形顶端
            help.SubCaption = subtitle;//二级标题，将显示在主标题下方，没有不显示
            help.BgColor = "ffffff";//背景色
            help.CanvasBorderColor = "ffffff";
            help.Decimals = 2;//显示小数位
            help.IsFormatNumberScale = false;//是否格式化数字，如1000显示成1K；
            return help.ToString();
        }

        /// <summary>
        /// 多指标线性图
        /// </summary>
        /// <param name="caption"></param>
        /// <param name="ds"></param>
        /// <param name="h"></param>
        /// <returns></returns>
        protected string CreateMSSplineChart(string caption, DataSet ds, int h = 550, int? max = null, int? min = null, string subtitle = "")
        {
            return CreateMSSplineChart(caption, ds.Tables[0], h, max, min, subtitle);
        }

        /// <summary>
        /// 多指标线性图
        /// </summary>
        /// <param name="caption"></param>
        /// <param name="dt"></param>
        /// <param name="h"></param>
        /// <returns></returns>
        protected string CreateMSSplineChart(string caption, DataTable dt, int h = 550, int? max = null, int? min = null, string subtitle = "", bool isformatscale = false, string DefaultNumberScale = "", string NumberScaleValue = "", string NumberScaleUnit = "")
        {
            FusionChartHelper help = new FusionChartHelper(FusionChartType.MSLine);
            help.SetDataSource(dt);
            help.ChartHeight = h;
            help.ChartWidth = "100%";
            help.Caption = caption;//主标题，将显示在图形顶端
            help.SubCaption = subtitle;//二级标题，将显示在主标题下方，没有不显示
            help.BgColor = "ffffff";//背景色
            help.CanvasBorderColor = "ffffff";
            help.Decimals = 2;//显示小数位
            var cols = dt.Columns;
            for (int i = 1; i < cols.Count; i++)
            {
                help.SetSeriesName(cols[i].ColumnName, cols[i].ColumnName);
            }
            help.IsFormatNumberScale = false;//是否格式化数字，如1000显示成1K；
            if (max != null)
                help.YMaxValue = max.Value;
            if (min != null)
                help.YMinValue = min.Value;
            help.IsFormatNumberScale = isformatscale;//是否格式化数字，如1000显示成1K；
            help.DefaultNumberScale = DefaultNumberScale;
            help.NumberScaleValue = NumberScaleValue;
            help.NumberScaleUnit = NumberScaleUnit;
            return help.ToString();
        }

        #endregion

        /// <summary>
        /// 设置组织下拉
        /// </summary>
        /// <param name="orgId"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        protected string SetViewBagOrgData(string orgId = null, string level = "4")
        {
            if (level != "4")
            {
                var organDao = new SYS_ORGANIZE_DAL();
                var orgs = organDao.GetListForUserId(UserState.UserID, level);

                ViewBag.Orgs = new SelectList(orgs, "ORG_ID", "ORG_NAM", orgId);

                if (string.IsNullOrEmpty(orgId))
                {
                    if (orgs != null && orgs.Count > 0)
                    {
                        return orgs[0].ORG_ID;
                    }
                }
            }
            else
            {
                var list = SYS_HALL_DAL.GetListByUserId(UserState.UserID);
                ViewBag.Orgs = new SelectList(list, "HALL_NO", "HALL_NAM", orgId);

                if (string.IsNullOrEmpty(orgId))
                {
                    if (list != null && list.Count > 0)
                    {
                        return list[0].HALL_NO;
                    }
                }
            }
            return orgId;
        }

        /// <summary>
        /// 获取下拉年
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        protected List<SelectListItem> GetYearsSelectList(int? value = null)
        {
            var thisYear = DateTime.Now.Year;
            var list = new List<SelectListItem>();

            var length = thisYear - DefaultYears;
            if (length > 4)
            {
                DefaultYears = thisYear - 4;
            }

            for (var i = 0; i < thisYear - DefaultYears + 1; i++)
            {
                var item = (DefaultYears + i);
                if (value != null && value == item)
                {
                    list.Add(new SelectListItem()
                    {
                        Value = item.ToString(),
                        Text = item.ToString(),
                        Selected = true
                    });
                }
                else
                {
                    list.Add(new SelectListItem()
                    {
                        Value = item.ToString(),
                        Text = item.ToString()
                    });
                }
            }
            return list;
        }

        /// <summary>
        /// 获取下拉的月份
        /// </summary>
        /// <param name="isAll"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        protected List<SelectListItem> GetMonthSelectList(bool isAll = true, int? month = null)
        {
            var list = new List<SelectListItem>();
            var defaultMonth = 1;

            if (isAll)
            {
                list.Add(new SelectListItem()
                {
                    Text = "-请选择-",
                    Value = ""
                });
            }

            for (var i = 0; i < 12; i++)
            {
                var m = defaultMonth + i;
                var isSelected = m == month ? true : false;
                list.Add(new SelectListItem()
                {
                    Text = m.ToString(),
                    Value = m.ToString(),
                    Selected = isSelected
                });
            }

            return list;
        }

        /// <summary>
        /// 日期初始化
        /// </summary>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        protected void DateTimeInit(ref DateTime? beginTime, ref DateTime? endTime)
        {
            if (beginTime == null || beginTime == DateTime.MinValue)
            {
                beginTime = DateTime.Now.DefaultBeginDateTime();

            }
            if (endTime == null || endTime == DateTime.MinValue)
            {
                endTime = DateTime.Now.DefaultEndDateTime();

            }
            ViewData["beginTime"] = beginTime.Value.ToString("yyyy-MM-dd");
            ViewData["endTime"] = endTime.Value.ToString("yyyy-MM-dd");

            ViewData["ddlMonth"] = "1";

            var queryStr = HttpContext.Request.QueryString["ddlMonth"];
            if (queryStr != null)
            {
                ViewData["ddlMonth"] = queryStr;
            }
        }


        /// <summary>
        /// 统计明细条件初始化(默认:按时统计)
        /// </summary>
        /// <param name="beginTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="detailType">统计明细类型</param>
        /// <param name="detailXValue">统计明细X轴值</param>
        /// 1:按时统计    X轴值:"9|10|11|12|13|14|15|16|17|18"
        /// 2:按天统计    X轴值:"2014-11-11|2014-11-12|2015-11-13"
        /// 3:按月统计    X轴值:"2014-11-01|2014-12-01|2015-01-01"
        /// 4:按年统计    X轴值:"2014-01-01|2015-01-01|2016-01-01"
        protected void StatDetailTypeInit(ref DateTime? beginTime, ref DateTime? endTime, out int detailType, out string detailXValue)
        {
            detailType = 1;
            detailXValue = "9|10|11|12|13|14|15|16|17|18";
            if (beginTime.HasValue && endTime.HasValue)
            {
                beginTime = beginTime.Value.Date;
                endTime = endTime.Value.Date;
                var totalDays = (endTime.Value - beginTime.Value).Days;
                if (totalDays == 0)
                {
                    detailType = 1;
                }
                else if (totalDays <= 31)
                {
                    detailType = 2;
                }
                else
                {
                    var totalMonths = (endTime.Value.Month - beginTime.Value.Month) +
                                      ((endTime.Value.Year - beginTime.Value.Year) * 12);
                    detailType = totalMonths <= 12 ? 3 : 4;
                }

                switch (detailType)
                {
                    case 2:
                        {
                            var tempDay = beginTime.Value;
                            detailXValue = tempDay.ToString("yyyy-MM-dd");
                            while (tempDay < endTime.Value)
                            {
                                tempDay = tempDay.AddDays(1);
                                detailXValue = string.Format("{0}|{1}", detailXValue, tempDay.ToString("yyyy-MM-dd"));
                            }
                        }
                        break;
                    case 3:
                        {
                            var tempMonth = new DateTime(beginTime.Value.Year, beginTime.Value.Month, endTime.Value.Day);
                            detailXValue = tempMonth.ToString("yyyy-MM-01");
                            while (tempMonth < endTime.Value)
                            {
                                tempMonth = tempMonth.AddMonths(1);
                                detailXValue = string.Format("{0}|{1}", detailXValue, tempMonth.ToString("yyyy-MM-01"));
                            }
                        }
                        break;
                    case 4:
                        {
                            var tempYear = beginTime.Value;
                            detailXValue = tempYear.ToString("yyyy-01-01");
                            while (tempYear.Year < endTime.Value.Year)
                            {
                                tempYear = tempYear.AddYears(1);
                                detailXValue = string.Format("{0}|{1}", detailXValue, tempYear.ToString("yyyy-01-01"));
                            }
                        }
                        break;
                }
            }
            else
            {
                beginTime = DateTime.Now.Date;
                endTime = DateTime.Now.Date;
            }
        }
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
            LogHelper.ErrorLog(string.Join("\r\n", errlist.ToArray()));
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
            if (!ModelState.IsValid)
            {
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
                LogHelper.ErrorLog(string.Join("\r\n", errlist.ToArray()));
            }
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
