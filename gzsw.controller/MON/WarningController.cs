using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Windows.Forms.VisualStyles;
using gzsw.controller.MON.Models;
using gzsw.controller.MyAuth;
using gzsw.dal;
using gzsw.dal.dao;
using gzsw.model;
using gzsw.util.Extensions;
using gzsw.model.dto;

namespace gzsw.controller.MON
{
    /// <summary>
    /// 监控预警
    /// </summary>
    public class WarningController:BaseController
    {
        [Ninject.Inject]
        public IDao<SYS_HALL> DaoHall { get; set; }

        [Ninject.Inject]
        public IDao<SYS_COUNTER> counterDao { get; set; }

        /// <summary>
        /// 监控预警主页面
        /// </summary>
        /// <param name="orgId"></param>
        /// <param name="hallNo"></param>
        /// <returns></returns>
        [UserAuth("MON_Warning_VIW")]
        public ActionResult Index(string orgId, string hallNo)
        {
            SYS_HALL hall = null;
            if (!string.IsNullOrEmpty(orgId))
            {
                hall = DaoHall.GetEntity("ORG_ID", orgId);
                hallNo = hall != null ? hall.HALL_NO : null;
            }
            if (hall==null)
            {
                hall = DaoHall.GetEntity("HALL_NO", hallNo);
            }
            ViewBag.Name = hall.HALL_NAM;

            return View((object)hallNo);
        }

