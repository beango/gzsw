using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using System.Windows.Forms;
using gzsw.dal;
using gzsw.model.Enums;
using gzsw.util;
using System.Drawing;

namespace gzsw.controller.STAT
{
    /// <summary>
    /// 统计的基础类
    /// </summary>
    public class StatController:BaseController
    {
        /// <summary>
        /// 获取报表标题组织的名称
        /// </summary>
        /// <param name="orgId"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        public string GetOrgName(string orgId,byte? level)
        {
            var orgall = new SYS_USER_DAL().GetUserORG(UserState.UserID);
            var MainTitle = string.Empty;

            if (!string.IsNullOrEmpty(orgId))
            {
                MainTitle = orgall.FirstOrDefault((o => o.ORG_ID == orgId)).ORG_NAM;
            }
            else
            {
                if (level!=null)
                {
                    MainTitle = orgall.Where(m=>m.ORG_LEVEL==2).OrderBy(o => o.ORG_LEVEL).FirstOrDefault().ORG_NAM;
                }
                else
                {
                    MainTitle = orgall.OrderBy(o => o.ORG_LEVEL).FirstOrDefault().ORG_NAM;
                }
            }

            return MainTitle;
        }

        /// <summary>
        /// 获取表格中的标题
        /// </summary>
        /// <param name="orgName"></param>
        /// <param name="serviceName"></param>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <param name="isHeadings">时间是否有小标题</param>
        /// <returns></returns>
        public string GetTitleName(string orgName,string serviceName, DateTime beginTime, DateTime endTime, bool isHeadings = true)
        {
            if (isHeadings)
            {
                orgName +=serviceName+ "<span style='font-size:12px;'>（" + beginTime.ToString("yyyy年MM月dd");
                orgName += " - " + endTime.ToString("yyyy年MM月dd") + "）</span>";
            }
            else
            {
                orgName +=serviceName+ "（" + beginTime.ToString("yyyy年MM月dd");
                orgName += " - " + endTime.ToString("yyyy年MM月dd") + "）";
            }

            return orgName;
        }

        public Color ToColor(string color)
        {

            int red, green, blue = 0;
            char[] rgb;
            color = color.TrimStart('#');
            color = Regex.Replace(color.ToLower(), "[g-zG-Z]", "");
            switch (color.Length)
            {
                case 3:
                    rgb = color.ToCharArray();
                    red = Convert.ToInt32(rgb[0].ToString() + rgb[0].ToString(), 16);
                    green = Convert.ToInt32(rgb[1].ToString() + rgb[1].ToString(), 16);
                    blue = Convert.ToInt32(rgb[2].ToString() + rgb[2].ToString(), 16);
                    return Color.FromArgb(red, green, blue);
                case 6:
                    rgb = color.ToCharArray();
                    red = Convert.ToInt32(rgb[0].ToString() + rgb[1].ToString(), 16);
                    green = Convert.ToInt32(rgb[2].ToString() + rgb[3].ToString(), 16);
                    blue = Convert.ToInt32(rgb[4].ToString() + rgb[5].ToString(), 16);
                    return Color.FromArgb(red, green, blue);
                default:
                    return Color.FromName(color);
            }
        }

        #region 统计图形的方法
        /// <summary>
        /// 创建图形统计
        /// </summary>
        /// <param name="title"></param>
        /// <param name="ds"></param>
        /// <param name="type">默认柱状</param>
        /// <returns></returns>
        public FusionChartHelper CreateFusionChartHelper(string title, DataSet ds, FusionChartType type = FusionChartType.MSColumn3D)
        {
            var help = new FusionChartHelper(type);
            help.SetDataSource(ds);
            help.ChartHeight = 580;
            help.ChartWidth = "100%";
            help.Caption = title;//主标题，将显示在图形顶端
            help.SubCaption = "";//二级标题，将显示在主标题下方，没有不显示
            help.BgColor = "e1f5ff";//背景色
            help.Decimals = 2;//显示小数位
            help.IsFormatNumberScale = false;//是否格式化数字，如1000显示成1K；
            return help;
        }
        public enum TimeSpenEnum
        {
            Year,
            Month,
            Day,
            Hour
        }

