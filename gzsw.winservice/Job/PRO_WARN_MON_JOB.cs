using gzsw.model;
using gzsw.util;
using PetaPoco;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gzsw.winservice.Job
{
    [DisallowConcurrentExecution]
    public class PRO_WARN_MON_JOB : IJob
    {
        #region IJob 成员

        public void Execute(IJobExecutionContext context)
        {
            try
            {
                LogHelper.WriteLog("PRO_WARN_MON_JOB");
                var db = gzswDB.GetInstance();

                model.SVR_TIM_EVENT e = db.SingleOrDefault<model.SVR_TIM_EVENT>("From SVR_TIM_EVENT where PROGRAM_METHOD='PRO_WARN_MON'");
                if (null != e)
                {
                    SVR_TIM_EVENT_LOG log = new SVR_TIM_EVENT_LOG();
                    log.EVENT_GUID = e.EVENT_GUID;
                    log.RUN_TIME = DateTime.Now;
                    log.RUN_STATE = 2;
                    log.PAR_INFO = "";
                    log.ERROR_INFO = "";

                    try
                    {
                        db.Execute("exec PRO_WARN_MON");
                        log.RUN_STATE = 1;
                    }
                    catch (Exception ex)
                    {
                        log.ERROR_INFO = ex.Message;
                        log.RUN_STATE = 2;
                    }
                    db.Insert("SVR_TIM_EVENT_LOG", "SEQ", log);//写日志
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("PRO_WARN_MON_JOB.Execute", ex);
            }
        }

        #endregion
    }
}
