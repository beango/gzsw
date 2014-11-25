using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gzsw.model;
using PetaPoco;

namespace gzsw.dal.dao
{
    public class Stored_DAL
    {
        // ReSharper disable once CSharpWarnings::CS1998
        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="stat">时间</param>
        /// <param name="objNo"></param>
        /// <param name="userId">用户ID</param>
        /// <param name="type">0 objNo就是服务厅编号，1就是员工编号</param>
        public static void UpdateData(int stat, string objNo, string userId, int type = 0)
        {
            var db = gzswDB.GetInstance();
            try
            {
                if (type == 1)
                {
                    var sql2 = Sql.Builder.Append(@" FROM SYS_STAFF WHERE STAFF_ID=@0 ", objNo);
                    var item = db.FirstOrDefault<SYS_STAFF>(sql2);
                    if (item != null)
                        objNo = item.ORG_ID;
                }

                var sql = Sql.Builder.Append(@" EXECUTE PRO_UPDATE_CHK_STAFF_COMPRE_EVAL_M  @@STAT_MO=@0, @@HALL_NO=@1 , @@USER_ID=@2 ", stat, objNo, userId);

                db.Execute(sql);

            }
            // ReSharper disable once EmptyGeneralCatchClause
            catch (Exception)
            {

            }
        }

        // ReSharper disable once CSharpWarnings::CS1998
      /// <summary>
      /// 
      /// </summary>
      /// <param name="stat">年月</param>
        /// <param name="hallno">服务厅编码</param>
      /// <param name="userId">当前操作人id</param>
        public static void UpdateDataByHall(int stat, string hallno, string userId)
        {


 //           @STAT_MO   int  ,  --要更新的月份,格式为 201411 ，201409  格式
 //@HALL_NO   varchar(128),--要更新的服务厅编码
 //@USER_ID   varchar(50)   --当前操作的用户登录名
            var db = gzswDB.GetInstance();
            try
            {


                var sql = Sql.Builder.Append(@" EXECUTE PRO_UPDATE_CHK_HALL_STAT_M  @@STAT_MO=@0, @@HALL_NO=@1 , @@USER_ID=@2 ", stat, hallno, userId);

                db.Execute(sql);

            }
            // ReSharper disable once EmptyGeneralCatchClause
            catch (Exception)
            {

            }
        }
    }
}
