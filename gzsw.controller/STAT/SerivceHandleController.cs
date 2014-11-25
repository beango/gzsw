using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.UI;
using gzsw.controller.MyAuth;
using gzsw.dal.dao;
using gzsw.model;
using gzsw.model.Enums;
using gzsw.model.Subclasses;
using gzsw.util;
using NPOI.SS.Formula.Functions;
using NPOI.SS.UserModel;
using PetaPoco;
using gzsw.util.Extensions;
using gzsw.util.Office;
using NPOI.HSSF.UserModel;
using NPOI.SS.Util;

namespace gzsw.controller.STAT
{
    /// <summary>
    /// 业务办理分析
    /// </summary>
    public class SerivceHandleController : StatController
    {
        /// <summary>
        /// 业务办理分析
        /// </summary>
        /// <param name="orgId"></param>
        /// <param name="endTime"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="field"></param>
        /// <param name="orderStr"></param>
        /// <param name="viewName"></param>
        /// <param name="beginTime"></param>
        /// <returns></returns>
        [UserAuth("STAT_STAFF_LARGE_BUSI_D_VIW")]
        public ActionResult Index(string orgId, DateTime? beginTime,DateTime? endTime, 
            int pageIndex = 1, int pageSize = 20,string field = "HALL_NAM", string orderStr = "ASC", 
            string viewName = "HallStat")
        {
            base.DateTimeInit(ref beginTime, ref endTime);

            switch (GetHighLV)
            {
                case UserLV_ENUM.省级:
                case UserLV_ENUM.市级:
                case UserLV_ENUM.区级:
                break;
                case UserLV_ENUM.服务厅级:
                    var list = SYS_HALL_DAL.GetListByUserId(UserState.UserID);
                    if (list.Count == 1 && string.IsNullOrEmpty(orgId))
                    {
                        orgId = list[0].HALL_NO;
                        viewName = "StaffStat";
                    }
                break;
            }


            ViewBag.ViewName = viewName;
            //因为现在只做到服务厅级别
            //return RedirectToAction("HallStat");
            var model =new
            {
                orgId=orgId,
                beginTime = beginTime,
                endTime = endTime,
                pageIndex = pageIndex,
                pageSize = pageSize,
                field = field,
                orderStr = orderStr,
                viewName = viewName
            };
            return View(model);
        }

        /// <summary>
        /// 服务厅级别的统计
        /// </summary>
        /// <param name="endTime"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="field"></param>
        /// <param name="orderStr"></param>
        /// <param name="beginTime"></param>
        /// <returns></returns>
        [UserAuth("STAT_STAFF_LARGE_BUSI_D_VIW")]
        public ActionResult HallStat(DateTime? beginTime, DateTime? endTime,
            int pageIndex = 1, int pageSize = 20, string field = "HALL_NAM", string orderStr = "ASC", bool export = false)
        {
            base.DateTimeInit(ref beginTime, ref endTime);

            var mainTielt= GetOrgName(null,2);
            ViewBag.MainTitle = GetTitleName(mainTielt, "业务办理分析", beginTime.GetValueOrDefault(), endTime.GetValueOrDefault());
            var subtitle = GetTitleName(mainTielt, "", beginTime.GetValueOrDefault(), endTime.GetValueOrDefault(), false);
            var exceltitle = GetTitleName(mainTielt, "业务办理分析", beginTime.GetValueOrDefault(), endTime.GetValueOrDefault(), false);

            //格表
            var list = STAT_STAFF_LARGE_BUSI_D_DAL.GetHallStatList(UserState.UserID, beginTime, endTime,null);
            var dic = list.GroupBy(m => m.HALL_NAM).OrderBy(m => m.Key).ToList();
            var plist = new Page<IGrouping<string, STAT_STAFF_LARGE_BUSI_D_Handle_SUB>>
                        {
                            Items = dic,
                            CurrentPage = pageIndex,
                            TotalItems = dic.Count,
                            ItemsPerPage = pageSize
                        };

            if (export)
            {
                var index = 0;
                return ExportData(exceltitle,
                    "业务办理分析报表-服务厅", "服务厅", dic);
            }

            SetChart(null, beginTime, endTime, 1, subtitle);

            ViewBag.ViewName = "HallStat";
            return View(plist);
        }

