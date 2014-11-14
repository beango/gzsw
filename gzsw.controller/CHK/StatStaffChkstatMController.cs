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
using NPOI.SS.Formula.Functions;

namespace gzsw.controller.CHK
{
    /// <summary>
    /// 个人考核数据管理
    ///     考勤数据修正
    /// </summary>
    public class StatStaffChkstatMController : BaseController<STAT_STAFF_CHKSTAT_M>
    {
        [UserAuth("CHK_STAT_STAFF_CHKSTAT_M_VIW")]
        public ActionResult Index(string statMo,string staffNo,string staffName,string orgId,int pageIndex=1,int pageSize=20)
        {
            int stat;
            if (string.IsNullOrEmpty(statMo))
            {
                statMo = DateTime.Now.AddMonths(-1).ToString("yyyy-MM");
            }
            int.TryParse(statMo.Replace("-",""),out stat);

            ViewBag.Stat = stat;
            ViewBag.StatMo = statMo;
            ViewBag.StaffNo = staffNo;
            ViewBag.StaffName = staffName;

            if (string.IsNullOrEmpty(orgId))
            {
                orgId = setOrgData();
            }
            else
            {
                setOrgData(orgId);
            }

            var list = STAT_STAFF_CHKSTAT_M_DAL.GetListBub(stat, staffNo, staffName, orgId, pageIndex, pageSize);
            
            return View(list);
        }

        [UserAuth("CHK_STAT_STAFF_CHKSTAT_M_VIW")]
        public ActionResult Details(int id, string staffId)
        {
            try
            {
                var model = STAT_STAFF_CHKSTAT_M_DAL.GetSub(id, staffId);
                return View(model);
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("查看出错", ex);
                return RedirectToAction("Error", "Home");
            }
        }

        [UserAuth("CHK_STAT_STAFF_CHKSTAT_M_EDT")]
        public ActionResult Edit(int id, string staffId)
        {
            try
            {
                var item = STAT_STAFF_CHKSTAT_M_DAL.GetSub(id, staffId);
                var model = new StatStaffChkstatMModel()
                           {
                               COR_EAR_DAY_CNT=item.COR_EAR_DAY_CNT,
                               COR_HOLLI_TYP1_CNT=item.COR_HOLLI_TYP1_CNT,
                               COR_HOLLI_TYP2_CNT = item.COR_HOLLI_TYP2_CNT,
                               COR_HOLLI_TYP3_CNT = item.COR_HOLLI_TYP3_CNT,
                               COR_HOLLI_TYP4_CNT = item.COR_HOLLI_TYP4_CNT,
                               COR_HOLLI_TYP5_CNT = item.COR_HOLLI_TYP5_CNT,
                               COR_HOLLI_TYP6_CNT = item.COR_HOLLI_TYP6_CNT,
                               COR_HOLLI_TYP7_CNT = item.COR_HOLLI_TYP7_CNT,
                               COR_HOLLI_TYP8_CNT = item.COR_HOLLI_TYP8_CNT,
                               COR_HOLLI_TYP9_CNT = item.COR_HOLLI_TYP9_CNT,
                               COR_HOLLI_TYP10_CNT = item.COR_HOLLI_TYP10_CNT,
                               COR_HOLLI_TYP11_CNT = item.COR_HOLLI_TYP11_CNT,
                               COR_HOLLI_TYP12_CNT = item.COR_HOLLI_TYP12_CNT,
                               COR_HOLLI_TYP13_CNT = item.COR_HOLLI_TYP13_CNT,
                               COR_LAT_DAY_CNT = item.COR_LAT_DAY_CNT,
                               COR_NONSIGN_OUT_CNT = item.COR_NONSIGN_OUT_CNT,
                               COR_WORK_DAY_CNT = item.COR_WORK_DAY_CNT,
                               STAFF_ID=item.STAFF_ID,
                               STAFF_NAM=item.STAFF_NAM,
                               STAT_MO=item.STAT_MO,
                               COR_ABSENT_DAY_CNT = item.COR_ABSENT_DAY_CNT
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
        [UserAuth("CHK_STAT_STAFF_CHKSTAT_M_EDT")]
        public ActionResult Edit(StatStaffChkstatMModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var item = dao.GetEntity("STAT_MO", model.STAT_MO,"STAFF_ID",model.STAFF_ID);
                    item.MODIFY_DTIME = DateTime.Now;
                    item.MODIFY_ID = UserState.UserID;
                    item.COR_EAR_DAY_CNT = item.COR_EAR_DAY_CNT;
                    item.COR_HOLLI_TYP1_CNT = model.COR_HOLLI_TYP1_CNT;
                    item.COR_HOLLI_TYP2_CNT = model.COR_HOLLI_TYP2_CNT;
                    item.COR_HOLLI_TYP3_CNT = model.COR_HOLLI_TYP3_CNT;
                    item.COR_HOLLI_TYP4_CNT = model.COR_HOLLI_TYP4_CNT;
                    item.COR_HOLLI_TYP5_CNT = model.COR_HOLLI_TYP5_CNT;
                    item.COR_HOLLI_TYP6_CNT = model.COR_HOLLI_TYP6_CNT;
                    item.COR_HOLLI_TYP7_CNT = model.COR_HOLLI_TYP7_CNT;
                    item.COR_HOLLI_TYP8_CNT = model.COR_HOLLI_TYP8_CNT;
                    item.COR_HOLLI_TYP9_CNT = model.COR_HOLLI_TYP9_CNT;
                    item.COR_HOLLI_TYP10_CNT = model.COR_HOLLI_TYP10_CNT;
                    item.COR_HOLLI_TYP11_CNT = model.COR_HOLLI_TYP11_CNT;
                    item.COR_HOLLI_TYP12_CNT = model.COR_HOLLI_TYP12_CNT;
                    item.COR_HOLLI_TYP13_CNT = model.COR_HOLLI_TYP13_CNT;
                    item.COR_LAT_DAY_CNT = model.COR_LAT_DAY_CNT;
                    item.COR_NONSIGN_OUT_CNT = model.COR_NONSIGN_OUT_CNT;
                    item.COR_WORK_DAY_CNT = model.COR_WORK_DAY_CNT;
                    item.COR_ABSENT_DAY_CNT = model.COR_ABSENT_DAY_CNT;

                    var rst = STAT_STAFF_CHKSTAT_M_DAL.Update(item);
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

        private string setOrgData(string orgId=null)
        {
            return SetViewBagOrgData(orgId);
        }
    }
}
