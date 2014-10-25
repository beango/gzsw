
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace gzsw.model.dto
{
    /// <summary>
    ///  排队信息统计日报表 模型
    /// </summary>
    /// <remark>
    ///     <para>    Creator：hcli </para>
    ///     <para>CreatedTime：2014/10/23 19:21:46</para>
    /// </remark>
    public class Statistics_DAYQUEUETJ
    {
        /// <summary>
        /// 员工编码
        /// </summary>
        public string QDDTJ_SNO { get; set; }
         

        /// <summary>
        /// 日期
        /// </summary>
        public DateTime QDDTJ_DAY { get; set; }

        /// <summary>
        /// 员工名称
        /// </summary>
        public string STAFF_NAM { get; set; }

        /// <summary>
        /// 呼叫量
        /// </summary>
        public int STAFF_QDDTJ_TICKETNUM { get; set; }

        /// <summary>
        /// 办理量
        /// </summary>
        public int STAFF_QDDTJ_RLL { get; set; }

        /// <summary>
        /// 弃号量
        /// </summary>
        public int STAFF_QDDTJ_QHNUM { get; set; }


        /// <summary>
        /// 呼叫量占比
        /// </summary>
        public decimal DoAllBFB { get; set; }

        /// <summary>
        /// 办理量占比
        /// </summary>
        public decimal NoDoBFB { get; set; }

        /// <summary>
        /// 弃号量占比
        /// </summary>
        public decimal NoDoCountBFB { get; set; }
    }
}
