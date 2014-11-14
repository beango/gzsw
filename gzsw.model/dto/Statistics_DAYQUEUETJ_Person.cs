 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace gzsw.model.dto
{
    /// <summary>
    ///  个人业务信息
    /// </summary>
    /// <remark>
    ///     <para>    Creator：hcli </para>
    ///     <para>CreatedTime：2014/10/25 17:25:38</para>
    /// </remark>
    public class Statistics_DAYQUEUETJ_Person
    {
        /// <summary>
        /// 员工编码
        /// </summary>
        public string QDDTJ_SNO { get; set; }


        /// <summary>
        /// 员工姓名
        /// </summary>
        public string STAFF_NAM { get; set; }

        /// <summary>
        /// 业务编码
        /// </summary>
        public string QDDTJ_QSERIALID { get; set; }

        /// <summary>
        /// 业务名称
        /// </summary>
        public string Q_SERIALNAME { get; set; }

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
    }
}
