 
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Aspose.Cells;
using System.Drawing;

namespace gzsw.util.Office
{
    /// <summary>
    /// Excel文件操作类,不依赖MS的Office COM组件
    /// </summary>
    /// <remarks>
    ///     <para>    Creator：GORSON</para>
    ///     <para>CreatedTime：2012-5-11 14:23:32</para>
    /// </remarks>
    public class AsposeExcelHelper
    {
        /// <summary>
        /// 从Excel获取DataTable
        /// </summary>
        /// <param name="worksheetName">工作区间名称</param>
        /// <param name="path">Excel文件路径</param>
        /// <param name="columnCount">列总数</param>
        /// <returns></returns>
        public static DataTable GetFileToDataTable(string worksheetName, string path, int columnCount)
        {

            Workbook workbook1 = new Workbook(path);
            var workBook = getSheet(workbook1,worksheetName);
           
            // DataTable结果集
            var result = new DataTable(worksheetName); 
            var maxRow = workBook.Cells.MaxRow;
            var maxColumn = workBook.Cells.MaxColumn + 1;
            
            if (maxColumn == 0)
            {
                // 如果列数为空则不进行操作
                return result;
            }
            DataTable nameList = workBook.Cells.ExportDataTable(0, 0, 1, maxColumn);

            // 列如下：  
            foreach (var row in nameList.Rows)
            {
                var rowObj = row as DataRow;
                foreach (var i in rowObj.ItemArray)
                {
                    result.Columns.Add(new DataColumn(i.ToString()));
                }
            } 

            // 行如下： 
            DataTable dataList = workBook.Cells.ExportDataTable(1, 0, maxRow, maxColumn);
            foreach (var row in dataList.Rows)
            {
                var rowObj = row as DataRow;
                foreach (var i in rowObj.ItemArray)
                {
                    result.Rows.Add(i);
                }
            }
            return result;

        }

        /// <summary>   
        /// 导出数据到本地   
        /// </summary>   
        /// <param name="dt">要导出的数据</param>   
        /// <param name="worksheetName">工作区间名称</param>   
        /// <param name="path">保存路径</param>   
        public static void OutFileToDisk(DataTable dt, string worksheetName, string path)
        {
            Workbook workbook = new Workbook(); //工作簿   
            Worksheet sheet = workbook.Worksheets[0]; //工作表   
            sheet.Name = worksheetName;
            Cells cells = sheet.Cells;//单元格   
/*

            //为标题设置样式       
            Style styleTitle = workbook.Styles[workbook.Styles.Add()];//新增样式   
            styleTitle.HorizontalAlignment = TextAlignmentType.Center;//文字居中   
            styleTitle.Font.Name = "宋体";//文字字体   
            styleTitle.Font.Size = 18;//文字大小   
            styleTitle.Font.IsBold = true;//粗体   
*/

            //样式2   
            Style style2 = workbook.Styles[workbook.Styles.Add()];//新增样式   
            style2.HorizontalAlignment = TextAlignmentType.Center;//文字居中   
            style2.Font.Name = "宋体";//文字字体   
            style2.Font.Size = 14;//文字大小   
            style2.Font.IsBold = true;//粗体   
            style2.IsTextWrapped = true;//单元格内容自动换行   
            style2.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
            style2.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
            style2.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
            style2.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;

            //样式3   
            Style style3 = workbook.Styles[workbook.Styles.Add()];//新增样式   
            style3.HorizontalAlignment = TextAlignmentType.Center;//文字居中   
            style3.Font.Name = "宋体";//文字字体   
            style3.Font.Size = 12;//文字大小   
            style3.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
            style3.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
            style3.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
            style3.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;

            int Colnum = dt.Columns.Count;//表格列数   
            int Rownum = dt.Rows.Count;//表格行数   

            /*//生成行1 标题行      
            cells.Merge(0, 0, 1, Colnum);//合并单元格   
            cells[0, 0].PutValue(tableName);//填写内容   
            cells[0, 0].SetStyle(styleTitle);
            cells.SetRowHeight(0, 38);*/

            //生成行2 列名行   
            for (int i = 0; i < Colnum; i++)
            {
                cells[0, i].PutValue(dt.Columns[i].ColumnName);
                cells[0, i].SetStyle(style2);
                cells.SetRowHeight(1, 25);
            }

            //生成数据行   
            for (int i = 0; i < Rownum; i++)
            {
                for (int k = 0; k < Colnum; k++)
                {
                    cells[1 + i, k].PutValue(dt.Rows[i][k].ToString());
                    cells[1 + i, k].SetStyle(style3);
                }
                cells.SetRowHeight(2 + i, 24);
            }

            workbook.Save(path);

        }

