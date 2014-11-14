using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace gzsw.model.dto
{
    /// <summary>
    /// 个人考核评定
    ///     综合评定
    /// </summary>
    public class ComprehensiveDto
    {
        /// <summary>
        /// 月份
        /// </summary>
        public int STAT_MO { get; set; }

        /// <summary>
        /// 员工编码
        /// </summary>
        public string STAFF_ID { get; set; }

        /// <summary>
        /// 员工名称
        /// </summary>
        public string STAFF_NAM { get; set; }

        /// <summary>
        /// 纳税人评分
        /// </summary>
        public decimal TaxpayerScore { get; set; }

        /// <summary>
        /// 业务表现分
        /// </summary>
        public decimal BusinessScore { get; set; }

        /// <summary>
        /// 效率分
        /// </summary>
        public decimal EfficiencyScore { get; set; }

        /// <summary>
        /// 质量分
        /// </summary>
        public decimal QualityScore { get; set; }

        /// <summary>
        /// 考勤分
        /// </summary>
        public decimal AttendanceScore { get; set; }

        /// <summary>
        /// 投诉分
        /// </summary>
        public decimal ComplaintsScore { get; set; }

        /// <summary>
        /// 日常行为分
        /// </summary>
        public decimal DailyScore { get; set; }

        /// <summary>
        /// 综合评议与额外奖罚分
        /// </summary>
        public decimal RewardsAndPunishmentScore { get; set; }

        /// <summary>
        /// 综合分
        /// </summary>
        public decimal ComprehensiveScore { get; set; }
    }
}
