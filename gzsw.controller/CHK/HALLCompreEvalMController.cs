using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using gzsw.controller.MyAuth;
using gzsw.dal;
using gzsw.dal.dao;
using gzsw.model;

namespace gzsw.controller.CHK
{
    public class HALLCompreEvalMController : BaseController<CHK_HALL_STAT_M>
    {


        [Ninject.Inject]
        public IDao<SYS_ORGANIZE> orgdao { get; set; }
        [UserAuth("CHK_HALL_COMPRE_EVAL_M_VIW")]
        public ActionResult Index(int? years, int? month,int? endMonth,string orgId, int pageIndex = 1, int pageSize = 20)
        {
            //month
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
            //.Select(x => x.HALL_NO).ToArray();
            var halllist = new List<string>();
            if (string.IsNullOrEmpty(orgId))
            {
                ViewBag.orgid = "";
                ViewBag.orgName = "";
               
            }
            else
            {
                halllist = new SYS_HALL_DAL().GetOrgHallAndChild(orgId);
                ViewBag.orgId = orgId;
                ViewBag.orgName = "";
                var orginfo = orgdao.GetEntity("ORG_ID", orgId);
                if (orginfo != null)
                {
                    ViewBag.orgName = orginfo.ORG_NAM;
                }
            }
            ViewBag.EndMonths = GetMonthSelectList(true, endMonth);
            ViewBag.Months = GetMonthSelectList(true, month);
            ViewBag.Years = GetYearsSelectList(years);

            ViewBag.Stat = stat;
            ViewBag.Endstat = endstat;


            //GetListSubByP
            if (month == endMonth)
            {
                var list = CHK_HALL_STAT_M_DAL.GetListSub(stat, halllist.ToArray(), pageIndex, pageSize);
                return View(list);
            }
            else
            {
                var list = CHK_HALL_STAT_M_DAL.GetListSubByP(stat, endstat, orgId);
                return View(list);
            }
           
        }

        public ActionResult DetailAttend(int mo, int endStat, string hallno, int pageIndex = 1, int pageSize = 20)
        { 
                  var beginMo = mo;
                  var endMo = endStat;
            if (mo.ToString().Length == 4)
            {
                beginMo = int.Parse(mo.ToString() + "01");
                endMo = int.Parse(mo.ToString() + "12");
            }
            var list = STAT_STAFF_CHKSTAT_M_DAL.GetListBubByHall(beginMo, endMo, hallno, pageIndex, pageSize);
            
            return View(list); 
        }

        public ActionResult DetailOther(int mo,int endStat, string hallno,int type, int pageIndex = 1, int pageSize = 20)
        {

            var endMoyear = endStat/100; 
            var year = mo/100;
            var beginMo = Convert.ToDateTime(year.ToString()+"-"+(mo-year*100).ToString() + "-1");
            var endMo = Convert.ToDateTime(endMoyear.ToString() + "-" + (endStat - year * 100).ToString() + "-1").AddMonths(1).AddSeconds(-1);

            var list = CHK_HALL_ITEM_MARK_DAL.GetListBub(beginMo, endMo, hallno, type, pageIndex, pageSize);

            //ViewBag.Total = CHK_HALL_ITEM_MARK_DAL.GetTotal(beginMo, endMo, hallno);

            return View(list);
        }

        public ActionResult DetailQueuedetain(int mo, int endStat, string hallno, int pageIndex = 1, int pageSize = 20)
        {
            var endMoyear = endStat / 100;
            var year = mo / 100;
            var beginMo = Convert.ToDateTime(year.ToString() + "-" + (mo - year * 100).ToString() + "-1");
            var endMo = Convert.ToDateTime(endMoyear.ToString() + "-" + (endStat - year * 100).ToString() + "-1").AddMonths(1).AddSeconds(-1);
            var list = STAT_STAFF_QUEUE_BUSI_D_DAL.GetListBubByHall(beginMo, endMo, hallno, pageIndex, pageSize);

            return View(list); 
        }

        public ActionResult DetailHandleontime(int mo, int endStat, string hallno, int pageIndex = 1, int pageSize = 20)
        {
          
            //STAT_STAFF_QUEUE_BUSI_D
            var endMoyear = endStat / 100;
            var year = mo / 100;
            var beginMo = Convert.ToDateTime(year.ToString() + "-" + (mo - year * 100).ToString() + "-1");
            var endMo = Convert.ToDateTime(endMoyear.ToString() + "-" + (endStat - year * 100).ToString() + "-1").AddMonths(1).AddSeconds(-1);
            var list = STAT_STAFF_LARGE_BUSI_D_DAL.GetListBubByHall(beginMo, endMo, hallno, pageIndex, pageSize);

            return View(list); 
        }

        public ActionResult DetailQuality(int mo,int endStat, string hallno, int pageIndex = 1, int pageSize = 20)
        {

            //STAT_STAFF_QUEUE_BUSI_D
            var endMoyear = endStat / 100;
            var year = mo / 100;
            var beginMo = Convert.ToDateTime(year.ToString() + "-" + (mo - year * 100).ToString() + "-1");
            var endMo = Convert.ToDateTime(endMoyear.ToString() + "-" + (endStat - year * 100).ToString() + "-1").AddMonths(1).AddSeconds(-1);
            var list = CHK_STAFF_QUALITY_MARK_DAL.GetListBubByHall(beginMo, endMo, hallno, pageIndex, pageSize);

            return View(list);
        }

        public ActionResult DetailEvalsatisfy(int mo,int endStat, string hallno, int pageIndex = 1, int pageSize = 20)
        {
            //STAT_STAFF_QUEUE_BUSI_D
            //STAT_STAFF_QUEUE_BUSI_D
            var endMoyear = endStat / 100;
            var year = mo / 100;
            var beginMo = Convert.ToDateTime(year.ToString() + "-" + (mo - year * 100).ToString() + "-1");
            var endMo = Convert.ToDateTime(endMoyear.ToString() + "-" + (endStat - year * 100).ToString() + "-1").AddMonths(1).AddSeconds(-1);
            var list = STAT_STAFF_BUSI_TOT_D_DAL.GetListBubByHall(beginMo, endMo, hallno, pageIndex, pageSize);

            return View(list);
        }

        public ActionResult DetailComplain(int mo, int endStat, string hallno, int pageIndex = 1, int pageSize = 20)
        {
            //STAT_STAFF_QUEUE_BUSI_D
            var endMoyear = endStat / 100;
            var year = mo / 100;
            var beginMo = Convert.ToDateTime(year.ToString() + "-" + (mo - year * 100).ToString() + "-1");
            var endMo = Convert.ToDateTime(endMoyear.ToString() + "-" + (endStat - year * 100).ToString() + "-1").AddMonths(1).AddSeconds(-1);
            var list = WARN_COMPLAIN_DETAIL_DAL.GetListBubByHall(beginMo, endMo, hallno, pageIndex, pageSize);

            return View(list);
        }
         
    }
}
