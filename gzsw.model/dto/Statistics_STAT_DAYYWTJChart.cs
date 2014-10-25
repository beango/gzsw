 

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
    ///     <para>CreatedTime：2014/10/24 1:27:21</para>
    /// </remark>
    public class Statistics_STAT_DAYYWTJChart
    {
        /// <summary>
        /// 员工编号
        /// </summary>
        public string QDDTJ_SNO { get; set; }

        /// <summary>
        /// 员工名称
        /// </summary>
        public string STAFF_NAM { get; set; }

        /// <summary>
        /// 未评级量
        /// </summary>
        public int QDDTJ_PJKEY0NUM { get; set; }

        /// <summary>
        /// 很满意次数
        /// </summary>
        public int QDDTJ_PJKEY1NUM { get; set; }

        /// <summary>
        /// 满意次数
        /// </summary>
        public int QDDTJ_PJKEY2NUM { get; set; }

        /// <summary>
        /// 一般次数
        /// </summary>
        public int QDDTJ_PJKEY3NUM { get; set; }


        /// <summary>
        /// 不满意次数
        /// </summary>
        public int QDDTJ_PJKEY4NUM { get; set; }


        /// <summary>
        /// 很不满意次数  
        /// </summary>
        public int QDDTJ_PJKEY5NUM { get; set; } 
    }
}