        /// <summary>
        /// 员工级别 业务办理分析
        /// </summary>
        /// <param name="orgId">服务编码</param>
        /// <param name="endTime"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="field"></param>
        /// <param name="orderStr"></param>
        /// <param name="beginTime"></param>
        /// <returns></returns>
        [UserAuth("STAT_STAFF_LARGE_BUSI_D_VIW")]
        public ActionResult StaffStat(string orgId,DateTime? beginTime, DateTime? endTime,
            int pageIndex = 1, int pageSize = 20, string field = "STAT_DT", string orderStr = "ASC", bool export = false)
        {
            base.DateTimeInit(ref beginTime, ref endTime);

            var mainTielt = GetOrgName(orgId, null);
            ViewBag.MainTitle = GetTitleName(mainTielt,"业务办理分析", beginTime.GetValueOrDefault(), endTime.GetValueOrDefault());
            var subtitle = GetTitleName(mainTielt,"", beginTime.GetValueOrDefault(), endTime.GetValueOrDefault(), false);
            var exceltitle = GetTitleName(mainTielt,"业务办理分析", beginTime.GetValueOrDefault(), endTime.GetValueOrDefault(), false);

            var list = STAT_STAFF_LARGE_BUSI_D_DAL.GetStatList(orgId, beginTime, endTime, null);

            var dic = list.GroupBy(m => m.STAFF_NAM).OrderBy(m=>m.Key).ToList();
            var plist = new Page<IGrouping<string, STAT_STAFF_LARGE_BUSI_D_Handle_SUB>>
            {
                Items = dic,
                CurrentPage = pageIndex,
                TotalItems = dic.Count,
                ItemsPerPage = pageSize
            };

            ViewBag.HallNo = orgId;

            if (export)
            {
                var index = 0;
                return ExportData(exceltitle,
                    "业务办理分析报表-员工", "员工名称", dic);
            }

            SetChart(orgId, beginTime, endTime, 2, subtitle);

            ViewBag.ViewName = "StaffStat";
            return View(plist);
        }


        /// <summary>
        /// 设置图形报表
        /// </summary>
        /// <param name="orgId"></param>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <param name="subtitle"></param>
        public void SetChart(string orgId, DateTime? beginTime, DateTime? endTime, int type = 1, string subtitle="")
        {
            var list = STAT_STAFF_LARGE_BUSI_D_DAL.GetStatList(orgId, UserState.UserID, beginTime, endTime);
            string titleName = "排队业务分析";
            var lineTable = new DataTable();
            lineTable.Columns.Add("Y_Name", typeof(string));
            lineTable.Columns.Add("业务笔数", typeof(int));
            lineTable.Columns.Add("业务折合量", typeof(int));
            lineTable.Columns.Add("超时办理笔数", typeof(int));
            lineTable.Columns.Add("同城业务笔数", typeof(int));


            var grou = new  List<IGrouping<string, STAT_STAFF_LARGE_BUSI_D_Handle_SUB>>();
            grou = type == 1 ? list.GroupBy(m => m.HALL_NAM).ToList() : list.GroupBy(m => m.STAFF_NAM).ToList();

            foreach (var item in grou)
            {
                var list2 = item.ToList();

                var r = lineTable.NewRow();
                r["Y_Name"] = item.Key;
                r["业务笔数"] = list2.Sum(m => m.BUSI_CNT);
                r["业务折合量"] = list2.Sum(m => m.CONVERT_BUSI_CNT);
                r["超时办理笔数"] = list2.Sum(m => m.OVERTIME_HANDLE_CNT);
                r["同城业务笔数"] = list2.Sum(m => m.LOCAL_CNT);
                lineTable.Rows.Add(r);
            }

            var dss = new DataSet();
            dss.Tables.Add(lineTable);


            ViewBag.ChartColumn3DXML = CreateMSColumn3DChart(titleName, lineTable, 430, subtitle);
            //ViewBag.ChartSplineXML = CreateMSSplineChart(titleName, SetLin(list, beginTime.GetValueOrDefault(), endTime.GetValueOrDefault()), 430,null,null, subtitle);
            ViewBag.ChartSplineXML = CreateMSSplineChart(titleName, dss, 430, null, null, subtitle);
            ViewBag.ChartPie3DXML = CreatePie3DChart(titleName, dss, 430, subtitle);
        }

