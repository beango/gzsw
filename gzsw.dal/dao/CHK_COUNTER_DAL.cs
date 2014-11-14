using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using gzsw.model;
using PetaPoco;

namespace gzsw.dal.dao
{
    /// <summary>
    /// 窗口排班
    /// </summary>
    public class CHK_COUNTER_DAL
    {
        /// <summary>
        /// 获取用户的服务厅下所有窗口的排班信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public Page<CounterFuncs> GetCounterFuncs(string hallNo, string hallName, string userId, int pageIndex, int pageSize)
        {
            var db = gzswDB.GetInstance();

            var sql = Sql.Builder.Append(@"SELECT COU.HALL_NO,
                                                        HA.HALL_NAM,
                                                        COU.COUNTER_ID,
                                                        COU.NOTE,
                                                        COU.MODIFY_ID,
                                                        COU.MODIFY_DTIME FROM CHK_COUNTER AS COU 
                                                JOIN SYS_HALL AS HA
                                                ON COU.HALL_NO=HA.HALL_NO
                                                JOIN SYS_USERORGANIZE AS US
                                                ON HA.ORG_ID = US.ORG_ID 
                                                WHERE US.[USER_ID]=@0 ",userId);
            if (!string.IsNullOrEmpty(hallNo))
            {
                sql.Append(@" AND COU.HALL_NO like @0 ", "%" + hallNo + "%");
            }
            if (!string.IsNullOrEmpty(hallName))
            {
                sql.Append(@" AND HA.HALL_NAM like @0 ", "%" + hallName + "%");
            }

            return db.Page<CounterFuncs>(pageIndex, pageSize, sql);
        }

        public void Delete(string hallNo, int counterId)
        {
            var db = gzswDB.GetInstance();
            var sql = Sql.Builder.Append(@"DELETE FROM CHK_COUNTER WHERE HALL_NO=@0 AND COUNTER_ID=@1", hallNo,
                counterId);

            db.Execute(sql);
        }

        public CHK_COUNTER GetCounterByHallNoCounterId(string hallNo, int counterId)
        {
            var db = gzswDB.GetInstance();

            var sql = Sql.Builder.Append(@"SELECT 
                                            COU.HALL_NO
                                          , HA.HALL_NAM
                                          ,COU.[COUNTER_ID]
                                          ,COU.[MODIFY_ID]
                                          ,COU.[MODIFY_DTIME]
                                          ,COU.[NOTE]
                                          ,COU.[W1A_STAFF_ID]
                                          ,COU.[W2A_STAFF_ID]
                                          ,COU.[W3A_STAFF_ID]
                                          ,COU.[W4A_STAFF_ID]
                                          ,COU.[W5A_STAFF_ID]
                                          ,COU.[W6A_STAFF_ID]
                                          ,COU.[W7A_STAFF_ID]
                                          ,COU.[W1P_STAFF_ID]
                                          ,COU.[W2P_STAFF_ID]
                                          ,COU.[W3P_STAFF_ID]
                                          ,COU.[W4P_STAFF_ID]
                                          ,COU.[W5P_STAFF_ID]
                                          ,COU.[W6P_STAFF_ID]
                                          ,COU.[W7P_STAFF_ID]   
                                    FROM CHK_COUNTER AS COU 
                                    JOIN SYS_HALL AS HA
                                    ON COU.HALL_NO=HA.HALL_NO 
                                    WHERE  COU.HALL_NO=@0 AND COU.COUNTER_ID=@1  ", hallNo, counterId);


            return db.FirstOrDefault<CHK_COUNTER>(sql);
        }

        public int Update(CHK_COUNTER item)
        {
            var db = gzswDB.GetInstance();
            var sql = Sql.Builder.Append(@"UPDATE CHK_COUNTER 
                                        SET    [MODIFY_ID] = @0
                                              ,[MODIFY_DTIME] = @1
                                              ,[NOTE] = @2
                                              ,[W1A_STAFF_ID] = @3
                                              ,[W2A_STAFF_ID] = @4
                                              ,[W3A_STAFF_ID] = @5
                                              ,[W4A_STAFF_ID] = @6
                                              ,[W5A_STAFF_ID] = @7
                                              ,[W6A_STAFF_ID] = @8
                                              ,[W7A_STAFF_ID] = @9
                                              ,[W1P_STAFF_ID] = @10
                                              ,[W2P_STAFF_ID] = @11
                                              ,[W3P_STAFF_ID] = @12
                                              ,[W4P_STAFF_ID] = @13
                                              ,[W5P_STAFF_ID] = @14
                                              ,[W6P_STAFF_ID] = @15
                                              ,[W7P_STAFF_ID] = @16
                                         WHERE HALL_NO=@17 AND COUNTER_ID=@18 ",
                                          item.MODIFY_ID,
                                          item.MODIFY_DTIME,
                                          item.NOTE,
                                          item.W1A_STAFF_ID,
                                          item.W2A_STAFF_ID,
                                          item.W3A_STAFF_ID,
                                          item.W4A_STAFF_ID,
                                          item.W5A_STAFF_ID,
                                          item.W6A_STAFF_ID,
                                          item.W7A_STAFF_ID,
                                          item.W1P_STAFF_ID,
                                          item.W2P_STAFF_ID,
                                          item.W3P_STAFF_ID,
                                          item.W4P_STAFF_ID,
                                          item.W5P_STAFF_ID,
                                          item.W6P_STAFF_ID,
                                          item.W7P_STAFF_ID,
                                          item.HALL_NO,
                                          item.COUNTER_ID);

            return db.Execute(sql);
        }
    }
}
