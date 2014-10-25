using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace gzsw.controller.CHK
{
    /// <summary>
    /// 窗口排班信息
    /// </summary>
    public class CounterModel
    {
        public CounterModel()
        {
            W1A_STAFF_ID = "-1";
            W1P_STAFF_ID = "-1";
            W2A_STAFF_ID = "-1";
            W2P_STAFF_ID = "-1";
            W3A_STAFF_ID = "-1";
            W3P_STAFF_ID = "-1";
            W4A_STAFF_ID = "-1";
            W4P_STAFF_ID = "-1";
            W5A_STAFF_ID = "-1";
            W5P_STAFF_ID = "-1";
            W6A_STAFF_ID = "-1";
            W6P_STAFF_ID = "-1";
            W7A_STAFF_ID = "-1";
            W7P_STAFF_ID = "-1";
        }


        [Required(ErrorMessage = "服务厅不能不空.")]

        public string HallNo { get; set; }

        public string HallName { get; set; }

        [Required(ErrorMessage = "窗口不能为空.")]
        [Remote("ValidateCounterId", "Counter", AdditionalFields = "HallNo", ErrorMessage = "该窗口已排班.")]
        public int CounterId { get; set; }

        public string Note { get; set; }

        [Required]
        public string W1A_STAFF_ID { get; set; }

        [Required]
        public string W1P_STAFF_ID { get; set; }

        [Required]
        public string W2A_STAFF_ID { get; set; }

        [Required]
        public string W2P_STAFF_ID { get; set; }

        [Required]
        public string W3A_STAFF_ID { get; set; }

        [Required]
        public string W3P_STAFF_ID { get; set; }

        [Required]
        public string W4A_STAFF_ID { get; set; }

        [Required]
        public string W4P_STAFF_ID { get; set; }

        [Required]
        public string W5A_STAFF_ID { get; set; }

        [Required]
        public string W5P_STAFF_ID { get; set; }

        [Required]
        public string W6A_STAFF_ID { get; set; }

        [Required]
        public string W6P_STAFF_ID { get; set; }

        [Required]
        public string W7A_STAFF_ID { get; set; }

        [Required]
        public string W7P_STAFF_ID { get; set; }
    }

    public class CunterEditModel
    {
        public CunterEditModel()
        {
            W1A_STAFF_ID = "-1";
            W1P_STAFF_ID = "-1";
            W2A_STAFF_ID = "-1";
            W2P_STAFF_ID = "-1";
            W3A_STAFF_ID = "-1";
            W3P_STAFF_ID = "-1";
            W4A_STAFF_ID = "-1";
            W4P_STAFF_ID = "-1";
            W5A_STAFF_ID = "-1";
            W5P_STAFF_ID = "-1";
            W6A_STAFF_ID = "-1";
            W6P_STAFF_ID = "-1";
            W7A_STAFF_ID = "-1";
            W7P_STAFF_ID = "-1";
        }


        [Required(ErrorMessage = "服务厅不能不空.")]

        public string HallNo { get; set; }

        public string HallName { get; set; }

        [Required(ErrorMessage = "窗口不能为空.")]
        public int CounterId { get; set; }

        public string Note { get; set; }

        [Required]
        public string W1A_STAFF_ID { get; set; }

        [Required]
        public string W1P_STAFF_ID { get; set; }

        [Required]
        public string W2A_STAFF_ID { get; set; }

        [Required]
        public string W2P_STAFF_ID { get; set; }

        [Required]
        public string W3A_STAFF_ID { get; set; }

        [Required]
        public string W3P_STAFF_ID { get; set; }

        [Required]
        public string W4A_STAFF_ID { get; set; }

        [Required]
        public string W4P_STAFF_ID { get; set; }

        [Required]
        public string W5A_STAFF_ID { get; set; }

        [Required]
        public string W5P_STAFF_ID { get; set; }

        [Required]
        public string W6A_STAFF_ID { get; set; }

        [Required]
        public string W6P_STAFF_ID { get; set; }

        [Required]
        public string W7A_STAFF_ID { get; set; }

        [Required]
        public string W7P_STAFF_ID { get; set; }
    }

    public class CunterViewModel
    {
        public string HallNo { get; set; }

        public string HallName { get; set; }

        public int CounterId { get; set; }

        public string Note { get; set; }

        public string W1A_STAFF_NAME { get; set; }

        public string W1P_STAFF_NAME { get; set; }

        public string W2A_STAFF_NAME { get; set; }

        public string W2P_STAFF_NAME { get; set; }

        public string W3A_STAFF_NAME { get; set; }

        public string W3P_STAFF_NAME { get; set; }

        public string W4A_STAFF_NAME { get; set; }

        public string W4P_STAFF_NAME { get; set; }

        public string W5A_STAFF_NAME { get; set; }

        public string W5P_STAFF_NAME { get; set; }

        public string W6A_STAFF_NAME { get; set; }

        public string W6P_STAFF_NAME { get; set; }

        public string W7A_STAFF_NAME { get; set; }

        public string W7P_STAFF_NAME { get; set; }
    }
}
