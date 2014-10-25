#region << 版 本 注 释 >>
/*
 * ========================================================================
 * Copyright(c) 2004-2015 北京云房数据技术有限责任公司, All Rights Reserved.
 * ========================================================================
*/
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace gzsw.util.Enum
{
    /// <summary>
    ///  
    /// </summary>
    /// <remark>
    ///     <para>    Creator：hcli </para>
    ///     <para>CreatedTime：2014/10/12 17:48:43</para>
    /// </remark>
    public enum AlterTypeEnum
    {
        /// <summary>
        /// 正确
        /// </summary>
        Success = 1,
        /// <summary>
        /// 提示
        /// </summary>
        Info = 4,
        /// <summary>
        /// 警告
        /// </summary>
        Warning = 0,
        /// <summary>
        /// 错误
        /// </summary>
        Error = 3
    }
}
