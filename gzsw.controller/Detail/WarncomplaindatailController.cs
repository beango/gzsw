using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using gzsw.controller.MyAuth;
using gzsw.dal;
using gzsw.dal.dao;
using gzsw.model;
using gzsw.util;

namespace gzsw.controller.Detail
{
    public class WarncomplaindatailController : BaseController<WARN_COMPLAIN_DETAIL>
    {

        [Ninject.Inject]
        public IDao<WARN_COMPLAIN_DETAIL> WarncomplaindetailDao { get; set; }


        [Ninject.Inject]
        public IDao<SYS_HALL> Halldao { get; set; }


        [Ninject.Inject]
        public IDao<SYS_USER> Userdao { get; set; }

        [Ninject.Inject]
        public IDao<SYS_STAFF> Staffdao { get; set; }


        [Ninject.Inject]
        public IDao<SYS_NSRINFO> nsrdao { get; set; }

        [Ninject.Inject]
        public IDao<WARN_COMPLAIN_TYP_CON> WarncomplaintypedaoDao { get; set; }


        [UserAuth("WARN_COMPLAIN_DETAIL_VIW")]
        public ActionResult Index(string nameorcode, string start, string end, int pageIndex = 1, int pageSize = 20, int status = 0)
        {

            ViewBag.nameorcode = nameorcode;
            ViewBag.status = status;

            DateTime? time1 = null;
            if (!string.IsNullOrEmpty(start))
            {
                DateTime t1;
                DateTime.TryParse(start, out t1);
                time1 = t1;
            }
            DateTime? time2 = null;
            if (!string.IsNullOrEmpty(end))
            {

                DateTime t2;
                DateTime.TryParse(end, out t2);
                time2 = t2;
            }

            ViewBag.start = start;
            ViewBag.end = end;
            var data = new WARN_COMPLAIN_DETAIL_DAL().GetList(pageIndex, pageSize, time1, time2, nameorcode, status == 0 ? (int?)null : status);

            return View(data);
        }

         

        private void GetCreateDt(string org = null)
        {
           

            ViewBag.WarnComplainlist = new SelectList(WarncomplaintypedaoDao.FindList().Select(x => x.COMPLAIN_NAM));


            var organDao = new SYS_ORGANIZE_DAL();
            var orgs = organDao.GetListForUserId(UserState.UserID, "4");

            ViewBag.UserORG = new SelectList(orgs, "ORG_ID", "ORG_NAM", org);
        }
         
        [UserAuth("WARN_COMPLAIN_DETAIL_VIW")]
        public ActionResult Detail(int id)
        {
            var data = WarncomplaindetailDao.GetEntity("SEQ", id);
            var hallname = Halldao.GetEntity("HALL_NO", data.HALL_NO).HALL_NAM;
            var name = nsrdao.GetEntity("NSR_SBM", data.NSR_SBM).NSR_NAME;
            ViewBag.HallName = hallname;
            ViewBag.NSRName = name;
            ViewBag.StaffName = "";
            if (!string.IsNullOrEmpty(data.STAFF_ID))
            {
                ViewBag.StaffName = Staffdao.GetEntity("STAFF_ID", data.STAFF_ID).STAFF_NAM;
            }
            return View(data);
        } 

        public ActionResult Check_NSR(string code)
        {
            var re = nsrdao.GetEntity("NSR_SBM", code);
            if (re == null)
            {
                //return JsonResult(false, "纳税人编码/企业编码不存在！", "WARN");
                return Json(new { isok = false, msg = "纳税人编码/企业编码不存在！" }, JsonRequestBehavior.AllowGet);

            }
            else
            {
                return Json(new { isok = true, msg = "" }, JsonRequestBehavior.AllowGet);
            }
        }
        
    }
}
