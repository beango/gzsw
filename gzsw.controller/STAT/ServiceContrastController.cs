using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Web.Mvc;
using gzsw.controller.MyAuth;
using gzsw.dal;
using gzsw.dal.dao;
using gzsw.model;
using gzsw.model.Enums;
using gzsw.model.Subclasses;
using gzsw.util.Extensions;
using Ninject;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using PetaPoco;

namespace gzsw.controller.STAT
{
    /// <summary>
    /// 业务量对比分析
    /// </summary>
    public class ServiceContrastController:StatController
    {
        [Inject]
        public IDao<SYS_DLSERIAL>  dlDal { get; set; }

        [UserAuth("STAT_SERVICEComparisonAnalysis")]
        public ActionResult Index()
        {
            switch (GetHighLV)
            {
                case UserLV_ENUM.省级:
                case UserLV_ENUM.市级:
                case UserLV_ENUM.区级:
                    break;
                case UserLV_ENUM.服务厅级:
                    var list = SYS_HALL_DAL.GetListByUserId(UserState.UserID);
                    if (list.Count == 1)
                    {
                        return RedirectToAction("StaffStat", new { orgId = list[0].HALL_NO });
                    }
                    break;
            }

            return RedirectToAction("HallStat");
        }

        /// <summary>
        /// 服务厅 业务量对比分析
        /// </summary>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [UserAuth("STAT_SERVICEComparisonAnalysis")]
        public ActionResult HallStat(DateTime? beginTime, DateTime? endTime,
            int pageIndex = 1, int pageSize = 20, bool export = false)
        {
            base.DateTimeInit(ref beginTime, ref endTime);

            var mainTielt = GetOrgName(null, 2);
            ViewBag.MainTitle = GetTitleName(mainTielt,"业务量对比分析", beginTime.GetValueOrDefault(), endTime.GetValueOrDefault());
            var title = GetTitleName(mainTielt, "", beginTime.GetValueOrDefault(), endTime.GetValueOrDefault(), false);
            var exceltitle = GetTitleName(mainTielt, "业务办理分析", beginTime.GetValueOrDefault(), endTime.GetValueOrDefault(), false);

            //格表
            var list = STAT_STAFF_LARGE_BUSI_D_DAL.GetHallStatList(UserState.UserID, beginTime, endTime, null);
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
                return ExportData(exceltitle, "业务办理分析报表-服务厅", "服务厅", dic);
            }

            SetMainChart(null, beginTime, endTime, title);

            return View(plist);
        }

        /// <summary>
        /// 员工 业务量对比分析
        /// </summary>
        /// <param name="orgId"></param>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="export"></param>
        /// <returns></returns>
        [UserAuth("STAT_SERVICEComparisonAnalysis")]
        public ActionResult StaffStat(string orgId,DateTime? beginTime, DateTime? endTime,
            int pageIndex = 1, int pageSize = 20, bool export = false)
        {
            base.DateTimeInit(ref beginTime, ref endTime);

            var mainTielt = GetOrgName(orgId, null);
            ViewBag.MainTitle = GetTitleName(mainTielt, "业务量对比分析", beginTime.GetValueOrDefault(), endTime.GetValueOrDefault());
            var title = GetTitleName(mainTielt, "", beginTime.GetValueOrDefault(), endTime.GetValueOrDefault(), false);
            var exceltitle = GetTitleName(mainTielt, "业务办理分析", beginTime.GetValueOrDefault(), endTime.GetValueOrDefault(), false);


            var list = STAT_STAFF_LARGE_BUSI_D_DAL.GetStatList(orgId, beginTime, endTime, null);

            var dic = list.GroupBy(m => m.STAFF_NAM).OrderBy(m => m.Key).ToList();
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
                return ExportData(exceltitle, "业务办理分析报表-员工", "员工名称", dic);
            }

            SetMainChart(orgId, beginTime, endTime, title);


