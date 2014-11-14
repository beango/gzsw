using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PetaPoco;
using gzsw.model;

namespace gzsw.dal.dao
{
    /// <summary>
    /// 当日台账业务明细
    /// </summary>
    public static class SYS_CURRYWHIST_DAL
    {
        /// <summary>
        /// 获取2次办理明细总数表
        /// </summary>
        /// <param name="orgId"></param>
        /// <param name="beginMo"></param>
        /// <param name="endMo"></param>
        /// <param name="nsrsbm"></param>
        /// <param name="nsrmc"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static Page<dynamic> GetMergeList(string orgId, int beginMo, int endMo, string nsrsbm, string nsrmc, int pageIndex, int pageSize)
        {
            var db = gzswDB.GetInstance();
            var sql = Sql.Builder.Append(@"SELECT * FROM (");
            sql.Append(@"SELECT YW_NSRSBM,YW_NSRMC,YW_STIME_MO,YW_SYSNO,COUNT(*) AS TOTAL,MAX(YW_STIME) AS LASTTIME  FROM(
                                            SELECT [YW_TRANSCODEID]
                                                  ,[YW_XH]
                                                  ,[YW_QTRANSCODEID]
                                                  ,[YW_SYSNO]
                                                  ,[YW_NUMBER]
                                                  ,[YW_QSERIALID]
                                                  ,[YW_DLSERIALID]
                                                  ,[YW_DETAILSERIALID]
                                                  ,[YW_COUNTER]
                                                  ,[YW_SNO]
                                                  ,[YW_STIME]
                                                  ,[YW_ETIME]
                                                  ,[YW_BLTIME]
                                                  ,[YW_NSRSBM]
                                                  ,[YW_NSRMC]
                                                  ,[YW_SWJGDM]
                                                  ,[YW_ISFINISHED]
                                                  ,CAST(YEAR(YW_STIME) AS varchar)+CAST(MONTH(YW_STIME) AS varchar) AS YW_STIME_MO
                                              FROM [SYS_CURRYWHIST]
                                              UNION ALL
                                              SELECT [HYW_TRANSCODEID] AS [YW_TRANSCODEID]
                                                  ,[HYW_XH] AS [YW_XH]
                                                  ,[HYW_QTRANSCODEID] AS[YW_QTRANSCODEID]
                                                  ,[HYW_SYSNO] AS [YW_SYSNO]
                                                  ,[HYW_NUMBER] AS [YW_NUMBER]
                                                  ,[HYW_QSERIALID] AS [YW_QSERIALID]
                                                  ,[HYW_DLSERIALID] AS [YW_DLSERIALID]
                                                  ,[HYW_DETAILSERIALID] AS [YW_DETAILSERIALID]
                                                  ,[HYW_COUNTER] AS [YW_COUNTER]
                                                  ,[HYW_SNO] AS [YW_SNO]
                                                  ,[HYW_STIME] AS [YW_STIME]
                                                  ,[HYW_ETIME] AS [YW_ETIME]
                                                  ,[HYW_BLTIME] AS [YW_BLTIME]
                                                  ,[HYW_NSRSBM] AS [YW_NSRSBM]
                                                  ,[HYW_NSRMC] AS [YW_NSRMC]
                                                  ,[HYW_SWJGDM] AS [YW_SWJGDM]
                                                  ,[HYW_ISFINISHED] AS [YW_ISFINISHED]
                                                  ,CAST(YEAR(HYW_STIME) AS varchar)+CAST(MONTH(HYW_STIME) AS varchar) AS YW_STIME_MO
                                              FROM [SYS_YWHIST]
                                              ) AS T1 
                                              GROUP BY YW_NSRSBM,YW_NSRMC,YW_STIME_MO,YW_SYSNO ");

            sql.Append(@" ) AS Table1 ");

            sql.Append(@" WHERE YW_SYSNO=@0 ", orgId);
            sql.Append(@" AND YW_STIME_MO >= @0 ", beginMo);
            sql.Append(@" AND YW_STIME_MO <= @0 ", endMo);

            if (!string.IsNullOrEmpty(nsrsbm))
            {
                sql.Append(@" AND YW_NSRSBM like @0 ", "%" + nsrsbm + "%");
            }
            if (!string.IsNullOrEmpty(nsrmc))
            {
                sql.Append(@" AND YW_NSRMC like @0 ", "%" + nsrmc + "%");
            }

            return db.Page<dynamic>(pageIndex, pageSize, sql);
        }

