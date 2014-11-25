using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gzsw.model.Subclasses;
using PetaPoco;
using gzsw.model;

namespace gzsw.dal.dao
{
    /// <summary>
    /// 个人考核评定
    ///     综合评定
    /// </summary>
    public static class CHK_STAFF_COMPRE_EVAL_M_DAL
    {
        public static Page<CHK_STAFF_COMPRE_EVAL_M_SUB> GetListSub(int STAT_MO,int endStatMo, string orgId, int pageIndex,int pageSize)
        {
            var db = gzswDB.GetInstance();
            var sql = Sql.Builder.Append(@"");

            if (STAT_MO != endStatMo)
            {
                sql.Append(@" EXECUTE PRO_GET_CHK_STAFF @@HALL_NO=@0 ,@@START_MO=@1,@@END_MO=@2 ",orgId, STAT_MO, endStatMo);

                var list=db.Fetch<CHK_STAFF_COMPRE_EVAL_M_SUB>(sql);
                return new Page<CHK_STAFF_COMPRE_EVAL_M_SUB>()
                       {
                           Items = list.Skip((pageIndex-1)*pageSize).Take(pageSize).ToList(),
                           CurrentPage = pageIndex,
                           TotalItems = list.Count,
                           ItemsPerPage = pageSize
                       };
            }
            else
            {

                sql.Append(@"SELECT * FROM (SELECT 
                                            C.[HALL_NO]
                                          ,C.[STAFF_ID]
                                          ,SUM(C.[COMPRE_SAN_SCORE])/ COUNT(*) as  COMPRE_SAN_SCORE
                                          ,SUM(C.[EVAL_SCORE])/ COUNT(*) as  EVAL_SCORE
                                          ,SUM(C.[SVR_SCORE])/ COUNT(*) as  SVR_SCORE
                                          ,SUM(C.[QUALITY_SCORE])/ COUNT(*) as  QUALITY_SCORE
                                          ,SUM(C.[EFFIC_SCORE])/ COUNT(*) as  EFFIC_SCORE
                                          ,SUM(C.[ATTEND_SCORE])/ COUNT(*) as  ATTEND_SCORE
                                          ,SUM(C.[USU_ACT_SCORE])/ COUNT(*) as  USU_ACT_SCORE
                                          ,SUM(C.[TOT_SCORE])/ COUNT(*) as  TOT_SCORE
                                          ,SUM(C.[STAR_LEVEL]) / COUNT(*) as STAR_LEVEL
                                          ,ST.STAFF_NAM
                                      FROM [CHK_STAFF_COMPRE_EVAL_M] AS C
                                      JOIN SYS_STAFF AS ST
                                      ON C.STAFF_ID = ST.STAFF_ID 
                                      WHERE 1=1  ");

                sql.Append(@" AND  C.HALL_NO=@0 ", orgId);
                sql.Append(@" AND (( C.STAT_MO>=@0 AND C.STAT_MO<=@1 ) OR C.STAT_MO=@2 ) ", STAT_MO, endStatMo, STAT_MO);
                sql.Append(@" group by C.[HALL_NO],C.[STAFF_ID],ST.STAFF_NAM");
                sql.Append(@" ) T1 ");
                return db.Page<CHK_STAFF_COMPRE_EVAL_M_SUB>(pageIndex, pageSize, sql);
            }
        }

        /// <summary>
        /// 获取总的综合分
        /// </summary>
        /// <param name="STAT_MO"></param>
        /// <param name="orgId"></param>
        /// <returns></returns>
        public static dynamic GetListTotalScore(int STAT_MO,int endstat, string orgId)
        {
            var db = gzswDB.GetInstance();
            var sql = Sql.Builder.Append(@"SELECT 
                                          SUM(C.[TOT_SCORE]) AS Total,
                                          COUNT(*) as [Count]
                                      FROM [CHK_STAFF_COMPRE_EVAL_M] AS C
                                      JOIN SYS_STAFF AS ST
                                      ON C.STAFF_ID = ST.STAFF_ID 
                                      WHERE 1=1  ");

            sql.Append(@" AND  C.HALL_NO=@0 ", orgId);


            sql.Append(@" AND (( C.STAT_MO>=@0 AND C.STAT_MO<=@1 ) OR C.STAT_MO=@2 ) ", STAT_MO, endstat, STAT_MO);


            return db.FirstOrDefault<dynamic>(sql);
        }
    }
}
