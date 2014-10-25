using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using PetaPoco;

namespace gzsw.model
{
    public partial class SYS_COUNTER
    {
        [Ignore]
        public string PRI1_BUSI_SER_NAM { get; set; }
        [Ignore]
        public string PRI2_BUSI_SER_NAM { get; set; }
        [Ignore]
        public string PRI3_BUSI_SER_NAM { get; set; }
        [Ignore]
        public string PRI4_BUSI_SER_NAM { get; set; }
        [Ignore]
        public string PRI5_BUSI_SER_NAM { get; set; }

        [Ignore]
        public SYS_HALL Hall { get; set; }

        [Ignore]
        public SYS_ORGANIZE Org { get; set; }

        /// <summary>
        /// 状态
        /// 1:正常启用，2：维修，3：停止使用
        /// </summary>
        public enum STATE_ENUM
        {
            [Description("正常启用")]
            正常启用 = 1,

            [Description("维修")]
            维修 = 2,

            [Description("停止使用")]
            停止使用 = 3

        }
    }
}
