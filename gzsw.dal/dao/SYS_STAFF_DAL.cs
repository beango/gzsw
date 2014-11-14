using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gzsw.model;
using gzsw.util;
using PetaPoco;

namespace gzsw.dal.dao
{
    public class SYS_STAFF_DAL
    {
        public bool AddStaffBusi(List<SYS_STAFFBUSI> busilist,string staff_id )
        {
            try
            {
                var db = gzswDB.GetInstance();
                db.BeginTransaction();
                db.Execute("delete SYS_STAFFBUSI where STAFF_ID=@0", staff_id);
                foreach (var sysStaffbusi in busilist)
                {
                    db.Insert(sysStaffbusi);
                }
                db.CompleteTransaction();
                return true;
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("AddStaffBusi",ex);
                return false;
            }
        }

        /// <summary>
        /// 通过服务厅编码，获取员工信息
        /// </summary>
        /// <param name="hallNo">服务厅编码</param>
        /// <returns></returns>
        public List<SYS_STAFF> GetListByHallNo(string hallNo,int? staffType=1)
        {
            var db = gzswDB.GetInstance();

            var sql = Sql.Builder.Append(@"SELECT ST.[STAFF_ID]
                                              ,ST.[STAFF_NAM]
                                              ,ST.[STAFF_PASSWORD]
                                              ,ST.[STAR_LEVEL]
                                              ,ST.[PHOTE_URL]
                                              ,ST.[STAFF_TYP]
                                              ,ST.[STAR_EVAL_TYP]
                                              ,ST.[STAR_EVAL_NOTE]
                                              ,ST.[NOTE]
                                              ,ST.[CREATE_ID]
                                              ,ST.[CREATE_DTIME]
                                              ,ST.[MODIFY_ID]
                                              ,ST.[MODIFY_DTIME]
                                              ,ST.[ORG_ID]
                                        FROM SYS_STAFF AS ST 
                                        JOIN SYS_HALL AS HA
                                        ON ST.ORG_ID=HA.ORG_ID
                                        WHERE HA.HALL_NO=@0  ", hallNo);

            if (staffType != null)
            {
                sql.Append(@"  AND ST.STAFF_TYP=@0", staffType);
            }

            return db.Fetch<SYS_STAFF>(sql);
        }
    }
}
