using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using gzsw.controller.MyAuth;
using gzsw.dal;
using gzsw.dal.dao;
using gzsw.model;
using gzsw.util;
using Ninject;

namespace gzsw.controller.Detail
{
    /// <summary>
    /// 明细查询
    ///     人员考勤明细
    /// </summary>
    public class StaffApplyDetailController:BaseController
    {
        [Inject]
        IDao<CHK_STAFF_APPLYITEM> appLyitemDal { get; set; }

        /// <summary>
        /// 人员考勤明细
        /// </summary>
        /// <param name="orgId"></param>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <param name="type"></param>
        /// <param name="staffNo"></param>
        /// <param name="staffName"></param>
        /// <param name="isSenior">是否高级查询</param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [UserAuth("Detail_CHK_STAFF_APPLYDETAIL_VIW")]
        public ActionResult Index(string orgId, DateTime? beginTime, DateTime? endTime, int? type, 
            string staffNo, string staffName,int isSenior=0, int pageIndex=1,int pageSize=20)
        {
            if (string.IsNullOrEmpty(orgId))
            {
                orgId = SetViewBagOrgData();
            }
            else
            {
                SetViewBagOrgData(orgId);
            }
            beginTime = beginTime ?? DateTime.Now;
            endTime = endTime ?? DateTime.Now;

            ViewBag.BeginTime = beginTime.GetValueOrDefault().ToString("yyyy-MM-dd");
            ViewBag.EndTime = endTime.GetValueOrDefault().ToString("yyyy-MM-dd");
            ViewBag.StaffNo = staffNo;
            ViewBag.StaffName = staffName;
            ViewBag.Type = type;
            ViewBag.IsSenior = isSenior;

            var list = CHK_STAFF_APPLYDETAIL_DAL.GetMergeList(orgId, staffNo, staffName, beginTime, endTime, type,
                pageIndex, pageSize);
            return View(list);
        }

        [UserAuth("Detail_CHK_STAFF_APPLYDETAIL_VIW")]
        public ActionResult Details(int id)
        {
            try
            {
                var appDal = new CHK_STAFF_APPLYDETAIL_DAL();
                var model = appDal.Get(id);

                ViewBag.Items = appLyitemDal.FindList("APPLYITEM", "APPLYDETAIL_ID", id);

                return View(model);
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("查看出错", ex);
                return RedirectToAction("Error", "Home");
            }
        } 
    }
}
