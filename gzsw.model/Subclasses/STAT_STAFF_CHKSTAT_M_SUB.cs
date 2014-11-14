using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PetaPoco;

namespace gzsw.model.Subclasses
{
    /// <summary>
    /// 员工考勤统计月表
    /// </summary>
    public class STAT_STAFF_CHKSTAT_M_SUB : STAT_STAFF_CHKSTAT_M
    {
        [ResultColumn]
        public string STAFF_NAM { get; set; }
    }
}
