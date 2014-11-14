
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PetaPoco;

namespace gzsw.model.dto
{
    /// <summary>
    ///  业务差错分析
    /// </summary>
    /// <remark>
    ///     <para>    Creator：hcli </para>
    ///     <para>CreatedTime：2014/11/9 13:38:18</para>
    /// </remark>
    public class ServiceSlipAnalysisDto
    {

        /// <summary>
        /// 时间
        /// </summary>
        [ResultColumn]
        public string STAT_DT { get; set; }

        /// <summary>
        /// 服务厅编码
        /// </summary>
        [ResultColumn]
        public string HALL_NO { get; set; }

        /// <summary>
        /// 服务厅名称
        /// </summary>
        [ResultColumn]
        public string HALL_NAM { get; set; }

        /// <summary>
        /// 业务大类编码
        /// </summary> 
        [ResultColumn]
        public string SSDLSERIALID { get; set; }

        /// <summary>
        /// 差错评价编码
        /// </summary> 
        [ResultColumn]
        public string QUALITY_CD { get; set; }
         
        /// <summary>
        /// 差错质量名称
        /// </summary>  
        [ResultColumn]
        public string QUALITY_NAM { get; set; }

        /// <summary>
        /// 业务大类名称
        /// </summary> 
        [ResultColumn]
        public string DLS_SERIALNAME { get; set; }
 

        /// <summary>
        /// 差错数量
        /// </summary>
        [ResultColumn]
        public int AMOUNT { get; set; }
 
    }
}
