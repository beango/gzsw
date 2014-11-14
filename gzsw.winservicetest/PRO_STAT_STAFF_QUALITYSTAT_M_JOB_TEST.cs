using gzsw.winservice.Job;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gzsw.winservicetest
{
    [TestClass]
    public class PRO_STAT_STAFF_QUALITYSTAT_M_JOB_TEST
    {
        [TestMethod]
        public void PRO_STAT_STAFF_QUALITYSTAT_M_JOB()
        {
            //IJobExecutionContext ctx = new JobExecutionContextImpl(null, TestUtil.NewMinimalRecoveringTriggerFiredBundle(), null);
            //ctx.MergedJobDataMap[SchedulerConstants.FailedJobOriginalTriggerName] = "originalTriggerName";
            //ctx.MergedJobDataMap[SchedulerConstants.FailedJobOriginalTriggerGroup] = "originalTriggerGroup";
            //var recoveringTriggerKey = ctx.RecoveringTriggerKey;

            new PRO_STAT_STAFF_QUALITYSTAT_M_JOB().Execute(null);
        }
    }
}
