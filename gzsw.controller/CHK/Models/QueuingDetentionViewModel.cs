using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace gzsw.controller.CHK.Models
{
    public class QueuingDetentionViewModel
    {
        public int STAT_MO { get; set; }

        public string HALL_NO { get; set; }

        [Required(ErrorMessage = "不能为空！")]
        [RegularExpression(@"^\d+$", ErrorMessage = @"应为正整数")]
        public int COR_OVERTIME_CNT { get; set; }
        [Required(ErrorMessage = "不能为空！")]
        [RegularExpression(@"^\d+$", ErrorMessage = @"应为正整数")]
        public int COR_VALID_QUEUE_CNT { get; set; } 
    }
}
