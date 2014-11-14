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
using gzsw.util;
using PetaPoco;
using gzsw.model;

namespace gzsw.dal.dao
{
    /// <summary>
    ///  考核质量差错类型
    /// </summary>
    /// <remark>
    ///     <para>    Creator：hcli </para>
    ///     <para>CreatedTime：2014/11/2 11:46:20</para>
    /// </remark>
    public class CHK_QUALITY_CON_DAL
    {

        /// <summary>
        ///  删除考核质量差错类型
        /// </summary>
        /// <param name="typeIds"></param>
        public bool Delete(params string[] typeIds)
        {
            var ids = typeIds.Aggregate(string.Empty, (current, i) => current + ("'" + i.GetSqlCheckStr() + "',"));
            ids = ids.TrimEnd(',');
            try
            {
                var db = gzswDB.GetInstance();
                db.BeginTransaction();
                db.Execute(string.Format("delete CHK_QUALITY_CON where QUALITY_CD in ({0});", ids)); 
                db.CompleteTransaction();
                return true;
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("Add CHK_QUALITY_CON Error", ex);
                return false;
            }
        }
    }
}
