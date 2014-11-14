using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using PetaPoco;

namespace gzsw.model
{
    public partial class CHK_STAFF_APPLYDETAIL
    {
        [ResultColumn]
        public string STAFF_NAM { get; set; }

        ///录入申请员工
        [ResultColumn]
        public string APPLY_USR_NAM { get; set; }

        [ResultColumn]
        public string ORG_ID { get; set; }

        [ResultColumn]
        public string ORG_NAM { get; set; }

        public enum APPLY_STATE_ENUM
        {
            [Description("草稿")]
            草稿 = 1,

            [Description("申请中")] 申请中 = 2,

            [Description("申请拒绝")] 申请拒绝 = 3,

            [Description("再申请")] 再申请 = 4,

            [Description("作废")] 作废 = 5,

            [Description("同意")] 同意 = 6
        }
    }
}
