using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using PetaPoco;

namespace gzsw.model.Subclasses
{
    /// <summary>
    /// 当日排队明细表 子类
    /// </summary>
    public class SYS_CURRQUEUEHIST_SUB : SYS_CURRQUEUEHIST
    {
        [ResultColumn]
        public string HALL_NAM { get; set; }

        [ResultColumn]
        public string STAFF_NAM { get; set; }

        [ResultColumn]
        public string Q_SERIALNAME { get; set; }

        [ResultColumn]
        public string CHQUEUE_HALL_NAM { get; set; }

        public enum PJRESULTENUM
        {
            [Description("")]
            未叫号=-1,
            [Description("未评价")]
            未评价 = 0,
            [Description("非常满意")]
            非常满意 = 1,
            [Description("满意")]
            满意 = 2,
            [Description("基本满意")]
            基本满意 = 3,
            [Description("不满意")]
            不满意 = 4
        }

        public enum STATUSENUM
        {
            [Description("未呼叫")]
            未呼叫=0,
            [Description("已呼叫")]
            已呼叫=1,
            [Description("已受理但未完成")]
            已受理但未完成=2,
            [Description("完成受理")]
            完成受理=3,
            [Description("弃号")]
            弃号=4
        }

        public enum TICKETTYPEEUNM
        {
            [Description("现场取号")]
            现场取号 = 0,
            [Description("远程取号")]
            远程取号 = 1,
            [Description("预约取号")]
            预约取号 = 2
        }

    }
}
