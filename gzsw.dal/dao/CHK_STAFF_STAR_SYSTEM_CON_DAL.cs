using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gzsw.model;
using PetaPoco;

namespace gzsw.dal.dao
{
    public static class CHK_STAFF_STAR_SYSTEM_CON_DAL
    {
        public static Page<CHK_STAFF_STAR_SYSTEM_CON> GetList(string orgId,string userId, int pageIndex,int pageSize)
        {
            var db = gzswDB.GetInstance();
            var sql = Sql.Builder.Append(@"SELECT S.[ORG_ID]
	                                          ,O.[ORG_NAM]
                                              ,S.[TIME_DUR_TYP]
                                              ,S.[STAR_5_MIN_SCORE]
                                              ,S.[STAR_4_MIN_SCORE]
                                              ,S.[STAR_3_MIN_SCORE]
                                              ,S.[STAR_2_MIN_SCORE]
                                              ,S.[STAR_1_MIN_SCORE]
                                              ,S.[STAR_4_MAX_SCORE]
                                              ,S.[STAR_3_MAX_SCORE]
                                              ,S.[STAR_2_MAX_SCORE]
                                              ,S.[STAR_1_MAX_SCORE]
                                              ,S.[EVAL_MIN_SCORE]
                                              ,S.[SVR_MIN_SCORE]
                                              ,S.[QUALITY_MIN_SCORE]
                                              ,S.[EFFIC_MIN_SCORE]
                                              ,S.[ATTEND_MIN_SCORE]
                                              ,S.[USU_ACT_MIN_SCORE]
                                              ,S.[COMPLAIN_MAX_CNT]
                                              ,S.[DEFAULT_STAR]
                                          FROM [CHK_STAFF_STAR_SYSTEM_CON] AS S
                                          JOIN SYS_ORGANIZE AS O
                                          ON S.ORG_ID=O.ORG_ID
                                          JOIN SYS_USERORGANIZE AS U
                                          ON S.ORG_ID=U.ORG_ID 
                                          WHERE U.[USER_ID]=@0 ",userId);

            if (!string.IsNullOrEmpty(orgId))
            {
                sql.Append(@" AND S.ORG_ID = @0 ", orgId);
            }

            return db.Page<CHK_STAFF_STAR_SYSTEM_CON>(pageIndex, pageSize, sql);
        }

        public static CHK_STAFF_STAR_SYSTEM_CON Get(string orgId, byte durType)
        {
            var db = gzswDB.GetInstance();
            var sql = Sql.Builder.Append(@"SELECT S.[ORG_ID]
	                                          ,O.[ORG_NAM]
                                              ,S.[TIME_DUR_TYP]
                                              ,S.[STAR_5_MIN_SCORE]
                                              ,S.[STAR_4_MIN_SCORE]
                                              ,S.[STAR_3_MIN_SCORE]
                                              ,S.[STAR_2_MIN_SCORE]
                                              ,S.[STAR_1_MIN_SCORE]
                                              ,S.[STAR_4_MAX_SCORE]
                                              ,S.[STAR_3_MAX_SCORE]
                                              ,S.[STAR_2_MAX_SCORE]
                                              ,S.[STAR_1_MAX_SCORE]
                                              ,S.[EVAL_MIN_SCORE]
                                              ,S.[SVR_MIN_SCORE]
                                              ,S.[QUALITY_MIN_SCORE]
                                              ,S.[EFFIC_MIN_SCORE]
                                              ,S.[ATTEND_MIN_SCORE]
                                              ,S.[USU_ACT_MIN_SCORE]
                                              ,S.[COMPLAIN_MAX_CNT]
                                              ,S.[DEFAULT_STAR]
                                          FROM [CHK_STAFF_STAR_SYSTEM_CON] AS S
                                          JOIN SYS_ORGANIZE AS O
                                          ON S.ORG_ID=O.ORG_ID
                                          WHERE S.ORG_ID=@0 AND S.TIME_DUR_TYP=@1 ", orgId, durType);

            return db.FirstOrDefault<CHK_STAFF_STAR_SYSTEM_CON>(sql);
        }

        public static int Delete(string orgId, byte durType)
        {
            var sql = Sql.Builder.Append(@"DELETE  CHK_STAFF_STAR_SYSTEM_CON
WHERE   ORG_ID = @0
        AND TIME_DUR_TYP = @1", orgId, durType);
            return gzswDB.GetInstance().Execute(sql);
        }

        public static int Update(CHK_STAFF_STAR_SYSTEM_CON item)
        {
            var sql = Sql.Builder.Append(@"UPDATE  CHK_STAFF_STAR_SYSTEM_CON
SET     STAR_5_MIN_SCORE = @STAR_5_MIN_SCORE ,
        STAR_4_MIN_SCORE = @STAR_4_MIN_SCORE ,
        STAR_3_MIN_SCORE = @STAR_3_MIN_SCORE ,
        STAR_2_MIN_SCORE = @STAR_2_MIN_SCORE ,
        STAR_1_MIN_SCORE = @STAR_1_MIN_SCORE ,
        STAR_4_MAX_SCORE = @STAR_4_MAX_SCORE ,
        STAR_3_MAX_SCORE = @STAR_3_MAX_SCORE ,
        STAR_2_MAX_SCORE = @STAR_2_MAX_SCORE ,
        STAR_1_MAX_SCORE = @STAR_1_MAX_SCORE ,
        EVAL_MIN_SCORE = @EVAL_MIN_SCORE ,
        SVR_MIN_SCORE = @SVR_MIN_SCORE ,
        QUALITY_MIN_SCORE = @QUALITY_MIN_SCORE ,
        EFFIC_MIN_SCORE = @EFFIC_MIN_SCORE ,
        ATTEND_MIN_SCORE = @ATTEND_MIN_SCORE ,
        USU_ACT_MIN_SCORE = @USU_ACT_MIN_SCORE ,
        COMPLAIN_MAX_CNT = @COMPLAIN_MAX_CNT ,
        DEFAULT_STAR = @DEFAULT_STAR
WHERE   ORG_ID = @ORG_ID
        AND TIME_DUR_TYP = @TIME_DUR_TYP", new
            {
                ORG_ID = item.ORG_ID,
                TIME_DUR_TYP = item.TIME_DUR_TYP,
                STAR_5_MIN_SCORE = item.STAR_5_MIN_SCORE,
                STAR_4_MIN_SCORE = item.STAR_4_MIN_SCORE,
                STAR_3_MIN_SCORE = item.STAR_3_MIN_SCORE,
                STAR_2_MIN_SCORE = item.STAR_2_MIN_SCORE,
                STAR_1_MIN_SCORE = item.STAR_1_MIN_SCORE,
                STAR_4_MAX_SCORE = item.STAR_4_MAX_SCORE,
                STAR_3_MAX_SCORE = item.STAR_3_MAX_SCORE,
                STAR_2_MAX_SCORE = item.STAR_2_MAX_SCORE,
                STAR_1_MAX_SCORE = item.STAR_1_MAX_SCORE,
                EVAL_MIN_SCORE = item.EVAL_MIN_SCORE,
                SVR_MIN_SCORE = item.SVR_MIN_SCORE,
                QUALITY_MIN_SCORE = item.QUALITY_MIN_SCORE,
                EFFIC_MIN_SCORE = item.EFFIC_MIN_SCORE,
                ATTEND_MIN_SCORE = item.ATTEND_MIN_SCORE,
                USU_ACT_MIN_SCORE =item.USU_ACT_MIN_SCORE,
                COMPLAIN_MAX_CNT =item.COMPLAIN_MAX_CNT,
                DEFAULT_STAR = item.DEFAULT_STAR
            });
            return gzswDB.GetInstance().Execute(sql);
        }
    }
}
