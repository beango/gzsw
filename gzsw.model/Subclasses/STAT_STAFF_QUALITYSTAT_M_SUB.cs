using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PetaPoco;

namespace gzsw.model.Subclasses
{
    public class STAT_STAFF_QUALITYSTAT_M_SUB : STAT_STAFF_QUALITYSTAT_M
    {
        [ResultColumn]
        public string STAFF_NAM { get; set; }

        [ResultColumn]
        public string QUALITY_NAM { get; set; }
    }
}
