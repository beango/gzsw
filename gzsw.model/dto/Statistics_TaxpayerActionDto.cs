
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PetaPoco;

namespace gzsw.model.dto
{
    /// <summary>
    ///  纳税人行为分析Dto
    /// </summary>
    /// <remark>
    ///     <para>    Creator：hcli </para>
    ///     <para>CreatedTime：2014/11/9 11:42:27</para>
    /// </remark>
    public class Statistics_TaxpayerActionDto
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
        /// 同城业务量
        /// </summary>
        [ResultColumn]
        public int LOCAL_CNT { get; set; }


        /// <summary>
        /// 二次办税业务量
        /// </summary>
        [ResultColumn]
        public int SECOND_SVR_CNT { get; set; }


    }
}
