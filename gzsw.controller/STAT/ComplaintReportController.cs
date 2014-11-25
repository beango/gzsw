using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using gzsw.controller.MyAuth;
using gzsw.dal.dao;
using gzsw.model.Enums;
using gzsw.util.Extensions;
using gzsw.util.Office;

namespace gzsw.controller.STAT
{
    public class ComplaintReportController : StatController
    {
        /// <summary>
        /// 投诉举报分析
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [UserAuth("STAT_COMPLAIN_HALL_STAT_D_VIW")]
        public ActionResult Index()
        {
            switch (GetHighLV)
            {

                case UserLV_ENUM.服务厅级:
                    var hallNo = string.Empty;
                    var list = SYS_HALL_DAL.GetListByUserId(UserState.UserID);
                    if (list != null && list.Any())
                    {
                        var hall = list.FirstOrDefault();
                        if (hall != null)
                        {
                            hallNo = hall.HALL_NO;
                        }
                    }
                    return RedirectToAction("PersonalIndex", new { hallNo = hallNo });
                    break;
                default:
                    return RedirectToAction("HallIndex");
                    break;
            }
        }

        /// <summary>
        /// 投诉举报分析 - 服务厅
        /// </summary>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <param name="export"></param>
        /// <returns></returns>
        [HttpGet]
        [UserAuth("STAT_COMPLAIN_HALL_STAT_D_VIW")]
        public ActionResult HallIndex(DateTime? beginTime, DateTime? endTime, bool export = false)
        {
            // 初始化日期
            DateTimeInit(ref beginTime, ref endTime);

            // 获取数据
            var result = STAT_COMPLAINT_REPORT_DAL.GetStatistics_ComplaintReportChart(UserState.UserID, beginTime, endTime);

            const string titleName = "投诉举报分析";
            var mainTielt = GetOrgName(null, OrgHighLevel);
            ViewBag.MainTitle = GetTitleName(mainTielt, titleName, beginTime.GetValueOrDefault(), endTime.GetValueOrDefault());
            var subtitle = GetTitleName(mainTielt, "", beginTime.GetValueOrDefault(), endTime.GetValueOrDefault(), false);
            var exceltitle = GetTitleName(mainTielt, titleName, beginTime.GetValueOrDefault(), endTime.GetValueOrDefault(), false);
            
            var dataTable = new DataTable();
            
            dataTable.Columns.Add("序号");
            dataTable.Columns.Add("服务厅编号");
            dataTable.Columns.Add("服务厅");
            dataTable.Columns.Add("处理情况");
            dataTable.Columns.Add("处理情况类型");
            dataTable.Columns.Add("投诉类型编号");

            //获取投诉类型集合
            var complaints = new Dictionary<int, string>();
            if (result != null && result.Any())
            {
                foreach (var re in result.Select(r => new { COMPLAIN_TYP_ID = r.COMPLAIN_TYP_ID, COMPLAIN_NAM = r.COMPLAIN_NAM }).Distinct())
                {
                    if (!complaints.ContainsKey(re.COMPLAIN_TYP_ID))
                    {
                        complaints.Add(re.COMPLAIN_TYP_ID, re.COMPLAIN_NAM);
                        dataTable.Columns.Add(re.COMPLAIN_NAM);
                    }
                }
            }
            
            ViewBag.Complaints = complaints;

            if (result != null && result.Any())
            {
                var index = 0;
                var complaintCount = complaints.Count;
                for (var mIndex = 0; mIndex < result.Count; mIndex++)
                {
                    var itemHall = result[mIndex];
                    var levelCodes = new[] { "N", "R", "U", "H" };
                    foreach (var levelCode in levelCodes)
                    {
                        var itemLevels =
                            result.Where(r => r.HALL_NO == itemHall.HALL_NO && r.L_Code == levelCode).ToList();
                        var itemLevel = itemLevels.First();

                        var dr = dataTable.NewRow();
                        dr["序号"] = (++index);
                        dr["服务厅"] = itemHall.HALL_NAM;
                        dr["服务厅编号"] = itemHall.HALL_NO;
                        dr["处理情况"] = itemLevel.L_Name;
                        dr["处理情况类型"] = itemLevel.L_Value;
                        foreach (var complaint in complaints)
                        {
                            var itemComplaint = itemLevels.First(r => r.COMPLAIN_TYP_ID == complaint.Key);
                            dr["投诉类型编号"] = itemComplaint.COMPLAIN_TYP_ID;
                            dr[itemComplaint.COMPLAIN_NAM] = itemComplaint.Count_Value;
                        }
                        dataTable.Rows.Add(dr);
                    }
                    mIndex += (complaintCount * 4 - 1);
                }
            }

            //导出
            if (export)
            {
                var exportData = dataTable.Copy();
                exportData.Columns.Remove("服务厅编号");
                exportData.Columns.Remove("处理情况类型");
                exportData.Columns.Remove("投诉类型编号");
                return AsposeExcelHelper.OutFileToRespone(exportData, "投诉举报分析报表-服务厅", exceltitle);
            }


            var dataList = dataTable.Rows.Cast<DataRow>().Where(r => r["处理情况"].ToString() == "合计").ToList();
            var chartData = dataTable.Clone();
            foreach (var dataRow in dataList)
            {
                chartData.Rows.Add(dataRow.ItemArray);
            }

            //不需要显示的列名
            var noShowColumns = new[] { "序号", "服务厅编号", "处理情况", "处理情况类型", "投诉类型编号" };
            foreach (var noShowColumn in noShowColumns)
            {
                chartData.Columns.Remove(noShowColumn);
            }

            // 输出图表
            ViewBag.ChartColumn3DXML = CreateMSColumn3DChart(titleName, chartData, 420, subtitle);
            ViewBag.ChartSplineXML = CreateMSSplineChart(titleName, chartData, 420, null, null, subtitle);

            return View(dataTable);
        }

