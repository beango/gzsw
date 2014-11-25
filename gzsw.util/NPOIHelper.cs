using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.IO;
using System.Data;

namespace gzsw.util
{
    public class NPOIHelper
    {
        static HSSFWorkbook hssfworkbook;

        public static DataTable ImportToDataTable(string filePath)
        {
            using (FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                hssfworkbook = new HSSFWorkbook(file);
            }

            ISheet sheet = hssfworkbook.GetSheetAt(0);
            System.Collections.IEnumerator rows = sheet.GetRowEnumerator();

            DataTable dt = new DataTable();

            int rowindex = 0;
            int cols = 0;
            while (rows.MoveNext())
            {
                IRow row = (HSSFRow)rows.Current;
                if (rowindex == 0)
                {
                    cols = row.LastCellNum;
                    for (int i = 0; i < cols; i++)
                    {
                        ICell cell = row.GetCell(i);

                        if (cell == null)
                        {
                            dt.Columns.Add(Convert.ToChar(('A') + i).ToString());
                        }
                        else
                        {
                            dt.Columns.Add(cell.ToString().Trim() + i.ToString());
                        }
                    }
                }
                else
                {
                    DataRow dr = dt.NewRow();

                    for (int i = 0; i < cols; i++)
                    {
                        ICell cell = row.GetCell(i);

                        if (cell == null)
                        {
                            dr[i] = null;
                        }
                        else
                        {
                            dr[i] = cell.ToString().Trim();
                        }
                    }
                    dt.Rows.Add(dr);
                }
                rowindex++;
            }
            return dt;
        }

        public static DataTable GetDataTable(string filePath)
        {
            using (FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                hssfworkbook = new HSSFWorkbook(file);
            }

            ISheet sheet = hssfworkbook.GetSheetAt(0);
            System.Collections.IEnumerator rows = sheet.GetRowEnumerator();

            DataTable dt = new DataTable();

            int rowindex = 0;
            int cols = 0;
            while (rows.MoveNext())
            {
                IRow row = (HSSFRow)rows.Current;
                if (rowindex == 0)
                {
                    cols = row.LastCellNum;
                    for (int i = 0; i < cols; i++)
                    {
                        ICell cell = row.GetCell(i);

                        if (cell == null)
                        {
                            dt.Columns.Add(Convert.ToChar(('A')).ToString());
                        }
                        else
                        {
                            dt.Columns.Add(cell.ToString().Trim());
                        }
                    }
                }
                else
                {
                    DataRow dr = dt.NewRow();

                    for (int i = 0; i < cols; i++)
                    {
                        ICell cell = row.GetCell(i);

                        if (cell == null)
                        {
                            dr[i] = null;
                        }
                        else
                        {
                            dr[i] = cell.ToString().Trim();
                        }
                    }
                    dt.Rows.Add(dr);
                }
                rowindex++;
            }
            return dt;
        }
    }
}
