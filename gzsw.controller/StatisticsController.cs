using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using gzsw.controller.MyAuth;
using gzsw.controller.STAT;
using gzsw.dal;
using gzsw.model;
using System.Web.Mvc;
using gzsw.model.Enums;
using gzsw.model.Subclasses;
using gzsw.util.Extensions;
using gzsw.util.Office;
using Ninject;
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
    public class StatisticsController : StatController
    {
        /// <summary>
        /// 质量类型
        /// </summary>
        [Inject]
        private IDao<CHK_QUALITY_CON> CHK_QUALITY_CON_Dao  { get; set; }

        /// <summary>
        /// 事项大类
        /// </summary>
        [Inject]
        private IDao<SYS_DLSERIAL> SYS_DLSERIAL_Dao { get; set; }


        #region 业务差错分析

        /// <summary>
        /// 业务差错分析-服务厅
        /// </summary>
        /// <param name="beginTime">开始时间</param>
        /// <param name="endTime">结束时间</param> 
        /// <param name="export">是否导出Excel</param>
        /// <returns></returns>
        [HttpGet]
        [UserAuth("STAT_STAFF_QUALITY_STAT_D_VIW")] 
        public ActionResult ServiceSlipAnalysis(
            DateTime? beginTime,
            DateTime? endTime, 
            bool export = false)

        {
            var orgId = string.Empty;
            switch (GetHighLV)
            {
                case UserLV_ENUM.省级:
                case UserLV_ENUM.市级:
                case UserLV_ENUM.区级:
                    break;
                case UserLV_ENUM.服务厅级:
                    var listOrgs = SYS_HALL_DAL.GetListByUserId(UserState.UserID);
                    if (listOrgs.Count == 1)
                    {
                        orgId = listOrgs[0].HALL_NO;
                    }
                    return RedirectToAction("ServiceSlipAnalysis_Person", new {orgId = orgId});
                    break;
            }

            base.DateTimeInit(ref beginTime, ref endTime);
            var data = new Statistics_DAL().GetStatistics_ServiceSlipAnalysis(beginTime, endTime, UserState.UserID);

            var mainTielt = GetOrgName(null, 2);
            ViewBag.MainTitle = GetTitleName(mainTielt, "业务差错分析", beginTime.GetValueOrDefault(), endTime.GetValueOrDefault());
            var subtitle = GetTitleName(mainTielt, "", beginTime.GetValueOrDefault(), endTime.GetValueOrDefault(), false);
            //var exceltitle = GetTitleName(mainTielt, "业务办理分析", beginTime.GetValueOrDefault(), endTime.GetValueOrDefault(), false);


            // 所有质量类型
            var typeList = CHK_QUALITY_CON_Dao.FindList();
            ViewData["typeList"] = typeList;

            // 所有事项大类
            var itemTypeList = SYS_DLSERIAL_Dao.FindList();
            ViewData["itemTypeList"] = itemTypeList;

            var list= data.GroupBy(m => m.HALL_NAM);

            SetChart(list, typeList, "服务厅业务差错分析", beginTime.GetValueOrDefault(), endTime.GetValueOrDefault(), subtitle);

            return View(list);
        }

        /// <summary>
        /// 业务差错分析-员工
        /// </summary>
        /// <param name="orgId">服务厅</param>
        /// <param name="beginTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="export">是否导出Excel</param>
        /// <returns></returns>
        public ActionResult ServiceSlipAnalysis_Person(
            string orgId,
            DateTime? beginTime,
            DateTime? endTime,
            bool export=false
            )
        {
            base.DateTimeInit(ref beginTime, ref endTime);

            var listStatistics = new Statistics_DAL().GetStatistics_ServiceSlipAnalysisList(beginTime, endTime,
               UserState.UserID, orgId);

            var mainTielt = GetOrgName(null, 2);
            ViewBag.MainTitle = GetTitleName(mainTielt, "业务差错分析", beginTime.GetValueOrDefault(), endTime.GetValueOrDefault());
            var subtitle = GetTitleName(mainTielt, "", beginTime.GetValueOrDefault(), endTime.GetValueOrDefault(), false);
            //var exceltitle = GetTitleName(mainTielt, "业务办理分析", beginTime.GetValueOrDefault(), endTime.GetValueOrDefault(), false);


            var dic = listStatistics.GroupBy(m => m.STAFF_NAM);

            // 所有质量类型
            var typeList = CHK_QUALITY_CON_Dao.FindList();
            ViewData["typeList"] = typeList;

            // 所有事项大类
            var itemTypeList = SYS_DLSERIAL_Dao.FindList();
            ViewData["itemTypeList"] = itemTypeList;


            SetStaffChart(listStatistics, typeList, "员工业务差错分析", beginTime.GetValueOrDefault(), endTime.GetValueOrDefault(), subtitle);

            return View(dic);
        }

        /// <summary>
        /// 业务差错统计图形
        /// </summary>
        /// <param name="list"></param>
        /// <param name="typeList"></param>
        /// <param name="title"></param>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        public void SetChart(IEnumerable<IGrouping<string, gzsw.model.dto.ServiceSlipAnalysisDto>> list,
            IList<CHK_QUALITY_CON> typeList, string title, DateTime beginTime, DateTime endTime, string subtitle)
        {
            #region 柱图

            var lineTable = new DataTable();
            lineTable.Columns.Add("Y_Name", typeof (string));

            foreach (var item in typeList)
            {
                lineTable.Columns.Add(item.QUALITY_NAM, typeof (int));
            }

            foreach (var item in list)
            {
                var items = item.ToList();

                var r = lineTable.NewRow();
                r["Y_Name"] = item.Key;

                foreach (var type in typeList)
                {
                    var def = items.Where(m => m.QUALITY_CD == type.QUALITY_CD).Sum(m => m.AMOUNT);
                    r[type.QUALITY_NAM] = def;
                }
                lineTable.Rows.Add(r);
            }

            var dss = new DataSet();
            dss.Tables.Add(lineTable);
            ViewBag.ChartColumn3DXML = base.CreateMSColumn3DChart(title, dss, 430, subtitle);

            #endregion

            #region 线形图
            var lineTable2 = new DataTable();
            lineTable2.Columns.Add("Y_Name", typeof(string));
            foreach (var item in typeList)
            {
                lineTable2.Columns.Add(item.QUALITY_NAM, typeof(int));
            }

            TimeSpan timeSpan = endTime.Subtract(beginTime);
            var tlist = new List<string>();
            base.SetLineYName(timeSpan, beginTime, lineTable2, tlist, endTime);

            var listStatistics = new Statistics_DAL().GetStatistics_ServiceSlipAnalysisList(beginTime, endTime,
                UserState.UserID);

            for (var i = 0; i < lineTable2.Rows.Count; i++)
            {
                var tempbtiem = Convert.ToDateTime(tlist[i]);
                var tempetiem = i + 1 < lineTable2.Rows.Count ? Convert.ToDateTime(tlist[i + 1]) : endTime;

                var soureList = new List<STAT_STAFF_QUALITY_STAT_D>();
                if (timeSpan.Days < 1)
                {
                    soureList = listStatistics.
                        Where(x => x.TIME_QUANTUM_CD == Convert.ToByte(tempbtiem.Hour.ToString())).ToList();
                }
                else
                {
                    if (Convert.ToDateTime(tempbtiem.ToShortDateString()) ==
                        Convert.ToDateTime(endTime.ToShortDateString()))
                    {
                        soureList =
                            listStatistics.Where(
                                x =>
                                    x.STAT_DT >= Convert.ToDateTime(tempbtiem.ToShortDateString()) &&
                                    x.STAT_DT < Convert.ToDateTime(tempetiem.AddDays(1).ToShortDateString())).ToList();
                    }
                    else
                    {
                        soureList =
                            listStatistics.Where(
                                x =>
                                    x.STAT_DT >= Convert.ToDateTime(tempbtiem.ToShortDateString()) &&
                                    x.STAT_DT < Convert.ToDateTime(tempetiem.ToShortDateString())).ToList();
                    }
                }

                foreach (var item in typeList)
                {
                    var def = 0;
                    def = soureList.Where(m => m.QUALITY_CD == item.QUALITY_CD).Sum(m => m.AMOUNT);
                    lineTable2.Rows[i][item.QUALITY_NAM] = def;
                }
            }
            var dss2 = new DataSet();
            dss2.Tables.Add(lineTable2);
            ViewBag.ChartSplineXML = base.CreateMSSplineChart(title, dss2, 430, null, null, subtitle); 
            #endregion
            
        }

        /// <summary>
        /// 员工业务差错统计图形
        /// </summary>
        /// <param name="list"></param>
        /// <param name="typeList"></param>
        /// <param name="title"></param>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        private void SetStaffChart(IEnumerable<STAT_STAFF_QUALITY_STAT_D_SUB> list,
            IList<CHK_QUALITY_CON> typeList, string title, DateTime beginTime, DateTime endTime, string subtitle)
        {
            #region 柱图
            var lineTable = new DataTable();
            lineTable.Columns.Add("Y_Name", typeof(string));

            foreach (var item in typeList)
            {
                lineTable.Columns.Add(item.QUALITY_NAM, typeof(int));
            }

            var dic = list.GroupBy(m => m.STAFF_NAM);

            foreach (var item in dic)
            {
                var items = item.ToList();

                var r = lineTable.NewRow();
                r["Y_Name"] = item.Key;

                foreach (var type in typeList)
                {
                    var def = items.Where(m => m.QUALITY_CD == type.QUALITY_CD).Sum(m => m.AMOUNT);
                    r[type.QUALITY_NAM] = def;
                }
                lineTable.Rows.Add(r);
            }

            var dss = new DataSet();
            dss.Tables.Add(lineTable);
            ViewBag.ChartColumn3DXML = base.CreateMSColumn3DChart(title, dss, 430, subtitle); 
            #endregion

            #region 线形图
            var lineTable2 = new DataTable();
            lineTable2.Columns.Add("Y_Name", typeof(string));
            foreach (var item in typeList)
            {
                lineTable2.Columns.Add(item.QUALITY_NAM, typeof(int));
            }

            TimeSpan timeSpan = endTime.Subtract(beginTime);
            var tlist = new List<string>();
            base.SetLineYName(timeSpan, beginTime, lineTable2, tlist, endTime);

            for (var i = 0; i < lineTable2.Rows.Count; i++)
            {
                var tempbtiem = Convert.ToDateTime(tlist[i]);
                var tempetiem = i + 1 < lineTable2.Rows.Count ? Convert.ToDateTime(tlist[i + 1]) : endTime;

                var soureList = new List<STAT_STAFF_QUALITY_STAT_D_SUB>();
                if (timeSpan.Days < 1)
                {
                    soureList = list.
                        Where(x => x.TIME_QUANTUM_CD == Convert.ToByte(tempbtiem.Hour.ToString())).ToList();
                }
                else
                {
                    if (Convert.ToDateTime(tempbtiem.ToShortDateString()) ==
                        Convert.ToDateTime(endTime.ToShortDateString()))
                    {
                        soureList =
                            list.Where(
                                x =>
                                    x.STAT_DT >= Convert.ToDateTime(tempbtiem.ToShortDateString()) &&
                                    x.STAT_DT < Convert.ToDateTime(tempetiem.AddDays(1).ToShortDateString())).ToList();
                    }
                    else
                    {
                        soureList =
                            list.Where(
                                x =>
                                    x.STAT_DT >= Convert.ToDateTime(tempbtiem.ToShortDateString()) &&
                                    x.STAT_DT < Convert.ToDateTime(tempetiem.ToShortDateString())).ToList();
                    }
                }

                foreach (var item in typeList)
                {
                    var def = 0;
                    def = soureList.Where(m => m.QUALITY_CD == item.QUALITY_CD).Sum(m => m.AMOUNT);
                    lineTable2.Rows[i][item.QUALITY_NAM] = def;
                }
            }
            var dss2 = new DataSet();
            dss2.Tables.Add(lineTable2);
            ViewBag.ChartSplineXML = base.CreateMSSplineChart(title, dss2, 430, null, null, subtitle);
            #endregion
        }

        #endregion

        #region 纳税人行为分析

        /// <summary>
        /// 服务厅行为分析
        /// </summary>
        /// <param name="beginTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="export">是否导出</param>
        /// <returns></returns>

        [HttpGet]
        [UserAuth("STAT_TAXPAYER_BEHAV_STAT_D_VIW")] 
        public ActionResult TaxpayerAction(
            DateTime? beginTime,
            DateTime? endTime,
            bool export = false)
        {
            // 初始化日期
            base.DateTimeInit(ref beginTime, ref endTime);
            // 获取数据
            var bll = new Statistics_DAL();
             var result = bll.GetStatistics_TaxpayerActionChat(beginTime, endTime, UserState.UserID);
             if (export)
             {
                 var index = 0;
                 var data = result.Select(x => new
                 {
                     序号 = ++index,
                     营业厅编码 = x.HALL_NO,
                     营业厅名称 = x.ORG_NAM,
                     同城业务量 = x.LOCAL_CNT,
                     二次办税业务量 = x.SECOND_SVR_CNT
                 }).ToList().ToDataTable();
                 return AsposeExcelHelper.OutFileToRespone(data, "服务厅行为分析报表");
             }
             var chartData = result.Select(x => new
             {
                 营业厅名称 = x.ORG_NAM,
                 同城业务量 = x.LOCAL_CNT,
                 二次办税业务量 = x.SECOND_SVR_CNT,
             }).ToList().ToDataTable();

             // 输出图表
             var chart1 = CreateMSColumn3DChart("服务厅行为分析", chartData);
             chart1.SetSeriesName("同城业务量", "同城业务量", "8BBA00");
             chart1.SetSeriesName("二次办税业务量", "二次办税业务量", "F6BD0F"); 
             ViewBag.ChartColumn3DXML = chart1.ToString();

             var chart2 = CreateSplineChart("服务厅行为分析", chartData);
             chart2.SetSeriesName("同城业务量", "同城业务量", "8BBA00");
             chart2.SetSeriesName("二次办税业务量", "二次办税业务量", "F6BD0F"); 
             ViewBag.ChartSplineXML = chart2.ToString();
             return View(result); 
        }

        /// <summary>
        /// 员工行为分析
        /// </summary>
        /// <param name="beginTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="orgId">服务厅ID</param>
        /// <param name="export">是否导出</param>
        /// <returns></returns>
        [HttpGet]
        [UserAuth("STAT_TAXPAYER_BEHAV_STAT_D_VIW")] 
        public ActionResult TaxpayerAction_Person(
            DateTime? beginTime,
            DateTime? endTime,
            string orgId,
            bool export = false)
        {

            // 初始化日期
            base.DateTimeInit(ref beginTime, ref endTime);
            // 获取数据
            var bll = new Statistics_DAL();
            var result = bll.GetStatistics_TaxpayerActionChat_Person(beginTime, endTime, orgId);
            if (export)
            {
                var index = 0;
                var data = result.Select(x => new
                {
                    序号 = ++index,
                    工号 = x.PersonNo,
                    姓名 = x.PersonName,
                    同城业务量 = x.LOCAL_CNT,
                    二次办税业务量 = x.SECOND_SVR_CNT,
                }).ToList().ToDataTable();
                return AsposeExcelHelper.OutFileToRespone(data, "员工行为分析报表");
            }

            var chartData = result.Select(x => new
            {
                姓名 = x.PersonName,
                同城业务量 = x.LOCAL_CNT,
                二次办税业务量 = x.SECOND_SVR_CNT,
            }).ToList().ToDataTable();

            // 输出图表
            var chart1 = CreateMSColumn3DChart("员工行为分析报表", chartData);
            chart1.SetSeriesName("同城业务量", "同城业务量", "8BBA00");
            chart1.SetSeriesName("二次办税业务量", "二次办税业务量", "F6BD0F"); 
            ViewBag.ChartColumn3DXML = chart1.ToString();

            var chart2 = CreateSplineChart("员工行为分析报表", chartData);
            chart1.SetSeriesName("同城业务量", "同城业务量", "8BBA00");
            chart1.SetSeriesName("二次办税业务量", "二次办税业务量", "F6BD0F");  
            ViewBag.ChartSplineXML = chart2.ToString(); 
            return View(result); 
        }

        #endregion

        #region 纳税人评价分析

        /// <summary>
        /// 纳税人评价分析 - 子项
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [UserAuth("STAT_STAFF_BUSI_TOT_D_VIW")]
        public ActionResult TaxpayerEval_Node(
            long type,
            DateTime? beginTime,
            DateTime? endTime)
        {

            // 初始化日期
            base.DateTimeInit(ref beginTime, ref endTime);
            // 获取数据
            var bll = new Statistics_DAL();
            var columnName = "VERY_SATISFY_CNT";
            var columnDisplayName = "很满意量";
            switch (type)
            {
                case 2:
                    columnName = "SATISFY_CNT";
                    columnDisplayName = "满意量";
                    break;
                case 3:
                    columnName = "COMMON_CNT";
                    columnDisplayName = "一般量";
                    break;
                case 4:
                    columnName = "UNSATISFY_CNT";
                    columnDisplayName = "不满意量";
                    break;  
                case 5:
                    columnName = "NON_EVAL_CNT";
                    columnDisplayName = "未评价量";
                    break;
                case 6:
                    columnDisplayName = "满意度";
                    break;
            }

            IList<Statistics_KeyValueDto> result = new List<Statistics_KeyValueDto>();

            if (type == 6)
            {
                result = bll.GetStatistics_TaxpayerEvalChart_ManYiDu_Node(beginTime, endTime, UserState.UserID);
            }
            else
            {
                result = bll.GetStatistics_TaxpayerEvalChart_Node("VERY_SATISFY_CNT", beginTime, endTime, UserState.UserID);

            }
           
            var  chartData =new DataTable();
            if (beginTime == endTime)
            {
                // 处理小时间隔
                var resultList = new List<Statistics_KeyValueDto>(); 
                for (var i = 8; i <= 18;i++)
                {
                    var obj = result.FirstOrDefault(x => x.Name == i.ToString());
                    if (obj != null)
                    {
                        resultList.Add(new Statistics_KeyValueDto()
                        {
                            Name=i+ "时", 
                            Value = obj.Value 
                        });
                    }
                    else
                    {
                        resultList.Add(new Statistics_KeyValueDto()
                        {
                            Name = i + "时",
                            Value = "0"
                        });
                    } 
                }
                
                // 处理其他情况
                chartData = resultList.ToDataTable(); 
            }
            else
            {
                // 处理其他情况
                chartData = result.Select(x => new
                {
                    日期 = x.Name,
                    Value = x.Value
                }).ToList().ToDataTable(); 
            }
          

            // 输出图表
            var chart1 = CreateMSColumn3DChart("服务厅评价分析", chartData);
            chart1.SetSeriesName("Value", columnDisplayName, "8BBA00"); 
            ViewBag.ChartColumn3DXML = chart1.ToString();

            var chart2 = CreateSplineChart("服务厅评价分析", chartData);
            chart2.SetSeriesName("Value", columnDisplayName, "8BBA00");
            ViewBag.ChartSplineXML = chart2.ToString();

            var chart3 = CreatePie3DChart("服务厅评价分析", chartData);
            chart3.SetSeriesName("Value", columnDisplayName, "8BBA00");
            ViewBag.ChartPie3DXML = chart3.ToString();

            return View();
        }


        /// <summary>
        /// 纳税人评价分析 - 员工
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [UserAuth("STAT_STAFF_BUSI_TOT_D_VIW")]
        public ActionResult TaxpayerEval_Person(
            DateTime? beginTime,
            DateTime? endTime,
            string orgId, 
            bool export = false)
        {

            // 初始化日期
            base.DateTimeInit(ref beginTime, ref endTime);
            // 获取数据
            var bll = new Statistics_DAL();
            var result = bll.GetStatistics_TaxpayerEvalChart_Person(beginTime, endTime, orgId);
             if (export)
             {
                 var index = 0;
                 var data = result.Select(x => new
                 {
                     序号 = ++index,
                     工号 = x.PersonNo,
                     姓名 = x.PersonName,
                     很满意 = x.VERY_SATISFY_CNT,
                     很满意率 = x.VERY_SATISFY_CNT_BFB + "%",
                     满意 = x.SATISFY_CNT,
                     满意率 = x.SATISFY_CNT_BFB + "%",
                     一般 = x.COMMON_CNT,
                     一般率 = x.COMMON_CNT_BFB + "%",
                     不满意 = x.UNSATISFY_CNT,
                     不满意率 = x.UNSATISFY_CNT_BFB + "%",
                     未评价 = x.NON_EVAL_CNT,
                     未评价率 = x.NON_EVAL_CNT_BFB + "%",
                     满意度 = x.ManYiDu_BFB + "%"
                 }).ToList().ToDataTable();
                 return AsposeExcelHelper.OutFileToRespone(data, "员工评价分析报表");
             }

             var chartData = result.Select(x => new
             {
                 姓名 = x.PersonName,
                 很满意 = x.VERY_SATISFY_CNT,
                 满意 = x.SATISFY_CNT,
                 一般 = x.COMMON_CNT,
                 不满意 = x.UNSATISFY_CNT,
                 未评价 = x.NON_EVAL_CNT,
             }).ToList().ToDataTable();

             // 输出图表
             var chart1 = CreateMSColumn3DChart("员工评价分析报表", chartData);
             chart1.SetSeriesName("很满意", "很满意", "8BBA00");
             chart1.SetSeriesName("满意", "满意", "F6BD0F");
             chart1.SetSeriesName("一般", "一般", "33efaf");
             chart1.SetSeriesName("不满意", "不满意", "9ddc24");
             chart1.SetSeriesName("未评价", "未评价", "e648f0");
             ViewBag.ChartColumn3DXML = chart1.ToString();

             var chart2 = CreateSplineChart("员工评价分析报表", chartData);
             chart2.SetSeriesName("很满意", "很满意", "8BBA00");
             chart2.SetSeriesName("满意", "满意", "F6BD0F");
             chart2.SetSeriesName("一般", "一般", "33efaf");
             chart2.SetSeriesName("不满意", "不满意", "9ddc24");
             chart2.SetSeriesName("未评价", "未评价", "e648f0");
             ViewBag.ChartSplineXML = chart2.ToString();
             
             return View(result); 
        }

 
        /// <summary>
        /// 纳税人评价分析 - 营业厅
        /// </summary>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <param name="export"></param>
        /// <returns></returns>
        [HttpGet]
        [UserAuth("STAT_STAFF_BUSI_TOT_D_VIW")]
        public ActionResult TaxpayerEval(
            DateTime? beginTime,
            DateTime? endTime,
            bool export = false) 
        {  
            // 判断当前用户组织结构
            var power = new SYS_HALL_DAL().GetList(UserState.UserID, "4");
            if (power.Count == 1)
            {
                var hallNo = power.FirstOrDefault().HALL_NO;

                return Redirect(Url.Action("TaxpayerEval_Person", "Statistics",  new
                {
                    @beginTime = beginTime,
                    @endTime = endTime,
                    @orgId = hallNo
                }));
            }

            // 初始化日期
            base.DateTimeInit(ref beginTime, ref endTime);
             
             // 获取数据
            var bll = new Statistics_DAL();
            var result = bll.GetStatistics_TaxpayerEvalChart(beginTime, endTime, UserState.UserID);
            if (export)
            {
                var index = 0;
                var data = result.Select(x =>new 
                {
                    序号 = ++index,
                    营业厅编码 = x.HALL_NO,
                    营业厅名称 = x.ORG_NAM,
                    很满意 = x.VERY_SATISFY_CNT, 
                    很满意率 = x.VERY_SATISFY_CNT_BFB+"%",
                    满意 =x.SATISFY_CNT,
                    满意率 = x.SATISFY_CNT_BFB + "%",
                    一般 = x.COMMON_CNT,
                    一般率 = x.COMMON_CNT_BFB + "%",
                    不满意 =x.UNSATISFY_CNT,
                    不满意率 = x.UNSATISFY_CNT_BFB + "%",
                    未评价 = x.NON_EVAL_CNT,
                    未评价率 = x.NON_EVAL_CNT_BFB + "%",
                    满意度 = x.ManYiDu_BFB + "%"
                }).ToList().ToDataTable();
                return AsposeExcelHelper.OutFileToRespone(data, "服务厅评价分析报表"); 
            }

            var chartData = result.Select(x => new
            {
                营业厅名称 = x.ORG_NAM,
                很满意 = x.VERY_SATISFY_CNT,
                满意 = x.SATISFY_CNT,
                一般 = x.COMMON_CNT,
                不满意 = x.UNSATISFY_CNT,
                未评价 = x.NON_EVAL_CNT,
            }).ToList().ToDataTable();

            // 输出图表
            var chart1 = CreateMSColumn3DChart("纳税人评价分析", chartData);
            chart1.SetSeriesName("很满意", "很满意", "8BBA00");
            chart1.SetSeriesName("满意", "满意", "F6BD0F");
            chart1.SetSeriesName("一般", "一般", "33efaf");
            chart1.SetSeriesName("不满意", "不满意", "9ddc24");
            chart1.SetSeriesName("未评价", "未评价", "e648f0");
            ViewBag.ChartColumn3DXML = chart1.ToString();
             
            var chart2 = CreateSplineChart("纳税人评价分析", chartData);  
            chart2.SetSeriesName("很满意", "很满意", "8BBA00");
            chart2.SetSeriesName("满意", "满意", "F6BD0F");
            chart2.SetSeriesName("一般", "一般", "33efaf");
            chart2.SetSeriesName("不满意", "不满意", "9ddc24");
            chart2.SetSeriesName("未评价", "未评价", "e648f0"); 
            ViewBag.ChartSplineXML = chart2.ToString(); 
            return View(result);
        }

        #endregion

        #region Helper

        /// <summary>
        /// 3D饼图
        /// </summary>
        /// <returns></returns>
        protected FusionChartHelper CreatePie3DChart(string title, DataTable dt)
        {
            FusionChartHelper help = new FusionChartHelper(FusionChartType.Pie3D);
            help.SetDataSource(dt);
            help.ChartHeight = 550;
            help.ChartWidth = "100%";
            help.Caption = title;//主标题，将显示在图形顶端
            help.SubCaption = "";//二级标题，将显示在主标题下方，没有不显示
            help.BgColor = "ffffff";//背景色
            help.CanvasBorderColor = "ffffff";
            help.Decimals = 2;//显示小数位
            help.IsFormatNumberScale = false;//是否格式化数字，如1000显示成1K；
            return help;
        }

        /// <summary>
        /// 纳税人评价分析报表
        /// </summary>
        /// <returns></returns>
        private FusionChartHelper CreateMSColumn3DChart( 
            string title,
            DataTable dt)
        {
            FusionChartHelper help = new FusionChartHelper(FusionChartType.MSColumn3D);
            help.SetDataSource(dt);
            help.ChartHeight = 580;
            help.ChartWidth = "100%";
            help.Caption = title;//主标题，将显示在图形顶端
            help.SubCaption = "";//二级标题，将显示在主标题下方，没有不显示
            help.BgColor = "e1f5ff";//背景色
            help.Decimals = 2;//显示小数位
            help.IsFormatNumberScale = false;//是否格式化数字，如1000显示成1K；

           /* help.SetSeriesName("营业厅名称", "营业厅名称", "56B9F9");//多数据属性时使用*/
           
            return help;
        }

        private FusionChartHelper CreateSplineChart(string title, DataTable ds)
        {
            FusionChartHelper help = new FusionChartHelper(FusionChartType.MSSpline);
            help.SetDataSource(ds);
            help.ChartHeight = 550;
            help.ChartWidth = "100%";
            help.Caption = title;//主标题，将显示在图形顶端
            help.SubCaption = "";//二级标题，将显示在主标题下方，没有不显示
            help.BgColor = "e1f5ff";//背景色
            help.Decimals = 2;//显示小数位
            help.IsFormatNumberScale = false;//是否格式化数字，如1000显示成1K； 
            return help;
        } 
        
        #endregion
    }

    
}
