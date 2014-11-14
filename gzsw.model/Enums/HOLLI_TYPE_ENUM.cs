using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace gzsw.model.Enums
{
    /// <summary>
    /// 请假类型
    /// </summary>
    public enum HOLLI_TYPE_ENUM
    {
        [Description("事假")]
        事假 = 1,

        [Description("病假")]
        病假 = 2,

        [Description("丧假")]
        丧假 = 3,

        [Description("婚假")]
        婚假 = 4,

        [Description("年休假")]
        年休假 = 5,

        [Description("工伤假")]
        工伤假 = 6,

        [Description("加班补休")]
        加班补休 = 7,

        [Description("陪产假")]
        陪产假 = 8,

        [Description("产假")]
        产假 = 9,

        [Description("公差")]
        公差 = 10,
        [Description("会议")]
        会议=11,
        [Description("学习")]
        学习=12,
        [Description("其他")]
        其他=13
    }
}
