using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PetaPoco;
using gzsw.model;

namespace gzsw.dal.dao
{
    public class STAT_STAFF_QUEUE_BUSI_D_DAL
    {
        /// <summary>
        /// 查询报表，按市统计
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="halllist"></param>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public List<dynamic> Q_STATDATA_GROUP_CITY(string[] halllist, DateTime? beginTime, DateTime? endTime,
          int statTyp)
        {
            Database db = gzswDB.GetInstance();
            var sql = Sql.Builder.Append("");
            if (statTyp == 1)
            {
                sql.Append(@"select * from(
                select t4.ORG_ID ID, t4.ORG_NAM NAM,t1.QUEUE_BUSI_CD,t5.Q_SERIALNAME");
            }
            if (statTyp == 2)
                sql.Append(@"select * from(
                select t1.HALL_NO ID,t2.HALL_NAM NAM,t1.QUEUE_BUSI_CD,t5.Q_SERIALNAME");
            if (statTyp == 3)
                sql.Append(@"select * from(
                select '' ID,'' NAM,t1.QUEUE_BUSI_CD,t5.Q_SERIALNAME");
            sql.Append(@"
                ,max(STAT_DT) MAX_STAT_DT,min(STAT_DT) MIN_STAT_DT,
                SUM(t1.CALL_CNT) AS CALL_CNT,
                SUM(t1.OVERTIME_WAIT_CNT) AS OVERTIME_WAIT_CNT,
                SUM(t1.HANDLE_CNT) AS HANDLE_CNT,
                SUM(t1.ABANDON_CNT) AS ABANDON_CNT,
                SUM(t1.HANDLE_DUR) AS HANDLE_DUR,
                SUM(t1.WAIT_DUR) AS WAIT_DUR,
                MAX(t1.MAX_WAIT_DUR) AS MAX_WAIT_DUR,
                MAX(t1.MAX_HANDLE_DUR) AS MAX_HANDLE_DUR,
                SUM(t1.OVERTIME_HANDLE_CNT) AS OVERTIME_HANDLE_CNT,
                SUM(t1.TOT_TICKET_CNT) AS TOT_TICKET_CNT
                from STAT_STAFF_QUEUE_BUSI_D t1
                left join SYS_HALL t2 on t1.HALL_NO=t2.HALL_NO
                join (
                    select ORG_ID,ORG_NAM,
                    (select ORG_ID from SYS_ORGANIZE where ORG_ID=(
	                    (select PAR_ORG_ID from SYS_ORGANIZE where ORG_ID=t1.PAR_ORG_ID) 
                    )) ORG_CITY
                    from SYS_ORGANIZE t1 where ORG_LEVEL=4
                ) t3 on t3.ORG_ID=t2.ORG_ID
                join SYS_ORGANIZE t4 on t4.ORG_ID=t3.ORG_CITY
                join SYS_QUEUESERIAL t5 on t1.QUEUE_BUSI_CD=t5.Q_SERIALID
                ");
            //sql.Append("where t1.TIME_QUANTUM_CD>=8 and t1.TIME_QUANTUM_CD<=18 ");
            if (null != beginTime)
                sql.Append("where t1.STAT_DT>=@beginTime ", new { beginTime = beginTime.Value });
            if (null != endTime)
                sql.Append("where t1.STAT_DT<=@endTime ", new { endTime = endTime.Value });
            if (null != halllist && halllist.Length > 0)
                sql.Append("where t1.HALL_NO in (@hall)", new { hall = halllist });
            if (statTyp == 1)
                sql.Append("GROUP BY t4.ORG_ID, t4.ORG_NAM,t1.QUEUE_BUSI_CD,t5.Q_SERIALNAME");
            if (statTyp == 2)
                sql.Append("GROUP BY t1.HALL_NO,t2.HALL_NAM,t1.QUEUE_BUSI_CD,t5.Q_SERIALNAME");
            if (statTyp == 3)
                sql.Append("GROUP BY t1.QUEUE_BUSI_CD,t5.Q_SERIALNAME");
            sql.Append(")t");
            return db.Fetch<dynamic>(sql);
        }

        public static dynamic GetListBubByHall(DateTime beginStatMo, DateTime endStatMo, string hallNo, int pageIndex, int pageSize)
        {
            var db = gzswDB.GetInstance();

            var sql = Sql.Builder.Append(@"select ssd.*,H.HALL_NAM,SQ.Q_SERIALNAME from 
            STAT_STAFF_QUEUE_BUSI_D ssd,SYS_HALL H,SYS_QUEUESERIAL SQ
            WHERE SSD.HALL_NO=H.HALL_NO
            AND SSD.QUEUE_BUSI_CD=SQ.Q_SERIALID ");

            sql.Append(@" AND ssd.[HALL_NO]=@0 ", hallNo);

            sql.Append(@" AND Ssd.STAT_DT>=@0 AND Ssd.STAT_DT<=@1 ", beginStatMo, endStatMo);

            var data = db.Page<dynamic>(pageIndex, pageSize, sql);
            data.ItemsPerPage = pageSize;
            return data;
        }

        public List<dynamic> GetStatsInfo(int pageIndex, int pageSize, string[] halllist, DateTime? beginTime, DateTime? endTime)
        {
            var db = gzswDB.GetInstance();

            var sql = Sql.Builder.Append(@"
             select  
                   QUEUE_BUSI_CD,
                   Q_SERIALNAME,
                      SUM(CALL_CNT) as CALL_CNT,
                      SUM(OVERTIME_WAIT_CNT) as OVERTIME_WAIT_CNT, 
                       SUM(HANDLE_CNT) as HANDLE_CNT,
                      SUM(ABANDON_CNT) as ABANDON_CNT, 
                       SUM(HANDLE_DUR) as HANDLE_DUR,
                      SUM(WAIT_DUR) as WAIT_DUR, 
                       MAX(MAX_WAIT_DUR) as MAX_WAIT_DUR,
                      MAX(MAX_HANDLE_DUR) as MAX_HANDLE_DUR, 
                       SUM(OVERTIME_HANDLE_CNT) as OVERTIME_HANDLE_CNT,
                     SUM(TOT_TICKET_CNT) as TOT_TICKET_CNT  from (
              select * from STAT_STAFF_QUEUE_BUSI_D ,SYS_QUEUESERIAL sq
              WHERE STAT_STAFF_QUEUE_BUSI_D.QUEUE_BUSI_CD=SQ.Q_SERIALID  ");
            if (null != halllist && halllist.Length > 0)
                sql.Append(" AND HALL_NO in (@hall)", new { hall = halllist });
            if (null != beginTime)
                sql.Append(" AND  STAT_DT>='" + beginTime.Value.ToShortDateString() + "'");
            if (null != endTime)
                sql.Append(" AND  STAT_DT<='" + endTime.Value.ToShortDateString() + "'");
            sql.Append(@"
             ) t
              group by t.QUEUE_BUSI_CD,Q_SERIALNAME        
            ");
            var data = db.Fetch<dynamic>(pageIndex, pageSize, sql);
            return data;
        }
    }
}
