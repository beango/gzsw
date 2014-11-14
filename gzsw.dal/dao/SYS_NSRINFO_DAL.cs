using gzsw.model;
using PetaPoco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace gzsw.dal.dao
{
    public class SYS_NSRINFO_DAL
    {
        public void IMPORT_NSR(Guid batchid)
        {
            var db = gzswDB.GetInstance();
            db.Execute("exec IMPORT_NSR @0", batchid);
        }
    }
}
