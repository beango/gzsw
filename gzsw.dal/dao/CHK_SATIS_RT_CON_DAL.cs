using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gzsw.model;
using PetaPoco;

namespace gzsw.dal.dao
{
    public static class CHK_SATIS_RT_CON_DAL
    {
        public static Page<CHK_SATIS_RT_CON> GetList(string orgId, string userId, int pageIndex, int pageSize)
        {
            var db = gzswDB.GetInstance();
            var sql = Sql.Builder.Append(@"SELECT S.[ORG_ID]
	                                          ,O.[ORG_NAM]
                                              ,S.[VERY_SATISFY_SCORE]
                                              ,S.[SATISFY_SCORE]
                                              ,S.[COMMON_SCORE]
                                              ,S.[UNSATISFY_SCORE]
                                              ,S.[NON_EVAL_SCORE]
                                          FROM [CHK_SATIS_RT_CON] AS S
                                          JOIN SYS_ORGANIZE AS O
                                          ON S.ORG_ID=O.ORG_ID
                                          JOIN SYS_USERORGANIZE AS U
                                          ON S.ORG_ID=U.ORG_ID 
                                          WHERE U.[User_ID]=@0  ", userId);
            if (!string.IsNullOrEmpty(orgId))
            {
                sql.Append(@" AND S.ORG_ID=@0 ", orgId);
            }

            return db.Page<CHK_SATIS_RT_CON>(pageIndex, pageSize, sql);
        }

        public static CHK_SATIS_RT_CON Get(string orgId)
        {
            var db = gzswDB.GetInstance();
            var sql = Sql.Builder.Append(@"SELECT S.[ORG_ID]
	                                          ,O.[ORG_NAM]
                                              ,S.[VERY_SATISFY_SCORE]
                                              ,S.[SATISFY_SCORE]
                                              ,S.[COMMON_SCORE]
                                              ,S.[UNSATISFY_SCORE]
                                              ,S.[NON_EVAL_SCORE]
                                          FROM [CHK_SATIS_RT_CON] AS S
                                          JOIN SYS_ORGANIZE AS O
                                          ON S.ORG_ID=O.ORG_ID 
                                          WHERE  S.[ORG_ID]=@0  ",orgId);
            return db.FirstOrDefault<CHK_SATIS_RT_CON>(sql);
        }
    }
}
