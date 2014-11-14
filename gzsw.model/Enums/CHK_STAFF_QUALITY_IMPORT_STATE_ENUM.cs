using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace gzsw.model.Enums
{
    /// <summary>
    /// 个人考核质量差错导入状态
    /// </summary>
    public enum CHK_STAFF_QUALITY_IMPORT_STATE_ENUM
    {
        [Description("未导入")]
        未导入 = 0,

        [Description("已成功导入")]
        已成功导入 = 1,

        [Description("员工数据有问题未导入")]
        员工数据有问题未导入 = 2,

        [Description("事项编码校验错误未导入")]
        事项编码校验错误未导入 = 3,

        [Description("质量类型错误未导入")]
        质量类型错误未导入 = 4,

        [Description("数量值错误未导入")]
        数量值错误未导入 = 5,

        [Description("错误发生日期有问题未导入")]
        错误发生日期有问题未导入 = 6
    }
}
