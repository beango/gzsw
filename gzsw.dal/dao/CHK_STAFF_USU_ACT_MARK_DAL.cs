using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gzsw.model;
using PetaPoco;

namespace gzsw.dal.dao
{
    public static class CHK_STAFF_USU_ACT_MARK_DAL
    {
        public static Page<CHK_STAFF_USU_ACT_MARK> GetList(string orgId,int?type,string staffNo, string staffName,string userId, int pageIndex, int pageSize)
        {
            var db = gzswDB.GetInstance();

            var sql = Sql.Builder.Append(@"SELECT U.[SEQ]
                                              ,U.[STAT_MO]
                                              ,U.[STAFF_ID]
                                              ,ST.ORG_ID
                                              ,O.ORG_NAM
	                                          ,ST.[STAFF_NAM]
                                              ,U.[USU_ACT_TYP]
                                              ,U.[DEDUCT]
                                              ,U.[MODIFY_ID]
                                              ,U.[MODIFY_DTIME]
                                              ,U.[REASON]
                                              ,U.[MARK_TIME]
                                          FROM [CHK_STAFF_USU_ACT_MARK] AS U
                                          JOIN SYS_STAFF AS ST
                                          ON U.STAFF_ID=ST.STAFF_ID
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
                sql.Append(@" AND U.[USU_ACT_TYP] = @0 ", type);
            }
            if (!string.IsNullOrEmpty(staffNo))
            {
                sql.Append(@" AND U.[STAFF_ID] like @0 ", "%" + staffNo + "%");
            }
            if (!string.IsNullOrEmpty(staffName))
            {
                sql.Append(@" AND ST.[STAFF_NAM] like @0 ", "%" + staffName + "%");
            }

            return db.Page<CHK_STAFF_USU_ACT_MARK>(pageIndex,pageSize,sql);
        }

