using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PetaPoco;

namespace gzsw.model
{
    public partial class CHK_STAFF_SYSTEM_CON
    {
        [ResultColumn]
        public string ORG_NAM { get; set; }
    }
}
