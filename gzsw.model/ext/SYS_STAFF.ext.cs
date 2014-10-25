using PetaPoco;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace gzsw.model
{
    public partial class SYS_STAFF
    {
        [Ignore]
        public SYS_ORGANIZE ORG { get; set; }

        public enum STAFF_TYP_ENUM
        {
            [Description("前台员工")]
            前台员工 = 1,

            [Description("后勤员工")]
            后勤员工 = 2
        }

        public enum STAR_LEVEL_ENUM
        {
            [Description("一星")]
            一星 = 1,

            [Description("二星")]
            二星 = 2,

            [Description("三星")]
            三星 = 3,

            [Description("四星")]
            四星 = 4,

            [Description("五星")]
            五星 = 5
        }
    }
}
