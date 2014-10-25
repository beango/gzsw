using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gzsw.web.Areas.SYS.Models;

namespace gzsw.controller.MON
{
    /// <summary>
    /// 虚拟大厅
    /// </summary>
    public class VirtualHall
    {
        public string HallNo { get; set; }

        public string HallName { get; set; }

        public string ImageUrl { get; set; }

        public List<HallTabConfig> HallTabConfigs { get; set; }

        public List<HallCameraConfig> HallCameraConfigs { get; set; }
    }
}
