using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace gzsw.controller.CHK.Models
{
    /// <summary>
    /// 个人考核指标管理
    /// 满意度设置
    /// </summary>
    public class SatisRtConAddModel
    {
        [Required(ErrorMessage = "不能为空！")]
        [Remote("ValidateOrgId", "SatisRtCon", ErrorMessage = "该组织已存在设置.")]
        public string ORG_ID { get; set; }

        [Required(ErrorMessage = "不能为空！")]
        [RegularExpression(@"^\d+(\.\d+)?$", ErrorMessage = @"应为非负数字")]
        public decimal VERY_SATISFY_SCORE { get; set; }

        [Required(ErrorMessage = "不能为空！")]
        [RegularExpression(@"^\d+(\.\d+)?$", ErrorMessage = @"应为非负数字")]
        public decimal SATISFY_SCORE { get; set; }

        [Required(ErrorMessage = "不能为空！")]
        [RegularExpression(@"^\d+(\.\d+)?$", ErrorMessage = @"应为非负数字")]
        public decimal COMMON_SCORE { get; set; }

        [Required(ErrorMessage = "不能为空！")]
        [RegularExpression(@"^\d+(\.\d+)?$", ErrorMessage = @"应为非负数字")]
        public decimal UNSATISFY_SCORE { get; set; }

        [Required(ErrorMessage = "不能为空！")]
        [RegularExpression(@"^\d+(\.\d+)?$", ErrorMessage = @"应为非负数字")]
        public decimal NON_EVAL_SCORE { get; set; }
    }

    public class SatisRtConModel
    {
        [Required(ErrorMessage = "不能为空！")]
        public string ORG_ID { get; set; }

        public string ORG_NAM { get; set; }

        [Required(ErrorMessage = "不能为空！")]
        [RegularExpression(@"^\d+(\.\d+)?$", ErrorMessage = @"应为非负数字")]
        public decimal VERY_SATISFY_SCORE { get; set; }

        [Required(ErrorMessage = "不能为空！")]
        [RegularExpression(@"^\d+(\.\d+)?$", ErrorMessage = @"应为非负数字")]
        public decimal SATISFY_SCORE { get; set; }

        [Required(ErrorMessage = "不能为空！")]
        [RegularExpression(@"^\d+(\.\d+)?$", ErrorMessage = @"应为非负数字")]
        public decimal COMMON_SCORE { get; set; }

        [Required(ErrorMessage = "不能为空！")]
        [RegularExpression(@"^\d+(\.\d+)?$", ErrorMessage = @"应为非负数字")]
        public decimal UNSATISFY_SCORE { get; set; }

        [Required(ErrorMessage = "不能为空！")]
        [RegularExpression(@"^\d+(\.\d+)?$", ErrorMessage = @"应为非负数字")]
        public decimal NON_EVAL_SCORE { get; set; }
    }
}
