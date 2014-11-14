using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace gzsw.controller.CHK.Models
{
    public enum PJRESULTENUM
    {
        [Description("未评价")]
        未评价=0,
        [Description("非常满意")]
        非常满意 = 1,
        [Description("满意")]
        满意=2,
        [Description("基本满意")]
        基本满意 = 3,
        [Description("不满意")]
        不满意=4
    }

    public class CurrqueuehistModel
    {
        [Required(ErrorMessage = "不能为空！")]
        public string CHQUEUE_TRANSCODEID { get; set; }

        [Required(ErrorMessage = "不能为空！")]
        public string CHQUEUE_SYSNO { get; set; }

        public string HALL_NAM { get; set; }

        [Required(ErrorMessage = "不能为空！")]
        public int CHQUEUE_COUNTER { get; set; }

        [Required(ErrorMessage = "不能为空！")]
        public string CHQUEUE_QSERIALID { get; set; }

        public string Q_SERIALNAME { get; set; }

        [Required(ErrorMessage = "不能为空！")]
        public string CHQUEUE_SNO { get; set; }

        public string STAFF_NAM { get; set; }

        public DateTime? CHQUEUE_ETIME { get; set; }

        [Required(ErrorMessage = "不能为空！")]
        [RegularExpression(@"^\d+$", ErrorMessage = @"应为正整数")]
        public int CHQUEUE_PJRESULT { get; set; }

        public string CHQUEUE_NSRSBM { get; set; }

        public string CHQUEUE_NSRMC { get; set; }
    }
}
