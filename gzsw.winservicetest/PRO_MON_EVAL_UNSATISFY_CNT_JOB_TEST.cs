﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using gzsw.winservice.Job;
using Quartz;
using Quartz.Impl;

namespace gzsw.winservicetest
{
    [TestClass]
    public class PRO_MON_EVAL_UNSATISFY_CNT_JOB_TEST
    {
        [TestMethod]
        public void PRO_MON_EVAL_UNSATISFY_CNT_JOB()
        {
            //IJobExecutionContext ctx = new JobExecutionContextImpl(null, TestUtil.NewMinimalRecoveringTriggerFiredBundle(), null);
            //ctx.MergedJobDataMap[SchedulerConstants.FailedJobOriginalTriggerName] = "originalTriggerName";
            //ctx.MergedJobDataMap[SchedulerConstants.FailedJobOriginalTriggerGroup] = "originalTriggerGroup";
            //var recoveringTriggerKey = ctx.RecoveringTriggerKey;

            new PRO_MON_EVAL_UNSATISFY_CNT_JOB().Execute(null);
        }
    }
}
