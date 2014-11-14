
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PetaPoco;

namespace gzsw.model.dto
{
    /// <summary>
    ///  个人业务统计
    /// </summary>
    /// <remark>
    ///     <para>    Creator：hcli </para>
    ///     <para>CreatedTime：2014/11/8 0:23:35</para>
    /// </remark>
    public class Statistics_TaxpayerEvalDto
    {
        /// <summary>
        /// 营业厅编码
        /// </summary>
        [ResultColumn]
        public string HALL_NO { get; set; }

        /// <summary>
        /// 营业厅名称
        /// </summary> 
        [ResultColumn]
        public string ORG_NAM { get; set; }


        /// <summary>
        /// 员工号
        /// </summary> 
        [ResultColumn]
        public string PersonNo { get; set; }


        /// <summary>
        /// 员工名称
        /// </summary> 
        [ResultColumn]
        public string PersonName { get; set; }

        /// <summary>
        /// 很满意量
        /// </summary> 
        [ResultColumn]
        public long VERY_SATISFY_CNT { get; set; }


        /// <summary>
        /// 很满意度
        /// </summary> 
        [ResultColumn]
        public decimal VERY_SATISFY_CNT_BFB { get; set; }

        /// <summary>
        /// 满意量
        /// </summary> 
        [ResultColumn]
        public long SATISFY_CNT { get; set; }

        /// <summary>
        /// 满意度
        /// </summary> 
        [ResultColumn]
        public decimal SATISFY_CNT_BFB { get; set; }


        /// <summary>
        /// 一般量
        /// </summary> 
        [ResultColumn]
        public long COMMON_CNT { get; set; }


        /// <summary>
        /// 一般度
        /// </summary> 
        [ResultColumn]
        public decimal COMMON_CNT_BFB { get; set; }


        /// <summary>
        /// 不满意
        /// </summary> 
        [ResultColumn]
        public long UNSATISFY_CNT { get; set; }



        /// <summary>
        /// 不满意度
        /// </summary> 
        [ResultColumn]
        public decimal UNSATISFY_CNT_BFB { get; set; }



        /// <summary>
        /// 未评价量
        /// </summary> 
        [ResultColumn]
        public long NON_EVAL_CNT { get; set; }


        /// <summary>
        /// 未评价度
        /// </summary> 
        [ResultColumn]
        public decimal NON_EVAL_CNT_BFB { get; set; }


        /// <summary>
        /// 满意度
        /// </summary> 
        [ResultColumn]
        public decimal ManYiDu_BFB  { get; set; }
    }
}
