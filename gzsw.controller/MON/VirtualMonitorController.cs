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
using gzsw.controller.MyAuth;

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
         [UserAuth("MON_VirtualMonitor_VIW")]
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
                Y = m.VERTI_SIGN,
                CameraConfig = getCameraConfig(cameraDefs, m.COUNTER_ID)
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

        private HallCameraConfig getCameraConfig(List<MON_HALL_CAMERA_DEF> cameraDefs, int counterId)
        {
            var camera = cameraDefs.FirstOrDefault(c => c.MON_COUNTER!=null && c.MON_COUNTER.Split('|').Contains(counterId.ToString()));
            if (camera == null)
            {
                return null;
            }
            return new HallCameraConfig()
                   {
                       Id=camera.SEQ,
                       CameraType=camera.CAMERA_TYP
                   };
        }

        /// <summary>
        /// 用户信息
        /// </summary>
        /// <param name="staffId">员工ID</param>
        /// <param name="hallNo">服务厅编号</param>
        /// <param name="counterId">窗口Id</param>
        /// <returns></returns>
        [UserAuth("MON_VirtualMonitor_VIW")]
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

        /// <summary>
        /// 播放视频 通过窗口ID
        /// </summary>
        /// <param name="hallNo">服务厅</param>
        /// <param name="counterId">窗口</param>
        /// <returns></returns>
        public ActionResult CounterPlayVideo(string hallNo,int counterId)
        {
            var listCamera = hallCameraDefDao.FindList("SEQ", "HALL_NO", hallNo);
            var camera = listCamera.FirstOrDefault(m => m.MON_COUNTER != null && m.MON_COUNTER.Split('|').Contains(counterId.ToString()));

            return View("PlayVideo",camera);
        }

        /// <summary>
        /// 获取窗口的视频信息
        /// </summary>
        /// <param name="hallNo"></param>
        /// <param name="counterId"></param>
        /// <returns></returns>
        public ActionResult GetCounterCamera(string hallNo, int counterId)
        {
            var listCamera = hallCameraDefDao.FindList("SEQ", "HALL_NO", hallNo);
            var camera = listCamera.FirstOrDefault(m => m.MON_COUNTER != null && m.MON_COUNTER.Split('|').Contains(counterId.ToString()));
            return Json(camera, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 回放
        /// </summary>
        /// <param name="hallNo">服务厅</param>
        /// <param name="counterId">ID</param>
        /// <param name="beginTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <returns></returns>
        public ActionResult PlayBack(string hallNo, int? counterId, DateTime? beginTime, DateTime? endTime)
        {
            var listCamera = hallCameraDefDao.FindList("SEQ", "HALL_NO", hallNo).ToList();

            if (beginTime == null)
            {
                beginTime = DateTime.Now.Date;
            }
            if (endTime == null)
            {
                endTime = DateTime.Now.Date.AddDays(1).AddSeconds(-1);
            }

            ViewBag.Seq = 0;
            if (counterId != null)
            {
                var camera = listCamera.FirstOrDefault(m => m.MON_COUNTER != null && m.MON_COUNTER.Split('|').Contains(counterId.ToString()));
                if (camera != null)
                {
                    ViewBag.Seq = camera.SEQ;
                }
            }

            ViewBag.BeginTime = beginTime;
            ViewBag.EndTime = endTime;

            return View(listCamera);
        }

        public ActionResult GetCameraInfo(int seq)
        {
            var camera = hallCameraDefDao.GetEntity("SEQ", seq);

            return Json(camera, JsonRequestBehavior.AllowGet);
        }
    }
}
