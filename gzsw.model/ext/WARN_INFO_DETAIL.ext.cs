using PetaPoco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace gzsw.model
{
    public partial class WARN_INFO_DETAIL
    {
        [ResultColumn]
        public STATE_ENUM? STATE_NAM
        {
            get
            {
                STATE_ENUM rst;
                if (Enum.TryParse<STATE_ENUM>(STATE.ToString(), out rst))
                {
                    return rst;
                }
                return null;
            }
        }

        [ResultColumn]
        public WARN_TYP_ENUM? WARN_TYP_NAM
        {
            get
            {
                WARN_TYP_ENUM rst;
                if (Enum.TryParse<WARN_TYP_ENUM>(WARN_TYP.ToString(), out rst))
                {
                    return rst;
                }
                return null;
            }
        }

        [ResultColumn]
        public WARN_LVL_ENUM? WARN_LVL_NAM
        {
            get
            {
                WARN_LVL_ENUM rst;
                if (Enum.TryParse<WARN_LVL_ENUM>(WARN_LEVEL.ToString(), out rst))
                {
                    return rst;
                }
                return null;
            }
        }

        [ResultColumn]
        public string HALL_NAM { get; set; }

        [ResultColumn]
        public string STAFF_NAM { get; set; }

        public enum STATE_ENUM
        {
            未处理 = 1,

            已处理 = 2
        }

        public enum WARN_TYP_ENUM
        {
            等候超时 = 1,
            等候超时率 = 2,
            窗口饱和度 = 3,
            大厅饱和度 = 4,
            超时办结率 = 5,
            超时业务笔数 = 6,
            弃号率 = 7,
            差评笔数预警 = 8,
            连续工作时长超界 = 9
        }

        public enum WARN_LVL_ENUM
        {
            黄色预警 = 1,
            橙色预警 = 2,
            红色预警 = 3
        }
    }
}
