using Common.Logging;
using gzsw.dal;
using gzsw.util;
using Quartz;
using Quartz.Impl;
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

        //[Ninject.Inject]
        //public IDao<gzsw.model.SVR_TIM_EVENT> DaoTimEvent { get; set; }

        public SVR_TIM_EVENT()
        {
            InitializeComponent();

            try
            {
                ISchedulerFactory schedulerFactory = new StdSchedulerFactory();
                scheduler = schedulerFactory.GetScheduler();
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("SVR_TIM_EVENT", ex);
            }
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                if (scheduler != null)
                    scheduler.Start();
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
}
