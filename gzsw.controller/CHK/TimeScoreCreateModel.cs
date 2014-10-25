using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace gzsw.controller.CHK
{
    /// <summary>
    /// 考勤参数 增加
    /// </summary>
    public class TimeScoreCreateModel
    {
        [Required(ErrorMessage = "组织不能不空.")]
        public string ORG_ID { get; set; }

        public string ORG_NAM { get; set; }

        [Required(ErrorMessage = "上午上班时间不能不空.")]
        public string A_BEGIN_TIME { get; set; }

        [Required(ErrorMessage = "下午上班时间不能不空.")]
        public string P_BEGIN_TIME { get; set; }

        [Required(ErrorMessage = "上午下班时间不能不空.")]
        public string A_END_TIME { get; set; }

        [Required(ErrorMessage = "下午下班时间不能不空.")]
        public string P_END_TIME { get; set; }

        [Required(ErrorMessage = "迟到后置分钟不能不空.")]
        [RegularExpression(@"^\d+$", ErrorMessage = @"迟到后置分钟应为正整数")]
        public int? LAT_LAST_MIN { get; set; }

        [Required(ErrorMessage = "早退前置分钟不能不空.")]
        [RegularExpression(@"^\d+$", ErrorMessage = @"早退前置分钟应为正整数")]
        public int? EAR_LAST_MIN { get; set; }

        [Required(ErrorMessage = "早退扣分不能不空.")]
        [RegularExpression(@"^\d+(\.\d+)?$", ErrorMessage = @"早退扣分应为非负数字")]
        public decimal? EAR_SCORE { get; set; }

        [Required(ErrorMessage = "旷工扣分不能不空.")]
        [RegularExpression(@"^\d+(\.\d+)?$", ErrorMessage = @"旷工扣分应为非负数字")]
        public decimal? NEG_SCORE { get; set; }

        [Required(ErrorMessage = "病假扣分不能不空.")]
        [RegularExpression(@"^\d+(\.\d+)?$", ErrorMessage = @"病假扣分应为非负数字")]
        public decimal? ILL_SCORE { get; set; }

        [Required(ErrorMessage = "事假扣分不能不空.")]
        [RegularExpression(@"^\d+(\.\d+)?$", ErrorMessage = @"事假扣分应为非负数字")]
        public decimal? ABS_SCORE { get; set; }

        [Required(ErrorMessage = "迟到扣分不能不空.")]
        [RegularExpression(@"^\d+(\.\d+)?$", ErrorMessage = @"迟到扣分应为非负数字")]
        public decimal? LAT_SCORE { get; set; }

        [Required(ErrorMessage = "未签退扣分不能不空.")]
        [RegularExpression(@"^\d+(\.\d+)?$", ErrorMessage = @"未签退扣分应为非负数字")]
        public decimal? NONSIGN_SCORE { get; set; }
    }
}