        /// <summary>
        /// 设置统计图形的时间显示内容
        /// </summary>
        /// <param name="timeSpan">时间间隔</param>
        /// <param name="bTime">开始时间</param>
        /// <param name="lineTable">表格</param>
        /// <param name="tlist">时间列表</param>
        /// <param name="eTime">结束时间</param>
        public void SetLineYName(TimeSpan timeSpan, DateTime bTime, DataTable lineTable, List<string> tlist, DateTime eTime)
        {
            var re = lineTable.NewRow();
            if (timeSpan.TotalDays > 365)
            {
                //按年份统计
                //计算间隔几年 
                var temp = bTime;
                re = lineTable.NewRow();
                re[0] = bTime.Year + "年";
                lineTable.Rows.Add(re); ;
                tlist.Add(bTime.ToString());
                while (temp.AddYears(1) <= eTime)
                {
                    temp = temp.AddYears(1);
                    re = lineTable.NewRow();
                    re[0] = temp.Year + "年";
                    lineTable.Rows.Add(re); ;
                    tlist.Add(temp.ToString());
                }
                if (temp.Year != eTime.Year)
                {
                    re = lineTable.NewRow();
                    re[0] = eTime.Year + "年";
                    tlist.Add(eTime.ToString());
                }
            }
            if (timeSpan.TotalDays > 31 && timeSpan.TotalDays <= 365)
            {
                //按月份统计

                //计算间隔几月

                var temp = bTime;
                re = lineTable.NewRow();
                re[0] = bTime.Year + "年" + bTime.Month + "月";
                lineTable.Rows.Add(re); ;
                tlist.Add(bTime.ToString());
                while (temp.AddMonths(1) <= eTime)
                {
                    temp = temp.AddMonths(1);
                    tlist.Add(temp.ToString());
                    re = lineTable.NewRow();
                    re[0] = temp.Year + "年" + temp.Month + "月";
                    lineTable.Rows.Add(re); ;
                }
                if (temp.Month != eTime.Month)
                {
                    re = lineTable.NewRow();
                    re[0] = eTime.Year + "年" + eTime.Month + "月";
                    lineTable.Rows.Add(re); ;
                    tlist.Add(eTime.ToString());
                }
            }
            if (timeSpan.TotalDays > 1 && timeSpan.TotalDays <= 31)
            {
                //按日统计
                //计算间隔几日 
                var temp = bTime;
                re = lineTable.NewRow();
                re[0] = bTime.ToString("yyyy-MM-dd");
                lineTable.Rows.Add(re); ;
                tlist.Add(bTime.ToString());
                while (temp.AddDays(1) <= eTime)
                {
                    temp = temp.AddDays(1);
                    tlist.Add(temp.ToString());
                    re = lineTable.NewRow();
                    re[0] = temp.ToString("yyyy-MM-dd");
                    lineTable.Rows.Add(re); ;
                }
                if (temp.Day != eTime.Day)
                {
                    re = lineTable.NewRow();
                    re[0] = eTime.ToString("yyyy-MM-dd");
                    lineTable.Rows.Add(re);
                    tlist.Add(eTime.ToString());
                }
            }
            if (timeSpan.TotalDays < 1 && timeSpan.Hours > 1)
            {
                var temp = bTime;
                re[0] = bTime.Hour + "时";
                lineTable.Rows.Add(re);
                tlist.Add(bTime.ToString());
                while (temp.AddHours(1) <= eTime)
                {
                    temp = temp.AddHours(1);
                    tlist.Add(temp.ToString());
                    re = lineTable.NewRow();
                    re[0] = temp.Hour + "时";
                    lineTable.Rows.Add(re);
                }
                if (temp.Hour != eTime.Hour)
                {
                    re = lineTable.NewRow();
                    re[0] = eTime.Hour + "时";
                    lineTable.Rows.Add(re);
                    tlist.Add(eTime.ToString());
                }
            }
        }

