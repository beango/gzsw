 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PetaPoco;

namespace gzsw.model.dto
{
    /// <summary>
    ///  业务差错分析Dto
    /// </summary>
    /// <remark>
    ///     <para>    Creator：hcli </para>
    ///     <para>CreatedTime：2014/11/9 12:33:45</para>
    /// </remark>
    public class ServiceSlipAnalysisViewModel
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public ServiceSlipAnalysisViewModel()
        {
            Items = new List<QualityViewModel>();
        }

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
        /// 质量类型
        /// </summary> 
        [ResultColumn]
        public IList<QualityViewModel> Items { get; set; }

    }
}
