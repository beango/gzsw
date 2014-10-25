using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace gzsw.dal
{
    public class SqlHelper
    {
        private static string GetConnection(string connectionKey)
        {
            return ConfigurationManager.ConnectionStrings[connectionKey].ConnectionString;
        }

        public static void BulkInsert(string connectStr, string tableName, DataTable dataTable)
        {
            if (dataTable.Rows.Count == 0)
            {
                return;
            }

            using (SqlBulkCopy sqlbulkCopy = new SqlBulkCopy(GetConnection(connectStr), SqlBulkCopyOptions.UseInternalTransaction))
            {
                sqlbulkCopy.DestinationTableName = tableName;
                for (int i = 0; i < dataTable.Columns.Count; i++)
                {
                    sqlbulkCopy.ColumnMappings.Add(dataTable.Columns[i].ColumnName,
                                                   dataTable.Columns[i].ColumnName);
                }
                sqlbulkCopy.WriteToServer(dataTable);
            }
        }
    }
}
