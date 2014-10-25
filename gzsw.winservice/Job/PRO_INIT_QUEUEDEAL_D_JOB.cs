using gzsw.model;
using gzsw.util;
using PetaPoco;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace gzsw.winservice.Job
{
    [DisallowConcurrentExecution]
    public class PRO_INIT_QUEUEDEAL_D_JOB : IJob
    {
        #region IJob 成员

        public void Execute(IJobExecutionContext context)
        {
            try
            {
                var db = new Database();
                //检查PRO_CHK_STAFFCHK_EVENT_D和PRO_REPORT_STAT_D作业当天是否已经执行
                //如果没有执行，PRO_INIT_QUEUEDEAL_D作业暂停1分钟，直到执行为止
                while (true)
                {
                    var isrun = db.Fetch<int>(@"
                    select DATEDIFF(D,GETDATE(),MAX(t1.RUN_TIME)) isRun
                    from SVR_TIM_EVENT_LOG t1
                    where t1.EVENT_GUID=(
                        select EVENT_GUID from SVR_TIM_EVENT where PROGRAM_METHOD='PRO_CHK_STAFFCHK_EVENT_D'
                    )
                    union all
                    select DATEDIFF(D,GETDATE(),MAX(t1.RUN_TIME)) isRun
                    from SVR_TIM_EVENT_LOG t1
                    where t1.EVENT_GUID=(
                        select EVENT_GUID from SVR_TIM_EVENT where PROGRAM_METHOD='PRO_REPORT_STAT_D'
                    )
                    ");
                    if (isrun.All(obj=>obj==0))//当天都已经执行
                    {
                         LogHelper.WriteLog("作业PRO_INIT_QUEUEDEAL_D_JOB从PRO_CHK_STAFFCHK_EVENT_D和PRO_REPORT_STAT_D暂停中恢复。");
                        context.Scheduler.ResumeJob(context.JobDetail.Key);
                        break;
                    }
                    LogHelper.WriteLog("作业PRO_INIT_QUEUEDEAL_D_JOB因为PRO_CHK_STAFFCHK_EVENT_D和PRO_REPORT_STAT_D暂停1分钟。");
                    context.Scheduler.PauseJob(context.JobDetail.Key);
                    Thread.Sleep(60 * 1000);
                }                
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("PRO_INIT_QUEUEDEAL_D_JOB", ex);
                return;
            }
            try
            {
                LogHelper.WriteLog("PRO_INIT_QUEUEDEAL_D_JOB");
                var db = new Database();

                model.SVR_TIM_EVENT e = db.SingleOrDefault<model.SVR_TIM_EVENT>("select * from SVR_TIM_EVENT where PROGRAM_METHOD='PRO_INIT_QUEUEDEAL_D'");
                if (null != e)
                {
                    SVR_TIM_EVENT_LOG log = new SVR_TIM_EVENT_LOG();
                    log.EVENT_GUID = e.EVENT_GUID;
                    log.RUN_TIME = DateTime.Now;
                    log.RUN_STATE = 1;
                    log.PAR_INFO = "";
                    log.ERROR_INFO = "";
                    ExecuteProd(log);
                    db.Insert(log);//写日志
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("PRO_INIT_QUEUEDEAL_D_JOB.Execute", ex);
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
                var db = new Database();
                db.Execute("exec PRO_INIT_QUEUEDEAL_D");
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
