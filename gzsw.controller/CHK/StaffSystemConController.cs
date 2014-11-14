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
    /// 考核体系设置
    /// </summary>
    public class StaffSystemConController : BaseController<CHK_STAFF_SYSTEM_CON>
    {

        [UserAuth("CHK_STAFF_SYSTEM_CON_VIW")]
        public ActionResult Index(string orgid, int pageIndex = 1, int pageSize = 20)
        {
            orgid = setOrgData(orgid);
            var list = CHK_STAFF_SYSTEM_CON_DAL.GetList(orgid, UserState.UserID, pageIndex, pageSize);
            return View(list);
        }

        [UserAuth("CHK_STAFF_SYSTEM_CON_VIW")]
        public ActionResult Details(string id)
        {
            try
            {
                var item = CHK_STAFF_SYSTEM_CON_DAL.Get(id);
                var model = new StaffSystemConModel()
                {
                    ORG_ID = item.ORG_ID,
                    ORG_NAM = item.ORG_NAM,
                    ATTEND_CHK_RT = item.ATTEND_CHK_RT.ToString("P"),
                    COMPRE_SAN_100_MAX_SCORE = item.COMPRE_SAN_100_MAX_SCORE,
                    COMPRE_SAN_TYP = item.COMPRE_SAN_TYP,
                    EFFIC_AVOID_EXC_DEDUCT = item.EFFIC_AVOID_EXC_DEDUCT,
                    EFFIC_AVOID_RT = item.EFFIC_AVOID_RT.ToString("P"),
                    EFFIC_CHK_RT = item.EFFIC_CHK_RT.ToString("P"),
                    EVAL_CHK_RT = item.EVAL_CHK_RT.ToString("P"),
                    QUALITY_CHK_RT = item.QUALITY_CHK_RT.ToString("P"),
                    SVR_CHK_RT = item.SVR_CHK_RT.ToString("P"),
                    USU_ACT_CHK_RT = item.USU_ACT_CHK_RT.ToString("P")
                };

                return View(model);
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("查看出错", ex);
                return RedirectToAction("Error", "Home");
            }
        }

        [UserAuth("CHK_STAFF_SYSTEM_CON_ADD")]
        public ActionResult Create()
        {
            setOrgData(null);
            return View(new StaffSystemConAddModel());
        }

        [HttpPost]
        [UserAuth("CHK_STAFF_SYSTEM_CON_ADD")]
        public ActionResult Create(StaffSystemConAddModel model)
        {
            setOrgData(model.ORG_ID);

            if (ModelState.IsValid)
            {
                try
                {
                    var item = new CHK_STAFF_SYSTEM_CON()
                    {
                        ORG_ID = model.ORG_ID,
                        ATTEND_CHK_RT = Convert.ToDecimal((model.ATTEND_CHK_RT.GetValueOrDefault() / 100).ToString("###.##")),
                        COMPRE_SAN_100_MAX_SCORE=model.COMPRE_SAN_100_MAX_SCORE,
                        COMPRE_SAN_TYP=model.COMPRE_SAN_TYP,
                        EFFIC_AVOID_EXC_DEDUCT=model.EFFIC_AVOID_EXC_DEDUCT.GetValueOrDefault(),
                        EFFIC_AVOID_RT= Convert.ToDecimal((model.EFFIC_AVOID_RT.GetValueOrDefault()/100).ToString("###.##")),
                        EFFIC_CHK_RT = Convert.ToDecimal((model.EFFIC_CHK_RT.GetValueOrDefault() / 100).ToString("###.##")),
                        EVAL_CHK_RT = Convert.ToDecimal((model.EVAL_CHK_RT.GetValueOrDefault() / 100).ToString("###.##")),
                        QUALITY_CHK_RT = Convert.ToDecimal((model.QUALITY_CHK_RT.GetValueOrDefault() / 100).ToString("###.##")),
                        SVR_CHK_RT = Convert.ToDecimal((model.SVR_CHK_RT.GetValueOrDefault() / 100).ToString("###.##")),
                        USU_ACT_CHK_RT = Convert.ToDecimal((model.USU_ACT_CHK_RT.GetValueOrDefault() / 100).ToString("###.##"))
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

        [UserAuth("CHK_STAFF_SYSTEM_CON_ADD")]
        public ActionResult ValidateOrgId(string ORG_ID)
        {
            var item = dao.GetEntity("ORG_ID", ORG_ID);
            return Json(item == null, JsonRequestBehavior.AllowGet);
        }

        [UserAuth("CHK_STAFF_SYSTEM_CON_EDT")]
        public ActionResult Edit(string id)
        {
            try
            {
                var item = CHK_STAFF_SYSTEM_CON_DAL.Get(id);
                var model = new StaffSystemConModel()
                {
                    ORG_ID = item.ORG_ID,
                    ORG_NAM = item.ORG_NAM,
                    ATTEND_CHK_RT=(item.ATTEND_CHK_RT*100).ToString("####.#"),
                    COMPRE_SAN_100_MAX_SCORE=item.COMPRE_SAN_100_MAX_SCORE,
                    COMPRE_SAN_TYP=item.COMPRE_SAN_TYP,
                    EFFIC_AVOID_EXC_DEDUCT=item.EFFIC_AVOID_EXC_DEDUCT,
                    EFFIC_AVOID_RT=(item.EFFIC_AVOID_RT*100).ToString("####.#"),
                    EFFIC_CHK_RT = (item.EFFIC_CHK_RT * 100).ToString("####.#"),
                    EVAL_CHK_RT = (item.EVAL_CHK_RT * 100).ToString("####.#"),
                    QUALITY_CHK_RT = (item.QUALITY_CHK_RT * 100).ToString("####.#"),
                    SVR_CHK_RT = (item.SVR_CHK_RT * 100).ToString("####.##"),
                    USU_ACT_CHK_RT = (item.USU_ACT_CHK_RT * 100).ToString("####.#")
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
        [UserAuth("CHK_STAFF_SYSTEM_CON_EDT")]
        public ActionResult Edit(StaffSystemConModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var item = dao.GetEntity("ORG_ID", model.ORG_ID);
                    item.ATTEND_CHK_RT = Convert.ToDecimal(model.ATTEND_CHK_RT)/100;
                    item.COMPRE_SAN_100_MAX_SCORE = model.COMPRE_SAN_100_MAX_SCORE;
                    item.COMPRE_SAN_TYP = model.COMPRE_SAN_TYP;
                    item.EFFIC_AVOID_EXC_DEDUCT = model.EFFIC_AVOID_EXC_DEDUCT;
                    item.EFFIC_AVOID_RT = Convert.ToDecimal(model.EFFIC_AVOID_RT)/100;
                    item.EFFIC_CHK_RT = Convert.ToDecimal(model.EFFIC_CHK_RT)/100;
                    item.EVAL_CHK_RT = Convert.ToDecimal(model.EVAL_CHK_RT)/100;
                    item.QUALITY_CHK_RT = Convert.ToDecimal(model.QUALITY_CHK_RT)/100;
                    item.SVR_CHK_RT = Convert.ToDecimal(model.SVR_CHK_RT)/100;
                    item.USU_ACT_CHK_RT = Convert.ToDecimal(model.USU_ACT_CHK_RT) / 100;

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

        [UserAuth("CHK_STAFF_SYSTEM_CON_DEL")]
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
