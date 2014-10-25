 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using gzsw.dal;
using gzsw.dal.dao;
using gzsw.model;
using gzsw.model.ajax;
using gzsw.model.dto;

namespace gzsw.controller.Map
{
    /// <summary>
    ///  
    /// </summary>
    /// <remark>
    ///     <para>    Creator：hcli </para>
    ///     <para>CreatedTime：2014/10/16 10:50:34</para>
    /// </remark>
    public class MapServerController : BaseController
    {
        /// <summary>
        /// 组织机构
        /// </summary>
        [Ninject.Inject]
        public IDao<SYS_ORGANIZE> Organize { get; set; }


        /// <summary>
        /// 地图标示组织机构
        /// </summary>
        /// <param name="orgId">组织机构ID</param>
        /// <returns></returns>
        public ActionResult Index(string  orgId)
        {

            var nowModel = Organize.GetEntity("ORG_ID", orgId); 
             
            var model = new SYS_ORGANIZE_DAL().GetListForParent(UserState.UserID, orgId);
            ViewData["Marker"] = model;
            ViewData["MapName"] = nowModel.ORG_NAM+"-监控分布地图";
            return View("~/Views/Map/Index.cshtml", nowModel);
        }

        /// <summary>
        /// 获取图标数据
        /// </summary>
        /// <returns></returns>
        public ActionResult GetChartData(string orgId)
        {
            var OrganizeObj = Organize.GetEntity("ORG_ID", orgId);
            if (OrganizeObj != null)
            { 
                byte level = (byte)OrganizeObj.ORG_LEVEL;
                if (level > 0 && level < 4)
                {
                    level += 1;
                }
                var organizeQueueingData = new Chart_DAL().GetOrganizeQueueing(UserState.UserID, orgId, level);
                return Json(new ApiJsonResult<List<OrganizeQueueingDto>>("成功")
                {
                    Data = organizeQueueingData.ToList()
                }, JsonRequestBehavior.AllowGet);
            }
            return Json(null, JsonRequestBehavior.AllowGet);
        }

   /*     /// <summary>
        /// 加载图表数据
        /// </summary>
        /// <param name="orgId">组织机构ID</param>
        /// <returns></returns>
        public ActionResult Chart(string orgId)
        {
            var OrganizeObj = Organize.GetEntity("ORG_ID", orgId);
            if (OrganizeObj != null && OrganizeObj.ORG_LEVEL!=null)
            {
                byte level = (byte) OrganizeObj.ORG_LEVEL;
                if (level > 0 && level < 4)
                {
                    level += 1;
                }
                var organizeQueueingData = new Chart_DAL().GetOrganizeQueueing(UserState.UserID,orgId, level);
                var organizeX = string.Empty; 
                var waitPersonY = string.Empty;
                var waitTimesY = string.Empty;
                foreach (var item in organizeQueueingData)
                {
                    organizeX = organizeX + "'"+item.Name+"'"+",";
                    waitPersonY = waitPersonY + item.WaitPersonsCount + ",";
                    waitTimesY = waitTimesY + item.WaitPersonsTime + ",";
                } 
                ViewData["organizeX"] = organizeX.TrimEnd(','); 
                ViewData["waitPersonY"] = waitPersonY.TrimEnd(','); 
                ViewData["waitTimesY"] = waitTimesY.TrimEnd(',');
            }

            //Chart_DAL
            return View("~/Views/Map/Chart.cshtml");
        }*/
    }
}
