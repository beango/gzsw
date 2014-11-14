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
    public class WarnComplainDetailController : BaseController<WARN_COMPLAIN_DETAIL>
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
           
            DateTime? time1=null;
            if (!string.IsNullOrEmpty(start))
            {
                DateTime t1;
                DateTime.TryParse(start,out t1);
                time1 = t1;
            }
            DateTime? time2=null;
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

       
        [HttpGet]
        [UserAuth("WARN_COMPLAIN_DETAIL_ADD")]
        public ActionResult Create()
        {
            GetCreateDt();
                var temp = new WARN_COMPLAIN_DETAIL();
            temp.INPUT_USER = UserState.UserName;
            temp.INPUT_TIME = DateTime.Now;
            return View(temp);
        }

        private void GetCreateDt(string org = null)
        {
            //ViewBag.STAR_LEVELLIST = EnumHelper.GetCategorySelectList(typeof(SYS_STAFF.STAR_LEVEL_ENUM));
            //ViewBag.STAFF_TYPLIST = EnumHelper.GetCategorySelectList(typeof(SYS_STAFF.STAFF_TYP_ENUM));
             


            ViewBag.WarnComplainlist = new SelectList(WarncomplaintypedaoDao.FindList().Select(x => x.COMPLAIN_NAM));
         

            var organDao = new SYS_ORGANIZE_DAL();
            var orgs = organDao.GetListForUserId(UserState.UserID, "4"); 

            ViewBag.UserORG = new SelectList(orgs, "ORG_ID", "ORG_NAM", org);
        }

        [HttpPost]
        [UserAuth("WARN_COMPLAIN_DETAIL_ADD")]
        public ActionResult Create(WARN_COMPLAIN_DETAIL model)
        {
            GetCreateDt(); 
            if (ModelState.IsValid)
            {
                try
                {
                    var item = new WARN_COMPLAIN_DETAIL()
                    {
                        COMPLAIN_METHOD=model.COMPLAIN_METHOD,
                        
                        HALL_NO = model.HALL_NO, 
                        COMPLAIN_NAM = model.COMPLAIN_NAM,
                        COMPLAIN_PRO = model.COMPLAIN_PRO,
                        COMPLAIN_REASON = model.COMPLAIN_REASON,
                        COMPLAIN_TIME = model.COMPLAIN_TIME,
                        INPUT_USER = UserState.UserName,
                        INPUT_TIME = DateTime.Now,
                        NSR_SBM=model.NSR_SBM,
                        STAFF_ID=model.STAFF_ID,
                        STATE=1   //默认未处理
                    };

                    var rst = WarncomplaindetailDao.AddObject(item);
                    if (null != rst)
                    {
                        Alter("新增成功！", util.Enum.AlterTypeEnum.Success, false, true);
                        return Redirect("/Home/Blank");
                    }

                    ModelState.AddModelError("", "新增失败。");
                    return View(model);
                }
                catch (Exception ex)
                {
                    LogHelper.ErrorLog("新增出错。", ex);
                    ModelState.AddModelError("", "新增出错。");
                    return View(model);
                }
            }
            return View(model); 
        }


        [UserAuth("WARN_COMPLAIN_DETAIL_VIW")]
        public ActionResult Detail(int id)
        {
            var data = WarncomplaindetailDao.GetEntity("SEQ",id);
            var hallname = Halldao.GetEntity("HALL_NO", data.HALL_NO).HALL_NAM;
            var name = nsrdao.GetEntity("NSR_SBM", data.NSR_SBM).NSR_NAME;
            ViewBag.HallName = hallname;
            ViewBag.NSRName = name;
            ViewBag.StaffName = "";
            if (!string.IsNullOrEmpty(data.STAFF_ID))
            {
              ViewBag.StaffName=  Staffdao.GetEntity("STAFF_ID", data.STAFF_ID).STAFF_NAM;
            }
            return View(data);
        }

        [UserAuth("WARN_COMPLAIN_DETAIL_DEL")]
        public ActionResult Delete(string id)
        {
            try
            {
                var arrid = id.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var _id in arrid)
                {
                   // WarncomplaindetailDao.Delete("SEQ", int.Parse(_id.Split('-')[0]));
                    var data = WarncomplaindetailDao.GetEntity("SEQ", _id);
                    data.STATE = 3;
                    var rst = WarncomplaindetailDao.UpdateObject(data);
                     
                }
                Alter("撤销成功！", util.Enum.AlterTypeEnum.Success, false, true);
                return RedirectToAction("/");

            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("删除出错", ex);
                return Redirect("/Home/Error");
            } 
        }

        public ActionResult Check_NSR(string code)
        {
            var re = nsrdao.GetEntity("NSR_SBM", code);
            if (re == null)
            {
                //return JsonResult(false, "纳税人编码/企业编码不存在！", "WARN");
                 return Json(new { isok = false, msg = "纳税人编码/企业编码不存在！" },JsonRequestBehavior.AllowGet);
                
            }
            else
            {
                return Json(new { isok = true, msg = "" }, JsonRequestBehavior.AllowGet);
            }
        }


        [UserAuth("WARN_COMPLAIN_DETAIL_EDT")]
        public ActionResult Edit(int id)
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
            ViewBag.STATE = EnumHelper.GetCategorySelectList(typeof(gzsw.model.ext.WARN_COMPLAIN_DETAIL.DETAIL_STATUS_ENUM), false);
            return View(data);
        }

        [HttpPost]
        [UserAuth("WARN_COMPLAIN_DETAIL_EDT")]
        public ActionResult Edit(WARN_COMPLAIN_DETAIL model)
        {
            try
            {
              
                var data = WarncomplaindetailDao.GetEntity("SEQ", model.SEQ);
                data.COMPLAIN_METHOD = model.COMPLAIN_METHOD;
                data.COMPLAIN_REASON = model.COMPLAIN_REASON;
                data.STATE =2;//默认状态为已处理
                    var rst = WarncomplaindetailDao.UpdateObject(data);
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


        public ActionResult DetailListByHall(string hallno, int pageIndex = 1, int pageSize = 20)
        {
            ViewBag.HALL_NO = hallno;
            var listdate = new WARN_COMPLAIN_DETAIL_DAL().GetListByHall(hallno, pageIndex, pageSize);
            return View(listdate);
        }
    }
}
