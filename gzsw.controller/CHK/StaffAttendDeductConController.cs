using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using gzsw.controller.CHK.Models;
using gzsw.controller.MyAuth;
using gzsw.dal.dao;
using gzsw.model;
using gzsw.util;

namespace gzsw.controller.CHK
{
    /// <summary>
    /// 个人考核考勤扣分设置
    /// </summary>
    public class StaffAttendDeductConController : BaseController<CHK_STAFF_ATTEND_DEDUCT_CON>
    {
        [UserAuth("CHK_STAFF_ATTEND_DEDUCT_CON_VIW")]
        public ActionResult Index(string orgId, int pageIndex = 1, int pageSize = 20)
        {
            SetViewBagOrgData(orgId, "2");
            var list = CHK_STAFF_ATTEND_DEDUCT_CON_DAL.GetList(orgId, UserState.UserID, pageIndex, pageSize);
            return View(list);
        }

        [UserAuth("CHK_STAFF_ATTEND_DEDUCT_CON_VIW")]
        public ActionResult Details(string id)
        {
            try
            {
                var item = CHK_STAFF_ATTEND_DEDUCT_CON_DAL.Get(id);

                return View(item);
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("查看出错", ex);
                return RedirectToAction("Error", "Home");
            }
        }

        [UserAuth("CHK_STAFF_ATTEND_DEDUCT_CON_ADD")]
        public ActionResult Create()
        {
            SetViewBagOrgData(null, "2");
            return View();
        }

        [HttpPost]
        [UserAuth("CHK_STAFF_ATTEND_DEDUCT_CON_ADD")]
        public ActionResult Create(StaffAttendDeductConAddModel model)
        {
            SetViewBagOrgData(model.ORG_ID, "2");

            if (ModelState.IsValid)
            {
                try
                {
                    var item = new CHK_STAFF_ATTEND_DEDUCT_CON()
                    {
                        ORG_ID = model.ORG_ID,
                        EAR_SCORE=model.EAR_SCORE,
                        LAT_SCORE=model.LAT_SCORE,
                        MODIFY_DTIME=DateTime.Now,
                        MODIFY_ID=UserState.UserID,
                        NEG_SCORE=model.NEG_SCORE
                    };

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

        [UserAuth("CHK_STAFF_ATTEND_DEDUCT_CON_EDT")]
        public ActionResult Edit(string id)
        {
            try
            {
                var item = CHK_STAFF_ATTEND_DEDUCT_CON_DAL.Get(id);
                var model = new StaffAttendDeductConModel()
                {
                    ORG_ID = item.ORG_ID,
                    ORG_NAM = item.ORG_NAM,
                    EAR_SCORE = item.EAR_SCORE,
                    LAT_SCORE = item.LAT_SCORE,
                    NEG_SCORE = item.NEG_SCORE
                };

                return View(model);
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("修改出错", ex);
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpPost]
        [UserAuth("CHK_STAFF_ATTEND_DEDUCT_CON_EDT")]
        public ActionResult Edit(StaffAttendDeductConModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var item = dao.GetEntity("ORG_ID", model.ORG_ID);
                    item.EAR_SCORE = model.EAR_SCORE;
                    item.LAT_SCORE = model.LAT_SCORE;
                    item.NEG_SCORE = model.NEG_SCORE;
                    item.MODIFY_DTIME = DateTime.Now;
                    item.MODIFY_ID = UserState.UserID;

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

        [UserAuth("CHK_STAFF_ATTEND_DEDUCT_CON_DEL")]
        public ActionResult Delete(string id)
        {
            try
            {
                var deleteId = id.Split(',');

                foreach (var t in deleteId)
                {
                    dao.Delete("ORG_ID", t);
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("删除出错", ex);
                return RedirectToAction("Error", "Home");
            }
        }

        [UserAuth("CHK_STAFF_ATTEND_DEDUCT_CON_ADD")]
        public ActionResult ValidateOrgId(string ORG_ID)
        {
            var item = dao.GetEntity("ORG_ID", ORG_ID);
            return Json(item == null, JsonRequestBehavior.AllowGet);
        }
    }
}
