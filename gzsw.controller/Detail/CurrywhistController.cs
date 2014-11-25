using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using gzsw.controller.MyAuth;
using gzsw.dal.dao;

namespace gzsw.controller.Detail
{
    /// <summary>
    /// 明细查询
    ///     当月2次办理明细
    /// </summary>
    public class CurrywhistController:BaseController
    {
        /// <summary>
        /// 2次办理明细总数表
        /// </summary>
        /// <param name="orgId"></param>
        /// <param name="beginMo"></param>
        /// <param name="endMo"></param>
        /// <param name="nsrsbm"></param>
        /// <param name="nsrmc"></param>
        /// <param name="isSenior"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [UserAuth("Detail_SYS_CURRYWHIST_VIW")]
        public ActionResult Index(string orgId, int? beginMo, int? endMo,
            string nsrsbm, string nsrmc,
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
                beginMo = int.Parse(DateTime.Now.ToString("yyyyMM"));

            }
            if (endMo == null)
            {
                endMo = int.Parse(DateTime.Now.ToString("yyyyMM"));
            }

            ViewBag.BeginTime = beginMo.ToString().Insert(4, "-");
            ViewBag.EndTime = endMo.ToString().Insert(4, "-");
            ViewBag.IsSenior = isSenior;
            ViewBag.Nsrsbm = nsrsbm;
            ViewBag.Nsrmc = nsrmc;

            var list = SYS_CURRYWHIST_DAL.GetMergeList(orgId, beginMo.GetValueOrDefault(), endMo.GetValueOrDefault(),
                nsrsbm, nsrmc, pageIndex, pageSize);

            return View(list);
        }

        /// <summary>
        /// 2次办理明细项列表
        /// </summary>
        /// <param name="orgId"></param>
        /// <param name="nsrsbm"></param>
        /// <param name="beginMo"></param>
        /// <param name="endMo"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [UserAuth("Detail_SYS_CURRYWHIST_VIW")]
        public ActionResult DetailList(string orgId,string nsrsbm, int? beginMo, int? endMo, int pageIndex = 1, int pageSize=20)
        {
            if (beginMo == null)
            {
                beginMo = int.Parse(DateTime.Now.ToString("yyyyMM"));

            }
            if (endMo == null)
            {
                endMo = int.Parse(DateTime.Now.ToString("yyyyMM"));
            }

            var list = SYS_CURRYWHIST_DAL.GetMergeDetailList(orgId,nsrsbm, beginMo.GetValueOrDefault(),
                endMo.GetValueOrDefault(), pageIndex, pageSize);

            return View(list);
        }
    }
}