        /// <summary>
        /// 导出报表
        /// </summary>
        /// <param name="title"></param>
        /// <param name="fileName"></param>
        /// <param name="Listname"></param>
        /// <param name="dic"></param>
        /// <returns></returns>
        public ActionResult ExportData(string title, string fileName, string Listname,
            List<IGrouping<string, STAT_STAFF_LARGE_BUSI_D_Handle_SUB>> dic)
        {
            #region 加载模板文件到工作簿对象中

            HSSFWorkbook hssfworkbook;
            using (FileStream file = new FileStream(Server.MapPath("/Areas/STAT/Views/STATTemp/业务办理分析.xls"), FileMode.Open, FileAccess.Read))
            {
                hssfworkbook = new HSSFWorkbook(file);
            }

            #endregion
            #region 根据模板设置工作表的内容

            ISheet sheet1 = hssfworkbook.GetSheet("业务办理分析");
            int rowIndex = 2;
            sheet1.GetRow(0).Cells[0].SetCellValue(title);
            sheet1.GetRow(1).Cells[1].SetCellValue(Listname);
            if (dic != null && dic.Count > 0)
            {
                #region 数据单元格样式

                var cellFont = hssfworkbook.CreateFont();
                var cellStyle = hssfworkbook.CreateCellStyle();

                cellStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.White.Index;
                cellStyle.FillPattern = NPOI.SS.UserModel.FillPattern.SolidForeground;

                #endregion

                var i = 0;
                var beginRowIndex = rowIndex;

                foreach (var stat in dic)
                {
                    ++i;
                    var index = i;
                    var hallname = stat.Key;
                    var templist = stat.ToList();

                    var busiTotal = templist.Sum(m => m.BUSI_CNT);
                    var convertTotal = templist.Sum(m => m.CONVERT_BUSI_CNT);
                    var handleTotal = templist.Sum(m => m.HANDLE_DUR);
                    var overTimeTotal = templist.Sum(m => m.OVERTIME_HANDLE_CNT);
                    var localTotal = templist.Sum(m => m.LOCAL_CNT);

                    beginRowIndex = rowIndex;

                    foreach (var item in templist)
                    {
                        var xlsrow = sheet1.CreateRow(rowIndex);
                        if (index == i)
                        {
                            xlsrow.CreateCell(0).SetCellValue(i);
                            xlsrow.CreateCell(1).SetCellValue(stat.Key);
                        }
                        else
                        {
                            xlsrow.CreateCell(0);
                            xlsrow.CreateCell(1);
                        }
                        xlsrow.CreateCell(2).SetCellValue(item.DLS_SERIALNAME);
                        xlsrow.CreateCell(3).SetCellValue(item.BUSI_CNT);
                        xlsrow.CreateCell(4).SetCellValue(item.CONVERT_BUSI_CNT.ToString("####"));
                        xlsrow.CreateCell(5).SetCellValue(item.AverageHANDLE.ToTimeString());
                        xlsrow.CreateCell(6).SetCellValue(item.OVERTIME_HANDLE_CNT);
                        xlsrow.CreateCell(7).SetCellValue(item.TimeoutRate.ToString("P"));
                        xlsrow.CreateCell(8).SetCellValue(item.LOCAL_CNT);
                        xlsrow.CreateCell(9).SetCellValue(item.CityRate.ToString("P"));

                        rowIndex++;
                        index++;

                        #region 设置单元格样式

                        SetCellStype(xlsrow, cellStyle);
                        #endregion
                    }
                    var totalrow = sheet1.CreateRow(rowIndex);

                    totalrow.CreateCell(0);
                    totalrow.CreateCell(1);
                    totalrow.CreateCell(2).SetCellValue("合计");
                    totalrow.CreateCell(3).SetCellValue(busiTotal);
                    totalrow.CreateCell(4).SetCellValue(convertTotal.ToString("####"));
                    totalrow.CreateCell(5).SetCellValue((busiTotal == 0 ? 0 : handleTotal / busiTotal).ToTimeString());
                    totalrow.CreateCell(6).SetCellValue(overTimeTotal);
                    totalrow.CreateCell(7).SetCellValue((busiTotal == 0 ? 0 : (decimal)overTimeTotal / busiTotal).ToString("P"));
                    totalrow.CreateCell(8).SetCellValue(localTotal);
                    totalrow.CreateCell(9).SetCellValue((busiTotal == 0 ? 0 : (decimal)localTotal / (decimal)busiTotal).ToString("P"));

                    var cellRangeAddress = new CellRangeAddress(beginRowIndex, beginRowIndex + templist.Count, 0, 0);
                    sheet1.AddMergedRegion(cellRangeAddress);
                    var cellRangeAddress2 = new CellRangeAddress(beginRowIndex, beginRowIndex + templist.Count, 1, 1);
                    sheet1.AddMergedRegion(cellRangeAddress2);
                    beginRowIndex++;
                    

                    SetCellStype(sheet1.GetRow(rowIndex), cellStyle);

                    var footercellFont = hssfworkbook.CreateFont();
                    var footercellStyle = hssfworkbook.CreateCellStyle();

                    footercellFont.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;
                    footercellStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.Yellow.Index;
                    footercellStyle.FillPattern = NPOI.SS.UserModel.FillPattern.SolidForeground;
                    var HSSFPalette = hssfworkbook.GetCustomPalette();
                    Color c = ToColor("#fffdbb");
                    HSSFPalette.SetColorAtIndex(NPOI.HSSF.Util.HSSFColor.Yellow.Index, c.R, c.G, c.B);
                    SetCellStype(totalrow, footercellStyle);

                    rowIndex++;
                }


            }
            //强制Excel重新计算表中所有的公式
            sheet1.ForceFormulaRecalculation = true;

            #endregion

            #region 写入到客户端

            var ms = new MemoryStream();
            //将工作簿的内容放到内存流中
            hssfworkbook.Write(ms);
            ms.Seek(0, SeekOrigin.Begin);
            return File(ms, "application/vnd.ms-excel", fileName + ".xls");

            #endregion
        }

