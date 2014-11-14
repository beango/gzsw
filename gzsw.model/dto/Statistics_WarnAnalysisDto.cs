using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PetaPoco;

namespace gzsw.model.dto
{
    /// <summary>
    /// 预警分析-服务厅
    /// </summary>
    public class Statistics_WarnAnalysisDto
    {
        /// <summary>
        /// 服务厅编码
        /// </summary>
        [ResultColumn]
        public string HALL_NO { get; set; }

        /// <summary>
        /// 服务厅
        /// </summary>
        [ResultColumn]
        public string HALL_NAM { get; set; }

        /// <summary>
        /// 预警级别（Y、O、R、H）
        /// </summary>
        [ResultColumn]
        public string L_Code { get; set; }

        /// <summary>
        /// 预警级别（黄、橙、红、合计）
        /// </summary>
        [ResultColumn]
        public string L_Name { get; set; }


        /// <summary>
        /// 预警级别（1、2、3、0）
        /// </summary>
        [ResultColumn]
        public int L_Value { get; set; }

        /// <summary>
        /// 预警类型（1：等候超时）
        /// </summary>
        [ResultColumn]
        public int Count_T1 { get; set; }

        /// <summary>
        /// 预警类型（2：等候超时率）
        /// </summary>
        [ResultColumn]
        public int Count_T2 { get; set; }

        /// <summary>
        /// 预警类型（3：窗口饱和度）
        /// </summary>
        [ResultColumn]
        public int Count_T3 { get; set; }

        /// <summary>
        /// 预警类型（4：大厅饱和度）
        /// </summary>
        [ResultColumn]
        public int Count_T4 { get; set; }

        /// <summary>
        /// 预警类型（5：超时办结率）
        /// </summary>
        [ResultColumn]
        public int Count_T5 { get; set; }

        /// <summary>
        /// 预警类型（6：超时业务笔数）
        /// </summary>
        [ResultColumn]
        public int Count_T6 { get; set; }

        /// <summary>
        /// 预警类型（7：弃号率）
        /// </summary>
        [ResultColumn]
        public int Count_T7 { get; set; }

        /// <summary>
        /// 预警类型（8：差评笔数预警）
        /// </summary>
        [ResultColumn]
        public int Count_T8 { get; set; }

        /// <summary>
        /// 预警类型（9：连续工作时长超界）
        /// </summary>
        [ResultColumn]
        public int Count_T9 { get; set; }
    }

    /// <summary>
    /// 统计分析-明细
    /// </summary>
    public class Statistics_AnalysisDetailDto
    {
        /// <summary>
        /// X轴ID
        /// </summary>
        [ResultColumn]
        public string X_Id { get; set; }
        
        /// <summary>
        /// X轴名称
        /// </summary>
        [ResultColumn]
        public string X_ShowName { get; set; }

        /// <summary>
        /// X轴类型(1:按小时统计 2:按日统计 3:按月统计 4:按年统计)
        /// </summary>
        [ResultColumn]
        public string X_Type { get; set; }

        /// <summary>
        /// Y轴数值
        /// </summary>
        [ResultColumn]
        public int Y_Value { get; set; }
    }
}
