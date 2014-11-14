using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gzsw.model.Subclasses;
using PetaPoco;
using gzsw.model;

namespace gzsw.dal.dao
{
    public static class STAT_STAFF_QUALITYSTAT_M_DAL
    {
        public static Page<STAT_STAFF_QUALITYSTAT_M_SUB> GetListBub(int STAT_MO, string orgId, int pageIndex, int pageSize)
        {
            var db = gzswDB.GetInstance();
            var sql = Sql.Builder.Append(@"SELECT S.[STAT_MO]
                                                  ,S.[STAFF_ID]
                                                  ,ST.[STAFF_NAM]
                                                  ,S.[QUALITY_CD]
                                                  ,Q.[QUALITY_NAM]
                                                  ,S.[MODIFY_ID]
                                                  ,S.[MODIFY_DTIME]
                                                  ,S.[ERROR_SVR_CNT]
                                                  ,S.[COR_ERROR_SVR_CNT]
                                              FROM [STAT_STAFF_QUALITYSTAT_M] AS S
	                                            JOIN SYS_STAFF AS ST
	                                            ON S.STAFF_ID=ST.STAFF_ID
	                                            JOIN CHK_QUALITY_CON AS Q
	                                            ON S.[QUALITY_CD]=Q.[QUALITY_CD]
	                                            WHERE 1=1 ");

            sql.Append(@" AND ST.[ORG_ID]=@0 AND S.[STAT_MO]=@1 ", orgId, STAT_MO);

            return db.Page<STAT_STAFF_QUALITYSTAT_M_SUB>(pageIndex, pageSize, sql);
        }

        public static STAT_STAFF_QUALITYSTAT_M_SUB GetSub(int STAT_MO, string staffId, string qualityCd)
        {
            var db = gzswDB.GetInstance();
            var sql = Sql.Builder.Append(@"SELECT S.[STAT_MO]
                                                  ,S.[STAFF_ID]
                                                  ,ST.[STAFF_NAM]
                                                  ,S.[QUALITY_CD]
                                                  ,Q.[QUALITY_NAM]
                                                  ,S.[MODIFY_ID]
                                                  ,S.[MODIFY_DTIME]
                                                  ,S.[ERROR_SVR_CNT]
                                                  ,S.[COR_ERROR_SVR_CNT]
                                              FROM [STAT_STAFF_QUALITYSTAT_M] AS S
	                                            JOIN SYS_STAFF AS ST
	                                            ON S.STAFF_ID=ST.STAFF_ID
	                                            JOIN CHK_QUALITY_CON AS Q
	                                            ON S.[QUALITY_CD]=Q.[QUALITY_CD]
	                                            WHERE 1=1 ");

            sql.Append(@" AND S.[STAFF_ID]=@0 AND S.[STAT_MO]=@1 AND S.[QUALITY_CD]=@2 ", staffId, STAT_MO, qualityCd);

            return db.FirstOrDefault<STAT_STAFF_QUALITYSTAT_M_SUB>(sql);
        }

        public static Page<STAT_STAFF_QUALITYSTAT_M_SUB> GetListBub(int beginStatMo, int endStatMo,
            string staffId, int pageIndex, int pageSize)
        {
            var db = gzswDB.GetInstance();
            var sql = Sql.Builder.Append(@"SELECT S.[STAT_MO]
                                                  ,S.[STAFF_ID]
                                                  ,ST.[STAFF_NAM]
                                                  ,S.[QUALITY_CD]
                                                  ,Q.[QUALITY_NAM]
                                                  ,S.[MODIFY_ID]
                                                  ,S.[MODIFY_DTIME]
                                                  ,S.[ERROR_SVR_CNT]
                                                  ,S.[COR_ERROR_SVR_CNT]
                                              FROM [STAT_STAFF_QUALITYSTAT_M] AS S
	                                            JOIN SYS_STAFF AS ST
	                                            ON S.STAFF_ID=ST.STAFF_ID
	                                            JOIN CHK_QUALITY_CON AS Q
	                                            ON S.[QUALITY_CD]=Q.[QUALITY_CD]
	                                            WHERE 1=1 ");

            sql.Append(@" AND S.[STAFF_ID]=@0 ", staffId);
            sql.Append(@" AND S.STAT_MO>=@0 AND S.STAT_MO<=@1 ", beginStatMo, endStatMo);

            return db.Page<STAT_STAFF_QUALITYSTAT_M_SUB>(pageIndex, pageSize, sql);
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
            var sql = Sql.Builder.Append(@"SELECT SUM(S.COR_ERROR_SVR_CNT) AS ERROR_SVR_CNT_TOTAL
                                              FROM [STAT_STAFF_QUALITYSTAT_M] AS S
	                                        WHERE 1=1 ");

            sql.Append(@" AND S.[STAFF_ID]=@0 ", staffId);
            sql.Append(@" AND S.STAT_MO>=@0 AND S.STAT_MO<=@1 ", beginStatMo, endStatMo);

            return db.FirstOrDefault<dynamic>(sql);
        }
    }
}
