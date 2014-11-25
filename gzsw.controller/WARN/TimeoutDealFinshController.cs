using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using gzsw.controller.MyAuth;
using gzsw.dal.dao;
using gzsw.util;

namespace gzsw.controller.WARN
{
    public class TimeoutDealFinshController : BaseController
    {
        /// <summary>
        /// 获取 超时办结 台账业务明细
        /// </summary>
        /// <param name="orgId">服务厅编码</param>
        /// <param name="pageIndex"></param>O
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [UserAuth("MON_Warning_VIW")]
        public ActionResult Index(string orgId, int pageIndex = 1, int pageSize = 20)
        {
            var list = SYS_CURRYWHIST_DAL.GetTimeoutDealFinshPager(orgId, pageIndex, pageSize);

            return View(list);
        }

        /// <summary>
        /// 查看明细
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [UserAuth("MON_Warning_VIW")]
        public ActionResult Details(string id)
        {
            try
            {
                var model = SYS_CURRYWHIST_DAL.GetOne(id);

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
