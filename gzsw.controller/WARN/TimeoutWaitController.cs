using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using gzsw.dal.dao;

namespace gzsw.controller.WARN
{
    public class TimeoutWaitController : BaseController
    {
        /// <summary>
        /// 获取 超时等待 排队业务明细
        /// </summary>
        /// <param name="orgId">服务厅编码</param>
        /// <param name="pageIndex"></param>O
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public ActionResult Index(string orgId, int pageIndex = 1, int pageSize = 20)
        {
            var list = SYS_CURRQUEUEHIST_DAL.GetTimeoutWaitPager(orgId, pageIndex, pageSize);

            return View(list);
        }
    }
}