        /// <summary>
        /// 获取2次办理明细项列表
        /// </summary>
        /// <param name="orgId"></param>
        /// <param name="beginMo"></param>
        /// <param name="endMo"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static Page<dynamic> GetMergeDetailList(string orgId,string nsrsbm, int beginMo, int endMo, int pageIndex, int pageSize)
        {
            var db = gzswDB.GetInstance();
            var sql = Sql.Builder.Append(@"SELECT T1.*,D.DLS_SERIALNAME,DS.SERIALNAME,ST.STAFF_NAM FROM(
                                            SELECT [YW_TRANSCODEID]
                                                  ,[YW_XH]
                                                  ,[YW_QTRANSCODEID]
                                                  ,[YW_SYSNO]
                                                  ,[YW_NUMBER]
                                                  ,[YW_QSERIALID]
                                                  ,[YW_DLSERIALID]
                                                  ,[YW_DETAILSERIALID]
                                                  ,[YW_COUNTER]
                                                  ,[YW_SNO]
                                                  ,[YW_STIME]
                                                  ,[YW_ETIME]
                                                  ,[YW_BLTIME]
                                                  ,[YW_NSRSBM]
                                                  ,[YW_NSRMC]
                                                  ,[YW_SWJGDM]
                                                  ,[YW_ISFINISHED]
                                                  ,CAST(YEAR(YW_STIME) AS varchar)+CAST(MONTH(YW_STIME) AS varchar) AS YW_STIME_MO
                                              FROM [SYS_CURRYWHIST]
                                              UNION ALL
                                              SELECT [HYW_TRANSCODEID] AS [YW_TRANSCODEID]
                                                  ,[HYW_XH] AS [YW_XH]
                                                  ,[HYW_QTRANSCODEID] AS[YW_QTRANSCODEID]
                                                  ,[HYW_SYSNO] AS [YW_SYSNO]
                                                  ,[HYW_NUMBER] AS [YW_NUMBER]
                                                  ,[HYW_QSERIALID] AS [YW_QSERIALID]
                                                  ,[HYW_DLSERIALID] AS [YW_DLSERIALID]
                                                  ,[HYW_DETAILSERIALID] AS [YW_DETAILSERIALID]
                                                  ,[HYW_COUNTER] AS [YW_COUNTER]
                                                  ,[HYW_SNO] AS [YW_SNO]
                                                  ,[HYW_STIME] AS [YW_STIME]
                                                  ,[HYW_ETIME] AS [YW_ETIME]
                                                  ,[HYW_BLTIME] AS [YW_BLTIME]
                                                  ,[HYW_NSRSBM] AS [YW_NSRSBM]
                                                  ,[HYW_NSRMC] AS [YW_NSRMC]
                                                  ,[HYW_SWJGDM] AS [YW_SWJGDM]
                                                  ,[HYW_ISFINISHED] AS [YW_ISFINISHED]
                                                  ,CAST(YEAR(HYW_STIME) AS varchar)+CAST(MONTH(HYW_STIME) AS varchar) AS YW_STIME_MO
                                              FROM [SYS_YWHIST]
                                              ) AS T1 
                                               JOIN SYS_DLSERIAL AS D  
                                               ON T1.YW_DLSERIALID = D.DLS_SERIALID
                                               JOIN SYS_DETAILSERIAL AS DS 
                                               ON T1.YW_DETAILSERIALID = DS.SERIALID
                                               JOIN SYS_STAFF AS ST
                                               ON T1.YW_SNO=ST.STAFF_ID ");

            sql.Append(@" WHERE YW_SYSNO=@0 AND YW_NSRSBM=@1 ", orgId, nsrsbm);
            sql.Append(@" AND YW_STIME_MO >= @0 ", beginMo);
            sql.Append(@" AND YW_STIME_MO <= @0 ", endMo);

            return db.Page<dynamic>(pageIndex,pageSize,sql);
        }

