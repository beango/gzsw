using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gzsw.model;
using gzsw.model.Subclasses;
using PetaPoco;

namespace gzsw.dal.dao
{
    public static class STAT_STAFF_CHKSTAT_M_DAL
    {
        public static int Update(STAT_STAFF_CHKSTAT_M item)
        {
            var db = gzswDB.GetInstance();
            var sql = Sql.Builder.Append(@"UPDATE [STAT_STAFF_CHKSTAT_M]
                                                SET 
                                                   [WORK_DAY_CNT] = @0
                                                  ,[LAT_DAY_CNT] = @1
                                                  ,[EAR_DAY_CNT] = @2
                                                  ,[NONSIGN_OUT_CNT] = @3
                                                  ,[HOLLI_TYP1_CNT] = @4
                                                  ,[HOLLI_TYP2_CNT] = @5
                                                  ,[HOLLI_TYP3_CNT] = @6
                                                  ,[HOLLI_TYP4_CNT] = @7
                                                  ,[HOLLI_TYP5_CNT] = @8
                                                  ,[HOLLI_TYP6_CNT] = @9
                                                  ,[HOLLI_TYP7_CNT] = @10
                                                  ,[HOLLI_TYP8_CNT] = @11
                                                  ,[HOLLI_TYP9_CNT] = @12
                                                  ,[HOLLI_TYP10_CNT] = @13
                                                  ,[HOLLI_TYP11_CNT] = @14
                                                  ,[HOLLI_TYP12_CNT] = @15
                                                  ,[HOLLI_TYP13_CNT] =@16
                                                  ,[COR_WORK_DAY_CNT] = @17
                                                  ,[COR_LAT_DAY_CNT] = @18
                                                  ,[COR_EAR_DAY_CNT] = @19
                                                  ,[COR_NONSIGN_OUT_CNT] = @20
                                                  ,[COR_HOLLI_TYP1_CNT] = @21
                                                  ,[COR_HOLLI_TYP2_CNT] = @22
                                                  ,[COR_HOLLI_TYP3_CNT] = @23
                                                  ,[COR_HOLLI_TYP4_CNT] = @24
                                                  ,[COR_HOLLI_TYP5_CNT] = @25
                                                  ,[COR_HOLLI_TYP6_CNT] = @26
                                                  ,[COR_HOLLI_TYP7_CNT] = @27
                                                  ,[COR_HOLLI_TYP8_CNT] = @28
                                                  ,[COR_HOLLI_TYP9_CNT] = @29
                                                  ,[COR_HOLLI_TYP10_CNT] = @30
                                                  ,[COR_HOLLI_TYP11_CNT] = @31
                                                  ,[COR_HOLLI_TYP12_CNT] = @32
                                                  ,[COR_HOLLI_TYP13_CNT] = @33
                                                  ,[MODIFY_ID] = @34
                                                  ,[MODIFY_DTIME] = @35
                                                  ,[ABSENT_DAY_CNT] = @36
                                                  ,[COR_ABSENT_DAY_CNT] = @37
                                             WHERE  1=1 ", item.WORK_DAY_CNT
                                                         , item.LAT_DAY_CNT
                                                         , item.EAR_DAY_CNT
                                                         , item.NONSIGN_OUT_CNT
                                                         , item.HOLLI_TYP1_CNT
                                                         , item.HOLLI_TYP2_CNT
                                                         , item.HOLLI_TYP3_CNT
                                                         , item.HOLLI_TYP4_CNT
                                                         , item.HOLLI_TYP5_CNT
                                                         , item.HOLLI_TYP6_CNT
                                                         , item.HOLLI_TYP7_CNT
                                                         , item.HOLLI_TYP8_CNT
                                                         , item.HOLLI_TYP9_CNT
                                                         , item.HOLLI_TYP10_CNT
                                                         , item.HOLLI_TYP11_CNT
                                                         , item.HOLLI_TYP12_CNT
                                                         , item.HOLLI_TYP13_CNT
                                                         , item.COR_WORK_DAY_CNT
                                                         , item.COR_LAT_DAY_CNT
                                                         , item.COR_EAR_DAY_CNT
                                                         , item.COR_NONSIGN_OUT_CNT
                                                         , item.COR_HOLLI_TYP1_CNT
                                                         , item.COR_HOLLI_TYP2_CNT
                                                         , item.COR_HOLLI_TYP3_CNT
                                                         , item.COR_HOLLI_TYP4_CNT
                                                         , item.COR_HOLLI_TYP5_CNT
                                                         , item.COR_HOLLI_TYP6_CNT
                                                         , item.COR_HOLLI_TYP7_CNT
                                                         , item.COR_HOLLI_TYP8_CNT
                                                         , item.COR_HOLLI_TYP9_CNT
                                                         , item.COR_HOLLI_TYP10_CNT
                                                         , item.COR_HOLLI_TYP11_CNT
                                                         , item.COR_HOLLI_TYP12_CNT
                                                         , item.COR_HOLLI_TYP13_CNT
                                                         , item.MODIFY_ID
                                                         , item.MODIFY_DTIME
                                                         , item.ABSENT_DAY_CNT
                                                         , item.COR_ABSENT_DAY_CNT
                                                         );

            sql.Append(@" AND [STAT_MO]=@0 AND [STAFF_ID]=@1", item.STAT_MO, item.STAFF_ID);

            return db.Execute(sql);
        }

