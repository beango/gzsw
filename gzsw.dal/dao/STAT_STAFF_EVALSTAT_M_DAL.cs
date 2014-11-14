using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gzsw.model;
using gzsw.model.Subclasses;
using PetaPoco;

namespace gzsw.dal.dao
{
    public static class STAT_STAFF_EVALSTAT_M_DAL
    {
        public static Page<STAT_STAFF_EVALSTAT_M_SUB> GetListSub(int STAT_MO, string orgId, int pageIndex, int pageSize)
        {
            var db = gzswDB.GetInstance();

            var sql = Sql.Builder.Append(@"SELECT S.[STAT_MO]
                                              ,S.[STAFF_ID]
                                              ,ST.[STAFF_NAM]
                                              ,S.[VERY_SATISFY_CNT]
                                              ,S.[SATISFY_CNT]
                                              ,S.[COMMON_CNT]
                                              ,S.[UNSATISFY_CNT]
                                              ,S.[NON_EVAL_CNT]
                                              ,S.[COR_VERY_SATISFY_CNT]
                                              ,S.[COR_SATISFY_CNT]
                                              ,S.[COR_COMMON_CNT]
                                              ,S.[COR_UNSATISFY_CNT]
                                              ,S.[COR_NON_EVAL_CNT]
                                              ,S.[MODIFY_ID]
                                              ,S.[MODIFY_DTIME]
                                          FROM [STAT_STAFF_EVALSTAT_M] AS S
	                                        JOIN SYS_STAFF AS ST
	                                        ON S.STAFF_ID=ST.STAFF_ID WHERE 1=1");

            sql.Append(@" AND ST.[ORG_ID]=@0 AND S.[STAT_MO]=@1 ", orgId, STAT_MO);

            return db.Page<STAT_STAFF_EVALSTAT_M_SUB>(pageIndex, pageSize, sql);
        }

        public static STAT_STAFF_EVALSTAT_M_SUB GetSub(int STAT_MO, string staffId)
        {
            var db = gzswDB.GetInstance();

            var sql = Sql.Builder.Append(@"SELECT S.[STAT_MO]
                                              ,S.[STAFF_ID]
                                              ,ST.[STAFF_NAM]
                                              ,S.[VERY_SATISFY_CNT]
                                              ,S.[SATISFY_CNT]
                                              ,S.[COMMON_CNT]
                                              ,S.[UNSATISFY_CNT]
                                              ,S.[NON_EVAL_CNT]
                                              ,S.[COR_VERY_SATISFY_CNT]
                                              ,S.[COR_SATISFY_CNT]
                                              ,S.[COR_COMMON_CNT]
                                              ,S.[COR_UNSATISFY_CNT]
                                              ,S.[COR_NON_EVAL_CNT]
                                              ,S.[MODIFY_ID]
                                              ,S.[MODIFY_DTIME]
                                          FROM [STAT_STAFF_EVALSTAT_M] AS S
	                                        JOIN SYS_STAFF AS ST
	                                        ON S.STAFF_ID=ST.STAFF_ID WHERE 1=1");

            sql.Append(@" AND S.[STAFF_ID]=@0 AND S.[STAT_MO]=@1 ", staffId, STAT_MO);

            return db.FirstOrDefault<STAT_STAFF_EVALSTAT_M_SUB>(sql);
        }

        public static int Update(STAT_STAFF_EVALSTAT_M item)
        {
            var db = gzswDB.GetInstance();
            var sql = Sql.Builder.Append(@"UPDATE [STAT_STAFF_EVALSTAT_M]
                                               SET 
                                                  [VERY_SATISFY_CNT] = @0
                                                  ,[SATISFY_CNT] = @1
                                                  ,[COMMON_CNT] = @2
                                                  ,[UNSATISFY_CNT] = @3
                                                  ,[NON_EVAL_CNT] = @4
                                                  ,[COR_VERY_SATISFY_CNT] = @5
                                                  ,[COR_SATISFY_CNT] = @6
                                                  ,[COR_COMMON_CNT] = @7
                                                  ,[COR_UNSATISFY_CNT] = @8
                                                  ,[COR_NON_EVAL_CNT] = @9
                                                  ,[MODIFY_ID] = @10
                                                  ,[MODIFY_DTIME] = @11
                                             WHERE [STAT_MO] = @12 
                                                  AND [STAFF_ID] = @13 ",
                                              item.VERY_SATISFY_CNT,
                                              item.SATISFY_CNT,
                                              item.COMMON_CNT,
                                              item.UNSATISFY_CNT,
                                              item.NON_EVAL_CNT,
                                              item.COR_VERY_SATISFY_CNT,
                                              item.COR_SATISFY_CNT,
                                              item.COR_COMMON_CNT,
                                              item.COR_UNSATISFY_CNT,
                                              item.COR_NON_EVAL_CNT,
                                              item.MODIFY_ID,
                                              item.MODIFY_DTIME,
                                              item.STAT_MO,
                                              item.STAFF_ID);

            return db.Execute(sql);
        }

        public static Page<STAT_STAFF_EVALSTAT_M_SUB> GetListSub(int beginStatMo, int endStatMo,
            string staffId, int pageIndex, int pageSize)
        {
            var db = gzswDB.GetInstance();

            var sql = Sql.Builder.Append(@"SELECT S.[STAT_MO]
                                              ,S.[STAFF_ID]
                                              ,ST.[STAFF_NAM]
                                              ,S.[VERY_SATISFY_CNT]
                                              ,S.[SATISFY_CNT]
                                              ,S.[COMMON_CNT]
                                              ,S.[UNSATISFY_CNT]
                                              ,S.[NON_EVAL_CNT]
                                              ,S.[COR_VERY_SATISFY_CNT]
                                              ,S.[COR_SATISFY_CNT]
                                              ,S.[COR_COMMON_CNT]
                                              ,S.[COR_UNSATISFY_CNT]
                                              ,S.[COR_NON_EVAL_CNT]
                                              ,S.[MODIFY_ID]
                                              ,S.[MODIFY_DTIME]
                                          FROM [STAT_STAFF_EVALSTAT_M] AS S
	                                        JOIN SYS_STAFF AS ST
	                                        ON S.STAFF_ID=ST.STAFF_ID WHERE 1=1");

            sql.Append(@" AND S.[STAFF_ID]=@0 ", staffId);
            sql.Append(@" AND S.STAT_MO>=@0 AND S.STAT_MO<=@1 ", beginStatMo, endStatMo);

            return db.Page<STAT_STAFF_EVALSTAT_M_SUB>(pageIndex, pageSize, sql);
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
            var sql = Sql.Builder.Append(@"SELECT SUM(S.[COR_VERY_SATISFY_CNT]) AS VERY_SATISFY_CNT_TOTAL
                                              ,SUM(S.[COR_SATISFY_CNT]) AS SATISFY_CNT_TOTAL
                                              ,SUM(S.[COR_COMMON_CNT]) AS COMMON_CNT_TOTAL
                                                ,SUM(S.[COR_UNSATISFY_CNT]) UNSATISFY_CNT_TOTAL
                                                ,SUM(S.[COR_NON_EVAL_CNT]) AS NON_EVAL_CNT_TOTAL
                                          FROM [STAT_STAFF_EVALSTAT_M] AS S
	                                        WHERE 1=1 ");

            sql.Append(@" AND S.[STAFF_ID]=@0 ", staffId);
            sql.Append(@" AND S.STAT_MO>=@0 AND S.STAT_MO<=@1 ", beginStatMo, endStatMo);

            return db.FirstOrDefault<dynamic>(sql);
        }
    }
}
