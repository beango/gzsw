using System;
using Quartz;
using System.Threading;
using gzsw.util;

namespace gzsw.winservice
{
    [DisallowConcurrentExecution]
    public class TestJob2 : IJob
    {
        #region IJob 成员

        public void Execute(IJobExecutionContext context)
        {
            try
            {
                LogHelper.WriteLog("TestJob2：\r\n");
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("TestJob.Execute", ex);
            }
        }

        #endregion
    }
}
