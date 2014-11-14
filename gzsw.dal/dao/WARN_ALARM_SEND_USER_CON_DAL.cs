using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PetaPoco;
using gzsw.model;

namespace gzsw.dal.dao
{
    public class WARN_ALARM_SEND_USER_CON_DAL
    {

        public Page<dynamic> GetList(int page, int PageSize, string hallno)
        {
            var db = gzswDB.GetInstance();

            var sql = PetaPoco.Sql.Builder.Append(@"select w.seq,w.hall_no,h.HALL_NAM,w.alarm_typ,w.mob_nbr,w.user_id,u.USER_NAM from WARN_ALARM_SEND_USER_CON w
 join SYS_HALL h on  w.HALL_NO = h.HALL_NO
 left join  SYS_USER u on w.USER_ID=u.USER_ID");
            if (!string.IsNullOrEmpty(hallno))
                sql.Append("where h.HALL_NO like '"+hallno+"' ");
            
            var data = db.Page<dynamic>(page, PageSize, sql);
            data.ItemsPerPage = PageSize;
            return data;
        } 
    }
}
