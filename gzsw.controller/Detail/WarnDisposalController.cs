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

namespace gzsw.controller.Detail
{
    public class WarnDisposalController : BaseController
    {
        /// <summary>
        /// 获取预警处置明细
        /// </summary>
        /// <param name="beginTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="warnType">警告类型</param>
        /// <param name="warnLevel">警告级别</param>
        /// <param name="orgId">服务厅编码</param>
        /// <param name="isSenior">是否高级查询</param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [UserAuth("DETAIL_WARN_DISPOSAL_VIW,MON_Warning_VIW")]
        public ActionResult Index(DateTime? beginTime, DateTime? endTime,
            int? warnType, int? warnLevel, string orgId,
            int isSenior = 0, int pageIndex = 1, int pageSize = 20)
        {
            var orgall = new SYS_USER_DAL().GetUserORG(UserState.UserID);
            ViewBag.UserORG = new SelectList(orgall.Where(obj => obj.ORG_LEVEL == 4)
                , "ORG_ID", "ORG_NAM", orgId);

            var selectlist = EnumHelper.GetCategorySelectList(typeof(WARN_INFO_DETAIL.WARN_TYP_ENUM));
            if (null != warnType)
                selectlist.Find(obj => obj.Value == warnType.Value.ToString()).Selected = true;
            ViewBag.WARN_TYP_SELECTLIST = selectlist;
            var selectlist2 = EnumHelper.GetCategorySelectList(typeof(WARN_INFO_DETAIL.WARN_LVL_ENUM));
            if (null != warnLevel)
                selectlist2.Find(obj => obj.Value == warnLevel.Value.ToString()).Selected = true;
            ViewBag.WARN_LVL_SELECTLIST = selectlist2;

            beginTime = beginTime ?? DateTime.Now;
            endTime = endTime ?? DateTime.Now;

            ViewBag.BeginTime = beginTime.GetValueOrDefault().ToString("yyyy-MM-dd");
            ViewBag.EndTime = endTime.GetValueOrDefault().ToString("yyyy-MM-dd");
            ViewBag.WarnType = warnType;
            ViewBag.WarnLevel = warnLevel;
            ViewBag.OrgId = orgId;
            ViewBag.IsSenior = isSenior;

            var list = DETAIL_WARN_DISPOSAL_DAL.GetPager(beginTime, endTime, warnType, warnLevel, orgId, pageIndex, pageSize);

            return View(list);
        }
    }
}
