
using gzsw.model.dto;
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
        /// <summary>
        /// 获取员工满意度数据
        /// </summary>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <param name="orgId"></param>
        /// <returns></returns>
        public IList<Statistics_STAT_DAYYWTJ> GetSTAT_DAYYWTJ(DateTime? beginTime,
            DateTime? endTime,
            string orgId)
        { 
            var sql = PetaPoco.Sql.Builder.Append(@" SELECT 
  A.[QDDTJ_SNO]   
	  ,D.STAFF_NAM    --员工姓名 
      ,sum(A.[QDDTJ_PJKEY1NUM]) as [QDDTJ_PJKEY1NUM]--很满意次数
      ,sum(A.[QDDTJ_PJKEY2NUM]) as [QDDTJ_PJKEY2NUM]--满意次数
      ,sum(A.[QDDTJ_PJKEY3NUM]) as [QDDTJ_PJKEY3NUM]--一般次数
      ,sum(A.[QDDTJ_PJKEY4NUM]) as [QDDTJ_PJKEY4NUM]--不满意次数
	  ,sum(A.[QDDTJ_PJKEY5NUM]) as [QDDTJ_PJKEY5NUM]--很不满意次数	 
	  ,sum(A.[QDDTJ_PJKEY5NUM]) as [QDDTJ_PJKEY0NUM]--未评级量	 
	  ,(case when SUM(A.[QDDTJ_PJKEY0NUM]+A.[QDDTJ_PJKEY1NUM]+A.[QDDTJ_PJKEY2NUM]+A.[QDDTJ_PJKEY3NUM]+A.[QDDTJ_PJKEY4NUM]+A.[QDDTJ_PJKEY5NUM]) =0 then 0 else SUM(A.[QDDTJ_PJKEY1NUM])*100/SUM(A.[QDDTJ_PJKEY0NUM]+A.[QDDTJ_PJKEY1NUM]+A.[QDDTJ_PJKEY2NUM]+A.[QDDTJ_PJKEY3NUM]+A.[QDDTJ_PJKEY4NUM]+A.[QDDTJ_PJKEY5NUM]) end ) as KEY1RT --很满意占比
	   ,(case when SUM(A.[QDDTJ_PJKEY0NUM]+A.[QDDTJ_PJKEY1NUM]+A.[QDDTJ_PJKEY2NUM]+A.[QDDTJ_PJKEY3NUM]+A.[QDDTJ_PJKEY4NUM]+A.[QDDTJ_PJKEY5NUM]) =0 then 0 else SUM(A.[QDDTJ_PJKEY2NUM])*100/SUM(A.[QDDTJ_PJKEY0NUM]+A.[QDDTJ_PJKEY1NUM]+A.[QDDTJ_PJKEY2NUM]+A.[QDDTJ_PJKEY3NUM]+A.[QDDTJ_PJKEY4NUM]+A.[QDDTJ_PJKEY5NUM]) end ) as KEY2RT--满意占比
       ,(case when SUM(A.[QDDTJ_PJKEY0NUM]+A.[QDDTJ_PJKEY1NUM]+A.[QDDTJ_PJKEY2NUM]+A.[QDDTJ_PJKEY3NUM]+A.[QDDTJ_PJKEY4NUM]+A.[QDDTJ_PJKEY5NUM]) =0 then 0 else SUM(A.[QDDTJ_PJKEY3NUM])*100/SUM(A.[QDDTJ_PJKEY0NUM]+A.[QDDTJ_PJKEY1NUM]+A.[QDDTJ_PJKEY2NUM]+A.[QDDTJ_PJKEY3NUM]+A.[QDDTJ_PJKEY4NUM]+A.[QDDTJ_PJKEY5NUM]) end ) as KEY3RT--一般占比
        ,(case when SUM(A.[QDDTJ_PJKEY0NUM]+A.[QDDTJ_PJKEY1NUM]+A.[QDDTJ_PJKEY2NUM]+A.[QDDTJ_PJKEY3NUM]+A.[QDDTJ_PJKEY4NUM]+A.[QDDTJ_PJKEY5NUM]) =0 then 0 else SUM(A.[QDDTJ_PJKEY4NUM])*100/SUM(A.[QDDTJ_PJKEY0NUM]+A.[QDDTJ_PJKEY1NUM]+A.[QDDTJ_PJKEY2NUM]+A.[QDDTJ_PJKEY3NUM]+A.[QDDTJ_PJKEY4NUM]+A.[QDDTJ_PJKEY5NUM]) end ) as KEY4RT--不满意占比
	    ,(case when SUM(A.[QDDTJ_PJKEY0NUM]+A.[QDDTJ_PJKEY1NUM]+A.[QDDTJ_PJKEY2NUM]+A.[QDDTJ_PJKEY3NUM]+A.[QDDTJ_PJKEY4NUM]+A.[QDDTJ_PJKEY5NUM]) =0 then 0 else SUM(A.[QDDTJ_PJKEY5NUM])*100/SUM(A.[QDDTJ_PJKEY0NUM]+A.[QDDTJ_PJKEY1NUM]+A.[QDDTJ_PJKEY2NUM]+A.[QDDTJ_PJKEY3NUM]+A.[QDDTJ_PJKEY4NUM]+A.[QDDTJ_PJKEY5NUM]) end ) as KEY5RT--很不满意占比
        ,(case when SUM(A.[QDDTJ_PJKEY0NUM]+A.[QDDTJ_PJKEY1NUM]+A.[QDDTJ_PJKEY2NUM]+A.[QDDTJ_PJKEY3NUM]+A.[QDDTJ_PJKEY4NUM]+A.[QDDTJ_PJKEY5NUM]) =0 then 0 else SUM(A.[QDDTJ_PJKEY0NUM])*100/SUM(A.[QDDTJ_PJKEY0NUM]+A.[QDDTJ_PJKEY1NUM]+A.[QDDTJ_PJKEY2NUM]+A.[QDDTJ_PJKEY3NUM]+A.[QDDTJ_PJKEY4NUM]+A.[QDDTJ_PJKEY5NUM]) end ) as KEY0RT--未评级量
  FROM [dbo].[STAT_DAYQUEUETJ] A JOIN  SYS_HALL B ON A.[QDDTJ_SYSNO]=B.HALL_NO 
  JOIN SYS_ORGANIZE E ON   [dbo].[isParent](B.ORG_ID,E.ORG_ID)=1 
   JOIN SYS_STAFF D ON D.STAFF_ID=A.[QDDTJ_SNO]
  WHERE E.ORG_ID=@0  " + GetSelectWhereTime(beginTime, endTime) + " GROUP BY A.[QDDTJ_SNO] ,D.STAFF_NAM ORDER BY D.STAFF_NAM  DESC ", orgId);
  Database db = new Database();
  return db.Fetch<Statistics_STAT_DAYYWTJ>(sql);
        }

        /// <summary>
        /// 获取满意度数据 图表数据
        /// </summary>
        /// <param name="orgId"></param>
        /// <returns></returns>
        public IList<Statistics_STAT_DAYYWTJChart> GetSTAT_DAYYWTJChart(string orgId,
            DateTime? beginTime,
            DateTime? endTime)
        {
            var sql = PetaPoco.Sql.Builder.Append(@" 
                 SELECT A.[QDDTJ_SNO]   
	  ,D.STAFF_NAM    --员工姓名 
      ,sum(A.[QDDTJ_PJKEY1NUM]) as [QDDTJ_PJKEY1NUM]--很满意次数
      ,sum(A.[QDDTJ_PJKEY2NUM]) as [QDDTJ_PJKEY2NUM]--满意次数
      ,sum(A.[QDDTJ_PJKEY3NUM]) as [QDDTJ_PJKEY3NUM]--一般次数
      ,sum(A.[QDDTJ_PJKEY4NUM]) as [QDDTJ_PJKEY4NUM]--不满意次数
   ,sum(A.[QDDTJ_PJKEY5NUM]) as [QDDTJ_PJKEY5NUM]--很不满意次数  
   ,sum(A.[QDDTJ_PJKEY0NUM]) as [QDDTJ_PJKEY0NUM]--未评级量  
  FROM [dbo].[STAT_DAYQUEUETJ] A JOIN  SYS_HALL B ON A.[QDDTJ_SYSNO]=B.HALL_NO 
  JOIN SYS_ORGANIZE E ON   [dbo].[isParent](B.ORG_ID,E.ORG_ID)=1 
   JOIN SYS_STAFF D ON D.STAFF_ID=A.[QDDTJ_SNO]
  WHERE E.ORG_ID=@0  " + GetSelectWhereTime(beginTime, endTime) + "  GROUP BY A.[QDDTJ_SNO]   ,D.STAFF_NAM ORDER BY D.STAFF_NAM DESC ", orgId);
            Database db = new Database();
            return db.Fetch<Statistics_STAT_DAYYWTJChart>(sql);
        }



        /// <summary>
        /// 获取个人绩效数据
        /// </summary>
        /// <param name="beginTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="orgId">组织结构ID</param>
        /// <returns></returns>
        public IList<Statistics_DAYQUEUETJ> GetDAYQUEUETJ(
            DateTime? beginTime,
            DateTime? endTime, 
            string orgId)
        {
            var sql = PetaPoco.Sql.Builder.Append(@"    SELECT 
K1.QDDTJ_SNO,
  K1.STAFF_NAM ,
  SUM(K1.STAFF_QDDTJ_TICKETNUM) as STAFF_QDDTJ_TICKETNUM--呼叫量
  ,SUM(K1.STAFF_QDDTJ_RLL) as STAFF_QDDTJ_RLL--办理量
  ,SUM(K1.STAFF_QDDTJ_QHNUM) as STAFF_QDDTJ_QHNUM--弃号量
  ,(CASE WHEN SUM(K2.HALL_QDDTJ_RLL)=0 then 0 else (SUM(K1.STAFF_QDDTJ_RLL*100)/SUM(K2.HALL_QDDTJ_RLL)) end   )as DoAllBFB --呼叫量占比
   ,(CASE WHEN SUM(K2.HALL_QDDTJ_QHNUM)=0 then 0 else  (SUM(K1.STAFF_QDDTJ_QHNUM*100)/SUM(K2.HALL_QDDTJ_QHNUM)) end )as NoDoBFB --办理量占比
    ,(CASE WHEN SUM(K2.HALL_QDDTJ_TICKETNUM)=0 then 0 else  (SUM(K1.STAFF_QDDTJ_TICKETNUM*100)/SUM(K2.HALL_QDDTJ_TICKETNUM)) end )as NoDoCountBFB --弃号量占比   
	from 
  ( SELECT    
    A.[QDDTJ_SYSNO]  ,a.QDDTJ_SNO	
	  ,D.STAFF_NAM    --员工姓名 
      ,sum(A.[QDDTJ_TICKETNUM]) AS STAFF_QDDTJ_TICKETNUM--人流量
      ,sum(A.[QDDTJ_RLL]) AS STAFF_QDDTJ_RLL--交易完结人流量
      ,sum(A.[QDDTJ_QHNUM]) AS STAFF_QDDTJ_QHNUM--弃号数量		 
  FROM [dbo].[STAT_DAYQUEUETJ] A JOIN  SYS_HALL B ON A.[QDDTJ_SYSNO]=B.HALL_NO 
  JOIN SYS_ORGANIZE E ON   [dbo].[isParent](B.ORG_ID,E.ORG_ID)=1 
   JOIN SYS_STAFF D ON D.STAFF_ID=A.[QDDTJ_SNO]
  WHERE E.ORG_ID=@0 " + GetSelectWhereTime(beginTime, endTime) + " GROUP BY A.[QDDTJ_SYSNO],a.QDDTJ_SNO,D.STAFF_NAM ) K1 join  ( SELECT A.[QDDTJ_SYSNO] ,sum(A.[QDDTJ_TICKETNUM]) AS HALL_QDDTJ_TICKETNUM ,sum(A.[QDDTJ_RLL]) AS HALL_QDDTJ_RLL  ,sum(A.[QDDTJ_QHNUM]) AS HALL_QDDTJ_QHNUM FROM [dbo].[STAT_DAYQUEUETJ] A JOIN  SYS_HALL B ON A.[QDDTJ_SYSNO]=B.HALL_NO  JOIN SYS_ORGANIZE E ON   [dbo].[isParent](B.ORG_ID,E.ORG_ID)=1  JOIN SYS_STAFF D ON D.STAFF_ID=A.[QDDTJ_SNO]  WHERE E.ORG_ID= @0 " + GetSelectWhereTime(beginTime, endTime) + " GROUP BY A.[QDDTJ_SYSNO] ) K2 on (K1.[QDDTJ_SYSNO]=K2.[QDDTJ_SYSNO] ) GROUP BY  K1.QDDTJ_SNO,K1.STAFF_NAM  ORDER BY  K1.STAFF_NAM DESC ", orgId);
             
            Database db = new Database();
            return db.Fetch<Statistics_DAYQUEUETJ>(sql);
        }

        /// <summary>
        /// 获取个人绩效 图表数据
        /// </summary>
        /// <param name="orgId"></param>
        /// <returns></returns>
        public IList<Statistics_DAYQUEUETJChart> GetCharForDAYQUEUETJChart(string orgId,
            DateTime? beginTime,
            DateTime? endTime)
        {

                var sql = PetaPoco.Sql.Builder.Append(@" 
                   SELECT A.[QDDTJ_SNO]   
	  ,D.STAFF_NAM    --员工姓名     --X轴
                    ,SUM(A.[QDDTJ_TICKETNUM]) as PersonCount--呼叫量  ---Y轴  
					 ,sum(A.[QDDTJ_RLL]) AS STAFF_QDDTJ_RLL--办理量
      ,sum(A.[QDDTJ_QHNUM]) AS STAFF_QDDTJ_QHNUM--弃号量   
                    ,MAX(E.ORG_NAM) AS ORG_NAM--组织机构名称   （在图表旁边标明，让操作人知道）
                    FROM [dbo].[STAT_DAYQUEUETJ] A JOIN  SYS_HALL B ON A.[QDDTJ_SYSNO]=B.HALL_NO
                    JOIN SYS_QUEUESERIAL C ON C.Q_SERIALID=A.[QDDTJ_QSERIALID]
                    JOIN SYS_ORGANIZE E ON   [dbo].[isParent](B.ORG_ID,E.ORG_ID)=1 
					  JOIN SYS_STAFF D ON D.STAFF_ID=A.[QDDTJ_SNO]
                    WHERE E.ORG_ID=@0 " + GetSelectWhereTime(beginTime, endTime) + "   GROUP BY A.[QDDTJ_SNO]  ,D.STAFF_NAM   ORDER BY D.STAFF_NAM  DESC", orgId);
                Database db = new Database();
                return db.Fetch<Statistics_DAYQUEUETJChart>(sql);
        }


        #region Helper
        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        private string GetSelectWhereTime(  DateTime? beginTime, DateTime? endTime )
        {
            var str = string.Empty;
            if(beginTime!=null&&beginTime!=DateTime.MinValue)
            {
                str += string.Format(" AND QDDTJ_DAY >= '{0}'",beginTime);
            }
            if(endTime!=null&&endTime!=DateTime.MinValue)
            {
                str += string.Format(" AND QDDTJ_DAY <= '{0}'", endTime);
            }
            return str;
        }
        #endregion
    }
}
