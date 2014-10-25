using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace gzsw.controller.CHK
{
    public class ApplyDetailModel
    {
        [Required(ErrorMessage = "组织不能不空.")]
        public string ORG_ID { get; set; }

        [Required(ErrorMessage = "员工不能不空.")]
        public string CHK_STAFF_ID { get; set; }

        [Required(ErrorMessage = "开始时间不能不空.")]
        public DateTime BeginTime { get; set; }

        public int BeginTimeHours { get; set; }

        [Required(ErrorMessage = "结束时间不能不空.")]
        public DateTime EndTime { get; set; }

        public int EndTimeHours { get; set; }

        [Required(ErrorMessage = "假期类型不能不空.")]
        public int HOLLI_TYP { get; set; }

        public string APPLY_REASON { get; set; }
    }
}
