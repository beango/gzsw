using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace gzsw.web.Areas.SYS.Models
{

    public class HallModel
    {
        public string HallNo { get; set; }

        public string ImageUrl { get; set; }

        /// <summary>
        /// 添加的窗口配置
        /// </summary>
        public List<HallTabConfig> HallTabConfigs { get; set; }

        /// <summary>
        /// 增加的摄像配置
        /// </summary>
        public List<HallCameraConfig> HallCameraConfigs { get; set; }

        /// <summary>
        /// 移除的配置项
        /// </summary>
        public List<HallConfig> RemoveHallConfigs { get; set; } 
    }

    /// <summary>
    /// 服务的配置
    /// </summary>
    public class HallConfig
    {
        public int Id { get; set; }

        public string HallNo { get; set; }

        public string X { get; set; }

        public string Y { get; set; }

        public string IconUrl { get; set; }

        /// <summary>
        /// 1窗口，0摄像
        /// </summary>
        public int Type { get; set; }
    }

    /// <summary>
    /// 窗口配置类
    /// </summary>
    public class HallTabConfig : HallConfig
    {
        /// <summary>
        /// 摄像的ID，只用于读数据,
        /// 用于在点击窗口时，进行摄像的播放
        /// </summary>
        public HallCameraConfig CameraConfig { get; set; }
    }

    /// <summary>
    /// 摄像配置类
    /// </summary>
    public class HallCameraConfig : HallConfig
    {
        public string CameraName { get; set; }
        /// <summary>
        /// 方向类型
        /// </summary>
        public int DirType { get; set; }

        /// <summary>
        /// 摄像头类型，1云台，2半球
        /// </summary>
        public byte CameraType { get; set; }

        public string Ip { get; set; }

        /// <summary>
        /// http协议，1表示http协议 2表示https协议
        /// </summary>
        public int HttpProtocol { get; set; }

        public int Iport { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        /// <summary>
        /// 1表示ISAPI,2表示PSIA
        /// </summary>
        public int? CgiProtpcpl { get; set; }

        /// <summary>
        /// 1主码流，2子码流
        /// </summary>
        public int? StringType { get; set; }

        /// <summary>
        /// 播放通道号
        /// </summary>
        public string ChannelId { get; set; }

        /// <summary>
        /// 是否播放零通道
        /// </summary>
        public bool? ZeroChannelInd { get; set; }

        /// <summary>
        /// RTSP端口号
        /// </summary>
        public int? RtspPort { get; set; }

        /// <summary>
        /// 监控的窗口号
        /// </summary>
        public string MonCounter { get; set; }

        /// <summary>
        /// 摄像是否在页面显示
        /// </summary>
        public bool MonShowing { get; set; }
    }
}