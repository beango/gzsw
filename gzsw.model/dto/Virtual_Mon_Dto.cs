using PetaPoco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace gzsw.model.dto
{
    /// <summary>
    /// 监控预警内容信息
    /// </summary>
    public class Virtual_Mon_Dto
    {
        /// <summary>
        /// 表，
        /// 1.管理情况，
        /// 2.大厅排队情况
        /// 3.服务情况
        /// 4.纳税人评价情况
        /// </summary>
        [ResultColumn]
        public int TABLELIST_ID { get; set; }

        [ResultColumn]
        public int LIST_ID { get; set; }

        [ResultColumn]
        public decimal VALUE { get; set; }
    }
}
