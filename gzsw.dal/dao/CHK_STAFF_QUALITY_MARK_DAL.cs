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
using gzsw.model;
using gzsw.model.dto;
using gzsw.util;
using PetaPoco;

namespace gzsw.dal.dao
{
    /// <summary>
    ///  个人考核质量差错打分表业务逻辑
    /// </summary>
    /// <remark>
    ///     <para>    Creator：hcli </para>
    ///     <para>CreatedTime：2014/11/2 15:31:58</para>
    /// </remark>
    public class CHK_STAFF_QUALITY_MARK_DAL
    {
        public CHK_STAFF_QUALITY_MARKDto Get(int id)
        {
            var db = gzswDB.GetInstance();
            var sql = Sql.Builder.Append(@"select a.*,b.ORG_ID,b.STAFF_NAM as PersonName,c.SERIALNAME as SERIAL_Name, c.SSDLSERIALID, " +
                      "d.QUALITY_NAM as QUALITY_Name, " +
                      "e.USER_NAM as MODIFY_Name " +
                      "From CHK_STAFF_QUALITY_MARK as a left join SYS_STAFF as b  on  " +
                      "a.STAFF_ID  = b.STAFF_ID " +
                      "left join  SYS_DETAILSERIAL  as c on  a.SERIALID = c.SERIALID " +
                      "left join CHK_QUALITY_CON as d on   a.QUALITY_CD = d.QUALITY_CD " +
                      "left join SYS_USER as e on a.MODIFY_ID = e.USER_ID where SEQ = @0 ",id); 
            return db.FirstOrDefault<CHK_STAFF_QUALITY_MARKDto>(sql); 
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="qUALITY_CD">质量类型编码</param>
        /// <param name="sERIALID">事项编码</param>
        /// <param name="sTAFF_ID">员工编码</param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public Page<CHK_STAFF_QUALITY_MARKDto> GetList(
            string qUALITY_CD,
            string sERIALID,
            string sTAFF_ID, 
            int pageIndex,
            int pageSize)
        {
            var db = gzswDB.GetInstance();
            var sql = Sql.Builder.Append(@"select a.*,b.STAFF_NAM as PersonName,c.SERIALNAME as SERIAL_Name,
            d.QUALITY_NAM as QUALITY_Name,
            e.USER_NAM as MODIFY_Name
            From CHK_STAFF_QUALITY_MARK as a left join SYS_STAFF as b  on  
            a.STAFF_ID  = b.STAFF_ID
            left join  SYS_DETAILSERIAL  as c on  a.SERIALID = c.SERIALID
            left join CHK_QUALITY_CON as d on   a.QUALITY_CD = d.QUALITY_CD
            left join SYS_USER as e on a.MODIFY_ID = e.USER_ID  
            where 1=1 ");
            if (!string.IsNullOrEmpty(qUALITY_CD))
            {
                sql.Append(@" AND a.QUALITY_CD like @0 ", "%" + qUALITY_CD + "%");
            }
             if (!string.IsNullOrEmpty(sERIALID))
            {
                sql.Append(@" AND a.SERIALID like @0 ", "%" + sERIALID + "%");
            }
             if (!string.IsNullOrEmpty(sTAFF_ID))
            {
                sql.Append(@" AND a.STAFF_ID like @0 ", "%" + sTAFF_ID + "%");
            }

             return db.Page<CHK_STAFF_QUALITY_MARKDto>(pageIndex, pageSize, sql);
        } 


        /// <summary>
        /// 查询是否存在考核质量差错类型数据
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public bool IsHaveForType(params string[] ids)
        {
            var objId = ids.Aggregate(string.Empty, (current, i) => current + ("'" + i.GetSqlCheckStr() + "',"));
            objId = objId.TrimEnd(',');
            try
            {
                var db = gzswDB.GetInstance();
                var list =   db.Fetch<CHK_STAFF_QUALITY_MARK>(
                    string.Format("select * from CHK_STAFF_QUALITY_MARK where QUALITY_CD in ({0});", ids));
                return list.Any() ? true : false;
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("Add CHK_QUALITY_CON Error", ex);
                return false;
            }
        }


        /// <summary>
        ///  删除用户
        /// </summary>
        /// <param name="ids"></param>
        public bool Delete(params string[] ids)
        {
            var idObjes = ids.Aggregate(string.Empty, (current, i) => current + ("'" + i.GetSqlCheckStr() + "',"));
            idObjes = idObjes.TrimEnd(',');
            try
            {

                var db = gzswDB.GetInstance();
                db.BeginTransaction();
                db.Execute(string.Format("delete CHK_STAFF_QUALITY_MARK where SEQ in ({0});", idObjes)); 
                db.CompleteTransaction();
                return true;
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("Delete CHK_STAFF_QUALITY_MARK Error", ex);
                return false;
            }
        }

        public static object GetListBubByHall(DateTime beginMo, DateTime endMo, string hallno, int pageIndex, int pageSize)
        {
            var db = gzswDB.GetInstance();

            var sql = Sql.Builder.Append(@"
select 
SEQ,
csq.STAFF_ID,
csq.SERIALID,
csq.QUALITY_CD,
AMOUNT,
OCCUR_DT,
csq.MODIFY_ID,
csq.MODIFY_DTIME,
sf.STAFF_NAM,
sd.SERIALNAME,
cqc.QUALITY_NAM,
H.HALL_NAM,
sf.ORG_ID
 from 
CHK_STAFF_QUALITY_MARK csq,
SYS_STAFF sf,SYS_DETAILSERIAL  sd,CHK_QUALITY_CON cqc,
SYS_HALL H
where csq.STAFF_ID=sf.STAFF_ID 
and csq.SERIALID=sd.SERIALID
and csq.QUALITY_CD=cqc.QUALITY_CD 
and sf.ORG_ID=H.HALL_NO");

            sql.Append(@" AND sf.ORG_ID=@0 ", hallno);

            sql.Append(@" AND csq.OCCUR_DT>=@0 AND csq.OCCUR_DT<=@1 ", beginMo, endMo);

            var data = db.Page<dynamic>(pageIndex, pageSize, sql);
            data.ItemsPerPage = pageSize;
            return data;
        }
    }
}
