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
    public class PRO_MON_COMPLETE_OVERTIME_CNT_JOB : IJob
    {
        #region IJob 成员

        public void Execute(IJobExecutionContext context)
        {
            try
            {
                LogHelper.WriteLog("PRO_MON_COMPLETE_OVERTIME_CNT_JOB");
                var db = gzswDB.GetInstance();

                model.SVR_TIM_EVENT e = db.SingleOrDefault<model.SVR_TIM_EVENT>("select * from SVR_TIM_EVENT where PROGRAM_METHOD='PRO_MON_COMPLETE_OVERTIME_CNT'");
                if (null != e)
                {
                    SVR_TIM_EVENT_LOG log = new SVR_TIM_EVENT_LOG();
                    log.EVENT_GUID = e.EVENT_GUID;
                    log.RUN_TIME = DateTime.Now;
                    log.RUN_STATE = 1;
                    log.PAR_INFO = "";
                    log.ERROR_INFO = "";
                    ExecuteProd(log);
                    db.Insert("SVR_TIM_EVENT_LOG", "SEQ", log);//写日志
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("PRO_MON_COMPLETE_OVERTIME_CNT_JOB.Execute", ex);
            }
        }

        /// <summary>
        /// 返回值bool：是否继续执行该job
        /// </summary>
        /// <param name="log"></param>
        /// <returns></returns>
        private void ExecuteProd(SVR_TIM_EVENT_LOG log)
        {
            try
            {
                var db = gzswDB.GetInstance();
                log.PAR_INFO = "";
                log.RUN_TIME = DateTime.Now;

                db.Execute("exec PRO_MON_COMPLETE_OVERTIME_CNT");
            }
            catch (Exception ex)
            {
                log.ERROR_INFO = ex.Message;
                log.RUN_STATE = 2;
            }
        }
        
        #endregion
    }
}
