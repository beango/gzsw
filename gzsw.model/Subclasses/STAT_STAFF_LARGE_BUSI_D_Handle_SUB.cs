using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PetaPoco;

namespace gzsw.model.Subclasses
{
    /// <summary>
    /// 业务办理分析类
    /// </summary>
    public class STAT_STAFF_LARGE_BUSI_D_Handle_SUB : STAT_STAFF_LARGE_BUSI_D
    {
        [ResultColumn]
        public string STAFF_NAM { get; set; }

        /// <summary>
        /// 业务大类名称
        /// </summary>
        [ResultColumn]
        public string DLS_SERIALNAME { get; set; }

        /// <summary>
        /// 服务厅名
        /// </summary>
        [ResultColumn]
        public string HALL_NAM { get; set; }

        /// <summary>
        /// 平均办理时长
        /// </summary>
        public int AverageHANDLE
        {
            get { return BUSI_CNT == 0 ? 0 : HANDLE_DUR / BUSI_CNT; }
        }

        /// <summary>
        /// 超时率
        /// </summary>
        public decimal TimeoutRate
        {
            get { return BUSI_CNT == 0 ? 0 : (decimal)OVERTIME_HANDLE_CNT / (decimal)BUSI_CNT; }
        }

        /// <summary>
        /// 同城办理率
        /// </summary>
        public decimal CityRate
        {
            get
            {
                return BUSI_CNT == 0 ? 0 : (decimal)LOCAL_CNT / (decimal)BUSI_CNT;
            }
        }
    }
}
