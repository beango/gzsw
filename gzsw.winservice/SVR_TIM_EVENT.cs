using Common.Logging;
using gzsw.dal;
using gzsw.util;
using gzsw.winservice.Job;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.Matchers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace gzsw.winservice
{
    partial class SVR_TIM_EVENT : ServiceBase
    {
        private IScheduler scheduler;

        public SVR_TIM_EVENT()
        {
            InitializeComponent();

            try
            {
                ISchedulerFactory schedulerFactory = new StdSchedulerFactory();
                scheduler = schedulerFactory.GetScheduler();
                #region group1
                RegListener<PRO_REPORT_STAT_D_JOB, PRO_STAT_STAFF_BUSI_TOT_D_JOB>();
                RegListener<PRO_STAT_STAFF_BUSI_TOT_D_JOB, PRO_STAT_STAFF_QUEUE_BUSI_D_JOB>();
                RegListener<PRO_STAT_STAFF_QUEUE_BUSI_D_JOB, PRO_STAT_STAFF_LARGE_BUSI_D_JOB>();
                RegListener<PRO_STAT_STAFF_LARGE_BUSI_D_JOB, PRO_STAT_TAXPAYER_BEHAV_STAT_D_JOB>();
                RegListener<PRO_STAT_TAXPAYER_BEHAV_STAT_D_JOB, PRO_CHK_STAFFCHK_EVENT_D_JOB>();
				#endregion

                #region group2
                RegListener<PRO_CHK_STAFFCHK_EVENT_D_JOB, PRO_INIT_QUEUEDEAL_D_JOB>();
				#endregion

                #region group3
                RegListener<PRO_INIT_QUEUEDEAL_D_JOB, PRO_STAT_STAFF_SVRSTAT_M_JOB>();//16-4
                RegListener<PRO_STAT_STAFF_SVRSTAT_M_JOB, PRO_STAT_STAFF_EVALSTAT_M_JOB>();//4-6
                RegListener<PRO_STAT_STAFF_EVALSTAT_M_JOB, PRO_STAT_STAFF_QUALITYSTAT_M_JOB>();//6-5
                RegListener<PRO_STAT_STAFF_QUALITYSTAT_M_JOB, PRO_STAT_STAFF_CHKSTAT_M_JOB>();//5-3
				#endregion

				#region group4
                RegListener<PRO_STAT_STAFF_CHKSTAT_M_JOB, PRO_CHK_STAFF_COMPRE_EVAL_M_JOB>();//3-19
				RegListener<PRO_CHK_STAFF_COMPRE_EVAL_M_JOB, PRO_CHK_HALL_STAT_M_JOB>();
                #endregion
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("SVR_TIM_EVENT", ex);
            }
        }

        private void RegListener<From, To>()
        {
            IJobListener listener5 = new JOBListener<From, To>();
            JobKey key5 = new JobKey(typeof(From).Name);
            IMatcher<JobKey> matcher5 = KeyMatcher<JobKey>.KeyEquals(key5);
            scheduler.ListenerManager.AddJobListener(listener5, matcher5);
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                if (scheduler != null)
                {
                    scheduler.Start();
                }

                LogHelper.WriteLog("Quartz服务启动。");
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("OnStart", ex);
            }
        }

        protected override void OnStop()
        {
            try
            {
                if (scheduler != null)
                    scheduler.Shutdown(false);
                LogHelper.WriteLog("Quartz服务成功终止");
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("OnStop", ex);
            }
        }
    }

    public class JOBListener<From, To> : IJobListener
    {
        /// <summary>
        /// Get the name of the <see cref="IJobListener" />.
        /// </summary>
        public string Name
        {
            get
            {
                return typeof(From).Name + "Listener";
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
            LogHelper.WriteLog(Name + "，开始触发事件!");
            #region 16。PRO_INIT_QUEUEDEAL_D，迁移库的服务
            IJobDetail job1 = JobBuilder.Create(typeof(To))
                .WithIdentity(typeof(To).Name)
                .Build();
            ITrigger trigger1 = TriggerBuilder.Create()
                .WithIdentity(typeof(To).Name + "Trigger")
                .StartNow()
                .Build();
            try
            {
                context.Scheduler.ScheduleJob(job1, trigger1);
            }
            catch (SchedulerException e)
            {
                LogHelper.WriteLog("Unable to schedule " + typeof(From).Name + "!");
            }
            #endregion
        }
    }
}