        /// <summary>
        /// 监控预警内容信息
        /// </summary>
        /// <param name="hallNo"></param>
        /// <returns></returns>
        [UserAuth("MON_Warning_VIW")]
        public ActionResult WarningContent(string hallNo)
        {
            var list = Warning_DAL.GetVirtualMon(hallNo);


            #region 管理情况
            var mang = new ManagementInfoModel();
            var total = getVirtualMonDto(list, 1, 1).VALUE;
            var online = getVirtualMonDto(list, 1, 2).VALUE;
            mang.TotalNum = total.ToInt();
            mang.OnlineNum = online.ToInt();
            mang.OpenRate = total == 0 ? 0 : online / total;
            mang.HandleNum = getVirtualMonDto(list, 1, 3).VALUE.ToInt();
            mang.HandleRate = total == 0 ? 0 : getVirtualMonDto(list, 1, 3).VALUE / total;
            mang.PoliceNum = getVirtualMonDto(list, 1,4).VALUE.ToInt();
            mang.ComplaintsNum = getVirtualMonDto(list, 1, 5).VALUE.ToInt();
            mang.SecondaryHandleNum = getVirtualMonDto(list, 1, 6).VALUE.ToInt();
            mang.CityNum = getVirtualMonDto(list, 1, 7).VALUE.ToInt();

            ViewBag.Management = mang;
            #endregion

            #region 大厅排队情况

            var que = new QueuingSituationModel();

            que.Total = getVirtualMonDto(list, 2, 1).VALUE.ToInt();
            que.WaitingNum = getVirtualMonDto(list, 2, 2).VALUE.ToInt();
            //等候达标率=(正在等候人次-等候超时人数)/正在等候人次
            que.WaitingRate = getVirtualMonDto(list, 2, 2).VALUE == 0 ? 0 : (getVirtualMonDto(list, 2, 2).VALUE - getVirtualMonDto(list, 2, 10).VALUE) / getVirtualMonDto(list, 2, 2).VALUE;
            //窗口饱和度= 当前等候人数 / 在线窗口数
            var p = mang.OnlineNum == 0 ? 0 : getVirtualMonDto(list, 2, 2).VALUE / mang.OnlineNum;
            que.WindowPate = SaturationEnum.未饱和;
            if (p > getVirtualMonDto(list, 2, 5).VALUE)
            {
                que.WindowPate = SaturationEnum.极度饱和;
            }
            else if (p > getVirtualMonDto(list, 2, 4).VALUE)
            {
                que.WindowPate = SaturationEnum.非常饱和;
            }
            else if (p > getVirtualMonDto(list, 2, 3).VALUE)
            {
                que.WindowPate = SaturationEnum.饱和;
            }
            //大厅饱和度=总出票数/大厅容量
            var hp = getVirtualMonDto(list, 2, 6).VALUE == 0 ? 0 : getVirtualMonDto(list, 2, 1).VALUE / getVirtualMonDto(list, 2, 6).VALUE;
            que.HallPate = SaturationEnum.未饱和;
            if (hp > getVirtualMonDto(list, 2, 9).VALUE)
            {
                que.HallPate = SaturationEnum.极度饱和;
            }
            else if (hp > getVirtualMonDto(list, 2, 8).VALUE)
            {
                que.HallPate = SaturationEnum.非常饱和;
            }
            else if (hp > getVirtualMonDto(list, 2, 7).VALUE)
            {
                que.HallPate = SaturationEnum.饱和;
            }

            //等候超时率=等候超时人数/正在等候人次
            que.WaitingTimeoutPate = getVirtualMonDto(list, 2, 2).VALUE == 0 ? 0 : getVirtualMonDto(list, 2, 10).VALUE / getVirtualMonDto(list, 2, 2).VALUE;
            //等待超时人次预警
            que.WaitingTimeout = AlarmLevelEnum.正常;
            var a = getVirtualMonDto(list, 2, 13).VALUE;
            if (que.WaitingTimeoutPate > a)
            {
                que.WaitingTimeout = AlarmLevelEnum.红色预警;
            }
            else if (que.WaitingTimeoutPate > getVirtualMonDto(list, 2, 12).VALUE)
            {
                que.WaitingTimeout = AlarmLevelEnum.橙色预警;
            }
            else if (que.WaitingTimeoutPate > getVirtualMonDto(list, 2, 11).VALUE)
            {
                que.WaitingTimeout = AlarmLevelEnum.黄色预警;
            }

            //平均等候时间=总等候时间/正在等候人次
            que.AverageTime = (getVirtualMonDto(list, 2, 2).VALUE == 0 ? 0 : getVirtualMonDto(list, 2, 14).VALUE / getVirtualMonDto(list, 2, 2).VALUE).ToInt();

            ViewBag.QueuingSituation = que;
            #endregion

            #region 服务情况

            var service = new ServiceInfoModel();
            service.AcceptedNum = getVirtualMonDto(list, 3, 1).VALUE.ToInt();
            service.OnServiceNum = getVirtualMonDto(list, 3, 2).VALUE.ToInt();
            //超时办结率= 超时业务笔数/已受理业务笔数
            service.TimeoutRate = getVirtualMonDto(list, 3, 2).VALUE == 0 ? 0 : getVirtualMonDto(list, 3, 3).VALUE / getVirtualMonDto(list, 3, 2).VALUE;
            //超时办结率= 超时业务笔数/已受理业务笔数
            service.OnTimeRate = getVirtualMonDto(list, 3, 2).VALUE == 0 ? 0 : (getVirtualMonDto(list, 3, 2).VALUE - getVirtualMonDto(list, 3, 3).VALUE) / getVirtualMonDto(list, 3, 2).VALUE;
            
            service.TimeoutServiceWarning = AlarmLevelEnum.正常;
            if (getVirtualMonDto(list, 3, 3).VALUE > getVirtualMonDto(list, 3, 11).VALUE)
            {
                service.TimeoutServiceWarning = AlarmLevelEnum.红色预警;
            }
            else if (getVirtualMonDto(list, 3, 3).VALUE > getVirtualMonDto(list, 3, 10).VALUE)
            {
                service.TimeoutServiceWarning = AlarmLevelEnum.橙色预警;
            }
            else if (getVirtualMonDto(list, 3, 3).VALUE > getVirtualMonDto(list, 3, 9).VALUE)
            {
                service.TimeoutServiceWarning = AlarmLevelEnum.黄色预警;
            }

            service.LeaveNum = getVirtualMonDto(list, 3, 12).VALUE.ToInt();
            //总呼叫号数/弃号数
            service.LeaveRate = getVirtualMonDto(list, 2, 1).VALUE == 0 ? 0 : getVirtualMonDto(list, 3, 12).VALUE / getVirtualMonDto(list, 2, 1).VALUE;
            service.LeaveWarning = AlarmLevelEnum.正常;
            if (service.LeaveRate > getVirtualMonDto(list, 3, 16).VALUE)
            {
                service.LeaveWarning = AlarmLevelEnum.红色预警;
            }
            else if (service.LeaveRate > getVirtualMonDto(list, 3, 15).VALUE)
            {
                service.LeaveWarning = AlarmLevelEnum.橙色预警;
            }
            else if (service.LeaveRate > getVirtualMonDto(list, 3, 14).VALUE)
            {
                service.LeaveWarning = AlarmLevelEnum.黄色预警;
            }

            service.OneMoreServiceNum = getVirtualMonDto(list, 3, 17).VALUE.ToInt();

            ViewBag.Service = service;
            #endregion

            #region 纳税人评价情况

            var ev = new EvaluationInfoModel();
            ev.Total = getVirtualMonDto(list, 4, 1).VALUE.ToInt();
            ev.UnTotal = getVirtualMonDto(list, 4, 2).VALUE.ToInt();
            ev.PoorNum = getVirtualMonDto(list, 4, 3).VALUE.ToInt();
            //未评价率=未评价总数/(评价总数+未评价总数)
            var tt = (getVirtualMonDto(list, 4, 2).VALUE + getVirtualMonDto(list, 4, 1).VALUE);
            ev.UnEvaluationRate = tt == 0 ? 0 : getVirtualMonDto(list, 4, 3).VALUE/tt;
            //满意率=（评价总数-差评数）/评价总数
            ev.SatisfactionRate = getVirtualMonDto(list, 4, 1).VALUE == 0
                ? 0
                : (getVirtualMonDto(list, 4, 1).VALUE - getVirtualMonDto(list, 4, 3).VALUE)/
                  getVirtualMonDto(list, 4, 1).VALUE;
            //差评率=差评数/评价总数
            ev.BadRate = getVirtualMonDto(list, 4, 1).VALUE == 0
                ? 0
                : getVirtualMonDto(list, 4, 3).VALUE/getVirtualMonDto(list, 4, 1).VALUE;

            ev.BadWarning = AlarmLevelEnum.正常;

            if (getVirtualMonDto(list, 4, 3).VALUE > getVirtualMonDto(list, 4, 6).VALUE)
            {
                ev.BadWarning = AlarmLevelEnum.红色预警;
            }
            else if (getVirtualMonDto(list, 4, 3).VALUE > getVirtualMonDto(list, 4, 5).VALUE)
            {
                ev.BadWarning = AlarmLevelEnum.橙色预警;
            }
            else if (getVirtualMonDto(list, 4, 3).VALUE > getVirtualMonDto(list, 4, 4).VALUE)
            {
                ev.BadWarning = AlarmLevelEnum.黄色预警;
            }

            ViewBag.EvaluationSituation = ev;
            #endregion
            
            return View();
        }

