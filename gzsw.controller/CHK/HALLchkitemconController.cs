using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using gzsw.controller.CHK.Models;
using gzsw.controller.MyAuth;
using gzsw.dal;
using gzsw.dal.dao;
using gzsw.model;
using gzsw.util;

namespace gzsw.controller.CHK
{
    public class HALLchkitemconController : BaseController<CHK_HALL_CHKITEM_CON>
    {


        [Ninject.Inject]
        public IDao<CHK_HALL_CHKITEM_CON>  HallchkitemconDao { get; set; }


        [UserAuth("CHK_HALL_CHKITEM_CON_VIW")]
        public ActionResult Index(string Name, int pageIndex = 1, int pageSize = 20)
        { 
            ViewBag.Name = Name; 
            ViewBag.CHKITEM_TYP = EnumHelper.GetCategorySelectList(typeof(gzsw.model.ext.CHK_HALL_CHKITEM_CON.CHKITEM_TYP));
            var list = new CHK_HALL_CHKITEM_CON_DAL().GetList(Name, pageIndex, pageSize);
            return View(list);
        }

        [UserAuth("CHK_HALL_CHKITEM_CON_ADD")]
        public ActionResult Create()
        {
            var temp = new CHK_HALL_CHKITEM_CON_VIEW();
            temp.MODIFY_DTIME = DateTime.Now;
            temp.MODIFY_ID = UserState.UserID;
            ViewBag.CHKITEM_TYP = EnumHelper.GetCategorySelectList(typeof(gzsw.model.ext.CHK_HALL_CHKITEM_CON.CHKITEM_TYP), false);
            return View(temp);
        }

        

        [HttpPost]
        [UserAuth("CHK_HALL_CHKITEM_CON_ADD")]
        public ActionResult Create(CHK_HALL_CHKITEM_CON_VIEW info)
        {
            try
            { 
                
                if (!ModelState.IsValid)
                {
                    ModelState.AddModelError("", "新增出错。");
                    return View(info);
                }
                var temp = new CHK_HALL_CHKITEM_CON();
                temp.CHKITEM_TYP = info.CHKITEM_TYP;
                temp.HALL_CHKITEM_CD = info.HALL_CHKITEM_CD;
                temp.HALL_CHKITEM_NAM = info.HALL_CHKITEM_NAM;
                temp.MODIFY_DTIME = DateTime.Now;
                temp.MODIFY_ID = UserState.UserID;
                HallchkitemconDao.AddObject(temp); 

                Alter("新增成功！", util.Enum.AlterTypeEnum.Success, false, true);
                return Redirect("/Home/Blank");
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("系统错误", ex);
                ModelState.AddModelError("", "系统错误！");
                return Redirect("/Home/Error");
            }
        }

        [UserAuth("CHK_HALL_CHKITEM_CON_EDT")]
        public ActionResult Edit(string id)
        {
            var temp = new CHK_HALL_CHKITEM_CON();
            var info = HallchkitemconDao.GetEntity("HALL_CHKITEM_CD", id);
            temp.CHKITEM_TYP = info.CHKITEM_TYP;
            temp.HALL_CHKITEM_CD = info.HALL_CHKITEM_CD;
            temp.HALL_CHKITEM_NAM = info.HALL_CHKITEM_NAM;
            temp.MODIFY_DTIME = DateTime.Now;
            temp.MODIFY_ID = UserState.UserID; 
            ViewBag.CHKITEM_TYP = EnumHelper.GetCategorySelectList(typeof(gzsw.model.ext.CHK_HALL_CHKITEM_CON.CHKITEM_TYP), false);
            return View(temp);
        }
        [HttpPost]
        [UserAuth("CHK_HALL_CHKITEM_CON_EDT")]
        public ActionResult Edit(CHK_HALL_CHKITEM_CON info)
        {
            ViewBag.CHKITEM_TYP = EnumHelper.GetCategorySelectList(typeof(gzsw.model.ext.CHK_HALL_CHKITEM_CON.CHKITEM_TYP), false);
            try
            {

                if (!ModelState.IsValid)
                {
                    ModelState.AddModelError("", "修改出错。");
                    return View(info);
                }
                var temp = HallchkitemconDao.GetEntity("HALL_CHKITEM_CD", info.HALL_CHKITEM_CD);
                temp.CHKITEM_TYP = info.CHKITEM_TYP; 
                temp.HALL_CHKITEM_NAM = info.HALL_CHKITEM_NAM;
                temp.MODIFY_DTIME = DateTime.Now;
                temp.MODIFY_ID = UserState.UserID;
                HallchkitemconDao.UpdateObject(temp);

                Alter("修改成功！", util.Enum.AlterTypeEnum.Success, false, true);
                return Redirect("/Home/Blank");
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("系统错误", ex);
                ModelState.AddModelError("", "系统错误！");
                return Redirect("/Home/Error");
            }
        }

        [UserAuth("CHK_HALL_CHKITEM_CON_DEL")]
        public ActionResult Delete(string id)
        {

            try
            {
                var arrid = id.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var _id in arrid)
                {
                    HallchkitemconDao.Delete("HALL_CHKITEM_CD", _id.Split('-')[0]);
                }


                return RedirectToAction("/");
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("删除出错", ex);
                return Redirect("/Home/Error");
            }
        }

        public ActionResult CheckCode(string hall_chkitem_cd)
        {
            var date = HallchkitemconDao.GetEntity("HALL_CHKITEM_CD", hall_chkitem_cd);
            return Json(date == null, JsonRequestBehavior.AllowGet);
        }
    }
}
