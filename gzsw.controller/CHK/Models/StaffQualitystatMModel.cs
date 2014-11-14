using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace gzsw.controller.CHK.Models
{
    public class StaffQualitystatMModel
    {
        [Required(ErrorMessage = "不能为空！")]
        public int STAT_MO { get; set; }

        [Required(ErrorMessage = "不能为空！")]
        public string STAFF_ID { get; set; }

        [Required(ErrorMessage = "不能为空！")]
        public string QUALITY_CD { get; set; }

        public string STAFF_NAM { get; set; }

        public string QUALITY_NAM { get; set; }

        [Required(ErrorMessage = "不能为空！")]
        [RegularExpression(@"^\d+$", ErrorMessage = @"应为正整数")]
        public int COR_ERROR_SVR_CNT { get; set; }
    }
}
