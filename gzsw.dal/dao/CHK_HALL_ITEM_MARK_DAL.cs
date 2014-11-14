using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PetaPoco;
using gzsw.model;

namespace gzsw.dal.dao
{
    public class CHK_HALL_ITEM_MARK_DAL
    {
        public Page<dynamic> GetList(int page, int PageSize, string hallno, int? id)
        {
            var db = gzswDB.GetInstance();

            var sql = PetaPoco.Sql.Builder.Append(@"select  SEQ,
c.CHKITEM_TYP,
c.HALL_NO,
c.HALL_CHKITEM_CD,
DEDUCT,
c.MODIFY_ID,
c.MODIFY_DTIME,
REASON,
MARK_TIME,
h.HALL_NAM,
u.USER_NAM,
con.HALL_CHKITEM_NAM
 from  CHK_HALL_ITEM_MARK c,
SYS_HALL h,SYS_USER u,CHK_HALL_CHKITEM_CON con
where c.HALL_NO=h.HALL_NO
and c.MODIFY_ID=u.USER_ID
and con.HALL_CHKITEM_CD=c.HALL_CHKITEM_CD
and  c.HALL_NO='" + hallno + "'");

            var data = db.Page<dynamic>(page, PageSize, sql);
            data.ItemsPerPage = PageSize;
            return data;
        }

        public static object GetListBub(DateTime beginMo, DateTime endMo, string hallno, int type, int pageIndex, int pageSize)
        {
            var db = gzswDB.GetInstance();
            
            var sql = PetaPoco.Sql.Builder.Append(@"select  SEQ,
c.CHKITEM_TYP,
c.HALL_NO,
c.HALL_CHKITEM_CD,
DEDUCT,
c.MODIFY_ID,
c.MODIFY_DTIME,
REASON,
MARK_TIME,
h.HALL_NAM,
u.USER_NAM,
con.HALL_CHKITEM_NAM
 from  CHK_HALL_ITEM_MARK c,
SYS_HALL h,SYS_USER u,CHK_HALL_CHKITEM_CON con
where c.HALL_NO=h.HALL_NO
and c.MODIFY_ID=u.USER_ID
and con.HALL_CHKITEM_CD=c.HALL_CHKITEM_CD
and  c.HALL_NO='" + hallno + "'");
            sql.Append(@" AND MARK_TIME>=@0 and MARK_TIME<=@1 ", beginMo, endMo);
            sql.Append(@" AND c.CHKITEM_TYP =@0  ", type);
            var data = db.Page<dynamic>(pageIndex, pageSize, sql);
            data.ItemsPerPage = pageSize;
            return data;
        }

        public static object GetTotal(int beginMo, int endMo, string hallno) 
        {
            var db = gzswDB.GetInstance();
//            var sql = Sql.Builder.Append(@"SELECT SUM(S.COR_DOOR_SVR_CNT) AS DOOR_SVR_CNT_TOTAL 
//                                            ,SUM(S.OTHER_SVR_CNT) AS OTHER_SVR_CNT_TOTAL
//                                            ,SUM(S.COR_OVERTIME_SVR_CNT) AS OVERTIME_SVR_CNT_TOTAL
//                                          FROM [STAT_STAFF_SVRSTAT_M] AS S
//	                                        WHERE 1=1 ");

//            sql.Append(@" AND  S.[STAFF_ID]=@0 ", hallno);
//            sql.Append(@" AND S.STAT_MO>=@0 AND S.STAT_MO<=@1 ", beginMo, endMo);

            //return db.FirstOrDefault<dynamic>(sql);
            return null;
        }
    }
}
