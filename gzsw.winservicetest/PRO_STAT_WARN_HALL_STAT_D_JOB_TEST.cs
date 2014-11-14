using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using gzsw.winservice.Job;
using Quartz;
using Quartz.Impl;

namespace gzsw.winservicetest
{
    [TestClass]
    public class PRO_STAT_WARN_HALL_STAT_D_JOB_TEST
    {
        [TestMethod]
        public void PRO_STAT_WARN_HALL_STAT_D_JOB()
        {
            //IJobExecutionContext ctx = new JobExecutionContextImpl(null, TestUtil.NewMinimalRecoveringTriggerFiredBundle(), null);
            //ctx.MergedJobDataMap[SchedulerConstants.FailedJobOriginalTriggerName] = "originalTriggerName";
            //ctx.MergedJobDataMap[SchedulerConstants.FailedJobOriginalTriggerGroup] = "originalTriggerGroup";
            //var recoveringTriggerKey = ctx.RecoveringTriggerKey;

            new PRO_STAT_WARN_HALL_STAT_D_JOB().Execute(null);
        }
    }
}