            return View(plist);
        }

        public ActionResult ShowCNT()
        {
            return View();
        }

        public void SetMainChart(string orgId, DateTime? beginTime, DateTime? endTime, string subtitle)
        {
            var list2 = STAT_STAFF_LARGE_BUSI_D_DAL.GetStatList(orgId, UserState.UserID, beginTime, endTime);
            string titleName = "业务量对比分析";
            var lineTable = new DataTable();

            lineTable.Columns.Add("X_Name", typeof(string));
            lineTable.Columns.Add("业务笔数", typeof(int));
            lineTable.Columns.Add("业务折合量", typeof(int));
            lineTable.Columns.Add("平均办理时间", typeof(string));
            lineTable.Columns.Add("超时办理笔数", typeof(int));
            lineTable.Columns.Add("超时率", typeof(string));
            lineTable.Columns.Add("同城业务笔数", typeof(int));
            lineTable.Columns.Add("同城办理率", typeof(string));

            //有服务厅编码按服务厅统计，否则按员工统计
            var halls =
                list2.GroupBy(r => string.IsNullOrEmpty(orgId) ? r.HALL_NAM : r.STAFF_NAM).OrderBy(m => m.Key).ToList();
            foreach (var hall in halls)
            {
                var itmes = hall.ToList();

                var busiTotal = itmes.Sum(m => m.BUSI_CNT);
                var convertTotal = itmes.Sum(m => m.CONVERT_BUSI_CNT);
                var handleTotal = itmes.Sum(m => m.HANDLE_DUR);
                var overTimeTotal = itmes.Sum(m => m.OVERTIME_HANDLE_CNT);
                var localTotal = itmes.Sum(m => m.LOCAL_CNT);

                var dr = lineTable.NewRow();
                dr["X_Name"] = hall.Key;
                dr["业务笔数"] = busiTotal;
                dr["业务折合量"] = convertTotal;
                dr["平均办理时间"] = (busiTotal == 0 ? 0 : handleTotal / busiTotal).ToTimeString();
                dr["超时办理笔数"] = overTimeTotal;
                dr["超时率"] = (busiTotal == 0 ? 0 : (decimal)overTimeTotal / (decimal)busiTotal).ToString("P");
                dr["同城业务笔数"] = localTotal;
                dr["同城办理率"] = (busiTotal == 0 ? 0 : (decimal)localTotal / (decimal)busiTotal).ToString("P");

                lineTable.Rows.Add(dr);
            }



            ViewBag.ChartColumn3DXML = CreateMSColumn3DChart(titleName, lineTable, 430, subtitle);
            ViewBag.ChartSplineXML = CreateMSSplineChart(titleName, lineTable, 430, null, null, subtitle);
        }

        /// <summary>
        /// 设置图形报表
        /// </summary>
        /// <param name="orgId"></param>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        public void SetChart(string orgId, DateTime? beginTime, DateTime? endTime, string subtitle)
        {
            var list2 = STAT_STAFF_LARGE_BUSI_D_DAL.GetStatList(orgId, UserState.UserID, beginTime, endTime);
            string titleName = "业务量对比分析";
            var lineTable = new DataTable();
            lineTable.Columns.Add("Y_Name", typeof(string));

            var listDLSERIAL = dlDal.FindList();

            foreach (var item in listDLSERIAL)
            {
                lineTable.Columns.Add(item.DLS_SERIALNAME, typeof(int));
            }

            beginTime = beginTime.GetValueOrDefault().AddHours(8);
            endTime = endTime.GetValueOrDefault().AddHours(18);

            TimeSpan timeSpan = endTime.GetValueOrDefault().Subtract(beginTime.GetValueOrDefault());
            var tlist = new List<string>();
            base.SetLineYName(timeSpan, beginTime.GetValueOrDefault(), lineTable, tlist, endTime.GetValueOrDefault());

            for (var i = 0; i < lineTable.Rows.Count; i++)
            {
                var tempbtiem = Convert.ToDateTime(tlist[i]);
                var tempetiem = i + 1 < lineTable.Rows.Count ? Convert.ToDateTime(tlist[i + 1]) : endTime.GetValueOrDefault();

                var soureList = new List<STAT_STAFF_LARGE_BUSI_D_Handle_SUB>();
                if (timeSpan.Days < 1)
                {
                    soureList = list2.
                        Where(x => x.TIME_QUANTUM_CD == Convert.ToByte(tempbtiem.Hour.ToString())).ToList();
                }
                else
                {
                    if (Convert.ToDateTime(tempbtiem.ToShortDateString()) ==
                        Convert.ToDateTime(endTime.GetValueOrDefault().ToShortDateString()))
                    {
                        soureList =
                            list2.Where(
                                x =>
                                x.STAT_DT >= Convert.ToDateTime(tempbtiem.ToShortDateString()) &&
                                x.STAT_DT < Convert.ToDateTime(tempetiem.AddDays(1).ToShortDateString())).ToList();
                    }
                    else
                    {
                        soureList =
                            list2.Where(
                                x =>
                                x.STAT_DT >= Convert.ToDateTime(tempbtiem.ToShortDateString()) &&
                                x.STAT_DT < Convert.ToDateTime(tempetiem.ToShortDateString())).ToList();
                    }

                }

                foreach (var item in listDLSERIAL)
                {
                    lineTable.Rows[i][item.DLS_SERIALNAME] =
                        soureList.Where(m => m.DLS_SERIALID == item.DLS_SERIALID).Sum(m => m.BUSI_CNT);
                }
            }


            ViewBag.ChartColumn3DXML = CreateMSColumn3DChart(titleName, lineTable, 430, subtitle);
            ViewBag.ChartSplineXML = CreateMSSplineChart(titleName, lineTable, 430, null, null, subtitle);
            ViewBag.ChartPie3DXML = CreatePie3DChart(titleName, lineTable, 430, subtitle);
        }

        #region 导出

        /// <summary>
        /// 导出报表
        /// </summary>
        /// <param name="title"></param>
        /// <param name="fileName"></param>
        /// <param name="Listname"></param>
        /// <param name="dic"></param>
        /// <returns></returns>
        public ActionResult ExportData(string title,string fileName, string Listname,
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
        #endregion
    }
}