        public static CHK_STAFF_USU_ACT_MARK Get(int seq)
        {
            var db = gzswDB.GetInstance();

            var sql = Sql.Builder.Append(@"SELECT U.[SEQ]
                                              ,U.[STAT_MO]
                                              ,U.[STAFF_ID]
                                              ,ST.ORG_ID
                                              ,O.ORG_NAM
	                                          ,ST.[STAFF_NAM]
                                              ,U.[USU_ACT_TYP]
                                              ,U.[DEDUCT]
                                              ,U.[MODIFY_ID]
                                              ,U.[MODIFY_DTIME]
                                              ,U.[REASON]
                                              ,U.[MARK_TIME]
                                          FROM [CHK_STAFF_USU_ACT_MARK] AS U
                                          JOIN SYS_STAFF AS ST
                                          ON U.STAFF_ID=ST.STAFF_ID
                                          JOIN SYS_ORGANIZE AS O
                                          ON ST.ORG_ID=O.ORG_ID WHERE U.[SEQ]=@0 ", seq);

            return db.FirstOrDefault<CHK_STAFF_USU_ACT_MARK>(sql);
        }

        public static Page<CHK_STAFF_USU_ACT_MARK> GetMergeList(string orgId, int? beginMo, int? endMo, 
            string staffId,int? type,int pageIndex,int pageSize)
        {
            var db = gzswDB.GetInstance();

            var sql = Sql.Builder.Append(@"SELECT U.[SEQ]
                                              ,U.[STAT_MO]
                                              ,U.[STAFF_ID]
	                                          ,ST.[STAFF_NAM]
                                              ,U.[USU_ACT_TYP]
                                              ,U.[DEDUCT]
                                              ,U.[MODIFY_ID]
                                              ,U.[MODIFY_DTIME]
                                              ,U.[REASON]
                                              ,U.[MARK_TIME]
                                              ,UR.USER_NAM AS MODIFY_NAM
                                          FROM [CHK_STAFF_USU_ACT_MARK] AS U
                                          JOIN SYS_STAFF AS ST
                                          ON U.STAFF_ID=ST.STAFF_ID
                                          JOIN SYS_USER AS UR
                                          ON U.MODIFY_ID=UR.USER_ID
                                          WHERE 1=1 ");

            sql.Append(@" AND ST.ORG_ID =@0 ", orgId);

            if (!string.IsNullOrEmpty(staffId))
            {
                sql.Append(@" AND U.STAFF_ID =@0 ", staffId);
            }
            if (type!=null)
            {
                sql.Append(@" AND U.USU_ACT_TYP =@0 ", type);
            }
            if (beginMo != null)
            {
                sql.Append(@" AND U.STAT_MO >= @0 ", beginMo);
            }
            if (endMo != null)
            {
                sql.Append(@" AND U.STAT_MO <= @0 ", endMo);
            }


            return db.Page<CHK_STAFF_USU_ACT_MARK>(pageIndex, pageSize, sql);
        }

        public static CHK_STAFF_USU_ACT_MARK GetMark(int seq)
        {
            var db = gzswDB.GetInstance();

            var sql = Sql.Builder.Append(@"SELECT U.[SEQ]
                                              ,U.[STAT_MO]
                                              ,U.[STAFF_ID]
	                                          ,ST.[STAFF_NAM]
                                              ,U.[USU_ACT_TYP]
                                              ,U.[DEDUCT]
                                              ,U.[MODIFY_ID]
                                              ,U.[MODIFY_DTIME]
                                              ,U.[REASON]
                                              ,U.[MARK_TIME]
                                              ,UR.USER_NAM AS MODIFY_NAM
                                          FROM [CHK_STAFF_USU_ACT_MARK] AS U
                                          JOIN SYS_STAFF AS ST
                                          ON U.STAFF_ID=ST.STAFF_ID
                                          JOIN SYS_USER AS UR
                                          ON U.MODIFY_ID=UR.USER_ID
                                          WHERE 1=1 ");
            sql.Append(" AND U.[SEQ] =@0 ", seq);
            return db.FirstOrDefault<CHK_STAFF_USU_ACT_MARK>(sql);
        }

        public static Page<CHK_STAFF_USU_ACT_MARK> GetList(int beginStatMo, int endStatMo,
            string staffId, int pageIndex, int pageSize)
        {
            var db = gzswDB.GetInstance();

            var sql = Sql.Builder.Append(@"SELECT U.[SEQ]
                                              ,U.[STAT_MO]
                                              ,U.[STAFF_ID]
                                              ,ST.ORG_ID
                                              ,O.ORG_NAM
	                                          ,ST.[STAFF_NAM]
                                              ,U.[USU_ACT_TYP]
                                              ,U.[DEDUCT]
                                              ,U.[MODIFY_ID]
                                              ,U.[MODIFY_DTIME]
                                              ,U.[REASON]
                                              ,U.[MARK_TIME]
                                          FROM [CHK_STAFF_USU_ACT_MARK] AS U
                                          JOIN SYS_STAFF AS ST
                                          ON U.STAFF_ID=ST.STAFF_ID
                                          JOIN SYS_ORGANIZE AS O
                                          ON ST.ORG_ID=O.ORG_ID
                                          WHERE 1=1 ");

            sql.Append(@" AND U.[STAFF_ID]=@0 ", staffId);
            sql.Append(@" AND U.STAT_MO>=@0 AND U.STAT_MO<=@1 ", beginStatMo, endStatMo);

            return db.Page<CHK_STAFF_USU_ACT_MARK>(pageIndex, pageSize, sql);
        }

        public static dynamic GetTotal(int beginStatMo, int endStatMo, string staffId)
        {
            var db = gzswDB.GetInstance();

            var sql = Sql.Builder.Append(@"SELECT SUM(S.[DEDUCT]) AS DEDUCT_TOTAL
                                              FROM [CHK_STAFF_USU_ACT_MARK] AS S
                                              WHERE 1=1 ");

            sql.Append(@" AND S.[STAFF_ID]=@0 ", staffId);
            sql.Append(@" AND S.STAT_MO>=@0 AND S.STAT_MO<=@1 ", beginStatMo, endStatMo);

            return db.FirstOrDefault<dynamic>(sql);
        }
    }
}
