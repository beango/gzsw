using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using gzsw.model;
using PetaPoco;

namespace gzsw.dal.dao
{
    /// <summary>
    /// 服务厅
    /// </summary>
    public class SYS_HALL_DAL
    {

        public List<SYS_HALL> GetList(string userId,string level)
        {
            var db = gzswDB.GetInstance();

            var sql = Sql.Builder.Append(@"  select * from SYS_HALL where ORG_ID in (select ORG_ID from SYS_ORGANIZE where ORG_ID in (
 select ORG_ID from SYS_USERORGANIZE where USER_ID =@0) and ORG_LEVEL =@1)", userId, level);
            return db.Fetch<SYS_HALL>(sql);

           
        }

        /// <summary>
        /// 获取所有的服务厅
        /// </summary>
        /// <returns></returns>
        public List<SYS_HALL> GetList()
        {
            var db = gzswDB.GetInstance();

            var sql = Sql.Builder.Append(@"FROM [SYS_HALL] ");

            return db.Fetch<SYS_HALL>(sql);
        }

        /// <summary>
        /// 获取服务厅信息
        /// </summary>
        /// <param name="hallNo">服务厅编号</param>
        /// <returns></returns>
        public SYS_HALL GetHall(string hallNo)
        {
            var db = gzswDB.GetInstance();

            var sql = Sql.Builder.Append(@"FROM [SYS_HALL] WHERE [HALL_NO]=@0", hallNo);
            return db.FirstOrDefault<SYS_HALL>(sql);
        }

        public void UpdatePictUrl(string hallNo, string pictUrl)
        {
            var item = GetHall(hallNo);
            item.HALL_PICT_URL = pictUrl;

            var db = gzswDB.GetInstance();
            db.Update(item);
        }

        public List<string> GetOrgAndChild(string orgid)
        {
            var db = gzswDB.GetInstance();
            var sql = Sql.Builder.Append(@";WITH ORGInfo AS(
                  SELECT ORG_ID FROM dbo.SYS_ORGANIZE WHERE ORG_ID = @0
                  UNION ALL
                  SELECT a.ORG_ID FROM SYS_ORGANIZE AS a,ORGInfo AS b WHERE a.PAR_ORG_ID = b.ORG_ID
                )", orgid)
                  .Append("select ORG_ID from ORGInfo");
            return db.Fetch<string>(sql);
        }

        /// <summary>
        /// 获取所有服务厅
        /// </summary>
        /// <param name="orgid"></param>
        /// <returns></returns>
        public List<string> GetOrgHallAndChild(string orgid)
        {
            var db = gzswDB.GetInstance();
            var sql = Sql.Builder.Append(@";WITH ORGInfo AS(
                  SELECT ORG_ID FROM dbo.SYS_ORGANIZE WHERE ((PAR_ORG_ID = @0) or (ORG_LEVEL=4 and ORG_ID = @0))
                  UNION ALL
                  SELECT a.ORG_ID FROM SYS_ORGANIZE AS a,ORGInfo AS b WHERE a.PAR_ORG_ID = b.ORG_ID
                )", orgid)
                  .Append(@"select t2.HALL_NO from ORGInfo t1 join SYS_HALL t2 on t1.ORG_ID=t2.ORG_ID");
            return db.Fetch<string>(sql);
        }

        public static List<SYS_HALL> GetListByUserId(string userId)
        {
            var db = gzswDB.GetInstance();
            var sql = Sql.Builder.Append(@" SELECT H.[HALL_NO]
                                              ,H.[HALL_NAM]
                                              ,H.[ADDRESS]
                                              ,H.[LONGITUDE]
                                              ,H.[DIMENSION]
                                              ,H.[COUNTER_CNT]
                                              ,H.[AUTO_CALL_SEC]
                                              ,H.[AUTO_CALL_IND]
                                              ,H.[AUTO_END_SEC]
                                              ,H.[AUTO_EVAL_SEC]
                                              ,H.[EVAL_STAY_SEC]
                                              ,H.[HAV_QUEUE_IND]
                                              ,H.[CREATE_ID]
                                              ,H.[CREATE_DTIME]
                                              ,H.[MODIFY_ID]
                                              ,H.[MODIFY_DTIME]
                                              ,H.[HEAD]
                                              ,H.[HEAD_TEL]
                                              ,H.[NOTE]
                                              ,H.[ORG_ID]
                                              ,H.[HALL_PICT_URL]
                                              ,H.[TICKETNUM]
                                              ,H.[LIMIT_CNT]
                                              ,H.[CHK_DETAIN_MIN]
                                          FROM [SYS_HALL] AS H
                                          JOIN SYS_USERORGANIZE AS UR
                                          ON H.ORG_ID=UR.ORG_ID 
                                          WHERE 1=1      ");
            sql.Append(@" AND UR.[USER_ID] =@0 ",userId);
            return db.Fetch<SYS_HALL>(sql);
        }
    }
}
