using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace gzsw.controller.CHK.Models
{
    public class CHK_HALL_STAR_SYSTEM_CON_Model
    {

       
        [Display(Name = @"星级，１星级填１，２星级填２")]

        [Required(ErrorMessage = "不能为空！")]
        public int STAR_LEVEL { get; set; }




          [RegularExpression(@"^(?:0|[1-9][0-9]?|100)$", ErrorMessage = "请输入0到100之间的整数")]
        public int ATTEND_SCORE { get; set; }





        [RegularExpression(@"^(?:0|[1-9][0-9]?|100)$", ErrorMessage = "请输入0到100之间的整数")]
        public int QUEUE_DETAIN_SCORE { get; set; }





        [RegularExpression(@"^(?:0|[1-9][0-9]?|100)$", ErrorMessage = "请输入0到100之间的整数")]
        public int HANDLE_ONTIME_SCORE { get; set; }





        [RegularExpression(@"^(?:0|[1-9][0-9]?|100)$", ErrorMessage = "请输入0到100之间的整数")]
        public int QUALITY_SCORE { get; set; }





        [RegularExpression(@"^(?:0|[1-9][0-9]?|100)$", ErrorMessage = "请输入0到100之间的整数")]
        public int EVAL_SATISFY_SCORE { get; set; }





        [RegularExpression(@"^(?:0|[1-9][0-9]?|100)$", ErrorMessage = "请输入0到100之间的整数")]
        public int COMPLAIN_SCORE { get; set; }





            [Required(ErrorMessage = "不能为空！")]

         
        public decimal? ENVIRON_SCORE { get; set; }





            [Required(ErrorMessage = "不能为空！")]
        public decimal? SYSTEM_SCORE { get; set; }





            [Required(ErrorMessage = "不能为空！")]
        public decimal? NORM_SCORE { get; set; }





            [Required(ErrorMessage = "不能为空！")]
        public decimal? PROFESS_SCORE { get; set; }





            [Required(ErrorMessage = "不能为空！")]
        public decimal? THIRD_SURVEY_SCORE { get; set; }





            [Required(ErrorMessage = "不能为空！")]
        public decimal? OTHER_SCORE { get; set; }
    }
}
