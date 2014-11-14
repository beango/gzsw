using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PetaPoco;

namespace gzsw.model.Subclasses
{
    public class CHK_STAFF_COMPRE_EVAL_M_SUB : CHK_STAFF_COMPRE_EVAL_M
    {
        [ResultColumn]
        public string STAFF_NAM { get; set; }

    }
}
