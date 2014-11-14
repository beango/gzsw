using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using gzsw.controller.MyAuth;
using gzsw.dal.dao;
using gzsw.util;

namespace gzsw.controller.Detail
{
    /// <summary>
    /// 排队叫号明细
    /// </summary>
    public class CurrqueuehistController : BaseController
    {
        /// <summary>
        /// 获取排队明细 今天加历史
        /// </summary>
        /// <param name="orgId">服务厅</param>
        /// <param name="beginTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="number">排队号码</param>
        /// <param name="counter">窗口编号</param>
        /// <param name="staffId">员工代码</param>
        /// <param name="nsrsbm">纳税人识别码</param>
        /// <param name="tickettype">取号方式</param>
        /// <param name="status">状态</param>
        /// <param name="isfinished">完成状态</param>
        /// <param name="isSenior">是否高级查询</param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [UserAuth("Detail_SYS_CURRQUEUEHIST_VIW")]
        public ActionResult Index(string orgId, DateTime? beginTime, DateTime? endTime, string number, int? counter,
            string staffId, string nsrsbm, int? tickettype, int? status, int? isfinished,
            int isSenior=0,int pageIndex = 1, int pageSize = 20)
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
            ViewBag.Number = number;
            ViewBag.Counter = counter;
            ViewBag.StaffId = staffId;
            ViewBag.Status = status;
            ViewBag.Nsrsbm = nsrsbm;
            ViewBag.IsSenior = isSenior;

            var list = SYS_CURRQUEUEHIST_DAL.GetMergeList(orgId, beginTime, endTime, number, counter, staffId, nsrsbm,
                tickettype, status, isfinished, pageIndex, pageSize);
            return View(list);
        }


        [UserAuth("Detail_SYS_CURRQUEUEHIST_VIW,DETAIL_TAXPAYER_EVALUATION_VIW")]
        public ActionResult Details(string id)
        {
            try
            {
                var model = SYS_CURRQUEUEHIST_DAL.GetMergeSub(id);
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
