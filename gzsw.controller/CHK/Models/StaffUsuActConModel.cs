using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using PetaPoco;

namespace gzsw.controller.CHK.Models
{
    /// <summary>
    /// 个人考核指标管理
    /// 日常行为表现
    /// </summary>
    public class StaffUsuActConModel
    {

        [Required(ErrorMessage = "不能为空！")]
        [Remote("ValidateOrgId", "StaffUsuActCon", ErrorMessage = "该组织已存在设置.")]
        public string ORG_ID { get; set; }

        public string ORG_NAM{get; set; }

        [Required(ErrorMessage = "不能为空！")]
        [RegularExpression(@"^\d+(\.\d+)?$", ErrorMessage = @"应为非负数字")]
        public decimal WORK_DIS_RT { get; set; }

        [Required(ErrorMessage = "不能为空！")]
        [RegularExpression(@"^\d+(\.\d+)?$", ErrorMessage = @"应为非负数字")]
        public decimal TAX_SVR_RT { get; set; }

        [Required(ErrorMessage = "不能为空！")]
        [RegularExpression(@"^\d+(\.\d+)?$", ErrorMessage = @"应为非负数字")]
        public decimal TAX_LOOK_RT { get; set; }

        [Required(ErrorMessage = "不能为空！")]
        [RegularExpression(@"^\d+(\.\d+)?$", ErrorMessage = @"应为非负数字")]
        public decimal TRAIN_RT { get; set; }

        [Required(ErrorMessage = "不能为空！")]
        [RegularExpression(@"^\d+(\.\d+)?$", ErrorMessage = @"应为非负数字")]
        public decimal SEC_HEAL_RT { get; set; }

        [Required(ErrorMessage = "不能为空！")]
        [RegularExpression(@"^\d+(\.\d+)?$", ErrorMessage = @"应为非负数字")]
        //[Remote("ValidateDecimal", "StaffUsuActCon", AdditionalFields = "WORK_DIS_RT,TAX_SVR_RT,TAX_LOOK_RT,TRAIN_RT,SEC_HEAL_RT", ErrorMessage = "各项相加应等于1.")]
        public decimal OTHER_RT { get; set; }
    }

    public class StaffUsuActConEditModel
    {
        [Required(ErrorMessage = "不能为空！")]
        public string ORG_ID { get; set; }

        public string ORG_NAM { get; set; }

        [Required(ErrorMessage = "不能为空！")]
        [RegularExpression(@"^\d+(\.\d+)?$", ErrorMessage = @"应为非负数字")]
        public decimal WORK_DIS_RT { get; set; }

        [Required(ErrorMessage = "不能为空！")]
        [RegularExpression(@"^\d+(\.\d+)?$", ErrorMessage = @"应为非负数字")]
        public decimal TAX_SVR_RT { get; set; }

        [Required(ErrorMessage = "不能为空！")]
        [RegularExpression(@"^\d+(\.\d+)?$", ErrorMessage = @"应为非负数字")]
        public decimal TAX_LOOK_RT { get; set; }

        [Required(ErrorMessage = "不能为空！")]
        [RegularExpression(@"^\d+(\.\d+)?$", ErrorMessage = @"应为非负数字")]
        public decimal TRAIN_RT { get; set; }

        [Required(ErrorMessage = "不能为空！")]
        [RegularExpression(@"^\d+(\.\d+)?$", ErrorMessage = @"应为非负数字")]
        public decimal SEC_HEAL_RT { get; set; }

        [Required(ErrorMessage = "不能为空！")]
        [RegularExpression(@"^\d+(\.\d+)?$", ErrorMessage = @"应为非负数字")]
        public decimal OTHER_RT { get; set; }
    }
}
