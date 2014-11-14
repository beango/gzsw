using gzsw.model;
using PetaPoco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace gzsw.dal.dao
{
    public class SYS_MENU_DAL
    {
        public void DeleteAndChildren(int funid)
        {
            var sql = PetaPoco.Sql.Builder.Append(@"WITH MENUInfo AS(
                              SELECT MENU_ID FROM dbo.SYS_MENU WHERE MENU_ID = @0
                              UNION ALL
                              SELECT a.MENU_ID FROM SYS_MENU AS a,MENUInfo AS b WHERE a.PAR_MENU_ID = b.MENU_ID
                           )
                           delete FROM SYS_MENU WHERE MENU_ID IN(SELECT MENU_ID from MENUInfo)"
                , funid);
            Database db = gzswDB.GetInstance();
            db.Execute(sql);
        }
    }
}
