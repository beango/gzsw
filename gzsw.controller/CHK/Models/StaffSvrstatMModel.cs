using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace gzsw.controller.CHK.Models
{
    public class StaffSvrstatMModel
    {

        [Required(ErrorMessage = "不能为空！")]
        public int STAT_MO { get; set; }

        [Required(ErrorMessage = "不能为空！")]
        public string STAFF_ID { get; set; }

        public string STAFF_NAM { get; set; }

        [Required(ErrorMessage = "不能为空！")]
        public string SERIALID { get; set; }

        public string SERIALNAME { get; set; }

        [Required(ErrorMessage = "不能为空！")]
        [RegularExpression(@"^\d+$", ErrorMessage = @"应为正整数")]
        public int COR_DOOR_SVR_CNT { get; set; }

        [Required(ErrorMessage = "不能为空！")]
        [RegularExpression(@"^\d+$", ErrorMessage = @"应为正整数")]
        public int OTHER_SVR_CNT { get; set; }

        [Required(ErrorMessage = "不能为空！")]
        [RegularExpression(@"^\d+$", ErrorMessage = @"应为正整数")]
        public int COR_OVERTIME_SVR_CNT { get; set; }

    }
}
