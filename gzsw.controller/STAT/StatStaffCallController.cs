using gzsw.controller.MyAuth;
using gzsw.dal;
using gzsw.dal.dao;
using gzsw.model;
using gzsw.util;
using InfoSoftGlobal;
using Ninject;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;
using NPOI.SS.Formula.Functions;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using PetaPoco;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace gzsw.controller.STAT
{
    public class StatStaffCallController : BaseController<STAT_STAFF_BUSI_TOT_D>
    {
        [Inject]
        public IDao<SYS_HALL> DaoHall { get; set; }
        [Inject]
        public IDao<SYS_STAFF> DaoStaff { get; set; }

        #region 省权限报表
        /// <summary>
        /// 省权限展示的报表
        /// </summary>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <param name="orgid"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet]
        [UserAuth("STAT_STAFF_QUEUE_BUSI_D_VIW")]
        public ActionResult Index(DateTime? beginTime, DateTime? endTime, string orgid, int pageIndex = 1, int pageSize = 20, bool export = false)
        {
            base.DateTimeInit(ref beginTime, ref endTime);

            ViewBag.beginTime = beginTime == null ? "" : beginTime.Value.ToString("yyyy-MM-dd");
            ViewBag.endTime = endTime == null ? "" : endTime.Value.ToString("yyyy-MM-dd");
            var t = 3;//员工报表
            ViewBag.NAM = "员工";
            if (string.IsNullOrEmpty(Request.QueryString["t"])
                || !int.TryParse(Request.QueryString["t"], out t))
            {
                t = 1;
            }
            if (t == 1)
            {
                ViewBag.NAM = "省市";
                ViewBag.NAMTitle = "/STAT/StatStaffCall?t=2";//如果是省级，点击进入市级
            }
            if (t == 2)
            {
                ViewBag.NAM = "服务厅";
                ViewBag.NAMTitle = "/STAT/StatStaffCall?t=3";//如果是省级，点击进入员工报表
            }
            if (GetHighLV == model.Enums.UserLV_ENUM.市级 && t < 2)//判断是否有权限
            {
                return Redirect("/STAT/StatStaffCall?t=2");
            }
            if (GetHighLV == model.Enums.UserLV_ENUM.区级 && t < 2)//判断是否有权限
            {
                return Redirect("/STAT/StatStaffCall?t=2");
            }
            if (GetHighLV == model.Enums.UserLV_ENUM.服务厅级 && t < 3)//判断是否有权限
            {
                return Redirect("/STAT/StatStaffCall?t=3");
            }
            if (GetHighLV == model.Enums.UserLV_ENUM.无权限)//判断是否有权限
            {
                return NoAuth;
            }
            var re = base.UserHall;
            var orgall = new SYS_USER_DAL().GetUserORG(UserState.UserID);
            ViewBag.UserORG = new SelectList(orgall.Where(obj => obj.ORG_LEVEL == 4)
                , "ORG_ID", "ORG_NAM", orgid);
            if (!string.IsNullOrEmpty(orgid))
            {
                orgall = orgall.Where(obj => obj.ORG_ID == orgid).ToList();
                if (null == orgall || orgall.Count() == 0)
                {
                    orgall = new List<SYS_ORGANIZE> { new SYS_ORGANIZE { ORG_ID = "-1" } };
                }
            }

            if (export)
                pageIndex = 0;
            Page<dynamic> data = null;

            if (t == 1)
            {
                data = new STAT_STAFF_BUSI_TOT_D_DAL().Q_STATDATA_GROUP_CITY(0, pageSize, null, beginTime, endTime);
            }
            if (t == 2)
            {
                var halllist = new SYS_HALL_DAL().GetOrgHallAndChild(orgid).ToArray();
                data = new STAT_STAFF_BUSI_TOT_D_DAL().Q_STATDATA_GROUP_HALL(0, pageSize, halllist, beginTime, endTime);
            }
            if (t == 3)
            {
                var halllist = new SYS_HALL_DAL().GetOrgHallAndChild(orgid).ToArray();
                data = new STAT_STAFF_BUSI_TOT_D_DAL().Q_STATDATA_GROUP_STAFF(0, pageSize, halllist, beginTime, endTime);
            }
            string MainTitle = "";
            if (!string.IsNullOrEmpty(orgid))
                MainTitle += orgall.FirstOrDefault((o => o.ORG_ID == orgid)).ORG_NAM;
            else
                MainTitle += orgall.OrderBy(o => o.ORG_LEVEL).FirstOrDefault().ORG_NAM;
            MainTitle += "排队叫号分析";
            if (beginTime == null && endTime == null)
            {
                var min = data.Items.Select(o => o.MIN_STAT_DT);
                if (null != min)
                    MainTitle += "<span style='font-size:12px;'>（" + min.Min().ToString("yyyy年MM月dd");
                MainTitle += " - " + data.Items.Select(o => o.MAX_STAT_DT).Max().ToString("yyyy年MM月dd") + "）</span>";
            }
            else
            {
                MainTitle += "<span style='font-size:12px;'>（" + beginTime.Value.ToString("yyyy年MM月dd");
                MainTitle += " - " + endTime.Value.ToString("yyyy年MM月dd") + "）</span>";
            }
            ViewBag.MainTitle = MainTitle;
            if (export)//导出
            {
                string xlnam = "排队叫号分析－省市";
                if (t == 2)
                    xlnam = "排队叫号分析－服务厅";
                if (t == 3)
                    xlnam = "排队叫号分析－员工";
                return ExportData(xlnam, MainTitle, data.Items);
            }
            else
            {
                DataSet ds = new DataSet();
                DataTable dt = new DataTable();
                ds.Tables.Add(dt);
                dt.Columns.Add("NAM", typeof(string));
                dt.Columns.Add("呼叫量", typeof(string));
                dt.Columns.Add("办理量", typeof(string));
                dt.Columns.Add("弃号量", typeof(string));
                foreach (var item in data.Items)
                {
                    DataRow r = dt.NewRow();
                    r["NAM"] = item.NAM;
                    r["呼叫量"] = item.CALL_CNT + ";JavaScript:return showCNT(\\\"CALLCNT\\\", \\\"" + Request.QueryString["orgid"] + "\\\")";
                    r["办理量"] = item.HANDLE_CNT + ";JavaScript:return showCNT(\\\"HANDLECNT\\\", \\\"" + Request.QueryString["orgid"] + "\\\")";
                    r["弃号量"] = item.ABANDON_CNT + ";JavaScript:return showCNT(\\\"ABANDONCNT\\\", \\\"" + Request.QueryString["orgid"] + "\\\")";
                    dt.Rows.Add(r);
                }
                MainTitle = MainTitle.Replace("<span style='font-size:12px;'>", "")
                    .Replace("</span>", "");
                ViewBag.ChartColumn3DXML = CreateMSColumn3DChart("排队叫号分析", ds, 420, MainTitle,true);
                var data2 = new STAT_STAFF_BUSI_TOT_D_DAL().GetSTATList_DT(null, ref beginTime, ref endTime, orgid);

                while (data2.Columns.Count > 4)
                    data2.Columns.RemoveAt(4);
                ViewBag.ChartSplineXML = CreateMSSplineChart("排队叫号分析", data2, 420,null,null, MainTitle);
                return View("Index", data);
            }
        }

        /// <summary>
        /// 报表导出
        /// </summary>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <param name="orgid"></param>
        /// <returns></returns>
        [UserAuth("STAT_STAFF_QUEUE_BUSI_D_VIW")]
        public ActionResult ExportData(string sheet, string MainTitle, List<dynamic> exportData)
        {
            #region 加载模板文件到工作簿对象中

            HSSFWorkbook hssfworkbook;
            using (FileStream file = new FileStream(Server.MapPath("/Areas/STAT/Views/STATTemp/排队叫号分析.xls"), FileMode.Open, FileAccess.Read))
            {
                hssfworkbook = new HSSFWorkbook(file);
            }

            #endregion

            #region 根据模板设置工作表的内容

            ISheet sheet1 = hssfworkbook.GetSheet("排队叫号分析");
            string maintitle = MainTitle.Replace("<span style='font-size:12px;'>", "").Replace("</span>", "");
            sheet1.GetRow(0).Cells[0].SetCellValue(maintitle);
            int rowIndex = 4;
            if (null != exportData)
            {
                #region 数据单元格样式
                var cellFont = hssfworkbook.CreateFont();
                var cellStyle = hssfworkbook.CreateCellStyle();

                cellStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.White.Index;
                cellStyle.FillPattern = NPOI.SS.UserModel.FillPattern.SolidForeground;
                #endregion
                sheet1.GetRow(3).Cells[1].SetCellValue(sheet.Split('－')[1]);
                foreach (var row in exportData)
                {
                    IRow xlsrow = sheet1.CreateRow(rowIndex);
                    xlsrow.CreateCell(0).SetCellValue(rowIndex - 3);
                    xlsrow.CreateCell(1).SetCellValue(row.NAM);
                    xlsrow.CreateCell(2).SetCellValue(row.CALL_CNT);
                    xlsrow.CreateCell(3).SetCellValue((row.CALL_CNT * 100.0 / exportData.Sum(obj => obj.CALL_CNT)).ToString("f2") + "%");
                    xlsrow.CreateCell(4).SetCellValue(row.HANDLE_CNT);
                    xlsrow.CreateCell(5).SetCellValue((row.HANDLE_CNT * 100.0 / exportData.Sum(obj => obj.HANDLE_CNT)).ToString("f2") + "%");
                    xlsrow.CreateCell(6).SetCellValue(row.OVERTIME_WAIT_CNT);
                    xlsrow.CreateCell(7).SetCellValue((row.OVERTIME_WAIT_CNT * 100.0 / exportData.Sum(obj => obj.OVERTIME_WAIT_CNT)).ToString("f2") + "%");
                    xlsrow.CreateCell(8).SetCellValue(row.SECOND_SVR_CNT);
                    xlsrow.CreateCell(9).SetCellValue((row.SECOND_SVR_CNT * 100.0 / exportData.Sum(obj => obj.SECOND_SVR_CNT)).ToString("f2") + "%");
                    xlsrow.CreateCell(10).SetCellValue((row.WAIT_DUR / row.CALL_CNT).ToString("f2") + "%");
                    xlsrow.CreateCell(11).SetCellValue(row.LOCAL_CNT);
                    xlsrow.CreateCell(12).SetCellValue((row.LOCAL_CNT * 100.0 / exportData.Sum(obj => obj.LOCAL_CNT)).ToString("f2") + "%");
                    xlsrow.CreateCell(13).SetCellValue(row.VOTE_MULTI_CNT);
                    xlsrow.CreateCell(14).SetCellValue((row.VOTE_MULTI_CNT * 100.0 / exportData.Sum(obj => obj.VOTE_MULTI_CNT)).ToString("f2") + "%");
                    xlsrow.CreateCell(15).SetCellValue(row.ABANDON_CNT);
                    xlsrow.CreateCell(16).SetCellValue((row.ABANDON_CNT * 100.0 / exportData.Sum(obj => obj.ABANDON_CNT)).ToString("f2") + "%");
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
                    rowIndex++;
                }
                IRow footer = sheet1.CreateRow(rowIndex);

                footer.CreateCell(0).SetCellValue("合计");//#fffdbb
                footer.CreateCell(1).SetCellValue("合计");//#fffdbb
                footer.CreateCell(2).SetCellValue(exportData.Sum(obj => obj.CALL_CNT));
                footer.CreateCell(3).SetCellValue("100%");
                footer.CreateCell(4).SetCellValue(exportData.Sum(obj => obj.HANDLE_CNT));
                footer.CreateCell(5).SetCellValue("100%");
                footer.CreateCell(6).SetCellValue(exportData.Sum(obj => obj.OVERTIME_WAIT_CNT));
                footer.CreateCell(7).SetCellValue("100%");
                footer.CreateCell(8).SetCellValue(exportData.Sum(obj => obj.SECOND_SVR_CNT));
                footer.CreateCell(9).SetCellValue("100%");

                footer.CreateCell(10).SetCellValue((exportData.Sum(obj2 => (double)obj2.WAIT_DUR) / exportData.Sum(obj2 => obj2.CALL_CNT)).ToString("f2") + "%");
                footer.CreateCell(11).SetCellValue(exportData.Sum(obj => obj.LOCAL_CNT));
                footer.CreateCell(12).SetCellValue("100%");
                footer.CreateCell(13).SetCellValue(exportData.Sum(obj => obj.VOTE_MULTI_CNT));
                footer.CreateCell(14).SetCellValue("100%");
                footer.CreateCell(15).SetCellValue(exportData.Sum(obj => obj.ABANDON_CNT));
                footer.CreateCell(16).SetCellValue("100%");
                #region 底部样式
                var footercellFont = hssfworkbook.CreateFont();
                var footercellStyle = hssfworkbook.CreateCellStyle();

                footercellFont.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;
                footercellStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.Yellow.Index;
                footercellStyle.FillPattern = NPOI.SS.UserModel.FillPattern.SolidForeground;
                var HSSFPalette = hssfworkbook.GetCustomPalette();
                Color c = ToColor("#fffdbb");
                HSSFPalette.SetColorAtIndex(NPOI.HSSF.Util.HSSFColor.Yellow.Index, c.R, c.G, c.B);
                #endregion

                foreach (var cell in footer.Cells)
                {
                    cell.CellStyle = footercellStyle;
                    cell.CellStyle.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Center;
                    cell.CellStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                    cell.CellStyle.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                    cell.CellStyle.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                    cell.CellStyle.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                    cell.CellStyle.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                }
                CellRangeAddress cellRangeAddress = new CellRangeAddress(rowIndex, rowIndex, 0, 1);
                sheet1.AddMergedRegion(cellRangeAddress);
            }

            //强制Excel重新计算表中所有的公式
            sheet1.ForceFormulaRecalculation = true;

            #endregion

            #region 写入到客户端

            MemoryStream ms = new MemoryStream();
            //将工作簿的内容放到内存流中
            hssfworkbook.Write(ms);
            ms.Seek(0, SeekOrigin.Begin);
            return File(ms, "application/vnd.ms-excel", sheet + ".xls");

            #endregion
        }
        #endregion

        /// <summary>
        /// 排队业务报表--呼叫量
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [UserAuth("STAT_STAFF_QUEUE_BUSI_D_VIW")]
        public ActionResult ShowCNT(string ct, DateTime? btime, DateTime? etime, string staffid, string orgid)
        {
            var dataDT = new STAT_STAFF_BUSI_TOT_D_DAL().GetSTATList_DT(staffid, ref btime,ref etime, orgid);
            string caption = "";
            if (ct == "CALLCNT")
                caption = "排队业务报表--呼叫量";
            if (ct == "HANDLECNT")
                caption = "排队业务报表--办理量";
            if (ct == "OVERTIMECNT")
                caption = "排队业务报表--等候超时量";
            if (ct == "SECONDCNT")
                caption = "排队业务报表--二次办税量";
            if (ct == "WAITPERCENT")
                caption = "排队业务报表--等候时间";
            if (ct == "LOCALCNT")
                caption = "排队业务报表--同城受理量";
            if (ct == "VOTECNT")
                caption = "排队业务报表--一票多业务量";
            if (ct == "ABANDONCNT")
                caption = "排队业务报表--弃号量";

            if (ct == "CALLCNT")
                dataDT.Columns[1].SetOrdinal(1);
            if (ct == "HANDLECNT")
                dataDT.Columns[2].SetOrdinal(1);
            if (ct == "OVERTIMECNT")
                dataDT.Columns[11].SetOrdinal(1);
            if (ct == "SECONDCNT")
                dataDT.Columns[12].SetOrdinal(1);
            if (ct == "WAITPERCENT")
                dataDT.Columns[13].SetOrdinal(1);
            if (ct == "LOCALCNT")
                dataDT.Columns[4].SetOrdinal(1);
            if (ct == "VOTECNT")
                dataDT.Columns[5].SetOrdinal(1);
            if (ct == "ABANDONCNT")
                dataDT.Columns[3].SetOrdinal(1);
            
            var orgall = new SYS_USER_DAL().GetUserORG(UserState.UserID);
            string MainTitle = "";
            if (!string.IsNullOrEmpty(orgid))
                MainTitle += orgall.FirstOrDefault((o => o.ORG_ID == orgid)).ORG_NAM;
            else
                MainTitle += orgall.OrderBy(o => o.ORG_LEVEL).FirstOrDefault().ORG_NAM;
            MainTitle += "<span style='font-size:12px;'>（" + btime.Value.ToString("yyyy年MM月dd");
            MainTitle += " - " + etime.Value.ToString("yyyy年MM月dd") + "）</span>";
            ViewBag.MainTitle = MainTitle;
            MainTitle = MainTitle.Replace("<span style='font-size:12px;'>", "")
                    .Replace("</span>", "");
            if (ct == "WAITPERCENT")
            {
                ViewBag.ChartXml = CreateColumn3DChart(caption, dataDT, MainTitle, true, "分", "60", "分");
                ViewBag.ChartSplineXML = CreateSplineChart(caption, dataDT, 550, null, null, MainTitle, true, "分", "60", "分");
            }
            else
            {
                ViewBag.ChartXml = CreateColumn3DChart(caption, dataDT, MainTitle);
                ViewBag.ChartSplineXML = CreateSplineChart(caption, dataDT, 550, null, null, MainTitle);
            }
            
            return View();
        }

        public ActionResult ShowSingDialog(DateTime? beginTime, DateTime? endTime, string orgid, string serid, int pageIndex = 1, int pageSize = 20)
        {
            var list = new STAT_STAFF_BUSI_TOT_D_DAL().GetSTATDetails(pageIndex, pageSize, null, beginTime, endTime, serid);
            return View(list);
        }

        #region 组织结构树 - 查找市下面所有服务厅，员工
        /// <summary>
        /// 获取权限树
        /// </summary>
        /// <returns></returns>
        public List<ZtreeNode_ORG> GetOrgs2Tree()
        {
            var all = new SYS_USER_DAL().GetUserORG(UserState.UserID);
            var hallall = DaoHall.FindList();
            var staffall = DaoStaff.FindList();
            foreach (var org in all.Where(obj => obj.PAR_ORG_ID == ""))
            {
                org.PAR_ORG_ID = null;
            }
            var treelist = all.Where(obj => obj.ORG_LEVEL == 2)//市级
                .Select(obj => new ZtreeNode_ORG
                {
                    id = obj.ORG_ID.ToString(),
                    name = obj.ORG_NAM
                }).ToList();
            foreach (var leaf in treelist)
            {
                List<ZtreeNode_ORG> leafchild = new List<ZtreeNode_ORG>();
                GenOrgs2Tree(all, staffall, hallall, ref leafchild, leaf.id);
                leaf.children = leafchild;
            }
            return treelist;
        }

        private void GenOrgs2Tree(IEnumerable<SYS_ORGANIZE> all, IEnumerable<SYS_STAFF> staffall, IEnumerable<SYS_HALL> hallall,
            ref List<ZtreeNode_ORG> leafchild,
            string parid)
        {
            var l = all.Where(obj => obj.PAR_ORG_ID == parid);
            foreach (var obj in l)
            {
                var h_ = hallall.Where(obj2 => obj2.ORG_ID == obj.ORG_ID)
                 .Select(obj2 => new ZtreeNode_ORG
                 {
                     id = obj2.HALL_NO,
                     Org_LV = 5,
                     children = staffall.Where(obj3 => obj3.ORG_ID == obj2.HALL_NO).Select(obj3 => new ZtreeNode_ORG
                     {
                         id = obj3.STAFF_ID,
                         Org_LV = 6
                     }).ToList()
                 }).ToList();

                leafchild.AddRange(h_);
                GenOrgs2Tree(all, staffall, hallall, ref  leafchild, obj.ORG_ID);
            }
        }
        #endregion

        #region 私有方法
        private Color ToColor(string color)
        {

            int red, green, blue = 0;
            char[] rgb;
            color = color.TrimStart('#');
            color = Regex.Replace(color.ToLower(), "[g-zG-Z]", "");
            switch (color.Length)
            {
                case 3:
                    rgb = color.ToCharArray();
                    red = Convert.ToInt32(rgb[0].ToString() + rgb[0].ToString(), 16);
                    green = Convert.ToInt32(rgb[1].ToString() + rgb[1].ToString(), 16);
                    blue = Convert.ToInt32(rgb[2].ToString() + rgb[2].ToString(), 16);
                    return Color.FromArgb(red, green, blue);
                case 6:
                    rgb = color.ToCharArray();
                    red = Convert.ToInt32(rgb[0].ToString() + rgb[1].ToString(), 16);
                    green = Convert.ToInt32(rgb[2].ToString() + rgb[3].ToString(), 16);
                    blue = Convert.ToInt32(rgb[4].ToString() + rgb[5].ToString(), 16);
                    return Color.FromArgb(red, green, blue);
                default:
                    return Color.FromName(color);
            }
        }
        #endregion
    }
}
