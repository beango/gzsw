
using gzsw.model;
using gzsw.model.dto;
using gzsw.model.Subclasses;
using PetaPoco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace gzsw.dal.dao
{
    /// <summary>
    ///  报表统计
    /// </summary>
    /// <remark>
    ///     <para>    Creator：hcli </para>
    ///     <para>CreatedTime：2014/10/23 18:46:37</para>
    /// </remark>
    public class Statistics_DAL
    {

        #region 业务差错分析


        /// <summary>
        /// 获取业务差错分析数据
        /// </summary>
        /// <param name="beginTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="userId">用户ID</param>
        /// <returns></returns>
        public IList<ServiceSlipAnalysisDto> GetStatistics_ServiceSlipAnalysis(
            DateTime? beginTime,
            DateTime? endTime,
            string userId)
        {
            var sql = PetaPoco.Sql.Builder.Append(@" 
            select A.HALL_NO 
            ,D.SSDLSERIALID 
            ,a.QUALITY_CD 
            ,MAX(B.HALL_NAM) as HALL_NAM 
            ,max(c.QUALITY_NAM) as QUALITY_NAM 
            ,MAX(E.DLS_SERIALNAME) as DLS_SERIALNAME 
            ,SUM(AMOUNT) AS AMOUNT 
            from STAT_STAFF_QUALITY_STAT_D A 
            join CHK_QUALITY_CON c on a.QUALITY_CD=c.QUALITY_CD 
            JOIN SYS_HALL B ON A.HALL_NO=B.HALL_NO 
            join SYS_DETAILSERIAL d on d.SERIALID=A.SERIALID 
            JOIN SYS_DLSERIAL E ON E.DLS_SERIALID=D.SSDLSERIALID 
            where A.HALL_NO in(select ORG_ID from SYS_ORGANIZE  
            where ORG_ID in ( select ORG_ID from SYS_USERORGANIZE where USER_ID =@0 )   and ORG_LEVEL =4) 
            and A.STAFF_ID is not null and A.STAFF_ID <>'' "
            + GetSelectWhereTime(beginTime, endTime, "STAT_DT"), userId); 
            sql.Append(@" group by   A.HALL_NO,D.SSDLSERIALID,a.QUALITY_CD ");
            Database db = gzswDB.GetInstance();
            return db.Fetch<ServiceSlipAnalysisDto>(sql);
        }

        /// <summary>
        /// 获取所有业务差错分析数据
        /// </summary>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <param name="userId"></param>
        /// <param name="orgId"></param>
        /// <returns></returns>
        public IList<STAT_STAFF_QUALITY_STAT_D> GetStatistics_ServiceSlipAnalysisList(
            DateTime? beginTime,
            DateTime? endTime,
            string userId)
        {
            var sql = Sql.Builder.Append(@" From STAT_STAFF_QUALITY_STAT_D  WHERE 1=1 "
                                        +GetSelectWhereTime(beginTime, endTime, "STAT_DT")
                                        ,userId);

            var db = gzswDB.GetInstance();
            return db.Fetch<STAT_STAFF_QUALITY_STAT_D>(sql);
        }

        public IList<STAT_STAFF_QUALITY_STAT_D_SUB> GetStatistics_ServiceSlipAnalysisList(
            DateTime? beginTime,
            DateTime? endTime,
            string userId,
            string orgId)
        {
            var sql = Sql.Builder.Append(@"SELECT
                                        Q.[STAT_DT]
                                        ,Q.[TIME_QUANTUM_CD]
                                        ,Q.[HALL_NO]
                                        ,Q.[STAFF_ID]
                                        ,Q.[QUALITY_CD]
                                        ,Q.[SERIALID]
                                        ,Q.[AMOUNT]
                                        ,ST.STAFF_NAM
                                        ,E.DLS_SERIALID
                                        From STAT_STAFF_QUALITY_STAT_D AS Q
                                        JOIN SYS_STAFF AS ST
                                        ON Q.STAFF_ID=ST.STAFF_ID
                                        JOIN SYS_DETAILSERIAL D on D.SERIALID=Q.SERIALID  
                                        JOIN SYS_DLSERIAL E ON E.DLS_SERIALID=D.SSDLSERIALID  
                                        WHERE 1=1 "
                                       + GetSelectWhereTime(beginTime, endTime, "Q.STAT_DT")
                                       , userId);

            if (string.IsNullOrEmpty(orgId))
            {
                sql.Append(@"");
            }

            var db = gzswDB.GetInstance();
            return db.Fetch<STAT_STAFF_QUALITY_STAT_D_SUB>(sql);
        }

        #endregion 

        #region 纳税人评价分析

        /// <summary>
        /// 纳税人行为分析报表统计
        /// </summary>
        /// <param name="beginTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="orgId">营业厅ID</param>
        /// <returns></returns>
        public IList<Statistics_TaxpayerActionDto> GetStatistics_TaxpayerActionChat_Person(
            DateTime? beginTime,
            DateTime? endTime,
            string orgId)
        {
            return new List<Statistics_TaxpayerActionDto>();
        }


        /// <summary>
        /// 纳税人行为分析报表统计
        /// </summary>
        /// <param name="beginTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="userId">用户</param>
        /// <returns></returns>
        public IList<Statistics_TaxpayerActionDto> GetStatistics_TaxpayerActionChat(
            DateTime? beginTime,
            DateTime? endTime,
            string userId)
        {
            return new List<Statistics_TaxpayerActionDto>();
        }

        #endregion

        #region 纳税人评价分析
        /// <summary>
        /// 获取营业厅统计报表
        /// </summary>
        /// <param name="beginTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="userId">用户ID</param> 
        /// <returns></returns>
        public IList<Statistics_TaxpayerEvalDto> GetStatistics_TaxpayerEvalChart( 
            DateTime? beginTime,
            DateTime? endTime, 
            string userId)
        {

            var sql = PetaPoco.Sql.Builder.Append(@" 
            SELECT HALL_NO,
            B.ORG_NAM, 
            sum(A.VERY_SATISFY_CNT) as VERY_SATISFY_CNT , 
            (case when SUM(A.[VERY_SATISFY_CNT]+A.[SATISFY_CNT]+A.[COMMON_CNT]+A.[UNSATISFY_CNT]) =0 then 0 else SUM(A.[VERY_SATISFY_CNT])*100/SUM(A.[VERY_SATISFY_CNT]+A.[SATISFY_CNT]+A.[COMMON_CNT]+A.[UNSATISFY_CNT]) end ) as VERY_SATISFY_CNT_BFB,
            sum(A.SATISFY_CNT) as SATISFY_CNT, 
           (case when SUM(A.[VERY_SATISFY_CNT]+A.[SATISFY_CNT]+A.[COMMON_CNT]+A.[UNSATISFY_CNT]) =0 then 0 else SUM(A.[SATISFY_CNT])*100/SUM(A.[VERY_SATISFY_CNT]+A.[SATISFY_CNT]+A.[COMMON_CNT]+A.[UNSATISFY_CNT]) end ) as SATISFY_CNT_BFB,
            sum(A.COMMON_CNT) as COMMON_CNT, 
           (case when SUM(A.[VERY_SATISFY_CNT]+A.[SATISFY_CNT]+A.[COMMON_CNT]+A.[UNSATISFY_CNT]) =0 then 0 else SUM(A.[COMMON_CNT])*100/SUM(A.[VERY_SATISFY_CNT]+A.[SATISFY_CNT]+A.[COMMON_CNT]+A.[UNSATISFY_CNT]) end ) as COMMON_CNT_BFB,
            sum(A.UNSATISFY_CNT) as UNSATISFY_CNT,  
            (case when SUM(A.[VERY_SATISFY_CNT]+A.[SATISFY_CNT]+A.[COMMON_CNT]+A.[UNSATISFY_CNT]) =0 then 0 else SUM(A.[UNSATISFY_CNT])*100/SUM(A.[VERY_SATISFY_CNT]+A.[SATISFY_CNT]+A.[COMMON_CNT]+A.[UNSATISFY_CNT]) end ) as UNSATISFY_CNT_BFB,
             sum(A.NON_EVAL_CNT) as NON_EVAL_CNT,
           (case when SUM(A.[VERY_SATISFY_CNT]+A.[SATISFY_CNT]+A.[COMMON_CNT]+A.[UNSATISFY_CNT]+A.[NON_EVAL_CNT]) =0 then 0 else SUM(A.[NON_EVAL_CNT])*100/SUM(A.[VERY_SATISFY_CNT]+A.[SATISFY_CNT]+A.[COMMON_CNT]+A.[UNSATISFY_CNT]+A.[NON_EVAL_CNT]) end ) as NON_EVAL_CNT_BFB,
           (case when SUM(A.[VERY_SATISFY_CNT]+A.[SATISFY_CNT]+A.[COMMON_CNT]) =0 then 0 else SUM(A.[VERY_SATISFY_CNT]+A.[SATISFY_CNT]+A.[COMMON_CNT])*100/SUM(A.[VERY_SATISFY_CNT]+A.[SATISFY_CNT]+A.[COMMON_CNT]+A.[UNSATISFY_CNT]) end ) as ManYiDu_BFB
             FROM STAT_STAFF_BUSI_TOT_D AS A 
            left join  SYS_ORGANIZE AS B on A.HALL_NO =B.ORG_ID 
            where HALL_NO in(select ORG_ID from SYS_ORGANIZE 
            where ORG_ID in ( select ORG_ID from SYS_USERORGANIZE where USER_ID =@0 )   and ORG_LEVEL =4)  and A.STAFF_ID is not null and A.STAFF_ID <>'' " + GetSelectWhereTime(beginTime, endTime, "STAT_DT"), userId);
          
           /*   if (period != null)
            {
                sql.Append(@" AND TIME_QUANTUM_CD = @0 ", period);
            } */
            sql.Append(@" group by HALL_NO,ORG_NAM  ");

            Database db = gzswDB.GetInstance();
            return db.Fetch<Statistics_TaxpayerEvalDto>(sql);
        }



        /// <summary>
        /// 获取个人统计报表
        /// </summary>
        /// <param name="beginTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="orgId">营业厅ID</param> 
        /// <returns></returns>
        public IList<Statistics_TaxpayerEvalDto> GetStatistics_TaxpayerEvalChart_Person(
            DateTime? beginTime,
            DateTime? endTime,
            string orgId)
        {

            var sql = PetaPoco.Sql.Builder.Append(@" 
            SELECT  A.STAFF_ID as PersonNo,
            B.STAFF_NAM as PersonName,  
            sum(A.VERY_SATISFY_CNT) as VERY_SATISFY_CNT , 
            (case when SUM(A.[VERY_SATISFY_CNT]+A.[SATISFY_CNT]+A.[COMMON_CNT]+A.[UNSATISFY_CNT]) =0 then 0 else SUM(A.[VERY_SATISFY_CNT])*100/SUM(A.[VERY_SATISFY_CNT]+A.[SATISFY_CNT]+A.[COMMON_CNT]+A.[UNSATISFY_CNT]) end ) as VERY_SATISFY_CNT_BFB,
            sum(A.SATISFY_CNT) as SATISFY_CNT, 
           (case when SUM(A.[VERY_SATISFY_CNT]+A.[SATISFY_CNT]+A.[COMMON_CNT]+A.[UNSATISFY_CNT]) =0 then 0 else SUM(A.[SATISFY_CNT])*100/SUM(A.[VERY_SATISFY_CNT]+A.[SATISFY_CNT]+A.[COMMON_CNT]+A.[UNSATISFY_CNT]) end ) as SATISFY_CNT_BFB,
            sum(A.COMMON_CNT) as COMMON_CNT, 
           (case when SUM(A.[VERY_SATISFY_CNT]+A.[SATISFY_CNT]+A.[COMMON_CNT]+A.[UNSATISFY_CNT]) =0 then 0 else SUM(A.[COMMON_CNT])*100/SUM(A.[VERY_SATISFY_CNT]+A.[SATISFY_CNT]+A.[COMMON_CNT]+A.[UNSATISFY_CNT]) end ) as COMMON_CNT_BFB,
            sum(A.UNSATISFY_CNT) as UNSATISFY_CNT,  
            (case when SUM(A.[VERY_SATISFY_CNT]+A.[SATISFY_CNT]+A.[COMMON_CNT]+A.[UNSATISFY_CNT]) =0 then 0 else SUM(A.[UNSATISFY_CNT])*100/SUM(A.[VERY_SATISFY_CNT]+A.[SATISFY_CNT]+A.[COMMON_CNT]+A.[UNSATISFY_CNT]) end ) as UNSATISFY_CNT_BFB,
             sum(A.NON_EVAL_CNT) as NON_EVAL_CNT,
           (case when SUM(A.[VERY_SATISFY_CNT]+A.[SATISFY_CNT]+A.[COMMON_CNT]+A.[UNSATISFY_CNT]+A.[NON_EVAL_CNT]) =0 then 0 else SUM(A.[NON_EVAL_CNT])*100/SUM(A.[VERY_SATISFY_CNT]+A.[SATISFY_CNT]+A.[COMMON_CNT]+A.[UNSATISFY_CNT]+A.[NON_EVAL_CNT]) end ) as NON_EVAL_CNT_BFB,
           (case when SUM(A.[VERY_SATISFY_CNT]+A.[SATISFY_CNT]+A.[COMMON_CNT]) =0 then 0 else SUM(A.[VERY_SATISFY_CNT]+A.[SATISFY_CNT]+A.[COMMON_CNT])*100/SUM(A.[VERY_SATISFY_CNT]+A.[SATISFY_CNT]+A.[COMMON_CNT]+A.[UNSATISFY_CNT]) end ) as ManYiDu_BFB
            
            FROM STAT_STAFF_BUSI_TOT_D AS A  
            left join SYS_STAFF AS B ON A.STAFF_ID =B.STAFF_ID 
            where HALL_NO  = @0  and A.STAFF_ID is not null and A.STAFF_ID <>'' " + GetSelectWhereTime(beginTime, endTime, "STAT_DT"), orgId);
             
            sql.Append(@" group by  A.STAFF_ID ,B.STAFF_NAM  ");

            Database db = gzswDB.GetInstance();
            return db.Fetch<Statistics_TaxpayerEvalDto>(sql);
        }


        public IList<Statistics_KeyValueDto> GetStatistics_TaxpayerEvalChart_Node(
            string columnName,
            DateTime? beginTime,
            DateTime? endTime,
            string userId)
        { 
            
            if (beginTime.HasValue && endTime.HasValue)
            {
                TimeSpan timeSpan = endTime.Value.Subtract(beginTime.Value);


                var sql = string.Empty;

                if (timeSpan.TotalDays > 365)
                {
                     // 以年为单位   
                    sql = "  select year(STAT_DT) as Name,count(" + columnName + ") as Value  FROM STAT_STAFF_BUSI_TOT_D " +
                          "   where HALL_NO in  (select ORG_ID from SYS_ORGANIZE    where ORG_ID in ( select ORG_ID from SYS_USERORGANIZE where USER_ID =@0 )   and ORG_LEVEL =4)   " + GetSelectWhereTime(beginTime, endTime, "STAT_DT") +
                          "  Group by  year(STAT_DT)";

                }else if (timeSpan.TotalDays > 31 && timeSpan.TotalDays <= 365)
                {
                    // 以月份为单位
                    sql = "  select convert(varchar(7),STAT_DT,120) as Name,count(" + columnName + ") as Value  FROM STAT_STAFF_BUSI_TOT_D  " +
                         "   where HALL_NO in (select ORG_ID from SYS_ORGANIZE    where ORG_ID in ( select ORG_ID from SYS_USERORGANIZE where USER_ID =@0 )  and ORG_LEVEL =4)   " + GetSelectWhereTime(beginTime, endTime, "STAT_DT") +
                        "  Group by convert(varchar(7),STAT_DT,120)";

                }
                else if (timeSpan.TotalDays > 1 && timeSpan.TotalDays <= 31)
                {
                    //以天为单位
                    sql = " select convert(varchar(100),STAT_DT,23) as Name,count(" + columnName + ") as Value  FROM STAT_STAFF_BUSI_TOT_D  " +
                          "  where HALL_NO in  (select ORG_ID from SYS_ORGANIZE    where ORG_ID in ( select ORG_ID from SYS_USERORGANIZE where USER_ID =@0 )  and ORG_LEVEL =4)  " + GetSelectWhereTime(beginTime, endTime, "STAT_DT") +
                        "   Group by convert(varchar(100),STAT_DT,23)";
                }
                else 
                {
                    sql = " select TIME_QUANTUM_CD as Name,count(" + columnName + ") as Value  FROM STAT_STAFF_BUSI_TOT_D  " +
                       "  where HALL_NO in  (select ORG_ID from SYS_ORGANIZE    where ORG_ID in ( select ORG_ID from SYS_USERORGANIZE where USER_ID =@0 )  and ORG_LEVEL =4)  " + GetSelectWhereTime(beginTime, endTime, "STAT_DT") +
                     "   Group by TIME_QUANTUM_CD";
                }
                var querySql = PetaPoco.Sql.Builder.Append(sql, userId);
                Database db = gzswDB.GetInstance();
                return db.Fetch<Statistics_KeyValueDto>(querySql); 
            }
            return new List<Statistics_KeyValueDto>();
        }


        /// <summary>
        /// 获取满意度
        /// </summary>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public IList<Statistics_KeyValueDto> GetStatistics_TaxpayerEvalChart_ManYiDu_Node( 
            DateTime? beginTime,
            DateTime? endTime,
            string userId)
        {

            if (beginTime.HasValue && endTime.HasValue)
            {
                var sql = string.Empty;

                if (beginTime.Value.Year != endTime.Value.Year)
                {
                    // 以年为单位   
                    sql = "  SELECT  year(STAT_DT) as Name,(CASE WHEN SUM(A.VERY_SATISFY_CNT+A.SATISFY_CNT+A.COMMON_CNT+A.UNSATISFY_CNT+A.NON_EVAL_CNT)=0 THEN 0 ELSE  " +
                          "SUM(A.VERY_SATISFY_CNT*B.VERY_SATISFY_SCORE + A.SATISFY_CNT*B.SATISFY_SCORE " +
                          "+A.COMMON_CNT*B.COMMON_SCORE " +
                          "+A.UNSATISFY_CNT*B.UNSATISFY_SCORE " +
                          "+A.NON_EVAL_CNT*B.NON_EVAL_SCORE) " +
                          "/SUM(A.VERY_SATISFY_CNT+A.SATISFY_CNT+A.COMMON_CNT+A.UNSATISFY_CNT+A.NON_EVAL_CNT) " +
                          "END) as Value FROM " +
                          "STAT_STAFF_BUSI_TOT_D A JOIN CHK_SATIS_RT_CON B ON  " +
                          " [dbo].[FUN_GET_HALL_CITY_ORG_ID](A.HALL_NO)=B.ORG_ID " +
                          "where HALL_NO in  (select ORG_ID from SYS_ORGANIZE    where ORG_ID in ( select ORG_ID from SYS_USERORGANIZE where USER_ID =@0 )   and ORG_LEVEL =4)   " +
                           GetSelectWhereTime(beginTime, endTime, "STAT_DT") +
                          " Group by  year(STAT_DT)";

                }
                else if (beginTime.Value.Month != endTime.Value.Month)
                {
                    // 以月份为单位 

                    sql = "  SELECT  convert(varchar(7),STAT_DT,120)  as Name,(CASE WHEN SUM(A.VERY_SATISFY_CNT+A.SATISFY_CNT+A.COMMON_CNT+A.UNSATISFY_CNT+A.NON_EVAL_CNT)=0 THEN 0 ELSE  " +
                          "SUM(A.VERY_SATISFY_CNT*B.VERY_SATISFY_SCORE + A.SATISFY_CNT*B.SATISFY_SCORE " +
                          "+A.COMMON_CNT*B.COMMON_SCORE " +
                          "+A.UNSATISFY_CNT*B.UNSATISFY_SCORE " +
                          "+A.NON_EVAL_CNT*B.NON_EVAL_SCORE) " +
                          "/SUM(A.VERY_SATISFY_CNT+A.SATISFY_CNT+A.COMMON_CNT+A.UNSATISFY_CNT+A.NON_EVAL_CNT) " +
                          "END) as Value FROM " +
                         "STAT_STAFF_BUSI_TOT_D A JOIN CHK_SATIS_RT_CON B ON  " +
                         " [dbo].[FUN_GET_HALL_CITY_ORG_ID](A.HALL_NO)=B.ORG_ID " +
                         "where HALL_NO in  (select ORG_ID from SYS_ORGANIZE    where ORG_ID in ( select ORG_ID from SYS_USERORGANIZE where USER_ID =@0 )   and ORG_LEVEL =4)   " +
                          GetSelectWhereTime(beginTime, endTime, "STAT_DT") +
                         " Group by  convert(varchar(7),STAT_DT,120) ";

                }
                else if (beginTime.Value.Day != endTime.Value.Day)
                {
                    //以天为单位
                    sql = "  SELECT  convert(varchar(100),STAT_DT,23) as Name,(CASE WHEN SUM(A.VERY_SATISFY_CNT+A.SATISFY_CNT+A.COMMON_CNT+A.UNSATISFY_CNT+A.NON_EVAL_CNT)=0 THEN 0 ELSE  " +
                          "SUM(A.VERY_SATISFY_CNT*B.VERY_SATISFY_SCORE + A.SATISFY_CNT*B.SATISFY_SCORE " +
                          "+A.COMMON_CNT*B.COMMON_SCORE " +
                          "+A.UNSATISFY_CNT*B.UNSATISFY_SCORE " +
                          "+A.NON_EVAL_CNT*B.NON_EVAL_SCORE) " +
                          "/SUM(A.VERY_SATISFY_CNT+A.SATISFY_CNT+A.COMMON_CNT+A.UNSATISFY_CNT+A.NON_EVAL_CNT) " +
                          "END) as Value FROM " +
                       "STAT_STAFF_BUSI_TOT_D A JOIN CHK_SATIS_RT_CON B ON  " +
                       " [dbo].[FUN_GET_HALL_CITY_ORG_ID](A.HALL_NO)=B.ORG_ID " +
                       "where HALL_NO in  (select ORG_ID from SYS_ORGANIZE    where ORG_ID in ( select ORG_ID from SYS_USERORGANIZE where USER_ID =@0 )   and ORG_LEVEL =4)   " +
                        GetSelectWhereTime(beginTime, endTime, "STAT_DT") +
                       " Group by   convert(varchar(100),STAT_DT,23) ";


                     
                }
                else if (beginTime.Value.Day == endTime.Value.Day)
                {
                    sql = "  SELECT  TIME_QUANTUM_CD as Name,(CASE WHEN SUM(A.VERY_SATISFY_CNT+A.SATISFY_CNT+A.COMMON_CNT+A.UNSATISFY_CNT+A.NON_EVAL_CNT)=0 THEN 0 ELSE  " +
                          "SUM(A.VERY_SATISFY_CNT*B.VERY_SATISFY_SCORE + A.SATISFY_CNT*B.SATISFY_SCORE " +
                          "+A.COMMON_CNT*B.COMMON_SCORE " +
                          "+A.UNSATISFY_CNT*B.UNSATISFY_SCORE " +
                          "+A.NON_EVAL_CNT*B.NON_EVAL_SCORE) " +
                          "/SUM(A.VERY_SATISFY_CNT+A.SATISFY_CNT+A.COMMON_CNT+A.UNSATISFY_CNT+A.NON_EVAL_CNT) " +
                          "END) as Value FROM " +
                       "STAT_STAFF_BUSI_TOT_D A JOIN CHK_SATIS_RT_CON B ON  " +
                       " [dbo].[FUN_GET_HALL_CITY_ORG_ID](A.HALL_NO)=B.ORG_ID " +
                       "where HALL_NO in  (select ORG_ID from SYS_ORGANIZE    where ORG_ID in ( select ORG_ID from SYS_USERORGANIZE where USER_ID =@0 )   and ORG_LEVEL =4)   " +
                        GetSelectWhereTime(beginTime, endTime, "STAT_DT") +
                       " Group by   TIME_QUANTUM_CD "; 
                }
                var querySql = PetaPoco.Sql.Builder.Append(sql, userId);
                Database db = gzswDB.GetInstance();
                return db.Fetch<Statistics_KeyValueDto>(querySql);
            }
            return new List<Statistics_KeyValueDto>();
        }
        #endregion


        #region Helper

        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <param name="dateTimeName"></param>
        /// <returns></returns>
        private string GetSelectWhereTime(
            DateTime? beginTime, 
            DateTime? endTime,
            string dateTimeName = "QDDTJ_DAY")
        {
            var str = string.Empty;
            if(beginTime!=null&&beginTime!=DateTime.MinValue)
            {
                str += string.Format(" AND " + dateTimeName + " >= '{0}'", beginTime);
            }
            if(endTime!=null&&endTime!=DateTime.MinValue)
            {
                str += string.Format(" AND  " + dateTimeName + "  <= '{0}'", endTime);
            }
            return str;
        }
         
        #endregion
    }
}