        /// <summary>
        /// 预警窗口信息
        /// </summary>
        /// <param name="hallNo">服务厅Id</param>
        /// <param name="type">类型：0总窗口，1在线窗口，2正在办理业务窗口</param>
        /// <returns></returns>
        [UserAuth("MON_Warning_VIW")]
        public ActionResult TotalWindow(string hallNo,int type)
        {
            var list = CLI_COUNTERSTATE_DAL.GetCounterList(hallNo);
            if (type == 1)
            {
                list = list.Where(m => m.LOGIN_STATE != null && m.LOGIN_STATE == 1).ToList();
            }
            else if (type == 2)
            {
                var state = new int[] {2,3};
                list = list.Where(m => m.LOGIN_STATE != null && m.LOGIN_STATE == 1 && m.STATE != null && state.Contains(m.STATE.GetValueOrDefault())).ToList();
            }
            return View(list);
        }

        /// <summary>
        /// 已受理业务笔数
        /// </summary>
        /// <param name="orgId"></param>
        /// <returns></returns>
        public ActionResult OnAcceptService(string hallNo, string number, int? counter,
            string staffId, string nsrsbm, int? tickettype, string hywDetailserialid, int? isfinished, string transcodeid,
            int isSenior = 0, int pageIndex = 1, int pageSize = 20)
        {
            var beginTime = DateTime.Now;
            var endTime =  DateTime.Now;

            ViewBag.HallNo = hallNo;
            ViewBag.Number = number;
            ViewBag.Counter = counter;
            ViewBag.StaffId = staffId;
            ViewBag.HYW_DETAILSERIALID = hywDetailserialid;
            ViewBag.Nsrsbm = nsrsbm;
            ViewBag.IsSenior = isSenior;

            var list = SYS_YWHIST_DAL.GetMergeList(hallNo, beginTime, endTime, number, counter, staffId, nsrsbm,
                tickettype, hywDetailserialid, isfinished, pageIndex, pageSize);
            return View(list);
        }

        /// <summary>
        /// 一票多业务号数
        /// </summary>
        /// <returns></returns>
        public ActionResult OneMoreService(string hallNo, string number, int? counter,
            string staffId, string nsrsbm, int? isfinished,string  transcodeId,
            int isSenior = 0, int pageIndex = 1, int pageSize = 20)
        {
            var beginTime = DateTime.Now;
            var endTime = DateTime.Now;
            ViewBag.Number = number;
            ViewBag.Counter = counter;
            ViewBag.StaffId = staffId;
            ViewBag.Nsrsbm = nsrsbm;
            ViewBag.IsSenior = isSenior;
            ViewBag.HallNo = hallNo;
            ViewBag.TRANSCODEID = transcodeId;

            var list = SYS_CURRQUEUEHIST_DAL.GetMergeList(hallNo, beginTime, endTime, number, counter, staffId, nsrsbm,
                null, 3, isfinished, pageIndex, pageSize, true, transcodeId);

            return View(list);
        }

        private Virtual_Mon_Dto getVirtualMonDto(IList<Virtual_Mon_Dto> list, int table, int listId)
        {
            return list.FirstOrDefault(m => m.TABLELIST_ID == table && m.LIST_ID == listId);
        }
    }
}
