using gzsw.model;
using PetaPoco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace gzsw.dal.dao
{
    public class WARN_RELEASE_DAL
    {
        public Page<dynamic> GetList(int page, int PageSize,int? mt,int? id, string title)
        {
            var db = gzswDB.GetInstance();

            var sql = PetaPoco.Sql.Builder.Append(@"
                SELECT [STAFF_DETAIL_ID],[RELEASE_USER_ID],[RELEASE_TIME],[RELEASE_MESSAGE],t3.HALL_NO,t1.[STAFF_ID]
                      ,CLI_READ_IND,[BEGIN_TIME],[END_TIME],[TITLE], 1 as MT, t2.STAFF_NAM as ObjNam
                  FROM [dbo].[WARN_RELEASE_STAFF_DETAIL] t1 join [dbo].[SYS_STAFF] t2 on t1.STAFF_ID=t2.STAFF_ID
                   join SYS_HALL t3 on t2.ORG_ID=t3.ORG_ID
                union all
                SELECT [COUNTER_DETAIL_ID] as [STAFF_DETAIL_ID],[RELEASE_USER_ID],[RELEASE_TIME],[RELEASE_MESSAGE]
                ,t1.[HALL_NO],CAST(t1.[COUNTER_ID] as varchar) as [STAFF_ID],null as CLI_READ_IND,[BEGIN_TIME],[END_TIME],[TITLE], 2 as MT,CAST(t2.COUNTER_ID as nvarchar) as ObjNam
                  FROM [dbo].[WARN_RELEASE_COUNTER_DETAIL] t1 join [dbo].[SYS_COUNTER] t2 on t1.HALL_NO=t2.HALL_NO
                  and t1.COUNTER_ID=t2.COUNTER_ID
                union all
                SELECT [TABLE_DETAIL_ID] as [STAFF_DETAIL_ID],[RELEASE_USER_ID],[RELEASE_TIME],[RELEASE_MESSAGE]
                ,t1.[HALL_NO],t1.[TABLE_CD]  as [STAFF_ID],null as CLI_READ_IND,[BEGIN_TIME],[END_TIME],[TITLE], 3 as MT,t2.TABLE_CD as ObjNam
                  FROM [dbo].[WARN_RELEASE_TABLE_DETAIL] t1 join TR_RELEASE_TABLE_INFO t2 on t1.HALL_NO=t2.HALL_NO
                  and t1.TABLE_CD=t2.TABLE_CD");


            sql = PetaPoco.Sql.Builder.Append("select * from (" + sql.SQL + ")t");
            if (!string.IsNullOrEmpty(title))
                sql.Append("where TITLE like @0 ", "%" + title + "%", title);
            if (id!=null)
            {
                sql.Append("where STAFF_DETAIL_ID=@0 and mt=@1", id,mt);
            }
            var data = db.Page<dynamic>(page, PageSize, sql);
            data.ItemsPerPage = PageSize;
            return data;
        }
    }
}
