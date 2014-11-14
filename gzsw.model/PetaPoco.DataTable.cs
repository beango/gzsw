using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace PetaPoco
{
    public partial class Database
    {
        #region operation Fill

        /// <summary>  
        /// 填充一个DataTable  
        /// </summary>  
        /// <param name="dt">DataTable的引用</param>  
        /// <param name="sql">Sql语句</param>  
        /// <param name="args">参数</param>  
        public DataTable Fill( string sql, params object[] args)
        {
            try
            {
                OpenSharedConnection();
                try
                {
                    using (var cmd = CreateCommand(_sharedConnection, sql, args))
                    {
                        var val = cmd.ExecuteReader();
                        OnExecutedCommand(cmd);
                        var dt = new DataTable();
                        dt.Load(val);
                        return dt; //(T)Convert.ChangeType(val, typeof(T));
                    }
                }
                finally
                {
                    CloseSharedConnection();
                }
            }
            catch (Exception x)
            {
                OnException(x);
                throw;
            }
        }

        #endregion
    }
}
