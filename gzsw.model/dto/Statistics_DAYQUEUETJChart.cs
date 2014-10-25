
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace gzsw.model.dto
{
    /// <summary>
    ///  排队信息统计日报表 图形
    /// </summary>
    /// <remark>
    ///     <para>    Creator：hcli </para>
    ///     <para>CreatedTime：2014/10/23 23:01:48</para>
    /// </remark>
    public class Statistics_DAYQUEUETJChart
    {
        /// <summary>
        /// 员工名称
        /// </summary>
        public string STAFF_NAM { get; set; }

        /// <summary>
        /// 呼叫量
        /// </summary>
        public int PersonCount { get; set; }

        /// <summary>
        /// 办理量
        /// </summary>
        public int STAFF_QDDTJ_RLL { get; set; }
       
        /// <summary>
        /// 弃号量
        /// </summary>
        public int STAFF_QDDTJ_QHNUM { get; set; }
 
        /// <summary>
        /// 组织机构名称
        /// </summary>
        public string ORG_NAM { get; set; }

    }
}
