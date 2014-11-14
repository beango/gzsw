using gzsw.model;
using gzsw.util;
using PetaPoco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace gzsw.dal.dao
{
    public class WARN_INFO_DETAIL_DAL
    {
        /// <summary>
        /// 处理预警状态
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool HandlerState(model.WARN_INFO_DETAIL model)
        {
            try
            {
                Database db = gzswDB.GetInstance();
                db.Update("WARN_INFO_DETAIL", "WARN_INFO_DETAIL_ID", model, new[] { "STATE", "HANDLE_USER", "HANDLE_TIME", "WARN_METHOD", "WARN_REASON" });
                return true;
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("WARN_INFO_DETAIL_DAL.HandlerState", ex);
                return false;
            }
        }

        /// <summary>
        /// 修改为已读状态
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool UpdateSendDetailReadState(int id)
        {
            try
            {
                Database db = gzswDB.GetInstance();
                db.Update<WARN_SENDINFO_DETAIL>("set CLI_READ_IND=1 where SENDINFO_DETAIL_ID=@0", id);
                return true;
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("WARN_INFO_DETAIL_DAL.UpdateSendDetailReadState", ex);
                return false;
            }
        }
    }
}