        public static Page<STAT_STAFF_CHKSTAT_M_SUB> GetListBub(int STAT_MO, string staffNo, string staffName,
            string orgId ,int pageIndex, int pageSize)
        {
            var db = gzswDB.GetInstance();

            var sql = Sql.Builder.Append(@"SELECT S.[STAT_MO]
                                                  ,S.[STAFF_ID]
                                                  ,ST.[STAFF_NAM]
                                                  ,S.[WORK_DAY_CNT]
                                                  ,S.[LAT_DAY_CNT]
                                                  ,S.[EAR_DAY_CNT]
                                                  ,S.[NONSIGN_OUT_CNT]
                                                  ,S.[HOLLI_TYP1_CNT]
                                                  ,S.[HOLLI_TYP2_CNT]
                                                  ,S.[HOLLI_TYP3_CNT]
                                                  ,S.[HOLLI_TYP4_CNT]
                                                  ,S.[HOLLI_TYP5_CNT]
                                                  ,S.[HOLLI_TYP6_CNT]
                                                  ,S.[HOLLI_TYP7_CNT]
                                                  ,S.[HOLLI_TYP8_CNT]
                                                  ,S.[HOLLI_TYP9_CNT]
                                                  ,S.[HOLLI_TYP10_CNT]
                                                  ,S.[HOLLI_TYP11_CNT]
                                                  ,S.[HOLLI_TYP12_CNT]
                                                  ,S.[HOLLI_TYP13_CNT]
                                                  ,S.[COR_WORK_DAY_CNT]
                                                  ,S.[COR_LAT_DAY_CNT]
                                                  ,S.[COR_EAR_DAY_CNT]
                                                  ,S.[COR_NONSIGN_OUT_CNT]
                                                  ,S.[COR_HOLLI_TYP1_CNT]
                                                  ,S.[COR_HOLLI_TYP2_CNT]
                                                  ,S.[COR_HOLLI_TYP3_CNT]
                                                  ,S.[COR_HOLLI_TYP4_CNT]
                                                  ,S.[COR_HOLLI_TYP5_CNT]
                                                  ,S.[COR_HOLLI_TYP6_CNT]
                                                  ,S.[COR_HOLLI_TYP7_CNT]
                                                  ,S.[COR_HOLLI_TYP8_CNT]
                                                  ,S.[COR_HOLLI_TYP9_CNT]
                                                  ,S.[COR_HOLLI_TYP10_CNT]
                                                  ,S.[COR_HOLLI_TYP11_CNT]
                                                  ,S.[COR_HOLLI_TYP12_CNT]
                                                  ,S.[COR_HOLLI_TYP13_CNT]
                                                  ,S.[MODIFY_ID]
                                                  ,S.[MODIFY_DTIME]
                                                  ,S.ABSENT_DAY_CNT
                                                  ,S.COR_ABSENT_DAY_CNT
                                              FROM [STAT_STAFF_CHKSTAT_M] AS S
                                              JOIN SYS_STAFF AS ST
                                              ON S.STAFF_ID=ST.STAFF_ID 
                                              WHERE 1=1 ");

            sql.Append(@" AND ST.[ORG_ID]=@0 AND S.[STAT_MO]=@1 ", orgId, STAT_MO);

            if (!string.IsNullOrEmpty(staffNo))
            {
                sql.Append(@" AND ST.[STAFF_ID] like @0 ", "%"+staffNo+"%");
            }

            if (!string.IsNullOrEmpty(staffName))
            {
                sql.Append(@" AND ST.[STAFF_NAM] like @0 ", "%" + staffName + "%");
            }

            return db.Page<STAT_STAFF_CHKSTAT_M_SUB>(pageIndex, pageSize, sql);
        }

        public static STAT_STAFF_CHKSTAT_M_SUB GetSub(int STAT_MO, string staffNo)
        {
            var db = gzswDB.GetInstance();

            var sql = Sql.Builder.Append(@"SELECT S.[STAT_MO]
                                                  ,S.[STAFF_ID]
                                                  ,ST.[STAFF_NAM]
                                                  ,S.[WORK_DAY_CNT]
                                                  ,S.[LAT_DAY_CNT]
                                                  ,S.[EAR_DAY_CNT]
                                                  ,S.[NONSIGN_OUT_CNT]
                                                  ,S.[HOLLI_TYP1_CNT]
                                                  ,S.[HOLLI_TYP2_CNT]
                                                  ,S.[HOLLI_TYP3_CNT]
                                                  ,S.[HOLLI_TYP4_CNT]
                                                  ,S.[HOLLI_TYP5_CNT]
                                                  ,S.[HOLLI_TYP6_CNT]
                                                  ,S.[HOLLI_TYP7_CNT]
                                                  ,S.[HOLLI_TYP8_CNT]
                                                  ,S.[HOLLI_TYP9_CNT]
                                                  ,S.[HOLLI_TYP10_CNT]
                                                  ,S.[HOLLI_TYP11_CNT]
                                                  ,S.[HOLLI_TYP12_CNT]
                                                  ,S.[HOLLI_TYP13_CNT]
                                                  ,S.[COR_WORK_DAY_CNT]
                                                  ,S.[COR_LAT_DAY_CNT]
                                                  ,S.[COR_EAR_DAY_CNT]
                                                  ,S.[COR_NONSIGN_OUT_CNT]
                                                  ,S.[COR_HOLLI_TYP1_CNT]
                                                  ,S.[COR_HOLLI_TYP2_CNT]
                                                  ,S.[COR_HOLLI_TYP3_CNT]
                                                  ,S.[COR_HOLLI_TYP4_CNT]
                                                  ,S.[COR_HOLLI_TYP5_CNT]
                                                  ,S.[COR_HOLLI_TYP6_CNT]
                                                  ,S.[COR_HOLLI_TYP7_CNT]
                                                  ,S.[COR_HOLLI_TYP8_CNT]
                                                  ,S.[COR_HOLLI_TYP9_CNT]
                                                  ,S.[COR_HOLLI_TYP10_CNT]
                                                  ,S.[COR_HOLLI_TYP11_CNT]
                                                  ,S.[COR_HOLLI_TYP12_CNT]
                                                  ,S.[COR_HOLLI_TYP13_CNT]
                                                  ,S.[MODIFY_ID]
                                                  ,S.[MODIFY_DTIME]
,S.ABSENT_DAY_CNT
                                                  ,S.COR_ABSENT_DAY_CNT
                                              FROM [STAT_STAFF_CHKSTAT_M] AS S
                                              JOIN SYS_STAFF AS ST
                                              ON S.STAFF_ID=ST.STAFF_ID 
                                              WHERE 1=1 ");

            sql.Append(@" AND ST.[STAFF_ID]=@0 AND S.[STAT_MO]=@1 ",staffNo, STAT_MO);

            return db.FirstOrDefault<STAT_STAFF_CHKSTAT_M_SUB>(sql);
        }

        public static Page<STAT_STAFF_CHKSTAT_M_SUB> GetListSub(int beginStatMo, int endStatMo,
            string staffId, int pageIndex, int pageSize)
        {
            var db = gzswDB.GetInstance();

            var sql = Sql.Builder.Append(@"SELECT S.[STAT_MO]
                                                  ,S.[STAFF_ID]
                                                  ,ST.[STAFF_NAM]
                                                  ,S.[WORK_DAY_CNT]
                                                  ,S.[LAT_DAY_CNT]
                                                  ,S.[EAR_DAY_CNT]
                                                  ,S.[NONSIGN_OUT_CNT]
                                                  ,S.[HOLLI_TYP1_CNT]
                                                  ,S.[HOLLI_TYP2_CNT]
                                                  ,S.[HOLLI_TYP3_CNT]
                                                  ,S.[HOLLI_TYP4_CNT]
                                                  ,S.[HOLLI_TYP5_CNT]
                                                  ,S.[HOLLI_TYP6_CNT]
                                                  ,S.[HOLLI_TYP7_CNT]
                                                  ,S.[HOLLI_TYP8_CNT]
                                                  ,S.[HOLLI_TYP9_CNT]
                                                  ,S.[HOLLI_TYP10_CNT]
                                                  ,S.[HOLLI_TYP11_CNT]
                                                  ,S.[HOLLI_TYP12_CNT]
                                                  ,S.[HOLLI_TYP13_CNT]
                                                  ,S.[COR_WORK_DAY_CNT]
                                                  ,S.[COR_LAT_DAY_CNT]
                                                  ,S.[COR_EAR_DAY_CNT]
                                                  ,S.[COR_NONSIGN_OUT_CNT]
                                                  ,S.[COR_HOLLI_TYP1_CNT]
                                                  ,S.[COR_HOLLI_TYP2_CNT]
                                                  ,S.[COR_HOLLI_TYP3_CNT]
                                                  ,S.[COR_HOLLI_TYP4_CNT]
                                                  ,S.[COR_HOLLI_TYP5_CNT]
                                                  ,S.[COR_HOLLI_TYP6_CNT]
                                                  ,S.[COR_HOLLI_TYP7_CNT]
                                                  ,S.[COR_HOLLI_TYP8_CNT]
                                                  ,S.[COR_HOLLI_TYP9_CNT]
                                                  ,S.[COR_HOLLI_TYP10_CNT]
                                                  ,S.[COR_HOLLI_TYP11_CNT]
                                                  ,S.[COR_HOLLI_TYP12_CNT]
                                                  ,S.[COR_HOLLI_TYP13_CNT]
                                                  ,S.[MODIFY_ID]
                                                  ,S.[MODIFY_DTIME]
,S.ABSENT_DAY_CNT
                                                  ,S.COR_ABSENT_DAY_CNT
                                              FROM [STAT_STAFF_CHKSTAT_M] AS S
                                              JOIN SYS_STAFF AS ST
                                              ON S.STAFF_ID=ST.STAFF_ID 
                                              WHERE 1=1 ");

            sql.Append(@" AND S.[STAFF_ID]=@0 ", staffId);

            sql.Append(@" AND S.STAT_MO>=@0 AND S.STAT_MO<=@1 ", beginStatMo, endStatMo);

            return db.Page<STAT_STAFF_CHKSTAT_M_SUB>(pageIndex, pageSize, sql);
        }

        public static dynamic GetTotal(int beginStatMo, int endStatMo, string staffId)
        {
            var db = gzswDB.GetInstance();

            var sql = Sql.Builder.Append(@"SELECT SUM(S.[COR_WORK_DAY_CNT]) AS COR_WORK_DAY_CNT_TOTAL
                                                  ,SUM(S.[COR_LAT_DAY_CNT]) AS COR_LAT_DAY_CNT_TOTAL
                                                ,SUM(S.[COR_EAR_DAY_CNT]) AS COR_EAR_DAY_CNT_TOTAL
                                                ,SUM(S.[COR_NONSIGN_OUT_CNT]) AS COR_NONSIGN_OUT_CNT_TOTAL
                                                ,SUM(S.[COR_HOLLI_TYP1_CNT]) AS COR_HOLLI_TYP1_CNT_TOTAL
                                                ,SUM(S.[COR_HOLLI_TYP2_CNT]) AS COR_HOLLI_TYP2_CNT_TOTAL
                                                ,SUM(S.[COR_HOLLI_TYP3_CNT]) AS COR_HOLLI_TYP3_CNT_TOTAL
                                                ,SUM(S.[COR_HOLLI_TYP4_CNT]) AS COR_HOLLI_TYP4_CNT_TOTAL
                                                ,SUM(S.[COR_HOLLI_TYP5_CNT]) AS COR_HOLLI_TYP5_CNT_TOTAL
                                                ,SUM(S.[COR_HOLLI_TYP6_CNT]) AS COR_HOLLI_TYP6_CNT_TOTAL
                                                ,SUM(S.[COR_HOLLI_TYP7_CNT]) AS COR_HOLLI_TYP7_CNT_TOTAL
                                                ,SUM(S.[COR_HOLLI_TYP8_CNT]) AS COR_HOLLI_TYP8_CNT_TOTAL
                                                ,SUM(S.[COR_HOLLI_TYP9_CNT]) AS COR_HOLLI_TYP9_CNT_TOTAL
                                                ,SUM(S.[COR_HOLLI_TYP10_CNT]) AS COR_HOLLI_TYP10_CNT_TOTAL
                                                ,SUM(S.[COR_HOLLI_TYP11_CNT]) AS COR_HOLLI_TYP11_CNT_TOTAL
                                                ,SUM(S.[COR_HOLLI_TYP12_CNT]) AS COR_HOLLI_TYP12_CNT_TOTAL
                                                ,SUM(S.[COR_HOLLI_TYP13_CNT]) AS COR_HOLLI_TYP13_CNT_TOTAL
                                                ,SUM(S.COR_ABSENT_DAY_CNT) AS COR_ABSENT_DAY_CNT
                                              FROM [STAT_STAFF_CHKSTAT_M] AS S
                                              WHERE 1=1 ");

            sql.Append(@" AND S.[STAFF_ID]=@0 ", staffId);

            sql.Append(@" AND S.STAT_MO>=@0 AND S.STAT_MO<=@1 ", beginStatMo, endStatMo);

            return db.FirstOrDefault<dynamic>(sql);
        }
        public static dynamic GetListBubByHall(int beginStatMo, int endStatMo, string hallNo, int pageIndex, int pageSize)
        {
            var db = gzswDB.GetInstance();

            var sql = Sql.Builder.Append(@"SELECT S.[STAT_MO]
                                                  ,S.[STAFF_ID]
                                                  ,ST.[STAFF_NAM]
                                                  ,S.[WORK_DAY_CNT]
                                                  ,S.[LAT_DAY_CNT]
                                                  ,S.[EAR_DAY_CNT]
                                                  ,S.[NONSIGN_OUT_CNT]
                                                  ,S.[HOLLI_TYP1_CNT]
                                                  ,S.[HOLLI_TYP2_CNT]
                                                  ,S.[HOLLI_TYP3_CNT]
                                                  ,S.[HOLLI_TYP4_CNT]
                                                  ,S.[HOLLI_TYP5_CNT]
                                                  ,S.[HOLLI_TYP6_CNT]
                                                  ,S.[HOLLI_TYP7_CNT]
                                                  ,S.[HOLLI_TYP8_CNT]
                                                  ,S.[HOLLI_TYP9_CNT]
                                                  ,S.[HOLLI_TYP10_CNT]
                                                  ,S.[HOLLI_TYP11_CNT]
                                                  ,S.[HOLLI_TYP12_CNT]
                                                  ,S.[HOLLI_TYP13_CNT]
                                                  ,S.[COR_WORK_DAY_CNT]
                                                  ,S.[COR_LAT_DAY_CNT]
                                                  ,S.[COR_EAR_DAY_CNT]
                                                  ,S.[COR_NONSIGN_OUT_CNT]
                                                  ,S.[COR_HOLLI_TYP1_CNT]
                                                  ,S.[COR_HOLLI_TYP2_CNT]
                                                  ,S.[COR_HOLLI_TYP3_CNT]
                                                  ,S.[COR_HOLLI_TYP4_CNT]
                                                  ,S.[COR_HOLLI_TYP5_CNT]
                                                  ,S.[COR_HOLLI_TYP6_CNT]
                                                  ,S.[COR_HOLLI_TYP7_CNT]
                                                  ,S.[COR_HOLLI_TYP8_CNT]
                                                  ,S.[COR_HOLLI_TYP9_CNT]
                                                  ,S.[COR_HOLLI_TYP10_CNT]
                                                  ,S.[COR_HOLLI_TYP11_CNT]
                                                  ,S.[COR_HOLLI_TYP12_CNT]
                                                  ,S.[COR_HOLLI_TYP13_CNT]
                                                  ,S.[MODIFY_ID]
                                                  ,S.[MODIFY_DTIME]
,S.ABSENT_DAY_CNT
                                                  ,S.COR_ABSENT_DAY_CNT
                                              FROM [STAT_STAFF_CHKSTAT_M] AS S
                                              JOIN SYS_STAFF AS ST
                                              ON S.STAFF_ID=ST.STAFF_ID  ");

            sql.Append(@" AND ST.[ORG_ID]=@0 ", hallNo);

            sql.Append(@" AND S.STAT_MO>=@0 AND S.STAT_MO<=@1 ", beginStatMo, endStatMo);
             
            var data = db.Page<dynamic>(pageIndex, pageSize, sql);
            data.ItemsPerPage = pageSize;
            return data;
        }
    
    
    }
}
