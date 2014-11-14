using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace gzsw.controller.CHK.Models
{
    public class StaffAttendDeductConAddModel
    {
        [Required(ErrorMessage = "不能为空！")]
        [Remote("ValidateOrgId", "StaffAttendDeductCon", ErrorMessage = "该组织已存在设置.")]
        public string ORG_ID { get; set; }

        [Required(ErrorMessage = "不能为空！")]
        [RegularExpression(@"^\d+(\.\d+)?$", ErrorMessage = @"应为非负数字")]
        public decimal? EAR_SCORE { get; set; }

        [Required(ErrorMessage = "不能为空！")]
        [RegularExpression(@"^\d+(\.\d+)?$", ErrorMessage = @"应为非负数字")]
        public decimal? NEG_SCORE { get; set; }

        [Required(ErrorMessage = "不能为空！")]
        [RegularExpression(@"^\d+(\.\d+)?$", ErrorMessage = @"应为非负数字")]
        public decimal? LAT_SCORE { get; set; }

        public string MODIFY_ID { get; set; }

        public DateTime? MODIFY_DTIME { get; set; }
    }

    public class StaffAttendDeductConModel
    {
        [Required(ErrorMessage = "不能为空！")]
        public string ORG_ID { get; set; }

        public string ORG_NAM { get; set; }

        [Required(ErrorMessage = "不能为空！")]
        [RegularExpression(@"^\d+(\.\d+)?$", ErrorMessage = @"应为非负数字")]
        public decimal? EAR_SCORE { get; set; }

        [Required(ErrorMessage = "不能为空！")]
        [RegularExpression(@"^\d+(\.\d+)?$", ErrorMessage = @"应为非负数字")]
        public decimal? NEG_SCORE { get; set; }

        [Required(ErrorMessage = "不能为空！")]
        [RegularExpression(@"^\d+(\.\d+)?$", ErrorMessage = @"应为非负数字")]
        public decimal? LAT_SCORE { get; set; }

        public string MODIFY_ID { get; set; }

        public DateTime? MODIFY_DTIME { get; set; }
    }
}
