using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gzsw.model;
using PetaPoco;

namespace gzsw.dal.dao
{
    public static class CHK_STAFF_COMPRE_SAN_MARK_DAL
    {
        public static Page<CHK_STAFF_COMPRE_SAN_MARK> GetList(string orgId,int?type, string staffNo, string staffName, string userId, int pageIndex, int pageSize)
        {
            var db = gzswDB.GetInstance();

            var sql = Sql.Builder.Append(@"SELECT C.[SEQ]
                                              ,C.[STAT_MO]
                                              ,C.[STAFF_ID]
                                              ,ST.ORG_ID
                                              ,O.ORG_NAM
	                                          ,ST.[STAFF_NAM]
                                              ,C.[COMPRE_SAN_TYP]
                                              ,C.[SCORE]
                                              ,C.[MODIFY_ID]
                                              ,C.[MODIFY_DTIME]
                                              ,C.[REASON]
                                              ,C.[MARK_TIME]
                                          FROM [CHK_STAFF_COMPRE_SAN_MARK] AS C
                                          JOIN SYS_STAFF AS ST
                                          ON C.STAFF_ID=ST.STAFF_ID
                                          JOIN SYS_ORGANIZE AS O
                                          ON ST.ORG_ID=O.ORG_ID 
                                          JOIN SYS_USERORGANIZE AS UE
                                          ON UE.ORG_ID = ST.ORG_ID WHERE UE.[USER_ID]=@0 ",userId);

            if (!string.IsNullOrEmpty(orgId))
            {
                sql.Append(@" AND ST.ORG_ID = @0 ", orgId);
            }
            if (type!=null)
            {
                sql.Append(@" AND C.[COMPRE_SAN_TYP] = @0 ", type);
            }
            if (!string.IsNullOrEmpty(staffNo))
            {
                sql.Append(@" AND C.[STAFF_ID] like @0 ", "%" + staffNo + "%");
            }
            if (!string.IsNullOrEmpty(staffName))
            {
                sql.Append(@" AND ST.[STAFF_NAM] like @0 ", "%" + staffName + "%");
            }

            return db.Page<CHK_STAFF_COMPRE_SAN_MARK>(pageIndex, pageSize, sql);
        }


        public static CHK_STAFF_COMPRE_SAN_MARK Get(int seq)
        {
            var db = gzswDB.GetInstance();
            var sql = Sql.Builder.Append(@"SELECT C.[SEQ]
                                              ,C.[STAT_MO]
                                              ,C.[STAFF_ID]
                                              ,ST.ORG_ID
                                              ,O.ORG_NAM
	                                          ,ST.[STAFF_NAM]
                                              ,C.[COMPRE_SAN_TYP]
                                              ,C.[SCORE]
                                              ,C.[MODIFY_ID]
                                              ,C.[MODIFY_DTIME]
                                              ,C.[REASON]
                                              ,C.[MARK_TIME]
                                          FROM [CHK_STAFF_COMPRE_SAN_MARK] AS C
                                          JOIN SYS_STAFF AS ST
                                          ON C.STAFF_ID=ST.STAFF_ID
                                          JOIN SYS_ORGANIZE AS O
                                          ON ST.ORG_ID=O.ORG_ID
                                          WHERE C.SEQ=@0  ",seq);

            return db.FirstOrDefault<CHK_STAFF_COMPRE_SAN_MARK>(sql);
        }


        public static Page<CHK_STAFF_COMPRE_SAN_MARK> GetList(int beginStatMo, int endStatMo,
            string staffId, int pageIndex, int pageSize)
        {
            var db = gzswDB.GetInstance();

            var sql = Sql.Builder.Append(@"SELECT C.[SEQ]
                                              ,C.[STAT_MO]
                                              ,C.[STAFF_ID]
	                                          ,ST.[STAFF_NAM]
                                              ,C.[COMPRE_SAN_TYP]
                                              ,C.[SCORE]
                                              ,C.[MODIFY_ID]
                                              ,C.[MODIFY_DTIME]
                                              ,C.[REASON]
                                              ,C.[MARK_TIME]
                                          FROM [CHK_STAFF_COMPRE_SAN_MARK] AS C
                                          JOIN SYS_STAFF AS ST
                                          ON C.STAFF_ID=ST.STAFF_ID
                                          WHERE 1=1 ");


            sql.Append(@" AND C.[STAFF_ID]=@0 ", staffId);
            sql.Append(@" AND C.STAT_MO>=@0 AND C.STAT_MO<=@1 ", beginStatMo, endStatMo);

            return db.Page<CHK_STAFF_COMPRE_SAN_MARK>(pageIndex, pageSize, sql);
        }

        public static dynamic GetTotal(int beginStatMo, int endStatMo, string staffId)
        {
            var db = gzswDB.GetInstance();

            var sql = Sql.Builder.Append(@"SELECT SUM(S.[SCORE]) AS SCORE_TOTAL
                                              FROM [CHK_STAFF_COMPRE_SAN_MARK] AS S
                                              WHERE 1=1 ");

            sql.Append(@" AND S.[STAFF_ID]=@0 ", staffId);
            sql.Append(@" AND S.STAT_MO>=@0 AND S.STAT_MO<=@1 ", beginStatMo, endStatMo);

            return db.FirstOrDefault<dynamic>(sql);
        }
    }
}
