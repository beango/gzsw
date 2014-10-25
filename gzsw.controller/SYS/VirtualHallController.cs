using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Razor.Generator;
using gzsw.controller.MyAuth;
using gzsw.dal;
using gzsw.dal.dao;
using gzsw.model;
using gzsw.web.Areas.SYS.Models;

namespace gzsw.controller.SYS
{
    /// <summary>
    /// 虚拟大厅
    /// </summary>
    public class VirtualHallController : BaseController<SYS_HALL>
    {
        [Ninject.Inject]
        public IDao<MON_HALL_TAB_DEF> MonHallTabDefDao { get; set; }
        [Ninject.Inject]
        public IDao<CHK_COUNTER> chkCounterDao { get; set; }

        [UserAuth("SYS_VIRTUALHALL_VIW")]
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 配置虚拟大厅
        /// </summary>
        /// <param name="hallNo">虚拟大厅编号</param>
        /// <returns></returns>
        [UserAuth("SYS_VIRTUALHALL_EDT")]
        public ActionResult Edit(string hallNo)
        {
            var hallService = new SYS_HALL_DAL();
            var hall = hallService.GetHall(hallNo);
            return View(hall);
        }

        /// <summary>
        /// 获取配置的摄像和窗口
        /// </summary>
        /// <param name="hallNo"></param>
        /// <returns></returns>
        [UserAuth("SYS_VIRTUALHALL_EDT")]
        public ActionResult GetHallConfig(string hallNo)
        {
            var monHallService = new MON_HALL_DAL();
            var tabDefs = monHallService.GetHallTabDefs(hallNo);
            var cameraDefs = monHallService.GetHallCameraDefs(hallNo);

            var list = new List<HallConfig>();
            list.AddRange(tabDefs.Select(m => new HallTabConfig()
                                            {
                                                HallNo=m.HALL_NO,
                                                IconUrl=m.ICON_URL,
                                                Id=m.COUNTER_ID,
                                                Type=1,
                                                X=m.HORIZ_SIGN,
                                                Y=m.VERTI_SIGN
                                            }));

            list.AddRange(cameraDefs.Select(m => new HallCameraConfig()
                                               {
                                                   HallNo = m.HALL_NO,
                                                   IconUrl = m.ICON_URL,
                                                   Id = m.SEQ,
                                                   Type = 0,
                                                   X=m.HORIZ_SIGN,
                                                   Y=m.VERTI_SIGN,
                                                   DirType = m.DIR_TYP == null ? 0 : (int)m.DIR_TYP.Value,
                                                   CgiProtpcpl=m.CGI_PROTOCOL,
                                                   ChannelId=m.CHANNEL_ID,
                                                   HttpProtocol=m.HTTP_PROTOCOL,
                                                   Ip=m.IP_ADDRESS,
                                                   Iport=m.IPORT,
                                                   MonCounter=m.MON_COUNTER,
                                                   Password=m.USER_PASSWORD,
                                                   RtspPort=m.RTSP_PORT,
                                                   StringType=m.STRING_TYP,
                                                   UserName=m.USER_NAME,
                                                   ZeroChannelInd=m.ZERO_CHANNEL_IND,
                                                   CameraType=m.CAMERA_TYP,
                                                   MonShowing=m.MON_SHOW_IND
                                               }));

            return Json(list, JsonRequestBehavior.AllowGet);
        }

        [UserAuth("SYS_VIRTUALHALL_EDT")]
        public ActionResult ValidateCounterId(int CounterId, string HallNo)
        {
            var item = chkCounterDao.GetEntity("HALL_NO", HallNo, "COUNTER_ID", CounterId);
            return Json(item == null, JsonRequestBehavior.AllowGet);
        }

        [UserAuth("SYS_VIRTUALHALL_EDT")]
        [HttpPost]
        public ActionResult Save(HallModel model)
        {
            try
            {
                var hallService = new SYS_HALL_DAL();
                var monHallService = new MON_HALL_DAL();

                hallService.UpdatePictUrl(model.HallNo, model.ImageUrl);


                #region 保存详细数据

                if (model.RemoveHallConfigs != null)
                {
                    foreach (var item in model.RemoveHallConfigs)
                    {
                        if (item.Type == 1)
                        {
                            monHallService.RemoveHallTabDef(item.HallNo, item.Id);
                        }
                        else
                        {
                            monHallService.RemoveHallCameraDef(item.Id);
                        }
                    }
                }

                if (model.HallTabConfigs != null)
                {
                    monHallService.DeleteHallTabDef(model.HallNo);
                    foreach (var tab in model.HallTabConfigs.Select(item => new MON_HALL_TAB_DEF()
                                                                            {
                                                                                COUNTER_ID = item.Id,
                                                                                HALL_NO = item.HallNo,
                                                                                HORIZ_SIGN = item.X,
                                                                                VERTI_SIGN = item.Y,
                                                                                ICON_URL = item.IconUrl
                                                                            }))
                    {
                        MonHallTabDefDao.AddObject(tab);
                    }
                }
                if (model.HallCameraConfigs != null)
                {
                    foreach (var camera in model.HallCameraConfigs.Select(item => new MON_HALL_CAMERA_DEF()
                                                                                  {
                                                                                      DIR_TYP = (byte?)item.DirType,
                                                                                      SEQ = item.Id,
                                                                                      HALL_NO = item.HallNo,
                                                                                      HORIZ_SIGN = item.X,
                                                                                      VERTI_SIGN = item.Y,
                                                                                      ICON_URL = item.IconUrl,
                                                                                      CGI_PROTOCOL=item.CgiProtpcpl,
                                                                                      CHANNEL_ID=item.ChannelId,
                                                                                      HTTP_PROTOCOL=item.HttpProtocol,
                                                                                      IP_ADDRESS=item.Ip,
                                                                                      IPORT=item.Iport,
                                                                                      MON_COUNTER=item.MonCounter,
                                                                                      RTSP_PORT=item.RtspPort,
                                                                                      STRING_TYP=item.StringType,
                                                                                      USER_NAME=item.UserName,
                                                                                      USER_PASSWORD=item.Password,
                                                                                      ZERO_CHANNEL_IND=item.ZeroChannelInd,
                                                                                      CAMERA_TYP=item.CameraType,
                                                                                      MON_SHOW_IND = item.MonShowing
                                                                                  }))
                    {
                        monHallService.SaveHallCameraDef(camera);
                    }
                }

                #endregion

                return Json(new {success = 0, JsonRequestBehavior.AllowGet});
            }
            catch
            {
                return Json(new { success = 1, JsonRequestBehavior.AllowGet });
            }
        }

        public ActionResult GetHallTree(string hallName)
        {
            var organizeList = new SYS_ORGANIZE_DAL().GetListForUserId(UserState.UserID);

            var list = organizeList.Select(m => new ZtreeNodeItem()
                                                {
                                                    id=m.ORG_ID,
                                                    name=m.ORG_NAM,
                                                    isParent = m.PAR_ORG_ID==null,
                                                    open = m.PAR_ORG_ID == null,
                                                    pId=m.PAR_ORG_ID,
                                                    enable = m.ORG_LEVEL==4,
                                                    highlight = (!string.IsNullOrEmpty(hallName) && m.ORG_NAM.Contains(hallName)),
                                                });
            return Json(list, JsonRequestBehavior.AllowGet);
        }
    }
}
