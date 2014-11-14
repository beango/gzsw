using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace gzsw.controller.CHK.Models
{
    public class CHK_HALL_CHKITEM_CON_VIEW
    {
         
        public string OLD_HALL_CHKITEM_CD { get; set; }


        [Required(ErrorMessage = "不能为空！")]
        [Remote("CheckCode", "HALLchkitemcon", AdditionalFields = "HALL_CHKITEM_CD", ErrorMessage = "该服务厅考核指标编码已存在.")]
        public string HALL_CHKITEM_CD { get; set; }




        [Required(ErrorMessage = "不能为空！")]
        public string HALL_CHKITEM_NAM { get; set; }




        [Required(ErrorMessage = "不能为空！")]
        public string MODIFY_ID { get; set; }




        [Required(ErrorMessage = "不能为空！")]
        public DateTime MODIFY_DTIME { get; set; }




        [Display(Name = @"1:环境建设
   2:制度建设
   3:规范建设
   4:行风建设
   5:第三方调查
   6:其他")]

        [Required(ErrorMessage = "不能为空！")]
        public byte CHKITEM_TYP { get; set; }

    }
}
