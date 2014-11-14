using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace gzsw.model.ext
{
    public partial class WARN_ALARM_SEND_USER_CON
    {

        public enum ALARM_TYP_ENUM
        {
            [Description("客户端报警")]
            客户端报警 = 1,

            [Description("差评报警")]
            差评报警 = 2,

            [Description("超时报警")]
            超时报警 = 3,

            [Description("投诉报警")]
            投诉报警 = 4,

        }
    }


    
}
