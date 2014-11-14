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
    /// 个人考核考勤扣分设置
    /// </summary>
    public  class CHK_STAFF_ATTEND_DEDUCT_CON_DAL
    {
        public static Page<CHK_STAFF_ATTEND_DEDUCT_CON_SUB> GetList(string orgId, string userId, int pageIndex, int pageSize)
        {
            var db = gzswDB.GetInstance();
            var sql = Sql.Builder.Append(@"SELECT S.[ORG_ID]
	                                          ,O.[ORG_NAM]
                                              ,S.[EAR_SCORE]
                                              ,S.[NEG_SCORE]
                                              ,S.[LAT_SCORE]
                                              ,S.[MODIFY_ID]
                                              ,S.[MODIFY_DTIME]
                                          FROM [CHK_STAFF_ATTEND_DEDUCT_CON] AS S
                                          JOIN SYS_ORGANIZE AS O
                                          ON S.ORG_ID=O.ORG_ID
                                          JOIN SYS_USERORGANIZE AS U
                                          ON S.ORG_ID=U.ORG_ID 
                                          WHERE U.[User_ID]=@0  ", userId);
            if (!string.IsNullOrEmpty(orgId))
            {
                sql.Append(@" AND S.ORG_ID=@0 ", orgId);
            }

            return db.Page<CHK_STAFF_ATTEND_DEDUCT_CON_SUB>(pageIndex, pageSize, sql);
        }


        public static CHK_STAFF_ATTEND_DEDUCT_CON_SUB Get(string orgId)
        {
            var db = gzswDB.GetInstance();
            var sql = Sql.Builder.Append(@"SELECT S.[ORG_ID]
	                                          ,O.[ORG_NAM]
                                              ,S.[EAR_SCORE]
                                              ,S.[NEG_SCORE]
                                              ,S.[LAT_SCORE]
                                              ,S.[MODIFY_ID]
                                              ,S.[MODIFY_DTIME]
                                          FROM [CHK_STAFF_ATTEND_DEDUCT_CON] AS S
                                          JOIN SYS_ORGANIZE AS O
                                          ON S.ORG_ID=O.ORG_ID
                                          WHERE 1=1  ");

                sql.Append(@" AND S.ORG_ID=@0 ", orgId);

            return db.FirstOrDefault<CHK_STAFF_ATTEND_DEDUCT_CON_SUB>(sql);
        }
    }
}
