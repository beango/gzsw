 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace gzsw.model.dto
{
    /// <summary>
    ///  个人满意度
    /// </summary>
    /// <remark>
    ///     <para>    Creator：hcli </para>
    ///     <para>CreatedTime：2014/10/24 1:14:16</para>
    /// </remark>
    public class Statistics_STAT_DAYYWTJ
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
        /// 未评级次数
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



        /// <summary>
        /// 未评级占比
        /// </summary>
        public decimal KEY0RT { get; set; }


        /// <summary>
        /// 很满意占比
        /// </summary>
        public decimal KEY1RT { get; set; }

        /// <summary>
        /// 满意占比
        /// </summary>
        public decimal KEY2RT { get; set; }

        /// <summary>
        /// 一般占比
        /// </summary>
        public decimal KEY3RT { get; set; }

        /// <summary>
        /// 不满意占比
        /// </summary>
        public decimal KEY4RT { get; set; }

        /// <summary>
        /// 很不满意占比
        /// </summary>
        public decimal KEY5RT { get; set; }
    }
}
