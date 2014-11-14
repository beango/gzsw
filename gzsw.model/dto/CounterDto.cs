using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using PetaPoco;

namespace gzsw.model.dto
{
    /// <summary>
    /// 窗口Dto
    /// </summary>
    public class CounterDto
    {
        [ResultColumn]
        public string HALL_NO { get; set; }

        [ResultColumn]
        public int COUNTER_ID { get; set; }

        [ResultColumn]
        public byte COUNTER_STATE { get; set; }

        [ResultColumn]
        public byte? LOGIN_STATE { get; set; }

        [ResultColumn]
        public byte? STATE { get; set; }

        [ResultColumn]
        public string STAFF_NAM { get; set; }

        [ResultColumn]
        public string LST_QUEUE_NUMBER { get; set; }

        [ResultColumn]
        public string LST_NSR_SBM { get; set; }

        [ResultColumn]
        public string SERIALNAME { get; set; }

        [ResultColumn]
        public string Q_SERIALNAME { get; set; }

        public string GetState()
        {
            if (LOGIN_STATE != 1 || STATE == null)
            {
                return "离线";
            }
            switch (STATE.GetValueOrDefault())
            {
                case 0:
                case 1:
                case 4:
                    return "在线";
                case 2:
                    return "办理中";
                case 3:
                    return "办理完结";
                case 5:
                    return "暂停";
            }
            return "离线";
        }
    }
}
