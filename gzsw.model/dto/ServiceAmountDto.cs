using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using PetaPoco;

namespace gzsw.model.dto
{
    /// <summary>
    /// 业务量
    /// </summary>
    public class ServiceAmountDto
    {
        /// <summary>
        /// 服务编码
        /// </summary>
        [ResultColumn]
        public string HALL_NO { get; set; }

        /// <summary>
        /// 服务厅名称 
        /// </summary>
        [ResultColumn]
        public string HALL_NAM { get; set; }

        /// <summary>
        /// 业务编码
        /// </summary>
        [ResultColumn]
        public string DLS_SERIALID { get; set; }
        /// <summary>
        /// 业务名称
        /// </summary>
        [ResultColumn]
        public string DLS_SERIALNAME { get; set; }

        /// <summary>
        /// 业务编码
        /// </summary>
        [ResultColumn]
        public string STAFF_ID { get; set; }

        /// <summary>
        /// 员工名称
        /// </summary>
        [ResultColumn]
        public string STAFF_NAM { get; set; }

        /// <summary>
        /// 业务笔数
        /// </summary>
        [ResultColumn]
        public int BUSI_CNT { get; set; }

        /// <summary>
        /// 业务折合量
        /// </summary>
        [ResultColumn]
        public decimal CONVERT_BUSI_CNT { get; set; }

        /// <summary>
        /// 总办理时长
        /// 单位：秒；展现到页面时需转化为 分钟  
        /// 平均办理时长＝总办理时长／总办理量
        /// </summary>
        [ResultColumn]
        public int HANDLE_DUR { get; set; }

        /// <summary>
        /// 平均办理时长
        /// </summary>
        public int AverageHANDLE {
            get { return BUSI_CNT==0?0:HANDLE_DUR/BUSI_CNT; }
        }

        /// <summary>
        /// 超时办理量
        /// </summary>
        [ResultColumn]
        public int OVERTIME_HANDLE_CNT { get; set; }

        /// <summary>
        /// 超时率
        /// </summary>
        public decimal TimeoutRate{
            get { return BUSI_CNT == 0 ? 0 : (decimal)OVERTIME_HANDLE_CNT / BUSI_CNT; }
        }

        /// <summary>
        /// 同城办理率
        /// </summary>
        public decimal CityRate
        {
            get
            {
                return BUSI_CNT == 0 ? 0 : (decimal)LOCAL_CNT / BUSI_CNT; 
            }
        }

        /// <summary>
        /// 超时办理量
        /// </summary>
        [ResultColumn]
        public int LOCAL_CNT { get; set; }
    }
}