        /// <summary>
        /// 设置统计图形的时间显示内容
        /// </summary>
        /// <param name="timeSpan">时间间隔</param>
        /// <param name="bTime">开始时间</param>
        /// <param name="lineTable">表格</param>
        /// <param name="tlist">时间列表</param>
        /// <param name="eTime">结束时间</param>
        public void SetLineYName(TimeSpan timeSpan, DateTime bTime, DataTable lineTable, List<string> tlist, DateTime eTime, out TimeSpenEnum eTimeSpenEnum)
        {
            eTimeSpenEnum = TimeSpenEnum.Day;
            var re = lineTable.NewRow();
            if (timeSpan.TotalDays > 365)
            {
                //按年份统计
                //计算间隔几年 
                var temp = bTime;
                re = lineTable.NewRow();
                re[0] = bTime.Year + "年";
                lineTable.Rows.Add(re); ;
                tlist.Add(bTime.ToString());
                while (temp.AddYears(1) <= eTime)
                {
                    temp = temp.AddYears(1);
                    re = lineTable.NewRow();
                    re[0] = temp.Year + "年";
                    lineTable.Rows.Add(re); ;
                    tlist.Add(temp.ToString());
                }
                if (temp.Year != eTime.Year)
                {
                    re = lineTable.NewRow();
                    re[0] = eTime.Year + "年";
                    tlist.Add(eTime.ToString());
                }
                eTimeSpenEnum = TimeSpenEnum.Year;
            }
            if (timeSpan.TotalDays > 31 && timeSpan.TotalDays <= 365)
            {
                //按月份统计

                //计算间隔几月

                var temp = bTime;
                re = lineTable.NewRow();
                re[0] = bTime.Year + "年" + bTime.Month + "月";
                lineTable.Rows.Add(re); ;
                tlist.Add(bTime.ToString());
                while (temp.AddMonths(1) <= eTime)
                {
                    temp = temp.AddMonths(1);
                    tlist.Add(temp.ToString());
                    re = lineTable.NewRow();
                    re[0] = temp.Year + "年" + temp.Month + "月";
                    lineTable.Rows.Add(re); ;
                }
                if (temp.Month != eTime.Month)
                {
                    re = lineTable.NewRow();
                    re[0] = eTime.Year + "年" + eTime.Month + "月";
                    lineTable.Rows.Add(re); ;
                    tlist.Add(eTime.ToString());
                }
                eTimeSpenEnum = TimeSpenEnum.Month;
            }
            if (timeSpan.TotalDays > 1 && timeSpan.TotalDays <= 31)
            {
                //按日统计
                //计算间隔几日 
                var temp = bTime;
                re = lineTable.NewRow();
                re[0] = bTime.ToString("yyyy-MM-dd");
                lineTable.Rows.Add(re); ;
                tlist.Add(bTime.ToString());
                while (temp.AddDays(1) <= eTime)
                {
                    temp = temp.AddDays(1);
                    tlist.Add(temp.ToString());
                    re = lineTable.NewRow();
                    re[0] = temp.ToString("yyyy-MM-dd");
                    lineTable.Rows.Add(re); ;
                }
                if (temp.Day != eTime.Day)
                {
                    re = lineTable.NewRow();
                    re[0] = eTime.ToString("yyyy-MM-dd");
                    lineTable.Rows.Add(re);
                    tlist.Add(eTime.ToString());
                }
                eTimeSpenEnum = TimeSpenEnum.Day;
            }
            if (timeSpan.TotalDays < 1 && timeSpan.Hours > 1)
            {
                var temp = bTime;
                re[0] = bTime.Hour + "时";
                lineTable.Rows.Add(re);
                tlist.Add(bTime.ToString());
                while (temp.AddHours(1) <= eTime)
                {
                    temp = temp.AddHours(1);
                    tlist.Add(temp.ToString());
                    re = lineTable.NewRow();
                    re[0] = temp.Hour + "时";
                    lineTable.Rows.Add(re);
                }
                if (temp.Hour != eTime.Hour)
                {
                    re = lineTable.NewRow();
                    re[0] = eTime.Hour + "时";
                    lineTable.Rows.Add(re);
                    tlist.Add(eTime.ToString());
                }
                eTimeSpenEnum = TimeSpenEnum.Hour;
            }
        }
        