        /// <summary>
        /// 投诉举报分析 - 明细
        /// </summary>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <param name="hallNo"></param>
        /// <param name="complaintType"></param>
        /// <param name="complaintTypeName"></param>
        /// <param name="complaintLevel"></param>
        /// <param name="complaintLevelName"></param>
        /// <returns></returns>
        [HttpGet]
        [UserAuth("STAT_COMPLAIN_HALL_STAT_D_VIW")]
        public ActionResult ComplaintReportDetail(DateTime? beginTime, DateTime? endTime, string hallNo = "", int? complaintType = 0, string complaintTypeName = "", int? complaintLevel = 0, string complaintLevelName = "")
        {
            int detailType;
            string detailXValue;

            //初始化统计条件
            StatDetailTypeInit(ref beginTime, ref endTime, out detailType, out detailXValue);

            // 获取数据
            var result = STAT_COMPLAINT_REPORT_DAL.GetStatistics_ComplaintReportDetailChart(UserState.UserID, beginTime, endTime, hallNo, detailType, detailXValue, complaintType, complaintLevel);

            var chartData = new DataTable();
            if (result != null && result.Any())
            {
                chartData.Columns.Add("周期");
                chartData.Columns.Add(complaintTypeName);
                foreach (var re in result)
                {
                    var dr = chartData.NewRow();
                    dr["周期"] = re.X_ShowName;
                    dr[complaintTypeName] = re.Y_Value;
                    chartData.Rows.Add(dr);
                }
            }

            //投诉类型名称
            if (!string.IsNullOrEmpty(complaintTypeName))
            {
                complaintTypeName = string.Format("--{0}", complaintTypeName);
            }

            //投诉级别名称
            if (!string.IsNullOrEmpty(complaintLevelName))
            {
                complaintLevelName = string.Format("（{0}）", complaintLevelName);
            }

            string titleName = string.Format("投诉举报分析{0}{1}", complaintTypeName, complaintLevelName);
            var mainTielt = GetOrgName(hallNo, OrgHighLevel);
            var subtitle = GetTitleName(mainTielt, "", beginTime.GetValueOrDefault(), endTime.GetValueOrDefault(), false);

            // 输出图表
            ViewBag.ChartColumn3DXML = CreateMSColumn3DChart(titleName, chartData, 580, subtitle);
            ViewBag.ChartSplineXML = CreateMSSplineChart(titleName, chartData, 580, null, null, subtitle);
            ViewBag.ChartPie3DXML = CreatePie3DChart(titleName, chartData, 580, subtitle);

            return View();
        }


