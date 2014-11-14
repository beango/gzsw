using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PetaPoco;

namespace gzsw.model
{
    public partial class CHK_STAFF_COMPRE_SAN_MARK
    {
        [ResultColumn]
        public string STAFF_NAM { get; set; }

        [ResultColumn]
        public string ORG_ID { get; set; }

        [ResultColumn]
        public string ORG_NAM { get; set; }
    }
}
