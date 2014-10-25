using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gzsw.controller.MyAuth;
using gzsw.dal;
using gzsw.model;
using System.Web.Mvc;
using PetaPoco;
using gzsw.util;
using gzsw.dal.dao;
using gzsw.model.dto;

namespace gzsw.controller
{
    /// <summary>
    ///  
    /// </summary>
    /// <remark>
    ///     <para>    Creator：hcli </para>
    ///     <para>CreatedTime：2014/10/23 17:16:24</para>
    /// </remark>
    public class StatisticsController : BaseController
    {
        /// <summary>
        /// 个人满意度报表查询
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [UserAuth("STAT_DAYYWTJ")]
        public ActionResult STAT_DAYYWTJ(
            DateTime? txtBeginTime,
            DateTime? txtEndTime,
            string hdORG_NAM,
            string txtORG_NAM)
        {
            if (txtBeginTime == null||txtBeginTime == DateTime.MinValue)
            {
                txtBeginTime = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd")).AddDays(-1);
            }
            if (txtEndTime == null || txtEndTime == DateTime.MinValue)
            {
                txtEndTime = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd "));
            }

            ViewData["organizeName"] = txtORG_NAM;
            ViewData["organizeId"] = hdORG_NAM;
            ViewData["beginTime"] = (txtBeginTime != null && txtBeginTime != DateTime.MinValue) ? ((DateTime)txtBeginTime).ToString("yyyy-MM-dd") : "";
            ViewData["txtEndTime"] = (txtEndTime != null && txtEndTime != DateTime.MinValue) ? ((DateTime)txtEndTime).ToString("yyyy-MM-dd") : "";
            if (string.IsNullOrEmpty(hdORG_NAM))
            {
                var organize = new SYS_ORGANIZE_DAL().GetTopForUserId(UserState.UserID);
                if (organize != null)
                {
                    var data = new Statistics_DAL().GetSTAT_DAYYWTJ(txtBeginTime, txtEndTime, organize.ORG_ID);
                    ViewData["organizeName"] = organize.ORG_NAM;
                    ViewData["organizeId"] = organize.ORG_ID;
                    // 加载满意度图表数据
                    LoadChartForDAYYWTJ(organize.ORG_ID, txtBeginTime,txtEndTime);
                   
                    return View(data);
                }
                return View();
            }
            // 加载满意度图表数据
            LoadChartForDAYYWTJ(hdORG_NAM, txtBeginTime, txtEndTime);
            var viewModel = new Statistics_DAL().GetSTAT_DAYYWTJ(txtBeginTime, txtEndTime, hdORG_NAM);
            return View(viewModel);
        }



        /// <summary>
        /// 个人绩效报表查询
        /// </summary>
        /// <param name="txtBeginTime">开始时间</param>
        /// <param name="txtEndTime">结束时间</param>
        /// <param name="hdORG_NAM">组织机构ID</param>
        /// <param name="txtORG_NAM">组织机构名称</param>
        /// <returns></returns>
        [HttpGet]
        [UserAuth("STAT_DAYQUEUETJ")]
        public ActionResult STAT_DAYQUEUETJ(
            DateTime? txtBeginTime,
            DateTime? txtEndTime,
            string hdORG_NAM,
            string txtORG_NAM)
        {

            if (txtBeginTime == null || txtBeginTime == DateTime.MinValue)
            {
                txtBeginTime = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd")).AddDays(-1);
            }
            if (txtEndTime == null || txtEndTime == DateTime.MinValue)
            {
                txtEndTime = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd "));
            }

            ViewData["organizeName"] = txtORG_NAM;
            ViewData["organizeId"] = hdORG_NAM;
            ViewData["beginTime"] = (txtBeginTime != null && txtBeginTime != DateTime.MinValue) ? ((DateTime)txtBeginTime).ToString("yyyy-MM-dd") : "";
            ViewData["txtEndTime"] = (txtEndTime != null && txtEndTime != DateTime.MinValue) ?  ((DateTime)txtEndTime).ToString("yyyy-MM-dd") : ""; 
            if (string.IsNullOrEmpty(hdORG_NAM))
            {
                var organize = new SYS_ORGANIZE_DAL().GetTopForUserId(UserState.UserID);
                if (organize != null)
                {
                    var data = new Statistics_DAL().GetDAYQUEUETJ(txtBeginTime, txtEndTime, organize.ORG_ID);
                    ViewData["organizeName"] = organize.ORG_NAM;
                    ViewData["organizeId"] = organize.ORG_ID;

                    LoadDAYQUEUETJChart(organize.ORG_ID, txtBeginTime, txtEndTime);
                    return View(data);
                }
                return View();
            }
            LoadDAYQUEUETJChart(hdORG_NAM, txtBeginTime, txtEndTime);
            var viewModel = new Statistics_DAL().GetDAYQUEUETJ(txtBeginTime, txtEndTime, hdORG_NAM);
            return View(viewModel);

        }