        /// <summary>
        /// 输出文件流(用于报表展示)
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public static MemoryStream OutFileToStream(DataTable dt, string tableName)
        {
            Workbook workbook = new Workbook(); //工作簿 
            Worksheet sheet = workbook.Worksheets[0]; //工作表
            sheet.Name = tableName;
            Cells cells = sheet.Cells;//单元格 
             

            //为标题设置样式     
            Style styleTitle = workbook.Styles[workbook.Styles.Add()];//新增样式 
            styleTitle.HorizontalAlignment = TextAlignmentType.Center;//文字居中 
            styleTitle.Font.Name = "宋体";//文字字体 
            styleTitle.Font.Size = 16;//文字大小 
            styleTitle.Font.IsBold = true;//粗体 
            styleTitle.Pattern = BackgroundType.Solid; 
            styleTitle.ForegroundColor = getARGB("#A3C0E8");

            //样式2 
            Style style2 = workbook.Styles[workbook.Styles.Add()];//新增样式 
            style2.HorizontalAlignment = TextAlignmentType.Center;//文字居中 
            style2.Font.Name = "宋体";//文字字体 
            style2.Font.Size = 12;//文字大小 
            style2.Font.IsBold = true;//粗体 
            style2.IsTextWrapped = true;//单元格内容自动换行 
            style2.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
            style2.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
            style2.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
            style2.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin; 
            style2.Pattern = BackgroundType.Solid;
            style2.ForegroundColor = getARGB("#A3C0E8");
          

            //样式3 
            Style style3 = workbook.Styles[workbook.Styles.Add()];//新增样式 
            style3.HorizontalAlignment = TextAlignmentType.Center;//文字居中 
            style3.Font.Name = "宋体";//文字字体 
            style3.Font.Size = 10;//文字大小 
            style3.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
            style3.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
            style3.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
            style3.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;

            int Colnum = dt.Columns.Count;//表格列数 
            int Rownum = dt.Rows.Count;//表格行数 

            //生成行1 标题行    
            cells.Merge(0, 0, 1, Colnum);//合并单元格 
            cells[0, 0].PutValue(tableName);//填写内容 
            cells[0, 0].SetStyle(styleTitle);
            cells.SetRowHeight(0, 38);

            //生成行2 列名行 
            for (int i = 0; i < Colnum; i++)
            {
                cells[1, i].PutValue(dt.Columns[i].ColumnName);
                cells[1, i].SetStyle(style2);
                cells.SetColumnWidth(i, 10);
                cells.SetRowHeight(1, 25);
            }

            //生成数据行 
            for (int i = 0; i < Rownum; i++)
            {
                for (int k = 0; k < Colnum; k++)
                {
                    cells[2 + i, k].PutValue(dt.Rows[i][k].ToString());
                    cells[2 + i, k].SetStyle(style3);
                }
                cells.SetRowHeight(2 + i, 24);
            }

            MemoryStream ms = workbook.SaveToStream();
            return ms;
        }


        /// <summary>
        /// 输出文件流(用于报表展示)
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public static ExcelFileResult OutFileToRespone(DataTable dt, string tableName)
        { 
            return new  ExcelFileResult(dt,tableName);
        }


        /// <summary>
        /// Excel文件结果集
        /// </summary>

        public class ExcelFileResult : System.Web.Mvc.ActionResult
        {
            protected DataTable datatable;
            protected string tableName;

            public ExcelFileResult(DataTable dt, string _tableName)
            {
                datatable = dt;
                tableName = _tableName;

            }

            public override void ExecuteResult(ControllerContext context)
            {
                //下载
                System.IO.MemoryStream ms = AsposeExcelHelper.OutFileToStream(datatable, tableName);
                byte[] bt = ms.ToArray();
                string fileName = tableName + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";//客户端保存的文件名  

                //以字符流的形式下载文件  
                HttpContext.Current.Response.ContentType = "application/vnd.ms-excel";
                //通知浏览器下载文件而不是打开
                HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment; filename=" + HttpUtility.UrlEncode(fileName, System.Text.Encoding.UTF8));
                HttpContext.Current.Response.BinaryWrite(bt);
                HttpContext.Current.Response.Flush();
                HttpContext.Current.Response.End();
               
            }

        }


        #region Helper

        private static Color getARGB(string strColor)
        {
            Color col = ColorTranslator.FromHtml(strColor);
            int alpha = col.A;
            int red = col.R;
            int green = col.G;
            int blue = col.B;
            return  Color.FromArgb(alpha, red, green, blue); 
        }

        /// <summary>
        /// 获取指定的sheet,没有指定默认返回第一个sheet
        /// </summary>
        /// <param name="wb"></param>
        /// <param name="sheetName"></param>
        /// <returns></returns>
        private static Worksheet getSheet(Workbook wb, string sheetName)
        {
            Worksheet result = null;

            if (!string.IsNullOrWhiteSpace(sheetName))
            {
                foreach (Worksheet wsItem in wb.Worksheets)
                {
                    if (wsItem.Name == sheetName)
                    {
                        result = wsItem;
                        break;
                    }
                }
            }
            else
            {
                result = wb.Worksheets[0];
            }

            return result;
        }
        #endregion
    }
}