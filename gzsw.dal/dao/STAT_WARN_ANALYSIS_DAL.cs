using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gzsw.model.dto;
using PetaPoco;
using gzsw.model;

namespace gzsw.dal.dao
{
    public class STAT_WARN_ANALYSIS_DAL
    {
        /// <summary>
        /// 获取服务厅预警分析报表
        /// </summary>
        /// <param name="beginTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="userId">用户ID</param> 
        /// <returns></returns>
        public static IList<Statistics_WarnAnalysisDto> GetStatistics_WarnAnalysisChart(string userId, DateTime? beginTime, DateTime? endTime)
        {
            var sql = Sql.Builder.Append(@";
WITH    WarnStatAll
          AS ( SELECT   *
               FROM     STAT_WARN_HALL_STAT_D
               WHERE    1 = 1
                        AND DATEDIFF(DAY, STAT_DT, @0) <= 0
                        AND DATEDIFF(DAY, STAT_DT, @1) >= 0
             )
    SELECT  Halls.HALL_NO ,
            Halls.HALL_NAM ,
            WarnLvls.LCode AS L_Code ,
            WarnLvls.LName AS L_Name ,
            WarnLvls.LValue AS L_Value ,
            ISNULL(( SELECT SUM(WarnStatAll.AMOUNT)
                     FROM   WarnStatAll
                     WHERE  WarnStatAll.HALL_NO = Halls.HALL_NO
                            AND ( WarnLvls.LValue = 0
                                  OR WarnStatAll.WARN_LEVEL = WarnLvls.LValue
                                )
                            AND WarnStatAll.WARN_TYP = 1
                   ), 0) AS Count_T1 ,
            ISNULL(( SELECT SUM(WarnStatAll.AMOUNT)
                     FROM   WarnStatAll
                     WHERE  WarnStatAll.HALL_NO = Halls.HALL_NO
                            AND ( WarnLvls.LValue = 0
                                  OR WarnStatAll.WARN_LEVEL = WarnLvls.LValue
                                )
                            AND WarnStatAll.WARN_TYP = 2
                   ), 0) AS Count_T2 ,
            ISNULL(( SELECT SUM(WarnStatAll.AMOUNT)
                     FROM   WarnStatAll
                     WHERE  WarnStatAll.HALL_NO = Halls.HALL_NO
                            AND ( WarnLvls.LValue = 0
                                  OR WarnStatAll.WARN_LEVEL = WarnLvls.LValue
                                )
                            AND WarnStatAll.WARN_TYP = 3
                   ), 0) AS Count_T3 ,
            ISNULL(( SELECT SUM(WarnStatAll.AMOUNT)
                     FROM   WarnStatAll
                     WHERE  WarnStatAll.HALL_NO = Halls.HALL_NO
                            AND ( WarnLvls.LValue = 0
                                  OR WarnStatAll.WARN_LEVEL = WarnLvls.LValue
                                )
                            AND WarnStatAll.WARN_TYP = 4
                   ), 0) AS Count_T4 ,
            ISNULL(( SELECT SUM(WarnStatAll.AMOUNT)
                     FROM   WarnStatAll
                     WHERE  WarnStatAll.HALL_NO = Halls.HALL_NO
                            AND ( WarnLvls.LValue = 0
                                  OR WarnStatAll.WARN_LEVEL = WarnLvls.LValue
                                )
                            AND WarnStatAll.WARN_TYP = 5
                   ), 0) AS Count_T5 ,
            ISNULL(( SELECT SUM(WarnStatAll.AMOUNT)
                     FROM   WarnStatAll
                     WHERE  WarnStatAll.HALL_NO = Halls.HALL_NO
                            AND ( WarnLvls.LValue = 0
                                  OR WarnStatAll.WARN_LEVEL = WarnLvls.LValue
                                )
                            AND WarnStatAll.WARN_TYP = 6
                   ), 0) AS Count_T6 ,
            ISNULL(( SELECT SUM(WarnStatAll.AMOUNT)
                     FROM   WarnStatAll
                     WHERE  WarnStatAll.HALL_NO = Halls.HALL_NO
                            AND ( WarnLvls.LValue = 0
                                  OR WarnStatAll.WARN_LEVEL = WarnLvls.LValue
                                )
                            AND WarnStatAll.WARN_TYP = 7
                   ), 0) AS Count_T7 ,
            ISNULL(( SELECT SUM(WarnStatAll.AMOUNT)
                     FROM   WarnStatAll
                     WHERE  WarnStatAll.HALL_NO = Halls.HALL_NO
                            AND ( WarnLvls.LValue = 0
                                  OR WarnStatAll.WARN_LEVEL = WarnLvls.LValue
                                )
                            AND WarnStatAll.WARN_TYP = 8
                   ), 0) AS Count_T8 ,
            ISNULL(( SELECT SUM(WarnStatAll.AMOUNT)
                     FROM   WarnStatAll
                     WHERE  WarnStatAll.HALL_NO = Halls.HALL_NO
                            AND ( WarnLvls.LValue = 0
                                  OR WarnStatAll.WARN_LEVEL = WarnLvls.LValue
                                )
                            AND WarnStatAll.WARN_TYP = 9
                   ), 0) AS Count_T9
    FROM    SYS_HALL AS Halls
            JOIN ( VALUES ( 'Y', '黄', 1), ( 'O', '橙' , 2), ( 'R', '红', 3),
            ( 'H', '合计', 0) ) AS WarnLvls ( LCode, LName, LValue ) ON 1 = 1
    WHERE   1 = 1
            AND Halls.HALL_NO IN (
            SELECT  ORG_ID
            FROM    SYS_ORGANIZE
            WHERE   ORG_ID IN ( SELECT  ORG_ID
                                FROM    SYS_USERORGANIZE
                                WHERE   [USER_ID] = @2 )
                    AND ORG_LEVEL = 4 )", beginTime, endTime, userId);


            Database db = gzswDB.GetInstance();
            return db.Fetch<Statistics_WarnAnalysisDto>(sql);
        }

        /// <summary>
        /// 获取服务厅预警分析详细报表
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <param name="hallNo"></param>
        /// <param name="detailType"></param>
        /// <param name="detailXValue"></param>
        /// <param name="warnType"></param>
        /// <param name="warnLevel"></param>
        /// <returns></returns>
        public static IList<Statistics_AnalysisDetailDto> GetStatistics_WarnAnalysisDetailChart(string userId, DateTime? beginTime, DateTime? endTime, string hallNo, int detailType, string detailXValue, int? warnType, int? warnLevel)
        {
            var sql = Sql.Builder.Append(@";
WITH    WarnStatAll
          AS ( SELECT   *
               FROM     STAT_WARN_HALL_STAT_D
               WHERE    1 = 1
                        AND HALL_NO IN (
                        SELECT  ORG_ID
                        FROM    SYS_ORGANIZE
                        WHERE   ORG_ID IN ( SELECT  ORG_ID
                                            FROM    SYS_USERORGANIZE
                                            WHERE   [USER_ID] = @0 )
                                AND ORG_LEVEL = 4 )
                        AND DATEDIFF(DAY, STAT_DT, @1) <= 0
                        AND DATEDIFF(DAY, STAT_DT, @2) >= 0
                        AND ( ( @3 = ''
                                OR @3 IS NULL
                              )
                              OR HALL_NO = @3
                            )
                        AND ( 0 = @4
                              OR WARN_TYP = @4
                            )
                        AND ( 0 = @5
                              OR WARN_LEVEL = @5
                            )
             )
    SELECT  X_Values.SValue AS X_Id ,
            ( CASE @6
                WHEN 2
                THEN CAST(CAST(X_Values.SValue AS DATE) AS NVARCHAR(10))
                WHEN 3 THEN CAST(CAST(X_Values.SValue AS DATE) AS NVARCHAR(7))
                WHEN 4 THEN CAST(CAST(X_Values.SValue AS DATE) AS NVARCHAR(4))
                ELSE CAST(( CAST(X_Values.SValue AS TINYINT) - 1 ) AS NVARCHAR(10))
                     + '点至' + X_Values.SValue + '点'
              END ) AS X_ShowName ,
            @6 AS X_Type ,
            ISNULL(( SELECT SUM(WarnStatAll.AMOUNT)
                     FROM   WarnStatAll
                     WHERE  1 = 1
                            AND ( ( 1 = @6
                                    AND WarnStatAll.TIME_QUANTUM_CD = CAST(X_Values.SValue AS TINYINT)
                                  )
                                  OR ( 2 = @6
                                       AND DATEDIFF(DAY, WarnStatAll.STAT_DT,
                                                    CAST(X_Values.SValue AS DATE)) = 0
                                     )
                                  OR ( 3 = @6
                                       AND DATEDIFF(MONTH, WarnStatAll.STAT_DT,
                                                    CAST(X_Values.SValue AS DATE)) = 0
                                     )
                                  OR ( 4 = @6
                                       AND DATEDIFF(YEAR, WarnStatAll.STAT_DT,
                                                    CAST(X_Values.SValue AS DATE)) = 0
                                     )
                                )
                   ), 0) AS Y_Value
    FROM    ( SELECT  DISTINCT
                        *
              FROM      dbo.Spliter(@7, '|')
            ) AS X_Values", userId, beginTime, endTime, hallNo, warnType, warnLevel, detailType, detailXValue);


            Database db = gzswDB.GetInstance();
            return db.Fetch<Statistics_AnalysisDetailDto>(sql);
        }
    }
}
