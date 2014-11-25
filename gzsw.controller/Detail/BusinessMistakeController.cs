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
using Ninject;

namespace gzsw.controller.Detail
{
    public class BusinessMistakeController : BaseController
    {
        /// <summary>
        /// 质量类型
        /// </summary>
        [Inject]
        public IDao<CHK_QUALITY_CON> QualityDao { get; set; }

        /// <summary>
        /// 获取业务差错明细 手动自定义和Excel导入
        /// </summary>
        /// <param name="orgId">服务厅</param>
        /// <param name="beginTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="qualityId">质量类型编码</param>
        /// <param name="serialName">明细业务名</param>
        /// <param name="isSenior">是否高级查询</param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [UserAuth("DETAIL_BUSINESS_MISTAKE_VIW")]
        public ActionResult Index(string orgId, DateTime? beginTime, DateTime? endTime,
            string qualityId, string serialName,
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

            var qualitys = QualityDao.FindList();
            if (qualitys != null)
            {
                var qualityList =
                    qualitys.Select(r => new {QUALITY_CD = r.QUALITY_CD, QUALITY_NAM = r.QUALITY_NAM}).ToList();
                qualityList.Insert(0, new {QUALITY_CD = string.Empty, QUALITY_NAM = "--请选择--"});

                ViewBag.Qualitys = new SelectList(qualityList, "QUALITY_CD", "QUALITY_NAM", qualityId ?? string.Empty);
            }

            beginTime = beginTime ?? DateTime.Now;
            endTime = endTime ?? DateTime.Now;

            ViewBag.BeginTime = beginTime.GetValueOrDefault().ToString("yyyy-MM-dd");
            ViewBag.EndTime = endTime.GetValueOrDefault().ToString("yyyy-MM-dd");
            ViewBag.QualityId = qualityId;
            ViewBag.SerialName = serialName;
            ViewBag.IsSenior = isSenior;

            var list = DETAIL_BUSINESS_MISTAKE_DAL.GetPager(beginTime, endTime, orgId, qualityId, serialName, pageIndex, pageSize);

            return View(list);
        }

        /// <summary>
        /// 查看明细
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [UserAuth("DETAIL_BUSINESS_MISTAKE_VIW")]
        public ActionResult Details(string id)
        {
            try
            {
                var dataSource = 1;
                var seq = 0;
                if (!string.IsNullOrEmpty(id))
                {
                    dataSource = Convert.ToInt32(id.Split('-')[0]);
                    seq = Convert.ToInt32(id.Split('-')[1]);
                }

                var model = DETAIL_BUSINESS_MISTAKE_DAL.GetOne(dataSource, seq);

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
