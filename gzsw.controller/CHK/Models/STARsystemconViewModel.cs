using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace gzsw.controller.CHK.Models
{
    public class STARsystemconViewModel
    {
        
        [Display(Name = @"星级，１星级填１，２星级填２")]
        [Required(ErrorMessage = "不能为空！")]
        [Remote("CheckLEVEL", "STARsystemcon", AdditionalFields = "STAR_LEVEL", ErrorMessage = "该星级已存在.")]
        public int STAR_LEVEL { get; set; }




        

        public decimal? ATTEND_SCORE { get; set; }




        

        public decimal? QUEUE_DETAIN_SCORE { get; set; }




        

        public decimal? HANDLE_ONTIME_SCORE { get; set; }




        

        public decimal? QUALITY_SCORE { get; set; }




        

        public decimal? EVAL_SATISFY_SCORE { get; set; }




        

        public decimal? COMPLAIN_SCORE { get; set; }




        

        public decimal? ENVIRON_SCORE { get; set; }




        

        public decimal? SYSTEM_SCORE { get; set; }




        

        public decimal? NORM_SCORE { get; set; }




        

        public decimal? PROFESS_SCORE { get; set; }




        

        public decimal? THIRD_SURVEY_SCORE { get; set; }




        

        public decimal? OTHER_SCORE { get; set; }
    }
}
