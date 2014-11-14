using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using gzsw.controller.MyAuth;
using gzsw.dal.dao;
using gzsw.model.Subclasses;
using gzsw.util;

namespace gzsw.controller.Detail
{
    public class TaxpayerEvaluationController : BaseController
    {
        /// <summary>
        /// 获取纳税人评价明细 今天加历史
        /// </summary>
        /// <param name="orgId">服务厅</param>
        /// <param name="beginTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="counter">窗口编号</param>
        /// <param name="staffId">员工代码</param>
        /// <param name="nsrsbm">纳税人识别码</param>
        /// <param name="serialName">业务名称</param>
        /// <param name="transcodeid">交易流水号</param>
        /// <param name="isSenior">是否高级查询</param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [UserAuth("DETAIL_TAXPAYER_EVALUATION_VIW")]
        public ActionResult Index(string orgId, DateTime? beginTime, DateTime? endTime, int? counter,
            string staffId, string nsrsbm, string serialName, string transcodeid,
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
            ViewBag.Counter = counter;
            ViewBag.StaffId = staffId;
            ViewBag.Number = transcodeid;
            ViewBag.SerialName = serialName;
            ViewBag.Nsrsbm = nsrsbm;
            ViewBag.IsSenior = isSenior;

            var list = DETAIL_TAXPAYER_EVALUATION_DAL.GetPager(beginTime, endTime, transcodeid, orgId, staffId, nsrsbm, serialName, counter, pageIndex, pageSize);

            return View(list);
        }
    }
}
