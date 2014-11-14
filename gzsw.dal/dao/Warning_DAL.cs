using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gzsw.model.dto;
using PetaPoco;
using gzsw.model;

namespace gzsw.dal.dao
{
    /// <summary>
    /// 监控预警
    /// </summary>
    public static class Warning_DAL
    {
        /// <summary>
        /// 获取监控预警信息
        /// </summary>
        /// <param name="hallNo"></param>
        /// <returns></returns>
        public static List<Virtual_Mon_Dto> GetVirtualMon(string hallNo)
        {
            var db = gzswDB.GetInstance();

            var sql = Sql.Builder.Append(@" EXECUTE PRO_GET_VIRTUAL_MON @@HALL_NO=@0 ", hallNo);

            return db.Query<Virtual_Mon_Dto>(sql).ToList();
        }
    }
}
