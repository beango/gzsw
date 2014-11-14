using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace gzsw.model.Enums
{
    /// <summary>
    /// 季度
    /// </summary>
    public enum QuarterEnum
    {
        /// <summary>
        /// 1-3月
        /// </summary>
        [Description("第一季度")]
        第一季度=1,

        [Description("第二季度")]
        第二季度 = 2,

        [Description("第三季度")]
        第三季度 = 3,

        [Description("第四季度")]
        第四季度 = 4
    }
}
