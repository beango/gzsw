using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PetaPoco;

namespace gzsw.model
{
    public partial class CHK_TIMESCORE_PARAM
    {
        [ResultColumn]
        public string HALL_NO { get; set; }

        [ResultColumn]
        public string HALL_NAM { get; set; }
    }
}
