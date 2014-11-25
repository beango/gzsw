using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using gzsw.controller.MyAuth;
using gzsw.dal.dao;

namespace gzsw.controller.CHK
{
    /// <summary>
    /// 奖金分配
    /// </summary>
    public class BonusController:BaseController
    {
        [UserAuth("CHK_BonusDistribution_VIW")]
        public ActionResult Index(int? years, int? month,int?endMonth, string orgId,decimal sonus=0, int pageIndex = 1, int pageSize = 20)
        {
            var stat = years.GetValueOrDefault();
            var endstat = stat;
            if (years == null)
            {
                years = DateTime.Now.Year;
                stat = years.GetValueOrDefault();
                endstat = years.GetValueOrDefault();
                if (month == null)
                {
                    month = DateTime.Now.Month;
                    endMonth = DateTime.Now.Month;
                }
            }

            if (month != null)
            {
                var sm = month.ToString();
                if (month < 10)
                {
                    sm = "0" + month;
                }
                int.TryParse(stat.ToString() + sm, out stat);
                if (endMonth == null)
                {
                    endstat = stat;
                }
            }
            else
            {
                stat = int.Parse(stat.ToString() + "01");
                if (endMonth == null)
                {
                    endstat = int.Parse(endstat + "12");
                }
            }

            if (endMonth != null)
            {
                var sm = endMonth.ToString();
                if (endMonth < 10)
                {
                    sm = "0" + endMonth;
                }
                int.TryParse(endstat.ToString() + sm, out endstat);
            }


            if (string.IsNullOrEmpty(orgId))
            {
                orgId = SetViewBagOrgData();
            }
            else
            {
                SetViewBagOrgData(orgId);
            }
            ViewBag.EndMonths = GetMonthSelectList(true, endMonth);
            ViewBag.Months = GetMonthSelectList(true, month);
            ViewBag.Years = GetYearsSelectList(years);

            ViewBag.Sonus = sonus.ToString("N");
            ViewBag.SonusNum = sonus;

            ViewBag.Stat = stat;
            ViewBag.Endstat = endstat;


            var list = CHK_STAFF_COMPRE_EVAL_M_DAL.GetListSub(stat, endstat, orgId, pageIndex, pageSize);
            var obj = CHK_STAFF_COMPRE_EVAL_M_DAL.GetListTotalScore(stat, endstat, orgId);
            var total = obj.Total;
            var count = obj.Count;

            ViewBag.AverageScore = 0;
            if (list.TotalItems > 0 && count>0)
            {
                ViewBag.AverageScore = total / count;
            }

            return View(list);
        }
    }
}
