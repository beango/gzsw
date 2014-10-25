using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using gzsw.model;
using PetaPoco;

namespace gzsw.dal.dao
{
    public class SYS_COUNTER_DAL : Basedao
    {
        /// <summary>
        /// 通过服务厅编号获取窗口信息
        /// </summary>
        /// <param name="hallNo">服务厅编号</param>
        /// <returns></returns>
        public List<SYS_COUNTER> GetListByHallNo(string hallNo)
        {
            var db = new Database();

            var sql = Sql.Builder.Append(@"SELECT  [HALL_NO]
                                          ,[COUNTER_ID]
                                          ,[SPEC_FUN_IND]
                                          ,[STATE]
                                          ,[MAX_BUSI_CNT]
                                          ,[PRI1_BUSI_SER]
                                          ,[PRI2_BUSI_SER]
                                          ,[PRI3_BUSI_SER]
                                          ,[PRI4_BUSI_SER]
                                          ,[PRI5_BUSI_SER]
                                          ,[CREATE_ID]
                                          ,[CREATE_DTIME]
                                          ,[MODIFY_ID]
                                          ,[MODIFY_DTIME]
                                          ,[NOTE]
                                      FROM [SYS_COUNTER]");

            sql.Append(string.Format(" WHERE [HALL_NO]={0}", hallNo));

            return db.Fetch<SYS_COUNTER>(sql);
        }
    }
}
