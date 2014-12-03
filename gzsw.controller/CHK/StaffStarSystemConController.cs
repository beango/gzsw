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
    /// 个人星级评定体系
    /// </summary>
    public class StaffStarSystemConController : BaseController<CHK_STAFF_STAR_SYSTEM_CON>
    {
        [UserAuth("CHK_STAFF_STAR_SYSTEM_CON_VIW")]
        public ActionResult Index(string orgId ,int pageIndex = 1, int pageSize=20)
        {
            setOrgData(orgId);
            var list = CHK_STAFF_STAR_SYSTEM_CON_DAL.GetList(orgId, UserState.UserID, pageIndex, pageSize);
            return View(list);
        }

        [UserAuth("CHK_STAFF_STAR_SYSTEM_CON_VIW")]
        public ActionResult Details(string id)
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

        [UserAuth("CHK_STAFF_STAR_SYSTEM_CON_ADD")]
        public ActionResult Create()
        {
            setOrgData(null);
            return View();
        }

        [HttpPost]
        [UserAuth("CHK_STAFF_STAR_SYSTEM_CON_ADD")]
        public ActionResult Create(StaffStarSystemConAddModel model)
        {
            setOrgData(model.ORG_ID);

            if (ModelState.IsValid)
            {
                try
                {
                    var item = new CHK_STAFF_STAR_SYSTEM_CON()
                    {
                        ORG_ID = model.ORG_ID,
                       ATTEND_MIN_SCORE=model.ATTEND_MIN_SCORE,
                       COMPLAIN_MAX_CNT=model.COMPLAIN_MAX_CNT,
                       DEFAULT_STAR=model.DEFAULT_STAR,
                       EFFIC_MIN_SCORE=model.EFFIC_MIN_SCORE,
                       EVAL_MIN_SCORE=model.EVAL_MIN_SCORE,
                       QUALITY_MIN_SCORE=model.QUALITY_MIN_SCORE,
                       STAR_1_MAX_SCORE=model.STAR_1_MAX_SCORE,
                       STAR_1_MIN_SCORE=model.STAR_1_MIN_SCORE,
                       STAR_2_MAX_SCORE=model.STAR_2_MAX_SCORE,
                       STAR_2_MIN_SCORE=model.STAR_2_MIN_SCORE,
                       STAR_3_MAX_SCORE=model.STAR_3_MAX_SCORE,
                       STAR_3_MIN_SCORE=model.STAR_3_MIN_SCORE,
                       STAR_4_MAX_SCORE=model.STAR_4_MAX_SCORE,
                       STAR_4_MIN_SCORE=model.STAR_4_MIN_SCORE,
                       STAR_5_MIN_SCORE=model.STAR_5_MIN_SCORE,
                       SVR_MIN_SCORE=model.SVR_MIN_SCORE,
                       TIME_DUR_TYP=model.TIME_DUR_TYP,
                       USU_ACT_MIN_SCORE=model.USU_ACT_MIN_SCORE
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


        [UserAuth("CHK_STAFF_STAR_SYSTEM_CON_ADD")]
        public ActionResult ValidateOrgId(string ORG_ID, byte? TIME_DUR_TYP)
        {
            var item = dao.GetEntity("ORG_ID", ORG_ID, "TIME_DUR_TYP", TIME_DUR_TYP.Value);
            return Json(item == null, JsonRequestBehavior.AllowGet);
        }

        [UserAuth("CHK_STAFF_STAR_SYSTEM_CON_EDT")]
        public ActionResult Edit(string id)
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
        [UserAuth("CHK_STAFF_STAR_SYSTEM_CON_EDT")]
        public ActionResult Edit(StaffStarSystemConModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var item = dao.GetEntity("ORG_ID", model.ORG_ID, "TIME_DUR_TYP", model.TIME_DUR_TYP);
                    item.ATTEND_MIN_SCORE = model.ATTEND_MIN_SCORE;
                    item.COMPLAIN_MAX_CNT = model.COMPLAIN_MAX_CNT;
                    item.DEFAULT_STAR = model.DEFAULT_STAR;
                    item.EFFIC_MIN_SCORE = model.EFFIC_MIN_SCORE;
                    item.EVAL_MIN_SCORE = model.EVAL_MIN_SCORE;
                    item.QUALITY_MIN_SCORE = model.QUALITY_MIN_SCORE;
                    item.STAR_1_MAX_SCORE = model.STAR_1_MAX_SCORE;
                    item.STAR_1_MIN_SCORE = model.STAR_1_MIN_SCORE;
                    item.STAR_2_MAX_SCORE = model.STAR_2_MAX_SCORE;
                    item.STAR_2_MIN_SCORE = model.STAR_2_MIN_SCORE;
                    item.STAR_3_MAX_SCORE = model.STAR_3_MAX_SCORE;
                    item.STAR_3_MIN_SCORE = model.STAR_3_MIN_SCORE;
                    item.STAR_4_MAX_SCORE = model.STAR_4_MAX_SCORE;
                    item.STAR_4_MIN_SCORE = model.STAR_4_MIN_SCORE;
                    item.STAR_5_MIN_SCORE = model.STAR_5_MIN_SCORE;
                    item.SVR_MIN_SCORE = model.SVR_MIN_SCORE;
                    item.USU_ACT_MIN_SCORE = model.USU_ACT_MIN_SCORE;

                    var rst = CHK_STAFF_STAR_SYSTEM_CON_DAL.Update(item);
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

        [UserAuth("CHK_STAFF_STAR_SYSTEM_CON_DEL")]
        public ActionResult Delete(string id)
        {
            try
            {
                var deleteId = id.Split(',');

                foreach (var tId in deleteId)
                {
                    var orgId = string.Empty;
                    var durType = Convert.ToByte(0);
                    if (!string.IsNullOrEmpty(id))
                    {
                        var ids = tId.Split('*');
                        if (ids.Length == 2)
                        {
                            orgId = ids[0];
                            durType = Convert.ToByte(ids[1]);
                        }
                    }

                    CHK_STAFF_STAR_SYSTEM_CON_DAL.Delete(orgId, durType);
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("删除出错", ex);
                return RedirectToAction("Error", "Home");
            }
        }


        private StaffStarSystemConModel getModel(string id)
        {
            var orgId = string.Empty;
            var durType = Convert.ToByte(0);
            if (!string.IsNullOrEmpty(id))
            {
                var ids = id.Split('*');
                if (ids.Length == 2)
                {
                    orgId = ids[0];
                    durType = Convert.ToByte(ids[1]);
                }
            }
            var item = CHK_STAFF_STAR_SYSTEM_CON_DAL.Get(orgId, durType);
            return new StaffStarSystemConModel()
                   {
                       ATTEND_MIN_SCORE=item.ATTEND_MIN_SCORE,
                       COMPLAIN_MAX_CNT=item.COMPLAIN_MAX_CNT,
                       DEFAULT_STAR=item.DEFAULT_STAR,
                       EFFIC_MIN_SCORE=item.EFFIC_MIN_SCORE,
                       EVAL_MIN_SCORE=item.EVAL_MIN_SCORE,
                       ORG_ID=item.ORG_ID,
                       ORG_NAM = item.ORG_NAM,
                       QUALITY_MIN_SCORE=item.QUALITY_MIN_SCORE,
                       STAR_1_MAX_SCORE=item.STAR_1_MAX_SCORE,
                       STAR_1_MIN_SCORE=item.STAR_1_MIN_SCORE,
                       STAR_2_MAX_SCORE = item.STAR_2_MAX_SCORE,
                       STAR_2_MIN_SCORE = item.STAR_2_MIN_SCORE,
                       STAR_3_MAX_SCORE = item.STAR_3_MAX_SCORE,
                       STAR_3_MIN_SCORE = item.STAR_3_MIN_SCORE,
                       STAR_4_MAX_SCORE = item.STAR_4_MAX_SCORE,
                       STAR_4_MIN_SCORE = item.STAR_4_MIN_SCORE,
                       STAR_5_MIN_SCORE = item.STAR_5_MIN_SCORE,
                       SVR_MIN_SCORE=item.SVR_MIN_SCORE,
                       TIME_DUR_TYP=item.TIME_DUR_TYP,
                       USU_ACT_MIN_SCORE=item.USU_ACT_MIN_SCORE
                   };
        }

        private void setOrgData(string orgId)
        {
            SetViewBagOrgData(orgId, "2");
        }
    }
}
