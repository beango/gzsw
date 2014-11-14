using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace gzsw.model.Enums
{
    /// <summary>
    /// 用户最高等级
    /// </summary>
    public enum UserLV_ENUM
    {
        [Description("无权限")]
        无权限=0,

        [Description("省级")]
        省级=1,

        [Description("市级")]
        市级=2,

        [Description("区级")]
        区级=3,

        [Description("服务厅级")]
        服务厅级=4
    }
}
