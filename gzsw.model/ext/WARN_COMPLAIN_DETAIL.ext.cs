using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using PetaPoco;

namespace gzsw.model.ext
{
    public partial class WARN_COMPLAIN_DETAIL
    {
        /// <summary>
        /// 纳税人名称
        /// </summary>
        [ResultColumn]
        public string NSR_NAm { get; set; }

        /// <summary>
        /// 服务厅名称
        /// </summary>
        [ResultColumn]
        public string HALL_Nam { get; set; }

        public enum DETAIL_STATUS_ENUM
        {
            [Description("未处理")]
            未处理 = 1,

            [Description("已处理")]
            已处理 = 2,

            [Description("已撤销")]
            已撤销 = 3
        }
    }

    
}
