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
    public class HALLstatMarkController : BaseController<CHK_HALL_STAT_M>
    {


        [Ninject.Inject]
        public IDao<CHK_HALL_STAT_M> ChkhallstatmDao { get; set; }


        [Ninject.Inject]
        public IDao<SYS_HALL> Halldao { get; set; }

        [UserAuth("HALLstatMark_VIW")]
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

            var data = new CHK_HALL_STAT_M_DAL().GetPersonnelAttendanceList(pageIndex, pageSize, orgId, stat);
            return View(data);
        }




       
        public ActionResult Detail(string id, string staffId)
        {
            try
            {
                var model = CHK_HALL_STAT_M_DAL.GetPersonnelAttendance(Convert.ToInt32(id), staffId);
                ViewBag.HALL_NAM = Halldao.GetEntity("HALL_NO", staffId).HALL_NAM;
                    ViewBag.STAT_MO = id;
                return View(model);
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("查看出错", ex);
                return RedirectToAction("Error", "Home");
            }
        }

      
        public ActionResult Edit(string id, string staffId)
        {
            ViewBag.HALL_NAM = Halldao.GetEntity("HALL_NO", staffId).HALL_NAM;
            ViewBag.STAT_MO = id;
            try
            {
                var item = CHK_HALL_STAT_M_DAL.GetPersonnelAttendance(Convert.ToInt32(id), staffId);

                var model = new PersonnelAttendanceViewModel()
                {
                    COR_ABSENT_DAY_CNT=(int)item.COR_ABSENT_DAY_CNT,
                    COR_ATTEND_PER_CNT = (int)item.COR_ATTEND_PER_CNT,
                    COR_EAR_DAY_CNT = (int)item.COR_EAR_DAY_CNT,
                    COR_LAT_DAY_CNT = (int)item.COR_LAT_DAY_CNT,
                    COR_WORK_DAY_CNT = (int)item.COR_WORK_DAY_CNT,
                    HALL_NO = item.HALL_NO,
                    STAT_MO = item.STAT_MO

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
        
        public ActionResult Edit(PersonnelAttendanceViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var item = dao.GetEntity("STAT_MO", model.STAT_MO, "HALL_NO", model.HALL_NO);

                    item.MODIFY_DTIME = DateTime.Now;
                    item.MODIFY_ID = UserState.UserID;
                    item.COR_ABSENT_DAY_CNT = model.COR_ABSENT_DAY_CNT;
                    item.COR_ATTEND_PER_CNT = model.COR_ATTEND_PER_CNT;
                    item.COR_EAR_DAY_CNT = model.COR_EAR_DAY_CNT;
                    item.COR_LAT_DAY_CNT = model.COR_LAT_DAY_CNT;
                    item.COR_WORK_DAY_CNT = model.COR_WORK_DAY_CNT;


                   CHK_HALL_STAT_M_DAL.UpdateObject(item);
                  //
                    var mo = Convert.ToInt32(item.STAT_MO.ToString().Substring(4, 2));
                    if (mo != DateTime.Now.Month)
                    {
                        Stored_DAL.UpdateDataByHall(item.STAT_MO,item.HALL_NO,UserState.UserID);
                    }
                    Alter("修改成功！", util.Enum.AlterTypeEnum.Success, false, true);
                        return Redirect("/Home/Blank"); 
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
