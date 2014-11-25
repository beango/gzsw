using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using gzsw.controller.MyAuth;
using gzsw.dal.dao;
using gzsw.model.dto;
using gzsw.model.Enums;
using gzsw.util;
using gzsw.util.Extensions;
using gzsw.util.Office;

namespace gzsw.controller.STAT
{
    public class WarnAnalysisController : StatController
    {
        /// <summary>
        /// 预警分析
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [UserAuth("STAT_WARN_HALL_STAT_D_VIW")]
        public ActionResult Index()
        {
            switch (GetHighLV)
            {
                default:
                    return RedirectToAction("HallIndex");
                    break;
            }
        }

        /// <summary> = 
        /// 预警分析 - 服务厅
        /// </summary>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <param name="export"></param>
        /// <returns></returns>
        [HttpGet]
        [UserAuth("STAT_WARN_HALL_STAT_D_VIW")]
        public ActionResult HallIndex(DateTime? beginTime, DateTime? endTime, bool export = false)
        {
            // 初始化日期
            DateTimeInit(ref beginTime, ref endTime);
            
            // 获取数据
            var result = STAT_WARN_ANALYSIS_DAL.GetStatistics_WarnAnalysisChart(UserState.UserID, beginTime, endTime);

            const string titleName = "预警分析";
            var mainTielt = GetOrgName(null, OrgHighLevel);
            ViewBag.MainTitle = GetTitleName(mainTielt, titleName, beginTime.GetValueOrDefault(), endTime.GetValueOrDefault());
            var subtitle = GetTitleName(mainTielt, "", beginTime.GetValueOrDefault(), endTime.GetValueOrDefault(), false);

           //导出
            if (export)
            {
                var exceltitle = GetTitleName(mainTielt, titleName, beginTime.GetValueOrDefault(), endTime.GetValueOrDefault(), false);
                 
                var index = 0;
                var data = result == null
                    ? new DataTable()
                    : result.Select(x => new
                    {
                        序号 = ++index,
                        服务厅 = x.HALL_NAM,
                        预警级别 = x.L_Name,
                        等候超时 = x.Count_T1,
                        等候超时率 = x.Count_T2,
                        窗口饱和度 = x.Count_T3,
                        大厅饱和度 = x.Count_T4,
                        超时办结率 = x.Count_T5,
                        超时业务笔数 = x.Count_T6,
                        弃号率 = x.Count_T7,
                        差评笔数预警 = x.Count_T8,
                        连续工作时长超界 = x.Count_T9,
                    }).ToList().ToDataTable();
                return AsposeExcelHelper.OutFileToRespone(data, "预警分析报表-服务厅", exceltitle);
            }


            var chartData = result == null
                ? new DataTable()
                : result.Where(r => r.L_Code == "H").Select(x => new
                {
                    服务厅 = x.HALL_NAM,
                    等候超时率 = x.Count_T2,
                    超时办结率 = x.Count_T5,
                    差评笔数预警 = x.Count_T8,

                }).ToList().ToDataTable();

            ViewBag.BeginTime = !beginTime.HasValue ? DateTime.Now.ToString("yyyy-MM-dd") : beginTime.Value.ToString("yyyy-MM-dd");
            ViewBag.EndTime = !endTime.HasValue ? DateTime.Now.ToString("yyyy-MM-dd") : endTime.Value.ToString("yyyy-MM-dd");

            // 输出图表
            ViewBag.ChartColumn3DXML = CreateMSColumn3DChart(titleName, chartData, 420, subtitle);
            ViewBag.ChartSplineXML = CreateMSSplineChart(titleName, chartData, 420, null, null, subtitle);
            
            return View(result);
        }

