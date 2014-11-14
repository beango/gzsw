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
                sql.Append(" AND  STAT_DT>='"+beginTime.Value.ToShortDateString()+"'");
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
