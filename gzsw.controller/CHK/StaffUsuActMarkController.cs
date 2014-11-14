using System;
using System.Collections.Generic;
using System.Globalization;
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
    /// 个人考核评定
    ///     日常行为表现（个人考核日常行为表现设置CHK_STAFF_USU_ACT_MARK）
    /// </summary>
    public class StaffUsuActMarkController : BaseController<CHK_STAFF_USU_ACT_MARK>
    {
        [UserAuth("CHK_STAFF_USU_ACT_MARK_VIW")]
        public ActionResult Index(string orgId,int? type, string staffNo, string staffName, int pageIndex = 1, int pageSize = 20)
        {
            ViewBag.StaffNo = staffNo;
            ViewBag.StaffName = staffName;

            if (string.IsNullOrEmpty(orgId))
            {
                orgId = SetViewBagOrgData(orgId);
            }
            else
            {
                SetViewBagOrgData(orgId);
            }

            var list = CHK_STAFF_USU_ACT_MARK_DAL.GetList(orgId,type,staffNo, staffName, UserState.UserID, pageIndex, pageSize);
            return View(list);
        }

        [UserAuth("CHK_STAFF_USU_ACT_MARK_VIW")]
        public ActionResult Details(int id)
        {
            try
            {
                var model = getModel(id);
                return View(model);
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("查看出错", ex);
                return RedirectToAction("Error", "Home");
            }
        }

        [UserAuth("CHK_STAFF_USU_ACT_MARK_ADD")]
        public ActionResult Create()
        {
            setOrgData();
            return View();
        }

        [HttpPost]
        [UserAuth("CHK_STAFF_USU_ACT_MARK_ADD")]
        public ActionResult Create(StaffUsuActMarkAddModel model)
        {
            setOrgData(model.ORG_ID);

            if (ModelState.IsValid)
            {
                try
                {
                    var item = new CHK_STAFF_USU_ACT_MARK()
                               {
                                   DEDUCT=model.DEDUCT,
                                   MARK_TIME=DateTime.Now,
                                   MODIFY_DTIME=DateTime.Now,
                                   MODIFY_ID=UserState.UserID,
                                   REASON=model.REASON,
                                   STAFF_ID=model.STAFF_ID,
                                   STAT_MO= int.Parse(model.STAT_MO.ToString("yyyyMM")),
                                   USU_ACT_TYP=model.USU_ACT_TYP
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

        [UserAuth("CHK_STAFF_USU_ACT_MARK_EDT")]
        public ActionResult Edit(int id)
        {
            try
            {
                var model = getModel(id);

                return View(model);
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("修改出错", ex);
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpPost]
        [UserAuth("CHK_STAFF_USU_ACT_MARK_EDT")]
        public ActionResult Edit(StaffUsuActMarkModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var item = dao.GetEntity("SEQ", model.SEQ);
                    item.DEDUCT = model.DEDUCT;
                    item.MODIFY_DTIME = DateTime.Now;
                    item.MODIFY_ID = UserState.UserID;
                    item.REASON = model.REASON;
                    item.STAFF_ID = model.STAFF_ID;
                    //item.STAT_MO = int.Parse(model.STAT_MO.Replace("-", ""));
                    //item.USU_ACT_TYP = model.USU_ACT_TYP;

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


        [UserAuth("CHK_STAFF_USU_ACT_MARK_DEL")]
        public ActionResult Delete(string id)
        {
            try
            {
                var deleteId = id.Split(',');

                foreach (var t in deleteId)
                {
                    dao.Delete("SEQ", t);
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("删除出错", ex);
                return RedirectToAction("Error", "Home");
            }
        }


        private StaffUsuActMarkModel getModel(int id)
        {
            var item = CHK_STAFF_USU_ACT_MARK_DAL.Get(id);
            var statmo = item.STAT_MO.ToString();
            statmo = statmo.Insert(4, "年")+"月";
           
            return new StaffUsuActMarkModel()
                   {
                       DEDUCT=item.DEDUCT,
                       ORG_ID=item.ORG_ID,
                       ORG_NAM=item.ORG_NAM,
                       REASON=item.REASON,
                       SEQ=item.SEQ,
                       STAFF_ID=item.STAFF_ID,
                       STAFF_NAM=item.STAFF_NAM,
                       USU_ACT_TYP=item.USU_ACT_TYP,
                       STAT_MO = statmo,
                       MARK_TIME=item.MARK_TIME.GetValueOrDefault()
                   };
        }

        private void setOrgData(string orgId=null)
        {
            SetViewBagOrgData(orgId);
        }
    }
}
