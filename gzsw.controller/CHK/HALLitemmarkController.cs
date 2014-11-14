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
    public class HALLitemmarkController : BaseController<CHK_HALL_ITEM_MARK>
    { 


        [Ninject.Inject]
        public IDao<SYS_HALL> Halldao { get; set; }


        [Ninject.Inject]
        public IDao<SYS_USER> SysuserDao { get; set; }
        [Ninject.Inject]
        public IDao<CHK_HALL_CHKITEM_CON> Chkhallchkitemcon { get; set; }
        [UserAuth("CHK_HALL_STAT_M_VIW")]
        public ActionResult Index( string orgId, int pageIndex = 1, int pageSize = 20)
        {
            var organDao = new SYS_ORGANIZE_DAL();
            var orgs = organDao.GetListForUserId(UserState.UserID, "4");

            ViewBag.UserORG = new SelectList(orgs, "ORG_ID", "ORG_NAM", null);

            ViewBag.orgId = orgId;
            if (string.IsNullOrEmpty(orgId))
            {
                orgId = setOrgData();
            }
            else
            {
                setOrgData(orgId);
            } 

            var data = new CHK_HALL_ITEM_MARK_DAL().GetList(pageIndex, pageSize, orgId,null);
            return View(data);
        }




        [UserAuth("CHK_STAT_STAFF_SVRSTAT_M_VIW")]
        public ActionResult Detail(string id)
        {
            try
            {
                var model = dao.GetEntity("SEQ", id);
                ViewBag.HALL_NAM = Halldao.GetEntity("HALL_NO", model.HALL_NO).HALL_NAM;
                ViewBag.HALL_CHKITEM_NAM = Chkhallchkitemcon.GetEntity("HALL_CHKITEM_CD", model.HALL_CHKITEM_CD).HALL_CHKITEM_NAM;
                ViewBag.MODIFY_ID = SysuserDao.GetEntity("USER_ID", model.MODIFY_ID).USER_NAM;


                return View(model);
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("查看出错", ex);
                return RedirectToAction("Error", "Home");
            }
        }

        [UserAuth("CHK_STAT_STAFF_SVRSTAT_M_EDT")]
        public ActionResult Edit(string id, string orgId)
        {
            ViewBag.orgId = orgId;
            try
            { 

                var temp = dao.GetEntity("SEQ", id);
                ViewBag.HALL_NAM = Halldao.GetEntity("HALL_NO", temp.HALL_NO).HALL_NAM;
                ViewBag.HALL_CHKITEM_NAM = Chkhallchkitemcon.GetEntity("HALL_CHKITEM_CD", temp.HALL_CHKITEM_CD).HALL_CHKITEM_NAM;
                ViewBag.MODIFY_ID = SysuserDao.GetEntity("USER_ID", temp.MODIFY_ID).USER_NAM;


                ViewBag.CHKITEM_TYP = EnumHelper.GetCategorySelectList(typeof(gzsw.model.ext.CHK_HALL_ITEM_MARK.CHKITEM_TYP), false);

                var organDao = new SYS_ORGANIZE_DAL();
                var orgs = organDao.GetListForUserId(UserState.UserID, "4");

                ViewBag.UserORG = new SelectList(orgs, "ORG_ID", "ORG_NAM", false);

                ViewBag.HALL_CHKITEM_CD = new SelectList(Chkhallchkitemcon.FindList(), "HALL_CHKITEM_CD", "HALL_CHKITEM_NAM");

                var model = new CHK_HALL_ITEM_MARKViewModel();
                 
                model.CHKITEM_TYP = temp.CHKITEM_TYP;
                model.DEDUCT = temp.DEDUCT;
                model.HALL_CHKITEM_CD = temp.HALL_CHKITEM_CD;
                model.HALL_NO = temp.HALL_NO;
                model.MARK_TIME = temp.MARK_TIME;
                model.REASON = temp.REASON;
                model.MODIFY_ID = UserState.UserID;
                model.SEQ = temp.SEQ;
                return View(model);
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("查看出错", ex);
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpPost]
        [UserAuth("CHK_STAT_STAFF_SVRSTAT_M_EDT")]
        public ActionResult Edit(CHK_HALL_ITEM_MARKViewModel model)
        {

            var organDao = new SYS_ORGANIZE_DAL();
            var orgs = organDao.GetListForUserId(UserState.UserID, "4");

            ViewBag.UserORG = new SelectList(orgs, "ORG_ID", "ORG_NAM", false);
            try
            {
                if (ModelState.IsValid)
                {
                    var item = dao.GetEntity("SEQ", model.SEQ);

                    item.MODIFY_DTIME = DateTime.Now;
                    item.MODIFY_ID = UserState.UserID;
                    item.CHKITEM_TYP = model.CHKITEM_TYP;
                    item.DEDUCT = model.DEDUCT;
                    item.HALL_CHKITEM_CD = model.HALL_CHKITEM_CD;
                    item.HALL_NO = model.HALL_NO;
                    item.MARK_TIME = DateTime.Now;
                    item.REASON = model.REASON;


                    var rst = dao.UpdateObject(item);
                    if (rst > 0)
                    {
                        Alter("修改成功！", util.Enum.AlterTypeEnum.Success, false, true);
                        return Redirect("/Home/Blank");
                    }
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

        private string setOrgData(string orgId = null)
        {
            var organDao = new SYS_ORGANIZE_DAL();
            var orgs = organDao.GetListForUserId(UserState.UserID, "4");

            ViewBag.Orgs = new SelectList(orgs, "ORG_ID", "ORG_NAM", orgId);

            if (string.IsNullOrEmpty(orgId))
            {
                if (orgs != null && orgs.Count > 0)
                {
                    return orgs[0].ORG_ID;
                }
            }
            return orgId;
        }


        public ActionResult Create(string orgId)
        {
            ViewBag.orgId = orgId;

            ViewBag.CHKITEM_TYP = EnumHelper.GetCategorySelectList(typeof(gzsw.model.ext.CHK_HALL_ITEM_MARK.CHKITEM_TYP), false);

            var organDao = new SYS_ORGANIZE_DAL();
            var orgs = organDao.GetListForUserId(UserState.UserID, "4");

            ViewBag.UserORG = new SelectList(orgs, "ORG_ID", "ORG_NAM", null);

            ViewBag.HALL_CHKITEM_CD = new SelectList(Chkhallchkitemcon.FindList(), "HALL_CHKITEM_CD", "HALL_CHKITEM_NAM");
            var temp = new CHK_HALL_ITEM_MARK();
            temp.MODIFY_ID = UserState.UserID;
            temp.MARK_TIME = DateTime.Now;
            temp.MODIFY_DTIME = DateTime.Now;
            return View(temp);
        }
        [HttpPost]
        public ActionResult Create(CHK_HALL_ITEM_MARK model)
        {

            ViewBag.CHKITEM_TYP = EnumHelper.GetCategorySelectList(typeof(gzsw.model.ext.CHK_HALL_ITEM_MARK.CHKITEM_TYP),false);

            var organDao = new SYS_ORGANIZE_DAL();
            var orgs = organDao.GetListForUserId(UserState.UserID, "4");

            ViewBag.UserORG = new SelectList(orgs, "ORG_ID", "ORG_NAM", null);

            ViewBag.HALL_CHKITEM_CD = new SelectList(Chkhallchkitemcon.FindList(), "HALL_CHKITEM_CD", "HALL_CHKITEM_NAM");
            if (ModelState.IsValid)
            {
                try
                {
                    var item = new CHK_HALL_ITEM_MARK();
                    item.MODIFY_DTIME = DateTime.Now;
                    item.MODIFY_ID = UserState.UserID;
                    item.CHKITEM_TYP = model.CHKITEM_TYP;
                    item.DEDUCT = model.DEDUCT;
                    item.HALL_CHKITEM_CD = model.HALL_CHKITEM_CD;
                    item.HALL_NO = model.HALL_NO;
                    item.MARK_TIME = model.MARK_TIME;
                    item.REASON = model.REASON;
                    var rst = dao.AddObject(item);
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
        [UserAuth("CHK_STAT_STAFF_SVRSTAT_M_DEL")]
        public ActionResult Delete(string id)
        {
            try
            {
                var arrid = id.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var _id in arrid)
                {
                    dao.Delete("SEQ", int.Parse(_id.Split('-')[0]));
                }


                return RedirectToAction("/");
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("删除出错", ex);
                return Redirect("/Home/Error");
            }
        }
    }
}
