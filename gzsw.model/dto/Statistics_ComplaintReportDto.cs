using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PetaPoco;

namespace gzsw.model.dto
{
    /// <summary>
    /// 投诉举报分析-服务厅
    /// </summary>
    public class Statistics_ComplaintReportDto
    {
        /// <summary>
        /// 服务厅编码
        /// </summary>
        [ResultColumn]
        public string HALL_NO { get; set; }

        /// <summary>
        /// 服务厅
        /// </summary>
        [ResultColumn]
        public string HALL_NAM { get; set; }

        /// <summary>
        /// 投诉类型ID
        /// </summary>
        [ResultColumn]
        public int COMPLAIN_TYP_ID { get; set; }

        /// <summary>
        /// 投诉类型名
        /// </summary>
        [ResultColumn]
        public string COMPLAIN_NAM { get; set; }

        /// <summary>
        /// 处理级别（N、R、U、H）
        /// </summary>
        [ResultColumn]
        public string L_Code { get; set; }

        /// <summary>
        /// 处理级别（未处理、已处理、撤销、合计）
        /// </summary>
        [ResultColumn]
        public string L_Name { get; set; }
        
        /// <summary>
        /// 处理级别（1、2、3、0）
        /// </summary>
        [ResultColumn]
        public int L_Value { get; set; }
        
        /// <summary>
        /// 数量值
        /// </summary>
        [ResultColumn]
        public int Count_Value { get; set; }
    }

    /// <summary>
    /// 投诉举报分析-员工
    /// </summary>
    public class Statistics_ComplaintReportPersonalDto
    {
        /// <summary>
        /// 员工编号
        /// </summary>
        [ResultColumn]
        public string STAFF_ID { get; set; }

        /// <summary>
        /// 员工姓名
        /// </summary>
        [ResultColumn]
        public string STAFF_NAM { get; set; }

        /// <summary>
        /// 投诉类型ID
        /// </summary>
        [ResultColumn]
        public int COMPLAIN_TYP_ID { get; set; }

        /// <summary>
        /// 投诉类型名
        /// </summary>
        [ResultColumn]
        public string COMPLAIN_NAM { get; set; }

        /// <summary>
        /// 处理级别（N、R、U、H）
        /// </summary>
        [ResultColumn]
        public string L_Code { get; set; }

        /// <summary>
        /// 处理级别（未处理、已处理、撤销、合计）
        /// </summary>
        [ResultColumn]
        public string L_Name { get; set; }

        /// <summary>
        /// 处理级别（1、2、3、0）
        /// </summary>
        [ResultColumn]
        public int L_Value { get; set; }

        /// <summary>
        /// 数量值
        /// </summary>
        [ResultColumn]
        public int Count_Value { get; set; }
    }
}
