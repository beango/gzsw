using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PetaPoco;

namespace gzsw.model.Subclasses
{
    /// <summary>
    /// 员工评价信息统计月表
    /// </summary>
    public class STAT_STAFF_EVALSTAT_M_SUB : STAT_STAFF_EVALSTAT_M
    {
        [ResultColumn]
        public string STAFF_NAM { get; set; }
    }
}
