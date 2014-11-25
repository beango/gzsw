using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using gzsw.controller.MyAuth;
using gzsw.dal.dao;
using gzsw.model;

namespace gzsw.controller.CHK
{
    /// <summary>
    /// 个人考核评定
    ///     综合评定
    /// </summary>
    public class StaffCompreEvalMController : BaseController<CHK_STAFF_COMPRE_EVAL_M>
    {

        [UserAuth("CHK_STAFF_COMPRE_EVAL_M_VIW")]
        public ActionResult Index(int? years, int? month,int? endMonth,string orgId, int pageIndex = 1, int pageSize = 20)
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
            ViewBag.Months = GetMonthSelectList(true,month);
            ViewBag.Years = GetYearsSelectList(years);

            ViewBag.Stat = stat;
            ViewBag.Endstat = endstat;

            var list = CHK_STAFF_COMPRE_EVAL_M_DAL.GetListSub(stat, endstat, orgId, pageIndex, pageSize);

            return View(list);
        }


        /// <summary>
        /// 业务分 详细
        /// </summary>
        /// <param name="stat">年月份</param>
        /// <param name="endStat"></param>
        /// <param name="staffId"></param>
        /// <returns></returns>
        [UserAuth("CHK_STAFF_COMPRE_EVAL_M_VIW")]
        public ActionResult DetailSVR(int stat,int endStat, string staffId, int pageIndex = 1, int pageSize = 20)
        {
            var list = STAT_STAFF_SVRSTAT_M_DAL.GetListBub(stat, endStat, staffId, pageIndex, pageSize);

            ViewBag.Total = STAT_STAFF_SVRSTAT_M_DAL.GetTotal(stat, endStat, staffId);

            return View(list);
        }

        /// <summary>
        /// 质量分 详细
        /// </summary>
        /// <param name="mo"></param>
        /// <param name="staffId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [UserAuth("CHK_STAFF_COMPRE_EVAL_M_VIW")]
        public ActionResult DetailQUALITY(int stat, int endStat, string staffId, int pageIndex = 1, int pageSize = 20)
        {
            var list = STAT_STAFF_QUALITYSTAT_M_DAL.GetListBub(stat, endStat, staffId, pageIndex, pageSize);

            ViewBag.Total = STAT_STAFF_QUALITYSTAT_M_DAL.GetTotal(stat, endStat, staffId);

            return View(list);
        }

        /// <summary>
        /// 效率分
        /// </summary>
        /// <param name="mo"></param>
        /// <param name="staffId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [UserAuth("CHK_STAFF_COMPRE_EVAL_M_VIW")]
        public ActionResult DetailEFFIC(int mo, string staffId, int pageIndex = 1, int pageSize = 20)
        {
            return View();
        }

        /// <summary>
        /// 评价分
        /// </summary>
        /// <param name="mo"></param>
        /// <param name="staffId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [UserAuth("CHK_STAFF_COMPRE_EVAL_M_VIW")]
        public ActionResult DetailEVAL(int stat, int endStat, string staffId, int pageIndex = 1, int pageSize = 20)
        {
            var list = STAT_STAFF_EVALSTAT_M_DAL.GetListSub(stat, endStat, staffId, pageIndex, pageSize);

            ViewBag.Total = STAT_STAFF_EVALSTAT_M_DAL.GetTotal(stat, endStat, staffId);
            return View(list);
        }

        /// <summary>
        /// 考勤分
        /// </summary>
        /// <param name="mo"></param>
        /// <param name="staffId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [UserAuth("CHK_STAFF_COMPRE_EVAL_M_VIW")]
        public ActionResult DetailATTEND(int stat, int endStat, string staffId, int pageIndex = 1, int pageSize = 20)
        {
            var list = STAT_STAFF_CHKSTAT_M_DAL.GetListSub(stat, endStat, staffId, pageIndex, pageSize);
            ViewBag.Total = STAT_STAFF_CHKSTAT_M_DAL.GetTotal(stat, endStat, staffId);
            return View(list);
        }

        /// <summary>
        /// 日常行为
        /// </summary>
        /// <param name="mo"></param>
        /// <param name="staffId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [UserAuth("CHK_STAFF_COMPRE_EVAL_M_VIW")]
        public ActionResult DetailUSUACTD(int stat, int endStat, string staffId, int pageIndex = 1, int pageSize = 20)
        {
            var list = CHK_STAFF_USU_ACT_MARK_DAL.GetList(stat, endStat, staffId, pageIndex, pageSize);
            ViewBag.Total = CHK_STAFF_USU_ACT_MARK_DAL.GetTotal(stat, endStat, staffId);
            return View(list);
        }

        /// <summary>
        /// 综合评定分
        /// </summary>
        /// <param name="mo"></param>
        /// <param name="staffId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [UserAuth("CHK_STAFF_COMPRE_EVAL_M_VIW")]
        public ActionResult DetailCOMPRESAN(int stat, int endStat, string staffId, int pageIndex = 1, int pageSize = 20)
        {
            var list = CHK_STAFF_COMPRE_SAN_MARK_DAL.GetList(stat, endStat, staffId, pageIndex, pageSize);
            ViewBag.Total = CHK_STAFF_COMPRE_SAN_MARK_DAL.GetTotal(stat, endStat, staffId);
            return View(list);
        }
    }
}
