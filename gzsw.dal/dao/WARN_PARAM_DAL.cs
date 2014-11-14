using gzsw.model;
using gzsw.util;
using PetaPoco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace gzsw.dal.dao
{
    public class WARN_PARAM_DAL
    {
        public bool AddList(List<model.WARN_PARAM> paralist)
        {
            try
            {
                var db = gzswDB.GetInstance();
                db.BeginTransaction();
                var exists = db.Fetch<model.WARN_PARAM>("From WARN_PARAM where HALL_NO=@0", paralist.FirstOrDefault().HALL_NO);
                foreach (var para in paralist)
                {
                    if (exists.Any(obj => obj.WARN_TYP == para.WARN_TYP && obj.WARN_LEVEL == para.WARN_LEVEL))
                        db.Update<model.WARN_PARAM>(@"SET [CRITICAL_VALUE] = @0
                                          ,[WARN_INFO_MODEL] = @1
                                          ,[MODIFY_ID] = @2
                                          ,[MODIFY_DTIME] =@3
                                          ,[FRE_MIN] = @4
                                     WHERE HALL_NO=@5 and WARN_TYP=@6 and WARN_LEVEL=@7"
                            , para.CRITICAL_VALUE
                            , para.WARN_INFO_MODEL
                            , para.MODIFY_ID
                            , para.MODIFY_DTIME
                            , para.FRE_MIN
                            , para.HALL_NO
                            , para.WARN_TYP
                            , para.WARN_LEVEL);
                    else
                        db.Insert(para);
                }
                db.CompleteTransaction();
                return true;
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("WARN_PARAM_DAL.AddList", ex);
                return false;
            }
        }

        public bool AddParamUserList(List<model.WARN_PARAM_SEND_USER_CON> models)
        {
            try
            {
                var db = gzswDB.GetInstance();
                db.BeginTransaction();
                var exists = db.Execute("Delete WARN_PARAM_SEND_USER_CON where HALL_NO=@0 and WARN_TYP=@1", models.FirstOrDefault().HALL_NO,models.FirstOrDefault().WARN_TYP);
                foreach (var model in models)
                {
                    db.Insert(model);
                }
                db.CompleteTransaction();
                return true;
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("WARN_PARAM_DAL.AddParamUserList", ex);
                return false;
            }
        }
    }
}