        /// <summary>
        /// 投诉举报分析 - 员工
        /// </summary>
        /// <param name="hallNo"></param>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <param name="export"></param>
        /// <returns></returns>
        [HttpGet]
        [UserAuth("STAT_COMPLAIN_HALL_STAT_D_VIW")]
        public ActionResult PersonalIndex(string hallNo, DateTime? beginTime, DateTime? endTime, bool export = false)
        {
            // 初始化日期
            DateTimeInit(ref beginTime, ref endTime);

            // 获取数据
            var result = STAT_COMPLAINT_REPORT_DAL.GetStatistics_ComplaintReportPersonalChart(hallNo, beginTime, endTime);

            const string titleName = "投诉举报分析";
            var mainTielt = GetOrgName(hallNo, null);
            ViewBag.MainTitle = GetTitleName(mainTielt, titleName, beginTime.GetValueOrDefault(), endTime.GetValueOrDefault());
            var subtitle = GetTitleName(mainTielt, "", beginTime.GetValueOrDefault(), endTime.GetValueOrDefault(), false);
            var exceltitle = GetTitleName(mainTielt, titleName, beginTime.GetValueOrDefault(), endTime.GetValueOrDefault(), false);

            var dataTable = new DataTable();

            dataTable.Columns.Add("序号");
            dataTable.Columns.Add("员工编码");
            dataTable.Columns.Add("员工姓名");
            dataTable.Columns.Add("处理情况");
            dataTable.Columns.Add("处理情况类型");
            dataTable.Columns.Add("投诉类型编号");

            //获取投诉类型集合
            var complaints = new Dictionary<int, string>();
            if (result != null && result.Any())
            {
                foreach (var re in result.Select(r => new { COMPLAIN_TYP_ID = r.COMPLAIN_TYP_ID, COMPLAIN_NAM = r.COMPLAIN_NAM }).Distinct())
                {
                    if (!complaints.ContainsKey(re.COMPLAIN_TYP_ID))
                    {
                        complaints.Add(re.COMPLAIN_TYP_ID, re.COMPLAIN_NAM);
                        dataTable.Columns.Add(re.COMPLAIN_NAM);
                    }
                }
            }

            ViewBag.Complaints = complaints;

            if (result != null && result.Any())
            {
                var index = 0;
                var complaintCount = complaints.Count;
                for (var mIndex = 0; mIndex < result.Count; mIndex++)
                {
                    var itemHall = result[mIndex];
                    var levelCodes = new[] { "N", "R", "U", "H" };
                    foreach (var levelCode in levelCodes)
                    {
                        var itemLevels =
                            result.Where(r => r.STAFF_ID == itemHall.STAFF_ID && r.L_Code == levelCode).ToList();
                        var itemLevel = itemLevels.First();

                        var dr = dataTable.NewRow();
                        dr["序号"] = (++index);
                        dr["员工姓名"] = itemHall.STAFF_NAM;
                        dr["员工编码"] = itemHall.STAFF_ID;
                        dr["处理情况"] = itemLevel.L_Name;
                        dr["处理情况类型"] = itemLevel.L_Value;
                        foreach (var complaint in complaints)
                        {
                            var itemComplaint = itemLevels.First(r => r.COMPLAIN_TYP_ID == complaint.Key);
                            dr["投诉类型编号"] = itemComplaint.COMPLAIN_TYP_ID;
                            dr[itemComplaint.COMPLAIN_NAM] = itemComplaint.Count_Value;
                        }
                        dataTable.Rows.Add(dr);
                    }
                    mIndex += (complaintCount * 4 - 1);
                }
            }

            //导出
            if (export)
            {
                var exportData = dataTable.Copy();
                exportData.Columns.Remove("员工编码");
                exportData.Columns.Remove("处理情况类型");
                exportData.Columns.Remove("投诉类型编号");
                return AsposeExcelHelper.OutFileToRespone(exportData, "投诉举报分析报表-员工", exceltitle);
            }


            var dataList = dataTable.Rows.Cast<DataRow>().Where(r => r["处理情况"].ToString() == "合计").ToList();
            var chartData = dataTable.Clone();
            foreach (var dataRow in dataList)
            {
                chartData.Rows.Add(dataRow.ItemArray);
            }

            //不需要显示的列名
            var noShowColumns = new[] { "序号", "员工编码", "投诉类型编号", "处理情况", "处理情况类型" };
            foreach (var noShowColumn in noShowColumns)
            {
                chartData.Columns.Remove(noShowColumn);
            }

            // 输出图表
            ViewBag.ChartColumn3DXML = CreateMSColumn3DChart(titleName, chartData, 420, subtitle);
            ViewBag.ChartSplineXML = CreateMSSplineChart(titleName, chartData, 420, null, null, subtitle);

            return View(dataTable);
        }


