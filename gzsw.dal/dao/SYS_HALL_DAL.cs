using System;
using System.Collections.Generic;
using System.Linq;
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
        /// <summary>
        /// 获取所有的服务厅
        /// </summary>
        /// <returns></returns>
        public List<SYS_HALL> GetList()
        {
            var db = new Database();

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
            var db = new Database();

            var sql = Sql.Builder.Append(@"FROM [SYS_HALL] WHERE [HALL_NO]=@0",hallNo);
            return db.FirstOrDefault<SYS_HALL>(sql);
        }

        public void UpdatePictUrl(string hallNo, string pictUrl)
        {
            var item = GetHall(hallNo);
            item.HALL_PICT_URL = pictUrl;

            var db = new Database();
            db.Update(item);
        }

        public List<string> GetOrgAndChild(string orgid)
        {
            var db = new Database();
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
            var db = new Database();
            var sql = Sql.Builder.Append(@";WITH ORGInfo AS(
                  SELECT ORG_ID FROM dbo.SYS_ORGANIZE WHERE ((PAR_ORG_ID = @0) or (ORG_LEVEL=4 and ORG_ID = @0))
                  UNION ALL
                  SELECT a.ORG_ID FROM SYS_ORGANIZE AS a,ORGInfo AS b WHERE a.PAR_ORG_ID = b.ORG_ID
                )", orgid)
                  .Append(@"select t2.HALL_NO from ORGInfo t1 join SYS_HALL t2 on t1.ORG_ID=t2.ORG_ID");
            return db.Fetch<string>(sql);
        }
    }
}
