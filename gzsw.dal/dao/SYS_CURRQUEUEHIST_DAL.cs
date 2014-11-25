using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gzsw.model;
using gzsw.model.Subclasses;
using PetaPoco;

namespace gzsw.dal.dao
{
    /// <summary>
    /// 当日排队明细
    /// </summary>
    public static class SYS_CURRQUEUEHIST_DAL
    {
        /// <summary>
        /// 获取当日排队明细
        /// </summary>
        /// <param name="orgId">服务ID</param>
        /// <param name="state">状态，10：未交易完结（非正常）1：交易完结（正常）</param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static Page<SYS_CURRQUEUEHIST_SUB> GetListSub(string orgId, int state, int pageIndex, int pageSize)
        {
            var db = gzswDB.GetInstance();

            var sql = Sql.Builder.Append(@"SELECT C.[CHQUEUE_TRANSCODEID]
                                                  ,C.[CHQUEUE_SYSNO]
                                                  ,C.[CHQUEUE_NUMBER]
                                                  ,C.[CHQUEUE_TICKETTIME]
                                                  ,C.[CHQUEUE_QSERIALID]
                                                  ,C.[CHQUEUE_COUNTER]
                                                  ,C.[CHQUEUE_SNO]
                                                  ,C.[CHQUEUE_CALLTIME]
                                                  ,C.[CHQUEUE_WAITTIME]
                                                  ,C.[CHQUEUE_STIME]
                                                  ,C.[CHQUEUE_ETIME]
                                                  ,C.[CHQUEUE_BLTIME]
                                                  ,C.[CHQUEUE_PJRESULT]
                                                  ,C.[CHQQUEUE_PJTIME]
                                                  ,C.[CHQUEUE_YWBS]
                                                  ,C.[CHQUEUE_NSRSBM]
                                                  ,C.[CHQUEUE_NSRMC]
                                                  ,C.[CHQUEUE_SWJGDM]
                                                  ,C.[CHQUEUE_TICKETTYPE]
                                                  ,C.[CHQUEUE_STATUS]
                                                  ,C.[CHQUEUE_ISFINISHED]
                                                  ,Q.Q_SERIALNAME
                                                  ,ST.STAFF_NAM
                                                  ,H.HALL_NAM
                                              FROM [SYS_CURRQUEUEHIST] AS C
	                                            JOIN SYS_STAFF AS ST
	                                            ON C.CHQUEUE_SNO=ST.STAFF_ID
	                                            JOIN SYS_HALL AS H
	                                            ON C.CHQUEUE_SYSNO=H.HALL_NO
	                                            JOIN SYS_QUEUESERIAL AS Q
	                                            ON C.CHQUEUE_QSERIALID=Q.Q_SERIALID WHERE 1=1 ");

            sql.Append(@" AND C.[CHQUEUE_SYSNO]=@0 AND C.CHQUEUE_ISFINISHED=@1 AND C.CHQUEUE_PJRESULT>-1 ", orgId, state);

            return db.Page<SYS_CURRQUEUEHIST_SUB>(pageIndex, pageSize, sql);
        }

        public static SYS_CURRQUEUEHIST_SUB GetSub(string id)
        {
            var db = gzswDB.GetInstance();

            var sql = Sql.Builder.Append(@"SELECT C.[CHQUEUE_TRANSCODEID]
                                                  ,C.[CHQUEUE_SYSNO]
                                                  ,C.[CHQUEUE_NUMBER]
                                                  ,C.[CHQUEUE_TICKETTIME]
                                                  ,C.[CHQUEUE_QSERIALID]
                                                  ,C.[CHQUEUE_COUNTER]
                                                  ,C.[CHQUEUE_SNO]
                                                  ,C.[CHQUEUE_CALLTIME]
                                                  ,C.[CHQUEUE_WAITTIME]
                                                  ,C.[CHQUEUE_STIME]
                                                  ,C.[CHQUEUE_ETIME]
                                                  ,C.[CHQUEUE_BLTIME]
                                                  ,C.[CHQUEUE_PJRESULT]
                                                  ,C.[CHQQUEUE_PJTIME]
                                                  ,C.[CHQUEUE_YWBS]
                                                  ,C.[CHQUEUE_NSRSBM]
                                                  ,C.[CHQUEUE_NSRMC]
                                                  ,C.[CHQUEUE_SWJGDM]
                                                  ,C.[CHQUEUE_TICKETTYPE]
                                                  ,C.[CHQUEUE_STATUS]
                                                  ,C.[CHQUEUE_ISFINISHED]
                                                  ,Q.Q_SERIALNAME
                                                  ,ST.STAFF_NAM
                                                  ,H.HALL_NAM
                                              FROM [SYS_CURRQUEUEHIST] AS C
	                                            JOIN SYS_STAFF AS ST
	                                            ON C.CHQUEUE_SNO=ST.STAFF_ID
	                                            JOIN SYS_HALL AS H
	                                            ON C.CHQUEUE_SYSNO=H.HALL_NO
	                                            JOIN SYS_QUEUESERIAL AS Q
	                                            ON C.CHQUEUE_QSERIALID=Q.Q_SERIALID WHERE 1=1 ");


            sql.Append(@" AND C.CHQUEUE_TRANSCODEID=@0 ", id);

            return db.FirstOrDefault<SYS_CURRQUEUEHIST_SUB>(sql);
        }

        /// <summary>
        /// 获取排队明细 今天加历史
        /// </summary>
        /// <param name="orgId">服务厅</param>
        /// <param name="beginTime">开始时间</param>
        /// <param name="endTime">线束时间</param>
        /// <param name="number">排队号码</param>
        /// <param name="counter">窗口编号</param>
        /// <param name="staffId">员工代码</param>
        /// <param name="nsrsbm">纳税人识别码</param>
        /// <param name="tickettype">取号方式</param>
        /// <param name="status">状态</param>
        /// <param name="isfinished">完成状态</param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="ywbs">该排队纳税人总共办理的业务笔数</param>
        /// <param name="transcodeId"></param>
        /// <returns></returns>
        public static Page<SYS_CURRQUEUEHIST_SUB> GetMergeList(string orgId, DateTime? beginTime, DateTime? endTime,
            string number, int? counter, string staffId, string nsrsbm, int? tickettype, int? status, int? isfinished,
            int pageIndex, int pageSize, bool? ywbs = null, string transcodeId=null)
        {

            var db = gzswDB.GetInstance();
            var sql = Sql.Builder.Append(@"SELECT T1.*,ST.STAFF_NAM FROM ( select QU.[CHQUEUE_TRANSCODEID]
                                              , QU.[CHQUEUE_SYSNO]
                                              , QU.[CHQUEUE_NUMBER]
                                              , QU.[CHQUEUE_TICKETTIME]
                                              , QU.[CHQUEUE_QSERIALID]
                                              , QU.[CHQUEUE_COUNTER]
                                              , QU.[CHQUEUE_SNO]
                                              , QU.[CHQUEUE_CALLTIME]
                                              , QU.[CHQUEUE_WAITTIME]
                                              , QU.[CHQUEUE_STIME]
                                              , QU.[CHQUEUE_ETIME]
                                              , QU.[CHQUEUE_BLTIME]
                                              , QU.[CHQUEUE_PJRESULT]
                                              , QU.[CHQQUEUE_PJTIME]
                                              , QU.[CHQUEUE_YWBS]
                                              , QU.[CHQUEUE_NSRSBM]
                                              , QU.[CHQUEUE_NSRMC]
                                              , QU.[CHQUEUE_SWJGDM]
                                              , QU.[CHQUEUE_TICKETTYPE]
                                              , QU.[CHQUEUE_STATUS]
                                              , QU.[CHQUEUE_ISFINISHED]
                                              ,Q.Q_SERIALNAME 
                                        from SYS_CURRQUEUEHIST AS QU
                                         JOIN SYS_QUEUESERIAL AS Q
	                                     ON QU.CHQUEUE_QSERIALID=Q.Q_SERIALID   WHERE 1=1  ");

            addSeleteWhere(sql, orgId, beginTime, endTime, number, counter, staffId, nsrsbm, tickettype, status,
                isfinished);
            sql.Append(@" union ");
            sql.Append(@" select  QU.[HQUEUE_TRANSCODEID] AS [CHQUEUE_TRANSCODEID]
                                              , QU.[HQUEUE_SYSNO]	AS [CHQUEUE_SYSNO]
                                              , QU.[HQUEUE_NUMBER]	AS [CHQUEUE_NUMBER]
                                              , QU.[HQUEUE_TICKETTIME] AS [CHQUEUE_TICKETTIME]
                                              , QU.[HQUEUE_QSERIALID] AS [CHQUEUE_QSERIALID]	
                                              , QU.[HQUEUE_COUNTER] AS [CHQUEUE_COUNTER]
                                              , QU.[HQUEUE_SNO] AS [CHQUEUE_SNO]
                                              , QU.[HQUEUE_CALLTIME] AS [CHQUEUE_CALLTIME]
                                              , QU.[HQUEUE_WAITTIME] AS [CHQUEUE_WAITTIME]
                                              , QU.[HQUEUE_STIME] AS [CHQUEUE_STIME]
                                              , QU.[HQUEUE_ETIME] AS [CHQUEUE_ETIME]
                                              , QU.[HQUEUE_BLTIME] AS [CHQUEUE_BLTIME]
                                              , QU.[HQUEUE_PJRESULT] AS [CHQUEUE_PJRESULT]
                                              , QU.[HQQUEUE_PJTIME] AS [CHQQUEUE_PJTIME]
                                              , QU.[HQUEUE_YWBS] AS [CHQUEUE_YWBS]
                                              , QU.[HQUEUE_NSRSBM] AS [CHQUEUE_NSRSBM]
                                              , QU.[HQUEUE_NSRMC] AS [CHQUEUE_NSRMC]
                                              , QU.[HQUEUE_SWJGDM] AS [CHQUEUE_SWJGDM]
                                              , QU.[HQUEUE_TICKETTYPE] AS [CHQUEUE_TICKETTYPE]
                                              , QU.[HQUEUE_STATUS]AS [CHQUEUE_STATUS] 
                                              , QU.[HQUEUE_ISFINISHED]AS [CHQUEUE_ISFINISHED]
                                              ,Q.Q_SERIALNAME 
                                               from SYS_QUEUEHIST AS QU
                                             JOIN SYS_QUEUESERIAL AS Q
	                                         ON QU.HQUEUE_QSERIALID=Q.Q_SERIALID WHERE 1=1 ");


            addSeleteWhere(sql, orgId, beginTime, endTime, number, counter, staffId, nsrsbm, tickettype, status,
                isfinished,false);

            sql.Append(@" ) AS T1 
                    JOIN SYS_STAFF AS ST
                    ON T1.CHQUEUE_SNO=ST.STAFF_ID
                    WHERE 1=1 ");

            if (ywbs != null && ywbs==true)
            {
                sql.Append(" AND T1.CHQUEUE_YWBS > 1 ");
            }
            if (!string.IsNullOrEmpty(transcodeId))
            {
                sql.Append(" AND T1.CHQUEUE_TRANSCODEID like @0 ", "%" + transcodeId + "%");
            }

            return db.Page<SYS_CURRQUEUEHIST_SUB>(pageIndex, pageSize, sql);
        }
        
        /// <summary>
        /// 获取排队明细 今天加历史
        /// </summary>
        /// <param name="orgId">服务厅</param>
        /// <param name="beginTime">开始时间</param>
        /// <param name="endTime">线束时间</param>
        /// <param name="number">排队号码</param>
        /// <param name="counter">窗口编号</param>
        /// <param name="staffId">员工代码</param>
        /// <param name="nsrsbm">纳税人识别码</param>
        /// <param name="tickettype">取号方式</param>
        /// <param name="status">状态</param>
        /// <param name="isfinished">完成状态</param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static Page<SYS_CURRQUEUEHIST_SUB> GetMergeListByCity(string orgId, DateTime? beginTime, DateTime? endTime,
            string number, int? counter, string staffId, string nsrsbm, int? tickettype, int? status, int? isfinished, int pageIndex, int pageSize)
        {

            var db = gzswDB.GetInstance();
            var sql = Sql.Builder.Append(@"SELECT * FROM ( select QU.[CHQUEUE_TRANSCODEID]
                                              , QU.[CHQUEUE_SYSNO]
                                              , QU.[CHQUEUE_NUMBER]
                                              , QU.[CHQUEUE_TICKETTIME]
                                              , QU.[CHQUEUE_QSERIALID]
                                              , QU.[CHQUEUE_COUNTER]
                                              , QU.[CHQUEUE_SNO]
                                              , QU.[CHQUEUE_CALLTIME]
                                              , QU.[CHQUEUE_WAITTIME]
                                              , QU.[CHQUEUE_STIME]
                                              , QU.[CHQUEUE_ETIME]
                                              , QU.[CHQUEUE_BLTIME]
                                              , QU.[CHQUEUE_PJRESULT]
                                              , QU.[CHQQUEUE_PJTIME]
                                              , QU.[CHQUEUE_YWBS]
                                              , QU.[CHQUEUE_NSRSBM]
                                              , QU.[CHQUEUE_NSRMC]
                                              , QU.[CHQUEUE_SWJGDM]
                                              , QU.[CHQUEUE_TICKETTYPE]
                                              , QU.[CHQUEUE_STATUS]
                                              , QU.[CHQUEUE_ISFINISHED]
                                              ,Q.Q_SERIALNAME 
                                        from SYS_CURRQUEUEHIST AS QU
                                         JOIN SYS_QUEUESERIAL AS Q
	                                     ON QU.CHQUEUE_QSERIALID=Q.Q_SERIALID   WHERE 1=1  ");

            addSeleteWhere(sql, orgId, beginTime, endTime, number, counter, staffId, nsrsbm, tickettype, status,
                isfinished);
            sql.Append(@" and  QU.[CHQUEUE_SWJGDM]<>QU.CHQUEUE_SYSNO ");
            sql.Append(@" union ");
            sql.Append(@" select  QU.[HQUEUE_TRANSCODEID] AS [CHQUEUE_TRANSCODEID]
                                              , QU.[HQUEUE_SYSNO]	AS [CHQUEUE_SYSNO]
                                              , QU.[HQUEUE_NUMBER]	AS [CHQUEUE_NUMBER]
                                              , QU.[HQUEUE_TICKETTIME] AS [CHQUEUE_TICKETTIME]
                                              , QU.[HQUEUE_QSERIALID] AS [CHQUEUE_QSERIALID]	
                                              , QU.[HQUEUE_COUNTER] AS [CHQUEUE_COUNTER]
                                              , QU.[HQUEUE_SNO] AS [CHQUEUE_SNO]
                                              , QU.[HQUEUE_CALLTIME] AS [CHQUEUE_CALLTIME]
                                              , QU.[HQUEUE_WAITTIME] AS [CHQUEUE_WAITTIME]
                                              , QU.[HQUEUE_STIME] AS [CHQUEUE_STIME]
                                              , QU.[HQUEUE_ETIME] AS [CHQUEUE_ETIME]
                                              , QU.[HQUEUE_BLTIME] AS [CHQUEUE_BLTIME]
                                              , QU.[HQUEUE_PJRESULT] AS [CHQUEUE_PJRESULT]
                                              , QU.[HQQUEUE_PJTIME] AS [CHQQUEUE_PJTIME]
                                              , QU.[HQUEUE_YWBS] AS [CHQUEUE_YWBS]
                                              , QU.[HQUEUE_NSRSBM] AS [CHQUEUE_NSRSBM]
                                              , QU.[HQUEUE_NSRMC] AS [CHQUEUE_NSRMC]
                                              , QU.[HQUEUE_SWJGDM] AS [CHQUEUE_SWJGDM]
                                              , QU.[HQUEUE_TICKETTYPE] AS [CHQUEUE_TICKETTYPE]
                                              , QU.[HQUEUE_STATUS]AS [CHQUEUE_STATUS] 
                                              , QU.[HQUEUE_ISFINISHED]AS [CHQUEUE_ISFINISHED]
                                              ,Q.Q_SERIALNAME 
                                               from SYS_QUEUEHIST AS QU
                                             JOIN SYS_QUEUESERIAL AS Q
	                                         ON QU.HQUEUE_QSERIALID=Q.Q_SERIALID WHERE 1=1 ");


            addSeleteWhere(sql, orgId, beginTime, endTime, number, counter, staffId, nsrsbm, tickettype, status,
                isfinished, false);
            sql.Append(@" and  QU.[CHQUEUE_SWJGDM]<>QU.CHQUEUE_SYSNO ");
            sql.Append(@" ) AS T1 ");

            return db.Page<SYS_CURRQUEUEHIST_SUB>(pageIndex, pageSize, sql);
        }
        private static void addSeleteWhere(Sql sql,  string orgId, DateTime? beginTime, DateTime? endTime,
            string number, int? counter, string staffId, string nsrsbm, int? tickettype, int? status, int? isfinished,bool IsHQUEUE=true)
        {
            sql.Append(IsHQUEUE ? @" AND  QU.CHQUEUE_SYSNO =@0   " : @" AND  QU.HQUEUE_SYSNO =@0   ", orgId);
            if (beginTime != null)
            {
                beginTime = beginTime.GetValueOrDefault().Date;
                sql.Append(IsHQUEUE ? @" AND  QU.CHQUEUE_TICKETTIME >= @0 " : @" AND  QU.HQUEUE_TICKETTIME >= @0 ",
                    beginTime);
            }
            if (endTime != null)
            {
                endTime = endTime.GetValueOrDefault().Date.AddDays(1);
                sql.Append(IsHQUEUE ? @" AND  QU.CHQUEUE_TICKETTIME < @0 " : @" AND  QU.HQUEUE_TICKETTIME < @0 ",
                    endTime);
            }
            if (!string.IsNullOrEmpty(number))
            {
                sql.Append(IsHQUEUE ? @" AND  QU.CHQUEUE_NUMBER like @0 " : @" AND  QU.HQUEUE_NUMBER like @0 ",
                    "%" + number + "%");
            }
            if (counter != null)
            {
                sql.Append(IsHQUEUE ? @" AND  QU.CHQUEUE_COUNTER =@0 " : @" AND  QU.HQUEUE_COUNTER =@0 ", counter);
            }
            if (!string.IsNullOrEmpty(staffId))
            {
                sql.Append(IsHQUEUE ? @" AND  QU.CHQUEUE_SNO like @0 " : @" AND  QU.HQUEUE_SNO like @0 ",
                    "%" + staffId + "%");
            }
            if (!string.IsNullOrEmpty(nsrsbm))
            {
                sql.Append(IsHQUEUE ? @" AND  QU.CHQUEUE_NSRSBM like @0 " : @" AND  QU.HQUEUE_NSRSBM like @0 ",
                    "%" + nsrsbm + "%");
            }
            if (tickettype != null)
            {
                sql.Append(IsHQUEUE ? @" AND  QU.CHQUEUE_TICKETTYPE =@0 " : @" AND  QU.HQUEUE_TICKETTYPE =@0 ",
                    tickettype);
            }
            if (status != null)
            {
                sql.Append(IsHQUEUE ? @" AND  QU.CHQUEUE_STATUS =@0 " : @" AND  QU.HQUEUE_STATUS =@0 ", status);
            }
            if (isfinished != null)
            {
                sql.Append(IsHQUEUE ? @" AND  QU.CHQUEUE_ISFINISHED =@0 " : @" AND  QU.HQUEUE_ISFINISHED =@0 ",
                    isfinished);
            }
        }


        public static SYS_CURRQUEUEHIST_SUB GetMergeSub(string CHQUEUE_TRANSCODEID)
        {
            var db = gzswDB.GetInstance();
            var sql = Sql.Builder.Append(@" SELECT * FROM (  SELECT C.[CHQUEUE_TRANSCODEID]
                                                  ,C.[CHQUEUE_SYSNO]
                                                  ,C.[CHQUEUE_NUMBER]
                                                  ,C.[CHQUEUE_TICKETTIME]
                                                  ,C.[CHQUEUE_QSERIALID]
                                                  ,C.[CHQUEUE_COUNTER]
                                                  ,C.[CHQUEUE_SNO]
                                                  ,C.[CHQUEUE_CALLTIME]
                                                  ,C.[CHQUEUE_WAITTIME]
                                                  ,C.[CHQUEUE_STIME]
                                                  ,C.[CHQUEUE_ETIME]
                                                  ,C.[CHQUEUE_BLTIME]
                                                  ,C.[CHQUEUE_PJRESULT]
                                                  ,C.[CHQQUEUE_PJTIME]
                                                  ,C.[CHQUEUE_YWBS]
                                                  ,C.[CHQUEUE_NSRSBM]
                                                  ,C.[CHQUEUE_NSRMC]
                                                  ,C.[CHQUEUE_SWJGDM]
                                                  ,C.[CHQUEUE_TICKETTYPE]
                                                  ,C.[CHQUEUE_STATUS]
                                                  ,C.[CHQUEUE_ISFINISHED]
                                                  ,Q.Q_SERIALNAME
                                                  ,ST.STAFF_NAM
                                                  ,H.HALL_NAM
                                                  ,H2.HALL_NAM AS CHQUEUE_HALL_NAM
                                              FROM [SYS_CURRQUEUEHIST] AS C
	                                            LEFT JOIN SYS_STAFF AS ST
	                                            ON C.CHQUEUE_SNO=ST.STAFF_ID
	                                            JOIN SYS_HALL AS H
	                                            ON C.CHQUEUE_SYSNO=H.HALL_NO
	                                            JOIN SYS_QUEUESERIAL AS Q
	                                            ON C.CHQUEUE_QSERIALID=Q.Q_SERIALID 
                                                JOIN SYS_HALL AS H2
                                                ON C.CHQUEUE_SWJGDM=H2.HALL_NO
                                               WHERE 1=1 ");
            sql.Append(@" union ");
            sql.Append(@" select  QU.[HQUEUE_TRANSCODEID] AS [CHQUEUE_TRANSCODEID]
                                              , QU.[HQUEUE_SYSNO]	AS [CHQUEUE_SYSNO]
                                              , QU.[HQUEUE_NUMBER]	AS [CHQUEUE_NUMBER]
                                              , QU.[HQUEUE_TICKETTIME] AS [CHQUEUE_TICKETTIME]
                                              , QU.[HQUEUE_QSERIALID] AS [CHQUEUE_QSERIALID]	
                                              , QU.[HQUEUE_COUNTER] AS [CHQUEUE_COUNTER]
                                              , QU.[HQUEUE_SNO] AS [CHQUEUE_SNO]
                                              , QU.[HQUEUE_CALLTIME] AS [CHQUEUE_CALLTIME]
                                              , QU.[HQUEUE_WAITTIME] AS [CHQUEUE_WAITTIME]
                                              , QU.[HQUEUE_STIME] AS [CHQUEUE_STIME]
                                              , QU.[HQUEUE_ETIME] AS [CHQUEUE_ETIME]
                                              , QU.[HQUEUE_BLTIME] AS [CHQUEUE_BLTIME]
                                              , QU.[HQUEUE_PJRESULT] AS [CHQUEUE_PJRESULT]
                                              , QU.[HQQUEUE_PJTIME] AS [CHQQUEUE_PJTIME]
                                              , QU.[HQUEUE_YWBS] AS [CHQUEUE_YWBS]
                                              , QU.[HQUEUE_NSRSBM] AS [CHQUEUE_NSRSBM]
                                              , QU.[HQUEUE_NSRMC] AS [CHQUEUE_NSRMC]
                                              , QU.[HQUEUE_SWJGDM] AS [CHQUEUE_SWJGDM]
                                              , QU.[HQUEUE_TICKETTYPE] AS [CHQUEUE_TICKETTYPE]
                                              , QU.[HQUEUE_STATUS]AS [CHQUEUE_STATUS] 
                                              , QU.[HQUEUE_ISFINISHED]AS [CHQUEUE_ISFINISHED]
                                              ,Q.Q_SERIALNAME
                                                ,ST.STAFF_NAM
                                                ,H.HALL_NAM 
                                                ,H2.HALL_NAM AS CHQUEUE_HALL_NAM
                                               from SYS_QUEUEHIST AS QU
                                                LEFT JOIN SYS_STAFF AS ST
	                                            ON QU.HQUEUE_SNO=ST.STAFF_ID
	                                            JOIN SYS_HALL AS H
	                                            ON QU.HQUEUE_SYSNO=H.HALL_NO
	                                            JOIN SYS_QUEUESERIAL AS Q
	                                            ON QU.HQUEUE_QSERIALID=Q.Q_SERIALID 
                                                JOIN SYS_HALL AS H2
                                                ON QU.HQUEUE_SWJGDM=H2.HALL_NO 
                                                WHERE 1=1 ");

            sql.Append(@" ) AS T1 WHERE T1.CHQUEUE_TRANSCODEID=@0 ", CHQUEUE_TRANSCODEID);

            return db.FirstOrDefault<SYS_CURRQUEUEHIST_SUB>(sql);
        }

        /// <summary>
        /// 根据评价类型获取当日排队业务明细
        /// </summary>
        /// <param name="orgId">服务厅编码</param>
        /// <param name="evaluationType">评价类型（1：评价总数  2：未平价总数  3：差评数  4：未评价率  5：满意率  6：差评率）</param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static object GetPagerByEvaluationType(string orgId, int evaluationType, int pageIndex, int pageSize)
        {
            var db = gzswDB.GetInstance();

            var sql = Sql.Builder.Append(@"SELECT  C.[CHQUEUE_TRANSCODEID] ,
        C.[CHQUEUE_SYSNO] ,
        C.[CHQUEUE_NUMBER] ,
        C.[CHQUEUE_TICKETTIME] ,
        C.[CHQUEUE_QSERIALID] ,
        C.[CHQUEUE_COUNTER] ,
        C.[CHQUEUE_SNO] ,
        C.[CHQUEUE_CALLTIME] ,
        C.[CHQUEUE_WAITTIME] ,
        C.[CHQUEUE_STIME] ,
        C.[CHQUEUE_ETIME] ,
        C.[CHQUEUE_BLTIME] ,
        C.[CHQUEUE_PJRESULT] ,
        C.[CHQQUEUE_PJTIME] ,
        C.[CHQUEUE_YWBS] ,
        C.[CHQUEUE_NSRSBM] ,
        C.[CHQUEUE_NSRMC] ,
        C.[CHQUEUE_SWJGDM] ,
        C.[CHQUEUE_TICKETTYPE] ,
        C.[CHQUEUE_STATUS] ,
        C.[CHQUEUE_ISFINISHED] ,
        Q.Q_SERIALNAME ,
        ST.STAFF_NAM ,
        H.HALL_NAM
FROM    [SYS_CURRQUEUEHIST] AS C
        JOIN SYS_STAFF AS ST ON C.CHQUEUE_SNO = ST.STAFF_ID
        JOIN SYS_HALL AS H ON C.CHQUEUE_SYSNO = H.HALL_NO
        JOIN SYS_QUEUESERIAL AS Q ON C.CHQUEUE_QSERIALID = Q.Q_SERIALID
WHERE   1 = 1 ");

            sql.Append(@" AND C.[CHQUEUE_SYSNO]= @0 ", orgId);

            switch (evaluationType)
            {
                    //评价数
                case 1:
                    sql.Append(@" AND C.CHQUEUE_PJRESULT IN ( 1, 2, 3, 4 ) ");
                    break;
                    //未平价数
                case 2:
                    //未评价率
                case 4:
                    sql.Append(@" AND C.CHQUEUE_PJRESULT = 0 ");
                    break;
                    //差评数
                case 3:
                    //差评率
                case 6:
                    sql.Append(@" AND c.CHQUEUE_PJRESULT = 4 ");
                    break;
                //满意率
                case 5:
                    sql.Append(@" AND C.CHQUEUE_PJRESULT IN ( 1, 2, 3 ) ");
                    break;
            }

            var data = db.Page<dynamic>(pageIndex, pageSize, sql);
            data.ItemsPerPage = pageSize;

            return data;
        }


        /// <summary>
        /// 获取当日超时等待的排队业务明细
        /// </summary>
        /// <param name="orgId">服务厅编码</param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static object GetTimeoutWaitPager(string orgId, int pageIndex, int pageSize)
        {
            var db = gzswDB.GetInstance();

            var sql = Sql.Builder.Append(@"SELECT  C.[CHQUEUE_TRANSCODEID] ,
        C.[CHQUEUE_SYSNO] ,
        C.[CHQUEUE_NUMBER] ,
        C.[CHQUEUE_TICKETTIME] ,
        C.[CHQUEUE_QSERIALID] ,
        C.[CHQUEUE_COUNTER] ,
        C.[CHQUEUE_SNO] ,
        C.[CHQUEUE_CALLTIME] ,
       case when C.CHQUEUE_STATUS=0 THEN 
		datediff(second, C.CHQUEUE_TICKETTIME, getdate())
							ELSE 
       C.[CHQUEUE_WAITTIME]  END AS CHQUEUE_WAITTIME ,  
        C.[CHQUEUE_STIME] ,
        C.[CHQUEUE_ETIME] ,
        C.[CHQUEUE_BLTIME] ,
        C.[CHQUEUE_PJRESULT] ,
        C.[CHQQUEUE_PJTIME] ,
        C.[CHQUEUE_YWBS] ,
        C.[CHQUEUE_NSRSBM] ,
        C.[CHQUEUE_NSRMC] ,
        C.[CHQUEUE_SWJGDM] ,
        C.[CHQUEUE_TICKETTYPE] ,
        C.[CHQUEUE_STATUS] ,
        C.[CHQUEUE_ISFINISHED] ,
       case when C.CHQUEUE_STATUS=0 THEN 
		( dbo.isWaitOuttime(C.CHQUEUE_SYSNO, C.CHQUEUE_QSERIALID,
                            datediff(second, C.CHQUEUE_TICKETTIME, getdate())) )
							ELSE 
        ( dbo.isWaitOuttime(C.CHQUEUE_SYSNO, C.CHQUEUE_QSERIALID,
                            C.CHQUEUE_WAITTIME) )  END AS IsOvertime ,
        Q.Q_SERIALNAME ,
        ST.STAFF_NAM ,
        H.HALL_NAM
FROM    [SYS_CURRQUEUEHIST] AS C
        LEFT JOIN SYS_STAFF AS ST ON C.CHQUEUE_SNO = ST.STAFF_ID
        JOIN SYS_HALL AS H ON C.CHQUEUE_SYSNO = H.HALL_NO
        JOIN SYS_QUEUESERIAL AS Q ON C.CHQUEUE_QSERIALID = Q.Q_SERIALID
WHERE   1 = 1 ");

            sql.Append(@" AND C.[CHQUEUE_SYSNO]= @0 ", orgId);

            var data = db.Page<dynamic>(pageIndex, pageSize, sql);
            data.ItemsPerPage = pageSize;

            return data;
        }

        /// <summary>
        /// 根据办理状态类型获取当日排队业务明细
        /// </summary>
        /// <param name="orgId">服务厅编码</param>
        /// <param name="dealStateType">办理状态类型（0：所有  1：等待中  2：已受理  3：已弃号）</param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static object GetPagerByDealStateType(string orgId, int dealStateType, int pageIndex, int pageSize)
        {
            var db = gzswDB.GetInstance();

            var sql = Sql.Builder.Append(@"SELECT  C.[CHQUEUE_TRANSCODEID] ,
        C.[CHQUEUE_SYSNO] ,
        C.[CHQUEUE_NUMBER] ,
        C.[CHQUEUE_TICKETTIME] ,
        C.[CHQUEUE_QSERIALID] ,
        C.[CHQUEUE_COUNTER] ,
        C.[CHQUEUE_SNO] ,
        C.[CHQUEUE_CALLTIME] ,
        case when C.CHQUEUE_STATUS=0 THEN 
		datediff(second, C.CHQUEUE_TICKETTIME, getdate())
							ELSE 
       C.[CHQUEUE_WAITTIME]  END AS CHQUEUE_WAITTIME ,  
        C.[CHQUEUE_STIME] ,
        C.[CHQUEUE_ETIME] ,
        C.[CHQUEUE_BLTIME] ,
        C.[CHQUEUE_PJRESULT] ,
        C.[CHQQUEUE_PJTIME] ,
        C.[CHQUEUE_YWBS] ,
        C.[CHQUEUE_NSRSBM] ,
        C.[CHQUEUE_NSRMC] ,
        C.[CHQUEUE_SWJGDM] ,
        C.[CHQUEUE_TICKETTYPE] ,
        C.[CHQUEUE_STATUS] ,
        C.[CHQUEUE_ISFINISHED] ,
        Q.Q_SERIALNAME ,
        ST.STAFF_NAM ,
        H.HALL_NAM
FROM    [SYS_CURRQUEUEHIST] AS C
        JOIN SYS_HALL AS H ON C.CHQUEUE_SYSNO = H.HALL_NO
        JOIN SYS_QUEUESERIAL AS Q ON C.CHQUEUE_QSERIALID = Q.Q_SERIALID
        LEFT JOIN SYS_STAFF AS ST ON C.CHQUEUE_SNO = ST.STAFF_ID
WHERE   1 = 1 ");

            sql.Append(@" AND C.[CHQUEUE_SYSNO]= @0 ", orgId);

            switch (dealStateType)
            {
                //所有
                case 0:
                    //sql.Append(@" AND C.CHQUEUE_STATUS IN ( 0, 1, 2, 3, 4 ) ");
                    break;
                //等待中
                case 1:
                    sql.Append(@" AND C.CHQUEUE_STATUS = 0 ");
                    break;
                //已受理
                case 2:
                    sql.Append(@" AND C.CHQUEUE_STATUS = 3 ");
                    break;
                //已弃号
                case 3:
                    sql.Append(@" AND C.CHQUEUE_STATUS = 4 ");
                    break;
            }

            var data = db.Page<dynamic>(pageIndex, pageSize, sql);
            data.ItemsPerPage = pageSize;

            return data;
        }
    }
}