        /// <summary>
        /// 投诉举报分析 - 员工 - 明细
        /// </summary>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <param name="personalId"></param>
        /// <param name="personalName"></param>
        /// <param name="complaintType"></param>
        /// <param name="complaintTypeName"></param>
        /// <param name="complaintLevel"></param>
        /// <param name="complaintLevelName"></param>
        /// <returns></returns>
        [HttpGet]
        [UserAuth("STAT_COMPLAIN_HALL_STAT_D_VIW")]
        public ActionResult ComplaintReportPersonalDetail(DateTime? beginTime, DateTime? endTime, string personalId = "", string personalName = "", int? complaintType = 0, string complaintTypeName = "", int? complaintLevel = 0, string complaintLevelName = "")
        {
            int detailType;
            string detailXValue;

            //初始化统计条件
            StatDetailTypeInit(ref beginTime, ref endTime, out detailType, out detailXValue);

            // 获取数据
            var result = STAT_COMPLAINT_REPORT_DAL.GetStatistics_ComplaintReportDetailPersonalChart(beginTime, endTime, personalId, detailType, detailXValue, complaintType, complaintLevel);

            var chartData = new DataTable();
            if (result != null && result.Any())
            {
                chartData.Columns.Add("周期");
                chartData.Columns.Add(complaintTypeName);
                foreach (var re in result)
                {
                    var dr = chartData.NewRow();
                    dr["周期"] = re.X_ShowName;
                    dr[complaintTypeName] = re.Y_Value;
                    chartData.Rows.Add(dr);
                }
            }


            //投诉类型名称
            if (!string.IsNullOrEmpty(complaintTypeName))
            {
                complaintTypeName = string.Format("--{0}", complaintTypeName);
            }

            //投诉级别名称
            if (!string.IsNullOrEmpty(complaintLevelName))
            {
                complaintLevelName = string.Format("（{0}）", complaintLevelName);
            }

            string titleName = string.Format("投诉举报分析{0}{1}", complaintTypeName, complaintLevelName);
            var mainTielt = string.IsNullOrEmpty(personalId) ? GetOrgName(null, OrgHighLevel) : personalName;
            var subtitle = GetTitleName(mainTielt, "", beginTime.GetValueOrDefault(), endTime.GetValueOrDefault(), false);

            // 输出图表
            ViewBag.ChartColumn3DXML = CreateMSColumn3DChart(titleName, chartData, 580, subtitle);
            ViewBag.ChartSplineXML = CreateMSSplineChart(titleName, chartData, 580, null, null, subtitle);
            ViewBag.ChartPie3DXML = CreatePie3DChart(titleName, chartData, 580, subtitle);

            return View("ComplaintReportDetail");
        }
    }
}
