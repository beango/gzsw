using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using gzsw.model;
using gzsw.util;
using PetaPoco;

namespace gzsw.dal
{
    public class SYS_USER_DAL
    {
        public List<UserFuncs> GetUserFunc(string userid)
        {
            var db = new Database();

            var sql = PetaPoco.Sql.Builder.Append(@"
                      select t4.FUNCTION_ID,t4.FUNCTION_NAM,t4.FUNCTION_COD,t4.PAR_FUNCTION_ID,t3.FUNCTION_TYP 
                      from SYS_USEROLE t2
                      join SYS_ROLEFUNCTION t3 on t2.ROLE_ID=t3.ROLE_ID
                      join SYS_FUNCTION t4 on t4.FUNCTION_ID=t3.FUNCTION_ID ");
            sql.Append("where t2.USER_ID=@0 ", userid);
            return db.Fetch<UserFuncs>(sql);
        }

        public List<UserOrgs> GetUserORG(string userid)
        {
            var db = new Database();
            var sql = PetaPoco.Sql.Builder.Append("");
            if (userid=="admin")
            {
                sql.Append(@"select t2.ORG_ID,t2.ORG_NAM,t2.PAR_ORG_ID,t2.ORG_LEVEL
                        from SYS_ORGANIZE t2");
            }
            else
            {
                sql.Append(@"select t2.ORG_ID,t2.ORG_NAM,t2.PAR_ORG_ID,t2.ORG_LEVEL
                        from SYS_USERORGANIZE t1 join SYS_ORGANIZE t2 on t1.ORG_ID=t2.ORG_ID");
                sql.Append("where t1.USER_ID=@0 ", userid);
            }
            
            return db.Fetch<UserOrgs>(sql);
        }

        /// <summary>
        ///  删除用户
        /// </summary>
        /// <param name="userIds"></param>
        public bool DeleteUser(params string[] userIds)
        {
            var ids = userIds.Aggregate(string.Empty, (current, i) => current + ("'" + i.GetSqlCheckStr() + "',"));
            ids = ids.TrimEnd(',');
            try
            {

                var db = new Database();
                db.BeginTransaction();
                db.Execute(string.Format("delete SYS_USER where USER_ID in ({0});",ids));
                db.Execute(string.Format("delete SYS_USEROLE where USER_ID in ({0});", ids));
                db.Execute(string.Format("delete SYS_USERORGANIZE where USER_ID in ({0});", ids)); 
                db.CompleteTransaction();
                return true;
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("AddStaffBusi", ex);
                return false;
            }
        }
    }
}
