using gzsw.model;
using PetaPoco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace gzsw.dal.dao
{
    public class SYS_FUNCTION_DAL
    {
        public void DeleteAndChildren(int funid)
        {
            var sql = PetaPoco.Sql.Builder.Append(@"WITH FUNCInfo AS(
                              SELECT FUNCTION_ID FROM dbo.SYS_FUNCTION WHERE FUNCTION_ID = @0
                              UNION ALL
                              SELECT a.FUNCTION_ID FROM SYS_FUNCTION AS a,FUNCInfo AS b WHERE a.PAR_FUNCTION_ID = b.FUNCTION_ID
                           )
                           delete FROM SYS_FUNCTION WHERE FUNCTION_ID IN(SELECT FUNCTION_ID from FUNCInfo)"
                ,funid);
            Database db = gzswDB.GetInstance();
            db.Execute(sql);
        }
    }
}
