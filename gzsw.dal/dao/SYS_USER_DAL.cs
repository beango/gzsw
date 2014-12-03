using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using gzsw.model;
using gzsw.util;
using PetaPoco;
using gzsw.util.Cache;

namespace gzsw.dal
{
    public class SYS_USER_DAL
    {
        /// <summary>
        /// 获取用户权限
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        /// [InterceptCache(TimeOut = 2)]
        public virtual List<UserFuncs> GetUserFunc(string userid)
        {
            var db = gzswDB.GetInstance();

            var sql = PetaPoco.Sql.Builder.Append(@"
                      select t4.FUNCTION_ID,t4.FUNCTION_NAM,t4.FUNCTION_COD,t4.PAR_FUNCTION_ID,t3.FUNCTION_TYP 
                      from SYS_USEROLE t2
                      join SYS_ROLEFUNCTION t3 on t2.ROLE_ID=t3.ROLE_ID
                      join SYS_FUNCTION t4 on t4.FUNCTION_ID=t3.FUNCTION_ID");
            sql.Append("where t2.USER_ID=@0 ", userid);
            return db.Fetch<UserFuncs>(sql);
        }

        /// <summary>
        /// 获取用户有权限访问的组织机构
        /// </summary>
        /// [InterceptCache(TimeOut = 1)]
        public virtual List<SYS_ORGANIZE> GetUserORG(string userid)
        {
            var db = gzswDB.GetInstance();
            var sql = PetaPoco.Sql.Builder.Append("");
            if (userid=="admin")
            {
                sql.Append(@"select t2.*
                        from SYS_ORGANIZE t2");
            }
            else
            {
                sql.Append(@"select t2.*
                        from SYS_USERORGANIZE t1 join SYS_ORGANIZE t2 on t1.ORG_ID=t2.ORG_ID");
                sql.Append("where t1.USER_ID=@0 ", userid);
            }
            
            return db.Fetch<SYS_ORGANIZE>(sql);
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

                var db = gzswDB.GetInstance();
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

        /// <summary>
        /// 获取组织机构下的用户
        /// </summary>
        public List<SYS_USER> GetORGUser(string orgid)
        {
            var db = gzswDB.GetInstance();
            var sql = PetaPoco.Sql.Builder.Append("");
            sql.Append(@"select t2.*
                        from SYS_USERORGANIZE t1 join SYS_USER t2 on t1.USER_ID=t2.USER_ID");
            sql.Append("where t1.ORG_ID=@0 ", orgid);

            return db.Fetch<SYS_USER>(sql);
        }


        public List<SYS_USER> GetUserOByHall(string Hall_No)
        {
            var db = gzswDB.GetInstance();

            var sql = PetaPoco.Sql.Builder.Append(@" select HALL_NO from SYS_HALL ");
            if (!string.IsNullOrEmpty(Hall_No))
            {
                sql = PetaPoco.Sql.Builder.Append(@" select HALL_NO from SYS_HALL where HALL_NO ='" + Hall_No + "'");
                //sql.Append("where HALL_NO =@0 ", Hall_No, Hall_No);
            }

            sql = PetaPoco.Sql.Builder.Append(@"select [USER_ID],
USER_NAM,
USER_PASSWORD,
TEL,
EMAIL,
NOTE,
CREATE_ID,
CREATE_DTIME,
MODIFY_ID,
MODIFY_DTIME from SYS_USER where USER_ID in(
 select [USER_ID] from SYS_USERORGANIZE where ORG_ID in(" + sql.SQL + "))");


            return db.Fetch<SYS_USER>(sql);
        }

        /// <summary>
        /// 1:省级，2：市级，3：区级，4：服务厅级
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public int GetHighLV(string userid)
        {
            var sql = Sql.Builder.Append(@"select TOP 1 C.ORG_LEVEL from 
                SYS_USER A JOIN 
                SYS_USERORGANIZE B ON A.USER_ID=B.USER_ID
                JOIN SYS_ORGANIZE C ON C.ORG_ID=B.ORG_ID
                WHERE A.USER_ID=@0
                GROUP BY C.ORG_ID,C.ORG_LEVEL
                ORDER BY C.ORG_LEVEL ASC
                ", userid);
            var userLV = gzswDB.GetInstance().ExecuteScalar<int>(sql);
            return userLV;
        }
    }
}
