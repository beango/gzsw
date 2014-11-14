using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Configuration;
using System.Text;
using System.Web.Mvc;
using gzsw.controller.CHK.Models;
using gzsw.controller.MyAuth;
using gzsw.dal;
using gzsw.dal.dao;
using gzsw.model;
using gzsw.util;
using Ninject;

namespace gzsw.controller.CHK
{
    /// <summary>
    /// 个人考核指标管理
    /// 日常行为表现
    /// </summary>
    public class StaffUsuActConController : BaseController<CHK_STAFF_USU_ACT_CON>
    {
            
        [UserAuth("CHK_STAFF_USU_ACT_CON_VIW")]
        public ActionResult Index(string orgId,int pageIndex=1,int pageSize=20)
        {
            setOrgData(orgId);
            var list= CHK_STAFF_USU_ACT_CON_DAL.GetList(orgId,UserState.UserID, pageIndex, pageSize);
            return View(list);
        }

        [UserAuth("CHK_STAFF_USU_ACT_CON_VIW")]
        public ActionResult Details(string id)
        {
            try
            {
                var item = CHK_STAFF_USU_ACT_CON_DAL.Get(id);
                var model = new StaffUsuActConEditModel()
                {
                    ORG_ID = item.ORG_ID,
                    ORG_NAM = item.ORG_NAM,
                    OTHER_RT = item.OTHER_RT,
                    SEC_HEAL_RT = item.SEC_HEAL_RT,
                    TAX_LOOK_RT = item.TAX_LOOK_RT,
                    TAX_SVR_RT = item.TAX_SVR_RT,
                    TRAIN_RT = item.TRAIN_RT,
                    WORK_DIS_RT = item.WORK_DIS_RT
                };

                return View(model);
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("查看出错", ex);
                return RedirectToAction("Error", "Home");
            }
        }

        [UserAuth("CHK_STAFF_USU_ACT_CON_ADD")]
        public ActionResult Create()
        {
            setOrgData(null);
            return View();
        }


        [HttpPost]
        [UserAuth("CHK_STAFF_USU_ACT_CON_ADD")]
        public ActionResult Create(StaffUsuActConModel model)
        {
            setOrgData(model.ORG_ID);

            if (ModelState.IsValid)
            {
                try
                {
                    var item = new CHK_STAFF_USU_ACT_CON()
                               {
                                   ORG_ID=model.ORG_ID,
                                   OTHER_RT=model.OTHER_RT/100,
                                   SEC_HEAL_RT = model.SEC_HEAL_RT / 100,
                                   TAX_LOOK_RT = model.TAX_LOOK_RT / 100,
                                   TAX_SVR_RT = model.TAX_SVR_RT / 100,
                                   TRAIN_RT = model.TRAIN_RT / 100,
                                   WORK_DIS_RT = model.WORK_DIS_RT / 100
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

        [UserAuth("CHK_STAFF_USU_ACT_CON_EDT")]
        public ActionResult Edit(string id)
        {
            try
            {
                var item = CHK_STAFF_USU_ACT_CON_DAL.Get(id);
                var model = new StaffUsuActConEditModel()
                {
                    ORG_ID=item.ORG_ID,
                    ORG_NAM=item.ORG_NAM,
                    OTHER_RT = item.OTHER_RT * 100,
                    SEC_HEAL_RT = item.SEC_HEAL_RT * 100,
                    TAX_LOOK_RT = item.TAX_LOOK_RT * 100,
                    TAX_SVR_RT = item.TAX_SVR_RT * 100,
                    TRAIN_RT = item.TRAIN_RT * 100,
                    WORK_DIS_RT = item.WORK_DIS_RT * 100
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
        [UserAuth("CHK_STAFF_USU_ACT_CON_EDT")]
        public ActionResult Edit(StaffUsuActConEditModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var item = dao.GetEntity("ORG_ID", model.ORG_ID);
                    item.OTHER_RT = model.OTHER_RT/100;
                    item.SEC_HEAL_RT = model.SEC_HEAL_RT/100;
                    item.TAX_LOOK_RT = model.TAX_LOOK_RT/100;
                    item.TAX_SVR_RT = model.TAX_SVR_RT/100;
                    item.TRAIN_RT = model.TRAIN_RT/100;
                    item.WORK_DIS_RT = model.WORK_DIS_RT/100;

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

        [UserAuth("CHK_STAFF_USU_ACT_CON_DEL")]
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

        /// <summary>
        /// 验证组织是否已配置
        /// </summary>
        /// <param name="ORG_ID"></param>
        /// <returns></returns>
        public ActionResult ValidateOrgId(string ORG_ID)
        {
            var item = dao.GetEntity("ORG_ID", ORG_ID);
            return Json(item == null, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 验证等于1
        /// </summary>
        /// <param name="OTHER_RT"></param>
        /// <param name="WORK_DIS_RT"></param>
        /// <param name="TAX_SVR_RT"></param>
        /// <param name="TAX_LOOK_RT"></param>
        /// <param name="TRAIN_RT"></param>
        /// <param name="SEC_HEAL_RT"></param>
        /// <returns></returns>
        public ActionResult ValidateDecimal(decimal OTHER_RT,decimal WORK_DIS_RT,decimal TAX_SVR_RT, decimal TAX_LOOK_RT,decimal TRAIN_RT, decimal SEC_HEAL_RT)
        {
            var num = OTHER_RT + WORK_DIS_RT + TAX_SVR_RT + TAX_LOOK_RT + TRAIN_RT + SEC_HEAL_RT;
            return Json(num == 1, JsonRequestBehavior.AllowGet);
        }

        private void setOrgData(string orgId)
        {
            SetViewBagOrgData(orgId, "2");
        }
    }
}
