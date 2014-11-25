using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using gzsw.model;
using gzsw.model.Subclasses;
using PetaPoco;

namespace gzsw.dal.dao
{
    public class STAT_STAFF_LARGE_BUSI_D_DAL
    {
        public static dynamic GetListBubByHall(DateTime beginStatMo, DateTime endStatMo, string hallNo, int pageIndex, int pageSize)
        {
            var db = gzswDB.GetInstance();

            var sql = Sql.Builder.Append(@"select 
                STAT_DT,
                TIME_QUANTUM_CD,
                ssd.HALL_NO,
                ssd.STAFF_ID,
                ssd.DLS_SERIALID,
                ssd.SERIALID,
                BUSI_CNT,
                CONVERT_BUSI_CNT,
                HANDLE_DUR,
                OVERTIME_HANDLE_CNT,
                LOCAL_CNT,
                h.HALL_NAM,
                s.STAFF_NAM,
                d.DLS_SERIALNAME,
                r.SERIALNAME
                from 
                STAT_STAFF_LARGE_BUSI_D ssd,
                SYS_HALL h,SYS_STAFF S,SYS_DLSERIAL d,SYS_DETAILSERIAL r
 
                where ssd.STAFF_ID=s.STAFF_ID
                and ssd.HALL_NO=h.HALL_NO
                and ssd.DLS_SERIALID=d.DLS_SERIALID
                and ssd.SERIALID=r.SERIALID ");

            sql.Append(@" AND ssd.[HALL_NO]=@0 ", hallNo);

            sql.Append(@" AND Ssd.STAT_DT>=@0 AND Ssd.STAT_DT<=@1 ", beginStatMo, endStatMo);

            var data = db.Page<dynamic>(pageIndex, pageSize, sql);
            data.ItemsPerPage = pageSize;
            return data;
        }

        /// <summary>
        /// 员工级别的业务办理分析
        /// </summary>
        /// <param name="orgId"></param>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <param name="period">时间段</param>
        /// <param name="field">排序字段</param>
        /// <param name="orderStr">排序 方式</param>
        /// <returns></returns>
        public static List<STAT_STAFF_LARGE_BUSI_D_Handle_SUB> GetStatList(string orgId, DateTime? beginTime, DateTime? endTime, int? period, string field = "STAFF_NAM", string orderStr = "ASC")
        {
            var db = gzswDB.GetInstance();
            var sql = Sql.Builder.Append(@"SELECT [DLS_SERIALID]
                                                ,SUM([BUSI_CNT]) AS [BUSI_CNT]
                                                ,SUM([CONVERT_BUSI_CNT]) AS [CONVERT_BUSI_CNT]
                                                ,SUM([HANDLE_DUR]) AS [HANDLE_DUR]
                                                ,SUM([OVERTIME_HANDLE_CNT]) AS [OVERTIME_HANDLE_CNT]
                                                ,SUM([LOCAL_CNT]) AS[LOCAL_CNT]
                                                ,DLS_SERIALNAME
                                                ,[STAFF_ID]
                                                ,STAFF_NAM
                                        FROM (
                                        SELECT LA.[STAT_DT]
                                          ,LA.[TIME_QUANTUM_CD]
                                          ,LA.[HALL_NO]
                                          ,LA.[STAFF_ID]
                                          ,LA.[DLS_SERIALID]
                                          ,LA.[SERIALID]
                                          ,LA.[BUSI_CNT]
                                          ,LA.[CONVERT_BUSI_CNT]
                                          ,LA.[HANDLE_DUR]
                                          ,LA.[OVERTIME_HANDLE_CNT]
                                          ,LA.[LOCAL_CNT]
                                          ,ST.STAFF_NAM
                                          ,DL.DLS_SERIALNAME
                                      FROM [STAT_STAFF_LARGE_BUSI_D] AS LA
                                      JOIN SYS_STAFF AS ST
                                      ON LA.STAFF_ID=ST.STAFF_ID
                                      JOIN SYS_DLSERIAL AS DL
                                      ON LA.DLS_SERIALID=DL.DLS_SERIALID
                                      WHERE 1=1 ");

            if (!string.IsNullOrEmpty(orgId))
            {
                sql.Append(@" AND LA.HALL_NO = @0 ", orgId);
            }
            if (period != null)
            {
                sql.Append(@" AND LA.TIME_QUANTUM_CD = @0 ", period);
            }
            if (beginTime != null)
            {
                sql.Append(@" AND LA.[STAT_DT] >= @0 ", beginTime.GetValueOrDefault().Date);
            }
            if (endTime != null)
            {
                sql.Append(@" AND LA.[STAT_DT] < @0 ", endTime.GetValueOrDefault().AddDays(1).Date);
            }


            sql.Append(@" ) AS T1 WHERE 1=1  GROUP BY 
		                        [DLS_SERIALID]
		                        ,DLS_SERIALNAME
		                        ,STAFF_ID
		                        ,STAFF_NAM ");

            //sql.Append(@" ORDER BY " + field + "  " + orderStr + " ");

            return db.Fetch<STAT_STAFF_LARGE_BUSI_D_Handle_SUB>(sql);
        }

        /// <summary>
        /// 服务厅级别的业务办理分析
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <param name="period"></param>
        /// <param name="field"></param>
        /// <param name="orderStr"></param>
        /// <returns></returns>
        public static List<STAT_STAFF_LARGE_BUSI_D_Handle_SUB> GetHallStatList(string userId, DateTime? beginTime, DateTime? endTime, int? period,  string field = "HALL_NAM", string orderStr = "ASC")
        {
            var db = gzswDB.GetInstance();
            var sql = Sql.Builder.Append(@"SELECT [DLS_SERIALID]
                                                ,SUM([BUSI_CNT]) AS [BUSI_CNT]
                                                ,SUM([CONVERT_BUSI_CNT]) AS [CONVERT_BUSI_CNT]
                                                ,SUM([HANDLE_DUR]) AS [HANDLE_DUR]
                                                ,SUM([OVERTIME_HANDLE_CNT]) AS [OVERTIME_HANDLE_CNT]
                                                ,SUM([LOCAL_CNT]) AS[LOCAL_CNT]
                                                ,DLS_SERIALNAME
                                                ,[HALL_NO]
                                                ,HALL_NAM
                                        FROM (
                                        SELECT LA.[STAT_DT]
                                                ,LA.[TIME_QUANTUM_CD]
                                                ,LA.[HALL_NO]
                                                ,LA.[STAFF_ID]
                                                ,LA.[DLS_SERIALID]
                                                ,LA.[SERIALID]
                                                ,LA.[BUSI_CNT]
                                                ,LA.[CONVERT_BUSI_CNT]
                                                ,LA.[HANDLE_DUR]
                                                ,LA.[OVERTIME_HANDLE_CNT]
                                                ,LA.[LOCAL_CNT]
                                                ,ST.STAFF_NAM
                                                ,DL.DLS_SERIALNAME
                                                ,HA.HALL_NAM
                                            FROM [STAT_STAFF_LARGE_BUSI_D] AS LA
                                            JOIN SYS_STAFF AS ST
                                            ON LA.STAFF_ID=ST.STAFF_ID
                                            JOIN SYS_DLSERIAL AS DL
                                            ON LA.DLS_SERIALID=DL.DLS_SERIALID
                                            JOIN SYS_HALL AS HA
                                            ON LA.HALL_NO=HA.HALL_NO
                                            JOIN SYS_USERORGANIZE AS UO
                                            ON LA.HALL_NO=UO.ORG_ID
                                            WHERE UO.[USER_ID]=@0  ", userId);
                                       

            if (period != null)
            {
                sql.Append(@" AND LA.TIME_QUANTUM_CD = @0 ", period);
            }
            if (beginTime != null)
            {
                sql.Append(@" AND LA.[STAT_DT] >= @0 ", beginTime.GetValueOrDefault().Date);
            }
            if (endTime != null)
            {
                sql.Append(@" AND LA.[STAT_DT] < @0 ", endTime.GetValueOrDefault().AddDays(1).Date);
            }

            sql.Append(@" ) AS T1 WHERE 1=1  GROUP BY [HALL_NO]
		                        ,[DLS_SERIALID]
		                        ,DLS_SERIALNAME
		                        ,HALL_NO
		                        ,HALL_NAM");

            sql.Append(@" ORDER BY " + field + "  " + orderStr + " ");

            return db.Fetch<STAT_STAFF_LARGE_BUSI_D_Handle_SUB>(sql);
        }

        /// <summary>
        /// 通过时间获取时间段的统计
        /// </summary>
        /// <param name="orgId"></param>
        /// <param name="userId"></param>
        /// <param name="serialId"></param>
        /// <param name="staffId"></param>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public static List<STAT_STAFF_LARGE_BUSI_D_Handle_SUB> GetDataStatList(string orgId,string userId,string serialId,
            string staffId, DateTime? beginTime,
            DateTime? endTime)
        {
            var db = gzswDB.GetInstance();
            var sql = Sql.Builder.Append(@"
                                        SELECT LA.[STAT_DT]
                                          ,LA.[TIME_QUANTUM_CD]
                                          ,LA.[HALL_NO]
                                          ,LA.[STAFF_ID]
                                          ,LA.[DLS_SERIALID]
                                          ,LA.[SERIALID]
                                          ,LA.[BUSI_CNT]
                                          ,LA.[CONVERT_BUSI_CNT]
                                          ,LA.[HANDLE_DUR]
                                          ,LA.[OVERTIME_HANDLE_CNT]
                                          ,LA.[LOCAL_CNT]
                                          ,ST.STAFF_NAM
                                      FROM [STAT_STAFF_LARGE_BUSI_D] AS LA
                                      JOIN SYS_USERORGANIZE AS UO
                                      ON LA.HALL_NO=UO.ORG_ID
                                      JOIN SYS_STAFF AS ST ON ST.STAFF_ID=LA.STAFF_ID
                                      WHERE 1=1 ");

            sql.Append(@" AND UO.[USER_ID]=@0 ", userId);

            if (!string.IsNullOrEmpty(orgId))
            {
                sql.Append(@" AND LA.HALL_NO = @0 ", orgId);
            }
            if (!string.IsNullOrEmpty(serialId))
            {
                sql.Append(@" AND LA.DLS_SERIALID = @0 ", serialId);
            }
            if (!string.IsNullOrEmpty(staffId))
            {
                sql.Append(@" AND LA.STAFF_ID = @0 ", staffId);
            }
            if (beginTime != null)
            {
                sql.Append(@" AND LA.[STAT_DT] >= @0 ", beginTime.GetValueOrDefault().Date);
            }
            if (endTime != null)
            {
                sql.Append(@" AND LA.[STAT_DT] < @0 ", endTime.GetValueOrDefault().AddDays(1).Date);
            }


            //sql.Append(@" ) AS T1 WHERE 1=1  GROUP BY STAT_DT  ");


            return db.Fetch<STAT_STAFF_LARGE_BUSI_D_Handle_SUB>(sql);
        }


        public static List<STAT_STAFF_LARGE_BUSI_D_Handle_SUB> GetStatList(string orgId, string userId,
            DateTime? beginTime,
            DateTime? endTime)
        {
            var db = gzswDB.GetInstance();
            var sql = Sql.Builder.Append(@"
                                        SELECT LA.[STAT_DT]
                                                ,LA.[TIME_QUANTUM_CD]
                                                ,LA.[HALL_NO]
                                                ,LA.[STAFF_ID]
                                                ,LA.[DLS_SERIALID]
                                                ,LA.[SERIALID]
                                                ,LA.[BUSI_CNT]
                                                ,LA.[CONVERT_BUSI_CNT]
                                                ,LA.[HANDLE_DUR]
                                                ,LA.[OVERTIME_HANDLE_CNT]
                                                ,LA.[LOCAL_CNT]
                                                ,ST.STAFF_NAM
                                                ,DL.DLS_SERIALNAME
                                                ,HA.HALL_NAM
                                            FROM [STAT_STAFF_LARGE_BUSI_D] AS LA
                                            JOIN SYS_STAFF AS ST
                                            ON LA.STAFF_ID=ST.STAFF_ID
                                            JOIN SYS_DLSERIAL AS DL
                                            ON LA.DLS_SERIALID=DL.DLS_SERIALID
                                            JOIN SYS_HALL AS HA
                                            ON LA.HALL_NO=HA.HALL_NO
                                            JOIN SYS_USERORGANIZE AS UO
                                            ON LA.HALL_NO=UO.ORG_ID
                                            WHERE UO.[USER_ID]=@0  ", userId);

            if (!string.IsNullOrEmpty(orgId))
            {
                sql.Append(@" AND LA.HALL_NO = @0 ", orgId);
            }
            if (beginTime != null)
            {
                sql.Append(@" AND LA.[STAT_DT] >= @0 ", beginTime.GetValueOrDefault().Date);
            }
            if (endTime != null)
            {
                sql.Append(@" AND LA.[STAT_DT] < @0 ", endTime.GetValueOrDefault().AddDays(1).Date);
            }
            

            return db.Fetch<STAT_STAFF_LARGE_BUSI_D_Handle_SUB>(sql);
        }

        public List<dynamic> GetStatsInfo(string[] halllist, DateTime? beginTime, DateTime? endTime)
        {
            var db = gzswDB.GetInstance();

            var sql = Sql.Builder.Append(@"				select 
				DLS_SERIALID,
				DLS_SERIALNAME,
				SUM(BUSI_CNT) as BUSI_CNT,
				SUM(CONVERT_BUSI_CNT) as CONVERT_BUSI_CNT,
				SUM(HANDLE_DUR) HANDLE_DUR,
				SUM(OVERTIME_HANDLE_CNT) as OVERTIME_HANDLE_CNT,
				SUM(LOCAL_CNT) as LOCAL_CNT
				from(
				select  
                h.HALL_NAM,
                s.STAFF_NAM,
                d.DLS_SERIALNAME,
                r.SERIALNAME,
                STAT_DT,
                TIME_QUANTUM_CD,
                ssd.HALL_NO,
                ssd.STAFF_ID,
                ssd.DLS_SERIALID,
                ssd.SERIALID,
                BUSI_CNT,
                CONVERT_BUSI_CNT,
                HANDLE_DUR,
                OVERTIME_HANDLE_CNT,
                LOCAL_CNT
                from 
                STAT_STAFF_LARGE_BUSI_D ssd,
                SYS_HALL h,SYS_STAFF S,SYS_DLSERIAL d,SYS_DETAILSERIAL r
 
                where ssd.STAFF_ID=s.STAFF_ID
                and ssd.HALL_NO=h.HALL_NO
                and ssd.DLS_SERIALID=d.DLS_SERIALID
                and ssd.SERIALID=r.SERIALID  ");
            if (null != halllist && halllist.Length > 0)
                sql.Append(" AND ssd.HALL_NO in (@hall)", new { hall = halllist });
            if (beginTime.HasValue)
            {
                sql.Append(@" and ssd.STAT_DT>='"+beginTime.Value+"'");
            }
            if (endTime.HasValue)
            {
                sql.Append(@" and ssd.STAT_DT<='"+endTime.Value+"'");
            }
            sql.Append(@" )t
                group by t.DLS_SERIALID,DLS_SERIALNAME");


            var data = db.Fetch<dynamic>(sql); 
            return data;
        }
    }
}
