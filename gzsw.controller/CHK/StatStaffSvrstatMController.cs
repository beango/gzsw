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
    /// 个人考核数据管理
    ///     业务量数据修正
    /// </summary>
    public class StatStaffSvrstatMController : BaseController<STAT_STAFF_SVRSTAT_M>
    {
        [UserAuth("CHK_STAT_STAFF_SVRSTAT_M_VIW")]
        public ActionResult Index(string statMo, string orgId,string staffName, int pageIndex = 1, int pageSize = 20)
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
            ViewBag.StaffName = staffName;

            var list = STAT_STAFF_SVRSTAT_M_DAL.GetListBub(staffName,stat, orgId, pageIndex, pageSize);

            return View(list);
        }

        [UserAuth("CHK_STAT_STAFF_SVRSTAT_M_VIW")]
        public ActionResult Details(int id, string staffId, string serialId)
        {
            try
            {
                var model = STAT_STAFF_SVRSTAT_M_DAL.GetSub(id, staffId, serialId);
                return View(model);
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("查看出错", ex);
                return RedirectToAction("Error", "Home");
            }
        }

        [UserAuth("CHK_STAT_STAFF_SVRSTAT_M_EDT")]
        public ActionResult Edit(int id, string staffId, string serialId)
        {
            try
            {
                var item = STAT_STAFF_SVRSTAT_M_DAL.GetSub(id, staffId, serialId);

                var model = new StaffSvrstatMModel()
                            {
                                COR_DOOR_SVR_CNT = item.COR_DOOR_SVR_CNT.GetValueOrDefault(),
                                COR_OVERTIME_SVR_CNT = item.COR_OVERTIME_SVR_CNT.GetValueOrDefault(),
                                OTHER_SVR_CNT=item.OTHER_SVR_CNT.GetValueOrDefault(),
                                SERIALID=item.SERIALID,
                                SERIALNAME=item.SERIALNAME,
                                STAFF_ID=item.STAFF_ID,
                                STAFF_NAM=item.STAFF_NAM,
                                STAT_MO=item.STAT_MO
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
        [UserAuth("CHK_STAT_STAFF_SVRSTAT_M_EDT")]
        public ActionResult Edit(StaffSvrstatMModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var item = dao.GetEntity("STAT_MO", model.STAT_MO, "STAFF_ID", model.STAFF_ID, "SERIALID", model.SERIALID);

                    item.MODIFY_DTIME = DateTime.Now;
                    item.MODIFY_ID = UserState.UserID;
                    item.COR_DOOR_SVR_CNT = model.COR_DOOR_SVR_CNT;
                    item.OTHER_SVR_CNT = model.OTHER_SVR_CNT;
                    item.COR_OVERTIME_SVR_CNT = model.COR_OVERTIME_SVR_CNT;


                    var rst = STAT_STAFF_SVRSTAT_M_DAL.Update(item);
                    if (rst > 0)
                    {
                        Stored_DAL.UpdateData(item.STAT_MO, item.STAFF_ID, UserState.UserID, 1);
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
