using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace gzsw.controller.CHK.Models
{

    /// <summary>
    /// 个人考核指标管理
    /// 考核体系
    /// </summary>
    public class StaffSystemConAddModel
    {
        public StaffSystemConAddModel()
        {
            COMPRE_SAN_100_MAX_SCORE = 0;
            COMPRE_SAN_TYP = 1;
        }

        [Required(ErrorMessage = "不能为空！")]
        [Remote("ValidateOrgId", "StaffSystemCon", ErrorMessage = "该组织已存在设置.")]
        public string ORG_ID { get; set; }

        [Required(ErrorMessage = "不能为空！")]
        public byte COMPRE_SAN_TYP { get; set; }

        [Required(ErrorMessage = "不能为空！")]
        [RegularExpression(@"^\d+(\.\d+)?$", ErrorMessage = @"应为非负数字")]
        public decimal COMPRE_SAN_100_MAX_SCORE { get; set; }

        [Required(ErrorMessage = "不能为空！")]
        [RegularExpression(@"^\d+(\.\d+)?$", ErrorMessage = @"应为非负数字")]
        public decimal? EVAL_CHK_RT { get; set; }

        [Required(ErrorMessage = "不能为空！")]
        [RegularExpression(@"^\d+(\.\d+)?$", ErrorMessage = @"应为非负数字")]
        public decimal? SVR_CHK_RT { get; set; }

        [Required(ErrorMessage = "不能为空！")]
        [RegularExpression(@"^\d+(\.\d+)?$", ErrorMessage = @"应为非负数字")]
        public decimal? QUALITY_CHK_RT { get; set; }

        [Required(ErrorMessage = "不能为空！")]
        [RegularExpression(@"^\d+(\.\d+)?$", ErrorMessage = @"应为非负数字")]
        public decimal? EFFIC_CHK_RT { get; set; }

        [Required(ErrorMessage = "不能为空！")]
        [RegularExpression(@"^\d+(\.\d+)?$", ErrorMessage = @"应为非负数字")]
        public decimal? EFFIC_AVOID_RT { get; set; }

        [Required(ErrorMessage = "不能为空！")]
        [RegularExpression(@"^\d+(\.\d+)?$", ErrorMessage = @"应为非负数字")]
        public decimal? EFFIC_AVOID_EXC_DEDUCT { get; set; }

        [Required(ErrorMessage = "不能为空！")]
        [RegularExpression(@"^\d+(\.\d+)?$", ErrorMessage = @"应为非负数字")]
        public decimal? ATTEND_CHK_RT { get; set; }

        [Required(ErrorMessage = "不能为空！")]
        [RegularExpression(@"^\d+(\.\d+)?$", ErrorMessage = @"应为非负数字")]
        public decimal? USU_ACT_CHK_RT { get; set; }
    }

    public class StaffSystemConModel
    {
        public StaffSystemConModel()
        {
            COMPRE_SAN_100_MAX_SCORE = 0;
            COMPRE_SAN_TYP = 1;
        }

        [Required(ErrorMessage = "不能为空！")]
        public string ORG_ID { get; set; }

        public string ORG_NAM { get; set; }

        [Required(ErrorMessage = "不能为空！")]
        public byte COMPRE_SAN_TYP { get; set; }

        [Required(ErrorMessage = "不能为空！")]
        [RegularExpression(@"^\d+(\.\d+)?$", ErrorMessage = @"应为非负数字")]
        public decimal COMPRE_SAN_100_MAX_SCORE { get; set; }

        [Required(ErrorMessage = "不能为空！")]
        [RegularExpression(@"^\d+(\.\d+)?$", ErrorMessage = @"应为非负数字")]
        public string EVAL_CHK_RT { get; set; }

        [Required(ErrorMessage = "不能为空！")]
        [RegularExpression(@"^\d+(\.\d+)?$", ErrorMessage = @"应为非负数字")]
        public string SVR_CHK_RT { get; set; }

        [Required(ErrorMessage = "不能为空！")]
        [RegularExpression(@"^\d+(\.\d+)?$", ErrorMessage = @"应为非负数字")]
        public string QUALITY_CHK_RT { get; set; }

        [Required(ErrorMessage = "不能为空！")]
        [RegularExpression(@"^\d+(\.\d+)?$", ErrorMessage = @"应为非负数字")]
        public string EFFIC_CHK_RT { get; set; }

        [Required(ErrorMessage = "不能为空！")]
        [RegularExpression(@"^\d+(\.\d+)?$", ErrorMessage = @"应为非负数字")]
        public string EFFIC_AVOID_RT { get; set; }

        [Required(ErrorMessage = "不能为空！")]
        [RegularExpression(@"^\d+(\.\d+)?$", ErrorMessage = @"应为非负数字")]
        public decimal EFFIC_AVOID_EXC_DEDUCT { get; set; }

        [Required(ErrorMessage = "不能为空！")]
        [RegularExpression(@"^\d+(\.\d+)?$", ErrorMessage = @"应为非负数字")]
        public string ATTEND_CHK_RT { get; set; }

        [Required(ErrorMessage = "不能为空！")]
        [RegularExpression(@"^\d+(\.\d+)?$", ErrorMessage = @"应为非负数字")]
        public string USU_ACT_CHK_RT { get; set; }
    }
}
