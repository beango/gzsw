using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace gzsw.controller.CHK.Models
{

    public class StaffUsuActMarkAddModel
    {
        [Required(ErrorMessage = "不能为空！")]
        public string ORG_ID { get; set; }

        public int SEQ { get; set; }

        [Required(ErrorMessage = "不能为空！")]
        [Display(Name = "统计月份")]
        public DateTime STAT_MO { get; set; }

        [Required(ErrorMessage = "不能为空！")]
        public string STAFF_ID { get; set; }

        [Required(ErrorMessage = "不能为空！")]
        public byte USU_ACT_TYP { get; set; }

        [Required(ErrorMessage = "不能为空！")]
        [RegularExpression(@"^\d+(\.\d+)?$", ErrorMessage = @"应为非负数字")]
        public decimal DEDUCT { get; set; }

        [Required(ErrorMessage = "不能为空！")]
        public string REASON { get; set; }
    }

    public class StaffUsuActMarkModel
    {
        [Required(ErrorMessage = "不能为空！")]
        public string ORG_ID { get; set; }

        public string ORG_NAM { get; set; }

        public int SEQ { get; set; }

        [Required(ErrorMessage = "不能为空！")]
        [Display(Name = "统计月份")]
        public string STAT_MO { get; set; }

        [Required(ErrorMessage = "不能为空！")]
        public string STAFF_ID { get; set; }

        public string STAFF_NAM { get; set; }

        [Required(ErrorMessage = "不能为空！")]
        public byte USU_ACT_TYP { get; set; }

        [Required(ErrorMessage = "不能为空！")]
        [RegularExpression(@"^\d+(\.\d+)?$", ErrorMessage = @"应为非负数字")]
        public decimal DEDUCT { get; set; }

        [Required(ErrorMessage = "不能为空！")]
        public string REASON { get; set; }

        public DateTime MARK_TIME { get; set; }
    }
}
