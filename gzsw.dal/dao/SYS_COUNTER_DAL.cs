using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using gzsw.model;
using PetaPoco;

namespace gzsw.dal.dao
{
    public class SYS_COUNTER_DAL
    {
        /// <summary>
        /// 通过服务厅编号获取窗口信息
        /// </summary>
        /// <param name="hallNo">服务厅编号</param>
        /// <returns></returns>
        public List<SYS_COUNTER> GetListByHallNo(string hallNo)
        {
            var db = gzswDB.GetInstance();

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

        public int Update(SYS_COUNTER info)
        {
            var sql = Sql.Builder.Append(@"UPDATE [dbo].[SYS_COUNTER]
               SET [SPEC_FUN_IND] = @SPEC_FUN_IND
                  ,[STATE] = @STATE
                  ,[MAX_BUSI_CNT] = @MAX_BUSI_CNT
                  ,[PRI1_BUSI_SER] = @PRI1_BUSI_SER
                  ,[PRI2_BUSI_SER] = @PRI2_BUSI_SER
                  ,[PRI3_BUSI_SER] = @PRI3_BUSI_SER
                  ,[PRI4_BUSI_SER] = @PRI4_BUSI_SER
                  ,[PRI5_BUSI_SER] = @PRI5_BUSI_SER
                  ,[MODIFY_ID] = @MODIFY_ID
                  ,[MODIFY_DTIME] = @MODIFY_DTIME
                  ,[NOTE] = @NOTE
             WHERE [HALL_NO] = @HALL_NO and [COUNTER_ID] = @COUNTER_ID"
                , new
                {
                    SPEC_FUN_IND = info.SPEC_FUN_IND,
                    STATE = info.STATE,
                    MAX_BUSI_CNT = info.MAX_BUSI_CNT,
                    PRI1_BUSI_SER = info.PRI1_BUSI_SER,
                    PRI2_BUSI_SER = info.PRI2_BUSI_SER,
                    PRI3_BUSI_SER = info.PRI3_BUSI_SER,
                    PRI4_BUSI_SER = info.PRI4_BUSI_SER,
                    PRI5_BUSI_SER = info.PRI5_BUSI_SER,
                    MODIFY_ID = info.MODIFY_ID,
                    MODIFY_DTIME = info.MODIFY_DTIME,
                    NOTE = info.NOTE,
                    HALL_NO = info.HALL_NO,
                    COUNTER_ID = info.COUNTER_ID
                });
            return gzswDB.GetInstance().Execute(sql);
        }
    }
}
