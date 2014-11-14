using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PetaPoco;
using gzsw.model;

namespace gzsw.dal.dao
{
    /// <summary>
    /// 纳税人评价明细_DAL
    /// </summary>
    public static class DETAIL_TAXPAYER_EVALUATION_DAL
    {
        /// <summary>
        /// 获取分页列表
        /// </summary>
        /// <param name="beginTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="TRANSCODEID">交易业务流水号</param>
        /// <param name="SYSNO">取号发生的服务厅编码</param>
        /// <param name="SNO">呼叫发生的操作员工代码</param>
        /// <param name="NSRSBM">取号纳税人识别码</param>
        /// <param name="SERIALNAME">业务名称</param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static object GetPager(DateTime? beginTime, DateTime? endTime, string TRANSCODEID, string SYSNO, string SNO, string NSRSBM, string SERIALNAME, int? counter, int pageIndex, int pageSize)
        {
            var db = gzswDB.GetInstance();

            var sql = Sql.Builder.Append(@"SELECT  SYS_QUEUEHISTALL.* ,
        SYS_QUEUESERIAL.Q_SERIALNAME AS SERIALNAME
FROM    ( SELECT    CHQUEUE_TRANSCODEID AS TRANSCODEID ,
                    CHQUEUE_QSERIALID AS QSERIALID ,
                    CHQUEUE_NSRMC AS NSRMC ,
                    CHQUEUE_BLTIME AS BLTIME ,
                    CHQUEUE_PJRESULT AS PJRESULT ,
                    CHQUEUE_ETIME AS ETIME ,
                    CHQQUEUE_PJTIME AS PJTIME ,
                    CHQUEUE_ISFINISHED AS ISFINISHED ,
                    CHQUEUE_SYSNO AS SYSNO ,
                    CHQUEUE_SNO AS SNO ,
                    CHQUEUE_NSRSBM AS NSRSBM ,
                    CHQUEUE_COUNTER AS [COUNTER]
          FROM      SYS_CURRQUEUEHIST
          WHERE     1 = 1
          UNION ALL
          SELECT    HQUEUE_TRANSCODEID AS TRANSCODEID ,
                    HQUEUE_QSERIALID AS QSERIALID ,
                    HQUEUE_NSRMC AS NSRMC ,
                    HQUEUE_BLTIME AS BLTIME ,
                    HQUEUE_PJRESULT AS PJRESULT ,
                    HQUEUE_ETIME AS ETIME ,
                    HQQUEUE_PJTIME AS PJTIME ,
                    HQUEUE_ISFINISHED AS ISFINISHED ,
                    HQUEUE_SYSNO AS SYSNO ,
                    HQUEUE_SNO AS SNO ,
                    HQUEUE_NSRSBM AS NSRSBM ,
                    HQUEUE_COUNTER AS [COUNTER]
          FROM      SYS_QUEUEHIST
          WHERE     1 = 1
        ) AS SYS_QUEUEHISTALL
		JOIN SYS_QUEUESERIAL ON SYS_QUEUESERIAL.Q_SERIALID = SYS_QUEUEHISTALL.QSERIALID
WHERE   SYS_QUEUEHISTALL.ISFINISHED = 1");


            if (!string.IsNullOrEmpty(TRANSCODEID))
            {
                sql.Append(@" AND SYS_QUEUEHISTALL.TRANSCODEID = @0 ", TRANSCODEID);
            }
            if (!string.IsNullOrEmpty(SYSNO))
            {
                sql.Append(@" AND SYS_QUEUEHISTALL.SYSNO = @0 ", SYSNO);
            }
            if (!string.IsNullOrEmpty(SNO))
            {
                sql.Append(@" AND SYS_QUEUEHISTALL.SNO = @0 ", SNO);
            }
            if (!string.IsNullOrEmpty(NSRSBM))
            {
                sql.Append(@" AND SYS_QUEUEHISTALL.NSRSBM = @0 ", NSRSBM);
            }
            if (beginTime.HasValue)
            {
                sql.Append(@" AND DATEDIFF(DAY, SYS_QUEUEHISTALL.ETIME, @0) <= 0 ", beginTime);
            }
            if (endTime.HasValue)
            {
                sql.Append(@" AND DATEDIFF(DAY, SYS_QUEUEHISTALL.ETIME, @0) >= 0 ", endTime);
            }
            if (counter.HasValue)
            {
                sql.Append(@" AND SYS_QUEUEHISTALL.[COUNTER] = @0 ", counter);
            }
            if (!string.IsNullOrEmpty(SERIALNAME))
            {
                sql.Append(@" AND SYS_QUEUESERIAL.Q_SERIALNAME LIKE @0 ", string.Format("%{0}%", SERIALNAME));
            }

            var data = db.Page<dynamic>(pageIndex, pageSize, sql);
            data.ItemsPerPage = pageSize;

            return data;
        }
    }
     
}
