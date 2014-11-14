using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using PetaPoco;

namespace gzsw.model
{
    public partial class CHK_STAFF_USU_ACT_MARK
    {
        [ResultColumn]
        public string STAFF_NAM { get; set; }

        [ResultColumn]
        public string ORG_ID { get; set; }

        [ResultColumn]
        public string ORG_NAM { get; set; }

        /// <summary>
        /// 修改人
        /// </summary>
        [ResultColumn]
        public string MODIFY_NAM { get; set; }
    }
}
