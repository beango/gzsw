using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PetaPoco;
using gzsw.model.ext;
using gzsw.model;

namespace gzsw.dal.dao
{
     public class WARN_COMPLAIN_DETAIL_DAL
    {

         public Page<dynamic> GetList(int page, int PageSize, DateTime? stime, DateTime? etime, string nameorcode,int? status)
         {
             var db = gzswDB.GetInstance();

             var sql = PetaPoco.Sql.Builder.Append(@"select w.SEQ, w.NSR_SBM,n.NSR_NAME,h.HALL_NAM,
w.COMPLAIN_PRO,w.COMPLAIN_NAM,w.COMPLAIN_TIME,
w.[STATE]
 from WARN_COMPLAIN_DETAIL w
left join SYS_NSRINFO n on  w.NSR_SBM=n.NSR_SBM
left join SYS_HALL h on  w.HALL_NO = h.HALL_NO");

             if (stime != null)
             {
                 sql.Append("where w.COMPLAIN_TIME >=@0 ", stime.Value.ToString("yyyy-MM-dd HH:mm:ss"));
             }
             if (etime != null)
             {
                 sql.Append("where w.COMPLAIN_TIME <=@0 ", etime.Value.ToString("yyyy-MM-dd HH:mm:ss"));
             }
             if (!string.IsNullOrEmpty(nameorcode))
                 sql.Append("where w.NSR_SBM like @0 or  or n.NSR_NAME like @0", "%" + nameorcode + "%", nameorcode);
             if (status != null)
             {
                 sql.Append("where w.[STATE]=@0", status.Value);
             }
             sql.Append(" order by w.COMPLAIN_TIME desc ");
             var data = db.Page<dynamic>(page, PageSize, sql);
             data.ItemsPerPage = PageSize;
             return data;
         }

         public static object GetListBubByHall(DateTime beginMo, DateTime endMo, string hallno, int pageIndex, int pageSize)
         {
             var db = gzswDB.GetInstance();

             var sql = PetaPoco.Sql.Builder.Append(@"select w.SEQ, w.NSR_SBM,n.NSR_NAME,h.HALL_NAM,
w.COMPLAIN_PRO,w.COMPLAIN_NAM,w.COMPLAIN_TIME,
w.[STATE],w.STAFF_ID,w.HALL_NO
 from WARN_COMPLAIN_DETAIL w
left join SYS_NSRINFO n on  w.NSR_SBM=n.NSR_SBM
left join SYS_HALL h on  w.HALL_NO = h.HALL_NO");


             sql.Append("where w.COMPLAIN_TIME >=@0 and w.COMPLAIN_TIME <=@1", beginMo,endMo);


             sql.Append("where  w.HALL_NO=@0", hallno);

             var data = db.Page<dynamic>(pageIndex, pageSize, sql);
             data.ItemsPerPage = pageSize;
             return data;
         }

         public object GetListByHall(string hallno, int pageIndex, int pageSize)
         {
             var db = gzswDB.GetInstance();

             var sql = PetaPoco.Sql.Builder.Append(@"select w.SEQ, w.NSR_SBM,n.NSR_NAME,h.HALL_NAM,
w.COMPLAIN_PRO,w.COMPLAIN_NAM,w.COMPLAIN_TIME,
w.[STATE],w.STAFF_ID,w.HALL_NO
 from WARN_COMPLAIN_DETAIL w
left join SYS_NSRINFO n on  w.NSR_SBM=n.NSR_SBM
left join SYS_HALL h on  w.HALL_NO = h.HALL_NO");


             sql.Append("where w.COMPLAIN_TIME >=@0 ", DateTime.Now.ToShortDateString());


             sql.Append("where  w.HALL_NO=@0", hallno);

             sql.Append(" order  by COMPLAIN_TIME desc", hallno);

             var data = db.Page<dynamic>(pageIndex, pageSize, sql);
             data.ItemsPerPage = pageSize;
             return data;
         }
    }
}
