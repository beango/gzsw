using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gzsw.model;
using PetaPoco;

namespace gzsw.dal.dao
{
    public static class CHK_STAFF_USU_ACT_CON_DAL
    {
        public static Page<CHK_STAFF_USU_ACT_CON> GetList(string orgId,string userId, int pageIndex, int pageSize)
        {
            var db = gzswDB.GetInstance();

            var sql = Sql.Builder.Append(@"SELECT CHK.[ORG_ID]
	                                      ,SY.[ORG_NAM]
                                          ,CHK.[WORK_DIS_RT]
                                          ,CHK.[TAX_SVR_RT]
                                          ,CHK.[TAX_LOOK_RT]
                                          ,CHK.[TRAIN_RT]
                                          ,CHK.[SEC_HEAL_RT]
                                          ,CHK.[OTHER_RT]
                                      FROM [CHK_STAFF_USU_ACT_CON] AS CHK
                                      JOIN SYS_ORGANIZE AS SY
                                      ON CHK.ORG_ID=SY.ORG_ID
                                      JOIN SYS_USERORGANIZE AS UE
                                      ON CHK.ORG_ID = UE.ORG_ID 
                                      WHERE  UE.[USER_ID]=@0 ",userId);
            if (!string.IsNullOrEmpty(orgId))
            {
                sql.Append(@" AND CHK.ORG_ID = @0 ", orgId);
            }

            return db.Page<CHK_STAFF_USU_ACT_CON>(pageIndex, pageSize, sql);
        }

        public static CHK_STAFF_USU_ACT_CON Get(string orgId)
        {
            var db = gzswDB.GetInstance();

            var sql = Sql.Builder.Append(@"SELECT CHK.[ORG_ID]
	                                      ,SY.[ORG_NAM]
                                          ,CHK.[WORK_DIS_RT]
                                          ,CHK.[TAX_SVR_RT]
                                          ,CHK.[TAX_LOOK_RT]
                                          ,CHK.[TRAIN_RT]
                                          ,CHK.[SEC_HEAL_RT]
                                          ,CHK.[OTHER_RT]
                                      FROM [CHK_STAFF_USU_ACT_CON] AS CHK
                                      JOIN SYS_ORGANIZE AS SY
                                      ON CHK.ORG_ID=SY.ORG_ID ");
            sql.Append(@" WHERE CHK.ORG_ID = @0 ", orgId);

            return db.FirstOrDefault<CHK_STAFF_USU_ACT_CON>(sql);
        }
    }
}
