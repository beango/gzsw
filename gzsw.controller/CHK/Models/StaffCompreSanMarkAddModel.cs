using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace gzsw.controller.CHK.Models
{
    public enum COMPRESANTYPEENUM
    {
        [Description("综合评议分")]
        综合评议分=1,

        [Description("额外奖分")]
        额外奖分=2,

        [Description("额外罚分")]
        额外罚分=3,

        [Description("投诉扣分")]
        投诉扣分=4
    }

    public class StaffCompreSanMarkAddModel
    {
        [Required(ErrorMessage = "不能为空！")]
        public string ORG_ID { get; set; }

        public int SEQ { get; set; }

        [Required(ErrorMessage = "不能为空！")]
        public string STAT_MO { get; set; }
        
        [Required(ErrorMessage = "不能为空！")]
        public string STAFF_ID { get; set; }
        
        [Required(ErrorMessage = "不能为空！")]
        public byte COMPRE_SAN_TYP { get; set; }

        [Required(ErrorMessage = "不能为空！")]
        [RegularExpression(@"^\d+(\.\d+)?$", ErrorMessage = @"应为非负数字")]
        public decimal SCORE { get; set; }

        [Required(ErrorMessage = "不能为空！")]
        public string REASON { get; set; }

    }

    public class StaffCompreSanMarkModel
    {
        [Required(ErrorMessage = "不能为空！")]
        public string ORG_ID { get; set; }

        public string ORG_NAM { get; set; }

        public int SEQ { get; set; }

        [Required(ErrorMessage = "不能为空！")]
        public string STAT_MO { get; set; }

        [Required(ErrorMessage = "不能为空！")]
        public string STAFF_ID { get; set; }

        public string STAFF_NAM { get; set; }

        [Required(ErrorMessage = "不能为空！")]
        public byte COMPRE_SAN_TYP { get; set; }

        [Required(ErrorMessage = "不能为空！")]
        [RegularExpression(@"^\d+(\.\d+)?$", ErrorMessage = @"应为非负数字")]
        public decimal SCORE { get; set; }

        [Required(ErrorMessage = "不能为空！")]
        public string REASON { get; set; }

        public DateTime MARK_TIME { get; set; }
    }
}