        /// <summary>
        /// 预警分析 - 明细
        /// </summary>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <param name="hallNo"></param>
        /// <param name="warnType"></param>
        /// <param name="warnTypeName"></param>
        /// <param name="warnLevel"></param>
        /// <param name="warnLevelName"></param>
        /// <returns></returns>
        [HttpGet]
        [UserAuth("STAT_WARN_HALL_STAT_D_VIW")]
        public ActionResult WarnAnalysisDetail(DateTime? beginTime, DateTime? endTime, string hallNo = "", int? warnType = 0, string warnTypeName = "", int? warnLevel = 0, string warnLevelName = "")
        {
            int detailType;
            string detailXValue;

            //初始化统计条件
            StatDetailTypeInit(ref beginTime, ref endTime, out detailType, out detailXValue);

            // 获取数据
            var result = STAT_WARN_ANALYSIS_DAL.GetStatistics_WarnAnalysisDetailChart(UserState.UserID, beginTime, endTime, hallNo, detailType, detailXValue, warnType, warnLevel);
            
            var chartData = default(DataTable);
            if (result == null)
            {
                chartData = new DataTable();
            }
            else
            {
                switch (warnType)
                {
                    default:
                        chartData = result.Select(x => new
                        {
                            周期 = x.X_ShowName,
                            预警分析 = x.Y_Value
                        }).ToList().ToDataTable();
                        break;
                    case 1:
                        chartData = result.Select(x => new
                        {
                            周期 = x.X_ShowName,
                            等候超时 = x.Y_Value
                        }).ToList().ToDataTable();
                        break;
                    case 2:
                        chartData = result.Select(x => new
                        {
                            周期 = x.X_ShowName,
                            等候超时率 = x.Y_Value
                        }).ToList().ToDataTable();
                        break;
                    case 3:
                        chartData = result.Select(x => new
                        {
                            周期 = x.X_ShowName,
                            窗口饱和度 = x.Y_Value
                        }).ToList().ToDataTable();
                        break;
                    case 4:
                        chartData = result.Select(x => new
                        {
                            周期 = x.X_ShowName,
                            大厅饱和度 = x.Y_Value
                        }).ToList().ToDataTable();
                        break;
                    case 5:
                        chartData = result.Select(x => new
                        {
                            周期 = x.X_ShowName,
                            超时办结率 = x.Y_Value
                        }).ToList().ToDataTable();
                        break;
                    case 6:
                        chartData = result.Select(x => new
                        {
                            周期 = x.X_ShowName,
                            超时业务笔数 = x.Y_Value
                        }).ToList().ToDataTable();
                        break;
                    case 7:
                        chartData = result.Select(x => new
                        {
                            周期 = x.X_ShowName,
                            弃号率 = x.Y_Value
                        }).ToList().ToDataTable();
                        break;
                    case 8:
                        chartData = result.Select(x => new
                        {
                            周期 = x.X_ShowName,
                            差评笔数预警 = x.Y_Value
                        }).ToList().ToDataTable();
                        break;
                    case 9:
                        chartData = result.Select(x => new
                        {
                            周期 = x.X_ShowName,
                            连续工作时长超界 = x.Y_Value
                        }).ToList().ToDataTable();
                        break;
                }
            }

            //预警类型名称
            if (!string.IsNullOrEmpty(warnTypeName))
            {
                warnTypeName = string.Format("--{0}", warnTypeName);
            }

            //预警级别名称
            if (!string.IsNullOrEmpty(warnLevelName))
            {
                warnLevelName = string.Format("（{0}）", warnLevelName);
            }
            
            string titleName = string.Format("预警分析{0}{1}", warnTypeName, warnLevelName);
            var mainTielt = GetOrgName(hallNo, OrgHighLevel);
            var subtitle = GetTitleName(mainTielt, "", beginTime.GetValueOrDefault(), endTime.GetValueOrDefault(), false);
            
            // 输出图表
            ViewBag.ChartColumn3DXML = CreateMSColumn3DChart(titleName, chartData, 580, subtitle);
            ViewBag.ChartSplineXML = CreateMSSplineChart(titleName, chartData, 580, null, null, subtitle);
            ViewBag.ChartPie3DXML = CreatePie3DChart(titleName, chartData, 580, subtitle);

            return View();
        }
    }
}
