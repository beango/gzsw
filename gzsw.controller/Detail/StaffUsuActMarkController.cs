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
    /// <summary>
    /// 明细查询
    ///     日常行为表现(个人考核日常行为表现打分表CHK_STAFF_USU_ACT_MARK)
    /// </summary>
    public class StaffUsuActMarkController : BaseController<CHK_STAFF_USU_ACT_MARK>
    {
        /// <summary>
        /// 日常行为表现
        /// </summary>
        /// <param name="orgId">组织</param>
        /// <param name="beginMo">开始时间</param>
        /// <param name="endMo">结束时间</param>
        /// <param name="staffId">员工编号</param>
        /// <param name="type">行为项类型</param>
        /// <param name="isSenior">是否高级查询</param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [UserAuth("Detail_CHK_STAFF_USU_ACT_MARK")]
        public ActionResult Index(string orgId, int? beginMo, int? endMo,
            string staffId, int? type,
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

            if (beginMo == null)
            {
                beginMo=int.Parse(DateTime.Now.ToString("yyyyMM"));
                
            }
            if (endMo == null)
            {
                endMo = int.Parse(DateTime.Now.ToString("yyyyMM"));
            }

            ViewBag.BeginTime = beginMo.ToString().Insert(4, "-");
            ViewBag.EndTime = endMo.ToString().Insert(4, "-");
            ViewBag.IsSenior = isSenior;
            ViewBag.Type = type;

            var list = CHK_STAFF_USU_ACT_MARK_DAL.GetMergeList(orgId, beginMo, endMo, staffId, type, pageIndex, pageSize);
            return View(list);
        }

        [UserAuth("Detail_CHK_STAFF_USU_ACT_MARK")]
        public ActionResult Details(int id)
        {
            try
            {
                var model = CHK_STAFF_USU_ACT_MARK_DAL.GetMark(id);
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
