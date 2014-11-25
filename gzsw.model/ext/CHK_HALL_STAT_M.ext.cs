using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PetaPoco;

namespace gzsw.model.ext
{
    public partial class CHK_HALL_STAT_M
    {
        [ResultColumn]
        public SYS_ORGANIZE HALL_NAM { get; set; }
    }
}
