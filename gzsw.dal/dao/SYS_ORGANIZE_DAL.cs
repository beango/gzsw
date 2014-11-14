 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gzsw.model;
using PetaPoco;
using Ninject;

namespace gzsw.dal.dao
{
    /// <summary>
    ///  
    /// </summary>
    /// <remark>
    ///     <para>    Creator：hcli </para>
    ///     <para>CreatedTime：2014/10/16 10:35:43</para>
    /// </remark>
   public  class SYS_ORGANIZE_DAL
    {

       public SYS_ORGANIZE GetTopForUserId(string userId, int? level= null)
       {
           if (level!=null)
           {
               var sql = PetaPoco.Sql.Builder.Append(@"   select  top 1 * from SYS_ORGANIZE where ORG_ID in ( select ORG_ID from SYS_USERORGANIZE where USER_ID =@0) and ORG_LEVEL=@1 order by ORG_LEVEL ", userId,level);
               Database db = gzswDB.GetInstance();
               return db.FirstOrDefault<SYS_ORGANIZE>(sql);
           }
           else
           {
               var sql = PetaPoco.Sql.Builder.Append(@"   select  top 1 * from SYS_ORGANIZE where ORG_ID in ( select ORG_ID from SYS_USERORGANIZE where USER_ID =@0) order by ORG_LEVEL ", userId);
               Database db = gzswDB.GetInstance();
               return db.FirstOrDefault<SYS_ORGANIZE>(sql);
           }
        
       }


       /// <summary>
       /// 获取用户组织结构
       /// </summary>
       /// <param name="userId">用户ID</param>
       /// <returns></returns>
       public IList<SYS_ORGANIZE> GetListForUserId(string userId)
       {
           var sql = PetaPoco.Sql.Builder.Append(@" select * from SYS_ORGANIZE where ORG_ID in ( select ORG_ID from SYS_USERORGANIZE where USER_ID =@0)",userId);
           Database db = gzswDB.GetInstance();
           return db.Fetch<SYS_ORGANIZE>(sql);
       }

        /// <summary>
        /// 获取用户组织结构
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="level">级别</param>
        /// <returns></returns>
        public IList<SYS_ORGANIZE> GetListForUserId(string userId, string level)
       {
           var sql = PetaPoco.Sql.Builder.Append(@" select * from SYS_ORGANIZE where ORG_ID in ( select ORG_ID from SYS_USERORGANIZE where USER_ID =@0) and ORG_LEVEL =@1", userId,level);
           Database db = gzswDB.GetInstance();
           return db.Fetch<SYS_ORGANIZE>(sql);
       }





       /// <summary>
       /// 获取用户组织结构
       /// </summary>
       /// <param name="userId">用户ID</param>
       /// <param name="orgId">组织机构ID</param>
       /// <returns></returns>
       public IList<SYS_ORGANIZE> GetListForParent(string userId,string orgId)
       {
           var sql = PetaPoco.Sql.Builder.Append(@" SELECT * FROM SYS_ORGANIZE where (dbo.isParent(ORG_ID,@0)=1)
                 and ORG_ID in ( select ORG_ID from SYS_USERORGANIZE where USER_ID =@1)", orgId,userId);
           Database db = gzswDB.GetInstance();
           return db.Fetch<SYS_ORGANIZE>(sql);
       }

       /// <summary>
       /// 删除组织及其下级机构
       /// </summary>
       /// <param name="orgid"></param>
       public void DeleteAndChildren(string orgid)
       {
           var sql = PetaPoco.Sql.Builder.Append(@"WITH ORGInfo AS(
                  SELECT ORG_ID FROM dbo.SYS_ORGANIZE WHERE ORG_ID = @0
                  UNION ALL
                  SELECT a.ORG_ID FROM SYS_ORGANIZE AS a,ORGInfo AS b WHERE a.PAR_ORG_ID = b.ORG_ID
                )
                Delete SYS_ORGANIZE WHERE ORG_ID IN(SELECT ORG_ID from ORGInfo);
                Delete SYS_HALL where ORG_ID=@0;"
               , orgid);
           Database db = gzswDB.GetInstance();
           db.Execute(sql);
       }

       [Inject]
       public IDao<SYS_ORGANIZE> DaoOrganize { get; set; }
       [Inject]
       public IDao<SYS_HALL> DaoHall { get; set; }

       public string Add(SYS_ORGANIZE org, SYS_HALL hal)
       {
           Database db = gzswDB.GetInstance();
           db.BeginTransaction();

           db.Insert(hal);
           var prikey = db.Insert(org);

           db.CompleteTransaction();
           return prikey.ToString();
       }
    }
}