        private void SetCellStype(IRow xlsrow, ICellStyle cellStyle)
        {               
                #region 设置单元格样式
                foreach (var cell in xlsrow.Cells)
                {
                    cell.CellStyle = cellStyle;
                    cell.CellStyle.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Center;
                    cell.CellStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                    cell.CellStyle.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                    cell.CellStyle.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                    cell.CellStyle.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                    cell.CellStyle.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                }
                #endregion
        }

        /// <summary>
        /// 公共图标统计方法
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="title"></param>
        /// <param name="orgId"></param>
        /// <param name="serialId"></param>
        /// <param name="staffId"></param>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        //[UserAuth("STAT_STAFF_LARGE_BUSI_D_VIW")]
        public ActionResult ShowCNT(string type, string title, string orgId,string serialId,
            string staffId, DateTime? beginTime, DateTime? endTime)
        {
            base.DateTimeInit(ref beginTime, ref endTime);
            var list  = STAT_STAFF_LARGE_BUSI_D_DAL.GetDataStatList(orgId, UserState.UserID,serialId,staffId, beginTime, endTime);

            var StaffName = string.Empty;
            if (!string.IsNullOrEmpty(staffId))
            {
                var item = list.FirstOrDefault();
                if (item != null)
                    StaffName = item.STAFF_NAM;
            }

            SetShowChart(list, type, beginTime.GetValueOrDefault(), endTime.GetValueOrDefault(), title, orgId, StaffName);
            return View();
        }

