﻿using gzsw.model;
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
    public class PRO_STAT_STAFF_CHKSTAT_M_JOB : IJob
    {
        #region IJob 成员

        public void Execute(IJobExecutionContext context)
        {
            try
            {
                var db = new Database();
                //检查PRO_INIT_QUEUEDEAL_D作业当天是否已经执行
                //如果没有执行，PRO_STAT_STAFF_CHKSTAT_M作业暂停1分钟，直到执行为止
                while (true)
                {
                    var isrun = db.ExecuteScalar<int>(@"
                    select DATEDIFF(D,GETDATE(),MAX(t1.RUN_TIME)) isRun
                    from SVR_TIM_EVENT_LOG t1
                    where t1.EVENT_GUID=(
                        select EVENT_GUID from SVR_TIM_EVENT where PROGRAM_METHOD='PRO_INIT_QUEUEDEAL_D'
                    )
                    ");
                    if (isrun==0)//当天已经执行
                    {
                        LogHelper.WriteLog("作业PRO_STAT_STAFF_CHKSTAT_M从PRO_INIT_QUEUEDEAL_D暂停中恢复。");
                        context.Scheduler.ResumeJob(context.JobDetail.Key);
                        break;
                    }
                    LogHelper.WriteLog("作业PRO_STAT_STAFF_CHKSTAT_M因为PRO_INIT_QUEUEDEAL_D暂停1分钟。");
                    context.Scheduler.PauseJob(context.JobDetail.Key);
                    Thread.Sleep(60 * 1000);
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("PRO_STAT_STAFF_CHKSTAT_M_JOB", ex);
                return;
            }
            try
            {
                LogHelper.WriteLog("PRO_STAT_STAFF_CHKSTAT_M_JOB");
                var db = new Database();

                model.SVR_TIM_EVENT e = db.SingleOrDefault<model.SVR_TIM_EVENT>("select * from SVR_TIM_EVENT where PROGRAM_METHOD='PRO_STAT_STAFF_CHKSTAT_M'");
                if (null != e)
                {
                    SVR_TIM_EVENT_LOG log = new SVR_TIM_EVENT_LOG();
                    log.EVENT_GUID = e.EVENT_GUID;
                    log.RUN_TIME = DateTime.Now;
                    log.RUN_STATE = 1;
                    log.PAR_INFO = "";
                    log.ERROR_INFO = "";     
                    while(true)
                    {
                        bool continu = ExecuteProd(log);
                        db.Insert(log);//写日志
                        if (!continu)
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("PRO_STAT_STAFF_CHKSTAT_M_JOB.Execute", ex);
            }
        }

        /// <summary>
        /// 返回值bool：是否继续执行该job
        /// </summary>
        /// <param name="log"></param>
        /// <returns></returns>
        private bool ExecuteProd(SVR_TIM_EVENT_LOG log)
        {
            try
            {
                var db = new Database();
                var parlist = db.Fetch<SVR_TIM_EVENT_PAR>("select * from SVR_TIM_EVENT_PAR where EVENT_GUID=@0 order by PAR_ORD asc", log.EVENT_GUID);

                log.PAR_INFO = string.Join(",", parlist.Select(obj => obj.PAR_VALUE));
                log.RUN_TIME = DateTime.Now;
                db.Execute("exec PRO_STAT_STAFF_CHKSTAT_M @0, @1", parlist.Select(obj => obj.PAR_VALUE).ToArray());
                var par1 = parlist.FirstOrDefault(obj => obj.PAR_ORD == 1);
                if (null != par1)
                {
                    par1.PAR_VALUE = par1.PAR_VALUE.Substring(0, 4) + "-" + par1.PAR_VALUE.Substring(4) + "-1";
                    var nextpar = DateTime.Parse(par1.PAR_VALUE).AddMonths(1);
                    par1.PAR_VALUE = nextpar.ToString("yyyyMM");
                    db.Save(par1);//更新参数
                    
                    if (nextpar <= new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1))
                        return true;//继续执行
                    return false;
                }
            }
            catch (Exception ex)
            {
                log.ERROR_INFO = ex.Message;
                log.RUN_STATE = 2;
            }
            return false;
        }

        #endregion
    }
}