using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gzsw.model;
using gzsw.model.Subclasses;
using PetaPoco;

namespace gzsw.dal.dao
{
    public static class STAT_STAFF_SVRSTAT_M_DAL
    {
        public static Page<STAT_STAFF_SVRSTAT_M_SUB> GetListBub(string staffName, int STAT_MO, string orgId, int pageIndex, int pageSize)
        {
            var db = gzswDB.GetInstance();
            var sql = Sql.Builder.Append(@"SELECT S.[STAT_MO]
                                              ,S.[STAFF_ID]
                                              ,ST.[STAFF_NAM]
                                              ,S.[SERIALID]
                                              ,D.SERIALNAME
                                              ,S.[MODIFY_ID]
                                              ,S.[MODIFY_DTIME]
                                              ,S.[DOOR_SVR_CNT]
                                              ,S.[OTHER_SVR_CNT]
                                              ,S.[COR_DOOR_SVR_CNT]
                                              ,S.[OVERTIME_SVR_CNT]
                                              ,S.[COR_OVERTIME_SVR_CNT]
                                          FROM [STAT_STAFF_SVRSTAT_M] AS S
	                                        JOIN SYS_STAFF AS ST
	                                        ON S.STAFF_ID=ST.STAFF_ID 
                                            JOIN SYS_DETAILSERIAL AS D
                                            ON S.SERIALID=D.SERIALID
	                                        WHERE 1=1 ");

            sql.Append(@" AND ST.[ORG_ID]=@0 AND S.[STAT_MO]=@1 ", orgId, STAT_MO);

            if (!string.IsNullOrEmpty(staffName))
            {
                sql.Append(@" AND ST.[STAFF_NAM] like @0 ", "%" + staffName + "%");
            }

            return db.Page<STAT_STAFF_SVRSTAT_M_SUB>(pageIndex, pageSize,sql);
        }

        public static STAT_STAFF_SVRSTAT_M_SUB GetSub(int STAT_MO, string staffId, string serialId)
        {
            var db = gzswDB.GetInstance();
            var sql = Sql.Builder.Append(@"SELECT S.[STAT_MO]
                                              ,S.[STAFF_ID]
                                              ,ST.[STAFF_NAM]
                                              ,S.[SERIALID]
                                              ,D.SERIALNAME
                                              ,S.[MODIFY_ID]
                                              ,S.[MODIFY_DTIME]
                                              ,S.[DOOR_SVR_CNT]
                                              ,S.[OTHER_SVR_CNT]
                                              ,S.[COR_DOOR_SVR_CNT]
                                              ,S.[OVERTIME_SVR_CNT]
                                              ,S.[COR_OVERTIME_SVR_CNT]
                                          FROM [STAT_STAFF_SVRSTAT_M] AS S
	                                        JOIN SYS_STAFF AS ST
	                                        ON S.STAFF_ID=ST.STAFF_ID 
                                            JOIN SYS_DETAILSERIAL AS D
                                            ON S.SERIALID=D.SERIALID
	                                        WHERE 1=1 ");

            sql.Append(@" AND S.[STAFF_ID]=@0 AND S.[STAT_MO]=@1 AND S.SERIALID=@2 ", staffId, STAT_MO, serialId);

            return db.FirstOrDefault<STAT_STAFF_SVRSTAT_M_SUB>(sql);
        }

        public static int Update(STAT_STAFF_SVRSTAT_M item)
        {
            var db = gzswDB.GetInstance();
            var sql = Sql.Builder.Append(@"UPDATE [STAT_STAFF_SVRSTAT_M]
                                                   SET [MODIFY_ID] = @0
                                                      ,[MODIFY_DTIME] = @1
                                                      ,[DOOR_SVR_CNT] = @2
                                                      ,[OTHER_SVR_CNT] = @3
                                                      ,[COR_DOOR_SVR_CNT] = @4
                                                      ,[OVERTIME_SVR_CNT] = @5
                                                      ,[COR_OVERTIME_SVR_CNT] = @6
                                                 WHERE [STAT_MO] = @7
                                                      AND [STAFF_ID] = @8 AND SERIALID=@9 ",
                                                 item.MODIFY_ID,
                                                 item.MODIFY_DTIME,
                                                 item.DOOR_SVR_CNT,
                                                 item.OTHER_SVR_CNT,
                                                 item.COR_DOOR_SVR_CNT,
                                                 item.OVERTIME_SVR_CNT,
                                                 item.COR_OVERTIME_SVR_CNT,
                                                 item.STAT_MO,
                                                 item.STAFF_ID,
                                                 item.SERIALID);

            return db.Execute(sql);
        }

        /// <summary>
        /// 用于综合评定查看详细
        /// </summary>
        /// <param name="beginStatMo"></param>
        /// <param name="endStatMo"></param>
        /// <param name="orgId"></param>
        /// <param name="staffId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static Page<STAT_STAFF_SVRSTAT_M_SUB> GetListBub(int beginStatMo, int endStatMo, 
            string staffId, int pageIndex, int pageSize)
        {
            var db = gzswDB.GetInstance();
            var sql = Sql.Builder.Append(@"SELECT S.[STAT_MO]
                                              ,S.[STAFF_ID]
                                              ,ST.[STAFF_NAM]
                                              ,S.[SERIALID]
                                              ,D.SERIALNAME
                                              ,S.[MODIFY_ID]
                                              ,S.[MODIFY_DTIME]
                                              ,S.[DOOR_SVR_CNT]
                                              ,S.[OTHER_SVR_CNT]
                                              ,S.[COR_DOOR_SVR_CNT]
                                              ,S.[OVERTIME_SVR_CNT]
                                              ,S.[COR_OVERTIME_SVR_CNT]
                                          FROM [STAT_STAFF_SVRSTAT_M] AS S
	                                        JOIN SYS_STAFF AS ST
	                                        ON S.STAFF_ID=ST.STAFF_ID 
                                            JOIN SYS_DETAILSERIAL AS D
                                            ON S.SERIALID=D.SERIALID
	                                        WHERE 1=1 ");

            sql.Append(@" AND  S.[STAFF_ID]=@0 ", staffId);
            sql.Append(@" AND S.STAT_MO>=@0 AND S.STAT_MO<=@1 ", beginStatMo, endStatMo);

            return db.Page<STAT_STAFF_SVRSTAT_M_SUB>(pageIndex, pageSize, sql);
        }

        /// <summary>
        /// 获取综合评定的总数
        /// </summary>
        /// <param name="beginStatMo"></param>
        /// <param name="endStatMo"></param>
        /// <param name="staffId"></param>
        /// <returns></returns>
        public static dynamic GetTotal(int beginStatMo, int endStatMo,
            string staffId)
        {
            var db = gzswDB.GetInstance();
            var sql = Sql.Builder.Append(@"SELECT SUM(S.COR_DOOR_SVR_CNT) AS DOOR_SVR_CNT_TOTAL 
                                            ,SUM(S.OTHER_SVR_CNT) AS OTHER_SVR_CNT_TOTAL
                                            ,SUM(S.COR_OVERTIME_SVR_CNT) AS OVERTIME_SVR_CNT_TOTAL
                                          FROM [STAT_STAFF_SVRSTAT_M] AS S
	                                        WHERE 1=1 ");

            sql.Append(@" AND  S.[STAFF_ID]=@0 ", staffId);
            sql.Append(@" AND S.STAT_MO>=@0 AND S.STAT_MO<=@1 ", beginStatMo, endStatMo);

            return db.FirstOrDefault<dynamic>(sql);
        }
    }
}