        #endregion

        #region 时间操作方法
        /// <summary>
        /// 设置时间
        /// </summary>
        /// <param name="years">年</param>
        /// <param name="month">月</param>
        /// <param name="period">时间段</param>
        /// <param name="quarter">季度</param>
        public void SetData(int years, int? month, int? period, int? quarter)
        {
            ViewBag.Periods = GetPeriodSelectList(true, period);
            ViewBag.Quarters = GetQuarterSelectList(true, quarter);
            ViewBag.Months = GetMonthSelectList(true, month);
            ViewBag.Years = GetYearsSelectList(years);
        }

        /// <summary>
        /// 时间段
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public List<SelectListItem> GetPeriodSelectList(bool isAll = true, int? value = null)
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

            for (var i = 0; i < 24; i++)
            {
                var m = defaultMonth + i;
                var isSelected = m == value;
                list.Add(new SelectListItem()
                {
                    Text = i + "点至" + m + "点",
                    Value = m.ToString(),
                    Selected = isSelected
                });
            }

            return list;
        }

        /// <summary>
        /// 季度
        /// </summary>
        /// <param name="isAll"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public List<SelectListItem> GetQuarterSelectList(bool isAll = true, int? value = null)
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
            for (var i = 0; i < 4; i++)
            {
                var m = defaultMonth + i;
                var isSelected = m == value;
                list.Add(new SelectListItem()
                {
                    Text = "第" + m + "季度",
                    Value = m.ToString(),
                    Selected = isSelected
                });
            }
            return list;
        }

        /// <summary>
        /// 获取开始时间
        /// </summary>
        /// <param name="years">年</param>
        /// <param name="quarter">季度</param>
        /// <param name="month">月</param>
        /// <returns></returns>
        public DateTime GetBeginData(int years, int? quarter, int? month = 1)
        {
            if (quarter == null && month == null)
            {
                return new DateTime(years, 1, 1);
            }
            if (month != null)
            {
                return new DateTime(years, month.GetValueOrDefault(), 1);
            }
            switch ((QuarterEnum)quarter)
            {
                case QuarterEnum.第一季度:
                    month = 1;
                    break;
                case QuarterEnum.第二季度:
                    month = 4;
                    break;
                case QuarterEnum.第三季度:
                    month = 7;
                    break;
                case QuarterEnum.第四季度:
                    month = 10;
                    break;
            }
            return new DateTime(years, month.GetValueOrDefault(), 1);
        }

        /// <summary>
        /// 获取结束时间
        /// </summary>
        /// <param name="years">年</param>
        /// <param name="quarter">季度</param>
        /// <param name="month">月</param>
        /// <returns></returns>
        public DateTime GetEndData(int years, int? quarter, int? month = 1)
        {
            var d = new DateTime(years, 1, 1);
            if (quarter == null && month == null)
            {
                var dd = d.AddYears(1);
                return dd.AddDays(-1);
            }
            if (month != null)
            {
                d = new DateTime(years, month.GetValueOrDefault(), 1);
                d = d.AddMonths(1);
                return d.AddDays(-1);
            }
            switch ((QuarterEnum)quarter)
            {
                case QuarterEnum.第一季度:
                    month = 3;
                    break;
                case QuarterEnum.第二季度:
                    month = 6;
                    break;
                case QuarterEnum.第三季度:
                    month = 9;
                    break;
                case QuarterEnum.第四季度:
                    month = 12;
                    break;
            }
            d = new DateTime(years, month.GetValueOrDefault(), 1);
            d = d.AddMonths(1);
            return d.AddDays(-1);
        } 
        #endregion
    }
}
