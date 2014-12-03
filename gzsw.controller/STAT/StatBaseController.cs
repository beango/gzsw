using gzsw.dal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace gzsw.controller.STAT
{
    public enum ENUM_STATLV
    {
        省市 = 1, 服务厅 = 2, 员工 = 3
    }
    public class StatBaseController : BaseController
    {
        /// <summary>
        /// 报表名称，如“排队叫号报表”
        /// </summary>
        protected string StatNAM { get; set; }

        /// <summary>
        /// 报表URL，如“/STAT/StatStaffCall”
        /// </summary>
        protected string StatURL { get; set; }

        /// <summary>
        /// 报表级别，1省市报表，2服务厅报表，3员工报表
        /// </summary>
        protected ENUM_STATLV LV
        {
            get
            {
                try
                {
                    var _lv = 3;//员工报表
                    if (string.IsNullOrEmpty(Request.QueryString["t"])
                        || !int.TryParse(Request.QueryString["t"], out _lv))
                    {
                        _lv = 1;//默认省市报表
                    }
                    return (ENUM_STATLV)Enum.Parse(typeof(ENUM_STATLV), _lv.ToString());
                }
                catch (Exception)
                {
                    return ENUM_STATLV.省市;
                }
            }
        }

        /// <summary>
        /// 获取报表标题，如“江西省地方税务局排队叫号分析（2014年11月23日 - 2014年11月29日）”
        /// </summary>
        /// <param name="orgid"></param>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        protected string GetStatTitle(string orgid, DateTime? beginTime, DateTime? endTime)
        {
            var orgall = new SYS_USER_DAL().GetUserORG(UserState.UserID);
            string _statTitle = "";
            if (!string.IsNullOrEmpty(orgid))
            {
                var d = orgall.FirstOrDefault(o => o.ORG_ID == orgid);
                if (null != d)
                    _statTitle += d.ORG_NAM;
            }
            else
            {
                var d = orgall.OrderBy(o => o.ORG_LEVEL).FirstOrDefault();
                if (null != d)
                    _statTitle += d.ORG_NAM;
            }

            _statTitle += StatNAM;
            if (beginTime != null && endTime != null)
            {
                _statTitle += "<span style='font-size:12px;'>（" + beginTime.Value.ToString("yyyy年MM月dd日");
                _statTitle += " - " + endTime.Value.ToString("yyyy年MM月dd日") + "）</span>";
            }
            return _statTitle;
        }

        /// <summary>
        /// 检查是否有访问更高级别报表的权限，没有则跳转到下一级别的报表
        /// </summary>
        protected void CHK_AUTH()
        {
            if (GetHighLV == model.Enums.UserLV_ENUM.市级 && (int)LV < (int)ENUM_STATLV.服务厅)//判断是否有权限
            {
                Response.Redirect(StatURL+"?t="+(int)ENUM_STATLV.服务厅);
            }
            if (GetHighLV == model.Enums.UserLV_ENUM.区级 && (int)LV < (int)ENUM_STATLV.服务厅)//判断是否有权限
            {
               Response.Redirect(StatURL + "?t="+(int)ENUM_STATLV.服务厅);
            }
            if (GetHighLV == model.Enums.UserLV_ENUM.服务厅级 && (int)LV < (int)ENUM_STATLV.员工)//判断是否有权限
            {
                Response.Redirect(StatURL + "?t="+(int)ENUM_STATLV.员工);
            }
            if (GetHighLV == model.Enums.UserLV_ENUM.无权限)//判断是否有权限
            {
                Response.Redirect("/Account/NoAuth");
            }
        }
    }
}
