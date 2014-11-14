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

namespace gzsw.controller.WARN
{
    public class WarnalarminfoController : BaseController<WARN_ALARM_INFO_DETAIL>
    {

        [Ninject.Inject]
        public IDao<WARN_ALARM_INFO_DETAIL> WarnalarminfodetailDao { get; set; }


        [Ninject.Inject]
        public IDao<SYS_HALL> Halldao { get; set; }


        [Ninject.Inject]
        public IDao<SYS_STAFF> Staffdao { get; set; }



        [Ninject.Inject]
        public IDao<WARN_ALARM_SENDINFO_DETAIL> WarnalarmsendinfodetailDao { get; set; }

        [UserAuth("WARN_ALARM_INFO_DETAIL_VIW")]
        public ActionResult Index(string hall,string start,string end,int status=0, int pageIndex = 1, int pageSize = 20)
        {
            var orgall = new SYS_USER_DAL().GetUserORG(UserState.UserID);
            ViewBag.UserORG = new SelectList(orgall.Where(obj => obj.ORG_LEVEL == 4), "ORG_ID", "ORG_NAM", false);
            int type = 0;
            ViewBag.hall = hall;
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
            var data = new WARN_ALARM_INFO_DETAIL_DAL().GetList(pageIndex, pageSize, "", time1, time2, type == 0 ? (int?)null : type, status == 0 ? (int?)null : status);
            return View(data);
        }

         [UserAuth("WARN_ALARM_INFO_DETAIL_VIW")]
        public ActionResult Details(int id)
        {
            var data = WarnalarminfodetailDao.GetEntity("ALARM_SEQ", id);
            var hallname = Halldao.GetEntity("HALL_NO", data.HALL_NO).HALL_NAM; 
            ViewBag.HallName = hallname; 
            ViewBag.StaffName = "";
            if (!string.IsNullOrEmpty(data.STAFF_ID))
            {
                ViewBag.StaffName = Staffdao.GetEntity("STAFF_ID", data.STAFF_ID).STAFF_NAM;
            }
            return View(data);
        }


        [UserAuth("WARN_ALARM_INFO_DETAIL_EDT")]
        public ActionResult Edit(int id)
        {
            var data = WarnalarminfodetailDao.GetEntity("ALARM_SEQ", id);
            var hallname = Halldao.GetEntity("HALL_NO", data.HALL_NO).HALL_NAM;
            ViewBag.HallName = hallname;
            ViewBag.StaffName = "";
            if (!string.IsNullOrEmpty(data.STAFF_ID))
            {
                ViewBag.StaffName = Staffdao.GetEntity("STAFF_ID", data.STAFF_ID).STAFF_NAM;
            }
            ViewBag.STATE = EnumHelper.GetCategorySelectList(typeof(gzsw.model.ext.WARN_ALARM_INFO_DETAIL.DETAIL_STATUS_ENUM), false);
            return View(data);
        }

        [HttpPost]
        [UserAuth("WARN_ALARM_INFO_DETAIL_EDT")]
        public ActionResult Edit(WARN_ALARM_INFO_DETAIL model)
        {

            ViewBag.STATE = EnumHelper.GetCategorySelectList(typeof(gzsw.model.ext.WARN_ALARM_INFO_DETAIL.ALARM_TYP_ENUM));
            try
            {

                var data = WarnalarminfodetailDao.GetEntity("ALARM_SEQ", model.ALARM_SEQ);
                data.ALARM_REASON = model.ALARM_REASON;
                data.ALARM_METHOD = model.ALARM_METHOD;
                data.STATE =2;  //默认为已处理 
                var rst = WarnalarminfodetailDao.UpdateObject(data);
                if (rst > 0)
                {
                    Alter("修改成功！", util.Enum.AlterTypeEnum.Success, false, true);
                    return Redirect("/Home/Blank");
                }
                ModelState.AddModelError("", "修改失败。");
                return View(model);
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("修改出错！", ex);
                ModelState.AddModelError("", "修改失败！" + ex.Message);
                return RedirectToAction("Error", "Home");
            }
        }

        [UserAuth("WARN_ALARM_INFO_DETAIL_VIW")]
        public ActionResult List(int id, string name, int pageIndex = 1, int pageSize = 20)
        {
            var data = WarnalarminfodetailDao.GetEntity("ALARM_SEQ", id);
            ViewBag.SENDINFO_DETAIL_ID = id;
            //ViewBag.HALL_NO = data.HALL_NO;
            ViewBag.Name = name;
            var listdate = new WARN_ALARM_INFO_DETAIL_DAL().GetSendInfoList(name,pageIndex, pageSize);
            return View(listdate);
        }

        [UserAuth("WARN_ALARM_INFO_DETAIL_VIW")]
        public ActionResult DetailListByHall(string hallno, int pageIndex = 1, int pageSize = 20)
        {
            ViewBag.HALL_NO = hallno;
            var listdate = new WARN_ALARM_INFO_DETAIL_DAL().GetListByHall(hallno, pageIndex, pageSize);
            return View(listdate);
        }
    }
}
