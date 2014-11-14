using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PetaPoco;

namespace gzsw.model.Subclasses
{
    public class CHK_STAFF_ATTEND_DEDUCT_CON_SUB : CHK_STAFF_ATTEND_DEDUCT_CON
    {
        [ResultColumn]
        public string ORG_NAM { get; set; }
    }
}
