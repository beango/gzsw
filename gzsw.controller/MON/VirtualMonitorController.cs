using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using gzsw.dal;
using gzsw.dal.dao;
using gzsw.model;
using gzsw.web.Areas.SYS.Models;

namespace gzsw.controller.MON
{
    /// <summary>
    /// 虚拟大厅监控
    /// </summary>
    public class VirtualMonitorController : BaseController
    {
        [Ninject.Inject]
        public IDao<SYS_HALL> DaoHall { get; set; }

        [Ninject.Inject]
        public IDao<SYS_STAFF> StaffDao { get; set; }
        [Ninject.Inject]
        public IDao<CLI_COUNTERSTATE> CounterStateDao { get; set; }

        [Ninject.Inject]
        public IDao<MON_HALL_CAMERA_DEF> hallCameraDefDao { get; set; }

        /// <summary>
        /// 监控页面
        /// </summary>
        /// <param name="orgId">组织结构ID</param>
        /// <param name="hallNo">服务厅编号</param>
        /// <returns></returns>
        public ActionResult Index(string orgId, string hallNo)
        {
            var hall = new SYS_HALL();
            if (!string.IsNullOrEmpty(orgId))
            {
                hall = DaoHall.GetEntity("ORG_ID", orgId);
            }
            else if (!string.IsNullOrEmpty(hallNo))
            {
                hall = DaoHall.GetEntity("HALL_NO", hallNo);
            }

            //if (hall == null)
            //{
            //    return HttpNotFound("Page Not Found");
            //}
            hallNo = hall.HALL_NO;
            var monHallService = new MON_HALL_DAL();
            var tabDefs = monHallService.GetHallTabDefs(hallNo);
            var cameraDefs = monHallService.GetHallCameraDefs(hallNo);
            var tabList= tabDefs.Select(m => new HallTabConfig()
            {
                HallNo = m.HALL_NO,
                IconUrl = m.ICON_URL,
                Id = m.COUNTER_ID,
                Type = 1,
                X = m.HORIZ_SIGN,
                Y = m.VERTI_SIGN
            }).ToList();

            var cameraList = cameraDefs.Where(m=>!m.MON_SHOW_IND).Select(m => new HallCameraConfig()
            {
                HallNo = m.HALL_NO,
                IconUrl = m.ICON_URL,
                Id = m.SEQ,
                Type = 0,
                X = m.HORIZ_SIGN,
                Y = m.VERTI_SIGN,
                DirType = m.DIR_TYP == null ? 0 : (int)m.DIR_TYP.Value,
                CgiProtpcpl = m.CGI_PROTOCOL,
                ChannelId = m.CHANNEL_ID,
                HttpProtocol = m.HTTP_PROTOCOL,
                Ip = m.IP_ADDRESS,
                Iport = m.IPORT,
                MonCounter = m.MON_COUNTER,
                Password = m.USER_PASSWORD,
                RtspPort = m.RTSP_PORT,
                StringType = m.STRING_TYP,
                UserName = m.USER_PASSWORD,
                ZeroChannelInd = m.ZERO_CHANNEL_IND,
                CameraType=m.CAMERA_TYP,
                MonShowing=m.MON_SHOW_IND
            }).ToList();

            var virtualHall = new VirtualHall()
                              {
                                  HallNo = hall.HALL_NO,
                                  HallName = hall.HALL_NAM,
                                  ImageUrl = hall.HALL_PICT_URL,
                                  HallTabConfigs = tabList,
                                  HallCameraConfigs = cameraList
                              };

            return View(virtualHall);
        }

        /// <summary>
        /// 用户信息
        /// </summary>
        /// <param name="staffId">员工ID</param>
        /// <param name="hallNo">服务厅编号</param>
        /// <param name="counterId">窗口Id</param>
        /// <returns></returns>
        public ActionResult TabPanel(string staffId, string hallNo, string counterId)
        {
            var staff = StaffDao.GetEntity("STAFF_ID", staffId);

            var counter = CounterStateDao.GetEntity("HALL_NO", hallNo, "COUNTER_ID", counterId);

            ViewBag.CounterState = counter;

            var listCamera= hallCameraDefDao.FindList("SEQ", "HALL_NO", hallNo);
            var camera = listCamera.FirstOrDefault(m => m.MON_COUNTER != null && m.MON_COUNTER.Split('|').Contains(counterId));
            if (camera != null)
            {
                ViewBag.CameraId = camera.SEQ;
                ViewBag.CameraType = camera.CAMERA_TYP;
            }
            else
            {
                ViewBag.CameraId = 0;
                ViewBag.CameraType = 0;
            }

            var counterStateService = new CLI_COUNTERSTATE_DAL();
            ViewBag.Number = counterStateService.GetTabNumber(hallNo, counterId);
            

            return View(staff);
        }

        /// <summary>
        /// 推送 消息
        /// </summary>
        /// <param name="hallNo"></param>
        /// <returns></returns>
        public JsonResult PushMessage(string hallNo)
        {
            var counterStateDal = new CLI_COUNTERSTATE_DAL();
            var counterStates = counterStateDal.GetListByHallNo(hallNo);

            var list = counterStateDal.GetOrganizeNumber(hallNo);
            var total = list.Sum(m => m.NUMBER);

            return Json(new { tabs = counterStates, business = list, total = total }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 播放视频
        /// </summary>
        /// <param name="seq">视频设置ID</param>
        /// <returns></returns>
        public ActionResult PlayVideo(int seq)
        {
            var item = hallCameraDefDao.GetEntity("SEQ", seq);
            return View(item);
        }
    }
}
