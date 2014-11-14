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

    public class TestListener1 : IJobListener
    {
        /// <summary>
        /// Get the name of the <see cref="IJobListener" />.
        /// </summary>
        public string Name
        {
            get
            {
                return "test1_to_test2";
            }
        }

        public void JobToBeExecuted(IJobExecutionContext context)
        {
        }

        public void JobExecutionVetoed(IJobExecutionContext context)
        {
        }

        /// <summary>
        /// Called by the <see cref="IScheduler" /> after a <see cref="JobDetail" />
        /// has been executed, and be for the associated <see cref="Trigger" />'s
        /// <see cref="Trigger.Triggered" /> method has been called.
        /// </summary>
        public void JobWasExecuted(IJobExecutionContext context, JobExecutionException jobException)
        {
            LogHelper.WriteLog("JobWasExecuted!");
            // Simple job #2
            IJobDetail job2 = JobBuilder.Create<TestJob2>()
                .WithIdentity("TestJob2")
                .Build();
            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity("TestJob2Trigger")
                .StartNow()
                .Build();
            try
            {
                context.Scheduler.ScheduleJob(job2, trigger);
            }
            catch (SchedulerException e)
            {
                LogHelper.WriteLog("Unable to schedule TestJob2!");
            }
        }
    }
}
