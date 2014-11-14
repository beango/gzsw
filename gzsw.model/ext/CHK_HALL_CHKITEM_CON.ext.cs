using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace gzsw.model.ext
{
    public partial class CHK_HALL_CHKITEM_CON
    {


//        1:环境建设
//2:制度建设
//3:规范建设
//4:行风建设
//5:第三方调查
//6:其他
        public enum CHKITEM_TYP
        {

            [Description("环境建设")]
            环境建设 = 1,

            [Description("制度建设")]
            制度建设 = 2,
            [Description("规范建设")]
            规范建设 = 3,

            [Description("行风建设")]
            行风建设 = 4,

            [Description("第三方调查")]
            第三方调查 = 5,
            [Description("其他")]
            其他 = 6
        }
    }
}
