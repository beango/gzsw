using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace gzsw.model.Enums
{
    /// <summary>
    /// 行为项类型
    /// </summary>
    public enum USUACTTYPEENUM
    {
        [Description("工作纪律")]
        工作纪律 = 1,
        [Description("办税服务")]
        办税服务 = 2,
        [Description("税容税貌")]
        税容税貌 = 3,
        [Description("学习培训")]
        学习培训 = 4,
        [Description("安全卫生")]
        安全卫生 = 5,
        [Description("其他项")]
        其他项 = 6
    }
}
