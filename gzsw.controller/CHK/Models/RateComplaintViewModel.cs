using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace gzsw.controller.CHK.Models
{
    public class RateComplaintViewModel
    {
        public int STAT_MO { get; set; }

        public string HALL_NO { get; set; }

        [Required(ErrorMessage = "不能为空！")]
        [RegularExpression(@"^\d+$", ErrorMessage = @"应为正整数")]
        public int COR_COMPLAIN_CNT { get; set; }
        [Required(ErrorMessage = "不能为空！")]
        [RegularExpression(@"^\d+$", ErrorMessage = @"应为正整数")]
        public int COR_HALL_STAFF_CNT { get; set; } 
    }
}
