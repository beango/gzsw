using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PetaPoco;

namespace gzsw.model
{
    public partial class CHK_DETAIL_SVR_COEF_CON
    {
        [ResultColumn]
        public string SERIALNAME { get; set; }

        [ResultColumn]
        public string ORG_NAM { get; set; }

        [ResultColumn]
        public string USER_NAM { get; set; }
    }
}
