using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PetaPoco;
using gzsw.model;

namespace gzsw.dal.dao
{
    public class WARN_ALARM_INFO_DETAIL_DAL
    {
        public Page<dynamic> GetList(int page, int PageSize, string hallno, DateTime? strat, DateTime? end, int? ALARM_TYP, int? STATE)
        {
            var db = gzswDB.GetInstance();

            var sql = PetaPoco.Sql.Builder.Append(@"SELECT  W.ALARM_SEQ,
W.STAFF_ID,
S.STAFF_NAM,
W.COUNTER_ID,
W.ALARM_TYP,
W.CREATE_DTIME,
W.HALL_NO,
H.HALL_NAM,
W.[STATE],
W.HANDLE_USER,
W.HANDLE_TIME,
W.ALARM_REASON,
W.ALARM_METHOD FROM WARN_ALARM_INFO_DETAIL W
JOIN SYS_HALL H ON W.HALL_NO=H.HALL_NO
LEFT JOIN SYS_STAFF  S ON W.STAFF_ID=S.STAFF_ID");
            if (!string.IsNullOrEmpty(hallno))
                sql.Append("where h.HALL_NO =@0 ", "%" + hallno + "%", hallno);
            if (strat != null)
            {
                sql.Append("where W.CREATE_DTIME >=@0 ", strat.Value.ToString("yyyy-MM-dd HH:mm:ss"));
            }
            if (end != null)
            {
                sql.Append("where W.CREATE_DTIME <=@0 ", end.Value.ToString("yyyy-MM-dd HH:mm:ss"));
            }
            if (STATE != null)
            {
                sql.Append("where w.[STATE]=@0", STATE.Value);
            }
            if (ALARM_TYP != null)
            {
                sql.Append("where w.[ALARM_TYP]=@0", ALARM_TYP.Value);
            }
            sql.Append("  order by W.CREATE_DTIME desc");
            var data = db.Page<dynamic>(page, PageSize, sql);
            data.ItemsPerPage = PageSize;
            return data;
        }

        public Page<dynamic> GetSendInfoList(string name, int page, int PageSize)
        {
            var db = gzswDB.GetInstance();

            var sql = PetaPoco.Sql.Builder.Append(@"select w.SENDINFO_DETAIL_ID,
w.ALARM_SEQ,
w.MOB_NBR,
w.USER_ID,
u.USER_NAM,
w.ALARM_INFO,
w.SEND_TIME,
w.MOB_SEND_STATE,
w.CLI_SEND_STATE,
w.CLI_READ_IND,
w.SMS_SEND_TIME,
w.HALL_NO,
h.HALL_NAM
from [WARN_ALARM_SENDINFO_DETAIL] w
join SYS_HALL h  on w.hall_NO=h.hall_no
left join SYS_USER u on w.user_id = u.user_id");
            if (!string.IsNullOrEmpty(name))
                sql.Append("where u.USER_NAM  like @0 ", "%" + name + "%", name);
            var data = db.Page<dynamic>(page, PageSize, sql);
            data.ItemsPerPage = PageSize;
            return data;
        }

        public Page<dynamic> GetListByHall(string hallno, int page, int PageSize)
        {
            var db = gzswDB.GetInstance();

            var sql = PetaPoco.Sql.Builder.Append(@"select w.SENDINFO_DETAIL_ID,
w.ALARM_SEQ,
w.MOB_NBR,
w.USER_ID,
u.USER_NAM,
w.ALARM_INFO,
w.SEND_TIME,
w.MOB_SEND_STATE,
w.CLI_SEND_STATE,
w.CLI_READ_IND,
w.SMS_SEND_TIME,
w.HALL_NO,
h.HALL_NAM
from [WARN_ALARM_SENDINFO_DETAIL] w
join SYS_HALL h  on w.hall_NO=h.hall_no
left join SYS_USER u on w.user_id = u.user_id");
            sql.Append("where w.HALL_NO =@0 ", hallno);
            sql.Append(" and  w.SEND_TIME >=@0 ",DateTime.Now.ToShortDateString());
            var data = db.Page<dynamic>(page, PageSize, sql);
            data.ItemsPerPage = PageSize;
            return data;
        }

        public List<WARN_ALARM_SENDINFO_DETAIL> GetALARMSendInfo(string userid)
        {
            var db = gzswDB.GetInstance();

            var sql = PetaPoco.Sql.Builder.Append(@"select t2.* from WARN_ALARM_INFO_DETAIL t1 join WARN_ALARM_SENDINFO_DETAIL t2 on t1.ALARM_SEQ=t2.ALARM_SEQ
              where t2.CLI_READ_IND=0 and t2.CLI_SEND_STATE=0 and t2.USER_ID=@0 and t1.STATE=1"
                ,userid);
            return db.Fetch<WARN_ALARM_SENDINFO_DETAIL>(sql);
        }
    }
}
