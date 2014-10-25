using PetaPoco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace gzsw.model
{
    public partial class SYS_TICKETQUEUESERIAL
    {
        [Ignore]
        public SYS_HALL Hall { get; set; }

        [Ignore]
        public SYS_ORGANIZE Org { get; set; }
    }
}
