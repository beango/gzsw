using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PetaPoco;
using gzsw.model;

namespace gzsw.dal.dao
{
    public class SYS_YWHIST_DAL
    {


        public static object GetMergeList(string orgId, DateTime? beginTime, DateTime? endTime, string number, int? counter, string staffId,
            string nsrsbm, int? tickettype, string hywDetailserialid, int? isfinished, int pageIndex, int pageSize)
        {
            var db = gzswDB.GetInstance();

            var sql = PetaPoco.Sql.Builder.Append(@"select  
tb1.[HYW_TRANSCODEID]
      ,tb1.[HYW_XH]
      ,tb1.[HYW_QTRANSCODEID]
      ,tb1.[HYW_SYSNO]
      ,tb1.[HYW_NUMBER]
      ,tb1.[HYW_QSERIALID]
      ,tb1.[HYW_DLSERIALID]
      ,tb1.[HYW_DETAILSERIALID]
      ,tb1.[HYW_COUNTER]
      ,tb1.[HYW_SNO]
      ,tb1.[HYW_STIME]
      ,tb1.[HYW_ETIME]
      ,tb1.[HYW_BLTIME]
      ,tb1.[HYW_NSRSBM]
      ,tb1.[HYW_NSRMC]
      ,tb1.[HYW_SWJGDM]
      ,tb1.[HYW_ISFINISHED],
        Q_SERIALNAME,
      DLS_SERIALNAME,
      SERIALNAME,
      STAFF_NAM
       from SYS_QUEUESERIAL q,SYS_DLSERIAL d,SYS_DETAILSERIAL
s,SYS_STAFF st,

(SELECT [HYW_TRANSCODEID]
      ,[HYW_XH]
      ,[HYW_QTRANSCODEID]
      ,[HYW_SYSNO]
      ,[HYW_NUMBER]
      ,[HYW_QSERIALID]
      ,[HYW_DLSERIALID]
      ,[HYW_DETAILSERIALID]
      ,[HYW_COUNTER]
      ,[HYW_SNO]
      ,[HYW_STIME]
      ,[HYW_ETIME]
      ,[HYW_BLTIME]
      ,[HYW_NSRSBM]
      ,[HYW_NSRMC]
      ,[HYW_SWJGDM]
      ,[HYW_ISFINISHED]
  FROM [SYS_YWHIST] h
  where  h.[HYW_STIME]>''
    union all
   SELECT [YW_TRANSCODEID]   AS [HYW_TRANSCODEID]
      ,[YW_XH]  AS [HYW_XH] 
      ,[YW_QTRANSCODEID] AS [HYW_QTRANSCODEID]
      ,[YW_SYSNO] AS [HYW_SYSNO]
      ,[YW_NUMBER] AS  [HYW_NUMBER] 
      ,[YW_QSERIALID] AS [HYW_QSERIALID]
      ,[YW_DLSERIALID] AS [HYW_DLSERIALID]
      ,[YW_DETAILSERIALID] AS [HYW_DETAILSERIALID]
      ,[YW_COUNTER] AS [HYW_COUNTER]
      ,[YW_SNO] AS [HYW_SNO]
      ,[YW_STIME] AS [HYW_STIME]
      ,[YW_ETIME]  AS [HYW_ETIME]
      ,[YW_BLTIME] AS [HYW_BLTIME] 
      ,[YW_NSRSBM] AS [HYW_NSRSBM]
      ,[YW_NSRMC] AS [HYW_NSRMC]
      ,[YW_SWJGDM] AS [HYW_SWJGDM]
      ,[YW_ISFINISHED] AS [HYW_ISFINISHED]
  FROM [SYS_CURRYWHIST] c
  where  c.YW_STIME>'') as tb1
  where tb1.HYW_QSERIALID=q.Q_SERIALID
   and tb1.HYW_DLSERIALID=d.DLS_SERIALID
   and tb1.HYW_DETAILSERIALID=s.SERIALID
   and tb1.HYW_SNO=st.STAFF_ID   ");
            addSeleteWhere(sql, orgId, beginTime, endTime, number, counter, staffId, nsrsbm, tickettype, hywDetailserialid,
              isfinished, false);
            var data = db.Page<dynamic>(pageIndex, pageSize, sql);
            data.ItemsPerPage = pageSize;
            return data;
        }


        private static void addSeleteWhere(Sql sql, string orgId, DateTime? beginTime, DateTime? endTime,
            string number, int? counter, string staffId, string nsrsbm, int? tickettype, string hywDetailserialid, int? isfinished, bool IsHQUEUE = true)
        {
            sql.Append(@" AND  HYW_SYSNO =@0  ", orgId);

            if (beginTime != null)
            {
                beginTime = beginTime.GetValueOrDefault().Date;
                sql.Append(@" AND  HYW_STIME >= @0 ", beginTime);
            }
            if (endTime != null)
            {
                endTime = endTime.GetValueOrDefault().Date.AddDays(1);
                sql.Append(@" AND  HYW_STIME < @0 ", endTime);
            }
            if (!string.IsNullOrEmpty(number))
            {
                sql.Append(@" AND  HYW_NUMBER like @0 ",
                    "%" + number + "%");
            }
            if (counter != null)
            {
                sql.Append(@" AND  HYW_COUNTER =@0 ", counter);
            }
            if (!string.IsNullOrEmpty(staffId))
            {
                sql.Append(@" AND  HYW_SNO like @0 ",
                    "%" + staffId + "%");
            }
            if (!string.IsNullOrEmpty(nsrsbm))
            {
                sql.Append(@" AND  HYW_NSRSBM like @0 ",
                    "%" + nsrsbm + "%");
            }
            if (!string.IsNullOrEmpty(hywDetailserialid))
            {
                sql.Append(@" AND  HYW_DETAILSERIALID =@0 ", hywDetailserialid);
            }
        }

        public static dynamic GetOne(string id)
        {
            var db = gzswDB.GetInstance();

            var sql = PetaPoco.Sql.Builder.Append(@"select  
tb1.[HYW_TRANSCODEID]
      ,tb1.[HYW_XH]
      ,tb1.[HYW_QTRANSCODEID]
      ,tb1.[HYW_SYSNO]
      ,tb1.[HYW_NUMBER]
      ,tb1.[HYW_QSERIALID]
      ,tb1.[HYW_DLSERIALID]
      ,tb1.[HYW_DETAILSERIALID]
      ,tb1.[HYW_COUNTER]
      ,tb1.[HYW_SNO]
      ,tb1.[HYW_STIME]
      ,tb1.[HYW_ETIME]
      ,tb1.[HYW_BLTIME]
      ,tb1.[HYW_NSRSBM]
      ,tb1.[HYW_NSRMC]
      ,tb1.[HYW_SWJGDM]
      ,tb1.[HYW_ISFINISHED],
      
        Q_SERIALNAME,
      DLS_SERIALNAME,
      SERIALNAME,
      st.STAFF_NAM,
      h.HALL_NAM,
      h2.HALL_NAM as  SWJGDM_NAM
       from SYS_QUEUESERIAL q,SYS_DLSERIAL d,SYS_DETAILSERIAL
s,SYS_STAFF st,SYS_HALL h, sys_hall h2,

(SELECT [HYW_TRANSCODEID]
      ,[HYW_XH]
      ,[HYW_QTRANSCODEID]
      ,[HYW_SYSNO]
      ,[HYW_NUMBER]
      ,[HYW_QSERIALID]
      ,[HYW_DLSERIALID]
      ,[HYW_DETAILSERIALID]
      ,[HYW_COUNTER]
      ,[HYW_SNO]
      ,[HYW_STIME]
      ,[HYW_ETIME]
      ,[HYW_BLTIME]
      ,[HYW_NSRSBM]
      ,[HYW_NSRMC]
      ,[HYW_SWJGDM]
      ,[HYW_ISFINISHED]
  FROM [SYS_YWHIST] h
  where  h.[HYW_STIME]>''
    union all
   SELECT [YW_TRANSCODEID]   AS [HYW_TRANSCODEID]
      ,[YW_XH]  AS [HYW_XH] 
      ,[YW_QTRANSCODEID] AS [HYW_QTRANSCODEID]
      ,[YW_SYSNO] AS [HYW_SYSNO]
      ,[YW_NUMBER] AS  [HYW_NUMBER] 
      ,[YW_QSERIALID] AS [HYW_QSERIALID]
      ,[YW_DLSERIALID] AS [HYW_DLSERIALID]
      ,[YW_DETAILSERIALID] AS [HYW_DETAILSERIALID]
      ,[YW_COUNTER] AS [HYW_COUNTER]
      ,[YW_SNO] AS [HYW_SNO]
      ,[YW_STIME] AS [HYW_STIME]
      ,[YW_ETIME]  AS [HYW_ETIME]
      ,[YW_BLTIME] AS [HYW_BLTIME] 
      ,[YW_NSRSBM] AS [HYW_NSRSBM]
      ,[YW_NSRMC] AS [HYW_NSRMC]
      ,[YW_SWJGDM] AS [HYW_SWJGDM]
      ,[YW_ISFINISHED] AS [HYW_ISFINISHED]
  FROM [SYS_CURRYWHIST] c
  where  c.YW_STIME>'') as tb1
  where 
  tb1.HYW_QSERIALID=q.Q_SERIALID
   and tb1.HYW_DLSERIALID=d.DLS_SERIALID
   and tb1.HYW_DETAILSERIALID=s.SERIALID
   
   and tb1.HYW_SNO=st.STAFF_ID 
  and tb1.HYW_SYSNO=h.HALL_NO
  and tb1.HYW_SWJGDM=h2.HALL_NO ");
            sql.Append(@" AND  HYW_TRANSCODEID = @0 ", id);

            return db.FirstOrDefault<dynamic>(sql);
        }

        public static dynamic GetMergeListByCode(string code, int pageIndex, int pageSize)
        {
            var db = gzswDB.GetInstance();

            var sql = PetaPoco.Sql.Builder.Append(@"select  
tb1.[HYW_TRANSCODEID]
      ,tb1.[HYW_XH]
      ,tb1.[HYW_QTRANSCODEID]
      ,tb1.[HYW_SYSNO]
      ,tb1.[HYW_NUMBER]
      ,tb1.[HYW_QSERIALID]
      ,tb1.[HYW_DLSERIALID]
      ,tb1.[HYW_DETAILSERIALID]
      ,tb1.[HYW_COUNTER]
      ,tb1.[HYW_SNO]
      ,tb1.[HYW_STIME]
      ,tb1.[HYW_ETIME]
      ,tb1.[HYW_BLTIME]
      ,tb1.[HYW_NSRSBM]
      ,tb1.[HYW_NSRMC]
      ,tb1.[HYW_SWJGDM]
      ,tb1.[HYW_ISFINISHED],
        Q_SERIALNAME,
      DLS_SERIALNAME,
      SERIALNAME,
      STAFF_NAM
       from SYS_QUEUESERIAL q,SYS_DLSERIAL d,SYS_DETAILSERIAL
s,SYS_STAFF st,

(SELECT [HYW_TRANSCODEID]
      ,[HYW_XH]
      ,[HYW_QTRANSCODEID]
      ,[HYW_SYSNO]
      ,[HYW_NUMBER]
      ,[HYW_QSERIALID]
      ,[HYW_DLSERIALID]
      ,[HYW_DETAILSERIALID]
      ,[HYW_COUNTER]
      ,[HYW_SNO]
      ,[HYW_STIME]
      ,[HYW_ETIME]
      ,[HYW_BLTIME]
      ,[HYW_NSRSBM]
      ,[HYW_NSRMC]
      ,[HYW_SWJGDM]
      ,[HYW_ISFINISHED]
  FROM [SYS_YWHIST] h
  where  h.[HYW_STIME]>''
    union all
   SELECT [YW_TRANSCODEID]   AS [HYW_TRANSCODEID]
      ,[YW_XH]  AS [HYW_XH] 
      ,[YW_QTRANSCODEID] AS [HYW_QTRANSCODEID]
      ,[YW_SYSNO] AS [HYW_SYSNO]
      ,[YW_NUMBER] AS  [HYW_NUMBER] 
      ,[YW_QSERIALID] AS [HYW_QSERIALID]
      ,[YW_DLSERIALID] AS [HYW_DLSERIALID]
      ,[YW_DETAILSERIALID] AS [HYW_DETAILSERIALID]
      ,[YW_COUNTER] AS [HYW_COUNTER]
      ,[YW_SNO] AS [HYW_SNO]
      ,[YW_STIME] AS [HYW_STIME]
      ,[YW_ETIME]  AS [HYW_ETIME]
      ,[YW_BLTIME] AS [HYW_BLTIME] 
      ,[YW_NSRSBM] AS [HYW_NSRSBM]
      ,[YW_NSRMC] AS [HYW_NSRMC]
      ,[YW_SWJGDM] AS [HYW_SWJGDM]
      ,[YW_ISFINISHED] AS [HYW_ISFINISHED]
  FROM [SYS_CURRYWHIST] c
  where  c.YW_STIME>'') as tb1
  where tb1.HYW_QSERIALID=q.Q_SERIALID
   and tb1.HYW_DLSERIALID=d.DLS_SERIALID
   and tb1.HYW_DETAILSERIALID=s.SERIALID
   and tb1.HYW_SNO=st.STAFF_ID   ");
            sql.Append(@" AND  HYW_QTRANSCODEID like @0 ", "%" + code + "%");
            var data = db.Page<dynamic>(pageIndex, pageSize, sql);
            data.ItemsPerPage = pageSize;
            return data;
        }
    }

}
