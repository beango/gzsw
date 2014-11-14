 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace gzsw.model.dto
{
    /// <summary>
    ///  事项大类 
    /// </summary>
    /// <remark>
    ///     <para>    Creator：hcli </para>
    ///     <para>CreatedTime：2014/11/9 12:59:46</para>
    /// </remark>
    public class ItemBigTypeViewModel
    {
        /// <summary>
        /// 事项大类名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 事项大类ID
        /// </summary>
        public string No { get; set; }

        /// <summary>
        /// 值
        /// </summary>
        public string Value { get; set; }
    }
}
