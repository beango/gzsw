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


        public enum HOLLI_TYPE_ENUM
        {
            [Description("事假")] 事假 = 1,

            [Description("病假")] 病假 = 2,

            [Description("丧假")] 丧假 = 3,

            [Description("婚假")] 婚假 = 4,

            [Description("年休假")] 年休假 = 5,

            [Description("工伤假")] 工伤假 = 6,

            [Description("加班补休")] 加班补休 = 7,

            [Description("陪产假")] 陪产假 = 8,

            [Description("产假")] 产假 = 9,

            [Description("出差")] 出差 = 6
        }


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
