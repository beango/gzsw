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
        private IDao<CHK_QUALITY_CON> CHK_QUALITY_CON_Dao { get; set; }

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
            }

            base.DateTimeInit(ref beginTime, ref endTime);

            var mainTielt = GetOrgName(null, 2);
            ViewBag.MainTitle = GetTitleName(mainTielt, "业务差错分析", beginTime.GetValueOrDefault(),
                endTime.GetValueOrDefault());
            var subtitle = GetTitleName(mainTielt, "", beginTime.GetValueOrDefault(), endTime.GetValueOrDefault(), false);

            if (export)
            {
                return ExportData(subtitle, "服务厅", beginTime.GetValueOrDefault(), endTime.GetValueOrDefault(), null);
            }

            var data = new Statistics_DAL().GetStatistics_ServiceSlipAnalysis(beginTime, endTime, UserState.UserID);


            //var exceltitle = GetTitleName(mainTielt, "业务办理分析", beginTime.GetValueOrDefault(), endTime.GetValueOrDefault(), false);


            // 所有质量类型
            var typeList = CHK_QUALITY_CON_Dao.FindList();
            ViewData["typeList"] = typeList;

            // 所有事项大类
            var itemTypeList = SYS_DLSERIAL_Dao.FindList();
            ViewData["itemTypeList"] = itemTypeList;
            var list = data.GroupBy(m => m.HALL_NAM);

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
        [UserAuth("STAT_STAFF_QUALITY_STAT_D_VIW")]
        public ActionResult ServiceSlipAnalysis_Person(
            string orgId,
            DateTime? beginTime,
            DateTime? endTime,
            bool export = false
            )
        {
            ViewBag.HallNo = orgId;
            base.DateTimeInit(ref beginTime, ref endTime);

            var mainTielt = GetOrgName(orgId, null);
            ViewBag.MainTitle = GetTitleName(mainTielt, "业务差错分析", beginTime.GetValueOrDefault(),
                endTime.GetValueOrDefault());
            var subtitle = GetTitleName(mainTielt, "", beginTime.GetValueOrDefault(), endTime.GetValueOrDefault(), false);

            if (export)
            {
                return ExportData(subtitle, "员工名称", beginTime.GetValueOrDefault(), endTime.GetValueOrDefault(), orgId);
            }

            var listStatistics = new Statistics_DAL().GetStatistics_ServiceSlipAnalysisList(beginTime, endTime,
                UserState.UserID, orgId, null);


            //var exceltitle = GetTitleName(mainTielt, "业务办理分析", beginTime.GetValueOrDefault(), endTime.GetValueOrDefault(), false);


            var dic = listStatistics.GroupBy(m => m.STAFF_NAM);

            // 所有质量类型
            var typeList = CHK_QUALITY_CON_Dao.FindList();
            ViewData["typeList"] = typeList;

            // 所有事项大类
            var itemTypeList = SYS_DLSERIAL_Dao.FindList();
            ViewData["itemTypeList"] = itemTypeList;


            SetStaffChart(listStatistics, typeList, "员工业务差错分析", beginTime.GetValueOrDefault(),
                endTime.GetValueOrDefault(), subtitle);

            return View(dic);
        }

        /// <summary>
        /// 业务差错分析-项
        /// </summary>
        /// <param name="orgId">组织Id</param>
        /// <param name="titleName"></param>
        /// <param name="qId"></param>
        /// <param name="ssId"></param>
        /// <param name="staffId"></param>
        /// <param name="beginTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <returns></returns>
        public ActionResult ServiceSlipAnalysis_None(
            string orgId,
            string titleName,
            string qId,
            string ssId,
            string staffId,
            DateTime? beginTime,
            DateTime? endTime)
        {
            base.DateTimeInit(ref beginTime, ref endTime);

            var mainTielt = GetOrgName(orgId, null);
            ViewBag.MainTitle = GetTitleName(mainTielt, "业务差错分析", beginTime.GetValueOrDefault(),
                endTime.GetValueOrDefault());
            var subtitle = GetTitleName(mainTielt, "", beginTime.GetValueOrDefault(), endTime.GetValueOrDefault(), false);

            var listStatistics = new Statistics_DAL().GetStatistics_ServiceSlipAnalysisList(beginTime, endTime,
                UserState.UserID, orgId, staffId);

            if (!string.IsNullOrEmpty(staffId))
            {
                var item = listStatistics.FirstOrDefault();
                if (item != null)
                    subtitle = GetTitleName(item.STAFF_NAM, "", beginTime.GetValueOrDefault(),
                        endTime.GetValueOrDefault(), false);
            }

            if (string.IsNullOrEmpty(titleName))
            {
                titleName = "Name";
            }

            var lineTable = new DataTable();

            lineTable.Columns.Add("Y_NAME", typeof (string));
            lineTable.Columns.Add(titleName, typeof (int));

            beginTime = beginTime.GetValueOrDefault().AddHours(8);
            endTime = endTime.GetValueOrDefault().AddHours(18);

            TimeSpan timeSpan = endTime.GetValueOrDefault().Subtract(beginTime.GetValueOrDefault());
            var tlist = new List<string>();
            base.SetLineYName(timeSpan, beginTime.GetValueOrDefault(), lineTable, tlist, endTime.GetValueOrDefault());

            for (var i = 0; i < lineTable.Rows.Count; i++)
            {
                var tempbtiem = Convert.ToDateTime(tlist[i]);
                var tempetiem = i + 1 < lineTable.Rows.Count ? Convert.ToDateTime(tlist[i + 1]) : endTime;

                var soureList = new List<STAT_STAFF_QUALITY_STAT_D_SUB>();
                if (timeSpan.Days < 1)
                {
                    soureList = listStatistics.
                        Where(x => x.TIME_QUANTUM_CD == Convert.ToByte(tempbtiem.Hour.ToString())).ToList();
                }
                else
                {
                    if (Convert.ToDateTime(tempbtiem.ToShortDateString()) ==
                        Convert.ToDateTime(endTime.GetValueOrDefault().ToShortDateString()))
                    {
                        soureList =
                            listStatistics.Where(
                                x =>
                                    x.STAT_DT >= Convert.ToDateTime(tempbtiem.ToShortDateString()) &&
                                    x.STAT_DT <
                                    Convert.ToDateTime(tempetiem.GetValueOrDefault().AddDays(1).ToShortDateString()))
                                .ToList();
                    }
                    else
                    {
                        soureList =
                            listStatistics.Where(
                                x =>
                                    x.STAT_DT >= Convert.ToDateTime(tempbtiem.ToShortDateString()) &&
                                    x.STAT_DT < Convert.ToDateTime(tempetiem.GetValueOrDefault().ToShortDateString()))
                                .ToList();
                    }
                }

                if (string.IsNullOrEmpty(ssId))
                {
                    lineTable.Rows[i][titleName] = soureList.Where(m => m.QUALITY_CD == qId).Sum(m => m.AMOUNT);
                }
                else
                {
                    lineTable.Rows[i][titleName] =
                        soureList.Where(m => m.QUALITY_CD == qId && m.DLS_SERIALID == ssId).Sum(m => m.AMOUNT);
                }

            }

            var dss = new DataSet();
            dss.Tables.Add(lineTable);


            ViewBag.ChartColumn3DXML = CreateMSColumn3DChart(titleName, dss.Tables[0], 430, subtitle);
            ViewBag.ChartSplineXML = CreateMSSplineChart(titleName, dss, 430, null, null, subtitle);
            ViewBag.ChartPie3DXML = CreatePie3DChart(titleName, dss, 430, subtitle);

            return View();
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
            ViewBag.ChartColumn3DXML = base.CreateMSColumn3DChart(title, dss.Tables[0], 430, subtitle);

            #endregion

            #region 线形图

            var lineTable2 = new DataTable();
            lineTable2.Columns.Add("Y_Name", typeof (string));
            foreach (var item in typeList)
            {
                lineTable2.Columns.Add(item.QUALITY_NAM, typeof (int));
            }

            beginTime = beginTime.AddHours(8);
            endTime = endTime.AddHours(18);

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
            lineTable.Columns.Add("Y_Name", typeof (string));

            foreach (var item in typeList)
            {
                lineTable.Columns.Add(item.QUALITY_NAM, typeof (int));
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
            ViewBag.ChartColumn3DXML = base.CreateMSColumn3DChart(title, dss.Tables[0], 430, subtitle);

            #endregion

            #region 线形图

            var lineTable2 = new DataTable();
            lineTable2.Columns.Add("Y_Name", typeof (string));
            foreach (var item in typeList)
            {
                lineTable2.Columns.Add(item.QUALITY_NAM, typeof (int));
            }

            beginTime = beginTime.AddHours(8);
            endTime = endTime.AddHours(18);
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

        /// <summary>
        /// 导出数据
        /// </summary>
        /// <param name="title"></param>
        /// <param name="fileName"></param>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public ActionResult ExportData(string title,
            string Listname,
            DateTime beginTime,
            DateTime endTime,
            string orgId)
        {


            var typeList = CHK_QUALITY_CON_Dao.FindList();
            var itemTypeList = SYS_DLSERIAL_Dao.FindList();

            var lineTable = new DataTable();
            lineTable.Columns.Add("序号", typeof (string));
            lineTable.Columns.Add(Listname, typeof (string));
            lineTable.Columns.Add("事项大类", typeof (string));

            foreach (var item in typeList)
            {
                lineTable.Columns.Add(item.QUALITY_NAM, typeof (int));
            }


            if (Listname == "服务厅")
            {
                var data = new Statistics_DAL().GetStatistics_ServiceSlipAnalysis(beginTime, endTime, UserState.UserID);
                var dics = data.GroupBy(m => m.HALL_NAM);

                var i = 1;
                foreach (var dic in dics)
                {
                    var items = dic.ToList();
                    var statistics = items.FirstOrDefault();

                    for (var j = 1; j < itemTypeList.Count(); j++)
                    {
                        var r = lineTable.NewRow();
                        r["序号"] = i;
                        r[Listname] = statistics.HALL_NAM;
                        r["事项大类"] = itemTypeList[j].DLS_SERIALNAME;
                        foreach (var item in typeList)
                        {
                            var def =
                                items.FirstOrDefault(
                                    m =>
                                        m.QUALITY_CD == item.QUALITY_CD &&
                                        m.SSDLSERIALID == itemTypeList[j].DLS_SERIALID);
                            r[item.QUALITY_NAM] = def == null ? 0 : def.AMOUNT;
                        }
                        lineTable.Rows.Add(r);

                        i++;
                    }
                }
            }
            else
            {
                var listStatistics = new Statistics_DAL().GetStatistics_ServiceSlipAnalysisList(beginTime, endTime,
                    UserState.UserID, orgId, null);
                var dics = listStatistics.GroupBy(m => m.STAFF_NAM);
                var i = 1;
                foreach (var dic in dics)
                {
                    var items = dic.ToList();
                    var statistics = items.FirstOrDefault();

                    for (var j = 1; j < itemTypeList.Count(); j++)
                    {
                        var r = lineTable.NewRow();
                        r["序号"] = i;
                        r[Listname] = statistics.STAFF_NAM;
                        r["事项大类"] = itemTypeList[j].DLS_SERIALNAME;
                        foreach (var item in typeList)
                        {
                            var def =
                                items.FirstOrDefault(
                                    m =>
                                        m.QUALITY_CD == item.QUALITY_CD &&
                                        m.DLS_SERIALID == itemTypeList[j].DLS_SERIALID);
                            r[item.QUALITY_NAM] = def == null ? 0 : def.AMOUNT;
                        }
                        lineTable.Rows.Add(r);
                        i++;
                    }
                }
            }

            return AsposeExcelHelper.OutFileToRespone(lineTable, title);
        }

        #endregion

        #region 纳税人行为分析

        /// <summary>
        /// 获取纳税人行为数据
        /// </summary>
        /// <param name="beginTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="headType">类型</param>
        /// <param name="personNo">员工ID</param>
        /// <param name="columnType">业务类型</param>
        /// <returns></returns>
        [HttpGet]
        [UserAuth("STAT_TAXPAYER_BEHAV_STAT_D_VIW")]
        public ActionResult TaxpayerAction_Node(
            DateTime? beginTime,
            DateTime? endTime,
            long headType,
            string personNo,
            string columnType)
        {

            var columnDisplayName = string.Empty;
            // 标题 
            switch (headType)
            {
                case 1:

                    columnDisplayName = "同城业务量";
                    break;
                case 2:
                    columnDisplayName = "二次办税业务量";
                    break;

            }

            // 初始化日期
            base.DateTimeInit(ref beginTime, ref endTime);

            // 控制标题 
            var MainTitle = base.GetOrgName(null, 3);
            var nodeTitle = string.Empty;
            MainTitle += "纳税人评价分析";
            nodeTitle += "（" + beginTime.Value.ToString("yyyy年MM月dd日");
            nodeTitle += " - " + endTime.Value.ToString("yyyy年MM月dd日") + "）";
            MainTitle += "--" + columnDisplayName;

            ViewBag.MainTitle = MainTitle;
            ViewBag.NodeTitle = nodeTitle;


            IList<STAT_TAXPAYER_BEHAV_STAT_D> result = null;

            if (!string.IsNullOrEmpty(personNo))
            {
                result = new Statistics_DAL().GetStatistics_TaxpayerEvalChartByPerson_Node(beginTime, endTime, personNo);
            }
            else
            {
                result = new Statistics_DAL().GetStatistics_TaxpayerEvalChart_Node(beginTime, endTime, UserState.UserID);
            }



            var lineTable = new DataTable();

            lineTable.Columns.Add("Y_NAME", typeof (string));
            lineTable.Columns.Add(columnDisplayName, typeof (int));

            beginTime = beginTime.GetValueOrDefault().AddHours(8);
            endTime = endTime.GetValueOrDefault().AddHours(18);

            TimeSpan timeSpan = endTime.GetValueOrDefault().Subtract(beginTime.GetValueOrDefault());
            var tlist = new List<string>();
            base.SetLineYName(timeSpan, beginTime.GetValueOrDefault(), lineTable, tlist, endTime.GetValueOrDefault());

            for (var i = 0; i < lineTable.Rows.Count; i++)
            {
                var tempbtiem = Convert.ToDateTime(tlist[i]);
                var tempetiem = i + 1 < lineTable.Rows.Count ? Convert.ToDateTime(tlist[i + 1]) : endTime;

                var soureList = new List<STAT_TAXPAYER_BEHAV_STAT_D>();
                if (timeSpan.Days < 1)
                {
                    soureList = result.
                        Where(x => x.TIME_QUANTUM_CD == Convert.ToByte(tempbtiem.Hour.ToString())).ToList();
                }
                else
                {
                    if (Convert.ToDateTime(tempbtiem.ToShortDateString()) ==
                        Convert.ToDateTime(endTime.GetValueOrDefault().ToShortDateString()))
                    {
                        soureList =
                            result.Where(
                                x =>
                                    x.STAT_DT >= Convert.ToDateTime(tempbtiem.ToShortDateString()) &&
                                    x.STAT_DT <
                                    Convert.ToDateTime(tempetiem.GetValueOrDefault().AddDays(1).ToShortDateString()))
                                .ToList();
                    }
                    else
                    {
                        soureList =
                            result.Where(
                                x =>
                                    x.STAT_DT >= Convert.ToDateTime(tempbtiem.ToShortDateString()) &&
                                    x.STAT_DT < Convert.ToDateTime(tempetiem.GetValueOrDefault().ToShortDateString()))
                                .ToList();
                    }
                }


                if (string.IsNullOrEmpty(columnType))
                {
                    if (headType == 1)
                    {
                        lineTable.Rows[i][columnDisplayName] = soureList.Sum(m => m.LOCAL_CNT);
                    }
                    else if (headType == 2)
                    {
                        lineTable.Rows[i][columnDisplayName] = soureList.Sum(m => m.SECOND_SVR_CNT);
                    }
                }
                else
                {
                    if (headType == 1)
                    {
                        lineTable.Rows[i][columnDisplayName] =
                            soureList.Where(x => x.DLS_SERIALID == columnType).Sum(m => m.LOCAL_CNT);
                    }
                    else if (headType == 2)
                    {
                        lineTable.Rows[i][columnDisplayName] =
                            soureList.Where(x => x.DLS_SERIALID == columnType).Sum(m => m.SECOND_SVR_CNT);
                    }
                }

            }


            ViewBag.ChartColumn3DXML = CreateMSColumn3DChart(MainTitle, lineTable, 430, nodeTitle);
            ViewBag.ChartSplineXML = CreateMSSplineChart(MainTitle, lineTable, 430, null, null, nodeTitle);
            ViewBag.ChartPie3DXML = CreatePie3DChart(MainTitle, lineTable, 430, nodeTitle);

            return View();
        }



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
            // 控制标题 
            var MainTitle = base.GetOrgName(null, 3);
            var nodeTitle = string.Empty;
            MainTitle += "纳税人评价分析";
            nodeTitle += "（" + beginTime.Value.ToString("yyyy年MM月dd日");
            nodeTitle += " - " + endTime.Value.ToString("yyyy年MM月dd日") + "）";
            ViewBag.MainTitle = MainTitle;
            ViewBag.NodeTitle = nodeTitle;


            if (export)
            {
                var index = 0;
                var data = result.Select(x => new
                {
                    序号 = ++index,
                    服务厅编码 = x.HALL_NO,
                    服务厅名称 = x.HALL_NAM,
                    同城业务量 = x.LOCAL_CNT,
                    二次办税业务量 = x.SECOND_SVR_CNT
                }).ToList().ToDataTable();
                return AsposeExcelHelper.OutFileToRespone(data, MainTitle + "报表");
            }

            // 输出图表   
            SetChart(result.GroupBy(x => x.HALL_NO), MainTitle, nodeTitle);

            // 所有事项大类
            var itemTypeList = SYS_DLSERIAL_Dao.FindList();
            ViewData["itemTypeList"] = itemTypeList;

            return View(result);
        }

        /// <summary>
        /// 业务差错统计图形
        /// </summary>
        /// <param name="list"></param> 
        /// <param name="title"></param> 
        /// <param name="subtitle"></param>
        private void SetChart_Person(
            IEnumerable<IGrouping<string, gzsw.model.dto.Statistics_TaxpayerActionDto>> list,
            string title,
            string subtitle)
        {
            var lineTable = new DataTable();
            lineTable.Columns.Add("Y_Name", typeof (string));
            lineTable.Columns.Add("同城业务量", typeof (int));
            lineTable.Columns.Add("二次办税业务量", typeof (int));

            foreach (var item in list)
            {
                var items = item.ToList();
                var dto = items.FirstOrDefault();
                var r = lineTable.NewRow();
                r["Y_Name"] = dto.PersonName;
                var def = items.Sum(m => m.LOCAL_CNT);
                var eef = items.Sum(m => m.SECOND_SVR_CNT);
                r["同城业务量"] = def;
                r["二次办税业务量"] = eef;
                lineTable.Rows.Add(r);
            }

            ViewBag.ChartColumn3DXML = base.CreateMSColumn3DChart(title, lineTable, 430, subtitle);


            // 多线图
            ViewBag.ChartSplineXML = base.CreateMSSplineChart(title, lineTable, 430, null, null, subtitle);


        }

        /// <summary>
        /// 业务差错统计图形
        /// </summary>
        /// <param name="list"></param> 
        /// <param name="title"></param> 
        /// <param name="subtitle"></param>
        private void SetChart(
            IEnumerable<IGrouping<string, gzsw.model.dto.Statistics_TaxpayerActionDto>> list,
            string title,
            string subtitle)
        {
            var lineTable = new DataTable();
            lineTable.Columns.Add("Y_Name", typeof (string));
            lineTable.Columns.Add("同城业务量", typeof (int));
            lineTable.Columns.Add("二次办税业务量", typeof (int));

            foreach (var item in list)
            {
                var items = item.ToList();
                var dto = items.FirstOrDefault();
                var r = lineTable.NewRow();
                r["Y_Name"] = dto.HALL_NAM;
                var def = items.Sum(m => m.LOCAL_CNT);
                var eef = items.Sum(m => m.SECOND_SVR_CNT);
                r["同城业务量"] = def;
                r["二次办税业务量"] = eef;
                lineTable.Rows.Add(r);
            }

            ViewBag.ChartColumn3DXML = base.CreateMSColumn3DChart(title, lineTable, 430, subtitle);


            // 多线图
            ViewBag.ChartSplineXML = base.CreateMSSplineChart(title, lineTable, 430, null, null, subtitle);


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

            // 控制标题 
            var MainTitle = base.GetOrgName(null, 3);
            var nodeTitle = string.Empty;
            MainTitle += "纳税人评价分析";
            nodeTitle += "（" + beginTime.Value.ToString("yyyy年MM月dd日");
            nodeTitle += " - " + endTime.Value.ToString("yyyy年MM月dd日") + "）)";
            ViewBag.MainTitle = MainTitle;
            ViewBag.NodeTitle = nodeTitle;

            if (export)
            {
                var index = 0;
                var data = result.Select(x => new
                {
                    序号 = ++index,
                    工号 = x.PersonNo,
                    姓名 = x.PersonName,
                    业务大类 = x.DLS_SERIALNAME,
                    同城业务量 = x.LOCAL_CNT,
                    二次办税业务量 = x.SECOND_SVR_CNT
                }).ToList().ToDataTable();
                return AsposeExcelHelper.OutFileToRespone(data, MainTitle + "报表");
            }

            SetChart_Person(result.GroupBy(x => x.PersonNo), MainTitle, nodeTitle);

            // 所有事项大类
            var itemTypeList = SYS_DLSERIAL_Dao.FindList();
            ViewData["itemTypeList"] = itemTypeList;


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
            var columnDisplayName = "很满意量";
            // 标题 
            switch (type)
            {
                case 2:
                    columnDisplayName = "满意量";
                    break;
                case 3:
                    columnDisplayName = "基本满意量";
                    break;
                case 4:
                    columnDisplayName = "不满意量";
                    break;
                case 5:
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
                result = bll.GetStatistics_TaxpayerEvalChart_Node("VERY_SATISFY_CNT", beginTime, endTime,
                    UserState.UserID);

            }

            var chartData = new DataTable();
            if (beginTime == endTime)
            {
                // 处理小时间隔
                var resultList = new List<Statistics_KeyValueDto>();
                for (var i = 8; i <= 18; i++)
                {
                    var obj = result.FirstOrDefault(x => x.Name == i.ToString());
                    if (obj != null)
                    {
                        resultList.Add(new Statistics_KeyValueDto()
                        {
                            Name = i + "时",
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

                switch (type)
                {
                    case 1: 
                         chartData = result.Select(x => new
                        {
                            日期 = x.Name,
                            很满意 = x.Value
                        }).ToList().ToDataTable(); 
                        break;
                    case 2:
                        chartData = result.Select(x => new
                        {
                            日期 = x.Name,
                            满意量 = x.Value
                        }).ToList().ToDataTable(); 
                        break;
                    case 3:
                        chartData = result.Select(x => new
                        {
                            日期 = x.Name,
                            基本满意量 = x.Value
                        }).ToList().ToDataTable();  
                        break;
                    case 4:
                        chartData = result.Select(x => new
                        {
                            日期 = x.Name,
                            不满意量 = x.Value
                        }).ToList().ToDataTable();   
                        break;
                    case 5:
                        chartData = result.Select(x => new
                        {
                            日期 = x.Name,
                            未评价量 = x.Value
                        }).ToList().ToDataTable();    
                        break;
                    case 6:
                        chartData = result.Select(x => new
                        {
                            日期 = x.Name,
                            满意度 = x.Value
                        }).ToList().ToDataTable();     
                        break;
                }  
            }
            var title = "服务厅评价分析--" + columnDisplayName;
            var nodeTitle = base.GetOrgName(null, 1);
            nodeTitle += "（" + beginTime.Value.ToString("yyyy年MM月dd日");
            nodeTitle += " - " + endTime.Value.ToString("yyyy年MM月dd日") + "）";
            // 输出图表
            ViewBag.ChartColumn3DXML = CreateMSColumn3DChart(title, chartData, 430, nodeTitle);

            ViewBag.ChartSplineXML = CreateMSSplineChart(title, chartData, 430, null, null, nodeTitle);

            ViewBag.ChartPie3DXML = CreatePie3DChart(title, chartData, 430, nodeTitle);

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


            // 控制标题 
            const string titleName = "纳税人评价分析";
            var mainTielt = GetOrgName(orgId, null);
            ViewBag.MainTitle = GetTitleName(mainTielt, titleName, beginTime.GetValueOrDefault(),
                endTime.GetValueOrDefault());
            var subtitle = GetTitleName(mainTielt, "", beginTime.GetValueOrDefault(), endTime.GetValueOrDefault(), false);
            var exceltitle = GetTitleName(mainTielt, titleName, beginTime.GetValueOrDefault(),
                endTime.GetValueOrDefault(), false);

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
                    基本满意 = x.COMMON_CNT,
                    基本满意率 = x.COMMON_CNT_BFB + "%",
                    不满意 = x.UNSATISFY_CNT,
                    不满意率 = x.UNSATISFY_CNT_BFB + "%",
                    未评价 = x.NON_EVAL_CNT,
                    未评价率 = x.NON_EVAL_CNT_BFB + "%",
                    满意度 = x.ManYiDu_BFB + "%"
                }).ToList().ToDataTable();
                return AsposeExcelHelper.OutFileToRespone(data, "纳税人评价分析报表-员工", exceltitle);
            }

            var chartData = result.Select(x => new
            {
                姓名 = x.PersonName,
                很满意 = x.VERY_SATISFY_CNT,
                满意 = x.SATISFY_CNT,
                基本满意 = x.COMMON_CNT,
                不满意 = x.UNSATISFY_CNT,
                未评价 = x.NON_EVAL_CNT,
            }).ToList().ToDataTable();

            // 输出图表
            ViewBag.ChartColumn3DXML = CreateMSColumn3DChart(titleName, chartData, 430, subtitle);


            ViewBag.ChartSplineXML = CreateMSSplineChart(titleName, chartData, 420, null, null, subtitle);


            return View(result);
        }


        /// <summary>
        /// 纳税人评价分析 - 服务厅
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

                return Redirect(Url.Action("TaxpayerEval_Person", "Statistics", new
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


            // 控制标题 
            const string titleName = "纳税人评价分析";
            var mainTielt = base.GetOrgName(null, 3);
            ViewBag.MainTitle = GetTitleName(mainTielt, titleName, beginTime.GetValueOrDefault(),
                endTime.GetValueOrDefault());
            var subtitle = GetTitleName(mainTielt, "", beginTime.GetValueOrDefault(), endTime.GetValueOrDefault(), false);
            var exceltitle = GetTitleName(mainTielt, titleName, beginTime.GetValueOrDefault(),
                endTime.GetValueOrDefault(), false);

            if (export)
            {
                var index = 0;
                var data = result.Select(x => new
                {
                    序号 = ++index,
                    服务厅编码 = x.HALL_NO,
                    服务厅名称 = x.ORG_NAM,
                    很满意 = x.VERY_SATISFY_CNT,
                    很满意率 = x.VERY_SATISFY_CNT_BFB + "%",
                    满意 = x.SATISFY_CNT,
                    满意率 = x.SATISFY_CNT_BFB + "%",
                    基本满意 = x.COMMON_CNT,
                    基本满意率 = x.COMMON_CNT_BFB + "%",
                    不满意 = x.UNSATISFY_CNT,
                    不满意率 = x.UNSATISFY_CNT_BFB + "%",
                    未评价 = x.NON_EVAL_CNT,
                    未评价率 = x.NON_EVAL_CNT_BFB + "%",
                    满意度 = x.ManYiDu_BFB + "%"
                }).ToList().ToDataTable();
                return AsposeExcelHelper.OutFileToRespone(data, "纳税人评价分析报表-服务厅", exceltitle);
            }

            var chartData = result.Select(x => new
            {
                服务厅名称 = x.ORG_NAM,
                很满意 = x.VERY_SATISFY_CNT,
                满意 = x.SATISFY_CNT,
                基本满意 = x.COMMON_CNT,
                不满意 = x.UNSATISFY_CNT,
                未评价 = x.NON_EVAL_CNT,
            }).ToList().ToDataTable();


            // 输出图表
            ViewBag.ChartColumn3DXML = CreateMSColumn3DChart(titleName, chartData, 430, subtitle);

            ViewBag.ChartSplineXML = CreateMSSplineChart(titleName, chartData, 430, null, null, subtitle);

            return View(result);
        }

        #endregion
    }
}
