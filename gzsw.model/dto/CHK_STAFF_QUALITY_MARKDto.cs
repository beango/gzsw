 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace gzsw.model.dto
{
    /// <summary>
    ///  
    /// </summary>
    /// <remark>
    ///     <para>    Creator：hcli </para>
    ///     <para>CreatedTime：2014/11/2 18:49:59</para>
    /// </remark>
    public class CHK_STAFF_QUALITY_MARKDto
    { 
        /// <summary>
        /// Id
        /// </summary>
        public int SEQ { get; set; }
         
        /// <summary>
        /// 员工编码
        /// </summary>
        public string STAFF_ID { get; set; }

        /// <summary>
        /// 营业厅编码
        /// </summary>
        public string ORG_ID { get; set; }

        /// <summary>
        /// 员工名称
        /// </summary>
        public string PersonName { get; set; }

        /// <summary>
        /// 事项大类编码
        /// </summary>
        public string SSDLSERIALID { get; set; }

        /// <summary>
        /// 事项编码
        /// </summary>
        public string SERIALID { get; set; }

        /// <summary>
        /// 事项名称
        /// </summary>
        public string SERIAL_Name { get; set; }
         
        /// <summary>
        /// 质量类型编码
        /// </summary>
        public string QUALITY_CD { get; set; }

        /// <summary>
        /// 质量类型名 
        /// </summary>
        public string QUALITY_Name { get; set; }
         
        /// <summary>
        /// 数量
        /// </summary>
        public int AMOUNT { get; set; }
         
        /// <summary>
        /// 差错发生日期
        /// </summary>
        public DateTime? OCCUR_DT { get; set; }
         
        /// <summary>
        /// 修改人ID
        /// </summary>
        public string MODIFY_ID { get; set; }

        /// <summary>
        /// 修改人名
        /// </summary>
        public string MODIFY_Name { get; set; }
          
        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime? MODIFY_DTIME { get; set; }
    }
}