        #region Helper
 


        /// <summary>
        /// 获取满意度图表数据
        /// </summary>
        /// <param name="orgId">组织结构ID</param>
        /// <param name="txtBeginTime">开始时间</param>
        /// <param name="txtEndTime">结束时间</param>
        private void LoadChartForDAYYWTJ(
            string orgId,
            DateTime? txtBeginTime,
            DateTime? txtEndTime)
        {
            var data = new Statistics_DAL().GetSTAT_DAYYWTJChart(orgId, txtBeginTime, txtEndTime);

            var xValue = string.Empty;
            var y0Value = string.Empty;
            var y1Value = string.Empty;
            var y2Value = string.Empty;
            var y3Value = string.Empty;
            var y4Value = string.Empty;
            var y5Value = string.Empty;
            foreach (var item in data)
            {
                xValue += "'" + item.STAFF_NAM + "',";
                y0Value += item.QDDTJ_PJKEY0NUM + ",";
                y1Value += item.QDDTJ_PJKEY1NUM + ",";
                y2Value += item.QDDTJ_PJKEY2NUM + ",";
                y3Value += item.QDDTJ_PJKEY3NUM + ",";
                y4Value += item.QDDTJ_PJKEY4NUM + ",";
                y5Value += item.QDDTJ_PJKEY5NUM + ",";
            }




            xValue = xValue.TrimEnd(','); 
            xValue = "[" + xValue + "]";
            y0Value = y0Value.TrimEnd(',');
            y0Value = "[" + y0Value + "]";
            y1Value = y1Value.TrimEnd(',');
            y1Value = "[" + y1Value + "]";
            y2Value = y2Value.TrimEnd(',');
            y2Value = "[" + y2Value + "]";
            y3Value = y3Value.TrimEnd(',');
            y3Value = "[" + y3Value + "]";
            y4Value = y4Value.TrimEnd(',');
            y4Value = "[" + y4Value + "]";
            y5Value = y5Value.TrimEnd(',');
            y5Value = "[" + y5Value + "]";

            ViewData["x"] = xValue;
            ViewData["y0Value"] = y0Value;
            ViewData["y1Value"] = y1Value;
            ViewData["y2Value"] = y2Value;
            ViewData["y3Value"] = y3Value;
            ViewData["y4Value"] = y4Value;
            ViewData["y5Value"] = y5Value;
        }

        /// <summary>
        /// 加载个人绩效报表
        /// </summary>
        /// <param name="orgId"></param>
        private void LoadDAYQUEUETJChart(string orgId,
            DateTime? beginTime,
            DateTime? endTime)
        {
            var data = new Statistics_DAL().GetCharForDAYQUEUETJChart(orgId, beginTime, endTime);

            var xValue = string.Empty;
            // 呼叫量
            var y1Value = string.Empty;
            // 办理量
            var y2Value = string.Empty;
            // 弃号量
            var y3Value = string.Empty;
            /* // 组织机构名称
            var y4Value = string.Empty; */



            foreach (var item in data)
            {
                xValue += "'" + item.STAFF_NAM + "',";
                y1Value += item.PersonCount + ",";
                y2Value += item.STAFF_QDDTJ_RLL + ",";
                y3Value += item.STAFF_QDDTJ_QHNUM + ",";
                /*  y4Value += item.ORG_NAM + ",";*/
            }
            xValue = xValue.TrimEnd(',');
            xValue = "[" + xValue + "]";

            y1Value = y1Value.TrimEnd(',');
            y1Value = "[" + y1Value + "]";

            y2Value = y2Value.TrimEnd(',');
            y2Value = "[" + y2Value + "]";

            y3Value = y3Value.TrimEnd(',');
            y3Value = "[" + y3Value + "]";

            ViewData["x"] = xValue;
            ViewData["y1"] = y1Value;
            ViewData["y2"] = y2Value;
            ViewData["y3"] = y3Value;
        }

        #endregion
    }

    
}
