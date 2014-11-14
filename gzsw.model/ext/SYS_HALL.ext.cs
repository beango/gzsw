using PetaPoco;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace gzsw.model
{
    public partial class SYS_HALL
    {
        [ResultColumn]
        public SYS_ORGANIZE HALLORG { get; set; }

        public enum TICKETNUM_ENUM
        {
            [Description("不限制")]
            不限制 = 0,

            [Description("只能取一个")]
            只能取一个 = 1,

            [Description("队列只能取一个号")]
            队列只能取一个号 = 2
        }
    }
}