        /// <summary>
        /// 获取当日超时办结的台账明细
        /// </summary>
        /// <param name="orgId">服务厅编码</param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static object GetTimeoutDealFinshPager(string orgId, int pageIndex, int pageSize)
        {
            var db = gzswDB.GetInstance();

            var sql = Sql.Builder.Append(@"SELECT  YW_TRANSCODEID AS TRANSCODEID ,
        YW_SYSNO AS SYSNO ,
        YW_NUMBER AS NUMBER ,
        YW_DETAILSERIALID AS DETAILSERIALID ,
        YW_COUNTER AS [COUNTER] ,
        YW_BLTIME AS BLTIME ,
        YW_NSRSBM AS NSRSBM ,
        YW_NSRMC AS NSRMC ,
        SYS_HALL.HALL_NAM AS HALLNAME ,
        SYS_DETAILSERIAL.SERIALNAME ,
        ( dbo.isOvertime(YW_DETAILSERIALID, YW_BLTIME) ) AS IsOvertime
FROM    SYS_CURRYWHIST
        JOIN SYS_HALL ON SYS_HALL.HALL_NO = SYS_CURRYWHIST.YW_SYSNO
        JOIN SYS_DETAILSERIAL ON SYS_DETAILSERIAL.SERIALID = SYS_CURRYWHIST.YW_DETAILSERIALID
WHERE   1 = 1
        AND YW_ISFINISHED = 1");

            sql.Append(@" AND YW_SYSNO = @0 ", orgId);

            var data = db.Page<dynamic>(pageIndex, pageSize, sql);
            data.ItemsPerPage = pageSize;

            return data;
        }

        /// <summary>
        /// 获取明细
        /// </summary>
        /// <returns></returns>
        public static dynamic GetOne(string id)
        {
            var db = gzswDB.GetInstance();

            var sql = Sql.Builder.Append(@"SELECT  YW_TRANSCODEID AS TRANSCODEID ,
        YW_XH AS XH ,
        YW_QTRANSCODEID AS QTRANSCODEID ,
        YW_SYSNO AS SYSNO ,
        YW_NUMBER AS NUMBER ,
        YW_QSERIALID AS QSERIALID ,
        YW_DLSERIALID AS DLSERIALID ,
        YW_DETAILSERIALID AS DETAILSERIALID ,
        YW_COUNTER AS [COUNTER] ,
        YW_SNO AS SNO ,
        YW_STIME AS STIME ,
        YW_ETIME AS ETIME ,
        YW_BLTIME AS BLTIME ,
        YW_NSRSBM AS NSRSBM ,
        YW_NSRMC AS NSRMC ,
        YW_SWJGDM AS SWJGDM ,
        YW_ISFINISHED AS ISFINISHED ,
        ( dbo.isOvertime(YW_DETAILSERIALID, YW_BLTIME) ) AS IsOvertime ,
        YW_HALL.HALL_NAM AS YWHALLNAME ,
        SYS_DETAILSERIAL.SERIALNAME ,
        SYS_QUEUESERIAL.Q_SERIALNAME AS QSERIALNAME ,
        SYS_DLSERIAL.DLS_SERIALNAME AS DLSSERIALNAME ,
        SYS_STAFF.STAFF_NAM AS STAFFNAME ,
		BELONG_HALL.HALL_NAM AS BELHALLNAME
FROM    SYS_CURRYWHIST
        JOIN SYS_HALL AS YW_HALL ON YW_HALL.HALL_NO = SYS_CURRYWHIST.YW_SYSNO
        JOIN SYS_DETAILSERIAL ON SYS_DETAILSERIAL.SERIALID = SYS_CURRYWHIST.YW_DETAILSERIALID
        JOIN SYS_QUEUESERIAL ON SYS_QUEUESERIAL.Q_SERIALID = SYS_CURRYWHIST.YW_QSERIALID
        JOIN SYS_DLSERIAL ON SYS_DLSERIAL.DLS_SERIALID = SYS_CURRYWHIST.YW_DLSERIALID
        JOIN SYS_STAFF ON SYS_STAFF.STAFF_ID = SYS_CURRYWHIST.YW_SNO
		JOIN SYS_HALL AS BELONG_HALL ON BELONG_HALL.HALL_NO = SYS_CURRYWHIST.YW_SWJGDM
WHERE   1 = 1");

            sql.Append(@" AND YW_TRANSCODEID = @0 ", id);

            return db.FirstOrDefault<dynamic>(sql);
        }

        /// <summary>
        /// 获取当日同城通办的台账明细
        /// </summary>
        /// <param name="orgId">服务厅编码</param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static object GetSameCityDealPager(string orgId, int pageIndex, int pageSize)
        {
            var db = gzswDB.GetInstance();

            var sql = Sql.Builder.Append(@"SELECT  YW_TRANSCODEID AS TRANSCODEID ,
        YW_SYSNO AS SYSNO ,
        YW_NUMBER AS NUMBER ,
        YW_DETAILSERIALID AS DETAILSERIALID ,
        YW_COUNTER AS [COUNTER] ,
        YW_BLTIME AS BLTIME ,
        YW_NSRSBM AS NSRSBM ,
        YW_NSRMC AS NSRMC ,
        YW_HALL.HALL_NAM AS YWHALLNAME ,
        SYS_DETAILSERIAL.SERIALNAME ,
        BELONG_HALL.HALL_NAM AS BELHALLNAME
FROM    SYS_CURRYWHIST
        JOIN SYS_HALL AS YW_HALL ON YW_HALL.HALL_NO = SYS_CURRYWHIST.YW_SYSNO
        JOIN SYS_DETAILSERIAL ON SYS_DETAILSERIAL.SERIALID = SYS_CURRYWHIST.YW_DETAILSERIALID
        JOIN SYS_HALL AS BELONG_HALL ON BELONG_HALL.HALL_NO = SYS_CURRYWHIST.YW_SWJGDM
WHERE   1 = 1
        AND SYS_CURRYWHIST.YW_SYSNO <> SYS_CURRYWHIST.YW_SWJGDM");

            sql.Append(@" AND YW_SYSNO = @0 ", orgId);

            var data = db.Page<dynamic>(pageIndex, pageSize, sql);
            data.ItemsPerPage = pageSize;

            return data;
        }

        /// <summary>
        /// 获取当日在当月2次办理人的台账明细
        /// </summary>
        /// <param name="orgId">服务厅编码</param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static object GetTwiceDealPager(string orgId, int pageIndex, int pageSize)
        {
            var db = gzswDB.GetInstance();

            var sql = Sql.Builder.Append(@"SELECT  YW_TRANSCODEID AS TRANSCODEID ,
        YW_SYSNO AS SYSNO ,
        YW_NUMBER AS NUMBER ,
        YW_DETAILSERIALID AS DETAILSERIALID ,
        YW_COUNTER AS [COUNTER] ,
        YW_BLTIME AS BLTIME ,
        YW_NSRSBM AS NSRSBM ,
        YW_NSRMC AS NSRMC ,
        SYS_HALL.HALL_NAM AS HALLNAME ,
        SYS_DETAILSERIAL.SERIALNAME ,
        YW_ISFINISHED AS ISFINISHED
FROM    SYS_CURRYWHIST
        JOIN SYS_HALL ON SYS_HALL.HALL_NO = SYS_CURRYWHIST.YW_SYSNO
        JOIN SYS_DETAILSERIAL ON SYS_DETAILSERIAL.SERIALID = SYS_CURRYWHIST.YW_DETAILSERIALID
WHERE   1 = 1");

            sql.Append(@" AND YW_SYSNO = @0 ", orgId);

            sql.Append(@" AND YW_NSRSBM IN (
        SELECT DISTINCT
                HYW_NSRSBM
        FROM    SYS_YWHIST
        WHERE   SYS_YWHIST.HYW_SYSNO = @0
                AND DATEDIFF(MONTH, SYS_YWHIST.HYW_STIME, GETDATE()) = 0 ) ", orgId);

            var data = db.Page<dynamic>(pageIndex, pageSize, sql);
            data.ItemsPerPage = pageSize;

            return data;
        }
    }
}
