﻿using gzsw.model;
using gzsw.util;
using PetaPoco;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gzsw.winservice.Job
{
    [DisallowConcurrentExecution]
    public class PRO_REPORT_STAT_D_JOB : IJob
    {
        #region IJob 成员

        public void Execute(IJobExecutionContext context)
        {
            try
            {
                LogHelper.WriteLog("PRO_REPORT_STAT_D_JOB");
                var db = new Database();

                model.SVR_TIM_EVENT e = db.SingleOrDefault<model.SVR_TIM_EVENT>("select * from SVR_TIM_EVENT where PROGRAM_METHOD='PRO_REPORT_STAT_D'");
                if (null != e)
                {
                    SVR_TIM_EVENT_LOG log = new SVR_TIM_EVENT_LOG();
                    log.EVENT_GUID = e.EVENT_GUID;
                    log.RUN_TIME = DateTime.Now;
                    log.RUN_STATE = 1;
                    log.PAR_INFO = "";
                    log.ERROR_INFO = "";
                    while (true)
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
                LogHelper.ErrorLog("PRO_REPORT_STAT_D_JOB.Execute", ex);
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
                db.Execute("exec PRO_REPORT_STAT_D @0, @1", parlist.Select(obj => obj.PAR_VALUE).ToArray());
                var par1 = parlist.FirstOrDefault(obj => obj.PAR_ORD == 1);
                if (null != par1)
                {
                    var nextpar = DateTime.Parse(par1.PAR_VALUE).AddDays(1);
                    par1.PAR_VALUE = nextpar.ToString("yyyy-MM-dd");
                    db.Save(par1);//更新参数

                    if (nextpar <= DateTime.Today)
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
