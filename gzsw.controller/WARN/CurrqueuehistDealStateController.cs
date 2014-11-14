using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using gzsw.dal.dao;

namespace gzsw.controller.WARN
{
    public class CurrqueuehistDealStateController : BaseController
    {
        /// <summary>
        /// 根据办理状态类型获取排队业务明细
        /// </summary>
        /// <param name="orgId">服务厅编码</param>
        /// <param name="dealStateType">办理状态类型（0：所有  1：等待中  2：已受理  3：已弃号）</param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public ActionResult Index(string orgId, int dealStateType, int pageIndex = 1, int pageSize = 20)
        {
            var list = SYS_CURRQUEUEHIST_DAL.GetPagerByDealStateType(orgId, dealStateType, pageIndex, pageSize);

            return View(list);
        }
    }
}
