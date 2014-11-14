using gzsw.model;
using PetaPoco;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace gzsw.dal.dao
{
    public class STAT_STAFF_BUSI_TOT_D_DAL
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
        public Page<dynamic> Q_STATDATA_GROUP_CITY(int pageIndex, int pageSize, string[] halllist, DateTime? beginTime, DateTime? endTime)
        {
            Database db = gzswDB.GetInstance();

            var sql = Sql.Builder.Append(@"select * from(
                select t4.ORG_ID ID, t4.ORG_NAM AS NAM,max(STAT_DT) MAX_STAT_DT,min(STAT_DT) MIN_STAT_DT,
                SUM(t1.CALL_CNT) AS CALL_CNT,SUM(t1.HANDLE_CNT) AS HANDLE_CNT,
                SUM(t1.ABANDON_CNT) AS ABANDON_CNT,SUM(t1.LOCAL_CNT) AS LOCAL_CNT,
                SUM(t1.VOTE_MULTI_CNT) AS VOTE_MULTI_CNT,SUM(t1.VERY_SATISFY_CNT) AS VERY_SATISFY_CNT,
                SUM(t1.SATISFY_CNT) AS SATISFY_CNT,SUM(t1.COMMON_CNT) AS COMMON_CNT,
                SUM(t1.UNSATISFY_CNT) AS UNSATISFY_CNT,SUM(t1.NON_EVAL_CNT) AS NON_EVAL_CNT,
                SUM(t1.OVERTIME_WAIT_CNT) AS OVERTIME_WAIT_CNT,SUM(t1.SECOND_SVR_CNT) AS SECOND_SVR_CNT,
                SUM(t1.WAIT_DUR) AS WAIT_DUR
                from STAT_STAFF_BUSI_TOT_D t1
                left join SYS_HALL t2 on t1.HALL_NO=t2.HALL_NO
                join (
                    select ORG_ID,ORG_NAM,
                    (select ORG_ID from SYS_ORGANIZE where ORG_ID=(
	                    (select PAR_ORG_ID from SYS_ORGANIZE where ORG_ID=t1.PAR_ORG_ID) 
                    )) ORG_CITY
                    from SYS_ORGANIZE t1 where ORG_LEVEL=4
                ) t3 on t3.ORG_ID=t2.ORG_ID
                join SYS_ORGANIZE t4 on t4.ORG_ID=t3.ORG_CITY");

            if (null != beginTime)
                sql.Append("where t1.STAT_DT>=@beginTime", new { beginTime = beginTime.Value });
            if (null != endTime)
                sql.Append("where t1.STAT_DT<=@endTime", new { endTime = endTime.Value });
            if (null != halllist && halllist.Length > 0)
                sql.Append("where t1.HALL_NO in (@hall)", new { hall = halllist });
            sql.Append("GROUP BY t4.ORG_ID, t4.ORG_NAM");
            sql.Append(")t");

            var data = db.Page<dynamic>(pageIndex, pageSize, sql);
            if (pageIndex == 0)
            {
                data.Items = db.Fetch<dynamic>(sql);
            }
            return data;
        }

        /// <summary>
        /// 查询报表，按员工统计
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="halllist"></param>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public Page<dynamic> Q_STATDATA_GROUP_STAFF(int pageIndex, int pageSize, string[] halllist, DateTime? beginTime, DateTime? endTime)
        {
            Database db = gzswDB.GetInstance();

            var sql = Sql.Builder.Append(@"select * from(
                select t1.STAFF_ID AS ID,t2.STAFF_NAM AS NAM,max(STAT_DT) MAX_STAT_DT,min(STAT_DT) MIN_STAT_DT,
                max(t3.HALL_NAM) AS HALL_NAM, max(t3.HALL_NO) AS HALL_NO,
                SUM(t1.CALL_CNT) AS CALL_CNT,SUM(t1.HANDLE_CNT) AS HANDLE_CNT,
                SUM(t1.ABANDON_CNT) AS ABANDON_CNT,SUM(t1.LOCAL_CNT) AS LOCAL_CNT,
                SUM(t1.VOTE_MULTI_CNT) AS VOTE_MULTI_CNT,SUM(t1.VERY_SATISFY_CNT) AS VERY_SATISFY_CNT,
                SUM(t1.SATISFY_CNT) AS SATISFY_CNT,SUM(t1.COMMON_CNT) AS COMMON_CNT,
                SUM(t1.UNSATISFY_CNT) AS UNSATISFY_CNT,SUM(t1.NON_EVAL_CNT) AS NON_EVAL_CNT,
                SUM(t1.OVERTIME_WAIT_CNT) AS OVERTIME_WAIT_CNT,SUM(t1.SECOND_SVR_CNT) AS SECOND_SVR_CNT,
                SUM(t1.WAIT_DUR) AS WAIT_DUR
                from STAT_STAFF_BUSI_TOT_D t1 join SYS_STAFF t2 on t1.STAFF_ID=t2.STAFF_ID
                join SYS_HALL t3 on t1.HALL_NO=t3.HALL_NO");

            if (null != beginTime)
                sql.Append("where t1.STAT_DT>=@beginTime", new { beginTime = beginTime.Value });
            if (null != endTime)
                sql.Append("where t1.STAT_DT<=@endTime", new { endTime = endTime.Value });
            if (null != halllist && halllist.Length > 0)
                sql.Append("where t1.HALL_NO in (@hall)", new { hall = halllist });
            sql.Append("GROUP BY t1.STAFF_ID,t2.STAFF_NAM");
            sql.Append(")t");

            var data = db.Page<dynamic>(pageIndex, pageSize, sql);
            if (pageIndex == 0)
            {
                data.Items = db.Fetch<dynamic>(sql);
            }
            return data;
        }

        /// <summary>
        /// 查询报表，按服务厅统计
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="halllist"></param>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public Page<dynamic> Q_STATDATA_GROUP_HALL(int pageIndex, int pageSize, string[] halllist, DateTime? beginTime, DateTime? endTime)
        {
            Database db = gzswDB.GetInstance();

            var sql = Sql.Builder.Append(@"select * from(
                select t1.HALL_NO ID,t2.HALL_NAM NAM,max(STAT_DT) MAX_STAT_DT,min(STAT_DT) MIN_STAT_DT,
                SUM(t1.CALL_CNT) AS CALL_CNT,SUM(t1.HANDLE_CNT) AS HANDLE_CNT,
                SUM(t1.ABANDON_CNT) AS ABANDON_CNT,SUM(t1.LOCAL_CNT) AS LOCAL_CNT,
                SUM(t1.VOTE_MULTI_CNT) AS VOTE_MULTI_CNT,SUM(t1.VERY_SATISFY_CNT) AS VERY_SATISFY_CNT,
                SUM(t1.SATISFY_CNT) AS SATISFY_CNT,SUM(t1.COMMON_CNT) AS COMMON_CNT,
                SUM(t1.UNSATISFY_CNT) AS UNSATISFY_CNT,SUM(t1.NON_EVAL_CNT) AS NON_EVAL_CNT,
                SUM(t1.OVERTIME_WAIT_CNT) AS OVERTIME_WAIT_CNT,SUM(t1.SECOND_SVR_CNT) AS SECOND_SVR_CNT,
                SUM(t1.WAIT_DUR) AS WAIT_DUR
                from STAT_STAFF_BUSI_TOT_D t1 join SYS_HALL t2 on t1.HALL_NO=t2.HALL_NO");

            if (null != beginTime)
                sql.Append("where t1.STAT_DT>=@beginTime", new { beginTime = beginTime.Value });
            if (null != endTime)
                sql.Append("where t1.STAT_DT<=@endTime", new { endTime = endTime.Value });
            if (null != halllist && halllist.Length > 0)
                sql.Append("where t1.HALL_NO in (@hall)", new { hall = halllist });
            sql.Append("GROUP BY t1.HALL_NO,t2.HALL_NAM");
            sql.Append(")t");

            var data = db.Page<dynamic>(pageIndex, pageSize, sql);
            if (pageIndex == 0)
            {
                data.Items = db.Fetch<dynamic>(sql);
            }
            return data;
        }

        /// <summary>
        /// 按时间统计
        /// </summary>
        /// <param name="staffid"></param>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <param name="orgid"></param>
        /// <returns></returns>
        public DataTable GetSTATList_DT(string staffid, ref DateTime? beginTime, ref DateTime? endTime, string orgid)
        {
            Database db = gzswDB.GetInstance();
            string field = "convert(varchar(50),t1.STAT_DT,23)";
            if (beginTime != null && endTime != null)
            {
                if (beginTime.Value.ToShortDateString() == endTime.Value.ToShortDateString())//同一天
                    field = "RIGHT('0'+cast(TIME_QUANTUM_CD as varchar(50)),2)+'点'";
                var d = (endTime.Value - beginTime.Value).Days;
                if (d >= 31)//按月查询
                    field = "Replace(Left(convert(varchar(50),t1.STAT_DT,23),7),'-','年')+'月'";
                if (d >= 365)//跨年查询
                    field = "Left(convert(varchar(50),t1.STAT_DT,23),4)+'年'";
            }

            db.EnableAutoSelect = false;
            var sql = Sql.Builder.Append(string.Format(@"select * from(
                select {0} as ID,
                SUM(isnull(t1.CALL_CNT,0)) AS 呼叫量,SUM(isnull(t1.HANDLE_CNT,0)) AS 办理量,
                SUM(isnull(t1.ABANDON_CNT,0)) AS 弃号量,SUM(t1.LOCAL_CNT) AS LOCAL_CNT,
                SUM(t1.VOTE_MULTI_CNT) AS VOTE_MULTI_CNT,SUM(t1.VERY_SATISFY_CNT) AS VERY_SATISFY_CNT,
                SUM(t1.SATISFY_CNT) AS SATISFY_CNT,SUM(t1.COMMON_CNT) AS COMMON_CNT,
                SUM(t1.UNSATISFY_CNT) AS UNSATISFY_CNT,SUM(t1.NON_EVAL_CNT) AS NON_EVAL_CNT,
                SUM(t1.OVERTIME_WAIT_CNT) AS OVERTIME_WAIT_CNT,SUM(t1.SECOND_SVR_CNT) AS SECOND_SVR_CNT,
                SUM(t1.WAIT_DUR) AS WAIT_DUR
                from STAT_STAFF_BUSI_TOT_D t1", field));

            if (!string.IsNullOrEmpty(staffid))
                sql.Append("where t1.STAFF_ID=@staffid", new { staffid = staffid });
            if (null != beginTime)
                sql.Append("where t1.STAT_DT>=@beginTime", new { beginTime = beginTime.Value });
            if (null != endTime)
                sql.Append("where t1.STAT_DT<=@endTime", new { endTime = endTime.Value });
            if (beginTime == endTime)
                sql.Append("where t1.TIME_QUANTUM_CD>=8 and TIME_QUANTUM_CD<=18");
            if (!string.IsNullOrEmpty(orgid))
            {
                var sql2 = Sql.Builder.Append(@"WITH t AS(
                      SELECT ORG_ID,ORG_NAM FROM dbo.SYS_ORGANIZE WHERE ORG_ID = @0
                      UNION ALL
                      SELECT a.ORG_ID,a.ORG_NAM FROM SYS_ORGANIZE AS a,t AS b WHERE a.PAR_ORG_ID = b.ORG_ID
                    )
                    SELECT t2.* FROM t JOIN dbo.SYS_HALL t2 ON t.ORG_ID=t2.ORG_ID", orgid);
                var hlist = db.Fetch<SYS_HALL>(sql2);
                sql.Append("where t1.HALL_NO in(@HALL_NO)", new { HALL_NO = hlist.Select(o => o.HALL_NO) });
            }
            sql.Append(string.Format("GROUP BY {0}", field));
            sql.Append(string.Format(")t order by ID asc", field));
            DataTable dt = db.Fill(sql.SQL, sql.Arguments);

            if ((null == beginTime || null == endTime) && dt != null && dt.Rows.Count > 0)
            {
                endTime = Convert.ToDateTime(dt.Compute("MAX(ID)", ""));
                beginTime = Convert.ToDateTime(dt.Compute("MIN(ID)", ""));
                return GetSTATList_DT(staffid, ref beginTime,ref endTime, orgid);
            }
            var e = dt.AsEnumerable().Select(o => o["ID"].ToString());
            if (beginTime == endTime)
            {
                for (int i = 8; i <= 18; i++)
                {
                    if (!e.Contains(i.ToString().PadLeft(2,'0')+"点"))
                    {
                        var newr = dt.NewRow();
                        newr[0] = i.ToString().PadLeft(2, '0') + "点";
                        for (int j = 1; j < dt.Columns.Count; j++)
                        {
                            newr[j] = 0;
                        }
                        dt.Rows.Add(newr);
                    }
                }
                
            }
            else
            {
                while (beginTime<=endTime)
                {
                    if (!e.Contains(beginTime.Value.ToString("yyyy年MM月dd")))
                    {
                        var newr = dt.NewRow();
                        newr[0] = beginTime.Value.ToString("yyyy年MM月dd");
                        for (int j = 1; j < dt.Columns.Count; j++)
                        {
                            newr[j] = 0;
                        }
                        dt.Rows.Add(newr);
                    }
                    beginTime = beginTime.Value.AddDays(1);
                }
            }
            dt.DefaultView.Sort = "ID";
            return dt.DefaultView.ToTable();
        }

        public Page<dynamic> GetSTATDetails(int pageIndex, int pageSize, string[] halllist, DateTime? beginTime, DateTime? endTime, string serid)
        {
            Database db = gzswDB.GetInstance();

            var sql = Sql.Builder.Append(@"SELECT [STAT_DT],
                      [TIME_QUANTUM_CD],t1.[Q_SERIALID],t2.Q_SERIALNAME,t1.[HALL_NO],t4.HALL_NAM,t1.[STAFF_ID],t3.STAFF_NAM,[CALL_CNT],
                      [HANDLE_CNT],[ABANDON_CNT],[LOCAL_CNT],[VOTE_MULTI_CNT],[VERY_SATISFY_CNT],[SATISFY_CNT],
                      [COMMON_CNT],[UNSATISFY_CNT],[NON_EVAL_CNT],[WAIT_DUR],[OVERTIME_WAIT_CNT],[SECOND_SVR_CNT]
                  FROM [dbo].[STAT_STAFF_BUSI_TOT_D] t1 join SYS_QUEUESERIAL t2 on t1.Q_SERIALID=t2.Q_SERIALID
                  join SYS_STAFF t3 on t3.STAFF_ID=t1.STAFF_ID
                  join SYS_HALL t4 on t4.HALL_NO=t1.HALL_NO");

            if (null != beginTime)
                sql.Append("where t1.STAT_DT>=@beginTime", new { beginTime = beginTime.Value });
            if (null != endTime)
                sql.Append("where t1.STAT_DT<=@endTime", new { endTime = endTime.Value });
            if (null != halllist && halllist.Length > 0)
                sql.Append("where t1.HALL_NO in (@hall)", new { hall = halllist });
            if (!string.IsNullOrEmpty(serid))
                sql.Append("where t1.Q_SERIALID=@Q_SERIALID", new { Q_SERIALID = serid });
            return db.Page<dynamic>(pageIndex, pageSize, sql);
        }

        public static Page<dynamic> GetListBubByHall(DateTime beginMo, DateTime endMo, string hallno, int pageIndex, int pageSize)
        {
            Database db = gzswDB.GetInstance();
            db.EnableAutoSelect = false;
            var sql = Sql.Builder.Append(@" SELECT 
            STAT_DT,
            TIME_QUANTUM_CD,
            SSD.Q_SERIALID,
            SSD.HALL_NO,
            SSD.STAFF_ID,
            CALL_CNT,
            HANDLE_CNT,
            ABANDON_CNT,
            LOCAL_CNT,
            VOTE_MULTI_CNT,
            VERY_SATISFY_CNT,
            SATISFY_CNT,
            COMMON_CNT,
            UNSATISFY_CNT,
            NON_EVAL_CNT,
            WAIT_DUR,
            OVERTIME_WAIT_CNT,
            SECOND_SVR_CNT,
            H.HALL_NAM,
            SA.STAFF_NAM
             FROM STAT_STAFF_BUSI_TOT_D  ssd,
             SYS_QUEUESERIAL SQ,SYS_HALL H,SYS_STAFF SA 
             WHERE SSD.Q_SERIALID=SQ.Q_SERIALID
             AND ssd.HALL_NO=H.HALL_NO
             AND SSD.STAFF_ID=SA.STAFF_ID ");

            sql.Append(" and ssd.STAT_DT>=@0", beginMo);
            sql.Append(" and ssd.STAT_DT<=@0", endMo);
            sql.Append(" and ssd.HALL_NO=@0", hallno);

            var data = db.Page<dynamic>(pageIndex, pageSize, sql);
            data.ItemsPerPage = pageSize;
            return data;
        }
    }
}
