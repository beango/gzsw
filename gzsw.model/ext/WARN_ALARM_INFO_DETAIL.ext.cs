using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace gzsw.model.ext
{
    public partial class WARN_ALARM_INFO_DETAIL
    {


        public enum DETAIL_STATUS_ENUM
        {
            [Description("未处理")]
            未处理 = 1,

            [Description("已处理")]
            已处理 = 2
        } 
        public enum ALARM_TYP_ENUM
        {
            [Description("客户端报警")]
            客户端报警 = 1,

            [Description("差评报警")]
            差评报警 = 2,
            [Description("超时报警")]
            超时报警 = 3
        }
    }
}
