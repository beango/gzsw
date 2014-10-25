using System.Collections.Generic;
using PetaPoco;
using System.ComponentModel;

namespace gzsw.model
{
    public partial class SYS_ORGANIZE
    {
        [Ignore]
        public IList<SYS_ORGANIZE> Par_OrgList { get; set; }
       
        public enum ORG_LEVEL_ENUM
        {
            [Description("省级")]
            省级=1,

            [Description("市级")]
            市级 = 2,

            [Description("区级")]
            区级 = 3,

            [Description("服务厅级")]
            服务厅级 = 4

        }
    }
}
