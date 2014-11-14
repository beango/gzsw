using System.Web.Mvc;
using gzsw.controller.CHK.Models;
using gzsw.controller.MyAuth;
using gzsw.dal.dao;
using gzsw.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gzsw.util;

namespace gzsw.controller.CHK
{
    /// <summary>
    /// 纳税人评价表现修正
    /// </summary>
    public class StaffEvalstatMController : BaseController<STAT_STAFF_EVALSTAT_M>
    {
        [UserAuth("CHK_STAT_STAFF_EVALSTAT_M_VIW")]
        public ActionResult Index(string statMo, string orgId, int pageIndex = 1, int pageSize = 20)
        {
            int stat;
            if (string.IsNullOrEmpty(statMo))
            {
                statMo = DateTime.Now.AddMonths(-1).ToString("yyyy-MM");
            }
            int.TryParse(statMo.Replace("-", ""), out stat);

            if (string.IsNullOrEmpty(orgId))
            {
                orgId = setOrgData();
            }
            else
            {
                setOrgData(orgId);
            }

            ViewBag.Stat = stat;
            ViewBag.StatMo = statMo;

            var list = STAT_STAFF_EVALSTAT_M_DAL.GetListSub(stat, orgId, pageIndex, pageSize);

            return View(list);
        }

        [UserAuth("CHK_STAT_STAFF_EVALSTAT_M_VIW")]
        public ActionResult Details(int id, string staffId)
        {
            try
            {
                var model = STAT_STAFF_EVALSTAT_M_DAL.GetSub(id, staffId);
                return View(model);
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("查看出错", ex);
                return RedirectToAction("Error", "Home");
            }
        }

        [UserAuth("CHK_STAT_STAFF_EVALSTAT_M_EDT")]
        public ActionResult Edit(int id, string staffId, string serialId)
        {
            try
            {
                var item = STAT_STAFF_EVALSTAT_M_DAL.GetSub(id, staffId);

                var model = new StaffEvalstatMModel()
                {
                    STAFF_ID = item.STAFF_ID,
                    STAFF_NAM = item.STAFF_NAM,
                    STAT_MO = item.STAT_MO,
                    COR_COMMON_CNT = item.COR_COMMON_CNT,
                    COR_NON_EVAL_CNT = item.COR_NON_EVAL_CNT,
                    COR_SATISFY_CNT=item.COR_SATISFY_CNT,
                    COR_UNSATISFY_CNT=item.COR_UNSATISFY_CNT,
                    COR_VERY_SATISFY_CNT=item.COR_VERY_SATISFY_CNT
                };

                return View(model);
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("查看出错", ex);
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpPost]
        [UserAuth("CHK_STAT_STAFF_EVALSTAT_M_EDT")]
        public ActionResult Edit(StaffEvalstatMModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var item = dao.GetEntity("STAT_MO", model.STAT_MO, "STAFF_ID", model.STAFF_ID);

                    item.MODIFY_DTIME = DateTime.Now;
                    item.MODIFY_ID = UserState.UserID;
                    item.COR_COMMON_CNT = model.COR_COMMON_CNT;
                    item.COR_NON_EVAL_CNT = model.COR_NON_EVAL_CNT;
                    item.COR_SATISFY_CNT = model.COR_SATISFY_CNT;
                    item.COR_UNSATISFY_CNT = model.COR_UNSATISFY_CNT;
                    item.COR_VERY_SATISFY_CNT = model.COR_VERY_SATISFY_CNT;


                    var rst = STAT_STAFF_EVALSTAT_M_DAL.Update(item);
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
            return SetViewBagOrgData(orgId);
        }
    }
}
