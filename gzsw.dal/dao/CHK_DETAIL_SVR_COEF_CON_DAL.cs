using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gzsw.model;
using PetaPoco;

namespace gzsw.dal.dao
{
    public static class CHK_DETAIL_SVR_COEF_CON_DAL
    {
        public static Page<CHK_DETAIL_SVR_COEF_CON> GetList(string orgId,string userId, int pageIndex, int pageSize)
        {
            var db = gzswDB.GetInstance();

            var sql = Sql.Builder.Append(@"SELECT DET.[SERIALID]
	                                          ,SER.[SERIALNAME]
                                              ,DET.[ORG_ID]
                                              ,ORG.[ORG_NAM]  
                                              ,DET.[COEFFICIENT]
                                              ,DET.[MODIFY_ID]
                                              ,U.USER_NAM
                                              ,DET.[MODIFY_DTIME]
                                          FROM [CHK_DETAIL_SVR_COEF_CON] AS DET
                                          JOIN SYS_DETAILSERIAL AS SER
                                          ON DET.SERIALID=SER.SERIALID
                                          JOIN SYS_ORGANIZE AS ORG
                                          ON DET.ORG_ID=ORG.ORG_ID
                                          JOIN SYS_USER AS U
                                          ON DET.MODIFY_ID=U.[USER_ID]
                                          JOIN SYS_USERORGANIZE AS UE
                                          ON DET.ORG_ID=UE.ORG_ID 
                                          WHERE UE.USER_ID = @0 ", userId);

            if (!string.IsNullOrEmpty(orgId))
            {
                sql.Append(@" AND DET.[ORG_ID] = @0 ", orgId);
            }

            return db.Page<CHK_DETAIL_SVR_COEF_CON>(pageIndex,pageSize,sql);
        }


        public static CHK_DETAIL_SVR_COEF_CON Get(string orgId, string serialId)
        {
            var db = gzswDB.GetInstance();

            var sql = Sql.Builder.Append(@"SELECT DET.[SERIALID]
	                                          ,SER.[SERIALNAME]
                                              ,DET.[ORG_ID]
                                              ,ORG.[ORG_NAM]  
                                              ,DET.[COEFFICIENT]
                                              ,DET.[MODIFY_ID]
                                              ,U.USER_NAM
                                              ,DET.[MODIFY_DTIME]
                                          FROM [CHK_DETAIL_SVR_COEF_CON] AS DET
                                          JOIN SYS_DETAILSERIAL AS SER
                                          ON DET.SERIALID=SER.SERIALID
                                          JOIN SYS_ORGANIZE AS ORG
                                          ON DET.ORG_ID=ORG.ORG_ID
                                          JOIN SYS_USER AS U
                                          ON DET.MODIFY_ID=U.[USER_ID]
                                          WHERE DET.[SERIALID] = @0 AND DET.[ORG_ID]=@1 ", serialId, orgId);

            return db.FirstOrDefault<CHK_DETAIL_SVR_COEF_CON>(sql);
        }


        public static void Delele(string orgId, string serialId)
        {
            var db = gzswDB.GetInstance();
            var sql = Sql.Builder.Append(@"DELETE FROM CHK_DETAIL_SVR_COEF_CON WHERE [SERIALID] = @0 AND [ORG_ID]=@1",
                serialId, orgId);
            db.Execute(sql);
        }

    }
}
