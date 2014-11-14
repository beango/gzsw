using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using gzsw.controller.MyAuth;
using gzsw.dal.dao;
using gzsw.model;
using gzsw.util;

namespace gzsw.controller.Detail
{
    public class HistroyAccountDetailController : BaseController<SYS_YWHIST>
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
        /// <param name="hywDetailserialid">明细业务ID编码</param>
        /// <param name="isfinished">完成状态</param>
        /// <param name="transcodeid">交易流水号</param>
        /// <param name="isSenior">是否高级查询</param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [UserAuth("MON_Warning")]
        public ActionResult Index(string orgId, DateTime? beginTime, DateTime? endTime, string number, int? counter,
            string staffId, string nsrsbm, int? tickettype, string hywDetailserialid, int? isfinished, string transcodeid,
            int isSenior = 0, int pageIndex = 1, int pageSize = 20)
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
            ViewBag.HYW_DETAILSERIALID = hywDetailserialid;
            ViewBag.Nsrsbm = nsrsbm;
            ViewBag.IsSenior = isSenior;

            var list = SYS_YWHIST_DAL.GetMergeList(orgId, beginTime, endTime, number, counter, staffId, nsrsbm,
                tickettype, hywDetailserialid, isfinished, pageIndex, pageSize);
            return View(list);
        }


        private string setOrgData(string orgId = null)
        {
            var organDao = new SYS_ORGANIZE_DAL();
            var orgs = organDao.GetListForUserId(UserState.UserID, "4");

            ViewBag.Orgs = new SelectList(orgs, "ORG_ID", "ORG_NAM", orgId);

            if (string.IsNullOrEmpty(orgId))
            {
                if (orgs != null && orgs.Count > 0)
                {
                    return orgs[0].ORG_ID;
                }
            }
            return orgId;
        }
        [UserAuth("MON_Warning")]
        public ActionResult Detail(string id)
        {
            try
            {
                var model = SYS_YWHIST_DAL.GetOne(id);
                return View(model);
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("查看出错", ex);
                return RedirectToAction("Error", "Home");
            }
        } 
        public ActionResult DetailList(string code,int pageIndex=1,int pageSize=20)
        {
            var list = SYS_YWHIST_DAL.GetMergeListByCode(code, pageIndex, pageSize);
            return View(list);
        }
    }
}
