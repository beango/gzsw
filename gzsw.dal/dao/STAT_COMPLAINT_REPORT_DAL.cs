using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gzsw.model.dto;
using PetaPoco;
using gzsw.model;

namespace gzsw.dal.dao
{
    public class STAT_COMPLAINT_REPORT_DAL
    {
        /// <summary>
        /// 获取服务厅投诉举报分析报表
        /// </summary>
        /// <param name="beginTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="userId">用户ID</param> 
        /// <returns></returns>
        public static IList<Statistics_ComplaintReportDto> GetStatistics_ComplaintReportChart(string userId, DateTime? beginTime, DateTime? endTime)
        {
            var sql = Sql.Builder.Append(@";
WITH    ComplanStatAll
          AS ( SELECT   *
               FROM     STAT_COMPLAIN_HALL_STAT_D
               WHERE    1 = 1
                        AND DATEDIFF(DAY, STAT_DT, @0) <= 0
                        AND DATEDIFF(DAY, STAT_DT, @1) >= 0
             )
    SELECT  Halls.HALL_NO ,
            Halls.HALL_NAM ,
            ComplanTypes.COMPLAIN_TYP_ID ,
            ComplanTypes.COMPLAIN_NAM ,
            ComplanLvls.LCode AS L_Code ,
            ComplanLvls.LName AS L_Name ,
            ComplanLvls.LValue AS L_Value ,
            ( SELECT    CASE ComplanLvls.LValue
                          WHEN 1 THEN ISNULL(SUM(NON_HANDLE_AMOUNT), 0)
                          WHEN 2 THEN ISNULL(SUM(HANDLE_AMOUNT), 0)
                          WHEN 3
                          THEN ( ISNULL(SUM(AMOUNT), 0)
                                 - ISNULL(SUM(NON_HANDLE_AMOUNT), 0)
                                 - ISNULL(SUM(HANDLE_AMOUNT), 0) )
                          ELSE ISNULL(SUM(AMOUNT), 0)
                        END
              FROM      ComplanStatAll
              WHERE     ComplanStatAll.HALL_NO = Halls.HALL_NO
                        AND ComplanStatAll.COMPLAIN_NAM = ComplanTypes.COMPLAIN_NAM
            ) AS Count_Value
    FROM    SYS_HALL AS Halls
            LEFT JOIN WARN_COMPLAIN_TYP_CON AS ComplanTypes ON 1 = 1
            JOIN ( VALUES ( 'N', '未处理', 1), ( 'R', '已处理' , 2), ( 'U', '撤销', 3),
            ( 'H', '合计', 0) ) AS ComplanLvls ( LCode, LName, LValue ) ON 1 = 1
    WHERE   1 = 1
            AND Halls.HALL_NO IN (
            SELECT  ORG_ID
            FROM    SYS_ORGANIZE
            WHERE   ORG_ID IN ( SELECT  ORG_ID
                                FROM    SYS_USERORGANIZE
                                WHERE   [USER_ID] = @2 )
                    AND ORG_LEVEL = 4 )", beginTime, endTime, userId);


            Database db = gzswDB.GetInstance();
            return db.Fetch<Statistics_ComplaintReportDto>(sql);
        }

        /// <summary>
        /// 获取服务厅投诉举报分析详细报表
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <param name="hallNo"></param>
        /// <param name="detailType"></param>
        /// <param name="detailXValue"></param>
        /// <param name="complaintType"></param>
        /// <param name="complaintLevel"></param>
        /// <returns></returns>
        public static IList<Statistics_AnalysisDetailDto> GetStatistics_ComplaintReportDetailChart(string userId, DateTime? beginTime, DateTime? endTime, string hallNo, int detailType, string detailXValue, int? complaintType, int? complaintLevel)
        {
            var sql = Sql.Builder.Append(@";
WITH    ComplanStatAll
          AS ( SELECT   *
               FROM     ( SELECT    STAT_COMPLAIN_HALL_STAT_D.STAT_DT ,
                                    STAT_COMPLAIN_HALL_STAT_D.TIME_QUANTUM_CD ,
                                    STAT_COMPLAIN_HALL_STAT_D.HALL_NO ,
                                    ( SELECT TOP 1
                                                ComplanTypes.COMPLAIN_TYP_ID
                                      FROM      WARN_COMPLAIN_TYP_CON AS ComplanTypes
                                      WHERE     ComplanTypes.COMPLAIN_NAM = STAT_COMPLAIN_HALL_STAT_D.COMPLAIN_NAM
                                    ) AS COMPLAIN_TYP ,
                                    ComplanLvls.LValue AS COMPLAIN_LEVEL ,
                                    ( CASE ComplanLvls.LValue
                                        WHEN 1
                                        THEN ISNULL(NON_HANDLE_AMOUNT, 0)
                                        WHEN 2 THEN ISNULL(HANDLE_AMOUNT, 0)
                                        WHEN 3
                                        THEN ( ISNULL(AMOUNT, 0)
                                               - ISNULL(NON_HANDLE_AMOUNT, 0)
                                               - ISNULL(HANDLE_AMOUNT, 0) )
                                        ELSE ISNULL(AMOUNT, 0)
                                      END ) AS AMOUNT
                          FROM      STAT_COMPLAIN_HALL_STAT_D
                                    JOIN ( VALUES ( 'N', '未处理', 1),
                                    ( 'R', '已处理' , 2), ( 'U', '撤销', 3),
                                    ( 'H', '合计', 0) ) AS ComplanLvls ( LCode, LName, LValue ) ON 1 = 1
                        ) AS ComplanAll
               WHERE    1 = 1
                        AND ComplanAll.HALL_NO IN (
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
                              OR ComplanAll.HALL_NO = @3
                            )
                        AND ( 0 = @4
                              OR ComplanAll.COMPLAIN_TYP = @4
                            )
                        AND ( ComplanAll.COMPLAIN_LEVEL = @5 )
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
            ISNULL(( SELECT SUM(ComplanStatAll.AMOUNT)
                     FROM   ComplanStatAll
                     WHERE  1 = 1
                            AND ( ( 1 = @6
                                    AND ComplanStatAll.TIME_QUANTUM_CD = CAST(X_Values.SValue AS TINYINT)
                                  )
                                  OR ( 2 = @6
                                       AND DATEDIFF(DAY,
                                                    ComplanStatAll.STAT_DT,
                                                    CAST(X_Values.SValue AS DATE)) = 0
                                     )
                                  OR ( 3 = @6
                                       AND DATEDIFF(MONTH,
                                                    ComplanStatAll.STAT_DT,
                                                    CAST(X_Values.SValue AS DATE)) = 0
                                     )
                                  OR ( 4 = @6
                                       AND DATEDIFF(YEAR,
                                                    ComplanStatAll.STAT_DT,
                                                    CAST(X_Values.SValue AS DATE)) = 0
                                     )
                                )
                   ), 0) AS Y_Value
    FROM    ( SELECT  DISTINCT
                        *
              FROM      dbo.Spliter(@7, '|')
            ) AS X_Values", userId, beginTime, endTime, hallNo, complaintType, complaintLevel, detailType, detailXValue);


            Database db = gzswDB.GetInstance();
            return db.Fetch<Statistics_AnalysisDetailDto>(sql);
        }

        /// <summary>
        /// 获取员工投诉举报分析报表
        /// </summary>
        /// <param name="beginTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="hallNo">服务厅编码</param>
        /// <returns></returns>
        public static IList<Statistics_ComplaintReportPersonalDto> GetStatistics_ComplaintReportPersonalChart(string hallNo, DateTime? beginTime, DateTime? endTime)
        {
            var sql = Sql.Builder.Append(@";
WITH    ComplanStatAll
          AS ( SELECT   *
               FROM     STAT_COMPLAIN_STAFF_STAT_D
               WHERE    1 = 1
                        AND DATEDIFF(DAY, STAT_DT, @0) <= 0
                        AND DATEDIFF(DAY, STAT_DT, @1) >= 0
             )
    SELECT  Staffs.STAFF_ID ,
            Staffs.STAFF_NAM ,
            ComplanTypes.COMPLAIN_TYP_ID ,
            ComplanTypes.COMPLAIN_NAM ,
            ComplanLvls.LCode AS L_Code ,
            ComplanLvls.LName AS L_Name ,
            ComplanLvls.LValue AS L_Value ,
            ( SELECT    CASE ComplanLvls.LValue
                          WHEN 1 THEN ISNULL(SUM(NON_HANDLE_AMOUNT), 0)
                          WHEN 2 THEN ISNULL(SUM(HANDLE_AMOUNT), 0)
                          WHEN 3
                          THEN ( ISNULL(SUM(AMOUNT), 0)
                                 - ISNULL(SUM(NON_HANDLE_AMOUNT), 0)
                                 - ISNULL(SUM(HANDLE_AMOUNT), 0) )
                          ELSE ISNULL(SUM(AMOUNT), 0)
                        END
              FROM      ComplanStatAll
              WHERE     ComplanStatAll.STAFF_ID = Staffs.STAFF_ID
                        AND ComplanStatAll.COMPLAIN_NAM = ComplanTypes.COMPLAIN_NAM
            ) AS Count_Value
    FROM    SYS_STAFF AS Staffs
            LEFT JOIN WARN_COMPLAIN_TYP_CON AS ComplanTypes ON 1 = 1
            JOIN ( VALUES ( 'N', '未处理', 1), ( 'R', '已处理' , 2), ( 'U', '撤销', 3),
            ( 'H', '合计', 0) ) AS ComplanLvls ( LCode, LName, LValue ) ON 1 = 1
    WHERE   1 = 1
            AND Staffs.ORG_ID = @2", beginTime, endTime, hallNo);


            Database db = gzswDB.GetInstance();
            return db.Fetch<Statistics_ComplaintReportPersonalDto>(sql);
        }


        /// <summary>
        /// 获取服务厅投诉举报分析详细报表
        /// </summary>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <param name="personalId"></param>
        /// <param name="detailType"></param>
        /// <param name="detailXValue"></param>
        /// <param name="complaintType"></param>
        /// <param name="complaintLevel"></param>
        /// <returns></returns>
        public static IList<Statistics_AnalysisDetailDto> GetStatistics_ComplaintReportDetailPersonalChart(DateTime? beginTime, DateTime? endTime, string personalId, int detailType, string detailXValue, int? complaintType, int? complaintLevel)
        {
            var sql = Sql.Builder.Append(@";
WITH    ComplanStatAll
          AS ( SELECT   *
               FROM     ( SELECT    STAT_COMPLAIN_STAFF_STAT_D.STAT_DT ,
                                    STAT_COMPLAIN_STAFF_STAT_D.TIME_QUANTUM_CD ,
                                    STAT_COMPLAIN_STAFF_STAT_D.STAFF_ID ,
                                    ( SELECT TOP 1
                                                ComplanTypes.COMPLAIN_TYP_ID
                                      FROM      WARN_COMPLAIN_TYP_CON AS ComplanTypes
                                      WHERE     ComplanTypes.COMPLAIN_NAM = STAT_COMPLAIN_STAFF_STAT_D.COMPLAIN_NAM
                                    ) AS COMPLAIN_TYP ,
                                    ComplanLvls.LValue AS COMPLAIN_LEVEL ,
                                    ( CASE ComplanLvls.LValue
                                        WHEN 1
                                        THEN ISNULL(NON_HANDLE_AMOUNT, 0)
                                        WHEN 2 THEN ISNULL(HANDLE_AMOUNT, 0)
                                        WHEN 3
                                        THEN ( ISNULL(AMOUNT, 0)
                                               - ISNULL(NON_HANDLE_AMOUNT, 0)
                                               - ISNULL(HANDLE_AMOUNT, 0) )
                                        ELSE ISNULL(AMOUNT, 0)
                                      END ) AS AMOUNT
                          FROM      STAT_COMPLAIN_STAFF_STAT_D
                                    JOIN ( VALUES ( 'N', '未处理', 1),
                                    ( 'R', '已处理' , 2), ( 'U', '撤销', 3),
                                    ( 'H', '合计', 0) ) AS ComplanLvls ( LCode, LName, LValue ) ON 1 = 1
                        ) AS ComplanAll
               WHERE    1 = 1
                        AND ComplanAll.STAFF_ID = @0
                        AND DATEDIFF(DAY, STAT_DT, @1) <= 0
                        AND DATEDIFF(DAY, STAT_DT, @2) >= 0
                        AND ( ( @0 = ''
                                OR @0 IS NULL
                              )
                              OR ComplanAll.STAFF_ID = @0
                            )
                        AND ( 0 = @3
                              OR ComplanAll.COMPLAIN_TYP = @3
                            )
                        AND ( ComplanAll.COMPLAIN_LEVEL = @4 )
             )
    SELECT  X_Values.SValue AS X_Id ,
            ( CASE @5
                WHEN 2
                THEN CAST(CAST(X_Values.SValue AS DATE) AS NVARCHAR(10))
                WHEN 3 THEN CAST(CAST(X_Values.SValue AS DATE) AS NVARCHAR(7))
                WHEN 4 THEN CAST(CAST(X_Values.SValue AS DATE) AS NVARCHAR(4))
                ELSE CAST(( CAST(X_Values.SValue AS TINYINT) - 1 ) AS NVARCHAR(10))
                     + '点至' + X_Values.SValue + '点'
              END ) AS X_ShowName ,
            @5 AS X_Type ,
            ISNULL(( SELECT SUM(ComplanStatAll.AMOUNT)
                     FROM   ComplanStatAll
                     WHERE  1 = 1
                            AND ( ( 1 = @5
                                    AND ComplanStatAll.TIME_QUANTUM_CD = CAST(X_Values.SValue AS TINYINT)
                                  )
                                  OR ( 2 = @5
                                       AND DATEDIFF(DAY,
                                                    ComplanStatAll.STAT_DT,
                                                    CAST(X_Values.SValue AS DATE)) = 0
                                     )
                                  OR ( 3 = @5
                                       AND DATEDIFF(MONTH,
                                                    ComplanStatAll.STAT_DT,
                                                    CAST(X_Values.SValue AS DATE)) = 0
                                     )
                                  OR ( 4 = @5
                                       AND DATEDIFF(YEAR,
                                                    ComplanStatAll.STAT_DT,
                                                    CAST(X_Values.SValue AS DATE)) = 0
                                     )
                                )
                   ), 0) AS Y_Value
    FROM    ( SELECT  DISTINCT
                        *
              FROM      dbo.Spliter(@6, '|')
            ) AS X_Values", personalId, beginTime, endTime, complaintType, complaintLevel, detailType, detailXValue);


            Database db = gzswDB.GetInstance();
            return db.Fetch<Statistics_AnalysisDetailDto>(sql);
        }

    }
}
