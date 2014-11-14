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
    /// 个人考核指标管理
    /// 满意度
    ///     考核满意度配置
    /// </summary>
    public class SatisRtConController : BaseController<CHK_SATIS_RT_CON>
    {
        [UserAuth("CHK_SATIS_RT_CON_VIW")]
        public ActionResult Index(string orgId, int pageIndex=1, int pageSize=20)
        {
            orgId= setOrgData(orgId);
            var list = CHK_SATIS_RT_CON_DAL.GetList(orgId, UserState.UserID, pageIndex, pageSize);
            return View(list);
        }

        [UserAuth("CHK_SATIS_RT_CON_VIW")]
        public ActionResult Details(string id)
        {
            try
            {
                var item = CHK_SATIS_RT_CON_DAL.Get(id);
                var model = new SatisRtConModel()
                {
                    ORG_ID = item.ORG_ID,
                    ORG_NAM = item.ORG_NAM,
                    COMMON_SCORE = item.COMMON_SCORE,
                    NON_EVAL_SCORE = item.NON_EVAL_SCORE,
                    SATISFY_SCORE = item.SATISFY_SCORE,
                    UNSATISFY_SCORE = item.UNSATISFY_SCORE,
                    VERY_SATISFY_SCORE = item.VERY_SATISFY_SCORE
                };

                return View(model);
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("查看出错", ex);
                return RedirectToAction("Error", "Home");
            }
        }

        [UserAuth("CHK_SATIS_RT_CON_ADD")]
        public ActionResult Create()
        {
            setOrgData(null);
            return View();
        }

        [HttpPost]
        [UserAuth("CHK_SATIS_RT_CON_ADD")]
        public ActionResult Create(SatisRtConAddModel model)
        {
            setOrgData(model.ORG_ID);

            if (ModelState.IsValid)
            {
                try
                {
                    var item = new CHK_SATIS_RT_CON()
                    {
                        ORG_ID = model.ORG_ID,
                        COMMON_SCORE=model.COMMON_SCORE,
                        NON_EVAL_SCORE=model.NON_EVAL_SCORE,
                        SATISFY_SCORE=model.SATISFY_SCORE,
                        UNSATISFY_SCORE=model.UNSATISFY_SCORE,
                        VERY_SATISFY_SCORE=model.VERY_SATISFY_SCORE
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

        [UserAuth("CHK_SATIS_RT_CON_EDT")]
        public ActionResult Edit(string id)
        {
            try
            {
                var item = CHK_SATIS_RT_CON_DAL.Get(id);
                var model = new SatisRtConModel()
                {
                    ORG_ID = item.ORG_ID,
                    ORG_NAM = item.ORG_NAM,
                    COMMON_SCORE=item.COMMON_SCORE,
                    NON_EVAL_SCORE=item.NON_EVAL_SCORE,
                    SATISFY_SCORE=item.SATISFY_SCORE,
                    UNSATISFY_SCORE=item.UNSATISFY_SCORE,
                    VERY_SATISFY_SCORE=item.VERY_SATISFY_SCORE
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
        [UserAuth("CHK_SATIS_RT_CON_EDT")]
        public ActionResult Edit(SatisRtConModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var item = dao.GetEntity("ORG_ID", model.ORG_ID);
                    item.COMMON_SCORE = model.COMMON_SCORE;
                    item.NON_EVAL_SCORE = model.NON_EVAL_SCORE;
                    item.SATISFY_SCORE = model.SATISFY_SCORE;
                    item.UNSATISFY_SCORE = model.UNSATISFY_SCORE;
                    item.VERY_SATISFY_SCORE = model.VERY_SATISFY_SCORE;

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

        [UserAuth("CHK_SATIS_RT_CON_DEL")]
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

        [UserAuth("CHK_SATIS_RT_CON_ADD")]
        public ActionResult ValidateOrgId(string ORG_ID)
        {
            var item = dao.GetEntity("ORG_ID", ORG_ID);
            return Json(item == null, JsonRequestBehavior.AllowGet);
        }

        private string setOrgData(string orgId)
        {
            if (string.IsNullOrEmpty(orgId))
            {
                orgId = SetViewBagOrgData(orgId, "2");
            }
            else
            {
                SetViewBagOrgData(orgId, "2");
            }
            return orgId;
        }
    }
}
