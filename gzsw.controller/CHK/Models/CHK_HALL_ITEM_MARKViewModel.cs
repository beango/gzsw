using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace gzsw.controller.CHK.Models
{
    public class CHK_HALL_ITEM_MARKViewModel
    {

         
        [Required(ErrorMessage = "不能为空！")]
        public int SEQ { get; set; }




         
        [Display(Name = @"1:环境建设
   2:制度建设
   3:规范建设
   4:行风建设
   5:第三方调查
   6:其他")]

        [Required(ErrorMessage = "不能为空！")]
        public byte CHKITEM_TYP { get; set; }




         
        [Display(Name = @"服务大厅信息表SYS_HALL")]

        [Required(ErrorMessage = "不能为空！")]
        public string HALL_NO { get; set; }




         

        public string HALL_CHKITEM_CD { get; set; }




         
        [Required(ErrorMessage = "不能为空！")]
        public decimal DEDUCT { get; set; }




         
        [Display(Name = @"对应用户表SYS_USER的USER_ID")]

        [Required(ErrorMessage = "不能为空！")]
        public string MODIFY_ID { get; set; }




         
        [Required(ErrorMessage = "不能为空！")]
        public DateTime MODIFY_DTIME { get; set; }




         
        [Display(Name = @"手工输入")]

        [Required(ErrorMessage = "不能为空！")]
        public string REASON { get; set; }




         
        [Display(Name = @"录入时的时间，系统生成")]

        [Required(ErrorMessage = "不能为空！")]
        public DateTime MARK_TIME { get; set; }
    }
}