        /// <summary>
        /// 子类的图
        /// </summary>
        /// <param name="list"></param>
        /// <param name="ct"></param>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        public void SetShowChart(List<STAT_STAFF_LARGE_BUSI_D_Handle_SUB> list,string ct,
            DateTime beginTime, DateTime endTime, string title, string orgId, string staffName=null)
        {
            

            string titleName;
            switch (ct)
            {
                case "BUSI_CNT":
                    titleName = "业务笔数";
                    break;
                case "CONVERT_BUSI_CNT":
                    titleName = "业务折合量";
                    break;
                case "AverageHANDLE":
                    titleName = "平均办理时间";
                    break;
                case "OVERTIME_HANDLE_CNT":
                    titleName = "超时办理笔数";
                    break;
                case "LOCAL_CNT":
                    titleName = "同城业务笔数";
                    break;
                default:
                    titleName = "业务笔数";
                    break;
            }

            var mainTielt = GetOrgName(orgId, null);
            ViewBag.MainTitle = GetTitleName(mainTielt, titleName, beginTime, endTime);
            string subtitle = string.Empty;
            if (!string.IsNullOrEmpty(staffName))
            {
                subtitle = GetTitleName(staffName, "", beginTime, endTime, false);
            }
            else
            {
                subtitle = GetTitleName(mainTielt, "", beginTime, endTime, false);
            }

            var lineTable = new DataTable();

            lineTable.Columns.Add("Y_NAME", typeof(string));
            if (ct == "AverageHANDLE")
            {
                lineTable.Columns.Add(titleName, typeof (decimal));
            }
            else
            {
                lineTable.Columns.Add(titleName, typeof(int));
            }


            beginTime = beginTime.AddHours(8);
            endTime = endTime.AddHours(18);

            TimeSpan timeSpan = endTime.Subtract(beginTime);
            var tlist = new List<string>();
            base.SetLineYName(timeSpan, beginTime, lineTable, tlist, endTime);

            for (var i = 0; i < lineTable.Rows.Count; i++)
            {
                var tempbtiem = Convert.ToDateTime(tlist[i]);
                var tempetiem = i + 1 < lineTable.Rows.Count ? Convert.ToDateTime(tlist[i + 1]) : endTime;

                var soureList = new List<STAT_STAFF_LARGE_BUSI_D_Handle_SUB>();
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
                switch (ct)
                {
                    case "BUSI_CNT":
                        lineTable.Rows[i][titleName] = soureList.Sum(m => m.BUSI_CNT);
                        break;
                    case "CONVERT_BUSI_CNT":
                        lineTable.Rows[i][titleName] = soureList.Sum(m => m.CONVERT_BUSI_CNT);
                        break;
                    case "AverageHANDLE":
                        var soure = soureList.Sum(m => m.HANDLE_DUR);
                        var busi = soureList.Sum(m => m.BUSI_CNT);
                        var value = busi == 0 ? 0 : (decimal)soure / (decimal)busi;
                        lineTable.Rows[i][titleName] = value;
                        break;
                    case "OVERTIME_HANDLE_CNT":
                        lineTable.Rows[i][titleName] = soureList.Sum(m => m.OVERTIME_HANDLE_CNT);
                        break;
                    case "LOCAL_CNT":
                        lineTable.Rows[i][titleName] = soureList.Sum(m => m.LOCAL_CNT);
                        break;
                    default:
                        lineTable.Rows[i][titleName] = soureList.Sum(m => m.BUSI_CNT);
                        break;
                }
            }

            var dss = new DataSet();
            dss.Tables.Add(lineTable);

            if (ct == "AverageHANDLE")
            {
                ViewBag.ChartColumn3DXML = CreateColumn3DChart(titleName, lineTable, subtitle, true, "分", "60", "分");
                ViewBag.ChartSplineXML = CreateMSSplineChart(titleName, lineTable, 550, null, null, subtitle, true, "分", "60", "分");
                ViewBag.ChartPie3DXML = CreatePie3DChart(title, dss, 430, subtitle);
            }
            else
            {
                ViewBag.ChartColumn3DXML = CreateMSColumn3DChart(title, dss.Tables[0], 430, subtitle);
                ViewBag.ChartSplineXML = CreateMSSplineChart(title, dss, 430, null,null,subtitle);
                ViewBag.ChartPie3DXML = CreatePie3DChart(title, dss, 430, subtitle);
            }
        }

        public DataSet SetLin(List<STAT_STAFF_LARGE_BUSI_D_Handle_SUB> list, 
            DateTime beginTime, DateTime endTime)
        {
            var lineTable = new DataTable();

            lineTable.Columns.Add("Y_NAME", typeof(string));
            lineTable.Columns.Add("业务笔数", typeof(int));
            lineTable.Columns.Add("业务折合量", typeof(int));
            lineTable.Columns.Add("超时办理笔数", typeof(int));
            lineTable.Columns.Add("同城业务笔数", typeof(int));

            beginTime = beginTime.AddHours(8);
            endTime = endTime.AddHours(18);

            TimeSpan timeSpan = endTime.Subtract(beginTime);
            var tlist = new List<string>();
            base.SetLineYName(timeSpan, beginTime, lineTable, tlist, endTime);

            for (var i = 0; i < lineTable.Rows.Count; i++)
            {
                var tempbtiem = Convert.ToDateTime(tlist[i]);
                var tempetiem = i + 1 < lineTable.Rows.Count ? Convert.ToDateTime(tlist[i + 1]) : endTime;

                var soureList = new List<STAT_STAFF_LARGE_BUSI_D_Handle_SUB>();
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

                lineTable.Rows[i]["业务笔数"] = soureList.Sum(m => m.BUSI_CNT);
                lineTable.Rows[i]["业务折合量"] = soureList.Sum(m => m.CONVERT_BUSI_CNT);
                lineTable.Rows[i]["超时办理笔数"] = soureList.Sum(m => m.OVERTIME_HANDLE_CNT);
                lineTable.Rows[i]["同城业务笔数"] = soureList.Sum(m => m.LOCAL_CNT);
            }

            var dss = new DataSet();
            dss.Tables.Add(lineTable);
            return dss;
        }
    }
}
