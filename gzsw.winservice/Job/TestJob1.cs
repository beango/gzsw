using System;
using Quartz;
using System.Threading;
using gzsw.util;

namespace gzsw.winservice
{
    [DisallowConcurrentExecution]
    public class TestJob1 : IJob
    {
        #region IJob 成员

        public void Execute(IJobExecutionContext context)
        {
            try
            {
                context.Scheduler.PauseJob(context.JobDetail.Key);
                Thread.Sleep(3000);
                context.Scheduler.ResumeJob(context.JobDetail.Key);

                LogHelper.WriteLog("TestJob1：\r\n");
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("TestJob.Execute", ex);
            }
        }

        #endregion
    }
}
