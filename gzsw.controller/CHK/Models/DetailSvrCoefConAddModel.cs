using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace gzsw.controller.CHK.Models
{
    /// <summary>
    /// 明细业务系统
    /// </summary>
    public class DetailSvrCoefConAddModel
    {

        [Required(ErrorMessage = "不能为空！")]
        [Remote("ValidateSerialId", "DetailSvrCoefCon", AdditionalFields = "ORG_ID", ErrorMessage = "该业务事项已设置.")]
        public string SERIALID { get; set; }

        [Required(ErrorMessage = "不能为空！")]
        public string ORG_ID { get; set; }

        [Required(ErrorMessage = "不能为空！")]
        [RegularExpression(@"^\d+(\.\d+)?$", ErrorMessage = @"应为非负数字")]
        public decimal COEFFICIENT { get; set; }
    }

    public class DetailSvrCoefConOneAddModel
    {
        [Required(ErrorMessage = "不能为空！")]
        public string ORG_ID { get; set; }

        [Required(ErrorMessage = "不能为空！")]
        [RegularExpression(@"^\d+(\.\d+)?$", ErrorMessage = @"应为非负数字")]
        public decimal COEFFICIENT { get; set; }
    }

    public class DetailSvrCoefConModel
    {
        [Required(ErrorMessage = "不能为空！")]
        public string SERIALID { get; set; }

        public string SERIALNAME { get; set; }

        [Required(ErrorMessage = "不能为空！")]
        public string ORG_ID { get; set; }

        public string ORG_NAM { get; set; }

        [Required(ErrorMessage = "不能为空！")]
        [RegularExpression(@"^\d+(\.\d+)?$", ErrorMessage = @"应为非负数字")]
        public decimal COEFFICIENT { get; set; }

        public string USER_NAM { get; set; }

        public DateTime MODIFY_DTIME { get; set; }

    }
}
