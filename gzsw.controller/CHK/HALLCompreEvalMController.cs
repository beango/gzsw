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
    public class HALLCompreEvalMController : BaseController<CHK_HALL_STAT_M>
    {


        [UserAuth("CHK_HALL_COMPRE_EVAL_M_VIW")]
        public ActionResult Index(int? years, int? month, string orgId, int pageIndex = 1, int pageSize = 20)
        {
            var stat = years.GetValueOrDefault();
            if (years == null)
            {
                years = DateTime.Now.Year;
                stat = years.GetValueOrDefault();
                if (month == null)
                {
                    month = DateTime.Now.Month;
                }
            }

            if (month != null)
            {
                int.TryParse(stat.ToString() + month.ToString(), out stat);
            }

            if (string.IsNullOrEmpty(orgId))
            {
                orgId = SetViewBagOrgData();
            }
            else
            {
                SetViewBagOrgData(orgId);
            }

            ViewBag.Months = GetMonthSelectList(true, month);
            ViewBag.Years = GetYearsSelectList(years);



            var list = CHK_HALL_STAT_M_DAL.GetListSub(stat, orgId, pageIndex, pageSize);

            return View(list);
        }

        public ActionResult DetailAttend(int mo, string hallno, int pageIndex = 1, int pageSize = 20)
        { 
                  var beginMo = mo;
            var endMo = mo;
            if (mo.ToString().Length == 4)
            {
                beginMo = int.Parse(mo.ToString() + "01");
                endMo = int.Parse(mo.ToString() + "12");
            }
            var list = STAT_STAFF_CHKSTAT_M_DAL.GetListBubByHall(beginMo, endMo, hallno, pageIndex, pageSize);
            
            return View(list); 
        }

        public ActionResult DetailOther(int mo, string hallno,int type, int pageIndex = 1, int pageSize = 20)
        {
            
          //  var endMo = mo; 
            var year = mo/100;
            var beginMo = Convert.ToDateTime(year.ToString()+"-"+(mo-year*100).ToString() + "-1");
            var endMo = beginMo.AddMonths(1).AddSeconds(-1);
                  
            var list = CHK_HALL_ITEM_MARK_DAL.GetListBub(beginMo, endMo, hallno, type, pageIndex, pageSize);

            //ViewBag.Total = CHK_HALL_ITEM_MARK_DAL.GetTotal(beginMo, endMo, hallno);

            return View(list);
        }

        public ActionResult DetailQueuedetain(int mo, string hallno, int pageIndex = 1, int pageSize = 20)
        {
            var year = mo / 100;
            var beginMo = Convert.ToDateTime(year.ToString() + "-" + (mo - year * 100).ToString() + "-1");
            var endMo = beginMo.AddMonths(1).AddSeconds(-1);
            var list = STAT_STAFF_QUEUE_BUSI_D_DAL.GetListBubByHall(beginMo, endMo, hallno, pageIndex, pageSize);

            return View(list); 
        }

        public ActionResult DetailHandleontime(int mo, string hallno, int pageIndex = 1, int pageSize = 20)
        {
          
            //STAT_STAFF_QUEUE_BUSI_D
            var year = mo / 100;
            var beginMo = Convert.ToDateTime(year.ToString() + "-" + (mo - year * 100).ToString() + "-1");
            var endMo = beginMo.AddMonths(1).AddSeconds(-1);
            var list = STAT_STAFF_LARGE_BUSI_D_DAL.GetListBubByHall(beginMo, endMo, hallno, pageIndex, pageSize);

            return View(list); 
        }

        public ActionResult DetailQuality(int mo, string hallno, int pageIndex = 1, int pageSize = 20)
        {

            //STAT_STAFF_QUEUE_BUSI_D
            var year = mo / 100;
            var beginMo = Convert.ToDateTime(year.ToString() + "-" + (mo - year * 100).ToString() + "-1");
            var endMo = beginMo.AddMonths(1).AddSeconds(-1);
            var list = CHK_STAFF_QUALITY_MARK_DAL.GetListBubByHall(beginMo, endMo, hallno, pageIndex, pageSize);

            return View(list);
        }

        public ActionResult DetailEvalsatisfy(int mo, string hallno, int pageIndex = 1, int pageSize = 20)
        {
            //STAT_STAFF_QUEUE_BUSI_D
            var year = mo / 100;
            var beginMo = Convert.ToDateTime(year.ToString() + "-" + (mo - year * 100).ToString() + "-1");
            var endMo = beginMo.AddMonths(1).AddSeconds(-1);
            var list = STAT_STAFF_BUSI_TOT_D_DAL.GetListBubByHall(beginMo, endMo, hallno, pageIndex, pageSize);

            return View(list);
        }

        public ActionResult DetailComplain(int mo, string hallno, int pageIndex = 1, int pageSize = 20)
        {
            var year = mo / 100;
            var beginMo = Convert.ToDateTime(year.ToString() + "-" + (mo - year * 100).ToString() + "-1");
            var endMo = beginMo.AddMonths(1).AddSeconds(-1);
            var list = WARN_COMPLAIN_DETAIL_DAL.GetListBubByHall(beginMo, endMo, hallno, pageIndex, pageSize);

            return View(list);
        }
         
    }
}
