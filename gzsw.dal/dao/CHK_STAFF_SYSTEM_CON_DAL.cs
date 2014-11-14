using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gzsw.model;
using PetaPoco;

namespace gzsw.dal.dao
{
    public static class CHK_STAFF_SYSTEM_CON_DAL
    {
        public static Page<CHK_STAFF_SYSTEM_CON> GetList(string orgId,string userId,int pageIndex,int pageSize)
        {
            var db = gzswDB.GetInstance();
            var sql = Sql.Builder.Append(@"SELECT S.[ORG_ID]
	                                      ,O.[ORG_NAM]
                                          ,S.[COMPRE_SAN_TYP]
                                          ,S.[COMPRE_SAN_100_MAX_SCORE]
                                          ,S.[EVAL_CHK_RT]
                                          ,S.[SVR_CHK_RT]
                                          ,S.[QUALITY_CHK_RT]
                                          ,S.[EFFIC_CHK_RT]
                                          ,S.[EFFIC_AVOID_RT]
                                          ,S.[EFFIC_AVOID_EXC_DEDUCT]
                                          ,S.[ATTEND_CHK_RT]
                                          ,S.[USU_ACT_CHK_RT]
                                      FROM [CHK_STAFF_SYSTEM_CON] AS S
                                      JOIN SYS_ORGANIZE AS O
                                      ON S.ORG_ID=O.ORG_ID
                                      JOIN SYS_USERORGANIZE AS U
                                      ON S.ORG_ID=U.ORG_ID 
                                      WHERE U.[USER_ID]=@0  ", userId);

            if (!string.IsNullOrEmpty(orgId))
            {
                sql.Append(@" AND S.ORG_ID = @0 ", orgId);
            }

            return db.Page<CHK_STAFF_SYSTEM_CON>(pageIndex, pageSize, sql);
        }

        public static CHK_STAFF_SYSTEM_CON Get(string orgId)
        {
            var db = gzswDB.GetInstance();
            var sql = Sql.Builder.Append(@"SELECT S.[ORG_ID]
	                                      ,O.[ORG_NAM]
                                          ,S.[COMPRE_SAN_TYP]
                                          ,S.[COMPRE_SAN_100_MAX_SCORE]
                                          ,S.[EVAL_CHK_RT]
                                          ,S.[SVR_CHK_RT]
                                          ,S.[QUALITY_CHK_RT]
                                          ,S.[EFFIC_CHK_RT]
                                          ,S.[EFFIC_AVOID_RT]
                                          ,S.[EFFIC_AVOID_EXC_DEDUCT]
                                          ,S.[ATTEND_CHK_RT]
                                          ,S.[USU_ACT_CHK_RT]
                                      FROM [CHK_STAFF_SYSTEM_CON] AS S
                                      JOIN SYS_ORGANIZE AS O
                                      ON S.ORG_ID=O.ORG_ID 
                                      WHERE S.ORG_ID = @0  ", orgId);


            return db.FirstOrDefault<CHK_STAFF_SYSTEM_CON>(sql);
        }
    }
}
