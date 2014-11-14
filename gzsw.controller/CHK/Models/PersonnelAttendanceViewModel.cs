using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace gzsw.controller.CHK.Models
{
    public class PersonnelAttendanceViewModel
    {
        public int STAT_MO { get; set; }

        public string HALL_NO { get; set; }

        [Required(ErrorMessage = "不能为空！")]
        [RegularExpression(@"^\d+$", ErrorMessage = @"应为正整数")]
        public int COR_LAT_DAY_CNT {get;set;}
        [Required(ErrorMessage = "不能为空！")]
        [RegularExpression(@"^\d+$", ErrorMessage = @"应为正整数")]
        public int COR_EAR_DAY_CNT {get;set;}
        [Required(ErrorMessage = "不能为空！")]
        [RegularExpression(@"^\d+$", ErrorMessage = @"应为正整数")]
        public int COR_ABSENT_DAY_CNT {get;set;}
        [Required(ErrorMessage = "不能为空！")]
        [RegularExpression(@"^\d+$", ErrorMessage = @"应为正整数")]
        public int COR_ATTEND_PER_CNT {get;set;}
        [Required(ErrorMessage = "不能为空！")]
        [RegularExpression(@"^\d+$", ErrorMessage = @"应为正整数")]
        public int COR_WORK_DAY_CNT {get;set;}
    }
}
