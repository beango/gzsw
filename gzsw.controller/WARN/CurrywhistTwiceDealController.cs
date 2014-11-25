﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using gzsw.controller.MyAuth;
using gzsw.dal.dao;

namespace gzsw.controller.WARN
{
    public class CurrywhistTwiceDealController : BaseController
    {
        /// <summary>
        /// 获取当月2次办理人业务明细
        /// </summary>
        /// <param name="orgId">服务厅编码</param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [UserAuth("MON_Warning_VIW")]
        public ActionResult Index(string orgId, int pageIndex = 1, int pageSize = 20)
        {
            var list = SYS_CURRYWHIST_DAL.GetTwiceDealPager(orgId, pageIndex, pageSize);

            return View(list);
        }
    }
}
