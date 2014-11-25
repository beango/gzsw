using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PetaPoco;
using gzsw.model;

namespace gzsw.dal.dao
{
    public class CHK_HALL_CHKITEM_CON_DAL
    {
        public Page<dynamic> GetList(string name, int page, int PageSize)
        {
            var db = gzswDB.GetInstance();

            var sql = PetaPoco.Sql.Builder.Append(@" select * from CHK_HALL_CHKITEM_CON");
            if (!string.IsNullOrEmpty(name))
                sql.Append("where HALL_CHKITEM_CD  =@0 ", name);
            var data = db.Page<dynamic>(page, PageSize, sql);
            data.ItemsPerPage = PageSize;
            return data;
        }
    }
}
